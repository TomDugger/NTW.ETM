using NTW.Attrebute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace NTW.Data.Context
{
    [ReportStat]
    public partial class NoteGroup
    {
        #region Propertys
        [ReportStat]
        public int NotesCount {
            get {
                int result = 0;
                using (DBContext context = new DBContext(false)) {
                    result = context.Notes.Count(x => x.Group == this.ID);
                }
                return result;
            }
        }

        public Brush Color
        {
            get
            {
                byte[] bb = BitConverter.GetBytes(this.IDColor);
                return new SolidColorBrush(System.Windows.Media.Color.FromRgb(bb[2], bb[1], bb[0]));
            }
        }
        #endregion
    }
}
