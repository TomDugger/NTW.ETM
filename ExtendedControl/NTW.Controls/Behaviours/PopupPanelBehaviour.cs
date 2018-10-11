using NTW.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace NTW.Controls.Behaviours
{
    public class PopupPanelBehaviour
    {
               
        public static readonly DependencyProperty TargetPositionProperty =
            DependencyProperty.RegisterAttached("TargetPosition", typeof(WindowPosition), typeof(PopupPanelBehaviour), new PropertyMetadata(WindowPosition.Non, new PropertyChangedCallback((d, a) => {
                if (d is FrameworkElement)
                    SetLengthOnParentPosition((FrameworkElement)d, (WindowPosition)a.NewValue, GetLength(d));
            })));

        public static WindowPosition GetTargetPosition(DependencyObject obj)
        {
            return (WindowPosition)obj.GetValue(TargetPositionProperty);
        }

        public static void SetTargetPosition(DependencyObject obj, WindowPosition value)
        {
            obj.SetValue(TargetPositionProperty, value);
        }

        
        public static readonly DependencyProperty LengthProperty =
            DependencyProperty.RegisterAttached("Length", typeof(double), typeof(PopupPanelBehaviour), new PropertyMetadata(double.NaN, new PropertyChangedCallback((d, a) => {
                if (d is FrameworkElement) 
                    SetLengthOnParentPosition((FrameworkElement)d, GetTargetPosition(d), (double)a.NewValue);
            })));

        public static double GetLength(DependencyObject obj)
        {
            return (double)obj.GetValue(LengthProperty);
        }

        public static void SetLength(DependencyObject obj, double value)
        {
            obj.SetValue(LengthProperty, value);
        }

        public static void SetLengthOnParentPosition(FrameworkElement element, WindowPosition parentPosition, double length) {
            switch (parentPosition) {
                case WindowPosition.Left:
                case WindowPosition.LeftBottom:
                case WindowPosition.LeftTop:
                case WindowPosition.Right:
                case WindowPosition.RightBottom:
                case WindowPosition.RightTop:
                    element.MaxWidth = double.IsNaN(length) ? double.PositiveInfinity : length;
                    element.MaxHeight = double.PositiveInfinity;
                    break;
                case WindowPosition.Top:
                case WindowPosition.Bottom:
                    element.MaxWidth = double.PositiveInfinity;
                    element.MaxHeight = double.IsNaN(length) ? double.PositiveInfinity : length;
                    break;
            }
        }
    }
}
