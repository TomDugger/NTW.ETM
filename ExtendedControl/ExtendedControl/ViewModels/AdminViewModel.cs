using ExtendedControl.Views.Panels.Child;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NTW.Controls.Behaviours;
using NTW.Core;
using System.Windows;
using System.Collections.ObjectModel;
using NTW.Data.Context;
using NTW.Data.Models;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Data;
using NTW.Data;
using System.Data.Objects;
using System.Windows.Threading;

namespace ExtendedControl.ViewModels
{
    public class AdminViewModel: ControlViewModel
    {
        #region Commands
        private Command _selectionAccesCommand;
        public Command SelectionAccesCommand {
            get {
                return _selectionAccesCommand ?? (_selectionAccesCommand = new Command(obj => {
                    if (CurrentAccess != -1)
                    {
                        CloseWindowsByType<AdminControlChildWindow>();
                        CloseWindowsByType<AdminChildWindow>();

                        AdminChildWindow acw = new AdminChildWindow();
                        acw.Owner = (Window)obj;
                        HideButtonBehaviour.SetCommand(acw, new Command((obj1) =>
                        {
                            CurrentAccess = -1;
                            if (_tasks != null) {

                                App.Invoke((Action)delegate {
                                    _tasks = null;
                                    _viewByTasks = null;
                                });
                            }
                            if (Users != null) {
                                App.Invoke(() => {
                                    _users = null;
                                    _viewByUsers = null;
                                });
                            }
                            if (Roles != null) {
                                App.Invoke(() => {
                                    _roles = null;
                                    _viewByRoles = null;
                                });
                            }
                            if (Settings != null) {
                                App.Invoke(() => {
                                    _settings = null;
                                    _viewBySettings = null;
                                });
                            }
                            if (Roles != null) {
                                App.Invoke(() => {
                                    _roles = null;
                                    _viewByRoles = null;
                                });
                            }

                            WindowVisibilityBehaviour.SetIsVisible(acw, false);
                        }, obj1 => obj1 != null));
                        WindowPositionBehaviour.SetWindowScreen(acw, WindowPositionBehaviour.GetWindowScreen(acw.Owner));
                        WindowPositionBehaviour.SetWindowPosition(acw, WindowPositionBehaviour.GetWindowPosition((acw.Owner)));
                        WindowVisibilityBehaviour.SetIsVisible(acw, true);
                    }
                }, obj => obj is Window));
            }
        }

        private Command _acceptedControlCommand;
        public Command AcceptedControlCommand {
            get {
                return _acceptedControlCommand;
            }
        }

        #region Tasks
        private Command _restoreTaskCommand;
        public Command RestoreTaskCommand {
            get {
                return _restoreTaskCommand ?? (_restoreTaskCommand = new Command(obj => {
                    Task t = (Task)((Tuple<object, object>)obj).Item1;
                    Task result = context.Tasks.FirstOrDefault(x => x.ID == t.ID);
                    result.IsDelete = false;
                    context.SaveChanges();
                    Tasks.Remove(t);
                    WindowMessageBehaviour.SetMessage((Window)((Tuple<object, object>)obj).Item2, string.Format(App.GetString("AdminTaskMessageRestore"), t.Caption), backColor: Colors.Green, foreColor: Colors.White);
                }, obj => obj != null));
            }
        }

        private Command _restoreAllTaskCommand;
        public Command RestoreAllTaskCommand {
            get {
                return _restoreAllTaskCommand ?? (_restoreAllTaskCommand = new Command(obj => {
                    Window w = (Window)obj;
                    ListCollectionView f = (ListCollectionView)ViewByTasks;
                    if (f.Count > 0) {
                        List<Task> temp = new List<Task>();
                        int count = f.Count;
                        foreach (Task item in f) {
                            Task result = context.Tasks.FirstOrDefault(x => x.ID == item.ID);
                            result.IsDelete = false;
                            context.SaveChanges();
                            temp.Add(item);
                        }

                        foreach (var item in temp)
                            Tasks.Remove(item);

                        WindowMessageBehaviour.SetMessage(w, string.Format(App.GetString("AdminTaskMessageAll"), count), backColor: Colors.Green, foreColor: Colors.White);
                    }
                }, obj => obj is Window));
            }
        }
        #endregion

        #region Users
        private Command _createNewUserCommand;
        public Command CreateNewUserCommand {
            get {
                return _createNewUserCommand ?? (_createNewUserCommand = new Command(obj => {
                    this.State = TypeControl.Create;

                    var select = (Window)obj;

                    CloseWindowsByType<AdminControlChildWindow>();

                    SelectedObject = new User {
                        IdRole = context.Roles.First().ID,
                        IdSetting = context.Settings.First().ID,
                        UserName = "User",
                        FullName = "Full name",
                        Password = string.Empty,
                        Language = "ru",
                        IpAdress = string.Empty
                    };

                    _acceptedControlCommand = new Command(wn => {

                        context.Users.AddObject((User)SelectedObject);
                        Users.Add((User)SelectedObject);
                        context.SaveChanges();
                        SelectedObject = null;
                        WindowVisibilityBehaviour.SetIsVisible((Window)wn, false);
                        this.State = TypeControl.Normal;
                    }, wn => wn != null);

                    AdminControlChildWindow accw = new AdminControlChildWindow
                    {
                        Owner = select
                    };

                    HideButtonBehaviour.SetCommand(accw, new Command((obj1) => {
                        if (this.State != TypeControl.Create)
                            context.Refresh(RefreshMode.StoreWins, SelectedObject);
                        SelectedObject = null;
                        WindowVisibilityBehaviour.SetIsVisible(accw, false);
                        this.State = TypeControl.Normal;
                    }, obj1 => obj1 != null));

                    WindowPositionBehaviour.SetWindowScreen(accw, WindowPositionBehaviour.GetWindowScreen(accw.Owner));
                    WindowPositionBehaviour.SetWindowPosition(accw, WindowPositionBehaviour.GetWindowPosition((accw.Owner)));
                    WindowVisibilityBehaviour.SetIsVisible(accw, true);
                }, obj => obj != null));
            }
        }

