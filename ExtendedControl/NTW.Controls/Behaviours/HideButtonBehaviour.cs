using NTW.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTW.Controls.Behaviours
{
    public class HideButtonBehaviour
    {
        public static readonly DependencyProperty UseHideButtonProperty = DependencyProperty.RegisterAttached(
            "UseHideButton", typeof(bool), typeof(HideButtonBehaviour), new PropertyMetadata(false));

        public static readonly DependencyProperty HideButtonPositionProperty = DependencyProperty.RegisterAttached(
            "HideButtonPosition", typeof(WindowPosition), typeof(HideButtonBehaviour), new PropertyMetadata(WindowPosition.Non, new PropertyChangedCallback((d, a) => {
                if (d is FrameworkElement) {

                    FrameworkElement ui = (FrameworkElement)d;
                    ui.HorizontalAlignment = HorizontalAlignment.Stretch;
                    ui.VerticalAlignment = VerticalAlignment.Stretch;

                    switch ((WindowPosition)a.NewValue) {
                        case WindowPosition.Left:
                            Grid.SetRow(ui, 1);
                            Grid.SetColumn(ui, 2);
                            break;
                        case WindowPosition.Top:
                            Grid.SetRow(ui, 2);
                            Grid.SetColumn(ui, 1);
                            break;
                        case WindowPosition.RightTop:
                        case WindowPosition.Right:
                            Grid.SetRow(ui, 1);
                            Grid.SetColumn(ui, 0);
                            break;
                        case WindowPosition.Bottom:
                            Grid.SetRow(ui, 0);
                            Grid.SetColumn(ui, 1);
                            break;
                        case WindowPosition.None:
                            Grid.SetRow(ui, 1);
                            Grid.SetColumn(ui, 1);
                            ui.HorizontalAlignment = HorizontalAlignment.Right;
                            ui.VerticalAlignment = VerticalAlignment.Top;
                            break;
                    }
                }
            })));

        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
            "Command", typeof(ICommand), typeof(HideButtonBehaviour), new PropertyMetadata(null));


        public static void SetUseHideButton(DependencyObject element, bool value) {
            element.SetValue(UseHideButtonProperty, value);
        }

        public static bool GetUseHideButton(DependencyObject element) {
            return (bool)element.GetValue(UseHideButtonProperty);
        }


        public static void SetHideButtonPosition(DependencyObject element, WindowPosition value) {
            element.SetValue(HideButtonPositionProperty, value);
        }

        public static WindowPosition GetHideButtonPosition(DependencyObject element) {
            return (WindowPosition)element.GetValue(HideButtonPositionProperty);
        }


        public static void SetCommand(DependencyObject element, ICommand value) {
            element.SetValue(CommandProperty, value);
        }

        public static ICommand GetCommand(DependencyObject element) {
            return (ICommand)element.GetValue(CommandProperty);
        }
    }
}
