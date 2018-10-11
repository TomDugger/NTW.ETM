using NTW.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ExtendedControl.ViewModels
{
    public class ReportsViewModel: ControlViewModel
    {
        #region Commands

        #endregion

        #region Members
        public ObservableCollection<ObjectNodeKey> ViewModels {
            get {
                return Reflection.GetTreeByNamespase("ExtendedControl.ViewModels", typeof(ReportsViewModel));
            }
        }
        #endregion

        #region Helps

        #endregion
    }
}
