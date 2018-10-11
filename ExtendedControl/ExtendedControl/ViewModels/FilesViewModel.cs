using NTW.Controls.Behaviours;
using NTW.Controls.ViewModels;
using NTW.Core;
using NTW.Data.Context;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace ExtendedControl.ViewModels
{
    public class FilesViewModel:ViewModel, IDropViewModel
    {
        #region Constructors
        public FilesViewModel(IEnumerable<TaskFile> filesList) {
            _files = new ObservableCollection<TaskFile>(filesList);
        }
        #endregion

        #region Commands
        private Command _acceptedControlFilesCommand;
        public Command AcceptedControlFilesCommand {
            get {
                return _acceptedControlFilesCommand ?? (_acceptedControlFilesCommand = new Command(obj => {
                    //не даем выйти пака активено состояние загрузки
                    Window w = (Window)obj;
                    WindowVisibilityBehaviour.SetIsDialogVisible(w, false);
                }, obj => obj is Window));
            }
        }

        private Command _removeFileOnListCommand;
        public Command RemoveFileOnListCommand {
            get {
                return _removeFileOnListCommand ?? (_removeFileOnListCommand = new Command(obj => {
                    Files.Remove(((TaskFile)obj).Disposing(App.DBSettings.Connection.PathToResourceDB));
                }, obj => obj is TaskFile));
            }
        }


        #endregion

        #region Members
        private ObservableCollection<TaskFile> _files;
        public ObservableCollection<TaskFile> Files {
            get { return _files; }
        }

        public void SetValues(Type typeContent, IEnumerable values)
        {
            //ловим формат и список данных
            if (values != null)
                foreach (var item in values)
                {
                    //формируем алгоритм загрузки файла и отображения последовательности загрузки
                    Files.Add(new TaskFile { OriginalName = Path.GetFileName(item.ToString()), StoreName = Guid.NewGuid().ToString() }.Inicial(App.DBSettings.Connection.PathToResourceDB, item.ToString()));
                }
        }

        public void SetValues(UIElement element, IEnumerable values) { }
        #endregion

        #region Helps

        #endregion
    }
}
