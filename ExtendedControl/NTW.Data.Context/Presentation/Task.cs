using NTW.Attrebute;
using NTW.Data.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace NTW.Data.Context
{
    [ReportStat]
    public partial class Task
    {
        [ReportStat]
        public string DateOnLine
        {
            get
            {
                string result = string.Empty;
                if (this.OpenDate.Date == this.EndDate.Date)
                    result = this.OpenDate.ToString("dd.MM.yyyy г. ") + " " +
                        this.OpenDate.ToString("HH:mm") + "-" + this.EndDate.ToString("HH:mm");
                else if (this.OpenDate.Month == this.EndDate.Month && this.OpenDate.Year == this.EndDate.Year)
                    result = this.OpenDate.ToString("dd") + "-" + this.EndDate.ToString("dd") + "." + this.OpenDate.ToString("MM.yyyy г.");
                else if (this.OpenDate.Year == this.EndDate.Year)
                    result = this.OpenDate.ToString("dd.MM") + "-" + this.EndDate.ToString("dd.MM") + "." + this.OpenDate.ToString("yyyy г.");
                return result;
            }
        }

        [ReportStat]
        public int CountDays {
            get {
                int result = 0;
                result = this.EndDate.Date.Subtract(this.OpenDate.Date).Days;
                if (result < 0)
                    result = 0;
                return result;
            }
        }
        
        [ReportStat]
        public string MinCaption {
            get
            {
                if (Caption.Length > 15)
                    return Caption.Substring(0, 12).Replace(System.Environment.NewLine, "") + "...";
                else
                    return Caption;
            }
        }

        private string _projectString;
        [ReportStat]
        public string ProjectString {
            get {
                return _projectString ?? (_projectString = GetProjectName());
            }
        }

        [ReportStat]
        public string TypeString {
            get {
                var items = ((DoubleStruct[])Application.Current.Resources["ArrayTypeTask"]);
                return ((DoubleStruct[])Application.Current.Resources["ArrayTypeTask"]).FirstOrDefault(x => x.Value == this.TypeTask).Name;
            }
        }

        [ReportStat]
        public string PrioretyString {
            get {
                return ((DoubleStruct[])Application.Current.Resources["ArrayPriorityTask"]).FirstOrDefault(x => x.Value == this.PriorityTask).Name;
            }
        }

        public bool IsStartExecutionByTask {
            get {
                Perfomer pf = this.Perfomers.FirstOrDefault(x => x.IsCurrentUser);
                if (pf == null)
                    return false;

                return pf.StartDate != this.OpenDate && pf.CloseDate == this.OpenDate;
            }
        }

        public bool IsExecutionTask {
            get {
                Perfomer pf = this.Perfomers.FirstOrDefault(x => x.IsCurrentUser);
                if (pf == null)
                    return false;

                return pf.State;
            }
        }

        [ReportStat]
        public DateTime EndDateOnly {
            get { return this.EndDate.Date; }
            set {
                this.EndDate = new DateTime(value.Year, value.Month, value.Day, this.EndDate.Hour, this.EndDate.Minute, this.EndDate.Second);
                this.OnPropertyChanged(nameof(EndDateOnly)); this.OnPropertyChanged(nameof(EndTime)); this.OnPropertyChanged(nameof(EndDate));
            }
        }

        [ReportStat]
        public TimeSpan EndTime {
            get { return this.EndDate.TimeOfDay; }
            set {
                this.EndDate = new DateTime(this.EndDate.Year, this.EndDate.Month, this.EndDate.Day, value.Hours, value.Minutes, value.Seconds);
                this.OnPropertyChanged(nameof(EndTime));
                this.OnPropertyChanged(nameof(EndDateOnly)); this.OnPropertyChanged(nameof(EndDate));
            }
        }

        [ReportStat]
        public string AddedTime {
            get {
                string result = string.Empty;
                DateTime currentDate = DateTime.Now;
                if (currentDate - this.OpenDate < new TimeSpan(0, 1, 0))
                    result = (currentDate - this.OpenDate).Seconds + " second(s) ago";
                else if (currentDate - this.OpenDate < new TimeSpan(1, 0, 0))
                    result = (currentDate - this.OpenDate).Minutes + " menute(s) ago";
                else if (currentDate - this.OpenDate < new TimeSpan(1, 0, 0, 0))
                    result = (currentDate - this.OpenDate).Hours + " hours ago";
                else if (currentDate - this.OpenDate < new TimeSpan(27, 0, 0, 0))
                    result = (currentDate - this.OpenDate).Days + " day(s) ago";
                else
                    result = this.OpenDate.ToString("dd.MM.yyyy HH:mm:ss");
                return result;
            }
        }

        #region Helps
        protected string GetProjectName() {
            string result = null;
            using (DBContext context = new DBContext(false))
                result = context.Projects.FirstOrDefault(x => x.ID == this.IdProject).Caption;
            return result;
        }

        public void OnPropertyChangedState() {
            this.OnPropertyChanged(nameof(IsStartExecutionByTask));
            this.OnPropertyChanged(nameof(IsExecutionTask));
        }
        #endregion
    }
}
