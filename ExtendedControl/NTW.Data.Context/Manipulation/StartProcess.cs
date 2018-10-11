using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace NTW.Data.Context
{
    public partial class StartProcess: IDataErrorInfo
    {
        public static StartProcess New() {
            StartProcess result = new StartProcess();
            result.Caption = "Process";
            result.PathToApp = "";
            return result;
        }

        #region Members
        public bool IsExist {
            get { return File.Exists(this.PathToApp) || Directory.Exists(this.PathToApp); }
        }
        #endregion

        #region Members IDataErrorInfo
        private Dictionary<string, string> _errors = new Dictionary<string, string>();
        public Dictionary<string, string> Errors {
            get { return _errors ?? (_errors = new Dictionary<string, string>()); }
        }

        public string Error { get { return string.Join(Environment.NewLine, _errors.Select(x => x.Value)); } }

        public bool HasError { get { return _errors.Count == 0 ? false : true; } }

        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;
                switch (columnName)
                {
                    case nameof(this.Caption):
                        if (this.Caption == null || this.Caption.Length == 0)
                            error = _errors[nameof(this.Caption)] = "ProcessControlCaptionIsNotNull";
                        else
                            _errors.Remove(nameof(this.Caption));
                        break;
                    case nameof(this.PathToApp):
                        if (this.PathToApp == null || this.PathToApp.Length == 0)
                            error = _errors[nameof(this.PathToApp)] = "ProcessControlPathIsNotNull";
                        else
                            _errors.Remove(nameof(this.PathToApp));
                        break;
                }
                this.OnPropertyChanged(nameof(Error));
                this.OnPropertyChanged(nameof(Errors));
                this.OnPropertyChanged(nameof(HasError));
                return error;
            }
        }
        #endregion

        #region Helps
        public bool ActualPath() {
            bool result = false;
            this.OnPropertyChanged(nameof(IsExist));
            result = IsDirectory(this.PathToApp) | IsFile(this.PathToApp);
            return result;
        }

        private bool IsDirectory(string path) {
            bool isDirectory = false;
            try {
                System.IO.FileAttributes fa = System.IO.File.GetAttributes(path);
                if ((fa & System.IO.FileAttributes.Directory) != 0) {
                    isDirectory = true;
                }
            }
            catch { }
            return isDirectory;
        }

        private bool IsFile(string path) {
            bool isFile = false;
            try {
                System.IO.FileAttributes fa = System.IO.File.GetAttributes(path);
                isFile = true;
            }
            catch { }
            return isFile;
        }

        public StartProcess Copy() {
            StartProcess result = new StartProcess();
            result.Caption = "Copy_" + this.Caption;
            result.PathToApp = this.PathToApp;
            return result;
        }
        #endregion
    }
}
