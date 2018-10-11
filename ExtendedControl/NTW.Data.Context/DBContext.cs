using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using NTW.Data.Connections;
using NTW.Data.Models;

namespace NTW.Data.Context
{
    public partial class DBContext
    {
        public DBContext(bool LazyLoading)
            : base(((DBSettings)Application.Current.Resources["DBSettings"]).Connection.GetConnectionString(), "DBContext")
        {
            this.ContextOptions.LazyLoadingEnabled = LazyLoading;
            OnContextCreated();
        }

        public static bool TestConnection(IConnection Settings)
        {
            bool result = false;
            using (DBContext context = new DBContext(Settings.GetConnectionString()))
            {
                try
                {
                    result = context.DatabaseExists();
                    User s = context.Users.FirstOrDefault();
                    result = true;
                }
                catch
                {
                    result = false;
                }
            }
            return result;
        }

        public bool CreateDatabase(bool createStartItem)
        {
            bool result = false;
            try
            {
                this.CreateDatabase();
                //создание предопределенных значений
                for (int i = 0; i < 3; i++)
                {
                    Project p = new Project { IsDelete = false, Caption = "Project " + (i + 1), Description = string.Empty };
                    this.Projects.AddObject(p);
                }

                //User settings
                Setting s = new Setting { Caption = "All users", AlwaysSpecifyAPlaceToDdownload = false, ExitCheck = false, IsDelete = false, UseAnimation = true };
                this.Settings.AddObject(s);

                //User roles
                //Admin
                Role adm = new Role
                {
                    IsDelete = false,
                    AllowCancellation = true,
                    CreateProject = true,
                    CreateNewRole = true,
                    CreateSetting = true,
                    CreateTask = true,
                    CreateUser = true,
                    DeleteProject = true,
                    DeleteRole = true,
                    DeleteSetting = true,
                    DeleteTask = true,
                    DeleteUser = true,
                    ExecutionTask = true,
                    ReportExecuteTask = true,
                    RestoreProject = true,
                    RestoreRole = true,
                    RestoreSetting = true,
                    RestoreTask = true,
                    RestoreUser = true,
                    RoleName = "Admin",
                    UpdateInfoTask = true,
                    UpdateInfoUser = true,
                    UpdateProject = true,
                    UpdateRole = true,
                    UpdateSetting = true,
                    ViewAllTask = true,
                    VisibleJournal = true
                };
                this.Roles.AddObject(adm);
                //Emp
                Role emp = new Role
                {
                    IsDelete = false,
                    RoleName = "Employee",
                    CreateTask = true,
                    ExecutionTask = true,
                };
                this.Roles.AddObject(emp);
                //Manager
                Role man = new Role
                {
                    IsDelete = false,
                    RoleName = "Manager",
                    ExecutionTask = true,
                    ViewAllTask = true,
                    ReportExecuteTask = true,
                    CreateProject = true,
                    UpdateProject = true,
                    DeleteProject = true
                };
                this.Roles.AddObject(man);
                //Coordinator
                Role cor = new Role
                {
                    IsDelete = false,
                    RoleName = "Coordinator",
                    CreateTask = true,
                    UpdateInfoTask = true,
                    ExecutionTask = true,
                    DeleteTask = true,
                    ViewAllTask = true,
                    CreateProject = true,
                    UpdateProject = true,
                    DeleteProject = true,
                    VisibleJournal = true
                };
                this.Roles.AddObject(cor);
                //Standart control
                Role sc = new Role
                {
                    IsDelete = false,
                    RoleName = "Standart control",
                    UpdateInfoTask = true,
                    ExecutionTask = true,
                    DeleteTask = true,
                    ViewAllTask = true,
                    CreateProject = true,
                    UpdateProject = true,
                    DeleteProject = true,
                    VisibleJournal = true
                };
                this.Roles.AddObject(sc);
                this.SaveChanges();

                //Users
                //admin
                User Admin = new User
                {
                    IsDelete = false,
                    UserName = "Admin",
                    Password = "AKVFbE4MYDhZuUS6T6bg2eZkF0La5pckDnoRrTwrpEJNeBclD8sBFD7Dxx1sBHHcyw==",
                    IdRole = adm.ID,
                    IdSetting = s.ID,
                    FullName = "Администратор",
                    Language = "ru",
                    IpAdress = string.Empty
                };
                this.Users.AddObject(Admin);

                //user
                User u = new User
                {
                    IsDelete = false,
                    UserName = "User",
                    Password = "AKVFbE4MYDhZuUS6T6bg2eZkF0La5pckDnoRrTwrpEJNeBclD8sBFD7Dxx1sBHHcyw==",
                    IdRole = emp.ID,
                    IdSetting = s.ID,
                    FullName = "Сотрудник",
                    Language = "en",
                    IpAdress = string.Empty
                };
                this.Users.AddObject(u);

                this.SaveChanges();

                NoteGroup ng = new NoteGroup
                {
                    Caption = string.Empty,
                    IDColor = 0,
                    PositionInList = 0
                };

                this.NoteGroups.AddObject(ng);
                this.SaveChanges();

                result = true;
            }
            catch(Exception ex) { result = false; }
            return result;
        }
    }
}
