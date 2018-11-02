using ExtendedControl.Views.ControlWindow;
using NTW.Controls.Behaviours;
using NTW.Controls.ViewModels;
using NTW.Core;
using NTW.Data;
using NTW.Data.Context;
using NTW.Data.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using NTW.Attrebute;
using ExtendedControl.Views.Panels.Child;
using Virtualization.Calendar;
using Virtualization.Models;
using NTW;
using Info.Controls;

namespace ExtendedControl.ViewModels
{
    [ReportStat]
    public class TasksViewModel : ControlViewModel, IDropViewModel
    {
        #region Commands
        private Command _createNewTaskCommand;
        public Command CreateTaskCommand {
            get { return _createNewTaskCommand ?? (_createNewTaskCommand = new Command(obj => {
                this.State = TypeControl.Create;
                FrameworkElementOpacityBehaviour.SetIsShow((Window)obj, false);
                ((Window)obj).Opacity = 0;
                using (DBContext context = new DBContext(false))
                {
                    CurrentTask = Task.New();
                    CurrentTask.Creater = CurrentUser.ID;
                    CurrentTask.OpenDate = DateTime.Now;
                    CurrentTask.EndDate = CurrentTask.OpenDate.AddDays(1);
                    CurrentTask.IdProject = context.Projects.FirstOrDefault().ID;
                    CurrentTask.TypeExecution = 0;
                    this.SendPropertyChanged(nameof(CurrentTask));

                    TaskControlWindow createWindow = new TaskControlWindow();
                    HideButtonBehaviour.SetCommand(createWindow, CanceliWndowCommand);
                    WindowPositionBehaviour.SetWindowPosition(createWindow, WindowPosition.None);
                    WindowVisibilityBehaviour.SetIsDialogVisible(createWindow, true);

                    if (createWindow.DialogResult == true)
                    {
                        context.Tasks.AddObject(CurrentTask);
                        context.SaveChanges();

                        Tasks.Add(CurrentTask);

                        ActionCalendar((panel) => panel.Add(CurrentTask));

                        CurrentTask.Perfomers.Load();
                        // entery data journal
                        AdminViewModel.SendDataJournal(TypeMessage.CreateTask, this.CurrentUser, CurrentTask);
                    }
                    else
                        CurrentTask.Disposing(App.DBSettings.Connection.PathToResourceDB);

                    CurrentTask = null;
                }
                this.State = TypeControl.Normal;
                FrameworkElementOpacityBehaviour.SetIsShow((Window)obj, true);
            }, obj => obj is Window)); }
        }

        private Command _fineControlCommand;
        public Command FineControlCommand {
            get { return _fineControlCommand ?? (_fineControlCommand = new Command(obj => {
                Window w = (Window)obj;
                //делаем отметку с проверкой на ошибки для данного объекта задачи 
                if (CurrentTask.Errors.Count == 0) {
                    w.DialogResult = true;
                    WindowVisibilityBehaviour.SetIsDialogVisible(w, false);
                }
                else
                    foreach (var item in CurrentTask.Errors) {
                        WindowMessageBehaviour.SetMessage(w, App.GetString(item.Value), Colors.Maroon, Colors.White);
                    }
            }, obj => CurrentTask != null && CurrentTask.TaskFiles.Count(x => x.State == StateFile.Load) == 0 && obj is Window)); }
        }

        private Command _cancelWindowCommand;
        public Command CanceliWndowCommand {
            get {
                return _cancelWindowCommand ?? (_cancelWindowCommand = new Command(obj => {
                    WindowVisibilityBehaviour.SetIsDialogVisible((Window)obj, false);
                    WindowVisibilityBehaviour.SetIsVisible((Window)obj, false);
                }, obj => CurrentTask != null && CurrentTask.TaskFiles.Count(x => x.State == StateFile.Load) == 0 && obj is Window));
            }
        }

        private Command _removeTaskCommand;
        public Command RemoveTaskCommand {
            get { return _removeTaskCommand ?? (_removeTaskCommand = new Command(obj => {
                Task res = (Task)obj;
                using (DBContext context = new DBContext(true))
                {
                    //просто помечаем на удаление основной объект задачи - остальные не требуется
                    Task t = context.Tasks.FirstOrDefault(x => x.ID == res.ID);
                    if (t != null) {
                        t.IsDelete = true;

                        context.TaskComments.AddObject(res.AddSystemComment("delete", CurrentUser.ID));

                        context.SaveChanges();
                    }

                    Tasks.Remove(res);

                    // entery data journal
                    AdminViewModel.SendDataJournal(TypeMessage.DeleteTask, this.CurrentUser, t);

                    ActionCalendar((panel) => {
                        panel.Remove(res);
                    });

                    SelectedTask = null;
                }
            }, obj => obj is Task));
            }
        }

