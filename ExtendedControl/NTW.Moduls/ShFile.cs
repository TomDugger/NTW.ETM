using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Media.Imaging;

namespace NTW.Moduls {
    public class ShFile {
        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        [DllImport("User32.dll")]
        private static extern int DestroyIcon(IntPtr hIcon);

        class Win32 {
            public const uint SHGFI_ICON = 0x100;
            public const uint SHGFI_LARGEICON = 0x0;
            public const uint SHGFI_SMALLICON = 0x1;

            [DllImport("shell32.dll")]
            public static extern IntPtr SHGetFileInfo(string pszPath,
            uint dwFileAttributes,
            ref SHFILEINFO psfi,
            uint cbSizeFileInfo,
            uint uFlags);
        }

        private Icon GetIconDrw(string path, bool bolshaya) {
            IntPtr hImgSmall;
            IntPtr hImgLarge;
            SHFILEINFO shinfo = new SHFILEINFO();

            if (!bolshaya) {
                hImgSmall = Win32.SHGetFileInfo(path, 0, ref shinfo,
                (uint)Marshal.SizeOf(shinfo),
                Win32.SHGFI_ICON |
                Win32.SHGFI_SMALLICON);
            }

            else {
                hImgLarge = Win32.SHGetFileInfo(path, 0,
                ref shinfo, (uint)Marshal.SizeOf(shinfo),
                Win32.SHGFI_ICON | Win32.SHGFI_LARGEICON);
            }

            System.Drawing.Icon myIcon = null;
            try
            {
                myIcon = (System.Drawing.Icon)System.Drawing.Icon.FromHandle(shinfo.hIcon).Clone();
            }
            catch { }

            DestroyIcon(shinfo.hIcon);
            return myIcon;
        }

        public BitmapImage GetIcon(string path, bool isBig) {
            BitmapImage bitmapimage = new BitmapImage();
            using (MemoryStream memory = new MemoryStream()) {
                Bitmap bmp = default(Bitmap);
                var ico = GetIconDrw(path, isBig);
                if (ico != null)
                {
                    bmp = new Bitmap(GetIconDrw(path, isBig).ToBitmap());
                    bmp.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                    memory.Position = 0;
                    bitmapimage.BeginInit();
                    bitmapimage.StreamSource = memory;
                    bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapimage.EndInit();
                }
            }
            return bitmapimage;
        }
    }
}
