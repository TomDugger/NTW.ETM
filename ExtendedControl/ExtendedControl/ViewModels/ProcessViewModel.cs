using ExtendedControl.Views.ControlWindow;
using NTW.Attrebute;
using NTW.Controls.Behaviours;
using NTW.Controls.ViewModels;
using NTW.Core;
using NTW.Data;
using NTW.Data.Context;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Media;

namespace ExtendedControl.ViewModels
{
    [ReportStat]
    public class ProcessViewModel : ControlViewModel, IDropViewModel
    {
        #region Commands
        private Command _createNewProcessCommand;
        public Command CreateNewProcessCommand {
            get { return _createNewProcessCommand ?? (_createNewProcessCommand = new Command(obj => {
                this.State = TypeControl.Create;
                using (DBContext context = new DBContext(false))
                {
                    CurrentStartProcess = StartProcess.New();
                    ProcessControlWindow pcw = new ProcessControlWindow();
                    this.IsFolderPath = false;
                    //pcw.Owner = (Window)obj;
                    //WindowPositionBehaviour.SetWindowPosition(pcw, WindowPositionBehaviour.GetWindowPosition((DependencyObject)obj));
                    WindowPositionBehaviour.SetWindowPosition(pcw, WindowPosition.None);
                    WindowVisibilityBehaviour.SetIsDialogVisible(pcw, true);
                    //ожидаем закрытия окна создания
                    if (pcw.DialogResult == true)
                    {
                        CurrentStartProcess.IdUser = App.CurrentUser.ID;
                        context.StartProcesses.AddObject(CurrentStartProcess);
                        context.SaveChanges();

                        _processCollection.Add(CurrentStartProcess);
                    }
                    CurrentStartProcess = null;
                }
                this.State = TypeControl.Normal;
            }, obj => obj is Window)); }
        }

        private Command _editProcessCommand;
        public Command EditProcessCommand {
            get { return _editProcessCommand ?? (_editProcessCommand = new Command(obj => {
                this.State = TypeControl.Edit;
                using (DBContext context = new DBContext(false))
                {
                    //с изменением придется помудрить
                    CurrentStartProcess = context.StartProcesses.FirstOrDefault(x=> x.ID == ((StartProcess)obj).ID);
                    FileAttributes attr = File.GetAttributes(CurrentStartProcess.PathToApp);
                    this.IsFolderPath = (attr & FileAttributes.Directory) == FileAttributes.Directory;
                    ProcessControlWindow pcw = new ProcessControlWindow();
                    WindowPositionBehaviour.SetWindowPosition(pcw, WindowPosition.None);
                    WindowVisibilityBehaviour.SetIsDialogVisible(pcw, true);

                    context.SaveChanges();
                    StartProcess sp = Process.FirstOrDefault(x => x.ID == CurrentStartProcess.ID);
                    int index = Process.IndexOf(sp);
                    Process.RemoveAt(index);
                    Process.Insert(index, CurrentStartProcess);

                    CurrentStartProcess = null;
                }
                this.State = TypeControl.Normal;
            }, obj => obj is StartProcess)); }
        }

        private Command _copyProcessCommand;
        public Command CopyProcessCommand {
            get {
                return _copyProcessCommand ?? (_copyProcessCommand = new Command(obj => {
                    this.State = TypeControl.Edit;
                    var copyElement = (StartProcess)obj;
                    using (DBContext context = new DBContext(false)) {
                        CurrentStartProcess = copyElement.Copy();
                        ProcessControlWindow pcw = new ProcessControlWindow();
                        this.IsFolderPath = false;
                        //pcw.Owner = (Window)obj;
                        //WindowPositionBehaviour.SetWindowPosition(pcw, WindowPositionBehaviour.GetWindowPosition((DependencyObject)obj));
                        WindowPositionBehaviour.SetWindowPosition(pcw, WindowPosition.None);
                        WindowVisibilityBehaviour.SetIsDialogVisible(pcw, true);
                        //ожидаем закрытия окна создания
                        if (pcw.DialogResult == true) {
                            CurrentStartProcess.IdUser = App.CurrentUser.ID;
                            context.StartProcesses.AddObject(CurrentStartProcess);
                            context.SaveChanges();

                            _processCollection.Add(CurrentStartProcess);
                        }
                        CurrentStartProcess = null;
                    }
                    this.State = TypeControl.Normal;
                }, obj => obj is StartProcess));
            }
        }

        private Command _fineControlCommand;
        public Command FineControlCommand {
            get { return _fineControlCommand ?? (_fineControlCommand = new Command(obj => {
                Window w = (Window)obj;
                if (CurrentStartProcess.Errors.Count == 0) {
                    if (CurrentStartProcess.ActualPath()) {
                        w.DialogResult = true;
                        WindowVisibilityBehaviour.SetIsDialogVisible(w, false);
                    }
                    else
                        WindowMessageBehaviour.SetMessage(w, App.GetString("ProcessControlStringNotIsath"), Colors.Maroon, Colors.White);
                }
                else {
                    foreach (var item in CurrentStartProcess.Errors) {
                        WindowMessageBehaviour.SetMessage(w, App.GetString(item.Value), Colors.Maroon, Colors.White);
                    }
                }
            }, obj => CurrentStartProcess != null && obj is Window)); }
        }

        private Command _deleteProcessCommand;
        public Command DeleteProcessCommand {
            get { return _deleteProcessCommand ?? (_deleteProcessCommand = new Command(obj => {
                StartProcess sp = (StartProcess)obj;
                using (DBContext context = new DBContext(false))
                {
                    context.StartProcesses.DeleteObject(context.StartProcesses.FirstOrDefault(x => x.ID == sp.ID));
                    context.SaveChanges();

                    Process.Remove(sp);
                }
            }
            )); }
        }

