using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ExtendedControl.Behaviour
{
    public class PasswordBoxBehaviour
    {
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.RegisterAttached(
            "Password", typeof(string), typeof(PasswordBoxBehaviour), new FrameworkPropertyMetadata(string.Empty, (d, e) => {
                if (d is PasswordBox)
                {
                    PasswordBox pb = d as PasswordBox;
                    pb.PasswordChanged -= new RoutedEventHandler(pb_PasswordChanged);
                    pb.PasswordChanged += new RoutedEventHandler(pb_PasswordChanged);
                    pb.Password = (string)e.NewValue;
                    pb.Focus();
                    keybd_event(0x23, 0, 0, 0);
                }
            }));

        public static void SetPassword(DependencyObject element, string value)
        {
            element.SetValue(PasswordBoxBehaviour.PasswordProperty, value);
        }

        public static string GetPassword(DependencyObject element)
        {
            return element.GetValue(PasswordBoxBehaviour.PasswordProperty).ToString();
        }

        static void pb_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBoxBehaviour.SetPassword(sender as DependencyObject, (sender as PasswordBox).Password);
        }

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

    }
}
