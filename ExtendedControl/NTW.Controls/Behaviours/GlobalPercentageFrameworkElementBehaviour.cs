using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace NTW.Controls.Behaviours {
    public class GlobalPercentageFrameworkElementBehaviour {

        public static readonly DependencyProperty ProcentageProperty =
            DependencyProperty.RegisterAttached("Procentage", typeof(int), typeof(GlobalPercentageFrameworkElementBehaviour), new PropertyMetadata(0, new PropertyChangedCallback((d, a) => {
                if (d is FrameworkElement) {
                    FrameworkElement fe = (FrameworkElement)d;
                    AgeType at = GetAgeType(fe);
                    int procent = (int)a.NewValue;
                    bool usePosition = GetUsePosition(fe);
                    TypeSize type = GetType(fe);
                    SetParametry(fe, procent, usePosition, at, type);
                }
            })));

        public static int GetProcentage(DependencyObject obj) {
            return (int)obj.GetValue(ProcentageProperty);
        }

        public static void SetProcentage(DependencyObject obj, int value) {
            obj.SetValue(ProcentageProperty, value);
        }


        public static readonly DependencyProperty AgeTypeProperty =
            DependencyProperty.RegisterAttached("AgeType", typeof(AgeType), typeof(GlobalPercentageFrameworkElementBehaviour), new PropertyMetadata(AgeType.None, new PropertyChangedCallback((d, a) => {
                if (d is FrameworkElement) {
                    FrameworkElement fe = (FrameworkElement)d;
                    AgeType at = (AgeType)a.NewValue;
                    int procent = GetProcentage(fe);
                    bool usePosition = GetUsePosition(fe);
                    TypeSize type = GetType(fe);
                    SetParametry(fe, procent, usePosition, at, type);
                }
            })));

        public static AgeType GetAgeType(DependencyObject obj) {
            return (AgeType)obj.GetValue(AgeTypeProperty);
        }

        public static void SetAgeType(DependencyObject obj, AgeType value) {
            obj.SetValue(AgeTypeProperty, value);
        }



        public static bool GetUseParentSize(DependencyObject obj) {
            return (bool)obj.GetValue(UseParentSizeProperty);
        }

        public static void SetUseParentSize(DependencyObject obj, bool value) {
            obj.SetValue(UseParentSizeProperty, value);
        }

        public static readonly DependencyProperty UseParentSizeProperty =
            DependencyProperty.RegisterAttached("UseParentSize", typeof(bool), typeof(GlobalPercentageFrameworkElementBehaviour), new PropertyMetadata(false, new PropertyChangedCallback((d, a) => {
                if (d is FrameworkElement) {
                    FrameworkElement fe = (FrameworkElement)d;
                    AgeType at = GetAgeType(fe);
                    int procent = GetProcentage(fe);
                    bool usePosition = GetUsePosition(fe);
                    TypeSize type = GetType(fe);
                    SetParametry(fe, procent, usePosition, at, type);
                }
            })));



        public static bool GetUsePosition(DependencyObject obj) {
            return (bool)obj.GetValue(UsePositionProperty);
        }

        public static void SetUsePosition(DependencyObject obj, bool value) {
            obj.SetValue(UsePositionProperty, value);
        }

        public static readonly DependencyProperty UsePositionProperty =
            DependencyProperty.RegisterAttached("UsePosition", typeof(bool), typeof(GlobalPercentageFrameworkElementBehaviour), new PropertyMetadata(false, new PropertyChangedCallback((d, a) => {
                if (d is FrameworkElement) {
                    FrameworkElement fe = (FrameworkElement)d;
                    AgeType at = GetAgeType(fe);
                    int procent = GetProcentage(fe);
                    bool usePosition = (bool)a.NewValue;
                    TypeSize type = GetType(fe);
                    SetParametry(fe, procent, usePosition, at, type);
                }
            })));


        public static TypeSize GetType(DependencyObject obj) {
            return (TypeSize)obj.GetValue(TypeProperty);
        }

        public static void SetType(DependencyObject obj, TypeSize value) {
            obj.SetValue(TypeProperty, value);
        }

        // Using a DependencyProperty as the backing store for Type.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.RegisterAttached("Type", typeof(TypeSize), typeof(GlobalPercentageFrameworkElementBehaviour), new PropertyMetadata(TypeSize.Current, new PropertyChangedCallback((d, a) => {
                if (d is FrameworkElement) {
                    FrameworkElement fe = (FrameworkElement)d;
                    AgeType at = GetAgeType(fe);
                    int procent = GetProcentage(fe);
                    bool usePosition = GetUsePosition(fe);
                    TypeSize type = (TypeSize)a.NewValue;
                    SetParametry(fe, procent, usePosition, at, type);
                }
            })));




        protected static void SetParametry(FrameworkElement fe, int procent, bool UsePosition, AgeType at, TypeSize type) {

            Size primarySize = new Size(SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight);
            if (GetUseParentSize(fe)) {
                if (fe.Parent is FrameworkElement) {
                    var parent = (FrameworkElement)fe.Parent;
                    primarySize = new Size(parent.Width == 0 ? parent.ActualWidth : parent.Width, parent.Height == 0 ? parent.ActualHeight : parent.Height);
                }
                else if (fe.Parent is Decorator) {
                    var parent = (Decorator)fe.Parent;
                    primarySize = new Size(parent.Width == 0 ? parent.ActualWidth : parent.Width, parent.Height == 0 ? parent.ActualHeight : parent.Height);
                }
            }

            fe.Width = double.NaN;
            fe.MinWidth = 0;
            fe.MaxWidth = double.PositiveInfinity;
            fe.Height = double.NaN;
            fe.MinHeight = 0;
            fe.MaxHeight = double.PositiveInfinity;

            if (!GetUsePosition(fe)) {
                switch (at) {
                    case AgeType.Width:
                        switch (type) {
                            case TypeSize.Current:
                                fe.Width = primarySize.Width * procent / 100;
                                fe.Height = double.NaN;
                                break;
                            case TypeSize.Static:
                                fe.Width = fe.MinWidth = fe.MaxWidth = primarySize.Width * procent / 100;
                                fe.Height = double.NaN;
                                fe.MinHeight = 0;
                                fe.MaxHeight = double.PositiveInfinity;
                                break;
                            case TypeSize.Max:
                                fe.MaxWidth = primarySize.Width * procent / 100;
                                fe.MaxHeight = double.PositiveInfinity;
                                break;
                            case TypeSize.Min:
                                fe.MinWidth = primarySize.Width * procent / 100;
                                fe.MinHeight = 0;
                                break;
                        }
                        break;
                    case AgeType.Height:
                        switch (type) {
                            case TypeSize.Current:
                                fe.Width = double.NaN;
                                fe.Height = primarySize.Height * procent / 100;
                                break;
                            case TypeSize.Static:
                                fe.Width = fe.MinWidth = fe.MaxWidth = double.NaN;
                                fe.Height = fe.MinHeight = fe.MaxHeight = primarySize.Height * procent / 100;
                                break;
                            case TypeSize.Max:
                                fe.MaxWidth = double.PositiveInfinity;
                                fe.MaxHeight = primarySize.Height * procent / 100;
                                break;
                            case TypeSize.Min:
                                fe.MinWidth = 0;
                                fe.MinHeight = primarySize.Height * procent / 100;
                                break;
                        }
                        break;
                    case AgeType.FullSize:
                        switch (type) {
                            case TypeSize.Current:
                                fe.Width = primarySize.Width * procent / 100;
                                fe.Height = primarySize.Height * procent / 100;
                                break;
                            case TypeSize.Static:
                                fe.Width = fe.MinWidth = fe.MaxWidth = primarySize.Width * procent / 100;
                                fe.Height = fe.MinHeight = fe.MaxHeight = primarySize.Height * procent / 100;
                                break;
                            case TypeSize.Max:
                                fe.MaxWidth = primarySize.Width * procent / 100;
                                fe.MaxHeight = primarySize.Height * procent / 100;
                                break;
                            case TypeSize.Min:
                                fe.MinWidth = primarySize.Width * procent / 100;
                                fe.MinHeight = primarySize.Height * procent / 100;
                                break;
                        }
                        break;
                }
            }
            else {
                Window parent = NTW.Common.FindAncestor<Window>(fe);
                if (parent != null) {
                    switch (WindowPositionBehaviour.GetWindowPosition(parent)) {
                        case Data.WindowPosition.Left:
                        case Data.WindowPosition.LeftBottom:
                        case Data.WindowPosition.LeftTop:
                        case Data.WindowPosition.Right:
                        case Data.WindowPosition.RightBottom:
                        case Data.WindowPosition.RightTop:
                            switch (type) {
                                case TypeSize.Current:
                                    fe.Width = double.NaN;
                                    fe.Height = primarySize.Height * procent / 100;
                                    break;
                                case TypeSize.Static:
                                    fe.Width = double.NaN;
                                    fe.MinWidth = 0;
                                    fe.MaxWidth = double.PositiveInfinity;
                                    fe.Height = fe.MinHeight = fe.MaxHeight = primarySize.Height * procent / 100;
                                    break;
                                case TypeSize.Max:
                                    fe.MaxWidth = double.PositiveInfinity;
                                    fe.MaxHeight = primarySize.Height * procent / 100;
                                    break;
                                case TypeSize.Min:
                                    fe.MinWidth = 0;
                                    fe.MinHeight = primarySize.Height * procent / 100;
                                    break;
                            }
                            break;
                        case Data.WindowPosition.Top:
                        case Data.WindowPosition.Bottom:
                            switch (type) {
                                case TypeSize.Current:
                                    fe.Width = primarySize.Width * procent / 100;
                                    fe.Height = double.NaN;
                                    break;
                                case TypeSize.Static:
                                    fe.Width = fe.MinWidth = fe.MaxWidth = primarySize.Width * procent / 100;
                                    fe.Height = double.NaN;
                                    fe.MinHeight = 0;
                                    fe.MaxHeight = double.PositiveInfinity;
                                    break;
                                case TypeSize.Max:
                                    fe.MaxWidth = primarySize.Width * procent / 100;
                                    fe.MaxHeight = double.PositiveInfinity;
                                    break;
                                case TypeSize.Min:
                                    fe.MinWidth = primarySize.Width * procent / 100;
                                    fe.MinHeight = 0;
                                    break;
                            }
                            break;
                    }
                }
                else {
                    Popup pParent = NTW.Common.FindAncestor<Popup>(fe);

                    if(pParent != null) {
                        switch (FrameworkElementPositionBehaviour.GetFrameworkElementPosition(pParent)) {
                            case Data.WindowPosition.Left:
                            case Data.WindowPosition.LeftBottom:
                            case Data.WindowPosition.LeftTop:
                            case Data.WindowPosition.Right:
                            case Data.WindowPosition.RightBottom:
                            case Data.WindowPosition.RightTop:
                                switch (type) {
                                    case TypeSize.Current:
                                        fe.Width = double.NaN;
                                        fe.Height = primarySize.Height * procent / 100;
                                        break;
                                    case TypeSize.Static:
                                        fe.Width = fe.MinWidth = fe.MaxWidth = double.NaN;
                                        fe.Height = fe.MinHeight = fe.MaxHeight = primarySize.Height * procent / 100;
                                        break;
                                    case TypeSize.Max:
                                        fe.MaxWidth = double.NaN;
                                        fe.MaxHeight = primarySize.Height * procent / 100;
                                        break;
                                    case TypeSize.Min:
                                        fe.MinWidth = double.NaN;
                                        fe.MinHeight = primarySize.Height * procent / 100;
                                        break;
                                }
                                break;
                            case Data.WindowPosition.Top:
                            case Data.WindowPosition.Bottom:
                                switch (type) {
                                    case TypeSize.Current:
                                        fe.Width = primarySize.Width * procent / 100;
                                        fe.Height = double.NaN;
                                        break;
                                    case TypeSize.Static:
                                        fe.Width = fe.MinWidth = fe.MaxWidth = primarySize.Width * procent / 100;
                                        fe.Height = fe.MinHeight = fe.MaxHeight = double.NaN;
                                        break;
                                    case TypeSize.Max:
                                        fe.MaxWidth = primarySize.Width * procent / 100;
                                        fe.MaxHeight = double.NaN;
                                        break;
                                    case TypeSize.Min:
                                        fe.MinWidth = primarySize.Width * procent / 100;
                                        fe.MinHeight = double.NaN;
                                        break;
                                }
                                break;
                        }
                    }
                }
            }
        }

        public enum AgeType {
            None,
            Width,
            Height,
            FullSize
        }

        public enum TypeSize {
            Static,
            Current,
            Max,
            Min
        }
    }
}
