using ExtendedControl.Views.ControlWindow;
using ExtendedControl.Views.DragDrop;
using NTW.Attrebute;
using NTW.Controls.Behaviours;
using NTW.Core;
using NTW.Data;
using NTW.Data.Context;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

namespace ExtendedControl.ViewModels
{
    [ReportStat]
    public class NotesViewModel : ControlViewModel
    {
        #region Commands
        private Command _createNewNoteGroupCommand;
        public Command CreateNewNoteGroupCommand {
            get { return _createNewNoteGroupCommand ?? (_createNewNoteGroupCommand = new Command(obj => {
                this.State = TypeControl.Create;
                Window w = (Window)obj;
                try {
                    using (DBContext context = new DBContext(false)) {
                        NoteGroup newNG = NoteGroup.New();
                        Color cr = Colors.LightBlue;
                        newNG.IDUser = App.CurrentUser.ID;
                        newNG.IDColor = BitConverter.ToInt32(new byte[] { cr.B, cr.G, cr.R, cr.A }, 0);
                        context.NoteGroups.AddObject(newNG);

                        NotesGroups.Add(newNG);

                        context.SaveChanges();
                    }
                }//временно, что бы отловить ошибки
                catch (Exception ex) {
                    WindowMessageBehaviour.SetMessage(w, ex.Message, Colors.Maroon, Colors.White);
                }

                this.State = TypeControl.Normal;
            })); }
        }

        private Command _createNewNoteCommand;
        public Command CreateNewNoteCommand {
            get { return _createNewNoteCommand ?? (_createNewNoteCommand = new Command(obj => {
                this.State = TypeControl.Create;

                NoteGroup ng = NotesGroups.FirstOrDefault(x => x.ShowNotes);
                if (ng != null)
                {
                    using (DBContext context = new DBContext(false)) {
                        Note newN = new Note();
                        FlowDocument fd = new FlowDocument { AllowDrop = true, PageWidth = 130, PageHeight = 130};
                        fd.Blocks.Add(new Paragraph(new Run("Note")));
                        newN.Text = XamlWriter.Save(fd);
                        newN.Owner = App.CurrentUser.ID;
                        newN.Group = ng.ID;
                        newN.CreateDate = DateTime.Now;
                        newN.Width = 130;
                        newN.Height = 130;
                        newN.IDColor = ng.IDColor;

                        context.Notes.AddObject(newN);

                        context.SaveChanges();
                        ng.AddNewNote(newN);
                    }
                }

                this.State = TypeControl.Normal;
            })); }
        }

        private Command _removeNoteGroupCommand;
        public Command RemoveNoteGroupCommand {
            get { return _removeNoteGroupCommand ?? (_removeNoteGroupCommand = new Command(obj => {
                using (DBContext context = new DBContext(false))
                {
                    NoteGroup ng = context.NoteGroups.FirstOrDefault(x => x.ID == ((NoteGroup)obj).ID);

                    ng.Notes.Load();
                    IEnumerable<Note> ns = ng.Notes.ToArray();
                    foreach (Note item in ns) {
                        context.Notes.DeleteObject(item);
                    }
                    context.NoteGroups.DeleteObject(ng);
                    context.SaveChanges();

                    NotesGroups.Remove((NoteGroup)obj);
                }
            }, obj => obj is NoteGroup)); }
        }

        private Command _copyNoteGroupCommand;
        public Command CopyNoteGroupCommand {
            get {
                return _copyNoteGroupCommand ?? (_copyNoteGroupCommand = new Command(obj => {
                    this.State = TypeControl.Create;
                    NoteGroup bgnote = (NoteGroup)obj;
                    using (DBContext context = new DBContext(false)) {
                        NoteGroup copygnote = bgnote.Copy();

                        context.NoteGroups.AddObject(copygnote);
                        NotesGroups.Add(copygnote);

                        context.SaveChanges();
                    }

                    this.State = TypeControl.Normal;
                }, obj => obj is NoteGroup));
            }
        }

