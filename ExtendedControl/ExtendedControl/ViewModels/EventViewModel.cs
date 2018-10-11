using Info.Commands;
using NTW.Attrebute;
using NTW.Communication.Services;
using NTW.Data;
using NTW.Data.Context;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Threading;

namespace ExtendedControl.ViewModels
{
    [ReportStat]
    public class EventViewModel: ControlViewModel
    {
        #region Commands
        private Command _clickMessageCommand;
        public Command ClickMessageCommand {
            get {
                return _clickMessageCommand ?? (_clickMessageCommand = new Command(obj =>
                {
                    EventItem item = (EventItem)obj;
                    item.Timer.Stop();
                }, obj => obj is EventItem));
            }
        }

        private Command _removeItemCommand;
        public Command RemoveItemCommand {
            get {
                return _removeItemCommand ?? (_removeItemCommand = new Command(obj =>
                {
                    EventItem item = (EventItem)obj;
                    item.Timer.Stop();
                    Events.Remove(item);
                }, obj => obj is EventItem));
            }
        }


        private Command _sendCommentCommand;
        public Command SendCommentCommand {
            get {
                return _sendCommentCommand ?? (_sendCommentCommand = new Command(obj =>
                {
                    if (this.CommentText != null && this.CommentText.Replace(" ", "").Length > 0)
                    {
                        var item = (EventItem)obj;

                        using (DBContext context = new DBContext(false))
                        { 
                            TaskComment newComment = new TaskComment();
                            newComment.IDUser = CurrentUser.ID;
                            newComment.IdTask = item.Parametry[1];
                            newComment.CreateDate = DateTime.Now;
                            newComment.Commentary = this.CommentText;
                            newComment.TypeCommentary = 0;

                            context.TaskComments.AddObject(newComment);
                            context.SaveChanges();
                        }
                        this.CommentText = "";

                        Events.Remove(item);
                    }
                }, obj => obj is EventItem));
            }
        }



        #endregion

        #region Members
        private ObservableCollection<EventItem> _events;
        public ObservableCollection<EventItem> Events {
            get { return _events ?? (_events = GetEvents()); }
        }

        private string _commentText;
        public string CommentText {
            get { return _commentText; }
            set { _commentText = value; this.SendPropertyChanged(nameof(CommentText)); }
        }
        #endregion

        #region Helps
        public ObservableCollection<EventItem> GetEvents() {
            CommandService.CommandEvent += (type, index, parameter) => {
                DispatcherTimer dt = new DispatcherTimer();
                dt.Interval = new TimeSpan(0, 0, 4);

                TypeMessage typeM = (TypeMessage)type;
                string[] g = (string[])App.Current.Resources["EventMessages"];
                string message = string.Format(g[type], GetParametry(typeM, index, parameter));

                var item = new EventItem(string.Format(g[type], GetParametry(typeM, index, parameter)), typeM, dt, GetColorMessage(typeM), GetColorMessage(typeM, true)) {
                    Index = index, Parametry = parameter
                };
                dt.Tick += (sender, e) => {
                        Events.Remove(item);
                };
                dt.Start();
                Events.Add(item);
            };

            return new ObservableCollection<EventItem>();
        }

        public string[] GetParametry(TypeMessage type, int index, int[] parameter) {
            string[] result = new string[4];
            using (DBContext context = new DBContext(false)) {
                result[0] = context.Users.FirstOrDefault(x => x.ID == index)?.FullName;
                int i = 0;
                if (parameter.Length != 0)
                    switch (type) {
                        case TypeMessage.CreateTask:
                        case TypeMessage.ChangedTask:
                        case TypeMessage.DeleteTask:
                        case TypeMessage.RestoreTask:
                        case TypeMessage.StartExecution:
                        case TypeMessage.ComplitedExecution:
                        case TypeMessage.CancelingExecution:
                            i = parameter[0];
                            var j = parameter[1];
                            result[1] = context.Tasks.FirstOrDefault(x => x.ID == i)?.Caption;
                            result[2] = context.Users.FirstOrDefault(x => x.ID == j)?.FullName;
                            break;
                        case TypeMessage.CreateUser:
                        case TypeMessage.ChangingUser:
                        case TypeMessage.DeleteUser:
                        case TypeMessage.RestoreUser:
                            i = parameter[0];
                            result[1] = context.Users.FirstOrDefault(x => x.ID == i)?.FullName;
                            break;
                        case TypeMessage.CreateRole:
                        case TypeMessage.ChangingRole:
                        case TypeMessage.DeleteRole:
                        case TypeMessage.RestoreRole:
                            i = parameter[0];
                            result[1] = context.Roles.FirstOrDefault(x => x.ID == i)?.RoleName;
                            break;
                        case TypeMessage.CreateUserSetting:
                        case TypeMessage.ChangingUserSetting:
                        case TypeMessage.DeleteUserSetting:
                        case TypeMessage.RestoreUserSetting:
                            i = parameter[0];
                            result[1] = context.Settings.FirstOrDefault(x => x.ID == i)?.Caption;
                            break;
                        case TypeMessage.CreateProject:
                        case TypeMessage.ChangingProject:
                        case TypeMessage.DeleteProject:
                        case TypeMessage.RestoreProject:
                            i = parameter[0];
                            result[1] = context.Projects.FirstOrDefault(x => x.ID == i)?.Caption;
                            break;
                        case TypeMessage.AddComment:
                            i = parameter[0];
                            var com = context.TaskComments.FirstOrDefault(x => x.ID == i);
                            result[1] = com?.Commentary;
                            break;
                    }
            }
            return result;
        }

