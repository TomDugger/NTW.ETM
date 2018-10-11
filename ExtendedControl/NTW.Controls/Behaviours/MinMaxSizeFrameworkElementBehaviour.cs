using NTW.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace NTW.Controls.Behaviours
{
    public class MinMaxSizeFrameworkElementBehaviour
    {
        public static readonly DependencyProperty UseMaxAndMinProperty = DependencyProperty.RegisterAttached(
            "UseMaxAndMin", typeof(bool), typeof(MinMaxSizeFrameworkElementBehaviour), new PropertyMetadata(false));

        public static void SetUseMaxAndMin(DependencyObject element, bool value) {
            element.SetValue(UseMaxAndMinProperty, value);
        }

        public static bool GetUseMaxAndMin(DependencyObject element) {
            return (bool)element.GetValue(UseMaxAndMinProperty);
        }


        public static readonly DependencyProperty ProcentScreenHeightProperty = DependencyProperty.RegisterAttached(
            "ProcentScreenHeight", typeof(double), typeof(MinMaxSizeFrameworkElementBehaviour), new PropertyMetadata(default(double), new PropertyChangedCallback((d, a) => {
                if (d is FrameworkElement) {
                    FrameworkElement fe = (FrameworkElement)d;
                    double value = (double)a.NewValue;
                    if (!double.IsInfinity(value) && !double.IsNaN(value) && !double.IsNegativeInfinity(value) && !double.IsPositiveInfinity(value)) {
                        //расчитываем размер от общего размера окна
                        fe.MinHeight = SystemParameters.PrimaryScreenHeight * value / 100;
                    }
                    else fe.MinHeight = 0;
                }
            })));

        public static void SetProcentScreenHeight(DependencyObject element, double value) {
            element.SetValue(ProcentScreenHeightProperty, value);
        }

        public static double GetProcentScreenHeight(DependencyObject element) {
            return (double)element.GetValue(ProcentScreenHeightProperty);
        }


        public static readonly DependencyProperty ProcentScreenWidthProperty = DependencyProperty.RegisterAttached(
            "ProcentScreenWidth", typeof(double), typeof(MinMaxSizeFrameworkElementBehaviour), new PropertyMetadata(default(double), new PropertyChangedCallback((d, a) => {
                if (d is FrameworkElement) {
                    FrameworkElement fe = (FrameworkElement)d;
                    double value = (double)a.NewValue;
                    if (!double.IsInfinity(value) && !double.IsNaN(value) && !double.IsNegativeInfinity(value) && !double.IsPositiveInfinity(value)) {
                        //расчитываем размер от общего размера окна
                        fe.MinWidth = SystemParameters.PrimaryScreenWidth * value / 100;
                    }
                    else fe.MinWidth = 0;
                }
            })));

        public static void SetProcentScreenWidth(DependencyObject element, double value) {
            element.SetValue(ProcentScreenWidthProperty, value);
        }

        public static double GetProcentScreenWidth(DependencyObject element) {
            return (double)element.GetValue(ProcentScreenWidthProperty);
        }


        public static readonly DependencyProperty ProcentScreenAllSizeProperty = DependencyProperty.RegisterAttached(
            "ProcentScreenAllSize", typeof(double), typeof(MinMaxSizeFrameworkElementBehaviour), new PropertyMetadata(default(double), new PropertyChangedCallback((d, a) => {
                if (d is FrameworkElement) {
                    FrameworkElement fe = (FrameworkElement)d;
                    double value = (double)a.NewValue;
                    if (!double.IsInfinity(value) && !double.IsNaN(value) && !double.IsNegativeInfinity(value) && !double.IsPositiveInfinity(value)) {
                        //расчитываем размер от общего размера окна
                        fe.MinWidth = SystemParameters.PrimaryScreenWidth * value / 100;
                        fe.MinHeight = SystemParameters.PrimaryScreenHeight * value / 100;

                        if(GetUseMaxAndMin(fe))
                        {
                            fe.MaxWidth = fe.MinWidth;
                            fe.MaxHeight = fe.MinHeight;
                        }
                    }
                    else {
                        fe.MinWidth = 0;
                        fe.MinHeight = 0;

                        if (GetUseMaxAndMin(fe))
                        {
                            fe.MaxWidth = fe.MinWidth;
                            fe.MaxHeight = fe.MinHeight;
                        }
                    }
                }
            })));

        public static void SetProcentScreenAllSize(DependencyObject element, double value) {
            element.SetValue(ProcentScreenAllSizeProperty, value);
        }

        public static double GetProcentScreenAllSize(DependencyObject element) {
            return (double)element.GetValue(ProcentScreenAllSizeProperty);
        }


        public static readonly DependencyProperty SizeOnPositionProperty = DependencyProperty.RegisterAttached(
            "SizeOnPosition", typeof(double), typeof(MinMaxSizeFrameworkElementBehaviour), new PropertyMetadata(double.PositiveInfinity));

        public static void SetSizeOnPosition(DependencyObject element, double value) {
            element.SetValue(SizeOnPositionProperty, value);
        }

        public static double GetSizeOnPosition(DependencyObject element) {
            return (double)element.GetValue(SizeOnPositionProperty);
        }


        public static readonly DependencyProperty ParentPositionProperty = DependencyProperty.RegisterAttached(
            "ParentPosition", typeof(WindowPosition), typeof(MinMaxSizeFrameworkElementBehaviour), new PropertyMetadata(WindowPosition.Non, new PropertyChangedCallback((d, a) => {
                if (d is FrameworkElement) {
                    FrameworkElement fe = (FrameworkElement)d;

                    double value = GetSizeOnPosition(fe);

                    switch ((WindowPosition)a.NewValue) {
                        case WindowPosition.Left:
                        case WindowPosition.Right:
                        case WindowPosition.LeftTop:
                        case WindowPosition.RightTop:
                        case WindowPosition.RightBottom:
                        case WindowPosition.LeftBottom:
                            fe.MaxWidth = SystemParameters.PrimaryScreenWidth * value / 100;
                            fe.MaxHeight = double.PositiveInfinity;
                            break;
                        case WindowPosition.Top:
                        case WindowPosition.Bottom:
                            fe.MaxWidth = double.PositiveInfinity;
                            fe.MaxHeight = SystemParameters.PrimaryScreenHeight * value / 100;
                            break;
                        case WindowPosition.None:
                            fe.MaxWidth = double.PositiveInfinity;
                            fe.MaxHeight = double.PositiveInfinity;
                            break;
                    }
                }
            })));


        public static void SetParentPosition(DependencyObject element, WindowPosition value) {
            element.SetValue(ParentPositionProperty, value);
        }

        public static WindowPosition GetParentPosition(DependencyObject element) {
            return (WindowPosition)element.GetValue(ParentPositionProperty);
        }


        public static readonly DependencyProperty MaxHeightOnParentProperty = DependencyProperty.RegisterAttached(
            "MaxHeightOnParent", typeof(bool), typeof(MinMaxSizeFrameworkElementBehaviour), new PropertyMetadata(false, new PropertyChangedCallback((d, a) => {
                if (d is FrameworkElement) {
                    FrameworkElement fe = (FrameworkElement)d;
                    if ((bool)a.NewValue) {
                        FrameworkElement parent = (FrameworkElement)fe.Parent;
                        if (parent != null) {
                            parent.SizeChanged += (sender, e) => {
                                fe.MaxHeight = e.NewSize.Height != 0 ? e.NewSize.Height : double.PositiveInfinity;
                            };
                        }
                    }
                    else
                        fe.MaxHeight = double.PositiveInfinity;
                }
            })));

        public static void SetMaxHeightOnParent(DependencyObject element, bool value) {
            element.SetValue(MaxHeightOnParentProperty, value);
        }

        public static bool GetMaxHeightOnParent(DependencyObject element) {
            return (bool)element.GetValue(MaxHeightOnParentProperty);
        }


        public static readonly DependencyProperty MinOnParentPositionProperty = DependencyProperty.RegisterAttached(
            "MinOnParentPosition", typeof(WindowPosition), typeof(MinMaxSizeFrameworkElementBehaviour), new PropertyMetadata(WindowPosition.Non, new PropertyChangedCallback((d, a) => {
                if (d is FrameworkElement) {
                    FrameworkElement fe = (FrameworkElement)d;
                    double value = GetSizeOnPosition(fe);

                    fe.MinHeight = fe.MinWidth = 0;

                    switch ((WindowPosition)a.NewValue) {
                        case WindowPosition.Left:
                        case WindowPosition.Right:
                        case WindowPosition.LeftTop:
                        case WindowPosition.LeftBottom:
                        case WindowPosition.RightTop:
                        case WindowPosition.RightBottom:
                            fe.MinWidth = value;
                            break;
                        case WindowPosition.Top:
                        case WindowPosition.Bottom:
                            fe.MinHeight = value;
                            break;
                    }
                }
            })));


        public static void SetMinOnParentPosition(DependencyObject element, WindowPosition value) {
            element.SetValue(MinOnParentPositionProperty, value);
        }

        public static WindowPosition GetMinOnParentPosition(DependencyObject element) {
            return (WindowPosition)element.GetValue(MinOnParentPositionProperty);
        }
    }
}