        private Command _editTaskCommand;
        public Command EditTaskCommand {
            get { return _editTaskCommand ?? (_editTaskCommand = new Command(obj => {

                Task temp = (Task)((Tuple<object, object>)obj).Item1;
                Window w = (Window)((Tuple<object, object>)obj).Item2;
                bool tempSelection = IsSelectedTask;
                IsSelectedTask = false;

                this.State = TypeControl.Edit;
                FrameworkElementOpacityBehaviour.SetIsShow(w, false);
                w.Opacity = 0;
                using (DBContext context = new DBContext(true)) {
                    CurrentTask = context.Tasks.FirstOrDefault(x => x.ID == temp.ID);
                    CurrentTask.TaskFiles.Load();
                    CurrentTask.Perfomers.Load();

                    TaskControlWindow createWindow = new TaskControlWindow();
                    WindowPositionBehaviour.SetWindowPosition(createWindow, WindowPosition.None);
                    WindowVisibilityBehaviour.SetIsDialogVisible(createWindow, true);

                    if (createWindow.DialogResult == true)
                    {
                        //сохраняем перед обработкой 
                        //проблема с исполнителями
                        foreach (var item in temp.Perfomers) {
                            if (CurrentTask.Perfomers.Count(x => x.ID == item.ID) == 0)
                                context.Perfomers.DeleteObject(context.Perfomers.FirstOrDefault(x => x.ID == item.ID));
                        }

                        foreach (var item in temp.TaskFiles) {
                            if (CurrentTask.TaskFiles.Count(x => x.ID == item.ID) == 0)
                                context.TaskFiles.DeleteObject(context.TaskFiles.FirstOrDefault(x => x.ID == item.ID));
                        }

                        if (temp.TaskFiles.Count == 0)
                            foreach (var item in CurrentTask.TaskFiles)
                                if (item.ID != 0)
                                    context.TaskFiles.DeleteObject(context.TaskFiles.FirstOrDefault(x => x.ID == item.ID));
                        
                        context.TaskComments.AddObject(CurrentTask.AddSystemComment("edit", CurrentUser.ID));

                        context.SaveChanges();

                        // entery data journal
                        AdminViewModel.SendDataJournal(TypeMessage.ChangedTask, this.CurrentUser, this.CurrentTask);

                        int id = Tasks.IndexOf(temp);
                        if (id != -1) {
                            Tasks.Remove(temp);
                            Tasks.Insert(id, CurrentTask);

                            ActionCalendar((panel) => panel.Remove(temp));
                            ActionCalendar((panel) => panel.Add(CurrentTask));
                        }
                    }

                    CurrentTask = null;
                }

                if (tempSelection)
                    SelectedTask = temp;

                this.State = TypeControl.Normal;
                FrameworkElementOpacityBehaviour.SetIsShow(w, true);
            }, obj => obj != null));
            }
        }

        private Command _copyTaskCommand;
        public Command CopyTaskCommand {
            get {
                return _copyTaskCommand ?? (_copyTaskCommand = new Command(obj => {
                    Task temp = (Task)((Tuple<object, object>)obj).Item1;
                    Window w = (Window)((Tuple<object, object>)obj).Item2;
                    bool tempSelection = IsSelectedTask;
                    IsSelectedTask = false;

                    this.State = TypeControl.Create;
                    FrameworkElementOpacityBehaviour.SetIsShow(w, false);
                    w.Opacity = 0;

                    using (DBContext context = new DBContext(false)) {
                        var tempTask = context.Tasks.FirstOrDefault(x => x.ID == temp.ID);
                        tempTask.Perfomers.Load();
                        CurrentTask = tempTask.Copy();

                        TaskControlWindow createWindow = new TaskControlWindow();
                        HideButtonBehaviour.SetCommand(createWindow, CanceliWndowCommand);
                        WindowPositionBehaviour.SetWindowPosition(createWindow, WindowPosition.None);
                        WindowVisibilityBehaviour.SetIsDialogVisible(createWindow, true);

                        if (createWindow.DialogResult == true) {
                            context.Tasks.AddObject(CurrentTask);
                            context.SaveChanges();

                            // entery data journal
                            AdminViewModel.SendDataJournal(TypeMessage.CreateTask, this.CurrentUser, CurrentTask);

                            Tasks.Add(CurrentTask);
                        }
                        else
                            CurrentTask.Disposing(App.DBSettings.Connection.PathToResourceDB);

                        CurrentTask = null;
                    }

                    if (tempSelection)
                        SelectedTask = temp;

                    this.State = TypeControl.Normal;
                    FrameworkElementOpacityBehaviour.SetIsShow(w, true);
                }, obj => obj != null));
            }
        }



        private Command _startExecutionTaskCommand;
        public Command StartExecutionTaskCommand {
            get {
                return _startExecutionTaskCommand ?? (_startExecutionTaskCommand = new Command(obj => {
                    Perfomer pf = SelectedTask.Perfomers.FirstOrDefault(x => x.IDUser == App.CurrentUser.ID);
                    if (pf != null) {
                        pf.StartDate = DateTime.Now;
                        using (DBContext context = new DBContext(false)) {
                            var temp = context.Perfomers.FirstOrDefault(x => x.ID == pf.ID);
                            temp.StartDate = pf.StartDate;

                            context.TaskComments.AddObject(SelectedTask.AddSystemComment("start", CurrentUser.ID));

                            context.SaveChanges();

                            // entery data journal
                            AdminViewModel.SendDataJournal(TypeMessage.StartExecution, this.CurrentUser, this.SelectedTask);
                        }
                        pf.OnPropertyChangedByExecution();
                    }

                    this.SendPropertyChanged(nameof(IsNotStartExecutionTask));
                    this.SendPropertyChanged(nameof(IsStartExecutionTask));
                    this.SendPropertyChanged(nameof(IsExecutionTask));
                    this.SendPropertyChanged(nameof(StartDateByCurrentPerfomer));
                    this.SendPropertyChanged(nameof(CloseDateByCurrentPerfomer));

                    SelectedTask.OnPropertyChangedState();

                }, obj => SelectedTask != null));
            }
        }

        private Command _executionTaskCommand;
        public Command ExecutionTaskCommand {
            get {
                return _executionTaskCommand ?? (_executionTaskCommand = new Command(obj => {
                    Perfomer pf = SelectedTask.Perfomers.FirstOrDefault(x => x.IDUser == App.CurrentUser.ID);
                    if (pf != null) {
                        if (pf.PersonInCharge && SelectedTask.Perfomers.Count(x => !x.PersonInCharge && !x.State) != 0) {
                            SetMessage(App.GetString("TaskPanelNonExecution"), Colors.Maroon, Colors.White);
                            return;
                        }

                        pf.CloseDate = DateTime.Now;
                        pf.State = true;

                        using (DBContext context = new DBContext(false)) {
                            var temp = context.Perfomers.FirstOrDefault(x => x.ID == pf.ID);
                            temp.CloseDate = pf.CloseDate;
                            temp.State = true;

                            context.TaskComments.AddObject(SelectedTask.AddSystemComment("fine", CurrentUser.ID));

                            context.SaveChanges();

                            // entery data journal
                            AdminViewModel.SendDataJournal(TypeMessage.ComplitedExecution, this.CurrentUser, this.SelectedTask, this.CurrentUser);

                            pf.OnPropertyChangedByExecution();
                        }

                        this.SendPropertyChanged(nameof(IsNotStartExecutionTask));
                        this.SendPropertyChanged(nameof(IsStartExecutionTask));
                        this.SendPropertyChanged(nameof(IsExecutionTask));
                        this.SendPropertyChanged(nameof(StartDateByCurrentPerfomer));
                        this.SendPropertyChanged(nameof(CloseDateByCurrentPerfomer));

                        SelectedTask.OnPropertyChangedState();
                    }
                }, obj => SelectedTask != null));
            }
        }

