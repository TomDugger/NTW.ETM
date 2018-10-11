using NTW.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NTW
{
    public partial class Common
    {
        private static Taskbar taskbar = new Taskbar();

        public static double GetIndentTaskBar(bool isBottom)
        {
            if (isBottom)
            {
                switch (taskbar.Position)
                {
                    case TaskbarPosition.Bottom:
                        return taskbar.Bounds.Height;
                    default:
                        return 0;
                }
            }
            else
            {
                switch (taskbar.Position)
                {
                    case TaskbarPosition.Right:
                        return taskbar.Bounds.Width;
                    default:
                        return 0;
                }
            }
        }
    }
}
