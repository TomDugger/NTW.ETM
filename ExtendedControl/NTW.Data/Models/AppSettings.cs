using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace NTW.Data.Models
{
    [Serializable]
    public class AppSettings 
    {
        #region Members
        private Theme _theme = Theme.Dark;

        private string _mainBackColor;
        private string _mainForeColor;

        private string _fBackColor;
        private string _sBackColor;
        private string _tBackColor;

        private string _fForeColor;
        private string _sForeColor;
        private string _tForeColor;

        private string _pathOfDownload;
        #endregion

        #region Property
        public Theme Theme {
            get { return _theme; }
            set { _theme = value; this.Save(); }
        }

        public string MainBackColor
        {
            get { return _mainBackColor; }
            set { _mainBackColor = value; this.Save(); }
        }
        public string MainForeColor {
            get { return _mainForeColor; }
            set { _mainForeColor = value; this.Save(); }
        }

        public string FBackColor {
            get { return _fBackColor; }
            set { _fBackColor = value; this.Save(); }
        }
        public string SBackColor {
            get { return _sBackColor; }
            set { _sBackColor = value; this.Save(); }
        }
        public string TBackColor {
            get { return _tBackColor; }
            set { _tBackColor = value; this.Save(); }
        }

        public string FForeColor {
            get { return _fForeColor; }
            set { _fForeColor = value; this.Save(); }
        }
        public string SForeColor {
            get { return _sForeColor; }
            set { _sForeColor = value; this.Save(); }
        }
        public string TForeColor {
            get { return _tForeColor; }
            set { _tForeColor = value; this.Save(); }
        }

        public string PathOfDownload {
            get { return _pathOfDownload ?? (_pathOfDownload = Path.Combine(Environment.CurrentDirectory, "")); }
            set { _pathOfDownload = value; this.Save(); }
        }
        #endregion

        #region Helps
        public void Save()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(Path.Combine(Environment.CurrentDirectory, "settings.cpf"), FileMode.Create))
            {
                formatter.Serialize(fs, this);
            }
        }

        public static bool ExistFileSettings()
        {
            return File.Exists(Path.Combine(Environment.CurrentDirectory, "settings.cpf"));
        }

        public static AppSettings Load()
        {
            AppSettings settings = null;
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(Path.Combine(Environment.CurrentDirectory, "settings.cpf"), FileMode.Open))
            {
                settings = (AppSettings)formatter.Deserialize(fs);
            }

            return settings;
        }
        #endregion
    }
}
