using NTW.Data.Connections;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace NTW.Data.Models
{
    [Serializable]
    public class DBSettings
    {
        #region Members
        private IConnection _connection;
        public IConnection Connection {
            get { return _connection; }
            set { _connection = value; }
        }
        #endregion

        #region Helps
        public void Save()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(Path.Combine(Environment.CurrentDirectory, "dbsettings.cpf"), FileMode.Create))
            {
                formatter.Serialize(fs, this);
            }
        }

        public static bool ExistFileSettings()
        {
            return File.Exists(Path.Combine(Environment.CurrentDirectory, "dbsettings.cpf"));
        }

        public static DBSettings Load(DBSettings defaultSettings = null)
        {
            DBSettings settings = defaultSettings;
            if (ExistFileSettings()) {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream fs = new FileStream(Path.Combine(Environment.CurrentDirectory, "dbsettings.cpf"), FileMode.Open)) {
                    settings = (DBSettings)formatter.Deserialize(fs);
                }
            }

            return settings;
        }

        public DBSettings Copy() {
            DBSettings result = new DBSettings();
            result.Connection = this.Connection;
            return result;
        }
        #endregion
    }
}