        private Command _editUserCommand;
        public Command EditUserCommand {
            get {
                return _editUserCommand ?? (_editUserCommand = new Command(obj => {
                    this.State = TypeControl.Edit;

                    var select = (Tuple<object, object>)obj;

                    CloseWindowsByType<AdminControlChildWindow>();

                    SelectedObject = context.Users.FirstOrDefault(x => x.ID == ((User)select.Item1).ID);
                    EditObject = select.Item1;
                    _acceptedControlCommand = new Command(wn => {
                        context.SaveChanges();
                        int idPos = Users.IndexOf((User)select.Item1);
                        Users.RemoveAt(idPos);
                        Users.Insert(idPos, (User)SelectedObject);
                        SelectedObject = null;
                        WindowVisibilityBehaviour.SetIsVisible((Window)wn, false);
                        EditObject = null;
                        this.State = TypeControl.Normal;
                    }, wn => wn != null);

                    AdminControlChildWindow accw = new AdminControlChildWindow
                    {
                        Owner = (Window)select.Item2
                    };

                    HideButtonBehaviour.SetCommand(accw, new Command((obj1) => {
                        context.Refresh(RefreshMode.StoreWins, SelectedObject);
                        SelectedObject = null;

                        WindowVisibilityBehaviour.SetIsVisible(accw, false);
                        EditObject = null;
                        this.State = TypeControl.Normal;
                    }, obj1 => obj1 != null));

                    WindowPositionBehaviour.SetWindowScreen(accw, WindowPositionBehaviour.GetWindowScreen(accw.Owner));
                    WindowPositionBehaviour.SetWindowPosition(accw, WindowPositionBehaviour.GetWindowPosition((accw.Owner)));
                    WindowVisibilityBehaviour.SetIsVisible(accw, true);
                }, obj => obj != null));
            }
        }

        private Command _removeUserCommand;
        public Command RemoveUserCommand {
            get {
                return _removeUserCommand ?? (_removeUserCommand = new Command(obj => {
                    var select = (Tuple<object, object>)obj;

                    User user = context.Users.FirstOrDefault(x => x.ID == ((User)select.Item1).ID);

                    user.IsDelete = true;
                    ((User)select.Item1).IsDelete = true;

                    context.SaveChanges();
                    WindowMessageBehaviour.SetMessage(((Window)select.Item2), string.Format(App.GetString("AdminUserMessageRemove"), ((User)select.Item1).FullName), backColor: Colors.Green, foreColor: Colors.White);
                }, obj => obj != null));
            }
        }

        private Command _restoredUserCommand;
        public Command RestoredUserCommand {
            get {
                return _restoredUserCommand ?? (_restoredUserCommand = new Command(obj => {
                    var select = (Tuple<object, object>)obj;

                    User user = context.Users.FirstOrDefault(x => x.ID == ((User)select.Item1).ID);

                    user.IsDelete = false;
                    ((User)select.Item1).IsDelete = false;

                    context.SaveChanges();
                    WindowMessageBehaviour.SetMessage(((Window)select.Item2), string.Format(App.GetString("AdminUserMessageRestore"), ((User)select.Item1).FullName), backColor: Colors.Green, foreColor: Colors.White);
                }, obj => obj != null));
            }
        }

        private Command _removeAllUsersCommand;
        public Command RemoveAllUsersCommand {
            get {
                return _removeAllUsersCommand ?? (_removeAllUsersCommand = new Command(obj => {
                    Window w = (Window)obj;
                    ListCollectionView f = (ListCollectionView)ViewByUsers;
                    int countOnExecution = 0;
                    if (f.Count > 0) {
                        foreach (User item in f) {
                            User result = context.Users.FirstOrDefault(x => x.ID == item.ID);
                            if (result != null && !result.IsDelete) {
                                result.IsDelete = true;
                                item.IsDelete = true;
                                countOnExecution++;
                            }
                        }
                        context.SaveChanges();
                        WindowMessageBehaviour.SetMessage(w, string.Format(App.GetString("AdminUserMessageRemoveAll"), countOnExecution), backColor: Colors.Green, foreColor: Colors.White);
                    }
                }, obj => obj is Window));
            }
        }

        private Command _restoreAllUsersCommand;
        public Command RestoreAllUsersCommand {
            get {
                return _restoreAllUsersCommand ?? (_restoreAllUsersCommand = new Command(obj => {
                    Window w = (Window)obj;
                    ListCollectionView f = (ListCollectionView)ViewByUsers;
                    int countOnExecution = 0;
                    if (f.Count > 0) {
                        foreach (User item in f) {
                            User result = context.Users.FirstOrDefault(x => x.ID == item.ID);
                            if (result != null && result.IsDelete) {
                                result.IsDelete = false;
                                item.IsDelete = false;
                                countOnExecution++;
                            }
                        }
                        context.SaveChanges();
                        WindowMessageBehaviour.SetMessage(w, string.Format(App.GetString("AdminUserMessageRestoredAll"), countOnExecution), backColor: Colors.Green, foreColor: Colors.White);
                    }
                }, obj => obj is Window));
            }
        }

        private Command _activeRoleOnUserCommand;
        public Command ActiveRoleOnUserCommand {
            get {
                return _activeRoleOnUserCommand ?? (_activeRoleOnUserCommand = new Command(obj => {
                    TypeControl tempstate = this.State;
                    this._state = TypeControl.Create;
                    int temp = _currentAccess;
                    _currentAccess = 3;

                    var select = (Window)obj;
                    var tempObject = SelectedObject;

                    _selectedObject = new Role {
                        RoleName = "New role"
                    };

                    AdminControlChildWindow accw = new AdminControlChildWindow
                    {
                        Owner = select
                    };
                    WindowPositionBehaviour.SetWindowPosition(accw, WindowPositionBehaviour.GetWindowPosition((accw.Owner)));
                    WindowVisibilityBehaviour.SetIsVisible(accw, true);

                    if (accw.DialogResult == true) {
                        context.Roles.AddObject((Role)SelectedObject);
                        Roles.Add((Role)SelectedObject);
                        context.SaveChanges();
                        this.SendPropertyChanged(nameof(RolesNameList));
                        ((User)tempObject).IdRole = ((Role)SelectedObject).ID;
                    }
                    SelectedObject = tempObject;
                    _currentAccess = temp;
                    this.State = tempstate;
                }, obj => obj != null));
            }
        }

        private Command _activeSettingOnUserCommand;
        public Command ActiveSettingOnUserCommand {
            get {
                return _activeSettingOnUserCommand ?? (_activeSettingOnUserCommand = new Command(obj => {
                    TypeControl tempstate = this.State;
                    this._state = TypeControl.Create;
                    int temp = _currentAccess;
                    _currentAccess = 4;

                    var select = (Window)obj;
                    var tempObject = SelectedObject;

                    _selectedObject = new Setting {
                        Caption = "New setting"
                    };

                    AdminControlChildWindow accw = new AdminControlChildWindow
                    {
                        Owner = select
                    };
                    WindowPositionBehaviour.SetWindowPosition(accw, WindowPositionBehaviour.GetWindowPosition((accw.Owner)));
                    WindowVisibilityBehaviour.SetIsVisible(accw, true);

                    if (accw.DialogResult == true) {
                        context.Settings.AddObject((Setting)SelectedObject);
                        Settings.Add((Setting)SelectedObject);
                        context.SaveChanges();
                        this.SendPropertyChanged(nameof(SettingsNamesList));
                        ((User)tempObject).IdSetting = ((Setting)SelectedObject).ID;
                    }
                    SelectedObject = tempObject;
                    _currentAccess = temp;
                    this.State = tempstate;
                }, obj => obj != null));
            }
        }
        #endregion

