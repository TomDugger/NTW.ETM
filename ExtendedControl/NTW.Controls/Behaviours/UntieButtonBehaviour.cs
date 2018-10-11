using NTW.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NTW.Controls.Behaviours
{
    public class UntieButtonBehaviour
    {
        public static readonly DependencyProperty UseUntieButtonProperty = DependencyProperty.RegisterAttached(
            "UseUntieButton", typeof(bool), typeof(UntieButtonBehaviour));

        public static readonly DependencyProperty UntieButtonPositionProperty = DependencyProperty.RegisterAttached(
            "UntieButtonPosition", typeof(WindowPosition), typeof(UntieButtonBehaviour), new PropertyMetadata(WindowPosition.Non, new PropertyChangedCallback((d, a) =>
            {
                if (d is FrameworkElement)
                {
                    FrameworkElement fe = (FrameworkElement)d;

                    switch ((WindowPosition)a.NewValue) {
                        case WindowPosition.Left:
                            fe.HorizontalAlignment = HorizontalAlignment.Right;
                            fe.VerticalAlignment = VerticalAlignment.Top;
                            fe.OpacityMask = new VisualBrush(new Polygon() { Fill = new SolidColorBrush(Colors.White), Points = new PointCollection(new Point[] { new Point(0, 0), new Point(30, 0), new Point(30, 30)}) });
                            break;
                        case WindowPosition.Top:
                            fe.HorizontalAlignment = HorizontalAlignment.Left;
                            fe.VerticalAlignment = VerticalAlignment.Bottom;
                            fe.OpacityMask = new VisualBrush(new Polygon() { Fill = new SolidColorBrush(Colors.White), Points = new PointCollection(new Point[] { new Point(0, 0), new Point(30, 30), new Point(0, 30) }) });
                            break;
                        case WindowPosition.Right:
                            fe.HorizontalAlignment = HorizontalAlignment.Left;
                            fe.VerticalAlignment = VerticalAlignment.Top;
                            fe.OpacityMask = new VisualBrush(new Polygon() { Fill = new SolidColorBrush(Colors.White), Points = new PointCollection(new Point[] { new Point(0, 0), new Point(30, 0), new Point(0, 30) }) });
                            break;
                        case WindowPosition.Bottom:
                            fe.HorizontalAlignment = HorizontalAlignment.Left;
                            fe.VerticalAlignment = VerticalAlignment.Top;
                            fe.OpacityMask = new VisualBrush(new Polygon() { Fill = new SolidColorBrush(Colors.White), Points = new PointCollection(new Point[] { new Point(0, 0), new Point(30, 0), new Point(0, 30) }) });
                            break;
                        case WindowPosition.None:
                            fe.HorizontalAlignment = HorizontalAlignment.Left;
                            fe.VerticalAlignment = VerticalAlignment.Top;
                            fe.OpacityMask = new VisualBrush(new Polygon() { Fill = new SolidColorBrush(Colors.White), Points = new PointCollection(new Point[] { new Point(0, 0), new Point(30, 0), new Point(0, 30) }) });
                            break;
                    }
                }
            })));

        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
            "Command", typeof(ICommand), typeof(UntieButtonBehaviour));


        public static void SetUseUntieButton(DependencyObject element, bool value) {
            element.SetValue(UseUntieButtonProperty, value);
        }

        public static bool GetUseUntieButton(DependencyObject element) {
            return (bool)element.GetValue(UseUntieButtonProperty);
        }


        public static void SetUntieButtonPosition(DependencyObject element, WindowPosition value) {
            element.SetValue(UntieButtonPositionProperty, value);
        }

        public static WindowPosition GetUntieButtonPosition(DependencyObject element) {
            return (WindowPosition)element.GetValue(UntieButtonPositionProperty);
        }


        public static void SetCommand(DependencyObject element, ICommand value) {
            element.SetValue(CommandProperty, value);
        }

        public static ICommand GetCommand(DependencyObject element) {
            return (ICommand)element.GetValue(CommandProperty);
        }
    }
}
