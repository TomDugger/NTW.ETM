using NTW.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace ExtendedControl.Behaviour
{
    public class RectanglePositionBehaviour
    {
        public static readonly DependencyProperty PositionProperty = DependencyProperty.RegisterAttached(
            "Position", typeof(WindowPosition), typeof(RectanglePositionBehaviour), new PropertyMetadata(WindowPosition.None, new PropertyChangedCallback((d, a) => {
                if (d is Rectangle || d is Border) {
                    FrameworkElement r = (FrameworkElement)d;

                    switch ((WindowPosition)a.NewValue) {
                        case WindowPosition.Left:
                            r.Height = double.NaN;
                            r.Width = 15;

                            r.VerticalAlignment = VerticalAlignment.Stretch;
                            r.HorizontalAlignment = HorizontalAlignment.Left;

                            r.Margin = new Thickness(0, 25, 0, 25);
                            break;
                        case WindowPosition.Top:
                            r.Height = 15;
                            r.Width = double.NaN;

                            r.VerticalAlignment = VerticalAlignment.Top;
                            r.HorizontalAlignment = HorizontalAlignment.Stretch;

                            r.Margin = new Thickness(25, 0, 25, 0);
                            break;
                        case WindowPosition.Right:
                            r.Height = double.NaN;
                            r.Width = 15;

                            r.VerticalAlignment = VerticalAlignment.Stretch;
                            r.HorizontalAlignment = HorizontalAlignment.Right;

                            r.Margin = new Thickness(0, 25, 0, 25);
                            break;
                        case WindowPosition.Bottom:
                            r.Height = 15;
                            r.Width = double.NaN;

                            r.VerticalAlignment = VerticalAlignment.Bottom;
                            r.HorizontalAlignment = HorizontalAlignment.Stretch;

                            r.Margin = new Thickness(25, 0, 25, 0);
                            break;

                        case WindowPosition.LeftTop:
                            r.Height = 20;
                            r.Width = 20;

                            r.VerticalAlignment = VerticalAlignment.Top;
                            r.HorizontalAlignment = HorizontalAlignment.Left;
                            break;
                        case WindowPosition.LeftBottom:
                            r.Height = 20;
                            r.Width = 20;

                            r.VerticalAlignment = VerticalAlignment.Bottom;
                            r.HorizontalAlignment = HorizontalAlignment.Left;
                            break;
                        case WindowPosition.RightTop:
                            r.Height = 20;
                            r.Width = 20;

                            r.VerticalAlignment = VerticalAlignment.Top;
                            r.HorizontalAlignment = HorizontalAlignment.Right;
                            break;
                        case WindowPosition.RightBottom:
                            r.Height = 20;
                            r.Width = 20;

                            r.VerticalAlignment = VerticalAlignment.Bottom;
                            r.HorizontalAlignment = HorizontalAlignment.Right;
                            break;
                    }
                }
            })));

        public static void SetPosition(DependencyObject element, WindowPosition value) {
            element.SetValue(PositionProperty, value);
        }

        public static WindowPosition GetPosition(DependencyObject element) {
            return (WindowPosition)element.GetValue(PositionProperty);
        }
    }
}