        private Command _exictPathToAppCommand;
        public Command ExictPathToAppCommand {
            get { return _exictPathToAppCommand ?? (_exictPathToAppCommand = new Command(obj => {
                if (!IsFolderPath) {
                    OpenFileDialog ofd = new OpenFileDialog();
                    switch (ofd.ShowDialog()) {
                        case DialogResult.OK:
                            CurrentStartProcess.PathToApp = ofd.FileName;
                            CurrentStartProcess.Caption = Path.GetFileName(ofd.FileName);
                            break;
                    }
                }
                else {
                    FolderBrowserDialog fbd = new FolderBrowserDialog();
                    switch (fbd.ShowDialog()) {
                        case DialogResult.OK:
                            CurrentStartProcess.PathToApp = fbd.SelectedPath;
                            CurrentStartProcess.Caption = Path.GetFileName(fbd.SelectedPath);
                            break;
                    }
                }
            }, obj => CurrentStartProcess != null)); }
        }

        private Command _executionAppCommand;
        public Command ExecutionAppCommand {
            get { return _executionAppCommand ?? (_executionAppCommand = new Command(obj =>
            {
                //запуск на исполнение
                StartProcess sp = (StartProcess)obj;
                if (File.Exists(sp.PathToApp) || Directory.Exists(sp.PathToApp)) {
                    System.Diagnostics.Process prc = System.Diagnostics.Process.Start(sp.PathToApp, "");
                }
                //как вариант, понадобится закрыть панель после запуска
                //стоит подумать
            }, obj => obj is StartProcess)); }
        }

        #region Unload
        private Command _unloadedCommand;
        public Command UnloadedCommand {
            get {
                return _unloadedCommand ?? (_unloadedCommand = new Command(obj => {
                    _processCollection = null;
                }));
            }
        }
        #endregion
        #endregion

        #region Members
        private StartProcess _currentStartProcess = null;
        public StartProcess CurrentStartProcess {
            get { return _currentStartProcess; }
            protected set { _currentStartProcess = value; this.SendPropertyChanged("CurrentStartProcess"); }
        }

        private ObservableCollection<StartProcess> _processCollection;
        [ReportStat]
        public ObservableCollection<StartProcess> Process {
            get { return _processCollection ?? (_processCollection = new ObservableCollection<StartProcess>(GetProcess())); }
        }

        private ICollectionView _processCollectionView;
        [ReportType(typeof(StartProcess))]
        [ReportStat]
        public ICollectionView ProcessCollectionView {
            get { return _processCollectionView ?? (_processCollectionView = GetProcessCollectionView()); }
        }

        private string _searchText;
        [ReportStat]
        public string SearchText {
            get { return _searchText; }
            set { _searchText = value;
                using (DBContext context = new DBContext(false)) {
                    string h = value.ToUpper();
                    ProcessCollectionView.Filter = new Predicate<object>((x) => ((StartProcess)x).Caption.ToUpper().Contains(h));
                }
                this.SendPropertyChanged(nameof(SearchText));
            }
        }

        private bool _isFolderPath;
        public bool IsFolderPath {
            get { return _isFolderPath; }
            set { _isFolderPath = value; this.SendPropertyChanged(nameof(IsFolderPath)); }
        }

        #endregion

        #region Helps
        protected IEnumerable<StartProcess> GetProcess() {
            IEnumerable<StartProcess> result = null;
            using (DBContext context = new DBContext(false))
            {
                result = context.StartProcesses.Where(x => x.IdUser == App.CurrentUser.ID).ToArray();
            }
            return result;
        }
        protected ICollectionView GetProcessCollectionView() {
            return CollectionViewSource.GetDefaultView(Process);
        }

        public void CreateProcess() {
            this.State = TypeControl.Create;
            using (DBContext context = new DBContext(false)) {
                CurrentStartProcess = StartProcess.New();
                ProcessControlWindow pcw = new ProcessControlWindow();
                this.IsFolderPath = false;
                WindowPositionBehaviour.SetWindowPosition(pcw, WindowPosition.None);
                WindowVisibilityBehaviour.SetIsDialogVisible(pcw, true);
                if (pcw.DialogResult == true) {
                    CurrentStartProcess.IdUser = App.CurrentUser.ID;
                    context.StartProcesses.AddObject(CurrentStartProcess);
                    context.SaveChanges();

                    _processCollection.Add(CurrentStartProcess);
                }
                CurrentStartProcess = null;
            }
            this.State = TypeControl.Normal;
        }

        public void CreateProcessOnClipboard() {

        }

        public void SetValues(Type typeContent, IEnumerable values) {
            this.State = TypeControl.Create;
            using (DBContext context = new DBContext(false))
            {
                foreach (var item in values)
                {
                    CurrentStartProcess = StartProcess.New();
                    CurrentStartProcess.IdUser = App.CurrentUser.ID;
                    CurrentStartProcess.PathToApp = item.ToString();
                    CurrentStartProcess.Caption = Path.GetFileNameWithoutExtension(item.ToString());
                    context.StartProcesses.AddObject(CurrentStartProcess);
                    context.SaveChanges();

                    Process.Add(CurrentStartProcess);
                    CurrentStartProcess = null;
                }
            }
            this.State = TypeControl.Normal;
        }

        public void SetValues(UIElement element, IEnumerable values) {
        }
        #endregion
    }
}