        private Command _cancellingExecutionOnTaskByPerfomerCommand;
        public Command CancellingExecutionOnTaskByPerfomerCommand {
            get {
                return _cancellingExecutionOnTaskByPerfomerCommand ?? (_cancellingExecutionOnTaskByPerfomerCommand = new Command(obj => {
                    Perfomer pf = (Perfomer)obj;
                    using (DBContext context = new DBContext(true)) {
                        Perfomer pfc = context.Perfomers.FirstOrDefault(x => x.ID == pf.ID);
                        if (pfc != null) {
                            pfc.CloseDate = pf.CloseDate = pfc.Task.OpenDate;
                            pfc.State = pf.State = false;

                            context.TaskComments.AddObject(SelectedTask.AddSystemComment("cancelling|" + pfc.FullNamePerfomer, CurrentUser.ID));

                            context.SaveChanges();

                            // entery data journal
                            AdminViewModel.SendDataJournal(TypeMessage.CancelingExecution, this.CurrentUser, this.SelectedTask, pfc.User);

                            pf.OnPropertyChangedByExecution();
                        }

                        this.SendPropertyChanged(nameof(IsNotStartExecutionTask));
                        this.SendPropertyChanged(nameof(IsStartExecutionTask));
                        this.SendPropertyChanged(nameof(IsExecutionTask));
                        this.SendPropertyChanged(nameof(StartDateByCurrentPerfomer));
                        this.SendPropertyChanged(nameof(CloseDateByCurrentPerfomer));

                        SelectedTask.OnPropertyChangedState();
                    }
                }, obj => obj != null));
            }
        }


        #region Commands by perfomers type
        private Command _addNewPerfomerCommand;
        public Command AddNewPerfomerCommand {
            get { return _addNewPerfomerCommand ?? (_addNewPerfomerCommand = new Command(obj => {
                if (CurrentTask.Perfomers.Count > 0)
                    CurrentTask.Perfomers.Add(new Perfomer { IDUser = (int)obj, StartDate = CurrentTask.OpenDate, CloseDate = CurrentTask.OpenDate, OpenDateOnTask = CurrentTask.OpenDate });
                else
                    CurrentTask.Perfomers.Add(new Perfomer { IDUser = (int)obj, StartDate = CurrentTask.OpenDate, CloseDate = CurrentTask.OpenDate, PersonInCharge = true, OpenDateOnTask = CurrentTask.OpenDate });
                this.SendPropertyChanged(nameof(UserListByPerfomers));
                CurrentTask.OnChangeCount();
            }, obj => obj is int)); }
        }

        private Command _removePerfomerOnListCommand;
        public Command RemovePerfomerOnListCommand {
            get {
                return _removePerfomerOnListCommand ?? (_removePerfomerOnListCommand = new Command(obj =>
                {
                    CurrentTask.Perfomers.Remove((Perfomer)obj);
                    this.SendPropertyChanged(nameof(UserListByPerfomers));
                    CurrentTask.OnChangeCount();
                }, obj => obj != null));
            }
        }

        private Command _clearAllPerfomerOnListCommand;
        public Command ClearAllPerfomerOnListCommand {
            get {
                return _clearAllPerfomerOnListCommand ?? (_clearAllPerfomerOnListCommand = new Command(obj =>
                {
                    IEnumerable<Perfomer> tempPf = CurrentTask.Perfomers.Where(x => !x.IsNotRemoved).ToArray();
                    foreach (var item in tempPf) {
                        CurrentTask.Perfomers.Remove(item);
                    }
                    this.SendPropertyChanged(nameof(UserListByPerfomers));
                    CurrentTask.OnChangeCount();
                }));
            }
        }

        private Command _setAllPerfomerOnListCommand;
        public Command SetAllPerfomerOnListCommand {
            get {
                return _setAllPerfomerOnListCommand ?? (_setAllPerfomerOnListCommand = new Command(obj =>
                {
                    bool isFirst = UserListByPerfomers.Count() != 0;
                    foreach (var item in UserListByPerfomers) {
                        if (!isFirst) {
                            isFirst = true;
                            CurrentTask.Perfomers.Add(new Perfomer { IDUser = item.Item2, StartDate = CurrentTask.OpenDate, CloseDate = CurrentTask.OpenDate, PersonInCharge = true, OpenDateOnTask = CurrentTask.OpenDate });
                        }
                        else
                            CurrentTask.Perfomers.Add(new Perfomer { IDUser = item.Item2, StartDate = CurrentTask.OpenDate, CloseDate = CurrentTask.OpenDate, OpenDateOnTask = CurrentTask.OpenDate });
                    }
                    this.SendPropertyChanged(nameof(UserListByPerfomers));
                    CurrentTask.OnChangeCount();
                }));
            }
        }


        private Command _updatePerfomerCountCommand;
        public Command UpdatePerfomerCountCommand {
            get {
                return _updatePerfomerCountCommand ?? (_updatePerfomerCountCommand = new Command(obj => {
                    CurrentTask.OnChangeCount();
                }));
            }
        }


        private Command _clickByMessageCommand;
        public Command ClickByMessageCommand {
            get {
                return _clickByMessageCommand ?? (_clickByMessageCommand = new Command(obj => {
                    var item = (ItemMessage)obj;
                    Messages.Remove(item);
                    this.SendPropertyChanged(nameof(ShowMessage));
                    CurrentTask?.OnChangeCount();
                }, obj => obj != null));
            }
        }


        #endregion

