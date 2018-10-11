using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace NTW.Controls.Behaviours
{
    public class WindowClosingBehaviour
    {

        public static readonly DependencyProperty UseAlterClosingProperty = DependencyProperty.RegisterAttached(
            "UseAlterClosing", typeof(bool), typeof(WindowClosingBehaviour), new PropertyMetadata(false, new PropertyChangedCallback((d, a) => {
                if (d is Window)
                {
                    Window w = (Window)d;
                    if ((bool)a.NewValue)
                    {
                        w.PreviewKeyDown += W_PreviewKeyDown;
                    }
                    else
                    {
                        w.PreviewKeyDown -= W_PreviewKeyDown;
                    }
                }
            })));

        private static void W_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if ((Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)) && Keyboard.IsKeyDown(Key.F4))
                e.Handled = true;
        }

        public static void OnCloseWindow(DependencyObject element) {
            if (element is Window)
            {
                ((Window)element).Close();
            }
        }


        public static void SetUseAlterClosing(DependencyObject element, bool value) {
            element.SetValue(UseAlterClosingProperty, value);
        }

        public static bool GetUseAlterClosing(DependencyObject element) {
            return (bool)element.GetValue(UseAlterClosingProperty);
        }
    }
}
