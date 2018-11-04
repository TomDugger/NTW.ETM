using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NTW.Data.Context//.Presentation
{
    public partial class Stage
    {
        public TimeSpan EndTime {
            get { return this.DeadLine.TimeOfDay; }
            set {
                this.DeadLine = new DateTime(this.DeadLine.Year, this.DeadLine.Month, this.DeadLine.Day, value.Hours, value.Minutes, value.Seconds);
                this.OnPropertyChanged(nameof(EndTime));
                this.OnPropertyChanged(nameof(DeadLine));
            }
        }
    }
}