        #region Files command
        private Command _controlFilesCommand;
        public Command ControlFilesCommand {
            get {
                return _controlFilesCommand ?? (_controlFilesCommand = new Command(obj => {
                    Window parent = (Window)obj;

                    FrameworkElementOpacityBehaviour.SetIsShow(parent, false);
                    parent.Opacity = 0;

                    FilesControlWindow fcw = new FilesControlWindow();
                    fcw.DataContext = new FilesViewModel(CurrentTask.TaskFiles);
                    WindowPositionBehaviour.SetWindowPosition(fcw, WindowPosition.Left);
                    WindowVisibilityBehaviour.SetIsDialogVisible(fcw, true);
                    //забираем все файлы отмеченные в представлении
                    CurrentTask.TaskFiles.Clear();
                    foreach (var item in ((FilesViewModel)fcw.DataContext).Files) {
                        item.IdTask = CurrentTask.ID;
                        item.AddUser = CurrentTask.Creater;
                        CurrentTask.TaskFiles.Add(item);
                    }
                    this.SendPropertyChanged(nameof(CurrentTask));

                    FrameworkElementOpacityBehaviour.SetIsShow(parent, true);
                }, obj => CurrentTask != null));
            }
        }

        private Command _removeFileOnListCommand;
        public Command RemoveFileOnListCommand {
            get {
                return _removeFileOnListCommand ?? (_removeFileOnListCommand = new Command(obj => {
                    CurrentTask.TaskFiles.Remove(((TaskFile)obj).Disposing(App.DBSettings.Connection.PathToResourceDB));
                    this.SendPropertyChanged(nameof(CurrentTask));
                }, obj => obj is TaskFile));
            }
        }

        private Command _clearAllFileOnTaskCommand;
        public Command ClearAllFileOnTaskCommand {
            get {
                return _clearAllFileOnTaskCommand ?? (_clearAllFileOnTaskCommand = new Command(obj => {
                    IEnumerable<TaskFile> temp = CurrentTask.TaskFiles.ToArray();
                    foreach (var item in temp) {
                        CurrentTask.TaskFiles.Remove(item.Disposing(App.DBSettings.Connection.PathToResourceDB));
                    }
                    this.SendPropertyChanged(nameof(CurrentTask));
                }, obj => CurrentTask != null && CurrentTask.TaskFiles.Count > 0 && CurrentTask.TaskFiles.Count(x => x.State == StateFile.Load) == 0));
            }
        }

        private Command _downloadFileCommand;
        public Command DownloadFileCommand {
            get {
                return _downloadFileCommand ?? (_downloadFileCommand = new Command(obj => {
                    TaskFile n = (TaskFile)obj;

                    DBSettings setting = DBSettings.Load();

                    if (CurrentUser.Setting.AlwaysSpecifyAPlaceToDdownload) {
                        File.Copy(Path.Combine(setting.Connection.PathToResourceDB, "Resources", n.StoreName), Path.Combine(AppSettings.Load().PathOfDownload, n.OriginalName), true);
                        Process.Start(@"explorer.exe", "/select, \"" + Path.Combine(AppSettings.Load().PathOfDownload, n.OriginalName) + "\"");
                    }
                    else {
                        System.Windows.Forms.FolderBrowserDialog broser = new System.Windows.Forms.FolderBrowserDialog();
                        switch (broser.ShowDialog()) {
                            case System.Windows.Forms.DialogResult.OK:
                                File.Copy(Path.Combine(setting.Connection.PathToResourceDB, "Resources", n.StoreName), Path.Combine(broser.SelectedPath, n.OriginalName), true);
                                Process.Start(@"explorer.exe", "/select, \"" + Path.Combine(broser.SelectedPath, n.OriginalName) + "\"");
                                break;
                        }
                    }
                }, obj => obj is TaskFile));
            }
        }


        private Command _downloadAllCommand;
        public Command DownloadAllCommand {
            get {
                return _downloadAllCommand ?? (_downloadAllCommand = new Command(obj => {
                    IEnumerable<TaskFile> ns = (IEnumerable<TaskFile>)obj;

                    DBSettings setting = DBSettings.Load();
                    string pathDownload = AppSettings.Load().PathOfDownload;

                    if (CurrentUser.Setting.AlwaysSpecifyAPlaceToDdownload) {
                        foreach (TaskFile n in ns)
                            File.Copy(Path.Combine(setting.Connection.PathToResourceDB, "Resources", n.StoreName), Path.Combine(pathDownload, n.OriginalName), true);
                    }
                    else {
                        System.Windows.Forms.FolderBrowserDialog broser = new System.Windows.Forms.FolderBrowserDialog();
                        switch (broser.ShowDialog()) {
                            case System.Windows.Forms.DialogResult.OK:
                                string arguments = "/select, ";
                                foreach (TaskFile n in ns) {
                                    File.Copy(Path.Combine(setting.Connection.PathToResourceDB, "Resources", n.StoreName), Path.Combine(broser.SelectedPath, n.OriginalName), true);
                                    arguments += "\"" + Path.Combine(broser.SelectedPath, n.OriginalName) + "\",";
                                }
                                Process.Start(@"explorer.exe", arguments);
                                break;
                        }
                    }
                }, obj => obj != null));
            }
        }
        #endregion

        #region Comments command
        private Command _sendCommentCommand;
        public Command SendCommentCommand {
            get {
                return _sendCommentCommand ?? (_sendCommentCommand = new Command(obj =>
                {
                var task = (Task)obj;
                    if (this.CommentText.Replace(" ", "").Length > 0)
                    {
                        TaskComment newComment = new TaskComment();
                        using (DBContext context = new DBContext(false))
                        {
                            newComment.IDUser = CurrentUser.ID;
                            newComment.IdTask = task.ID;
                            newComment.CreateDate = DateTime.Now;
                            newComment.Commentary = this.CommentText;
                            newComment.TypeCommentary = 0;

                            context.TaskComments.AddObject(newComment);
                            context.SaveChanges();

                            Dispatcher.CurrentDispatcher.BeginInvoke((Action)(() =>
                            {
                                task.AddComment(newComment);
                            }));
                        }
                        this.CommentText = "";
                        commentScrollInfoPanel.ScrollToBottom();

                        //передаем глобальное сообщение о том что был добавлен комментарий
                        NTW.Communication.Beginers.ClientBeginer.Send("localhost", 8810, "commands", (int)TypeMessage.AddComment, newComment.IDUser, new int[] { newComment.ID, newComment.IdTask });
                    }
                }, obj => obj is Task));
            }
        }

