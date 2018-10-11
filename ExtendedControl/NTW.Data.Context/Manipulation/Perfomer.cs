using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace NTW.Data.Context//.Manipulation
{
    public partial class Perfomer
    {
        public bool IsCurrentUser {
            get { return this.IDUser == ((User)Application.Current.Resources["CurrentUser"]).ID; }
        }
    }
}
