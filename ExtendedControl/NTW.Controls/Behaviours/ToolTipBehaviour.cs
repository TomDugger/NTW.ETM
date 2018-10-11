using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace NTW.Controls.Behaviours
{
    public class ToolTipBehaviour
    {
        public static readonly DependencyProperty ResourceProperty = DependencyProperty.RegisterAttached(
            "Resource", typeof(string), typeof(ToolTipBehaviour), new PropertyMetadata(null, new PropertyChangedCallback((d, a) => {
                if (d is FrameworkElement)
                {
                    FrameworkElement fe = (FrameworkElement)d;

                    if (Application.Current.TryFindResource(a.NewValue) != null)
                    {
                        ToolTip tt = new ToolTip();
                        TextBlock tb = new TextBlock();
                        tb.SetResourceReference(TextBlock.TextProperty, a.NewValue);
                        tt.Content = tb;
                        fe.ToolTip = tt;
                    }
                    else
                    {
                        fe.ToolTip = null;
                    }
                }
            })));


        public static void SetResource(DependencyObject element, string value) {
            element.SetValue(ResourceProperty, value);
        }

        public static string GetResource(DependencyObject element) {
            return (string)element.GetValue(ResourceProperty);
        }
    }
}