        private Command _removeNoteCommand;
        public Command RemoveNoteCommand {
            get { return _removeNoteCommand ?? (_removeNoteCommand = new Command(obj => {
                Note n = (Note)obj;
                using (DBContext context = new DBContext(false)) {
                    Note on = context.Notes.FirstOrDefault(x => x.ID == n.ID);
                    context.Notes.DeleteObject(on);
                    context.SaveChanges();

                    NoteGroup ng = NotesGroups.FirstOrDefault(x => x.ContainsNoteOnGroup(n));
                    if (ng != null)
                        ng.RemoveNote(n);
                }

            }, obj => obj is Note)); }
        }

        private Command _copyNoteCommand;
        public Command CopyNoteCommand {
            get {
                return _copyNoteCommand ?? (_copyNoteCommand = new Command(obj => {
                    this.State = TypeControl.Create;

                    var temp = (Tuple<object, object>)obj;
                    var copyNote = (Note)temp.Item1;
                    var idGroup = (int)temp.Item2;

                    NoteGroup ng = NotesGroups.FirstOrDefault(x => x.ID == idGroup);
                    if (ng != null)
                    {
                        using (DBContext context = new DBContext(false))
                        {
                            Note newN = copyNote.Copy(idGroup);

                            context.Notes.AddObject(newN);

                            context.SaveChanges();

                            ng.AddNewNote(newN);
                        }
                    }

                    this.State = TypeControl.Normal;
                }, obj => obj != null));
            }
        }

        private Command _clearNoteGroupCommand;
        public Command ClearNoteGroupCommand {
            get { return _clearNoteGroupCommand ?? (_clearNoteGroupCommand = new Command(obj => {
                NoteGroup ng = (NoteGroup)obj;
                using (DBContext context = new DBContext(false))
                {
                    foreach (var item in ng.GetCurrentNotes()) {
                        ng.RemoveNote(item);
                        context.Notes.DeleteObject(context.Notes.FirstOrDefault(x => x.ID == item.ID));
                    }
                    context.SaveChanges();
                }
                ng.Refresh();
            }, obj => obj is NoteGroup)); }
        }

        private Command _setNoteCommand;
        public Command SetNoteCommand {
            get { return _setNoteCommand ?? (_setNoteCommand = new Command(obj => {
                Note n = (Note)obj;
                NoteGroup ng = NotesGroups.FirstOrDefault(x => x.NotesView.Contains(n));
                using (DBContext context = new DBContext(false)) {
                    Note no = context.Notes.FirstOrDefault(x => x.ID == n.ID);
                    no.IsShowin = true;
                    context.SaveChanges();
                }
                if (ng != null)
                    ng.RemoveNote(n);
            }, obj => obj is Note)); }
        }

        private Command _unsafeNoteCommand;
        public Command UnsafeNoteCommand {
            get { return _unsafeNoteCommand ?? (_unsafeNoteCommand = new Command(obj => {
                Window w = (Window)obj;
                Note n = (Note)w.DataContext;
                using (DBContext context = new DBContext(false))
                {
                    Note no = context.Notes.FirstOrDefault(x => x.ID == n.ID);
                    no.IsShowin = false;
                    context.SaveChanges();
                }
                NoteGroup ng = NotesGroups.FirstOrDefault(x => x.ID == n.Group);
                if (ng != null && ng.ShowNotes)
                    ng.Refresh();
                w.Close();
            }, obj => obj is Window
            )); }
        }

        private Command _loadedCommand;
        public Command LoadedCommand {
            get { return _loadedCommand ?? (_loadedCommand = new Command(obj => {
                ShowNote();
            })); }
        }

        private Command _unloadedCommand;
        public Command UnloadedCommand {
            get { return _unloadedCommand ?? (_unloadedCommand = new Command(obj => {
                foreach (Window item in Application.Current.Windows) {
                    if (item is ItemNoteWindow && !(((ItemNoteWindow)item).DataContext as Note).IsTop)
                        WindowVisibilityBehaviour.SetIsVisible(item, false);
                }
            })); }
        }

        private Command _onPointCommand;
        public Command OnPointCommand {
            get { return _onPointCommand ?? (_onPointCommand = new Command(obj => {
                Tuple<object, Point> value = (Tuple<object, Point>)obj;
                //все же стоит воспользоваться паралельной записью либо все же проигнорить
                ((Note)value.Item1).PosX = (int)value.Item2.X;
                ((Note)value.Item1).PosY = (int)value.Item2.Y;
                Console.WriteLine(value.Item2);
            })); }
        }

