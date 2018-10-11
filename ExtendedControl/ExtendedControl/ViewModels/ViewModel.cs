using NTW.Controls.Behaviours;
using NTW.Core;
using NTW.Data.Context;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Windows;

namespace ExtendedControl.ViewModels
{
    public abstract class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void SendPropertyChanged(string propertyName = "") {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            
        }

        public bool HasError { get; set; }
        public virtual string Error { get; }

        public User CurrentUser {
            get { return App.CurrentUser; }
        }

        #region Default commands
        private Command _closeWindowCommand;
        public virtual Command CloseWindowCommand {
            get { return _closeWindowCommand ?? (_closeWindowCommand = new Command(obj => {
                ((Window)obj).DialogResult = false;
                WindowVisibilityBehaviour.SetIsDialogVisible((Window)obj, false);
                WindowVisibilityBehaviour.SetIsVisible((Window)obj, false);
            }, obj => obj is Window)); }
        }
        #endregion
    }
}
