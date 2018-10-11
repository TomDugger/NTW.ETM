using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace NTW.Core
{
    internal sealed class Taskbar
    {
        private const string ClassName = "Shell_TrayWnd";

        internal System.Drawing.Rectangle Bounds
        {
            get;
            private set;
        }
        internal TaskbarPosition Position
        {
            get;
            private set;
        }
        internal System.Drawing.Point Location
        {
            get
            {
                return this.Bounds.Location;
            }
        }
        internal System.Drawing.Size Size
        {
            get
            {
                return this.Bounds.Size;
            }
        }
        //Always returns false under Windows 7
        internal bool AlwaysOnTop
        {
            get;
            private set;
        }
        internal bool AutoHide
        {
            get;
            private set;
        }

        internal Taskbar()
        {
            IntPtr taskbarHandle = User32.FindWindow(Taskbar.ClassName, null);

            APPBARDATA data = new APPBARDATA();
            data.cbSize = (uint)Marshal.SizeOf(typeof(APPBARDATA));
            data.hWnd = taskbarHandle;
            IntPtr result = Shell32.SHAppBarMessage(ABM.GetTaskbarPos, ref data);
            if (result == IntPtr.Zero)
                throw new InvalidOperationException();

            this.Position = (TaskbarPosition)data.uEdge;
            this.Bounds = System.Drawing.Rectangle.FromLTRB(data.rc.left, data.rc.top, data.rc.right, data.rc.bottom);

            data.cbSize = (uint)Marshal.SizeOf(typeof(APPBARDATA));
            result = Shell32.SHAppBarMessage(ABM.GetState, ref data);
            int state = result.ToInt32();
            this.AlwaysOnTop = (state & ABS.AlwaysOnTop) == ABS.AlwaysOnTop;
            this.AutoHide = (state & ABS.Autohide) == ABS.Autohide;
        }
    }

    internal enum ABM : uint
    {
        New = 0x00000000,
        Remove = 0x00000001,
        QueryPos = 0x00000002,
        SetPos = 0x00000003,
        GetState = 0x00000004,
        GetTaskbarPos = 0x00000005,
        Activate = 0x00000006,
        GetAutoHideBar = 0x00000007,
        SetAutoHideBar = 0x00000008,
        WindowPosChanged = 0x00000009,
        SetState = 0x0000000A,
    }

    internal enum ABE : uint
    {
        Left = 0,
        Top = 1,
        Right = 2,
        Bottom = 3
    }

    internal static class ABS
    {
        public const int Autohide = 0x0000001;
        public const int AlwaysOnTop = 0x0000002;
    }

    internal static class Shell32
    {
        [DllImport("shell32.dll", SetLastError = true)]
        public static extern IntPtr SHAppBarMessage(ABM dwMessage, [In] ref APPBARDATA pData);
    }

    internal static class User32
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct APPBARDATA
    {
        public uint cbSize;
        public IntPtr hWnd;
        public uint uCallbackMessage;
        public ABE uEdge;
        public RECT rc;
        public int lParam;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    public enum TaskbarPosition
    {
        Unknown = -1,
        Left,
        Top,
        Right,
        Bottom,
    }
}