        private Command _saveChangedCommand;
        public Command SaveChangedCommand {
            get { return _saveChangedCommand ?? (_saveChangedCommand = new Command(obj => {
                Note n = (Note)obj;
                using (DBContext context = new DBContext(false)) {
                    Note on = context.Notes.FirstOrDefault(x => x.ID == n.ID);
                    if (on != null)
                    {
                        on.PosX = n.PosX;
                        on.PosY = n.PosY;
                        on.Width = n.Width;
                        on.Height = n.Height;
                        context.SaveChanges();
                    }
                }

            }, obj => obj is Note)); }
        }

        private Command _returnNoteOnGroupCommand;
        public Command ReturnNoteOnGroupCommand {
            get { return _returnNoteOnGroupCommand ?? (_returnNoteOnGroupCommand = new Command(obj => {
                NoteGroup ng = (NoteGroup)obj;
                foreach (Window item in Application.Current.Windows)
                {
                    if (item is ItemNoteWindow && ((Note)item.DataContext).Group == ng.ID)
                    {

                        Note n = (Note)item.DataContext;
                        using (DBContext context = new DBContext(false))
                        {
                            Note on = context.Notes.FirstOrDefault(x => x.ID == n.ID);
                            if (on != null)
                            {
                                on.IsShowin = false;
                                context.SaveChanges();
                            }
                        }
                        WindowVisibilityBehaviour.SetIsVisible(item, false);
                    }
                }
                if (ng.ShowNotes)
                    ng.Refresh();

            }, obj => obj is NoteGroup)); }
        }

        private Command _returnAllNoteCommand;
        public Command ReturnAllNoteCommand {
            get { return _returnAllNoteCommand ?? (_returnAllNoteCommand = new Command(obj => {
                foreach (Window item in Application.Current.Windows)
                {
                    if (item is ItemNoteWindow)
                    {

                        Note n = (Note)item.DataContext;
                        using (DBContext context = new DBContext(false))
                        {
                            Note on = context.Notes.FirstOrDefault(x => x.ID == n.ID);
                            if (on != null)
                            {
                                on.IsShowin = false;
                                context.SaveChanges();
                            }
                        }
                        WindowVisibilityBehaviour.SetIsVisible(item, false);
                    }
                }
                foreach (var item in NotesGroups)
                    if (item.ShowNotes)
                        item.Refresh();
            })); }
        }

        private Command _createTaskByNoteCommand;
        public Command CreateTaskByNoteCommand {
            get {
                return _createTaskByNoteCommand ?? (_createTaskByNoteCommand = new Command(obj => {
                    Note note = (Note)((Tuple<object, object>)obj).Item1;
                    Window w = (Window)((Tuple<object, object>)obj).Item2;
                    this.State = TypeControl.Create;

                    FrameworkElementOpacityBehaviour.SetIsShow(w, false);
                    w.Opacity = 0;
                    TasksViewModel tvm = App.GetViewModel<TasksViewModel>();
                    using (DBContext context = new DBContext(false)) {
                        Task CurrentTask = Task.New();
                        CurrentTask.Caption = "Task by note";

                        CurrentTask.Text = note.Text;

                        CurrentTask.Creater = App.CurrentUser.ID;
                        CurrentTask.OpenDate = DateTime.Now;
                        CurrentTask.EndDate = CurrentTask.OpenDate.AddDays(1);
                        CurrentTask.IdProject = context.Projects.FirstOrDefault().ID;

                        tvm.SetCurrentTask(CurrentTask);

                        TaskControlWindow createWindow = new TaskControlWindow();
                        WindowPositionBehaviour.SetWindowPosition(createWindow, WindowPosition.None);
                        WindowVisibilityBehaviour.SetIsDialogVisible(createWindow, true);

                        if (createWindow.DialogResult == true) {
                            context.Tasks.AddObject(CurrentTask);
                            context.SaveChanges();
                        }
                        else
                            CurrentTask.Disposing(App.DBSettings.Connection.PathToResourceDB);

                        CurrentTask = null;
                    }

                    this.State = TypeControl.Normal;
                    FrameworkElementOpacityBehaviour.SetIsShow(w, true);
                }, obj => obj != null));
            }
        }

