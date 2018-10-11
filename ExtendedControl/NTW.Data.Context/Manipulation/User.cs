using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NTW.Data.Context //.Manipulation 
    {
    public partial class User {

        private string _hasPassword = " ";
        public string HasPassword {
            get { return _hasPassword; }
            set { _hasPassword = value;
                this.Password = Security.HashPassword(value);
            }
        }
    }
}
