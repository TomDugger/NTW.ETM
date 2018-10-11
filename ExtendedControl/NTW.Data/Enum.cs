using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NTW.Data
{
    public enum WindowPosition
    {
        None = 0,
        Left = 1, Right = 2,
        Top = 3, Bottom = 4,

        LeftTop = 5, RightTop = 6,
        LeftBottom = 7, RightBottom = 8,

        Non = 9
    }

    public enum Theme
    {
        Custom = 0,
        Dark = 1,
        Light = 2
    }

    //task enums
    public enum TypeTask : int
    {
        Task = 1,
        Proposal = 2,
        Enhancement = 3,
        Bug = 4
    }

    public enum PriorityTask : int
    {
        Minor = 1,
        Major = 2,
        Critical = 3
    }

    public enum StateFile : int {
        Exists,
        Load,
        NotExists
    }

    public enum TypeControl {
        Normal = 0,
        Edit = 1,
        Create = 2
    }
}
