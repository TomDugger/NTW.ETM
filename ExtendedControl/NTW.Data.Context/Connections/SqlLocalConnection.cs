using NTW.Data.Connections;
using NTW.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.EntityClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace NTW.Data.Context.Connections {
    [Serializable]
    public class SqlLocalConnection: IConnection, INotifyPropertyChanged, IDataErrorInfo {
        private string _provider = "System.Data.SqlClient";
        public string Provider { get { return _provider; } set { _provider = value; } }

        private string _pathToFile = Path.Combine(Environment.CurrentDirectory, "Data",  "Task_DB.mdf");
        public string PathToFile {
            get { return _pathToFile; }
            set { _pathToFile = value; this.SendPropertyChanged(nameof(PathToFile)); }
        }

        public string GetConnectionString() {
            var entityBuilder = new EntityConnectionStringBuilder();

            entityBuilder.Provider = _provider;
            entityBuilder.ProviderConnectionString = "data source = (LocalDB)\\MSSQLLocalDB; attachdbfilename =" + PathToFile + "; integrated security = True; MultipleActiveResultSets = True; App = EntityFramework";
            entityBuilder.Metadata = @"res://*/DB.csdl|res://*/DB.ssdl|res://*/DB.msl";

            return entityBuilder.ToString();
        }

        [field: NonSerialized]
        public IEnumerable<string> NamesServers {
            get { return null; }
        }

        [field: NonSerialized]
        public IEnumerable<string> NamesDB {
            get { return null; }
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;


        protected void SendPropertyChanged(string propertyName = "") {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _pathToResourceDB = Environment.CurrentDirectory;

        public string PathToResourceDB {
            get { return _pathToResourceDB; }
            set { _pathToResourceDB = value; this.SendPropertyChanged(nameof(PathToResourceDB)); ((DBSettings)Application.Current.Resources["DBSettings"]).Save(); }
        }

        #region Members IDataErrorInfo
        private Dictionary<string, string> _erorrs = new Dictionary<string, string>();

        public string Error { get { return string.Join(Environment.NewLine, _erorrs.Select(x => x.Value)); } }

        public bool HasError { get { return _erorrs.Count == 0 ? false : true; } }

        public string this[string columnName] {
            get {
                string error = string.Empty;
                switch (columnName) {
                    case nameof(PathToFile):
                        if (PathToFile == null || PathToFile.Length == 0)
                            error = _erorrs[nameof(PathToFile)] = "Filelt Datasource value by not null or empty";
                        else
                            _erorrs.Remove(nameof(PathToFile));
                        break;
                }
                this.SendPropertyChanged("Error");
                this.SendPropertyChanged("HasError");
                return error;
            }
        }
        #endregion
    }
}