        #region Roles
        private Command _createNewRoleCommand;
        public Command CreateNewRoleCommand {
            get {
                return _createNewRoleCommand ?? (_createNewRoleCommand = new Command(obj => {
                    this.State = TypeControl.Create;

                    var select = (Window)obj;

                    CloseWindowsByType<AdminControlChildWindow>();

                    SelectedObject = new Role {
                        RoleName = "New role"
                    };

                    _acceptedControlCommand = new Command(wn => {

                        context.Roles.AddObject((Role)SelectedObject);
                        Roles.Add((Role)SelectedObject);
                        context.SaveChanges();
                        SelectedObject = null;
                        WindowVisibilityBehaviour.SetIsVisible((Window)wn, false);
                        this.State = TypeControl.Normal;
                    }, wn => wn != null);

                    AdminControlChildWindow accw = new AdminControlChildWindow
                    {
                        Owner = select
                    };

                    HideButtonBehaviour.SetCommand(accw, new Command((obj1) => {
                        if (SelectedObject != null && this.State != TypeControl.Create)
                            context.Refresh(RefreshMode.StoreWins, SelectedObject);
                        SelectedObject = null;
                        WindowVisibilityBehaviour.SetIsVisible((Window)obj1, false);
                        this.State = TypeControl.Normal;
                    }, obj1 => obj1 != null));

                    WindowPositionBehaviour.SetWindowScreen(accw, WindowPositionBehaviour.GetWindowScreen(accw.Owner));
                    WindowPositionBehaviour.SetWindowPosition(accw, WindowPositionBehaviour.GetWindowPosition((accw.Owner)));
                    WindowVisibilityBehaviour.SetIsVisible(accw, true);

                }, obj => obj != null));
            }
        }

        private Command _editRoleCommand;
        public Command EditRoleCommand {
            get {
                return _editRoleCommand ?? (_editRoleCommand = new Command(obj => {
                    this.State = TypeControl.Edit;

                    var select = (Tuple<object, object>)obj;

                    CloseWindowsByType<AdminControlChildWindow>();

                    SelectedObject = context.Roles.FirstOrDefault(x => x.ID == ((Role)select.Item1).ID);
                    EditObject = select.Item1;
                    _acceptedControlCommand = new Command(wn => {
                        context.SaveChanges();
                        int idPos = Roles.IndexOf((Role)select.Item1);
                        Roles.RemoveAt(idPos);
                        Roles.Insert(idPos, (Role)SelectedObject);
                        WindowVisibilityBehaviour.SetIsVisible((Window)wn, false);
                        SelectedObject = null;
                        EditObject = null;
                        this.State = TypeControl.Normal;
                    }, wn => wn != null);

                    AdminControlChildWindow accw = new AdminControlChildWindow
                    {
                        Owner = (Window)select.Item2
                    };

                    HideButtonBehaviour.SetCommand(accw, new Command((obj1) => {
                        context.Refresh(RefreshMode.StoreWins, SelectedObject);
                        SelectedObject = null;
                        WindowVisibilityBehaviour.SetIsVisible(accw, false);
                        EditObject = null;
                        this.State = TypeControl.Normal;
                    }, obj1 => obj1 != null));

                    WindowPositionBehaviour.SetWindowScreen(accw, WindowPositionBehaviour.GetWindowScreen(accw.Owner));
                    WindowPositionBehaviour.SetWindowPosition(accw, WindowPositionBehaviour.GetWindowPosition((accw.Owner)));
                    WindowVisibilityBehaviour.SetIsVisible(accw, true);
                    
                }, obj => obj != null));
            }
        }

        private Command _removeRoleCommand;
        public Command RemoveRoleCommand {
            get {
                return _removeRoleCommand ?? (_removeRoleCommand = new Command(obj => {
                    var select = (Tuple<object, object>)obj;

                    Role role = context.Roles.FirstOrDefault(x => x.ID == ((Role)select.Item1).ID);

                    role.IsDelete = true;
                    ((Role)select.Item1).IsDelete = true;

                    context.SaveChanges();
                    WindowMessageBehaviour.SetMessage(((Window)select.Item2), string.Format(App.GetString("AdminRoleMessageRemove"), ((Role)select.Item1).RoleName), backColor: Colors.Green, foreColor: Colors.White);
                }, obj => obj != null));
            }
        }

        private Command _restoreRoleCommand;
        public Command RestoreRoleCommand {
            get {
                return _restoreRoleCommand ?? (_restoreRoleCommand = new Command(obj => {
                    var select = (Tuple<object, object>)obj;

                    Role role = context.Roles.FirstOrDefault(x => x.ID == ((Role)select.Item1).ID);

                    role.IsDelete = false;
                    ((Role)select.Item1).IsDelete = false;

                    context.SaveChanges();
                    WindowMessageBehaviour.SetMessage(((Window)select.Item2), string.Format(App.GetString("AdminRoleMessageRestore"), ((Role)select.Item1).RoleName), backColor: Colors.Green, foreColor: Colors.White);
                }, obj => obj != null));
            }
        }

        private Command _removeAllRolesCommand;
        public Command RemoveAllRolesCommand {
            get {
                return _removeAllRolesCommand ?? (_removeAllRolesCommand = new Command(obj => {
                    Window w = (Window)obj;
                    ListCollectionView f = (ListCollectionView)ViewByRoles;
                    int countOnExecution = 0;
                    if (f.Count > 0) {
                            foreach (Role item in f) {
                                Role result = context.Roles.FirstOrDefault(x => x.ID == item.ID);
                            if (result != null && !result.IsDelete) {
                                result.IsDelete = true;
                                item.IsDelete = true;
                                countOnExecution++;
                            }
                            context.SaveChanges();
                            WindowMessageBehaviour.SetMessage(w, string.Format(App.GetString("AdminRoleMessageRemoveAll"), countOnExecution), backColor: Colors.Green, foreColor: Colors.White);
                        }
                    }
                }, obj => obj != null));
            }
        }

