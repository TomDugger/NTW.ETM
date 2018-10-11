using NTW.Communication.Beginers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NTW.Communication.Services
{
    public class CommandService : ICommandService
    {
        public static CommandHandler CommandEvent;

        public void SetCommand(int commandType, int index, int[] parametry)
        {
            CommandEvent?.Invoke(commandType, index, parametry);
        }
    }
}
