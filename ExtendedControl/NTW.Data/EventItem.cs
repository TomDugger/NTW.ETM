using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Threading;

namespace NTW.Data
{
    public class EventItem
    {
        public EventItem(string message, TypeMessage type, DispatcherTimer timer, Color backColor, Color foreColor) {
            this.Message = message;
            this.Timer = timer;
            this.Type = type;

            BackBrush = new SolidColorBrush(backColor);
            ForeBrush = new SolidColorBrush(foreColor);
        }

        public string Message { get; set; }

        public int Index { get; set; }

        public int[] Parametry { get; set; }

        public TypeMessage Type { get; set; }

        public DispatcherTimer Timer { get; set; }

        public SolidColorBrush BackBrush { get; private set; }

        public SolidColorBrush ForeBrush { get; private set; }
    }

    public enum TypeMessage {
        CreateTask = 1,
        ChangedTask = 2,
        DeleteTask = 3,
        RestoreTask = 4,

        StartExecution = 5,
        ComplitedExecution = 6,
        CancelingExecution = 7,
        AddComment = 8,

        UserEnter = 9,
        UserExit = 10,
        CreateUser = 11,
        ChangingUser = 12,
        DeleteUser = 13,
        RestoreUser = 14,

        CreateRole = 15,
        ChangingRole = 16,
        DeleteRole = 17,
        RestoreRole = 18,

        CreateUserSetting = 19,
        ChangingUserSetting = 20,
        DeleteUserSetting = 21,
        RestoreUserSetting = 22,

        CreateProject = 23,
        ChangingProject = 24,
        DeleteProject = 25,
        RestoreProject = 26,

        NonMessage = 0
    }
}
