using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NTW.Data.Context //Presentation
{
    public partial class Perfomer
    {
        private string _fullNamePerfomer;
        public string FullNamePerfomer {
            get { return _fullNamePerfomer ?? (_fullNamePerfomer = GetFullName());
            }
        }

        public Char FirstCharName {
            get { return FullNamePerfomer.ToUpper()[0]; }
        }

        public string GetFullName() {
            string result = null;
            using (DBContext context = new DBContext(false))
                result = context.Users.FirstOrDefault(x => x.ID == this.IDUser).FullName;
            return result;
        }

        private DateTime? _openDateOnTask;
        public DateTime OpenDateOnTask {
            get { return (DateTime)(_openDateOnTask ?? (_openDateOnTask = GetOpenDateOnTask())); }
            set { _openDateOnTask = value; }
        }

        private DateTime GetOpenDateOnTask() {
            DateTime openDate = DateTime.Now;
            using (DBContext context = new DBContext(false)) {
                openDate = context.Tasks.FirstOrDefault(x => x.Perfomers.Count(y => y.ID == this.ID) == 1).OpenDate;
            }
            return openDate;
        }

        public bool IsStartExecutionByPerfomer {
            get {
                return OpenDateOnTask != this.StartDate && OpenDateOnTask == this.CloseDate;
            }
        }

        public bool IsExecutionByPerfomer {
            get {
                return this.State;
            }
        }

        public bool IsNotRemoved {
            get { var temp = OpenDateOnTask != this.StartDate | this.State; return temp; }
        }

        public void OnPropertyChangedByExecution() {
            _openDateOnTask = null;
            this.OnPropertyChanged(nameof(IsStartExecutionByPerfomer));
            this.OnPropertyChanged(nameof(IsExecutionByPerfomer));
        }
    }
}