        private Command _loadMoreCommentsCommand;
        public Command LoadMoreCommentsCommand {
            get {
                return _loadMoreCommentsCommand ?? (_loadMoreCommentsCommand = new Command(obj =>
                {
                    var task = (Task)obj;
                    task.LoadMoreComment();
                    commentScrollInfoPanel.ScrollToTop();

                }, obj => obj is Task));
            }
        }



        private Command _loadCommentPanelCommand;
        public Command LoadCommentPanelCommand {
            get {
                return _loadCommentPanelCommand ?? (_loadCommentPanelCommand = new Command(obj => {
                    var ic = (ScrollViewInfo)obj;
                    commentScrollInfoPanel = ic;

                    ic.ScrollToBottom();
                }, obj => obj is ScrollViewInfo));
            }
        }


        #endregion

        #region Unload
        private Command _unloadedCommand;
        public Command UnloadedCommand {
            get {
                return _unloadedCommand ?? (_unloadedCommand = new Command(obj => {
                    _tasks = null;
                    _tasksCollectionView = null;
                }));
            }
        }
        #endregion

        private Command _loadedCalendarCommand;
        public Command LoadedCalendarCommand {
            get {
                return _loadedCalendarCommand ?? (_loadedCalendarCommand = new Command(obj =>
                {
                    var panel = (CalendarPanel)obj;
                    calendar = panel;
                    //panel.DataSource = GetTasks();

                    panel.SetParametry(
                        item => new object[] { ((Task)item).TypeString },
                        item => Enumerable.Range(0, 1 + ((Task)item).CountDays).Select(x => (object)((Task)item).OpenDate.Date.AddDays(x)).ToArray(),

                        () => { return ((DoubleStruct[])Application.Current.Resources["ArrayTypeTask"]).Select(x => x.Name).ToArray(); },
                        () => {
                            IEnumerable<object> result = null;
                            panel.Dispatcher.Invoke((Action)(() => {
                                result = Enumerable.Range(0, 1 + panel.EndDateTime.Date.Subtract(panel.BeginDateTime.Date).Days).Select(x => (object)panel.BeginDateTime.Date.AddDays(x)).ToArray(); ;
                            }));
                            return result;
                        },

                        (row, index) => new Virtualization.Models.Position(index, 0, 0, rowSpan: 4),
                        (col, index) => new Virtualization.Models.Position(index, 0, 0, colSpan: 4),

                        null,
                        new Func<object, object>[] {
                            (col) => ((DateTime)col).Year,
                            (col) => ((DateTime)col).Month },

                        null,
                        null,

                        (item, col) => ((Task)item).OpenDate.Date != ((DateTime)col).Date ? 0 : (((Task)item).OpenDate.Hour < 6 ? 0 : (((Task)item).OpenDate.Hour < 12 ? 1 : (((Task)item).OpenDate.Hour < 18 ? 2 : 3))),
                        (item, col, colLast) => (((Task)item).OpenDate.Date != ((DateTime)col).Date ? 0 : (((Task)item).OpenDate.Hour < 6 ? 0 : (((Task)item).OpenDate.Hour < 12 ? 1 : (((Task)item).OpenDate.Hour < 18 ? 2 : 3))))
                        + (((Task)item).EndDate.Date > ((DateTime)colLast) ? 0 : ((((Task)item).EndDate.Hour >= 18 ? 0 : (((Task)item).EndDate.Hour >= 12 ? 1 : (((Task)item).EndDate.Hour >= 6 ? 2 : 3))))),
                        (item) => {
                            if (item != null)
                            {
                                var p = (Tuple<Position, object>)item;
                                return new Tuple<int, int, int, int, Color>(0, p.Item1.Row, 1, p.Item1.RowSpan, ConverterTypeTaskToColor(((Task)p.Item2).TypeTask));
                            }
                            else
                                return null;
                        },
                        (item) => {
                            if (item != null)
                            {
                                var p = (Tuple<Position, object>)item;
                                return new Tuple<int, int, int, int, Color>(p.Item1.Column, 0, p.Item1.ColumnSpan, 1, ConverterTypeTaskToColor(((Task)p.Item2).TypeTask));
                            }
                            else return null;
                        },
                        (info) => { info.Row = ConvertColorToTypeTask((Color)info.Color); },
                        (info) => { info.Column = ConvertColorToTypeTask((Color)info.Color); });

                    panel.SetEventHandlerDateChange((s, a) =>
                    {
                        _tasks = null;
                        _tasksCollectionView = null;
                        this.SendPropertyChanged(nameof(Tasks));
                        this.SendPropertyChanged(nameof(TaskCollectionView));
                    });

                    panel.SelectionDoubleClickChanged += Panel_SelectionDoubleClickChanged;
                }));
            }
        }

