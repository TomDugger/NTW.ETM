using NTW.Data.Connections;
using NTW.Data.Models;
using NTW.Moduls.SQL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows;

namespace NTW.Data.Context.Connections
{
    [Serializable]
    public class SqlConnection: IConnection, INotifyPropertyChanged, IDataErrorInfo
    {
        private string _provider = "System.Data.SqlClient";
        public string Provider { get { return _provider; } set { _provider = value; } }

        private string _dataSource = "192.168.3.222";
        public string DataSource
        {
            get { return _dataSource; }
            set { _dataSource = value; this.SendPropertyChanged("NamesDB"); }
        }

        private string _initialCatalog = "TaskMenu_DB12";
        public string InitialCatalog { get { return _initialCatalog; } set { _initialCatalog = value; } }

        private SqlSecurityInfo _security = new SqlSecurityInfo() { User = "sa", Password = "Nts123" };
        public SqlSecurityInfo Security { get { return _security; } set { _security = value; } }

        public string GetConnectionString()
        {
            var entityBuilder = new EntityConnectionStringBuilder();

            entityBuilder.Provider = _provider;
            entityBuilder.ProviderConnectionString = "Data Source=" + DataSource + ";Initial Catalog=" + InitialCatalog + Security.ToString();
            entityBuilder.Metadata = @"res://*/DB.csdl|res://*/DB.ssdl|res://*/DB.msl";

            return entityBuilder.ToString();
        }

        [Serializable]
        public class SqlSecurityInfo
        {
            public string User { get; set; }
            public string Password { get; set; }

            public override string ToString()
            {
                string result = "";
                if (User != null && User != string.Empty)
                    result = ";Persist Security Info=True;User ID=" + User + ";Password=" + Password;
                else result = ";Integrated Security=True;MultipleActiveResultSets=True";
                return result;
            }

            public SqlSecurityInfo Copy() {
                var result = new SqlSecurityInfo();
                result.User = this.User;
                result.Password = this.Password;
                return result;
            }
        }

        private string _pathToResourceDB = Environment.CurrentDirectory;

        public string PathToResourceDB {
            get { return _pathToResourceDB; }
            set { _pathToResourceDB = value; this.SendPropertyChanged(nameof(PathToResourceDB)); ((DBSettings)Application.Current.Resources["DBSettings"]).Save(); }
        }

        [field: NonSerialized]
        public IEnumerable<string> NamesServers {
            get { return SqlLocator.GetServers(); }
        }

        [field: NonSerialized]
        public IEnumerable<string> NamesDB {
            get {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = this.DataSource;
                builder.UserID = Security.User;
                builder.Password = Security.Password;
                builder.IntegratedSecurity = true;
                return SqlLocator.GetBDNames(builder);
            }
        }

        public IConnection Copy() {
            var result = new SqlConnection();
            result.Provider = this.Provider;
            result.DataSource = this.DataSource;
            result.InitialCatalog = this.InitialCatalog;
            result.Security = this.Security.Copy();
            result.PathToResourceDB = this.PathToResourceDB;
            return result;
        }

        #region Members IDataErrorInfo
        private Dictionary<string, string> _erorrs = new Dictionary<string, string>();

        public string Error { get { return string.Join(Environment.NewLine, _erorrs.Select(x => x.Value)); } }

        public bool HasError { get { return _erorrs.Count == 0 ? false : true; } }

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
                    case nameof(InitialCatalog):
                        if (InitialCatalog == null || InitialCatalog.Length == 0)
                            error = _erorrs[nameof(InitialCatalog)] = "Filelt InitialCatalog value by not null or empty";
                        else
                            _erorrs.Remove(nameof(InitialCatalog));
                        break;
                }
                this.SendPropertyChanged("Error");
                this.SendPropertyChanged("HasError");
                return error;
            }
        }
        #endregion

        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;


        protected void SendPropertyChanged(string propertyName = "") {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
