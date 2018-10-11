using NTW.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace NTW.Controls.Behaviours
{
    public class FrameworkElementPositionBehaviour
    {
        public static readonly DependencyProperty FrameworkElementPositionProperty = DependencyProperty.RegisterAttached(
            "FrameworkElementPosition", typeof(WindowPosition), typeof(FrameworkElementPositionBehaviour), new PropertyMetadata(WindowPosition.Non, new PropertyChangedCallback((d, a) => {
                if (d is ItemsControl)
                {
                    ItemsControl ic = (ItemsControl)d;

                    switch ((WindowPosition)a.NewValue)
                    {
                        case WindowPosition.Left:
                            FrameworkElementFactory stack = new FrameworkElementFactory(typeof(StackPanel));
                            stack.SetValue(StackPanel.OrientationProperty, Orientation.Vertical);
                            ic.ItemsPanel = new ItemsPanelTemplate(stack);
                            break;
                        case WindowPosition.Right:
                            FrameworkElementFactory stack1 = new FrameworkElementFactory(typeof(StackPanel));
                            stack1.SetValue(StackPanel.OrientationProperty, Orientation.Vertical);
                            ic.ItemsPanel = new ItemsPanelTemplate(stack1);
                            break;
                        case WindowPosition.Top:
                            FrameworkElementFactory stack2 = new FrameworkElementFactory(typeof(StackPanel));
                            stack2.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
                            ic.ItemsPanel = new ItemsPanelTemplate(stack2);
                            break;
                        case WindowPosition.Bottom:
                            FrameworkElementFactory stack3 = new FrameworkElementFactory(typeof(StackPanel));
                            stack3.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
                            ic.ItemsPanel = new ItemsPanelTemplate(stack3);
                            break;
                        case WindowPosition.LeftTop:
                        case WindowPosition.LeftBottom:
                        case WindowPosition.RightTop:
                        case WindowPosition.RightBottom:
                        case WindowPosition.None:
                            ic.MaxWidth = 100;

                            FrameworkElementFactory wrap = new FrameworkElementFactory(typeof(WrapPanel));
                            wrap.SetValue(WrapPanel.OrientationProperty, Orientation.Horizontal);
                            ic.ItemsPanel = new ItemsPanelTemplate(wrap);
                            break;
                    }
                }
                else if (d is Grid)
                {
                    Grid g = (Grid)d;

                    Window parent = Common.FindAncestor<Window>(g);

                    var indexScreen = WindowPositionBehaviour.GetWindowScreen(parent);
                    var size = System.Windows.Forms.Screen.AllScreens[indexScreen].Bounds.Size;

                    Thickness th = WindowPositionBehaviour.GetWindowThickness(parent);

                    //как извлекать параметры для окна относительно указанного экрана

                    switch ((WindowPosition)a.NewValue)
                    {
                        case WindowPosition.Left:
                        case WindowPosition.Right:
                            g.MaxWidth = double.PositiveInfinity;
                            g.MaxHeight = g.Height = size.Height - th.Top - th.Bottom - NTW.Common.GetIndentTaskBar(true);

                            g.Width = double.NaN;
                            break;
                        case WindowPosition.Top:
                        case WindowPosition.Bottom:
                            g.MaxWidth = g.Width = size.Width - th.Left - th.Right - NTW.Common.GetIndentTaskBar(false);
                            g.MaxHeight = double.PositiveInfinity;
                            g.Height = double.NaN;
                            break;
                        case WindowPosition.None:
                        case WindowPosition.LeftTop:
                        case WindowPosition.LeftBottom:
                        case WindowPosition.RightTop:
                        case WindowPosition.RightBottom:
                            //vг... тут стоит задуматься о варации подгрузки данных для управления размером и расположением окна
                            g.MaxHeight = double.PositiveInfinity;
                            g.MaxWidth = double.PositiveInfinity;

                            g.Width = double.NaN;
                            g.Height = double.NaN;
                            break;
                    }
                }
                else if (d is WrapPanel)
                {
                    WrapPanel wp = (WrapPanel)d;

                    switch ((WindowPosition)a.NewValue)
                    {
                        case WindowPosition.Left:
                        case WindowPosition.Right:
                        case WindowPosition.LeftTop:
                        case WindowPosition.LeftBottom:
                        case WindowPosition.RightTop:
                        case WindowPosition.RightBottom:
                        case WindowPosition.None:
                            wp.Orientation = Orientation.Vertical;
                            break;
                        case WindowPosition.Top:
                        case WindowPosition.Bottom:
                            wp.Orientation = Orientation.Horizontal;
                            break;
                    }
                }
                else if (d is StackPanel)
                {
                    StackPanel sp = (StackPanel)d;
                    switch ((WindowPosition)a.NewValue)
                    {
                        case WindowPosition.Left:
                        case WindowPosition.Right:
                        case WindowPosition.LeftTop:
                        case WindowPosition.LeftBottom:
                        case WindowPosition.RightTop:
                        case WindowPosition.RightBottom:
                        case WindowPosition.None:
                            sp.Orientation = Orientation.Vertical;
                            break;
                        case WindowPosition.Top:
                        case WindowPosition.Bottom:
                            sp.Orientation = Orientation.Horizontal;
                            break;
                    }
                }
                else if (d is Popup)
                {
                    Popup p = (Popup)d;

                    switch ((WindowPosition)a.NewValue)
                    {
                        case WindowPosition.Left:
                            p.Placement = PlacementMode.Right;
                            if (p.Child is ItemsControl)
                            {
                                FrameworkElementFactory stack = new FrameworkElementFactory(typeof(StackPanel));
                                stack.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
                                (p.Child as ItemsControl).ItemsPanel = new ItemsPanelTemplate(stack);
                            }
                            break;
                        case WindowPosition.LeftTop:
                        case WindowPosition.LeftBottom:
                        case WindowPosition.RightTop:
                        case WindowPosition.RightBottom:
                        case WindowPosition.Right:
                            p.Placement = PlacementMode.Left;
                            if (p.Child is ItemsControl)
                            {
                                FrameworkElementFactory stack1 = new FrameworkElementFactory(typeof(StackPanel));
                                stack1.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
                                (p.Child as ItemsControl).ItemsPanel = new ItemsPanelTemplate(stack1);
                            }
                            break;
                        case WindowPosition.Top:
                            p.Placement = PlacementMode.Bottom;
                            if (p.Child is ItemsControl)
                            {
                                FrameworkElementFactory stack2 = new FrameworkElementFactory(typeof(StackPanel));
                                stack2.SetValue(StackPanel.OrientationProperty, Orientation.Vertical);
                                (p.Child as ItemsControl).ItemsPanel = new ItemsPanelTemplate(stack2);
                            }
                            break;
                        case WindowPosition.Bottom:
                            p.Placement = PlacementMode.Top;
                            if (p.Child is ItemsControl)
                            {
                                FrameworkElementFactory stack3 = new FrameworkElementFactory(typeof(StackPanel));
                                stack3.SetValue(StackPanel.OrientationProperty, Orientation.Vertical);
                                (p.Child as ItemsControl).ItemsPanel = new ItemsPanelTemplate(stack3);
                            }
                            break;
                    }
                }
                else if (d is Border)
                {
                    Border b = (Border)d;
                    switch ((WindowPosition)a.NewValue)
                    {
                        case WindowPosition.Left:
                            b.BorderThickness = new Thickness(2, 0, 0, 0);
                            ContextMenuService.SetPlacement(b, PlacementMode.Right);
                            break;
                        case WindowPosition.Top:
                            b.BorderThickness = new Thickness(0, 2, 0, 0);
                            ContextMenuService.SetPlacement(b, PlacementMode.Bottom);
                            break;
                        case WindowPosition.Right:
                            b.BorderThickness = new Thickness(0, 0, 2, 0);
                            ContextMenuService.SetPlacement(b, PlacementMode.Left);
                            break;
                        case WindowPosition.Bottom:
                            b.BorderThickness = new Thickness(0, 0, 0, 2);
                            ContextMenuService.SetPlacement(b, PlacementMode.Top);
                            break;
                        case WindowPosition.LeftTop:
                            b.BorderThickness = new Thickness(2, 2, 0, 0);
                            ContextMenuService.SetPlacement(b, PlacementMode.Right);
                            break;
                        case WindowPosition.RightTop:
                            b.BorderThickness = new Thickness(0, 2, 2, 0);
                            ContextMenuService.SetPlacement(b, PlacementMode.Left);
                            break;
                        case WindowPosition.RightBottom:
                            b.BorderThickness = new Thickness(2, 2, 2, 2);
                            ContextMenuService.SetPlacement(b, PlacementMode.Left);
                            break;
                        case WindowPosition.LeftBottom:
                            b.BorderThickness = new Thickness(2, 2, 2, 2);
                            ContextMenuService.SetPlacement(b, PlacementMode.Right);
                            break;
                        case WindowPosition.None:
                            b.BorderThickness = new Thickness(0);
                            ContextMenuService.SetPlacement(b, PlacementMode.MousePoint);
                            break;
                    }
                }
            })));
        
        public static readonly DependencyProperty PanelPositionProperty =
            DependencyProperty.RegisterAttached("PanelPosition", typeof(WindowPosition), typeof(FrameworkElementPositionBehaviour), new PropertyMetadata(WindowPosition.Non, new PropertyChangedCallback((d, a) => {
                if (d is Grid g)
                {
                    Window parent = Common.FindAncestor<Window>(g);

                    var indexScreen = WindowPositionBehaviour.GetWindowScreen(parent);
                    var size = System.Windows.Forms.Screen.AllScreens[indexScreen].Bounds.Size;

                    Thickness th = WindowPositionBehaviour.GetWindowThickness(parent);

                    switch ((WindowPosition)a.NewValue)
                    {
                        case WindowPosition.Left:
                        case WindowPosition.Right:

                        case WindowPosition.LeftTop:
                        case WindowPosition.LeftBottom:
                        case WindowPosition.RightTop:
                        case WindowPosition.RightBottom:
                            g.MaxWidth = double.PositiveInfinity;
                            g.MaxHeight = g.Height = size.Height - th.Top - th.Bottom - NTW.Common.GetIndentTaskBar(true);

                            g.Width = double.NaN;
                            break;
                        case WindowPosition.Top:
                        case WindowPosition.Bottom:
                            g.MaxWidth = g.Width = size.Width - th.Left - th.Right - NTW.Common.GetIndentTaskBar(false);
                            g.MaxHeight = double.PositiveInfinity;
                            g.Height = double.NaN;
                            break;
                        case WindowPosition.None:
                            //vг... тут стоит задуматься о варации подгрузки данных для управления размером и расположением окна
                            g.MaxHeight = double.PositiveInfinity;
                            g.MaxWidth = double.PositiveInfinity;

                            g.Width = double.NaN;
                            g.Height = double.NaN;
                            break;
                    }
                }
                else if (d is Border b)
                {
                    double size = GetPanelSize(b) == 0 ? double.PositiveInfinity : GetPanelSize(b);

                    switch ((WindowPosition)a.NewValue)
                    {
                        case WindowPosition.Left:
                        case WindowPosition.Right:

                        case WindowPosition.LeftTop:
                        case WindowPosition.LeftBottom:
                        case WindowPosition.RightTop:
                        case WindowPosition.RightBottom:
                            b.MaxWidth = b.Width = size;
                            b.MaxHeight = double.PositiveInfinity;

                            b.Height = double.NaN;
                            break;
                        case WindowPosition.Top:
                        case WindowPosition.Bottom:
                            b.MaxWidth = double.PositiveInfinity;
                            b.MaxHeight = b.Height = size;

                            b.Width = double.NaN;
                            break;
                        case WindowPosition.None:
                            //vг... тут стоит задуматься о варации подгрузки данных для управления размером и расположением окна
                            b.MaxHeight = double.PositiveInfinity;
                            b.MaxWidth = double.PositiveInfinity;

                            b.Width = double.NaN;
                            b.Height = double.NaN;
                            break;
                    }
                }
            })));

        public static readonly DependencyProperty PanelSizeProperty = DependencyProperty.RegisterAttached("PanelSize", typeof(double), typeof(FrameworkElementPositionBehaviour), new PropertyMetadata(0.0));

        #region Property
        public static WindowPosition GetPanelPosition(DependencyObject obj)
        {
            return (WindowPosition)obj.GetValue(PanelPositionProperty);
        }

        public static void SetPanelPosition(DependencyObject obj, WindowPosition value)
        {
            obj.SetValue(PanelPositionProperty, value);
        }


        public static double GetPanelSize(DependencyObject obj) {
            return (double)obj.GetValue(PanelSizeProperty);
        }

        public static void SetPanelSize(DependencyObject obj, double value) {
            obj.SetValue(PanelSizeProperty, value);
        }


        public static void SetFrameworkElementPosition(DependencyObject element, WindowPosition value)
        {
            element.SetValue(FrameworkElementPositionProperty, value);
        }

        public static WindowPosition GetFrameworkElementPosition(DependencyObject element)
        {
            return (WindowPosition)element.GetValue(FrameworkElementPositionProperty);
        }
        #endregion
    }
}