        public Color GetColorMessage(TypeMessage type, bool isForeColor = false) {
            Color result = default(Color);
            Color backBase = App.GetColorResource("FBackColor");
            Color foreBase = App.GetColorResource("FForeColor");
            if (!isForeColor)
                switch (type) {
                    case TypeMessage.CreateProject:
                    case TypeMessage.CreateRole:
                    case TypeMessage.CreateTask:
                    case TypeMessage.CreateUser:
                    case TypeMessage.CreateUserSetting:
                        result = Colors.DarkSlateBlue;
                        break;
                    case TypeMessage.ChangedTask:
                    case TypeMessage.ChangingProject:
                    case TypeMessage.ChangingRole:
                    case TypeMessage.ChangingUser:
                    case TypeMessage.ChangingUserSetting:
                        result = Colors.DarkOrange;
                        break;
                    case TypeMessage.DeleteProject:
                    case TypeMessage.DeleteRole:
                    case TypeMessage.DeleteTask:
                    case TypeMessage.DeleteUser:
                    case TypeMessage.DeleteUserSetting:
                        result = Colors.Maroon;
                        break;
                    case TypeMessage.RestoreProject:
                    case TypeMessage.RestoreRole:
                    case TypeMessage.RestoreTask:
                    case TypeMessage.RestoreUser:
                    case TypeMessage.RestoreUserSetting:
                        result = Colors.YellowGreen;
                        break;
                    case TypeMessage.UserEnter:
                        result = Colors.DarkViolet;
                        break;
                    case TypeMessage.UserExit:
                        result = Colors.MediumVioletRed;
                        break;
                    case TypeMessage.StartExecution:
                        result = Colors.LightGoldenrodYellow;
                        break;
                    case TypeMessage.ComplitedExecution:
                        result = Colors.DarkGreen;
                        break;
                    case TypeMessage.CancelingExecution:
                        result = Colors.DarkRed;
                        break;
                    case TypeMessage.AddComment:
                        result = backBase;
                        break;
                }
            else
                switch (type)
                {
                    case TypeMessage.CreateProject:
                    case TypeMessage.CreateRole:
                    case TypeMessage.CreateTask:
                    case TypeMessage.CreateUser:
                    case TypeMessage.CreateUserSetting:
                        result = Colors.White;
                        break;
                    case TypeMessage.ChangedTask:
                    case TypeMessage.ChangingProject:
                    case TypeMessage.ChangingRole:
                    case TypeMessage.ChangingUser:
                    case TypeMessage.ChangingUserSetting:
                        result = Colors.White;
                        break;
                    case TypeMessage.DeleteProject:
                    case TypeMessage.DeleteRole:
                    case TypeMessage.DeleteTask:
                    case TypeMessage.DeleteUser:
                    case TypeMessage.DeleteUserSetting:
                        result = Colors.White;
                        break;
                    case TypeMessage.RestoreProject:
                    case TypeMessage.RestoreRole:
                    case TypeMessage.RestoreTask:
                    case TypeMessage.RestoreUser:
                    case TypeMessage.RestoreUserSetting:
                        result = Colors.White;
                        break;
                    case TypeMessage.UserEnter:
                        result = Colors.White;
                        break;
                    case TypeMessage.UserExit:
                        result = Colors.White;
                        break;
                    case TypeMessage.StartExecution:
                        result = Colors.White;
                        break;
                    case TypeMessage.ComplitedExecution:
                        result = Colors.White;
                        break;
                    case TypeMessage.CancelingExecution:
                        result = Colors.White;
                        break;
                    case TypeMessage.AddComment:
                        result = foreBase;
                        break;
                }
            return result;
        }
        #endregion
    }
}
