using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace NTW.Data.Models
{
    [Serializable]
    public class UserSettings
    {
        #region Members
        private bool _autoEnter = false;
        public bool AutoEnter {
            get { return _autoEnter; }
            set { _autoEnter = value; }
        }

        private string _userName;
        public string UserName {
            get { return _userName; }
            set { _userName = value; }
        }

        private string _password;
        public string Password {
            get { return _password; }
            set { _password = value; }
        }
        #endregion

        #region Helps
        public void Save()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(Path.Combine(Environment.CurrentDirectory, "usettings.cpf"), FileMode.Create))
            {
                formatter.Serialize(fs, this);
            }
        }

        public static void Save(string name, string password, bool autoEnter) {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(Path.Combine(Environment.CurrentDirectory, "usettings.cpf"), FileMode.Create))
            {
                formatter.Serialize(fs, new UserSettings { UserName = name, Password = password, AutoEnter = autoEnter });
            }
        } 

        public static bool ExistFileSettings()
        {
            return File.Exists(Path.Combine(Environment.CurrentDirectory, "usettings.cpf"));
        }

        public static UserSettings Load(UserSettings defaultSettings = null)
        {
            UserSettings settings = defaultSettings;
            if (ExistFileSettings())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream fs = new FileStream(Path.Combine(Environment.CurrentDirectory, "usettings.cpf"), FileMode.Open))
                {
                    settings = (UserSettings)formatter.Deserialize(fs);
                }
            }

            return settings;
        }
        #endregion
    }
}
