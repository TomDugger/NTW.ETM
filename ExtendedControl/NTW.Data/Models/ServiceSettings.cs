using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace NTW.Data.Models
{
    [Serializable]
    public class ServiceSettings
    {
        #region Members
        private string _ipTimeServer = "127.0.0.1";
        public string IPTimeServer {
            get { return _ipTimeServer; }
            set { _ipTimeServer = value; }
        }
        #endregion

        #region Helps
        public void Save()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(Path.Combine(Environment.CurrentDirectory, "ssettings.cpf"), FileMode.Create))
            {
                formatter.Serialize(fs, this);
            }
        }

        public static bool ExistFileSettings()
        {
            return File.Exists(Path.Combine(Environment.CurrentDirectory, "ssettings.cpf"));
        }

        public static ServiceSettings Load()
        {
            ServiceSettings settings = null;
            if (ExistFileSettings())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream fs = new FileStream(Path.Combine(Environment.CurrentDirectory, "ssettings.cpf"), FileMode.Open))
                {
                    settings = (ServiceSettings)formatter.Deserialize(fs);
                }
            }
            else
                settings = new ServiceSettings();
            return settings;
        }
        #endregion
    }
}
