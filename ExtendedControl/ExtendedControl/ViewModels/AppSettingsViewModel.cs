using NTW.Controls;
using NTW.Core;
using NTW.Data;
using NTW.Data.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Media;

namespace ExtendedControl.ViewModels
{
    public class AppSettingsViewModel:ViewModel
    {
        #region Commands
        private Command _loadCommand;
        public Command LoadCommand {
            get {
                return _loadCommand ?? (_loadCommand = new Command(obj => {

                }));
            }
        }

        private Command _unloadCommand;
        public Command UnloadCommand {
            get {
                return _unloadCommand ?? (_unloadCommand = new Command(obj => {
                    //внезависимости от требуется или не требуется сохраняем значения
                    if (AppTheme == Theme.Custom) {
                        foreach (DictionaryEntry item in TempColors) {
                            App.SetColorResource(item.Key.ToString(), (Color)item.Value, true);
                        }
                    }
                }));
            }
        }
        #endregion

        #region Members
        public CultureInfo Language
        {
            get { return App.Language; }
            set { App.Language = value; }
        }

        public IEnumerable<CultureInfo> Languages {
            get { return App.Languages; }
        }

        public Theme AppTheme {
            get { return App.AppSettings.Theme; }
            set {
                App.AppSettings.Theme = value;
                //в зависимости от выбранного шаблона выставляем значения
                StateColorsResource(value);
                this.SendPropertyChanged(nameof(AppTheme));
                this.SendPropertyChanged(nameof(IsCustom));
            }
        }

        private Hashtable _tempColors;
        protected Hashtable TempColors {
            get { return _tempColors ?? (_tempColors = GetColors()); }
        }

        public bool IsCustom {
            get { return AppTheme == Theme.Custom; }
        }

        private bool _setOnValues = false;
        public bool SetOnValues {
            get { return _setOnValues; }
            set {
                _setOnValues = value;
                this.SendPropertyChanged(nameof(SetOnValues));
            }
        }

        public IEnumerable<Tuple<string, Theme>> ThemeList
        {
            get { return Enum.GetNames(typeof(Theme)).Select(x => new Tuple<string, Theme>(x, (Theme)Enum.Parse(typeof(Theme), x))); }
        }

        public string IpSyncServer {
            get { return ServiceSettings.Load().IPTimeServer; }
            set {
                ServiceSettings ss = ServiceSettings.Load();
                ss.IPTimeServer = value; ss.Save();
            }
        }

        [IndexerName("Item")]
        public Color this[string name] {
            get {

                return  SetOnValues ? App.GetColorResource(name) : (Color)TempColors[name];
            }
            set {
                if (SetOnValues)
                    App.SetColorResource(name, value, SetOnValues);
                else
                    TempColors[name] = value;
            }
        }
        #endregion

        #region Helps
        private Hashtable GetColors() {
            Hashtable result = new Hashtable();

                result["MainBackColor"] = App.AppSettings.MainBackColor != null ?
                    (Color)ColorConverter.ConvertFromString(App.AppSettings.MainBackColor) :
                    App.GetColorResource("MainBackColor");

                result["MainForeColor"] = App.AppSettings.MainForeColor != null ? 
                    (Color)ColorConverter.ConvertFromString(App.AppSettings.MainForeColor) :
                    App.GetColorResource("MainForeColor");

            
                result["FBackColor"] = App.AppSettings.FBackColor != null ? 
                    (Color)ColorConverter.ConvertFromString(App.AppSettings.FBackColor) :
                    App.GetColorResource("FBackColor");

                result["SBackColor"] = App.AppSettings.SBackColor != null ? 
                    (Color)ColorConverter.ConvertFromString(App.AppSettings.SBackColor) :
                    App.GetColorResource("SBackColor");

                result["TBackColor"] = App.AppSettings.TBackColor != null ? 
                    (Color)ColorConverter.ConvertFromString(App.AppSettings.TBackColor) :
                    App.GetColorResource("TBackColor");


                result["FForeColor"] = App.AppSettings.FForeColor != null ? 
                    (Color)ColorConverter.ConvertFromString(App.AppSettings.FForeColor) :
                    App.GetColorResource("FForeColor");

                result["SForeColor"] = App.AppSettings.SForeColor != null ? 
                    (Color)ColorConverter.ConvertFromString(App.AppSettings.SForeColor) :
                    App.GetColorResource("SForeColor");

                result["TForeColor"] = App.AppSettings.TForeColor != null ? 
                    (Color)ColorConverter.ConvertFromString(App.AppSettings.TForeColor) :
                    App.GetColorResource("TForeColor");
            return result;
        }

        public void StateColorsResource(Theme theme) {
            switch (theme)
            {
                case Theme.Dark:
                    App.SetColorResource("MainBackColor", Color.FromArgb(255, 45, 45, 48)); //Colors.DimGray);
                    App.SetColorResource("MainForeColor", Colors.White);

                    App.SetColorResource("FBackColor", Color.FromArgb(255, 45, 45, 48)); //Colors.Gray);
                    App.SetColorResource("SBackColor", Colors.Orange);
                    App.SetColorResource("TBackColor", Colors.DodgerBlue);

                    App.SetColorResource("FForeColor", Colors.White);
                    App.SetColorResource("SForeColor", Colors.Black);
                    App.SetColorResource("TForeColor", Colors.White);
                    break;
                case Theme.Light:
                    App.SetColorResource("MainBackColor", Colors.White);
                    App.SetColorResource("MainForeColor", Colors.DimGray);

                    App.SetColorResource("FBackColor", Colors.White);
                    App.SetColorResource("SBackColor", Colors.Black);
                    App.SetColorResource("TBackColor", Colors.White);

                    App.SetColorResource("FForeColor", Colors.Gray);
                    App.SetColorResource("SForeColor", Colors.Orange);
                    App.SetColorResource("TForeColor", Colors.DodgerBlue);
                    break;
                case Theme.Custom:
                    //пробуем подгрузить из файла последние сохраненные
                    if(App.AppSettings.MainBackColor != null)
                        App.SetColorResource("MainBackColor", (Color)ColorConverter.ConvertFromString(App.AppSettings.MainBackColor));
                    if (App.AppSettings.MainForeColor != null)
                        App.SetColorResource("MainForeColor", (Color)ColorConverter.ConvertFromString(App.AppSettings.MainForeColor));

                    if(App.AppSettings.FBackColor != null)
                    App.SetColorResource("FBackColor", (Color)ColorConverter.ConvertFromString(App.AppSettings.FBackColor));
                    if (App.AppSettings.SBackColor != null)
                        App.SetColorResource("SBackColor", (Color)ColorConverter.ConvertFromString(App.AppSettings.SBackColor));
                    if (App.AppSettings.TBackColor != null)
                        App.SetColorResource("TBackColor", (Color)ColorConverter.ConvertFromString(App.AppSettings.TBackColor));

                    if (App.AppSettings.FForeColor != null)
                        App.SetColorResource("FForeColor", (Color)ColorConverter.ConvertFromString(App.AppSettings.FForeColor));
                    if (App.AppSettings.SForeColor != null)
                        App.SetColorResource("SForeColor", (Color)ColorConverter.ConvertFromString(App.AppSettings.SForeColor));
                    if (App.AppSettings.TForeColor != null)
                        App.SetColorResource("TForeColor", (Color)ColorConverter.ConvertFromString(App.AppSettings.TForeColor));
                    break;
            }
            this.SendPropertyChanged("Item[]");
        }
        #endregion
    }
}
