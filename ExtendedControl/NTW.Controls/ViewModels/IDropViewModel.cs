using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace NTW.Controls.ViewModels
{
    public interface IDropViewModel
    {
        #region Helps
        void SetValues(Type typeContent, IEnumerable values);
        void SetValues(UIElement element, IEnumerable values);
        #endregion
    }
}
