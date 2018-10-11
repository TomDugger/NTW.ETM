using NTW.Attrebute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace NTW.Data.Context//Presentation
{
    [ReportStat]
    public partial class Note
    {
        public Brush Color {
            get {
                byte[] bb = BitConverter.GetBytes(this.IDColor);
                return new SolidColorBrush(System.Windows.Media.Color.FromRgb(bb[2], bb[1], bb[0]));
            }
        }
    }
}
