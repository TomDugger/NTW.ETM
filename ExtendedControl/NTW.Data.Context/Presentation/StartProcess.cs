using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace NTW.Data.Context
{
    public partial class StartProcess
    {
        public object Icon {
            get {
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage = new NTW.Moduls.ShFile().GetIcon(this.PathToApp, true);
                return bitmapimage;
            }
        }
    }
}
