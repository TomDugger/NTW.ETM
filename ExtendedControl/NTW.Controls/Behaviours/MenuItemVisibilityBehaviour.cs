using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace NTW.Controls.Behaviours
{
    public class MenuItemVisibilityBehaviour
    {
        public static readonly DependencyProperty UseAnimationProperty = DependencyProperty.RegisterAttached(
            "UseAnimation", typeof(bool), typeof(MenuItemVisibilityBehaviour), new PropertyMetadata(true));

        public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.RegisterAttached(
            "IsVisible", typeof(bool?), typeof(MenuItemVisibilityBehaviour), new PropertyMetadata(null, new PropertyChangedCallback((d, a) => {
                if (d is FrameworkElement) {

                    FrameworkElement fe = (FrameworkElement)d;

                    if ((bool)a.NewValue) {
                        if (GetUseAnimation(fe))
                        {
                            fe.RenderTransformOrigin = new Point(0.5, 0.5);
                            fe.RenderTransform = new ScaleTransform();
                            DoubleAnimation animCenterX = new DoubleAnimation(0, 1, new Duration(new TimeSpan(0, 0, 0, 0, 300)));
                            DoubleAnimation animCenterY = new DoubleAnimation(0, 1, new Duration(new TimeSpan(0, 0, 0, 0, 300)));
                            Storyboard sb = new Storyboard();
                            Storyboard.SetTargetProperty(animCenterX, new PropertyPath("RenderTransform.ScaleX"));
                            Storyboard.SetTargetProperty(animCenterY, new PropertyPath("RenderTransform.ScaleY"));
                            sb.Children.Add(animCenterX);
                            sb.Children.Add(animCenterY);
                            fe.BeginStoryboard(sb);
                        }
                        else
                        {
                            fe.RenderTransform = new ScaleTransform(1, 1);
                        }
                    }
                    else {
                        if (GetUseAnimation(fe))
                        {
                            fe.RenderTransformOrigin = new Point(0.5, 0.5);
                            fe.RenderTransform = new ScaleTransform();
                            DoubleAnimation animCenterX = new DoubleAnimation(1, 0, new Duration(new TimeSpan(0, 0, 0, 0, 300)));
                            DoubleAnimation animCenterY = new DoubleAnimation(1, 0, new Duration(new TimeSpan(0, 0, 0, 0, 300)));
                            Storyboard sb = new Storyboard();
                            Storyboard.SetTargetProperty(animCenterX, new PropertyPath("RenderTransform.ScaleX"));
                            Storyboard.SetTargetProperty(animCenterY, new PropertyPath("RenderTransform.ScaleY"));
                            sb.Children.Add(animCenterX);
                            sb.Children.Add(animCenterY);
                            fe.BeginStoryboard(sb);
                        }
                        else
                        {
                            fe.RenderTransform = new ScaleTransform(0, 0);
                        }
                    }
                }
            })));

        public static void SetUseAnimation(DependencyObject element, bool value) {
            element.SetValue(UseAnimationProperty, value);
        }

        public static bool GetUseAnimation(DependencyObject element) {
            return (bool)element.GetValue(UseAnimationProperty);
        }


        public static void SetIsVisible(DependencyObject element, bool? value) {
            element.SetValue(IsVisibleProperty, value);
        }

        public static bool? GetIsVisible(DependencyObject element) {
            return (bool?)element.GetValue(IsVisibleProperty);
        }
    }
}
