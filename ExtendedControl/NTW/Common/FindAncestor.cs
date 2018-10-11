using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace NTW
{
    public partial class Common
    {
        public static T FindAncestor<T>(DependencyObject from) where T : class
        {
            if (from == null)
            {
                return null;
            }

            T candidate = from as T;
            if (candidate != null)
            {
                return candidate;
            }

            DependencyObject temp = VisualTreeHelper.GetParent(from);

            return FindAncestor<T>(temp ?? (temp = LogicalTreeHelper.GetParent(from)));
        }
    }
}
