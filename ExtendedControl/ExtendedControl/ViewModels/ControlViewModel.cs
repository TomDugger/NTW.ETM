using NTW.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtendedControl.ViewModels
{
    public abstract class ControlViewModel: ViewModel
    {
        #region Commands

        #endregion

        #region Members
        protected TypeControl _state = TypeControl.Normal;
        public TypeControl State {
            get { return _state; }
            set { _state = value; this.SendPropertyChanged(nameof(State)); }
        }
        #endregion

        #region Helps

        #endregion
    }
}
