using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace NTW.Controls.Behaviours
{
    public class AlwaysActiveteBehaviour
    {
        public static readonly DependencyProperty AlwaysActiveteProperty = DependencyProperty.Register(
            "AlwaysActivete", typeof(bool), typeof(AlwaysActiveteBehaviour), new PropertyMetadata(false, new PropertyChangedCallback((d, a) => {
                if (d is Window)
                {
                    Window w = (Window)d;

                    
                }
            })));


        public static void SetAlwaysActivete(DependencyObject element, bool value) {
            element.SetValue(AlwaysActiveteProperty, value);
        }

        public static bool GetAlwaysActivete(DependencyObject element) {
            return (bool)element.GetValue(AlwaysActiveteProperty);
        }
    }
}