        private Command _restoreAllRolesCommand;
        public Command RestoreAllRolesCommand {
            get {
                return _restoreAllRolesCommand ?? (_restoreAllRolesCommand = new Command(obj => {
                    Window w = (Window)obj;
                    ListCollectionView f = (ListCollectionView)ViewByRoles;
                    int countOnExecution = 0;
                    if (f.Count > 0) {
                        foreach (Role item in f) {
                            Role result = context.Roles.FirstOrDefault(x => x.ID == item.ID);
                            if (result != null && result.IsDelete) {
                                result.IsDelete = false;
                                item.IsDelete = false;
                                countOnExecution++;
                            }
                            context.SaveChanges();
                            WindowMessageBehaviour.SetMessage(w, string.Format(App.GetString("AdminRoleMessageRestoreAll"), countOnExecution), backColor: Colors.Green, foreColor: Colors.White);
                        }
                    }
                }, obj => obj != null));
            }
        }
        #endregion

        #region Settings
        private Command _createNewSettingCommand;
        public Command CreateNewSettingCommand {
            get {
                return _createNewSettingCommand ?? (_createNewSettingCommand = new Command(obj => {
                    this.State = TypeControl.Create;

                    var select = (Window)obj;

                    CloseWindowsByType<AdminControlChildWindow>();

                    SelectedObject = new Setting {
                        Caption = "New setting"
                    };

                    _acceptedControlCommand = new Command(wn => {

                        context.Settings.AddObject((Setting)SelectedObject);
                        Settings.Add((Setting)SelectedObject);
                        context.SaveChanges();
                        SelectedObject = null;
                        WindowVisibilityBehaviour.SetIsVisible((Window)wn, false);
                        this.State = TypeControl.Normal;
                    }, wn => wn != null);

                    AdminControlChildWindow accw = new AdminControlChildWindow
                    {
                        Owner = select
                    };

                    HideButtonBehaviour.SetCommand(accw, new Command((obj1) => {
                        if (this.State != TypeControl.Create)
                            context.Refresh(RefreshMode.StoreWins, SelectedObject);
                        SelectedObject = null;

                        WindowVisibilityBehaviour.SetIsVisible(accw, false);
                        this.State = TypeControl.Normal;
                    }, obj1 => obj1 != null));

                    WindowPositionBehaviour.SetWindowScreen(accw, WindowPositionBehaviour.GetWindowScreen(accw.Owner));
                    WindowPositionBehaviour.SetWindowPosition(accw, WindowPositionBehaviour.GetWindowPosition((accw.Owner)));
                    WindowVisibilityBehaviour.SetIsVisible(accw, true);
                }, obj => obj != null));
            }
        }

        private Command _editSettingCommand;
        public Command EditSettingCommand {
            get {
                return _editSettingCommand ?? (_editSettingCommand = new Command(obj => {
                    this.State = TypeControl.Edit;

                    var select = (Tuple<object, object>)obj;

                    CloseWindowsByType<AdminControlChildWindow>();

                    SelectedObject = context.Settings.FirstOrDefault(x => x.ID == ((Setting)select.Item1).ID);
                    EditObject = select.Item1;
                    _acceptedControlCommand = new Command(wn => {
                        context.SaveChanges();
                        int idPos = Settings.IndexOf((Setting)select.Item1);
                        Settings.RemoveAt(idPos);
                        Settings.Insert(idPos, (Setting)SelectedObject);
                        SelectedObject = null;
                        WindowVisibilityBehaviour.SetIsVisible((Window)wn, false);
                        EditObject = null;
                        this.State = TypeControl.Normal;
                    }, wn => wn != null);

                    AdminControlChildWindow accw = new AdminControlChildWindow
                    {
                        Owner = (Window)select.Item2
                    };

                    HideButtonBehaviour.SetCommand(accw, new Command((obj1) => {
                        context.Refresh(RefreshMode.StoreWins, SelectedObject);
                        SelectedObject = null;

                        WindowVisibilityBehaviour.SetIsVisible(accw, false);
                        EditObject = null;
                        this.State = TypeControl.Normal;
                    }, obj1 => obj1 != null));

                    WindowPositionBehaviour.SetWindowScreen(accw, WindowPositionBehaviour.GetWindowScreen(accw.Owner));
                    WindowPositionBehaviour.SetWindowPosition(accw, WindowPositionBehaviour.GetWindowPosition((accw.Owner)));
                    WindowVisibilityBehaviour.SetIsVisible(accw, true);
                    
                }, obj => obj != null));
            }
        }

        private Command _removeSettingCommand;
        public Command RemoveSettingCommand {
            get {
                return _removeSettingCommand ?? (_removeSettingCommand = new Command(obj => {
                    var select = (Tuple<object, object>)obj;

                    Setting setting = context.Settings.FirstOrDefault(x => x.ID == ((Setting)select.Item1).ID);

                    setting.IsDelete = true;
                    ((Setting)select.Item1).IsDelete = true;

                    context.SaveChanges();
                    WindowMessageBehaviour.SetMessage(((Window)select.Item2), string.Format(App.GetString("AdminSettingsMessageRemove"), ((Setting)select.Item1).Caption), backColor: Colors.Green, foreColor: Colors.White);
                }, obj => obj != null));
            }
        }

        private Command _restoreSettingCommand;
        public Command RestoreSettingCommand {
            get {
                return _restoreSettingCommand ?? (_restoreSettingCommand = new Command(obj => {
                    var select = (Tuple<object, object>)obj;

                    Setting setting = context.Settings.FirstOrDefault(x => x.ID == ((Setting)select.Item1).ID);

                    setting.IsDelete = false;
                    ((Setting)select.Item1).IsDelete = false;

                    context.SaveChanges();
                    WindowMessageBehaviour.SetMessage(((Window)select.Item2), string.Format(App.GetString("AdminSettingsMessageRestore"), ((Setting)select.Item1).Caption), backColor: Colors.Green, foreColor: Colors.White);
                }, obj => obj != null));
            }
        }

        private Command _removeAllSettingsCommand;
        public Command RemoveAllSettingsCommand {
            get {
                return _removeAllSettingsCommand ?? (_removeAllSettingsCommand = new Command(obj => {
                    Window w = (Window)obj;
                    ListCollectionView f = (ListCollectionView)ViewBySettings;
                    int countOnExecution = 0;
                    if (f.Count > 0) {
                        foreach (Setting item in f) {
                            Setting result = context.Settings.FirstOrDefault(x => x.ID == item.ID);
                            if (result != null && !result.IsDelete) {
                                result.IsDelete = true;
                                item.IsDelete = true;
                                countOnExecution++;
                            }
                            context.SaveChanges();
                            WindowMessageBehaviour.SetMessage(w, string.Format(App.GetString("AdminSettingsMessageRemoveAll"), countOnExecution), backColor: Colors.Green, foreColor: Colors.White);
                        }
                    }
                }, obj => obj != null));
            }
        }

