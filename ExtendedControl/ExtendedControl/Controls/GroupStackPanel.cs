using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace ExtendedControl.Controls
{
    public class GroupStackPanel: StackPanel
    {
        public GroupStackPanel():base() {
            this.Loaded += (s, e) => {
                if (s is StackPanel && (s as StackPanel).Children.Count > 0
            && (((s as StackPanel).Children[0] as GroupItem).DataContext as CollectionViewGroup).Name is DateTime)
                {
                    StackPanel panel = (StackPanel)s;
                    panel.Orientation = Orientation.Vertical;
                }
            }; 
        }
    }
}
