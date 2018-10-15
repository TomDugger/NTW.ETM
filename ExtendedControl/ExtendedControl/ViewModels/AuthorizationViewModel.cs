using NTW.Controls.Behaviours;
using NTW.Core;
using NTW.Data.Context;
using NTW.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ExtendedControl.ViewModels
{
    public class AuthorizationViewModel: ViewModel
    {
        #region Commands
        private Command _hideWindowCommand;
        public Command HideWindowCommand {
            get { return _hideWindowCommand ?? (_hideWindowCommand = new Command(obj => {
                ((Window)obj).WindowState = WindowState.Minimized;
                //((Window)obj).Visibility = Visibility.Hidden;
            }, obj => obj is Window)); }
        }

        private Command _enterUserCommand;
        public Command EnterUserCommand {
            get { return _enterUserCommand ?? (_enterUserCommand = new Command(obj => {

                User user;

                using (DBContext context = new DBContext(true)) {
                    //проверка пользователя
                    if ((user = context.Users.FirstOrDefault(i => i.UserName == EnterUserName)) != null) {
                        //проверка пароля
                        string password = Password;
                        if (Security.VerifyHashedPassword(user.Password, password)) {
                            //регистрируем пользователя и перестраиваем панели
                            int h = user.Role.ID;
                            h = user.Setting.ID;
                            App.CurrentUser = user;

                            UserSettings.Save(EnterUserName, Password, AutoEnter);

                            EnterUserName = " ";
                            Password = "    ";
                            ((Window)obj).DialogResult = true;
                            WindowClosingBehaviour.OnCloseWindow((Window)obj);
                        }
                        else
                            WindowMessageBehaviour.SetMessage((Window)obj, App.GetString("AutorizationIncorrectPassword"), Colors.Maroon, Colors.White);
                    } else {
                        //error - ненайден пользователь с таким именем
                        WindowMessageBehaviour.SetMessage((Window)obj, App.GetString("AutorizationUserNotFound"), Colors.Maroon, Colors.White);
                    }
                }
            }, obj => obj is Window)); }
        }

        private Command _exitToApplicationCommand;
        public Command ExitToApplicationCommand
        {
            get { return _exitToApplicationCommand ?? (_exitToApplicationCommand = new Command(obj => {
                //если используется выход потребуется формально убрать и пароль
                UserSettings us = UserSettings.Load(new UserSettings());
                us.Password = string.Empty;
                us.Save();
                ((Window)obj).DialogResult = false;
                WindowClosingBehaviour.OnCloseWindow((Window)obj);
            }, obj => obj is Window)); }
        }
        
        private Command _anyKeyCommand;
        public Command AnyKeyCommand {
            get {
                return _anyKeyCommand ?? (_anyKeyCommand = new Command(obj =>
                {
                    this.SendPropertyChanged(nameof(ActiveCapsLock));
                }));
            }
        }


        #endregion

        #region Members
        public IEnumerable<string> UserNames {
            get
            {
                //хоть и не правильно, но каждый раз когда запрашиваем список пользователей подгружаем имена пользователей
                //формируем начальные параметры для текущего пользователя если они есть (кроме пароля)
                UserSettings us = UserSettings.Load(new UserSettings());
                EnterUserName = us.UserName;
                AutoEnter = us.AutoEnter;

                IEnumerable<string> result = null;
                using (DBContext context = new DBContext(false)) {
                    result = context.Users.Where(i => !i.IsHiddenUser && !i.IsDelete).Select(i => i.UserName).ToArray();
                }
                return result;
            }
        }

        private string _enterUserName;
        public string EnterUserName {
            get { return _enterUserName; }
            set { _enterUserName = value; this.SendPropertyChanged("EnterUserName"); this.SendPropertyChanged(nameof(ActiveCapsLock)); }
        }

        private string _password;
        public string Password {
            get { return _password; }
            set { _password = value; this.SendPropertyChanged("Password"); this.SendPropertyChanged(nameof(ActiveCapsLock)); }
        }

        private bool _autoEnter;
        public bool AutoEnter {
            get { return _autoEnter; }
            set { _autoEnter = value; this.SendPropertyChanged("AutoEnter"); }
        }
        
        public bool ActiveCapsLock {
            get { return Console.CapsLock; }
        }
        #endregion

        #region Helps

        #endregion
    }
}