        private Command _restoreAllSettingsCommand;
        public Command RestoreAllSettingsCommand {
            get {
                return _restoreAllSettingsCommand ?? (_restoreAllSettingsCommand = new Command(obj => {
                    Window w = (Window)obj;
                    ListCollectionView f = (ListCollectionView)ViewBySettings;
                    int countOnExecution = 0;
                    if (f.Count > 0) {
                        foreach (Setting item in f) {
                            Setting result = context.Settings.FirstOrDefault(x => x.ID == item.ID);
                            if (result != null && result.IsDelete) {
                                result.IsDelete = false;
                                item.IsDelete = false;
                                countOnExecution++;
                            }
                            context.SaveChanges();
                            WindowMessageBehaviour.SetMessage(w, string.Format(App.GetString("AdminSettingsMessageRestoreAll"), countOnExecution), backColor: Colors.Green, foreColor: Colors.White);
                        }
                    }
                }, obj => obj != null));
            }
        }
        #endregion

        #region Projects
        private Command _createNewProjectCommand;
        public Command CreateNewProjectCommand {
            get {
                return _createNewProjectCommand ?? (_createNewProjectCommand = new Command(obj => {
                    this.State = TypeControl.Create;

                    var select = (Window)obj;

                    CloseWindowsByType<AdminControlChildWindow>();

                    SelectedObject = new Project {
                        Caption = "New project",
                        Description = string.Empty
                    };

                    _acceptedControlCommand = new Command(wn => {

                        context.Projects.AddObject((Project)SelectedObject);
                        Projects.Add((Project)SelectedObject);
                        context.SaveChanges();
                        SelectedObject = null;
                        WindowVisibilityBehaviour.SetIsVisible((Window)wn, false);
                        this.State = TypeControl.Normal;
                        this.State = TypeControl.Normal;
                    }, wn => wn != null);

                    AdminControlChildWindow accw = new AdminControlChildWindow
                    {
                        Owner = select
                    };

                    HideButtonBehaviour.SetCommand(accw, new Command((obj1) => {
                        if (this.State != TypeControl.Create)
                            context.Refresh(RefreshMode.StoreWins, SelectedObject);
                        SelectedObject = null;

                        WindowVisibilityBehaviour.SetIsVisible(accw, false);
                        this.State = TypeControl.Normal;
                    }, obj1 => obj1 != null));

                    WindowPositionBehaviour.SetWindowScreen(accw, WindowPositionBehaviour.GetWindowScreen(accw.Owner));
                    WindowPositionBehaviour.SetWindowPosition(accw, WindowPositionBehaviour.GetWindowPosition((accw.Owner)));
                    WindowVisibilityBehaviour.SetIsVisible(accw, true);
                }, obj => obj != null));
            }
        }

        private Command _editProjectCommand;
        public Command EditProjectCommand {
            get {
                return _editProjectCommand ?? (_editProjectCommand = new Command(obj => {
                    this.State = TypeControl.Edit;

                    var select = (Tuple<object, object>)obj;

                    CloseWindowsByType<AdminControlChildWindow>();

                    SelectedObject = context.Projects.FirstOrDefault(x => x.ID == ((Project)select.Item1).ID);
                    EditObject = select.Item1;
                    _acceptedControlCommand = new Command(wn => {
                        context.SaveChanges();
                        int idPos = Projects.IndexOf((Project)select.Item1);
                        Projects.RemoveAt(idPos);
                        Projects.Insert(idPos, (Project)SelectedObject);
                        SelectedObject = null;
                        WindowVisibilityBehaviour.SetIsVisible((Window)wn, false);
                        EditObject = null;
                        this.State = TypeControl.Normal;
                    }, wn => wn != null);

                    AdminControlChildWindow accw = new AdminControlChildWindow
                    {
                        Owner = (Window)select.Item2
                    };

                    HideButtonBehaviour.SetCommand(accw, new Command((obj1) => {
                        context.Refresh(RefreshMode.StoreWins, SelectedObject);
                        SelectedObject = null;

                        WindowVisibilityBehaviour.SetIsVisible(accw, false);
                        EditObject = null;
                        this.State = TypeControl.Normal;
                    }, obj1 => obj1 != null));

                    WindowPositionBehaviour.SetWindowScreen(accw, WindowPositionBehaviour.GetWindowScreen(accw.Owner));
                    WindowPositionBehaviour.SetWindowPosition(accw, WindowPositionBehaviour.GetWindowPosition((accw.Owner)));
                    WindowVisibilityBehaviour.SetIsVisible(accw, true);
                    this.State = TypeControl.Normal;
                }, obj => obj != null));
            }
        }

        private Command _removeProjectCommand;
        public Command RemoveProjectCommand {
            get {
                return _removeProjectCommand ?? (_removeProjectCommand = new Command(obj => {
                    var select = (Tuple<object, object>)obj;

                    Project setting = context.Projects.FirstOrDefault(x => x.ID == ((Project)select.Item1).ID);

                    setting.IsDelete = true;
                    ((Project)select.Item1).IsDelete = true;

                    context.SaveChanges();

                    VerticalProjects = null;
                    VerticalProjects = Projects;

                    WindowMessageBehaviour.SetMessage(((Window)select.Item2), string.Format(App.GetString("AdminProjectMessageRemove"), ((Project)select.Item1).Caption), backColor: Colors.Green, foreColor: Colors.White);
                }, obj => obj != null));
            }
        }

        private Command _restoreProjectCommand;
        public Command RestoreProjectCommand {
            get {
                return _restoreProjectCommand ?? (_restoreProjectCommand = new Command(obj => {
                    var select = (Tuple<object, object>)obj;

                    Project project = context.Projects.FirstOrDefault(x => x.ID == ((Project)select.Item1).ID);

                    project.IsDelete = false;
                    ((Project)select.Item1).IsDelete = false;

                    context.SaveChanges();

                    VerticalProjects = null;
                    VerticalProjects = Projects;

                    WindowMessageBehaviour.SetMessage(((Window)select.Item2), string.Format(App.GetString("AdminProjectMessageRestore"), ((Project)select.Item1).Caption), backColor: Colors.Green, foreColor: Colors.White);
                }, obj => obj != null));
            }
        }