        #region Color commands
        private Command[] _colorsCommand;
        public Command[] ColorsCommand {
            get { return _colorsCommand ?? (_colorsCommand = new Command[] {
                //LightBlue
                new Command(obj => {
                    Color cr = Colors.LightBlue;
                    if(obj is Note) {
                        ((Note)obj).DIdColor = BitConverter.ToInt32 (new byte [] { cr.B, cr.G, cr.R, cr.A }, 0);
                        //не забыть про автосохранение
                    } else if(obj is NoteGroup) {
                        ((NoteGroup)obj).DIdColor = BitConverter.ToInt32 (new byte [] { cr.B, cr.G, cr.R, cr.A }, 0);
                    }
                }, obj => obj != null),
                //LightGreen
                new Command(obj => {
                    Color cr = Colors.LightGreen;
                    if(obj is Note) {
                        ((Note)obj).DIdColor = BitConverter.ToInt32 (new byte [] { cr.B, cr.G, cr.R, cr.A }, 0);
                        //не забыть про автосохранение
                    } else if(obj is NoteGroup) {
                        ((NoteGroup)obj).DIdColor = BitConverter.ToInt32 (new byte [] { cr.B, cr.G, cr.R, cr.A }, 0);
                    }
                }, obj => obj != null),
                //Pink
                new Command(obj => {
                    Color cr = Colors.LightPink;
                    if(obj is Note) {
                        ((Note)obj).DIdColor = BitConverter.ToInt32 (new byte [] { cr.B, cr.G, cr.R, cr.A }, 0);
                        //не забыть про автосохранение
                    } else if(obj is NoteGroup) {
                        ((NoteGroup)obj).DIdColor = BitConverter.ToInt32 (new byte [] { cr.B, cr.G, cr.R, cr.A }, 0);
                    }
                }, obj => obj != null),
                //White
                new Command(obj => {
                    Color cr = Colors.White;
                    if(obj is Note) {
                        ((Note)obj).DIdColor = BitConverter.ToInt32 (new byte [] { cr.B, cr.G, cr.R, cr.A }, 0);
                        //не забыть про автосохранение
                    } else if(obj is NoteGroup) {
                        ((NoteGroup)obj).DIdColor = BitConverter.ToInt32 (new byte [] { cr.B, cr.G, cr.R, cr.A }, 0);
                    }
                }, obj => obj != null),
                //LightYelloy
                new Command(obj => {
                    Color cr = Colors.LightYellow;
                    if(obj is Note) {
                        ((Note)obj).DIdColor = BitConverter.ToInt32 (new byte [] { cr.B, cr.G, cr.R, cr.A }, 0);
                        //не забыть про автосохранение
                    } else if(obj is NoteGroup) {
                        ((NoteGroup)obj).DIdColor = BitConverter.ToInt32 (new byte [] { cr.B, cr.G, cr.R, cr.A }, 0);
                    }
                }, obj => obj != null),
                //LightGrey
                new Command(obj => {
                    Color cr = Colors.LightGray;
                    if(obj is Note) {
                        ((Note)obj).DIdColor = BitConverter.ToInt32 (new byte [] { cr.B, cr.G, cr.R, cr.A }, 0);
                        //не забыть про автосохранение
                    } else if(obj is NoteGroup) {
                        ((NoteGroup)obj).DIdColor = BitConverter.ToInt32 (new byte [] { cr.B, cr.G, cr.R, cr.A }, 0);
                    }
                }, obj => obj != null)
            }); }
        }
        #endregion
        #endregion

        #region Members

        private string _searchText;
        [ReportStat]
        public string SearchText {
            get { return _searchText; }
            set {
                _searchText = value;
                using (DBContext context = new DBContext(false))
                    NotesGroupsCollectionView.Filter = new Predicate<object>((x) => ((NoteGroup)x).Caption.ToUpper().Contains(value.Replace(" ", "").ToUpper()) | ((NoteGroup)x).GetContainsTextOnNotes(context, value.Replace(" ", "").ToUpper()));
                this.SendPropertyChanged(nameof(SearchText));
            }
        }