        private void Panel_SelectionDoubleClickChanged(object sender, Virtualization.Delegate.SelectionValueEventArgs args)
        {
            if (args.SelectedValue == null)
            {
                var parent = Common.FindAncestor<Window>(sender as DependencyObject);

                DateTime df = (DateTime)args.SelectedColumn.Content;
                df = df.AddHours(6 * args.SelectedColumn.Step);

                this.State = TypeControl.Create;
                FrameworkElementOpacityBehaviour.SetIsShow(parent, false);
                parent.Opacity = 0;
                using (DBContext context = new DBContext(false))
                {
                    CurrentTask = Task.New();
                    CurrentTask.Creater = App.CurrentUser.ID;
                    CurrentTask.OpenDate = df;
                    CurrentTask.EndDate = CurrentTask.OpenDate.AddDays(1);
                    CurrentTask.IdProject = context.Projects.FirstOrDefault().ID;
                    CurrentTask.TypeTask = ((DoubleStruct[])Application.Current.Resources["ArrayTypeTask"]).FirstOrDefault(x => x.Name == args.SelectedRow.Content.ToString()).Value;

                    TaskControlWindow createWindow = new TaskControlWindow();
                    HideButtonBehaviour.SetCommand(createWindow, CanceliWndowCommand);
                    WindowPositionBehaviour.SetWindowPosition(createWindow, WindowPosition.None);
                    WindowVisibilityBehaviour.SetIsDialogVisible(createWindow, true);

                    if (createWindow.DialogResult == true)
                    {
                        context.Tasks.AddObject(CurrentTask);
                        context.SaveChanges();

                        Tasks.Add(CurrentTask);

                        ActionCalendar((panel) => panel.Add(CurrentTask));
                    }
                    else
                        CurrentTask.Disposing(App.DBSettings.Connection.PathToResourceDB);

                    CurrentTask = null;
                }
                this.State = TypeControl.Normal;
                FrameworkElementOpacityBehaviour.SetIsShow(parent, true);
            }
        }
        #endregion

        #region Members
        private Task _currentTask = null;
        public Task CurrentTask {
            get { return _currentTask; }
            protected set { _currentTask = value; this.SendPropertyChanged(nameof(CurrentTask)); }
        }

        private Task _selectedTask;
        public Task SelectedTask {
            get { return _selectedTask; }
            set { //придется сделать что то вроде подгрузки для задачи когда она выбрана
                //using (DBContext context = new DBContext(false)) {
                _selectedTask?.DisposeComments();
                //}
                Console.WriteLine("-> " + _selectedTask);
                _selectedTask = null;
                this.SendPropertyChanged(nameof(SelectedTask));
                this.SendPropertyChanged(nameof(ShowPerfomerPanel));
                this.SendPropertyChanged(nameof(ShowFilePanel));
                _selectedTask = value;
                Console.WriteLine("<- " + _selectedTask);
                this.SendPropertyChanged(nameof(SelectedTask));
                this.SendPropertyChanged(nameof(IsSelectedTask));

                this.SendPropertyChanged(nameof(IsNotStartExecutionTask));
                this.SendPropertyChanged(nameof(IsStartExecutionTask));
                this.SendPropertyChanged(nameof(IsExecutionTask));
                this.SendPropertyChanged(nameof(StartDateByCurrentPerfomer));
                this.SendPropertyChanged(nameof(CloseDateByCurrentPerfomer));

                this.SendPropertyChanged(nameof(ShowPerfomerPanel));
                this.SendPropertyChanged(nameof(ShowFilePanel));
                Console.WriteLine("selection " + _selectedTask);
            }
        }

        public bool IsSelectedTask {
            get { return SelectedTask != null; }
            set { SelectedTask = null; }
        }

        public IEnumerable<Project> Projects {
            get {
                IEnumerable<Project> result = null;
                using (DBContext context = new DBContext(false))
                    result = context.Projects.ToArray();
                return result;
            }
        }

        private DateTime _beginDate = DateTime.Now.Date;
        public DateTime BeginDate {
            get { return _beginDate; }
            set { _beginDate = value; _tasks = null; _tasksCollectionView = null; this.SendPropertyChanged(nameof(Tasks)); this.SendPropertyChanged(nameof(TaskCollectionView)); this.SendPropertyChanged(nameof(BeginDate)); }
        }

        private DateTime _endDate =DateTime.Now.Date.AddDays(7);
        public DateTime EndDate {
            get { return _endDate; }
            set { _endDate = value; _tasks = null; _tasksCollectionView = null; this.SendPropertyChanged(nameof(Tasks)); this.SendPropertyChanged(nameof(TaskCollectionView)); this.SendPropertyChanged(nameof(EndDate)); }
        }

        private string _searchText;
        [ReportStat]
        public string SearchText {
            get { return _searchText; }
            set { _searchText = value;
                string h = value.ToUpper();
                using (DBContext context = new DBContext(false))
                    TaskCollectionView.Filter = new Predicate<object>((x) => ((Task)x).Caption.ToUpper().Contains(h)
                    | ((Task)x).Text.ToUpper().Contains(h) |
                    ((Task)x).Perfomers.Count(y => y.FullNamePerfomer.ToUpper().Contains(h)) > 0);
                this.SendPropertyChanged(nameof(SearchText));
            }
        }

        private ObservableCollection<Task> _tasks;
        [ReportStat]
        public ObservableCollection<Task> Tasks {
            get { return _tasks ?? (_tasks = new ObservableCollection<Task>(GetTasks())); }
        }

        private ICollectionView _tasksCollectionView;
        [ReportType(typeof(Task))]
        [ReportStat]
        public ICollectionView TaskCollectionView {
            get { return _tasksCollectionView ?? (_tasksCollectionView = CollectionViewSource.GetDefaultView(Tasks)); }
        }

        private bool _showFullSizeEditor;
        public bool ShowFullSizeEditor {
            get { return _showFullSizeEditor; }
            set { _showFullSizeEditor = value; this.SendPropertyChanged(nameof(ShowFullSizeEditor)); }
        }

        public bool IsCurrentTaskPerson {
            get { return CurrentTask.IsPersonal; }
            set {
                CurrentTask.IsPersonal = value;

                if (value) {
                    ShowClearAllButton = ShowSetAllButton = ShowAddPanel = false;
                    CurrentTask.Perfomers.Clear();
                    CurrentTask.Perfomers.Add(new Perfomer { IDUser = CurrentUser.ID, StartDate = CurrentTask.OpenDate, CloseDate = CurrentTask.OpenDate, OpenDateOnTask = CurrentTask.OpenDate });
                    CurrentTask.RefreshErrorsState();
                }
                else
                    this.SendPropertyChanged(nameof(UserListByPerfomers));
                this.SendPropertyChanged(nameof(IsCurrentTaskPerson));
            }
        }

