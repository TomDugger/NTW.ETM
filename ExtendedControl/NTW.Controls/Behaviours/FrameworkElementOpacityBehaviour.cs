using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Animation;

namespace NTW.Controls.Behaviours
{
    public class FrameworkElementOpacityBehaviour
    {
        public static readonly DependencyProperty IsShowProperty =
            DependencyProperty.RegisterAttached("IsShow", typeof(bool), typeof(FrameworkElementOpacityBehaviour), new PropertyMetadata(true, new PropertyChangedCallback((d, a) => {
                if(d is FrameworkElement) {
                    FrameworkElement fe = (FrameworkElement)d;
                    bool isAnimated = GetIsAnimated(fe);
                    if ((bool)a.NewValue)
                    {
                        if (isAnimated)
                        {
                            TimeSpan line = GetTimeTick(fe);
                            DoubleAnimation animLeft = new DoubleAnimation(0, 1, new Duration(line));
                            Storyboard sbLeft = new Storyboard();
                            Storyboard.SetTargetProperty(animLeft, new PropertyPath("Opacity"));
                            sbLeft.Children.Add(animLeft);
                            fe.BeginStoryboard(sbLeft);
                        }
                        else
                            fe.Opacity = 1;
                    }
                    else {
                        if (isAnimated)
                        {
                            TimeSpan line = GetTimeTick(fe);
                            DoubleAnimation animLeft = new DoubleAnimation(1, 0, new Duration(line));
                            Storyboard sbLeft = new Storyboard();
                            Storyboard.SetTargetProperty(animLeft, new PropertyPath("Opacity"));
                            sbLeft.Children.Add(animLeft);
                            fe.BeginStoryboard(sbLeft);
                        }
                        else
                            fe.Opacity = 0;
                    }
                }

            })));

        public static bool GetIsShow(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsShowProperty);
        }

        public static void SetIsShow(DependencyObject obj, bool value)
        {
            obj.SetValue(IsShowProperty, value);
        }


        public static readonly DependencyProperty IsAnimatedProperty =
            DependencyProperty.RegisterAttached("IsAnimated", typeof(bool), typeof(FrameworkElementOpacityBehaviour), new PropertyMetadata(false));

        public static bool GetIsAnimated(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsAnimatedProperty);
        }

        public static void SetIsAnimated(DependencyObject obj, bool value)
        {
            obj.SetValue(IsAnimatedProperty, value);
        }


        public static readonly DependencyProperty TimeTickProperty =
            DependencyProperty.RegisterAttached("TimeTick", typeof(TimeSpan), typeof(FrameworkElementOpacityBehaviour), new PropertyMetadata(new TimeSpan(0, 0, 0, 0, 200)));

        public static TimeSpan GetTimeTick(DependencyObject obj)
        {
            return (TimeSpan)obj.GetValue(TimeTickProperty);
        }

        public static void SetTimeTick(DependencyObject obj, TimeSpan value)
        {
            obj.SetValue(TimeTickProperty, value);
        }
    }
}
