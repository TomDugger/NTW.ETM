using NTW.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace NTW.Controls.Behaviours
{
    public class WindowPositionBehaviour
    {
        public static readonly DependencyProperty WindowThicknessPropery = DependencyProperty.RegisterAttached(
            "WindowThickness", typeof(Thickness), typeof(WindowPositionBehaviour), new PropertyMetadata(new Thickness()));

        public static readonly DependencyProperty WindowPositionProperty = DependencyProperty.RegisterAttached(
            "WindowPosition", typeof(WindowPosition), typeof(WindowPositionBehaviour), new PropertyMetadata(WindowPosition.Non, new PropertyChangedCallback(WindowParametryChanged)));

        public static readonly DependencyProperty WindowVisibleBorderProperty = DependencyProperty.RegisterAttached(
            "WindowVisibleBorder", typeof(bool), typeof(WindowPositionBehaviour), new PropertyMetadata(true));

        public static readonly DependencyProperty WindowScreenProperty = DependencyProperty.RegisterAttached(
            "WindowScreen", typeof(int), typeof(WindowPositionBehaviour), new PropertyMetadata(0));

        #region Methods
        public static void SetWindowThickness(DependencyObject element, Thickness value) {
            element.SetValue(WindowThicknessPropery, value);
        }

        public static Thickness GetWindowThickness(DependencyObject element) {
            return (Thickness)element.GetValue(WindowThicknessPropery);
        }


        public static void SetWindowPosition(DependencyObject element, WindowPosition value)
        {
            element.SetValue(WindowPositionProperty, value);
        }

        public static WindowPosition GetWindowPosition(DependencyObject element)
        {
            return (WindowPosition)element.GetValue(WindowPositionProperty);
        }


        public static void SetWindowVisibleBorder(DependencyObject element, bool value) {
            element.SetValue(WindowVisibleBorderProperty, value);
        }

        public static bool GetWindowVisibleBorder(DependencyObject element) {
            return (bool)element.GetValue(WindowVisibleBorderProperty);
        }


        public static void SetWindowScreen(DependencyObject element, int value) {
            element.SetValue(WindowScreenProperty, value);
        }

        public static int GetWindowScreen(DependencyObject element) {
            return (int)element.GetValue(WindowScreenProperty);
        }
        #endregion

        #region Helps
        private static void WindowParametryChanged(DependencyObject d, DependencyPropertyChangedEventArgs a) {
            if (d is Window)
            {
                Window w = d as Window;

                var size = System.Windows.Forms.Screen.AllScreens[GetWindowScreen(w)].Bounds.Size;
                var location = System.Windows.Forms.Screen.AllScreens[GetWindowScreen(w)].Bounds.Location;

                WindowPosition position = GetWindowPosition(d);

                w.SizeChanged -= WindowRight_SizeChanged;
                w.SizeChanged -= WindowBottom_SizeChanged;

                Thickness th = GetWindowThickness(w);
                Point indent = new Point();
                if (w.Owner != null)
                {
                    position = WindowPositionBehaviour.GetWindowPosition(w.Owner);
                    switch (WindowPositionBehaviour.GetWindowPosition(w.Owner))
                    {
                        case WindowPosition.Left:
                        case WindowPosition.LeftBottom:
                        case WindowPosition.LeftTop:
                            indent.X = w.Owner.Width + (HideButtonBehaviour.GetUseHideButton(w.Owner) ? -30 + w.Owner.Left - location.X - 6 : 0);
                            break;
                        case WindowPosition.Right:
                        case WindowPosition.RightBottom:
                        case WindowPosition.RightTop:
                            indent.X = -(w.Owner.Width - (HideButtonBehaviour.GetUseHideButton(w.Owner) ? 30 : 0));
                            break;
                        case WindowPosition.Top:
                            indent.Y = w.Owner.Height + (HideButtonBehaviour.GetUseHideButton(w.Owner) ? -30 + w.Owner.Top - location.Y - 6 : 0);
                            break;
                        case WindowPosition.Bottom:
                            indent.Y = -w.Owner.Height + (HideButtonBehaviour.GetUseHideButton(w.Owner) ? 30 : 0);
                            break;
                    }
                }

                switch (position)
                {
                    case WindowPosition.Left:
                        w.SizeToContent = SizeToContent.Height;
                        w.Left = location.X + th.Left + indent.X;
                        w.Top = location.Y + th.Top + indent.Y; ;

                        w.SizeChanged -= WindowRightBottom_SizeChanged;
                        w.SizeChanged -= WindowRight_SizeChanged;
                        w.SizeChanged -= WindowBottom_SizeChanged;
                        break;
                    case WindowPosition.Right:
                        w.SizeToContent = SizeToContent.Height;

                        w.Left = location.X + size.Width + th.Left - th.Right - NTW.Common.GetIndentTaskBar(false) + indent.X;
                        w.Top = location.Y + th.Top + indent.Y;

                        w.SizeChanged -= WindowRightBottom_SizeChanged;
                        w.SizeChanged -= WindowBottom_SizeChanged;

                        w.SizeChanged += WindowRight_SizeChanged;
                        break;
                    case WindowPosition.Top:
                        w.SizeToContent = SizeToContent.Width;

                        w.Left = location.X + th.Left + indent.X;
                        w.Top = location.Y + th.Top + indent.Y;

                        w.SizeChanged -= WindowRightBottom_SizeChanged;
                        w.SizeChanged -= WindowRight_SizeChanged;
                        w.SizeChanged -= WindowBottom_SizeChanged;
                        break;
                    case WindowPosition.Bottom:
                        w.SizeToContent = SizeToContent.Width;

                        w.Left = location.X + th.Left + indent.X;
                        w.Top = location.Y * 2 + size.Height + th.Top - th.Bottom - NTW.Common.GetIndentTaskBar(true) + indent.Y;

                        w.SizeChanged -= WindowRightBottom_SizeChanged;
                        w.SizeChanged -= WindowRight_SizeChanged;

                        w.SizeChanged += WindowBottom_SizeChanged;
                        break;
                    case WindowPosition.None:
                        break;
                    case WindowPosition.LeftTop:
                        w.SizeToContent = SizeToContent.WidthAndHeight;

                        w.Left = location.X + th.Left + indent.X;
                        w.Top = location.Y + th.Top + indent.Y;

                        w.SizeChanged -= WindowRightBottom_SizeChanged;
                        w.SizeChanged -= WindowRight_SizeChanged;
                        w.SizeChanged -= WindowBottom_SizeChanged;
                        break;
                    case WindowPosition.LeftBottom:
                        w.SizeToContent = SizeToContent.WidthAndHeight;

                        w.Left = location.X + th.Left + indent.X;
                        w.Top = location.Y + size.Height + th.Top - th.Bottom - NTW.Common.GetIndentTaskBar(true) + indent.Y;

                        w.SizeChanged -= WindowRightBottom_SizeChanged;
                        w.SizeChanged -= WindowRight_SizeChanged;

                        w.SizeChanged += WindowBottom_SizeChanged;
                        break;
                    case WindowPosition.RightTop:
                        w.SizeToContent = SizeToContent.WidthAndHeight;

                        w.Left = location.X + size.Width + th.Left - th.Right - NTW.Common.GetIndentTaskBar(false) + indent.X;
                        w.Top = location.Y + th.Top + indent.Y;

                        w.SizeChanged -= WindowBottom_SizeChanged;
                        w.SizeChanged -= WindowRightBottom_SizeChanged;

                        w.SizeChanged += WindowRight_SizeChanged;
                        break;
                    case WindowPosition.RightBottom:
                        w.SizeToContent = SizeToContent.WidthAndHeight;

                        w.Left = location.X + size.Width + th.Left - th.Right - NTW.Common.GetIndentTaskBar(false) + indent.X;
                        w.Top = location.Y + size.Height + th.Top - th.Bottom - NTW.Common.GetIndentTaskBar(true) + indent.Y;

                        w.SizeChanged -= WindowRight_SizeChanged;
                        w.SizeChanged -= WindowBottom_SizeChanged;

                        w.SizeChanged += WindowRightBottom_SizeChanged;
                        break;
                }
            }
        } 

        private static void WindowRightBottom_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Window w = sender as Window;

            var size = System.Windows.Forms.Screen.AllScreens[GetWindowScreen(w)].Bounds.Size;
            var location = System.Windows.Forms.Screen.AllScreens[GetWindowScreen(w)].Bounds.Location;

            Point indent = new Point();
            if (w.Owner != null)
            {
                WindowPosition position = WindowPositionBehaviour.GetWindowPosition(w.Owner);
                switch (WindowPositionBehaviour.GetWindowPosition(w.Owner))
                {
                    case WindowPosition.Left:
                    case WindowPosition.LeftBottom:
                    case WindowPosition.LeftTop:
                        indent.X = w.Owner.Width + (HideButtonBehaviour.GetUseHideButton(w.Owner) ? -30 : 0);
                        break;
                    case WindowPosition.Right:
                    case WindowPosition.RightBottom:
                    case WindowPosition.RightTop:
                        indent.X = -(w.Owner.Width - (HideButtonBehaviour.GetUseHideButton(w.Owner) ? 30 : 0));
                        break;
                    case WindowPosition.Top:
                        indent.Y = w.Owner.Height + (HideButtonBehaviour.GetUseHideButton(w.Owner) ? -30 + (size.Width - w.Owner.ActualHeight - 6) : 0);
                        break;
                    case WindowPosition.Bottom:
                        indent.Y = -w.Owner.Height + (HideButtonBehaviour.GetUseHideButton(w.Owner) ? 30 : 0);
                        break;
                }
            }

            Thickness th = WindowPositionBehaviour.GetWindowThickness(w);

            w.Left = location.X + size.Width - e.NewSize.Width - NTW.Common.GetIndentTaskBar(false) - th.Left - th.Right + indent.X;
            w.Top = location.Y + size.Height - e.NewSize.Height - NTW.Common.GetIndentTaskBar(true) - th.Top - th.Bottom + indent.Y;
        }

        private static void WindowRight_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Window w = sender as Window;

            var size = System.Windows.Forms.Screen.AllScreens[GetWindowScreen(w)].Bounds.Size;
            var location = System.Windows.Forms.Screen.AllScreens[GetWindowScreen(w)].Bounds.Location;

            Point indent = new Point();
            if (w.Owner != null)
            {
                WindowPosition position = WindowPositionBehaviour.GetWindowPosition(w.Owner);
                switch (WindowPositionBehaviour.GetWindowPosition(w.Owner))
                {
                    case WindowPosition.Left:
                    case WindowPosition.LeftBottom:
                    case WindowPosition.LeftTop:
                        indent.X = (HideButtonBehaviour.GetUseHideButton(w.Owner) ? -30 : 0);
                        break;
                    case WindowPosition.Right:
                    case WindowPosition.RightBottom:
                    case WindowPosition.RightTop:
                        indent.X = -(HideButtonBehaviour.GetUseHideButton(w.Owner) ? 30 - (size.Width - w.Owner.Left - w.Owner.ActualWidth - 6) : -(size.Width - w.Owner.Left - w.Owner.ActualWidth - 6));
                        break;
                    case WindowPosition.Top:
                        indent.Y = (HideButtonBehaviour.GetUseHideButton(w.Owner) ? -30 : 0);
                        break;
                    case WindowPosition.Bottom:
                        indent.Y = (HideButtonBehaviour.GetUseHideButton(w.Owner) ? 30 : 0);
                        break;
                }
            }

            Thickness th = WindowPositionBehaviour.GetWindowThickness(w);

            w.Left = -(w.Owner != null ? w.Owner.Width + indent.X : 0) + location.X + size.Width - e.NewSize.Width - NTW.Common.GetIndentTaskBar(false) - th.Left - th.Right;
            w.Top = th.Top;
        }

        private static void WindowBottom_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Window w = sender as Window;

            var size = System.Windows.Forms.Screen.AllScreens[GetWindowScreen(w)].Bounds.Size;
            var location = System.Windows.Forms.Screen.AllScreens[GetWindowScreen(w)].Bounds.Location;

            Point indent = new Point();
            if (w.Owner != null)
            {
                WindowPosition position = WindowPositionBehaviour.GetWindowPosition(w.Owner);
                switch (WindowPositionBehaviour.GetWindowPosition(w.Owner))
                {
                    case WindowPosition.Left:
                    case WindowPosition.LeftBottom:
                    case WindowPosition.LeftTop:
                        indent.X = w.Owner.Width + (HideButtonBehaviour.GetUseHideButton(w.Owner) ? -30 : 0);
                        break;
                    case WindowPosition.Right:
                    case WindowPosition.RightBottom:
                    case WindowPosition.RightTop:
                        indent.X = -(w.Owner.Width - (HideButtonBehaviour.GetUseHideButton(w.Owner) ? 30 : 0));
                        break;
                    case WindowPosition.Top:
                        indent.Y = w.Owner.Height + (HideButtonBehaviour.GetUseHideButton(w.Owner) ? -30 : 0);
                        break;
                    case WindowPosition.Bottom:
                        indent.Y = -w.Owner.Height + (HideButtonBehaviour.GetUseHideButton(w.Owner) ? 30 - ((size.Height - NTW.Common.GetIndentTaskBar(true)) - w.Owner.Top - w.Owner.ActualHeight - 6) : 0);
                        break;
                }
            }

            Thickness th = WindowPositionBehaviour.GetWindowThickness(w);

            w.Left = th.Top;
            w.Top = location.Y + size.Height - e.NewSize.Height - NTW.Common.GetIndentTaskBar(true) - th.Top - th.Bottom + indent.Y;
        }
        #endregion
    }
}