        #region Members for perfomers
        public IEnumerable<Tuple<string, int>> UserListByPerfomers {
            get {
                IEnumerable<Tuple<string, int>> result = null;
                using (DBContext context = new DBContext(false)) {
                    result = context.Users.Where(x => x.Role.ExecutionTask && !x.IsDelete).ToArray().Select(x => new Tuple<string, int>(x.FullName, x.ID)).ToArray();
                }
                result = result.Where(x => CurrentTask.Perfomers.Count(y => y.IDUser == x.Item2) == 0);

                ShowClearAllButton = result.Count() == 0;
                ShowSetAllButton = ShowAddPanel = result.Count() != 0;

                CurrentTask.RefreshErrorsState();
                return result;
            }
        }

        private bool _showClearAllButton = true;
        public bool ShowClearAllButton {
            get { return _showClearAllButton; }
            set { _showClearAllButton = value; this.SendPropertyChanged(nameof(ShowClearAllButton)); }
        }

        private bool _showSetAllButton = true;
        public bool ShowSetAllButton {
            get { return _showSetAllButton; }
            set { _showSetAllButton = value; this.SendPropertyChanged(nameof(ShowSetAllButton)); }
        }

        private bool _showAddPanel = true;
        public bool ShowAddPanel {
            get { return _showAddPanel; }
            set { _showAddPanel = value; this.SendPropertyChanged(nameof(ShowAddPanel)); }
        }

        private ObservableCollection<ItemMessage> messages;
        public ObservableCollection<ItemMessage> Messages {
            get { return messages ?? (messages = new ObservableCollection<ItemMessage>()); }
        }

        public bool ShowMessage {
            get { return Messages.Count > 0; }
        }

        public bool ShowPerfomerPanel {
            get { return SelectedTask == null ? false : SelectedTask.Perfomers.Count > 0; }
        }
        #endregion

        public bool ShowFilePanel {
            get { return SelectedTask == null ? false : SelectedTask.TaskFiles.Count > 0; }
        }

        public bool IsNotStartExecutionTask {
            get {
                if (SelectedTask == null)
                    return false;

                Perfomer pf = SelectedTask.Perfomers.FirstOrDefault(x => x.IDUser == App.CurrentUser.ID);
                if (pf == null)
                    return false;

                return pf.StartDate == SelectedTask.OpenDate && pf.CloseDate == SelectedTask.OpenDate;
            }
        }

        public bool IsStartExecutionTask {
            get {
                if (SelectedTask == null)
                    return false;

                Perfomer pf = SelectedTask.Perfomers.FirstOrDefault(x => x.IDUser == App.CurrentUser.ID);
                if (pf == null)
                    return false;

                return pf.StartDate != SelectedTask.OpenDate && pf.CloseDate == SelectedTask.OpenDate;
            }
        }

        public bool IsExecutionTask {
            get {
                if (SelectedTask == null)
                    return false;

                Perfomer pf = SelectedTask.Perfomers.FirstOrDefault(x => x.IDUser == App.CurrentUser.ID);
                if (pf == null)
                    return false;

                return pf.State;
            }
        }

        public string StartDateByCurrentPerfomer {
            get {
                if (SelectedTask == null)
                    return "";
                Perfomer pf = SelectedTask.Perfomers.FirstOrDefault(x => x.IDUser == App.CurrentUser.ID);
                if (pf == null)
                    return "";

                return string.Format("{0:dd.MM.yyyy HH:mm:ss}", pf.StartDate);
            }
        }

        public string CloseDateByCurrentPerfomer {
            get {
                if (SelectedTask == null)
                    return "";
                Perfomer pf = SelectedTask.Perfomers.FirstOrDefault(x => x.IDUser == App.CurrentUser.ID);
                if (pf == null)
                    return "";

                return string.Format("{0:dd.MM.yyyy HH:mm:ss}", pf.CloseDate);
            }
        }

        private int _selectionVisulaType = 1;
        public int SelectionVisualType {
            get { return _selectionVisulaType; }
            set {
                if (value != _selectionVisulaType) {
                    _selectionVisulaType = value;
                    this.SendPropertyChanged(nameof(SelectionVisualType));
                    if (_tasks != null)
                        _tasks.Clear();
                    _tasks = null;
                    _tasksCollectionView = null;
                    this.SendPropertyChanged(nameof(TaskCollectionView));
                    this.SendPropertyChanged(nameof(Tasks));
                }
            }
        }

        private bool _isCalendar = false;
        public bool IsCalendar {
            get { return _isCalendar; }
            set { _isCalendar = value; if (!value) { calendar.SelectionDoubleClickChanged -= Panel_SelectionDoubleClickChanged; calendar = null; } this.SendPropertyChanged(nameof(IsCalendar)); }
        }

        private CalendarPanel calendar;

        #region Commants
        private string _commentText;
        public string CommentText {
            get { return _commentText; }
            set { _commentText = value; this.SendPropertyChanged(nameof(CommentText)); }
        }

        public Func<object, Tuple<int, int, int, int, Color>> VerticalMetric 
            {
            get {
                return (item) =>
                {
                    if (item != null)
                    {
                        var p = (TaskComment)item;
                        return new Tuple<int, int, int, int, Color>(1 - p.TypeCommentary, SelectedTask.Comments.IndexOf(p), 1, 1, p.TypeCommentary == 0 ? Colors.Green : Colors.DodgerBlue );
                    }
                    else return null;
                };
            }
        }

        private ScrollViewInfo commentScrollInfoPanel;
        #endregion
        #endregion