        private Command _removeAllProjectCommand;
        public Command RemoveAllProjectCommand {
            get {
                return _removeAllProjectCommand ?? (_removeAllProjectCommand = new Command(obj => {
                    Window w = (Window)obj;
                    ListCollectionView f = (ListCollectionView)ViewByProjects;
                    int countOnExecution = 0;
                    if (f.Count > 0) {
                            foreach (Project item in f) {
                                Project result = context.Projects.FirstOrDefault(x => x.ID == item.ID);
                                if (result != null && !result.IsDelete) {
                                    result.IsDelete = true;
                                    item.IsDelete = true;
                                    countOnExecution++;
                                }
                            context.SaveChanges();

                            VerticalProjects = null;
                            VerticalProjects = Projects;

                            WindowMessageBehaviour.SetMessage(w, string.Format(App.GetString("AdminProjectMessageRemoveAll"), countOnExecution), backColor: Colors.Green, foreColor: Colors.White);
                        }
                    }
                }, obj => obj != null));
            }
        }

        private Command _restoreAllProjectCommand;
        public Command RestoreAllProjectCommand {
            get {
                return _restoreAllProjectCommand ?? (_restoreAllProjectCommand = new Command(obj => {
                    Window w = (Window)obj;
                    ListCollectionView f = (ListCollectionView)ViewByProjects;
                    int countOnExecution = 0;
                    if (f.Count > 0) {
                        foreach (Project item in f) {
                            Project result = context.Projects.FirstOrDefault(x => x.ID == item.ID);
                            if (result != null && result.IsDelete) {
                                result.IsDelete = false;
                                item.IsDelete = false;
                                countOnExecution++;
                            }
                        }
                        context.SaveChanges();

                        VerticalProjects = null;
                        VerticalProjects = Projects;

                        WindowMessageBehaviour.SetMessage(w, string.Format(App.GetString("AdminProjectMessageRestoreAll"), countOnExecution), backColor: Colors.Green, foreColor: Colors.White);
                    }
                }, obj => obj != null));
            }
        }
        #endregion

        #region Unload
        private Command _unloadedCommand;
        public Command UnloadedCommand {
            get {
                return _unloadedCommand ?? (_unloadedCommand = new Command(obj => {
                    _tasks = null;
                    _viewByTasks = null;
                    _users = null;
                    _viewByUsers = null;
                    _roles = null;
                    _viewByRoles = null;
                    _settings = null;
                    _viewBySettings = null;
                    _projects = null;
                    _viewByProjects = null;
                }));
            }
        }
        #endregion
        #endregion

        #region Members
        private DBContext _context;
        protected DBContext context {
            get { return _context ?? (_context = new DBContext(true)); }
        }

        private int _currentAccess = -1;

        public int CurrentAccess
        {
            get { return _currentAccess; }
            set {
                _currentAccess = value;
                this.SendPropertyChanged(nameof(CurrentAccess));
            }
        }

        private object _selectedObject;
        public object SelectedObject {
            get { return _selectedObject; }
            set {
                _selectedObject = value;
                this.SendPropertyChanged(nameof(SelectedObject));
            }
        }

        private double _procentPanelSize = 20;
        public double ProcentPanelSize {
            get { return _procentPanelSize; }
            set { _procentPanelSize = value; this.SendPropertyChanged(nameof(ProcentPanelSize)); }
        }

        public IEnumerable<DoubleStruct> AccessList {
            get {
                List<DoubleStruct> result = ((IEnumerable<DoubleStruct>)Application.Current.Resources["ArrayItemsOnAdminControl"]).ToList();
                User cu = App.CurrentUser;
                if (!cu.Role.ViewAllTask && !cu.Role.RestoreTask)
                    result.Remove(result.FirstOrDefault(x => x.Value == 1));
                if (!cu.Role.CreateUser && !cu.Role.UpdateInfoUser && !cu.Role.DeleteUser && !cu.Role.RestoreUser)
                    result.Remove(result.FirstOrDefault(x => x.Value == 2));
                if (!cu.Role.CreateNewRole && !cu.Role.UpdateRole && !cu.Role.DeleteRole && !cu.Role.RestoreRole)
                    result.Remove(result.FirstOrDefault(x => x.Value == 3));
                if (!cu.Role.CreateSetting && !cu.Role.UpdateSetting && !cu.Role.DeleteSetting && !cu.Role.RestoreSetting)
                    result.Remove(result.FirstOrDefault(x => x.Value == 4));
                if (!cu.Role.CreateProject && !cu.Role.UpdateProject && !cu.Role.DeleteProject && !cu.Role.RestoreProject)
                    result.Remove(result.FirstOrDefault(x => x.Value == 5));
                if (!cu.Role.VisibleJournal)
                    result.Remove(result.FirstOrDefault(x => x.Value == 6));
                return result;
            }
        }

        #region Tasks members
        private ObservableCollection<Task> _tasks;
        public ObservableCollection<Task> Tasks {
            get {
                return _tasks ?? (_tasks = GetDeletedTask());
            }
        }

        private ICollectionView _viewByTasks;
        public ICollectionView ViewByTasks {
            get { return _viewByTasks ?? (_viewByTasks = GetView()); }
        }

        private string _searchText = string.Empty;
        public string SearchText {
            get { return _searchText; }
            set {
                _searchText = value;
                string h = value.ToUpper();
                ViewByTasks.Filter = new Predicate<object>((x) => 
                    ((Task)x).Caption.ToUpper().Contains(h) | 
                    ((Task)x).Text.ToUpper().Contains(h) | 
                    ((Task)x).CreaterName.ToUpper().Contains(h) |
                    ((Task)x).TypeString.ToUpper().Contains(h) | 
                    ((Task)x).PrioretyString.ToUpper().Contains(h) |
                    ((Task)x).ProjectString.ToUpper().Contains(h));
                this.SendPropertyChanged(nameof(SearchText));

            }
        }

        private object _editObject;
        public object EditObject {
            get { return _editObject; }
            set { _editObject = value; this.SendPropertyChanged(nameof(EditObject)); }
        }
        #endregion

        #region Users members
        private ObservableCollection<User> _users;
        public ObservableCollection<User> Users {
            get { return _users ?? (_users = GetAllUsers()); }
        }

        private ICollectionView _viewByUsers;
        public ICollectionView ViewByUsers {
            get { return _viewByUsers ?? (_viewByUsers = GetUsersView()); }
        }

        private string _searchTextUser = string.Empty;
        public string SearchTextUser {
            get { return _searchTextUser; }
            set {
                _searchTextUser = value;
                string h = value.ToUpper();
                ViewByUsers.Filter = new Predicate<object>((x) =>
                ((User)x).FullName.ToUpper().Contains(h) |
                ((User)x).UserName.ToUpper().Contains(h));
                this.SendPropertyChanged(nameof(SearchTextUser));
            }
        }