        private ObservableCollection<NoteGroup> _notesGroups;
        [ReportStat]
        public ObservableCollection<NoteGroup> NotesGroups {
            get { return _notesGroups ?? (_notesGroups = new ObservableCollection<NoteGroup>(GetNotesGroups())); }
        }

        private ICollectionView _notesGroupsCollectionView;
        [ReportType(typeof(NoteGroup))]
        [ReportStat]
        public ICollectionView NotesGroupsCollectionView {
            get { return _notesGroupsCollectionView ?? (_notesGroupsCollectionView = CollectionViewSource.GetDefaultView(NotesGroups)); }
        }

        private string _searchTextMenu;
        public string SearchTextMenu {
            get { return _searchTextMenu; }
            set {
                _searchTextMenu = value;
                var h = value.ToUpper();
                if (NotesGroupsMenuCollectionView.SourceCollection.Cast<Tuple<object, object>>().Count(x => ((string)x.Item1).ToUpper().Contains(h)) > 0)
                    NotesGroupsMenuCollectionView.Filter = new Predicate<object>((x) => ((string)((Tuple<object, object>)x).Item1).ToUpper().Contains(h));
                else
                    NotesGroupsMenuCollectionView.Filter = new Predicate<object>((x) => ((int)((Tuple<object, object>)x).Item2) == -1);
                this.SendPropertyChanged(nameof(SearchTextMenu));
            }
        }

        private ICollectionView _notesGroupsMenuCollectionView;

        public ICollectionView NotesGroupsMenuCollectionView {
            get { return _notesGroupsMenuCollectionView ?? (_notesGroupsMenuCollectionView = CollectionViewSource.GetDefaultView(NoteGoupId())); }
        }

        private NoteGroup _selectedGroup;
        public NoteGroup SelectedGroup {
            get { return _selectedGroup; }
            set {
                _selectedGroup = null;
                this.SendPropertyChanged(nameof(SelectedGroup));
                _selectedGroup = value;
                this.SendPropertyChanged(nameof(SelectedGroup));
            }
        }
        #endregion

        #region Helps
        protected IEnumerable<NoteGroup> GetNotesGroups() {
            IEnumerable<NoteGroup> result = null;
            using (DBContext context = new DBContext(false)) {
                result = context.NoteGroups.Where(x => x.IDUser == App.CurrentUser.ID).ToArray();
            }
            return result;
        }