        #region Helps
        protected IEnumerable<Task> GetTasks() {
            IEnumerable<Task> result = null;
            using (DBContext context = new DBContext(false)) {
                //пака без постусловий
                switch (SelectionVisualType) {
                    case 1:
                        result = context.Tasks.Where(x => x.Perfomers.Count(y => y.IDUser == App.CurrentUser.ID) > 0 && !x.IsDelete && !(x.OpenDate > EndDate || x.EndDate < BeginDate)).ToArray();
                        break;
                    case 2:
                        result = context.Tasks.Where(x => x.Creater == App.CurrentUser.ID && !x.IsDelete && !(x.OpenDate > EndDate || x.EndDate < BeginDate)).ToArray();
                        break;
                    case 3:
                        result = context.Tasks.Where(x => x.Creater == App.CurrentUser.ID && x.IsPersonal && !x.IsDelete && !(x.OpenDate > EndDate || x.EndDate < BeginDate)).ToArray();
                        break;
                    case 4:
                        result = context.Tasks.Where(x => x.Favorites.Count(y => y.IDUser == CurrentUser.ID) > 0 && !x.IsDelete && !(x.OpenDate > EndDate || x.EndDate < BeginDate)).ToArray();
                        break;
                }
                foreach (var item in result) {
                    item.Perfomers.Load();
                    item.TaskFiles.Load();
                }
            }
            return result;
        }

        public void SetValues(Type typeContent, IEnumerable values)
        {
            if (values != null) {
                foreach (var item in values) {
                    CurrentTask.TaskFiles.Add(new TaskFile { OriginalName = System.IO.Path.GetFileName(item.ToString()), StoreName = Guid.NewGuid().ToString(), AddUser = App.CurrentUser.ID }.Inicial(App.DBSettings.Connection.PathToResourceDB, item.ToString()));
                }
                this.SendPropertyChanged(nameof(CurrentTask));
            }
        }

        public void SetValues(UIElement element, IEnumerable values) { }

        public void SetCurrentTask(Task task) {
            CurrentTask = task;
        }

        public void SetMessage(object message,
                               Color? backColor = null,
                               Color? foreColor = null,
                               VerticalAlignment position = VerticalAlignment.Bottom,
                               TimeSpan? timeOut = null) {
            var item = new ItemMessage(backColor ?? Colors.DimGray, foreColor ?? Colors.Red, message);

            Messages.Add(item);
            this.SendPropertyChanged(nameof(ShowMessage));

            DispatcherTimer dt = new DispatcherTimer();
            dt.Interval = timeOut == null ? new TimeSpan(0, 0, 3) : (TimeSpan)timeOut;
            dt.Tick += new EventHandler((s, e) => {
                dt.Stop();
                Messages.Remove(item);
                this.SendPropertyChanged(nameof(ShowMessage));
            });
            dt.Start();
        }

        public void ActionCalendar(Action<CalendarPanel> action) {
            if (calendar != null)
                action(calendar);
        }

        public Color ConverterTypeTaskToColor(int type)
        {
            Color color = new Color();
            switch (type)
            {
                case (int)TypeTask.Bug:
                    color = Colors.Maroon;
                    break;
                case (int)TypeTask.Enhancement:
                    color = Colors.DodgerBlue;
                    break;
                case (int)TypeTask.Proposal:
                    color = Colors.DarkGoldenrod;
                    break;
                case (int)TypeTask.Task:
                    color = Colors.Olive;
                    break;
            }
            return color;
        }

        public int ConvertColorToTypeTask(Color color)
        {
            int result = 0;
            if (color == Colors.Maroon)
                result = (int)TypeTask.Bug;
            else if (color == Colors.DodgerBlue)
                result = (int)TypeTask.Enhancement;
            else if (color == Colors.DarkGoldenrod)
                result = (int)TypeTask.Proposal;
            else if (color == Colors.Olive)
                result = (int)TypeTask.Task;
            return result;
        }
        #endregion

        #region FuncOnKeys
        public void CreateTask() {
            this.State = TypeControl.Create;
            using (DBContext context = new DBContext(false)) {
                CurrentTask = Task.New();
                CurrentTask.Creater = App.CurrentUser.ID;
                CurrentTask.OpenDate = DateTime.Now;
                CurrentTask.EndDate = CurrentTask.OpenDate.AddDays(1);
                CurrentTask.IdProject = context.Projects.FirstOrDefault().ID;

                TaskControlWindow createWindow = new TaskControlWindow();
                HideButtonBehaviour.SetCommand(createWindow, CanceliWndowCommand);
                WindowPositionBehaviour.SetWindowPosition(createWindow, WindowPosition.None);
                WindowVisibilityBehaviour.SetIsDialogVisible(createWindow, true);

                if (createWindow.DialogResult == true) {
                    context.Tasks.AddObject(CurrentTask);
                    context.SaveChanges();

                    Tasks.Add(CurrentTask);
                }
                else
                    CurrentTask.Disposing(App.DBSettings.Connection.PathToResourceDB);

                CurrentTask = null;
            }
            this.State = TypeControl.Normal;
        }

        public void CreateTaskOfClipboard() {
            if (Clipboard.GetText() != null && Clipboard.GetText() != "") {
                this.State = TypeControl.Create;
                using (DBContext context = new DBContext(false)) {
                    CurrentTask = Task.New();
                    FlowDocument fd = new FlowDocument();
                    fd.Blocks.Add(new Paragraph(new Run(Clipboard.GetText())));
                    CurrentTask.PText = XamlWriter.Save(fd);
                    CurrentTask.Creater = App.CurrentUser.ID;
                    CurrentTask.OpenDate = DateTime.Now;
                    CurrentTask.EndDate = CurrentTask.OpenDate.AddDays(1);
                    CurrentTask.IdProject = context.Projects.FirstOrDefault().ID;

                    TaskControlWindow createWindow = new TaskControlWindow();
                    HideButtonBehaviour.SetCommand(createWindow, CanceliWndowCommand);
                    WindowPositionBehaviour.SetWindowPosition(createWindow, WindowPosition.None);
                    WindowVisibilityBehaviour.SetIsDialogVisible(createWindow, true);

                    if (createWindow.DialogResult == true) {
                        context.Tasks.AddObject(CurrentTask);
                        context.SaveChanges();

                        Tasks.Add(CurrentTask);
                    }
                    else
                        CurrentTask.Disposing(App.DBSettings.Connection.PathToResourceDB);

                    CurrentTask = null;
                }
                this.State = TypeControl.Normal;
            }
        }
        #endregion
    }
}
