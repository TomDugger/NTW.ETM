using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace NTW.Data.Context//.Presentation
{
    public partial class TaskComment
    {
        private string _createrName;
        public string CreaterName {
            get {
                return _createrName ?? (_createrName = GetCreaterName());
            }
        }

        protected string GetCreaterName()
        {
            string result = string.Empty;
            using (DBContext context = new DBContext(false))
            {
                User u = context.Users.FirstOrDefault(x => x.ID == this.IDUser);
                if (u != null)
                    result = u.FullName;
            }
            return result;
        }

        public DateTime OnlyDateCreateDate {
            get { return this.CreateDate.Date
; }
        }

        public string CommentaryBody {
            get {
                string result = "";
                if (this._TypeCommentary == 0)
                    result = this.Commentary;
                else {
                    if (this.Commentary.StartsWith("start"))
                        result = string.Format(Application.Current.Resources["CommentStartExecutionTask"].ToString(), this.CreaterName);
                    else if (this.Commentary.StartsWith("fine"))
                        result = string.Format(Application.Current.Resources["CommentComplitedExecutionTask"].ToString(), this.CreaterName);
                    else if (this.Commentary.StartsWith("cancelling"))
                        result = string.Format(Application.Current.Resources["CommentCancelingExecutionTask"].ToString(), this.CreaterName, this.Commentary.Substring(this.Commentary.IndexOf('|') + 1));
                    else if(this.Commentary.StartsWith("edit"))
                        result = string.Format(Application.Current.Resources["TaskEditTask"].ToString(), this.CreaterName);
                    else if (this.Commentary.StartsWith("delete"))
                        result = string.Format(Application.Current.Resources["TaskDeleteTask"].ToString(), this.CreaterName);
                }
                return result;
            }
        }
    }
}
