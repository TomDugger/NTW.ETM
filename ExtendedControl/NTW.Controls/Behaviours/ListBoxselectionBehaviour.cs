using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace NTW.Controls.Behaviours {
    public class ListBoxselectionBehaviour {        
        public static readonly DependencyProperty OnlyLeftselectionProperty =
            DependencyProperty.RegisterAttached("OnlyLeftselection", typeof(bool), typeof(ListBoxselectionBehaviour), new PropertyMetadata(false, new PropertyChangedCallback((d, a) => {
                if (d is ListBox) {
                    ListBox lb = (ListBox)d;

                    if ((bool)a.NewValue) {
                        lb.PreviewMouseRightButtonDown += Lb_PreviewMouseRightButtonDown;
                    }
                    else
                        lb.PreviewMouseRightButtonDown -= Lb_PreviewMouseRightButtonDown;
                }
            })));

        private static void Lb_PreviewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            e.Handled = true;
        }

        public static bool GetOnlyLeftselection(DependencyObject obj) {
            return (bool)obj.GetValue(OnlyLeftselectionProperty);
        }

        public static void SetOnlyLeftselection(DependencyObject obj, bool value) {
            obj.SetValue(OnlyLeftselectionProperty, value);
        }
    }
}
