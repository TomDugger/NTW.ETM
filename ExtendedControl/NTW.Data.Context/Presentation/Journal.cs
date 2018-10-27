using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace NTW.Data.Context//.Presentation
{
    public partial class Journal
    {
        public string PresentValue {
            get {
                string result = string.Empty;
                string[] g = (string[])Application.Current.Resources["EventMessages"];
                result = string.Format(g[this.TypeMessage], GetParametry((NTW.Data.TypeMessage)this.TypeMessage, Convert.ToInt32(this.ValueMessage), this.SecondMessageValue.Replace("[", "").Replace("]", "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt32(x)).ToArray()));
                return result;
            }
        }

        #region helps
        protected string[] GetParametry(TypeMessage type, int index, int[] parameter)
        {
            string[] result = new string[4];
            using (DBContext context = new DBContext(false))
            {
                result[0] = context.Users.FirstOrDefault(x => x.ID == index)?.FullName;
                int i = 0;
                if (parameter.Length != 0)
                    switch (type)
                    {
                        case NTW.Data.TypeMessage.CreateTask:
                        case NTW.Data.TypeMessage.ChangedTask:
                        case NTW.Data.TypeMessage.DeleteTask:
                        case NTW.Data.TypeMessage.RestoreTask:
                        case NTW.Data.TypeMessage.StartExecution:
                        case NTW.Data.TypeMessage.ComplitedExecution:
                        case NTW.Data.TypeMessage.CancelingExecution:
                            i = parameter[0];
                            var j = parameter[1];
                            result[1] = context.Tasks.FirstOrDefault(x => x.ID == i)?.Caption;
                            result[2] = context.Users.FirstOrDefault(x => x.ID == j)?.FullName;
                            break;
                        case NTW.Data.TypeMessage.CreateUser:
                        case NTW.Data.TypeMessage.ChangingUser:
                        case NTW.Data.TypeMessage.DeleteUser:
                        case NTW.Data.TypeMessage.RestoreUser:
                            i = parameter[0];
                            result[1] = context.Users.FirstOrDefault(x => x.ID == i)?.FullName;
                            break;
                        case NTW.Data.TypeMessage.CreateRole:
                        case NTW.Data.TypeMessage.ChangingRole:
                        case NTW.Data.TypeMessage.DeleteRole:
                        case NTW.Data.TypeMessage.RestoreRole:
                            i = parameter[0];
                            result[1] = context.Roles.FirstOrDefault(x => x.ID == i)?.RoleName;
                            break;
                        case NTW.Data.TypeMessage.CreateUserSetting:
                        case NTW.Data.TypeMessage.ChangingUserSetting:
                        case NTW.Data.TypeMessage.DeleteUserSetting:
                        case NTW.Data.TypeMessage.RestoreUserSetting:
                            i = parameter[0];
                            result[1] = context.Settings.FirstOrDefault(x => x.ID == i)?.Caption;
                            break;
                        case NTW.Data.TypeMessage.CreateProject:
                        case NTW.Data.TypeMessage.ChangingProject:
                        case NTW.Data.TypeMessage.DeleteProject:
                        case NTW.Data.TypeMessage.RestoreProject:
                            i = parameter[0];
                            result[1] = context.Projects.FirstOrDefault(x => x.ID == i)?.Caption;
                            break;
                        case NTW.Data.TypeMessage.AddComment:
                            i = parameter[0];
                            var com = context.TaskComments.FirstOrDefault(x => x.ID == i);
                            result[1] = com?.Commentary;
                            break;
                    }
            }
            return result;
        }
        #endregion
    }
}