        public static void ShowNote() {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                using (DBContext context = new DBContext(false))
                {
                    foreach (var item in context.Notes.Where(x => x.Owner == App.CurrentUser.ID && x.IsShowin && !x.IsTop))
                    {
                        App.BeginInvoke(() =>
                        {
                            ItemNoteWindow w = new ItemNoteWindow();
                            w.DataContext = item;
                            WindowVisibilityBehaviour.SetCloseByHidden(w, true);
                            WindowVisibilityBehaviour.SetIsVisible(w, true);
                        });
                    }
                }
            });
        }

        public static void ShowTopWindow() {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                using (DBContext context = new DBContext(false))
                {
                    foreach (var item in context.Notes.Where(x => x.Owner == App.CurrentUser.ID && x.IsShowin && x.IsTop))
                    {
                        App.BeginInvoke(() =>
                        {
                            ItemNoteWindow w = new ItemNoteWindow();
                            w.DataContext = item;
                            WindowVisibilityBehaviour.SetCloseByHidden(w, true);
                            WindowVisibilityBehaviour.SetIsVisible(w, true);
                        });
                    }
                }
            });
        }

        public static void HideAllNote() {
            foreach (Window item in Application.Current.Windows)
            {
                if (item is ItemNoteWindow) {

                    Note n = (Note)item.DataContext;
                    using (DBContext context = new DBContext(false))
                    {
                        Note on = context.Notes.FirstOrDefault(x => x.ID == n.ID);
                        if (on != null)
                        {
                            on.PosX = n.PosX;
                            on.PosY = n.PosY;
                            on.Width = n.Width;
                            on.Height = n.Height;
                            context.SaveChanges();
                        }
                    }
                    WindowVisibilityBehaviour.SetIsVisible(item, false);
                }
            }
        }

        public void CreateNote() {
            this.State = TypeControl.Create;

            NoteGroup ng = NotesGroups.FirstOrDefault(x => x.ShowNotes);

            using (DBContext context = new DBContext(false)) {
                if (ng == null) {
                    if (NotesGroups.Count > 0)
                        ng = NotesGroups.First();
                    else {
                        ng = NoteGroup.New();
                        Color cr = Colors.LightBlue;
                        ng.IDUser = App.CurrentUser.ID;
                        ng.IDColor = BitConverter.ToInt32(new byte[] { cr.B, cr.G, cr.R, cr.A }, 0);
                        context.NoteGroups.AddObject(ng);

                        NotesGroups.Add(ng);

                        context.SaveChanges();
                    }
                }

                if (ng != null) {
                    Note newN = new Note();
                    FlowDocument fd = new FlowDocument { AllowDrop = true, PageWidth = 130, PageHeight = 130 };
                    fd.Blocks.Add(new Paragraph(new Run("Note")));
                    newN.Text = XamlWriter.Save(fd);
                    newN.Owner = App.CurrentUser.ID;
                    newN.Group = ng.ID;
                    newN.CreateDate = DateTime.Now;
                    newN.Width = 130;
                    newN.Height = 130;
                    newN.PosX = (int)(SystemParameters.PrimaryScreenWidth - 130) / 2;
                    newN.PosY = (int)(SystemParameters.PrimaryScreenHeight - 130) / 2;
                    newN.IDColor = ng.IDColor;
                    newN.IsShowin = true;
                    newN.IsTop = true;

                    context.Notes.AddObject(newN);

                    context.SaveChanges();
                    //ng.AddNewNote(newN);

                    ItemNoteWindow w = new ItemNoteWindow();
                    w.DataContext = newN;
                    WindowVisibilityBehaviour.SetCloseByHidden(w, true);
                    WindowVisibilityBehaviour.SetIsVisible(w, true);
                }
            }

            this.State = TypeControl.Normal;
        }

        public void CreateNoteOfClipBoard() {
            this.State = TypeControl.Create;

            NoteGroup ng = NotesGroups.FirstOrDefault(x => x.ShowNotes);

            using (DBContext context = new DBContext(false)) {
                if (ng == null) {
                    if (NotesGroups.Count > 0)
                        ng = NotesGroups.First();
                    else {
                        ng = NoteGroup.New();
                        Color cr = Colors.LightBlue;
                        ng.IDUser = App.CurrentUser.ID;
                        ng.IDColor = BitConverter.ToInt32(new byte[] { cr.B, cr.G, cr.R, cr.A }, 0);
                        context.NoteGroups.AddObject(ng);

                        NotesGroups.Add(ng);

                        context.SaveChanges();
                    }
                }

                if (ng != null) {
                    Note newN = new Note();
                    FlowDocument fd = new FlowDocument { AllowDrop = true, PageWidth = 130, PageHeight = 130 };
                    fd.Blocks.Add(new Paragraph(new Run(Clipboard.GetText())));
                    newN.Text = XamlWriter.Save(fd);
                    newN.Owner = App.CurrentUser.ID;
                    newN.Group = ng.ID;
                    newN.CreateDate = DateTime.Now;
                    newN.Width = 130;
                    newN.Height = 130;
                    newN.PosX = (int)(SystemParameters.PrimaryScreenWidth - 130) / 2;
                    newN.PosY = (int)(SystemParameters.PrimaryScreenHeight - 130) / 2;
                    newN.IDColor = ng.IDColor;
                    newN.IsShowin = true;
                    newN.IsTop = true;

                    context.Notes.AddObject(newN);

                    context.SaveChanges();
                    //ng.AddNewNote(newN);

                    ItemNoteWindow w = new ItemNoteWindow();
                    w.DataContext = newN;
                    WindowVisibilityBehaviour.SetCloseByHidden(w, true);
                    WindowVisibilityBehaviour.SetIsVisible(w, true);
                }
            }

            this.State = TypeControl.Normal;
        }

        public IEnumerable<Tuple<object, object>> NoteGoupId() {
            var result = GetNotesGroups().Select(x => new Tuple<object, object>(x.Caption, x.ID)).ToList();
            result.Add(new Tuple<object, object>("", -1));
            return result;
        }
        #endregion
    }
}
