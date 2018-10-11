using NTW.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace NTW.Controls.Behaviours
{
    public class FlowDirectionOnPositionBehaviour
    {
        public static readonly DependencyProperty PositionProperty = DependencyProperty.RegisterAttached(
            "Position", typeof(WindowPosition), typeof(FlowDirectionOnPositionBehaviour), new PropertyMetadata(WindowPosition.Non, new PropertyChangedCallback((d, a) => {
                if (d is FrameworkElement) {
                    FrameworkElement fe = (FrameworkElement)d;
                    switch ((WindowPosition)a.NewValue) {
                        case WindowPosition.Bottom:
                        case WindowPosition.Top:
                        case WindowPosition.Left:
                            fe.FlowDirection = FlowDirection.LeftToRight;
                            break;
                        case WindowPosition.LeftTop:
                        case WindowPosition.RightTop:
                        case WindowPosition.RightBottom:
                        case WindowPosition.LeftBottom:
                        case WindowPosition.Right:
                            fe.FlowDirection = FlowDirection.RightToLeft;
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


        public static readonly DependencyProperty NegativePositionProperty =
            DependencyProperty.RegisterAttached("NegativePosition", typeof(WindowPosition), typeof(FlowDirectionOnPositionBehaviour), new PropertyMetadata(WindowPosition.Non, new PropertyChangedCallback((d, a) => {
                if (d is FrameworkElement) {
                    FrameworkElement fe = (FrameworkElement)d;
                    switch ((WindowPosition)a.NewValue) {
                        case WindowPosition.Bottom:
                        case WindowPosition.Top:
                        case WindowPosition.Left:
                            fe.FlowDirection = FlowDirection.RightToLeft;
                            break;
                        case WindowPosition.LeftTop:
                        case WindowPosition.RightTop:
                        case WindowPosition.RightBottom:
                        case WindowPosition.LeftBottom:
                        case WindowPosition.Right:
                            fe.FlowDirection = FlowDirection.LeftToRight;
                            break;
                    }
                }
            })));

        public static WindowPosition GetNegativePosition(DependencyObject obj) {
            return (WindowPosition)obj.GetValue(NegativePositionProperty);
        }

        public static void SetNegativePosition(DependencyObject obj, WindowPosition value) {
            obj.SetValue(NegativePositionProperty, value);
        }
    }
}