        public IEnumerable<NTW.Data.Info> RolesNameList {
            get {
                IEnumerable<NTW.Data.Info> result = null;
                using (DBContext context = new DBContext(false))
                    result = context.Roles.Where(x => !x.IsDelete).Select(x => new NTW.Data.Info { Caption = x.RoleName, ID = x.ID }).ToArray();
                return result;
            }
        }

        public IEnumerable<NTW.Data.Info> SettingsNamesList {
            get {
                IEnumerable<NTW.Data.Info> result = null;
                using (DBContext context = new DBContext(false))
                    result = context.Settings.Where(x => !x.IsDelete).Select(x => new NTW.Data.Info { Caption = x.Caption, ID = x.ID }).ToArray();
                return result;
            }
        }
        #endregion

        #region Role members
        private ObservableCollection<Role> _roles;
        public ObservableCollection<Role> Roles {
            get { return _roles ?? (_roles = GetAllRoles()); }
        }

        private ICollectionView _viewByRoles;
        public ICollectionView ViewByRoles {
            get { return _viewByRoles ?? (_viewByRoles = GetRolesView()); }
        }

        private string _searchTextRole = string.Empty;
        public string SearchTextRole {
            get { return _searchTextRole; }
            set {
                _searchTextRole = value;
                string h = value.ToUpper();
                ViewByRoles.Filter = new Predicate<object>((x) =>
                ((Role)x).RoleName.ToUpper().Contains(h));
                this.SendPropertyChanged(nameof(SearchTextRole));
            }
        }
        #endregion

        #region Settings members
        private ObservableCollection<Setting> _settings;
        public ObservableCollection<Setting> Settings {
            get { return _settings ?? (_settings = GetAllSettings()); }
        }

        private ICollectionView _viewBySettings;
        public ICollectionView ViewBySettings {
            get { return _viewBySettings ?? (_viewBySettings = GetSettingsView()); }
        }

        private string _searchTextSettings = string.Empty;
        public string SearchTextSettings {
            get { return _searchTextSettings; }
            set {
                _searchTextSettings = value;
                string h = value.ToUpper();
                ViewBySettings.Filter = new Predicate<object>((x) =>
                ((Setting)x).Caption.ToUpper().Contains(h));
                this.SendPropertyChanged(nameof(SearchTextSettings));
            }
        }
        #endregion

        #region Projects Members
        private ObservableCollection<Project> _projects;
        public ObservableCollection<Project> Projects {
            get { return _projects ?? (_projects = GetAllProjects()); }
        }

        private ICollectionView _viewByProjects;
        public ICollectionView ViewByProjects {
            get { return _viewByProjects ?? (_viewByProjects = GetProjectsView()); }
        }

        private string _searchTextProjects = string.Empty;
        public string SearchTextProjects {
            get { return _searchTextProjects; }
            set {
                _searchTextProjects = value;
                string h = value.ToUpper();
                ViewByProjects.Filter = new Predicate<object>((x) =>
                ((Project)x).Caption.ToUpper().Contains(h));
                this.SendPropertyChanged(nameof(SearchTextProjects));
            }
        }

        private IEnumerable<Project> _verticalProjects;
        public IEnumerable<Project> VerticalProjects {
            get { return _verticalProjects; }
            set { _verticalProjects = value; this.SendPropertyChanged(nameof(VerticalProjects)); }
        }

        public Func<object, Tuple<int, int, int, int, Color>> VerticalProjectMetric {
            get {
                return (item) =>
                {
                    if (item != null)
                    {
                        var p = (Project)item;
                        return new Tuple<int, int, int, int, Color>(p.IsDelete ? 0 : 1, Projects.IndexOf(p), 1, 1, !p.IsDelete ? Colors.Green : Colors.Maroon);
                    }
                    else return null;
                };
            }
        }
        #endregion

        #region Journal members
        private DateTime _journalStartDate = DateTime.Now.Date;
        public DateTime JournalStartDate {
            get { return _journalStartDate; }
            set {
                _journalStartDate = value.Date;
                this.SendPropertyChanged(nameof(JournalStartDate));

                if (_journalEndDate < _journalStartDate) {
                    _journalEndDate = _journalStartDate;
                    this.SendPropertyChanged(nameof(JournalEndDate));
                }

                _journalValues = null;
                _journalCollectionView = null;
                this.SendPropertyChanged(nameof(JournalCollectionView));
            }
        }

        private DateTime _journalEndDate = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
        public DateTime JournalEndDate {
            get { return _journalEndDate; }
            set {
                _journalEndDate = value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                this.SendPropertyChanged(nameof(JournalEndDate));

                if (_journalEndDate < _journalStartDate) {
                    _journalStartDate = _journalEndDate;
                    this.SendPropertyChanged(nameof(JournalValues));
                }

                _journalValues = null;
                _journalCollectionView = null;
                this.SendPropertyChanged(nameof(JournalCollectionView));
            }
        }

        private ObservableCollection<Journal> _journalValues;
        public ObservableCollection<Journal> JournalValues {
            get { return _journalValues ?? (_journalValues = GetJournalValues()); }
        }

        private ICollectionView _journalCollectionView;
        public ICollectionView JournalCollectionView {
            get { return _journalCollectionView ?? (_journalCollectionView = GetJournalCollection()); }
        }
        #endregion
        #endregion

        #region Helps
        #region Tasks helps
        protected ObservableCollection<Task> GetDeletedTask() {
            IEnumerable<Task> result = null;
            //пака без условий
            using (DBContext ccontext = new DBContext(false))
                result = ccontext.Tasks.Where(x => x.IsDelete).ToArray();

            return new ObservableCollection<Task>(result);

        }

        protected ICollectionView GetView() {
            return CollectionViewSource.GetDefaultView(Tasks);
        }
        #endregion

        #region Users helps
        protected ObservableCollection<User> GetAllUsers() {
            IEnumerable<User> result = null;
            //пака без условий
            using (DBContext ccontext = new DBContext(false))
                result = ccontext.Users.ToArray();

            return new ObservableCollection<User>(result);
        }

        protected ICollectionView GetUsersView() {
            return CollectionViewSource.GetDefaultView(Users);
        }
        #endregion

        #region Roles helps
        protected ObservableCollection<Role> GetAllRoles() {
            IEnumerable<Role> result = null;
            //пака без условий
            using (DBContext ccontext = new DBContext(false))
                result = ccontext.Roles.ToArray();

            return new ObservableCollection<Role>(result);
        }

        protected ICollectionView GetRolesView() {
            return CollectionViewSource.GetDefaultView(Roles);
        }
        #endregion

        #region Settings helps
        protected ObservableCollection<Setting> GetAllSettings() {
            IEnumerable<Setting> result = null;
            //пака без условий
            using (DBContext ccontext = new DBContext(false))
                result = ccontext.Settings.ToArray();

            return new ObservableCollection<Setting>(result);
        }

