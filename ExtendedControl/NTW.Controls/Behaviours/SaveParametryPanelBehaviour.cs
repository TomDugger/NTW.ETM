using NTW.Data;
using NTW.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace NTW.Controls.Behaviours
{
    public class SaveParametryPanelBehaviour
    {
        public static readonly DependencyProperty IsSaveByCloseProperty = DependencyProperty.RegisterAttached(
            "IsSaveByClose", typeof(bool), typeof(SaveParametryPanelBehaviour), new PropertyMetadata(false, new PropertyChangedCallback((d, a) => {
                if (d is Window)
                {
                    Window w = (Window)d;
                    if ((bool)a.NewValue) 
                        w.Closed += W_Closed;
                    else 
                        w.Closed -= W_Closed;
                }
            })));

        private static void W_Closed(object sender, EventArgs e) {
            Window w = (Window)sender;
            //собираем основную информацию о окне и сохраняем
            PanelItemParametry pip = new PanelItemParametry();
            pip.Element = w.GetType();
            pip.Bounds = new Rect(w.Left, w.Top, w.Width, w.Height);
            pip.Position = WindowPositionBehaviour.GetWindowPosition(w);

            PanelSettings.SingleBuild(sender.GetType(), pip);
        }

        public static readonly DependencyProperty IsSaVeBySizeChangedProperty = DependencyProperty.RegisterAttached(
            "IsSaVeBySizeChanged", typeof(bool), typeof(SaveParametryPanelBehaviour), new PropertyMetadata((d, a) => {
                if (d is Window)
                {
                    Window w = (Window)d;
                    if ((bool)a.NewValue)
                        w.SizeChanged += W_SizeChanged;
                    else
                        w.SizeChanged -= W_SizeChanged;
                }
            }));

        private static void W_SizeChanged(object sender, SizeChangedEventArgs e) {
            Window w = (Window)sender;
            //собираем основную информацию о окне и сохраняем
            PanelItemParametry pip = new PanelItemParametry();
            pip.Element = w.GetType();
            pip.Bounds = new Rect(w.Left, w.Top, w.Width, w.Height);
            pip.Position = WindowPositionBehaviour.GetWindowPosition(w);

            PanelSettings.SingleBuild(sender.GetType(), pip);
        }


        public static void SetIsSaveByClose(DependencyObject element, bool value) {
            element.SetValue(IsSaveByCloseProperty, value);
        }

        public static bool GetIsSaveByClose(DependencyObject element) {
            return (bool)element.GetValue(IsSaveByCloseProperty);
        }


        public static void SetIsSaVeBySizeChanged(DependencyObject element, bool value) {
            element.SetValue(IsSaVeBySizeChangedProperty, value);
        }

        public static bool GetIsSaVeBySizeChanged(DependencyObject element) {
            return (bool)element.GetValue(IsSaVeBySizeChangedProperty);
        }
    }
}
