using NTW.Controls.Behaviours;
using NTW.Core;
using NTW.Data.Connections;
using NTW.Data.Context;
using NTW.Data.Context.Connections;
using NTW.Data.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using f = System.Windows.Forms;

namespace ExtendedControl.ViewModels
{
    public class DBSettingsViewModel : ViewModel
    {
        #region Commands
        private Command _cancelCommand;
        public Command CancelCommand {
            get { return _cancelCommand ?? (_cancelCommand = new Command(obj => {
                ((Window)obj).DialogResult = false;
                WindowClosingBehaviour.OnCloseWindow((Window)obj);

            }, obj => obj is Window)); }
        }

        private Command _acceptedCommand;
        public Command AcceptedCommand {
            get { return _acceptedCommand ?? (_acceptedCommand = new Command(obj => {

                ((Window)obj).IsEnabled = false;

                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    //предварительно выполняем весь спектр проверок считая что заполнение настроек выполнено успешно
                    if (DBContext.TestConnection(_settings.Connection))
                    {
                        App.BeginInvoke(() =>
                        {
                            //заполняем параметрами подключения к БД
                            _settings.Save();
                            App.DBSettings = _settings;

                            ((Window)obj).DialogResult = true;
                            WindowClosingBehaviour.OnCloseWindow((Window)obj);
                        });
                    }
                    else
                    {
                        App.BeginInvoke(() =>
                        {
                            ((Window)obj).IsEnabled = true;

                            WindowMessageBehaviour.SetMessage((Window)obj, App.GetString("DBSettingsDataBaseNotExists"), Colors.Maroon, Colors.White);
                        });
                    }
                });
            }, obj => obj is Window)); }
        }


        private Command _testCommand;
        public Command TestCommand {
            get { return _testCommand ?? (_testCommand = new Command(obj => {
                ((Window)obj).IsEnabled = false;

                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    if (DBContext.TestConnection(_settings.Connection))
                        App.BeginInvoke(() =>
                        {
                            ((Window)obj).IsEnabled = true;

                            WindowMessageBehaviour.SetMessage((Window)obj, App.GetString("DBSettingsTestComplited"), Colors.DarkGreen, Colors.White);
                        });
                    else
                        App.BeginInvoke(() =>
                        {
                            ((Window)obj).IsEnabled = true;

                            WindowMessageBehaviour.SetMessage((Window)obj, App.GetString("DBSettingsDataBaseNotExists"), Colors.Maroon, Colors.White);
                        });
                });
                
            }, obj => obj is Window)); }
        }

        private Command _createCommand;
        public Command CreateCommand {
            get { return _createCommand ?? (_createCommand = new Command(obj => {
                ((Window)obj).IsEnabled = false;
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    if (!DBContext.TestConnection(_settings.Connection)) {
                        bool resCreate = true;
                        using (DBContext context = new DBContext(_settings.Connection.GetConnectionString())) {
                            resCreate = context.CreateDatabase(true);
                        }

                        App.BeginInvoke(() =>
                        {
                            ((Window)obj).IsEnabled = true;
                            if (resCreate)
                                WindowMessageBehaviour.SetMessage((Window)obj, App.GetString("DBSettingsCreateDBComplited"), Colors.Green, Colors.White);
                            else
                                WindowMessageBehaviour.SetMessage((Window)obj, App.GetString("DBSettingsNotCreateDatabase"), Colors.Maroon, Colors.White);
                        });
                    }
                    else {
                        App.BeginInvoke(() =>
                        {
                            ((Window)obj).IsEnabled = true;

                            WindowMessageBehaviour.SetMessage((Window)obj, App.GetString("DBSettingsDBExists"), Colors.Maroon, Colors.White);
                        });
                    }
                });
            }, obj => obj is Window)); }
        }

        private Command _deleteCommand;
        public Command DeleteCommand {
            get { return _deleteCommand ?? (_deleteCommand = new Command(obj => {
                ((Window)obj).IsEnabled = false;
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    if (DBContext.TestConnection(_settings.Connection)) {
                        if (DBContext.TestConnection(_settings.Connection)) {
                            using (DBContext context = new DBContext(_settings.Connection.GetConnectionString())) {
                                context.DeleteDatabase();
                            }
                        }

                        App.BeginInvoke(() =>
                        {
                            ((Window)obj).IsEnabled = true;

                            WindowMessageBehaviour.SetMessage((Window)obj, App.GetString("DBSettingsDeleteDBComplited"), Colors.DarkGreen, Colors.White);
                        });
                    }
                    else {
                        App.BeginInvoke(() =>
                        {
                            ((Window)obj).IsEnabled = true;

                            WindowMessageBehaviour.SetMessage((Window)obj, App.GetString("DBSettingsDBExists"), Colors.Maroon, Colors.White);
                        });
                    }
                });
            }, obj => obj is Window)); }
        }

        private Command _selectPathToDBCommand;
        public Command SelectPathToDBCommand {
            get { return _selectPathToDBCommand ?? (_selectPathToDBCommand = new Command(obj => {
                f.FolderBrowserDialog fbd = new f.FolderBrowserDialog();
                switch (fbd.ShowDialog())
                {
                    case f.DialogResult.OK:
                        _settings.Connection.PathToResourceDB = fbd.SelectedPath;
                        this.SendPropertyChanged("PathToResourceDB");
                        break;
                }
            })); }
        }

        private Command _selectPathToDSCommand;
        public Command SelectPathToDSCommand {
            get {
                return _selectPathToDSCommand ?? (_selectPathToDSCommand = new Command(obj => {
                    f.OpenFileDialog fbd = new f.OpenFileDialog();
                    fbd.Filter = "Sql Server Compact Database|*.sdf";
                    switch (fbd.ShowDialog()) {
                        case f.DialogResult.OK:
                            ((SqlCompactConnection)_settings.Connection).DataSource = fbd.FileName;
                            break;
                    }
                }));
            }
        }

        private Command _selectPathToFileMdb;
        public Command SelectPathToFileMdb {
            get { return _selectPathToFileMdb ?? (_selectPathToFileMdb = new Command(obj => {
                f.OpenFileDialog ofd = new f.OpenFileDialog();
                ofd.Filter = "DB MS SQL Server (*.mdf)|*.mdf";
                ofd.AddExtension = true;
                ofd.CheckFileExists = false;
                ofd.CheckPathExists = true;
                ofd.DefaultExt = "*.mdf";
                switch (ofd.ShowDialog()) {
                    case f.DialogResult.OK:
                        ((SqlLocalConnection)_settings.Connection).PathToFile = ofd.FileName;
                        break;
                }
            })); }
        }



        private Command _loadCommand;
        public Command LoadCommand {
            get {
                return _loadCommand ?? (_loadCommand = new Command(obj =>
                {
                    _settings = DBSettings.Load();
                    this.SendPropertyChanged(nameof(Connection));
                }));
            }
        }


        private Command _unloadCommand;
        public Command UnloadCommand {
            get {
                return _unloadCommand ?? (_unloadCommand = new Command(obj =>
                {
                    System.Threading.Tasks.Task.Factory.StartNew(() =>
                    {
                        if (DBContext.TestConnection(_settings.Connection))
                            App.BeginInvoke(() =>
                            {
                                App.DBSettings.Connection = _settings.Connection;
                                _settings.Save();
                            });
                    });
                }));
            }
        }



        #endregion

        #region Members
        // проблема с работой для настроек соединени
        // стоит расмотреть вариант с разделением настроек при работе с блоком данных и работать только с временными настройками на этапе открытой панели
        private DBSettings _settings = App.DBSettings; 

        public IEnumerable<string> NamesProvider {
            get { return new string[] {
                "Sql server",
                "Sql server (Local)"
            }; }
        }

        private string _provider = "Sql server";
        public string Provider {
            get { return _provider; }
            set { _provider = value;
                this.SendPropertyChanged("Provider");
                this.SendPropertyChanged("TypeProvider");
                this.SendPropertyChanged(nameof(TemplateProvider)); }
        }

        public Type TypeProvider {
            get {
                switch (_provider) {
                    case "Sql server":
                        if (_settings.Connection == null || _settings.Connection.GetType() != typeof(SqlConnection))
                            _settings.Connection = new SqlConnection();
                        return typeof(SqlConnection);
                    case "Sql server compact":
                        if (_settings.Connection == null || _settings.Connection.GetType() != typeof(SqlCompactConnection))
                            _settings.Connection = new SqlCompactConnection();
                        return typeof(SqlCompactConnection);
                    case "Sql server (Local)":
                        if (_settings.Connection == null || _settings.Connection.GetType() != typeof(SqlLocalConnection))
                            _settings.Connection = new SqlLocalConnection();
                        return typeof(SqlLocalConnection);
                    default:
                        return typeof(SqlConnection);
                }
            }
        }

        public ControlTemplate TemplateProvider {
            get { return (ControlTemplate)Application.Current.FindResource(TypeProvider); }
        }

        public IConnection Connection {
            get { return _settings.Connection; }
        }

        public string PathToResourceDB {
            get { return Connection.PathToResourceDB; }
        }
        #endregion

        #region Helps
        public static bool TestConnection(DBSettings settings) {
            bool result = false;

                if (DBContext.TestConnection(settings.Connection))
                    result = true;
                else
                    result = false;

            return result;
        }
        #endregion
    }
}