        protected ICollectionView GetSettingsView() {
            return CollectionViewSource.GetDefaultView(Settings);
        }
        #endregion

        #region Projects helps
        protected ObservableCollection<Project> GetAllProjects() {
            IEnumerable<Project> result = null;
            //пака без условий
            using (DBContext ccontext = new DBContext(false))
                result = ccontext.Projects.ToArray();

            VerticalProjects = result;

            return new ObservableCollection<Project>(result);
        }

        protected ICollectionView GetProjectsView() {
            return CollectionViewSource.GetDefaultView(Projects);
        }
        #endregion

        #region Journal helps
        private ObservableCollection<Journal> GetJournalValues() {
            IEnumerable<Journal> result = null;
            using (DBContext ccontext = new DBContext(false))
                result = ccontext.Journals.Where(x => x.CreateDate >= JournalStartDate && x.CreateDate <= JournalEndDate && x.TypeMessage != 0).ToArray();
            return new ObservableCollection<Journal>(result);
        }

        private ICollectionView GetJournalCollection() {
            ICollectionView view = CollectionViewSource.GetDefaultView(JournalValues);
            view.GroupDescriptions.Add(new PropertyGroupDescription("UserName"));
            //view.GroupDescriptions.Add(new PropertyGroupDescription("CreateDate.Date"));
            return view;
        }
        #endregion

        protected void CloseWindowsByType<T>() {
            foreach (Window item in App.Current.Windows)
                if (item is T)
                    WindowVisibilityBehaviour.SetIsVisible(item, false);
        }
        #endregion

        #region Statick helps
        public static void SendDataJournal(TypeMessage type, User user, Task task = null, User sUser = null, Setting userSettings = null, Project project = null, DateTime? createDate = null) {
            var parameter = SendMessage(type, user, task, user, userSettings, project);

            using (DBContext context = new DBContext(false)) {
                var newEntry = new Journal {
                    TypeMessage = (int)type,
                    CreateDate = createDate != null ? (DateTime)createDate : DateTime.Now,
                    IdUser = user.ID,
                    UserName = user.UserName,
                    ValueMessage = user.ID.ToString(),
                    SecondMessageValue = string.Format("[{0}]", ((int[])parameter).Select(x => x.ToString()).Aggregate((i, j) => i + ", " + j))
                };

                context.Journals.AddObject(newEntry);
                context.SaveChanges();
            }
        }

        protected static int[] SendMessage(TypeMessage type, User user, Task task = null, User sUser = null, Setting userSettings = null, Project project = null, Role role = null) {
            // отбираем пользоватлей для оповещения
            IEnumerable<string> ips = null;
            int[] parametry = new int[0];

            using (DBContext context = new DBContext(false)) {
                switch (type) {
                    // для исполнителей и ответственных лиц
                    case TypeMessage.AddComment:
                    case TypeMessage.ChangedTask:
                    case TypeMessage.CreateTask:
                    case TypeMessage.DeleteTask:
                        ips = context.Users.Where(x => x.IpAdress != string.Empty).ToArray().Where(x => task.Perfomers.FirstOrDefault(u => u.IDUser == x.ID) != null || task.Creater == x.ID).Select(x => x.IpAdress).ToArray();
                        parametry = new int[] { task.ID, user.ID };
                        break;
                    // для ответственных лиц
                    case TypeMessage.CancelingExecution:
                    case TypeMessage.ComplitedExecution:
                    case TypeMessage.StartExecution:
                        ips = context.Users.Where(x => x.IpAdress != string.Empty).ToArray().Where(x => task.Perfomers.FirstOrDefault(u => u.IDUser == x.ID && u.PersonInCharge) != null || task.Creater == x.ID).Select(x => x.IpAdress).ToArray();
                        parametry = new int[] { task.ID, sUser.ID };
                        break;
                    // для администраторов
                    case TypeMessage.RestoreProject:
                    case TypeMessage.DeleteProject:
                    case TypeMessage.CreateProject:
                    case TypeMessage.ChangingProject:
                        ips = context.Users.Where(x => x.IpAdress != string.Empty 
                        && (x.Role.RestoreProject 
                        || x.Role.DeleteRole
                        || x.Role.CreateProject
                        || x.Role.UpdateRole)).Select(x => x.IpAdress).ToArray();
                        parametry = new int[] { project.ID};
                        break;
                    case TypeMessage.RestoreRole:
                    case TypeMessage.DeleteRole:
                    case TypeMessage.CreateRole:
                    case TypeMessage.ChangingRole:
                        ips = context.Users.Where(x => x.IpAdress != string.Empty
                        && (x.Role.RestoreRole 
                        || x.Role.DeleteRole 
                        || x.Role.CreateNewRole
                        || x.Role.UpdateRole)).Select(x => x.IpAdress).ToArray();
                        parametry = new int[] { role.ID};
                        break;
                    case TypeMessage.RestoreUser:
                    case TypeMessage.DeleteUser:
                    case TypeMessage.CreateUser:
                        ips = context.Users.Where(x => x.IpAdress != string.Empty
                        && (x.Role.RestoreUser
                        || x.Role.DeleteUser
                        || x.Role.CreateUser)).Select(x => x.IpAdress).ToArray();
                        parametry = new int[] { user.ID};
                        break;
                    case TypeMessage.RestoreUserSetting:
                    case TypeMessage.DeleteUserSetting:
                    case TypeMessage.CreateUserSetting:
                        ips = context.Users.Where(x => x.IpAdress != string.Empty
                        && (x.Role.RestoreSetting
                        || x.Role.DeleteSetting
                        || x.Role.CreateSetting)).Select(x => x.IpAdress).ToArray();
                        parametry = new int[] { userSettings.ID };
                        break;
                    // для пользователя
                    case TypeMessage.ChangingUser:
                    case TypeMessage.ChangingUserSetting:
                        ips = context.Users.Where(x => x.IpAdress != string.Empty).Select(x => x.IpAdress).ToArray();
                        parametry = new int[] { user.ID };
                        break;
                    // для всех
                    case TypeMessage.UserEnter:
                    case TypeMessage.UserExit:
                        ips = context.Users.Where(x => x.IpAdress != string.Empty).Select(x => x.IpAdress).ToArray();
                        parametry = new int[] { user.ID };
                        break;
                }

                return parametry;
            }

            foreach (string ip in ips)
            {
                NTW.Communication.Beginers.ClientBeginer.Send(ip, 8810, "commands", (int)type, user.ID, parametry);
            }
        }
        #endregion
    }
}
