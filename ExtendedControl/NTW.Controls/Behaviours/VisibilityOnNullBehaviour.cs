using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace NTW.Controls.Behaviours
{
    public class VisibilityOnNullBehaviour
    {
        public static readonly DependencyProperty OriginalSizeProperty = DependencyProperty.RegisterAttached(
            "OriginalSize", typeof(Point), typeof(VisibilityOnNullBehaviour), new PropertyMetadata(new Point(50, 50)));

        public static readonly DependencyProperty VisibilityOnNullProperty = DependencyProperty.RegisterAttached(
            "VisibilityOnNull", typeof(object), typeof(VisibilityOnNullBehaviour), new PropertyMetadata(null, new PropertyChangedCallback((d, a) => {
                if (d is FrameworkElement)
                {
                    FrameworkElement fe = (FrameworkElement)d;

                    if ((a.NewValue is int && (int)a.NewValue == 0) || (a.NewValue == null) || (a.NewValue is bool && !(bool)a.NewValue))
                        fe.Width = fe.Height = 0;
                    else  {

                        Point size = GetOriginalSize(fe);
                        fe.Width = size.X;
                        fe.Height = size.Y;
                    }
                }
            })));

        public static readonly DependencyProperty NegativeVisibilityOnNullProperty = DependencyProperty.RegisterAttached(
            "NegativeVisibilityOnNull", typeof(object), typeof(VisibilityOnNullBehaviour), new PropertyMetadata(null, new PropertyChangedCallback((d, a) => {
                if (d is FrameworkElement)
                {
                    FrameworkElement fe = (FrameworkElement)d;

                    if ((a.NewValue is int && (int)a.NewValue != 0) || 
                    (a.NewValue == null) || 
                    (a.NewValue is bool && (bool)a.NewValue))
                        fe.Width = fe.Height = 0;
                    else
                    {

                        Point size = GetOriginalSize(fe);
                        fe.Width = size.X;
                        fe.Height = size.Y;
                    }
                }
            })));

        #region Propertys
        public static void SetVisibilityOnNull(DependencyObject element, object value)
        {
            element.SetValue(VisibilityOnNullProperty, value);
        }

        public static object GetVisibilityOnNull(DependencyObject element)
        {
            return element.GetValue(VisibilityOnNullProperty);
        }


        public static void SetNegativeVisibilityOnNull(DependencyObject element, object value) {
            element.SetValue(NegativeVisibilityOnNullProperty, value);
        }

        public static object GetNegativeVisibilityOnNull(DependencyObject element) {
            return element.GetValue(NegativeVisibilityOnNullProperty);
        }

        public static void SetOriginalSize(DependencyObject element, Point value) {
            element.SetValue(OriginalSizeProperty, value);
        }

        public static Point GetOriginalSize(DependencyObject element)
        {
            return (Point)element.GetValue(OriginalSizeProperty);
        }
        #endregion
    }
}
