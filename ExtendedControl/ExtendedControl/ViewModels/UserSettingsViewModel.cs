using NTW.Controls.Behaviours;
using NTW.Core;
using NTW.Data.Context;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ExtendedControl.ViewModels
{
    public class UserSettingsViewModel:ViewModel
    {
        #region Commands
        private Command _loadCommand;
        public Command LoadCommand {
            get {
                return _loadCommand ?? (_loadCommand = new Command(obj =>
                {
                    UserName = CurrentUser.UserName;
                    FullName = CurrentUser.FullName;
                    Language = new CultureInfo(CurrentUser.Language);
                    IsHiddenUser = CurrentUser.IsHiddenUser;
                }));
            }
        }

        private Command _unloadCommand;
        public Command UnloadCommand {
            get {
                return _unloadCommand ?? (_unloadCommand = new Command(obj =>
                {
                    using (DBContext context = new DBContext(true)) {
                        User u = context.Users.FirstOrDefault(x => x.ID == CurrentUser.ID);

                        if (u.UserName != this.UserName)
                            u.UserName = this.UserName;

                        if (u.FullName != this.FullName)
                            u.FullName = this.FullName;

                        if (u.Language != this.Language.ToString())
                            u.Language = this.Language.ToString();

                        if (u.IsHiddenUser != this.IsHiddenUser)
                            u.IsHiddenUser = this.IsHiddenUser;

                        context.SaveChanges();
                        int h = u.Role.ID;
                        h = u.Setting.ID;
                        App.CurrentUser = u;

                        //lang
                        App.Language = this.Language;

                        this.UserName = null;
                        this.FullName = null;
                        this.Language = null;
                    }
                }));
            }
        }

        private Command _setNewPasswordCommand;
        public Command SetNewPasswordCommand {
            get {
                return _setNewPasswordCommand ?? (_setNewPasswordCommand = new Command(obj => {

                }));
            }
        }

        private Command _showPanelByPasswordCommand;
        public Command ShowPanelByPasswordCommand {
            get {
                return _showPanelByPasswordCommand ?? (_showPanelByPasswordCommand = new Command(obj =>
                {
                    IsSetNewPassword = !IsSetNewPassword;
                }));
            }
        }



        private Command _updatePasswordCommand;
        public Command UpdatePasswordCommand {
            get {
                return _updatePasswordCommand ?? (_updatePasswordCommand = new Command(obj =>
                {
                    Window w = (Window)obj;
                    //проверка и запись нового пароля
                    using (DBContext context = new DBContext(false)) {
                        User u = context.Users.FirstOrDefault(x => x.ID == CurrentUser.ID);
                        if (u != null) {
                            if (Security.VerifyHashedPassword(u.Password, OldPassword))
                            {
                                //считаем что старый пароль подтвержден
                                if (NewPassword != null && NewPassword == ConfirmNewPassword) {
                                    u.HasPassword = NewPassword;
                                    context.SaveChanges();
                                    IsSetNewPassword = false;
                                    //Message: The new password is set successfully
                                    WindowMessageBehaviour.SetMessage(w, App.GetString("UserSeettingsSetPassSuccessfully"), Colors.Green, Colors.White);
                                }
                                else {
                                    //Message: New password not verified
                                    WindowMessageBehaviour.SetMessage(w, App.GetString("UserSeettingsNotVerified"), Colors.Maroon, Colors.White);
                                }
                            }
                            else {
                                //Message: invalid old password
                                WindowMessageBehaviour.SetMessage(w, App.GetString("UserSettingsInvalidPass"), Colors.Maroon, Colors.White);
                            }
                        }
                        else {
                            //Message: Not found user
                            WindowMessageBehaviour.SetMessage(w, App.GetString("UserSettingsNotFoundUser"), Colors.Maroon, Colors.White);
                        }
                    }
                }, obj => obj is Window));
            }
        }


        #endregion

        #region Members
        private string _userName;
        public string UserName { get { return _userName; } set { _userName = value; this.SendPropertyChanged(nameof(UserName)); } }

        private string _fullName;
        public string FullName { get { return _fullName; } set { _fullName = value; this.SendPropertyChanged(nameof(FullName)); } }

        private CultureInfo _language;
        public CultureInfo Language { get { return _language; } set { _language = value; this.SendPropertyChanged(nameof(Language)); } }

        public IEnumerable<CultureInfo> Languages {
            get { return App.Languages; }
        }

        private bool _isHiddenUser;
        public bool IsHiddenUser {
            get { return _isHiddenUser; }
            set { _isHiddenUser = value; this.SendPropertyChanged(nameof(IsHiddenUser)); }
        }


        private bool _isSetNewPassword;
        public bool IsSetNewPassword {
            get { return _isSetNewPassword; }
            set { _isSetNewPassword = value; this.SendPropertyChanged(nameof(IsSetNewPassword)); }
        }

        private string _oldPassword;
        public string OldPassword {
            get { return _oldPassword; }
            set { _oldPassword = value; this.SendPropertyChanged(nameof(OldPassword)); }
        }

        private string _newPassword;
        public string NewPassword {
            get { return _newPassword; }
            set { _newPassword = value; this.SendPropertyChanged(nameof(NewPassword)); }
        }

        private string _confirmNewPassword;
        public string ConfirmNewPassword {
            get { return _confirmNewPassword; }
            set { _confirmNewPassword = value; this.SendPropertyChanged(nameof(ConfirmNewPassword)); }
        }
        #endregion

        #region Helps

        #endregion
    }
}
