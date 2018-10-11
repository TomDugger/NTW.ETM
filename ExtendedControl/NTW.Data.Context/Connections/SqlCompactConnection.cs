using NTW.Data.Connections;
using NTW.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.EntityClient;
using System.Linq;
using System.Text;
using System.Windows;

namespace NTW.Data.Context.Connections {
    [Serializable]
    public class SqlCompactConnection: IConnection, INotifyPropertyChanged, IDataErrorInfo {

        private string _provider = "System.Data.SqlServerCe.3.5";
        public string Provider { get { return _provider; } set { _provider = value; } }

        private string _dataSource = Environment.CurrentDirectory;
        public string DataSource {
            get { return _dataSource; }
            set { _dataSource = value; this.SendPropertyChanged("DataSource"); }
        }

        public string GetConnectionString() {
            var entityBuilder = new EntityConnectionStringBuilder();

            entityBuilder.Provider = _provider;
            entityBuilder.ProviderConnectionString = "Data Source=" + DataSource;
            entityBuilder.Metadata = @"res://*/DB.csdl|res://*/DB.ssdl|res://*/DB.msl";

            return entityBuilder.ToString();
        }

        #region Members IDataErrorInfo
        private Dictionary<string, string> _erorrs = new Dictionary<string, string>();

        public string Error { get { return string.Join(Environment.NewLine, _erorrs.Select(x => x.Value)); } }

        public bool HasError { get { return _erorrs.Count == 0 ? false : true; } }

        private string _pathToResourceDB = Environment.CurrentDirectory;

        public string PathToResourceDB {
            get { return _pathToResourceDB; }
            set { _pathToResourceDB = value; this.SendPropertyChanged(nameof(PathToResourceDB)); ((DBSettings)Application.Current.Resources["DBSettings"]).Save(); }
        }

        public IEnumerable<string> NamesServers => null;

        public IEnumerable<string> NamesDB => null;

        public string this[string columnName] {
            get {
                string error = string.Empty;
                switch (columnName) {
                    case nameof(DataSource):
                        if (DataSource == null || DataSource.Length == 0)
                            error = _erorrs[nameof(DataSource)] = "Filelt Datasource value by not null or empty";
                        else
                            _erorrs.Remove(nameof(DataSource));
                        break;
                }
                this.SendPropertyChanged("Error");
                this.SendPropertyChanged("HasError");
                return error;
            }
        }
        #endregion

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;


        protected void SendPropertyChanged(string propertyName = "") {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
