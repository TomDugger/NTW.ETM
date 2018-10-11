using NTW.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace NTW.Controls.Behaviours
{
    public class WindowVisibilityBehaviour
    {
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.RegisterAttached(
            "IsActive", typeof(bool), typeof(WindowVisibilityBehaviour), new PropertyMetadata(false, new PropertyChangedCallback((d, a) => {
                if (d is Window)
                {
                    Window w = (Window)d;

                    WindowPosition wp = WindowPositionBehaviour.GetWindowPosition(w);
                    if (w.Owner != null)
                        wp = WindowPositionBehaviour.GetWindowPosition(w.Owner);

                    if ((bool)a.NewValue)
                    {
                        switch (wp)
                        {
                            case Data.WindowPosition.Left:
                                w.RenderTransformOrigin = new Point(0, 0);
                                w.RenderTransform = new ScaleTransform();
                                DoubleAnimation animLeft = new DoubleAnimation(0, 1, new Duration(new TimeSpan(0, 0, 0, 0, 300)));
                                Storyboard sbLeft = new Storyboard();
                                Storyboard.SetTargetProperty(animLeft, new PropertyPath("RenderTransform.ScaleX"));
                                sbLeft.Children.Add(animLeft);
                                w.BeginStoryboard(sbLeft);
                                break;
                            case Data.WindowPosition.Right:
                                w.RenderTransformOrigin = new Point(1, 0);
                                w.RenderTransform = new ScaleTransform();
                                DoubleAnimation animRight = new DoubleAnimation(0, 1, new Duration(new TimeSpan(0, 0, 0, 0, 300)));
                                Storyboard sbRight = new Storyboard();
                                Storyboard.SetTargetProperty(animRight, new PropertyPath("RenderTransform.ScaleX"));
                                sbRight.Children.Add(animRight);
                                w.BeginStoryboard(sbRight);
                                break;
                            case Data.WindowPosition.Top:
                                w.RenderTransformOrigin = new Point(0, 0);
                                w.RenderTransform = new ScaleTransform();
                                DoubleAnimation animTop = new DoubleAnimation(0, 1, new Duration(new TimeSpan(0, 0, 0, 0, 300)));
                                Storyboard sbTop = new Storyboard();
                                Storyboard.SetTargetProperty(animTop, new PropertyPath("RenderTransform.ScaleY"));
                                sbTop.Children.Add(animTop);
                                w.BeginStoryboard(sbTop);
                                break;
                            case Data.WindowPosition.Bottom:
                                w.RenderTransformOrigin = new Point(0, 1);
                                w.RenderTransform = new ScaleTransform();
                                DoubleAnimation animBottom = new DoubleAnimation(0, 1, new Duration(new TimeSpan(0, 0, 0, 0, 300)));
                                Storyboard sbBottom = new Storyboard();
                                Storyboard.SetTargetProperty(animBottom, new PropertyPath("RenderTransform.ScaleY"));
                                sbBottom.Children.Add(animBottom);
                                w.BeginStoryboard(sbBottom);
                                break;
                            case Data.WindowPosition.None:
                            case Data.WindowPosition.Non:
                                w.RenderTransformOrigin = new Point(0.5, 0.5);
                                w.RenderTransform = new ScaleTransform();
                                DoubleAnimation animNoneF = new DoubleAnimation(0, 1, new Duration(new TimeSpan(0, 0, 0, 0, 300)));
                                DoubleAnimation animNoneS = new DoubleAnimation(0, 1, new Duration(new TimeSpan(0, 0, 0, 0, 300)));

                                Storyboard sbNone = new Storyboard();
                                Storyboard.SetTargetProperty(animNoneF, new PropertyPath("RenderTransform.ScaleX"));
                                Storyboard.SetTargetProperty(animNoneS, new PropertyPath("RenderTransform.ScaleY"));

                                sbNone.Children.Add(animNoneF);
                                sbNone.Children.Add(animNoneS);

                                w.BeginStoryboard(sbNone);
                                break;
                            case Data.WindowPosition.LeftTop:
                                w.RenderTransformOrigin = new Point(0, 0);
                                w.RenderTransform = new ScaleTransform();
                                DoubleAnimation animNoneFAlt = new DoubleAnimation(0, 1, new Duration(new TimeSpan(0, 0, 0, 0, 300)));
                                DoubleAnimation animNoneSAlt = new DoubleAnimation(0, 1, new Duration(new TimeSpan(0, 0, 0, 0, 300)));

                                Storyboard sbNoneAlt = new Storyboard();
                                Storyboard.SetTargetProperty(animNoneFAlt, new PropertyPath("RenderTransform.ScaleX"));
                                Storyboard.SetTargetProperty(animNoneSAlt, new PropertyPath("RenderTransform.ScaleY"));

                                sbNoneAlt.Children.Add(animNoneFAlt);
                                sbNoneAlt.Children.Add(animNoneSAlt);

                                w.BeginStoryboard(sbNoneAlt);
                                break;
                            case Data.WindowPosition.RightTop:
                                w.RenderTransformOrigin = new Point(1, 0);
                                w.RenderTransform = new ScaleTransform();
                                DoubleAnimation animNoneFArt = new DoubleAnimation(0, 1, new Duration(new TimeSpan(0, 0, 0, 0, 300)));
                                DoubleAnimation animNoneSArt = new DoubleAnimation(0, 1, new Duration(new TimeSpan(0, 0, 0, 0, 300)));

                                Storyboard sbNoneArt = new Storyboard();
                                Storyboard.SetTargetProperty(animNoneFArt, new PropertyPath("RenderTransform.ScaleX"));
                                Storyboard.SetTargetProperty(animNoneSArt, new PropertyPath("RenderTransform.ScaleY"));

                                sbNoneArt.Children.Add(animNoneFArt);
                                sbNoneArt.Children.Add(animNoneSArt);

                                w.BeginStoryboard(sbNoneArt);
                                break;
                            case Data.WindowPosition.RightBottom:
                                w.RenderTransformOrigin = new Point(1, 1);
                                w.RenderTransform = new ScaleTransform();
                                DoubleAnimation animNoneFArb = new DoubleAnimation(0, 1, new Duration(new TimeSpan(0, 0, 0, 0, 300)));
                                DoubleAnimation animNoneSArb = new DoubleAnimation(0, 1, new Duration(new TimeSpan(0, 0, 0, 0, 300)));

                                Storyboard sbNoneArb = new Storyboard();
                                Storyboard.SetTargetProperty(animNoneFArb, new PropertyPath("RenderTransform.ScaleX"));
                                Storyboard.SetTargetProperty(animNoneSArb, new PropertyPath("RenderTransform.ScaleY"));

                                sbNoneArb.Children.Add(animNoneFArb);
                                sbNoneArb.Children.Add(animNoneSArb);

                                w.BeginStoryboard(sbNoneArb);
                                break;
                            case Data.WindowPosition.LeftBottom:
                                w.RenderTransformOrigin = new Point(0, 1);
                                w.RenderTransform = new ScaleTransform();
                                DoubleAnimation animNoneFAlb = new DoubleAnimation(0, 1, new Duration(new TimeSpan(0, 0, 0, 0, 300)));
                                DoubleAnimation animNoneSAlb = new DoubleAnimation(0, 1, new Duration(new TimeSpan(0, 0, 0, 0, 300)));

                                Storyboard sbNoneAlb = new Storyboard();
                                Storyboard.SetTargetProperty(animNoneFAlb, new PropertyPath("RenderTransform.ScaleX"));
                                Storyboard.SetTargetProperty(animNoneSAlb, new PropertyPath("RenderTransform.ScaleY"));

                                sbNoneAlb.Children.Add(animNoneFAlb);
                                sbNoneAlb.Children.Add(animNoneSAlb);

                                w.BeginStoryboard(sbNoneAlb);
                                break;
                        }
                    }
                    else {
                        switch (wp)
                        {
                            case Data.WindowPosition.Left:
                                w.RenderTransformOrigin = new Point(0, 0);
                                w.RenderTransform = new ScaleTransform();
                                DoubleAnimation animLeft = new DoubleAnimation(1, 0, new Duration(new TimeSpan(0, 0, 0, 0, 300)));
                                Storyboard sbLeft = new Storyboard();
                                Storyboard.SetTargetProperty(animLeft, new PropertyPath("RenderTransform.ScaleX"));
                                sbLeft.Children.Add(animLeft);
                                if (GetCloseByHidden(w))
                                    sbLeft.Completed += (s, e) =>
                                    {
                                        w.Close();
                                    };
                                w.BeginStoryboard(sbLeft);
                                break;
                            case Data.WindowPosition.Right:
                                w.RenderTransformOrigin = new Point(1, 0);
                                w.RenderTransform = new ScaleTransform();
                                DoubleAnimation animRight = new DoubleAnimation(1, 0, new Duration(new TimeSpan(0, 0, 0, 0, 300)));
                                Storyboard sbRight = new Storyboard();
                                Storyboard.SetTargetProperty(animRight, new PropertyPath("RenderTransform.ScaleX"));
                                sbRight.Children.Add(animRight);
                                if (GetCloseByHidden(w))
                                    sbRight.Completed += (s, e) =>
                                    {
                                        w.Close();
                                    };
                                w.BeginStoryboard(sbRight);
                                break;
                            case Data.WindowPosition.Top:
                                w.RenderTransformOrigin = new Point(0, 0);
                                w.RenderTransform = new ScaleTransform();
                                DoubleAnimation animTop = new DoubleAnimation(1, 0, new Duration(new TimeSpan(0, 0, 0, 0, 600)));
                                Storyboard sbTop = new Storyboard();
                                Storyboard.SetTargetProperty(animTop, new PropertyPath("RenderTransform.ScaleY"));
                                sbTop.Children.Add(animTop);
                                if (GetCloseByHidden(w))
                                    sbTop.Completed += (s, e) => {
                                        w.Close();
                                    };
                                w.BeginStoryboard(sbTop);
                                break;
                            case Data.WindowPosition.Bottom:
                                w.RenderTransformOrigin = new Point(0, 1);
                                w.RenderTransform = new ScaleTransform();
                                DoubleAnimation animBottom = new DoubleAnimation(1, 0, new Duration(new TimeSpan(0, 0, 0, 0, 300)));
                                Storyboard sbBottom = new Storyboard();
                                Storyboard.SetTargetProperty(animBottom, new PropertyPath("RenderTransform.ScaleY"));
                                sbBottom.Children.Add(animBottom);
                                if (GetCloseByHidden(w))
                                    sbBottom.Completed += (s, e) =>
                                    {
                                        w.Close();
                                    };
                                w.BeginStoryboard(sbBottom);
                                break;
                            case Data.WindowPosition.None:
                            case Data.WindowPosition.Non:
                                w.RenderTransformOrigin = new Point(0.5, 0.5);
                                w.RenderTransform = new ScaleTransform();
                                DoubleAnimation animNoneF = new DoubleAnimation(1, 0, new Duration(new TimeSpan(0, 0, 0, 0, 300)));
                                DoubleAnimation animNoneS = new DoubleAnimation(1, 0, new Duration(new TimeSpan(0, 0, 0, 0, 300)));

                                Storyboard sbNone = new Storyboard();
                                Storyboard.SetTargetProperty(animNoneF, new PropertyPath("RenderTransform.ScaleX"));
                                Storyboard.SetTargetProperty(animNoneS, new PropertyPath("RenderTransform.ScaleY"));

                                sbNone.Children.Add(animNoneF);
                                sbNone.Children.Add(animNoneS);
                                if (GetCloseByHidden(w))
                                    sbNone.Completed += (s, e) =>
                                    {
                                        w.Close();
                                    };

                                w.BeginStoryboard(sbNone);
                                break;
                            case Data.WindowPosition.LeftTop:
                                w.RenderTransformOrigin = new Point(0, 0);
                                w.RenderTransform = new ScaleTransform();
                                DoubleAnimation animNoneFAlt = new DoubleAnimation(1, 0, new Duration(new TimeSpan(0, 0, 0, 0, 300)));
                                DoubleAnimation animNoneSAlt = new DoubleAnimation(1, 0, new Duration(new TimeSpan(0, 0, 0, 0, 300)));

                                Storyboard sbNoneAlt = new Storyboard();
                                Storyboard.SetTargetProperty(animNoneFAlt, new PropertyPath("RenderTransform.ScaleX"));
                                Storyboard.SetTargetProperty(animNoneSAlt, new PropertyPath("RenderTransform.ScaleY"));

                                sbNoneAlt.Children.Add(animNoneFAlt);
                                sbNoneAlt.Children.Add(animNoneSAlt);

                                if (GetCloseByHidden(w))
                                    sbNoneAlt.Completed += (s, e) =>
                                    {
                                        w.Close();
                                    };

                                w.BeginStoryboard(sbNoneAlt);
                                break;
                            case Data.WindowPosition.RightTop:
                                w.RenderTransformOrigin = new Point(1, 0);
                                w.RenderTransform = new ScaleTransform();
                                DoubleAnimation animNoneFA = new DoubleAnimation(1, 0, new Duration(new TimeSpan(0, 0, 0, 0, 600)));
                                DoubleAnimation animNoneSA = new DoubleAnimation(1, 0, new Duration(new TimeSpan(0, 0, 0, 0, 600)));

                                Storyboard sbNoneA = new Storyboard();
                                Storyboard.SetTargetProperty(animNoneFA, new PropertyPath("RenderTransform.ScaleX"));
                                Storyboard.SetTargetProperty(animNoneSA, new PropertyPath("RenderTransform.ScaleY"));

                                sbNoneA.Children.Add(animNoneFA);
                                sbNoneA.Children.Add(animNoneSA);

                                if (GetCloseByHidden(w))
                                    sbNoneA.Completed += (s, e) =>
                                    {
                                        w.Close();
                                    };

                                w.BeginStoryboard(sbNoneA);
                                break;
                            case Data.WindowPosition.RightBottom:
                                w.RenderTransformOrigin = new Point(1, 1);
                                w.RenderTransform = new ScaleTransform();
                                DoubleAnimation animNoneFArb = new DoubleAnimation(1, 0, new Duration(new TimeSpan(0, 0, 0, 0, 300)));
                                DoubleAnimation animNoneSArb = new DoubleAnimation(1, 0, new Duration(new TimeSpan(0, 0, 0, 0, 300)));

                                Storyboard sbNoneArb = new Storyboard();
                                Storyboard.SetTargetProperty(animNoneFArb, new PropertyPath("RenderTransform.ScaleX"));
                                Storyboard.SetTargetProperty(animNoneSArb, new PropertyPath("RenderTransform.ScaleY"));

                                sbNoneArb.Children.Add(animNoneFArb);
                                sbNoneArb.Children.Add(animNoneSArb);

                                if (GetCloseByHidden(w))
                                    sbNoneArb.Completed += (s, e) =>
                                    {
                                        w.Close();
                                    };

                                w.BeginStoryboard(sbNoneArb);
                                break;
                            case Data.WindowPosition.LeftBottom:
                                w.RenderTransformOrigin = new Point(0, 1);
                                w.RenderTransform = new ScaleTransform();
                                DoubleAnimation animNoneFAlb = new DoubleAnimation(1, 0, new Duration(new TimeSpan(0, 0, 0, 0, 300)));
                                DoubleAnimation animNoneSAlb = new DoubleAnimation(1, 0, new Duration(new TimeSpan(0, 0, 0, 0, 300)));

                                Storyboard sbNoneAlb = new Storyboard();
                                Storyboard.SetTargetProperty(animNoneFAlb, new PropertyPath("RenderTransform.ScaleX"));
                                Storyboard.SetTargetProperty(animNoneSAlb, new PropertyPath("RenderTransform.ScaleY"));

                                sbNoneAlb.Children.Add(animNoneFAlb);
                                sbNoneAlb.Children.Add(animNoneSAlb);

                                if (GetCloseByHidden(w))
                                    sbNoneAlb.Completed += (s, e) =>
                                    {
                                        w.Close();
                                    };

                                w.BeginStoryboard(sbNoneAlb);
                                break;
                        }
                    }
                }
            })));

        public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.RegisterAttached(
            "IsVisible", typeof(bool), typeof(WindowVisibilityBehaviour), new PropertyMetadata(false, new PropertyChangedCallback((d, a) => {
                if (d is Window) {

                    Window w = (Window)d;

                    if ((bool)a.NewValue) {
                        SetIsActive(w, true);
                        w.Show();
                    }
                    else {
                        SetIsActive(w, false);
                    }

                }
            })));

        public static readonly DependencyProperty IsDialogVisibleProperty = DependencyProperty.RegisterAttached(
            "IsDialogVisible", typeof(bool), typeof(WindowVisibilityBehaviour), new PropertyMetadata(false, new PropertyChangedCallback((d, a) => {
                if (d is Window)
                {

                    Window w = (Window)d;

                    if ((bool)a.NewValue)
                    {
                        SetIsActive(w, true);
                        w.ShowDialog();
                    }
                    else
                    {
                        SetIsActive(w, false);
                    }

                }
            })));

        public static readonly DependencyProperty CloseByHiddenProperty = DependencyProperty.RegisterAttached(
            "CloseByHidden", typeof(bool), typeof(WindowVisibilityBehaviour), new PropertyMetadata(false));


        public static void SetIsActive(DependencyObject element, bool value)
        {
            element.SetValue(IsActiveProperty, value);
        }

        public static bool GetIsActive(DependencyObject element) {
            return (bool)element.GetValue(IsActiveProperty);
        }


        public static void SetIsVisible(DependencyObject element, bool value) {
            element.SetValue(IsVisibleProperty, value);
        }

        public static bool GetIsVisible(DependencyObject element) {
            return (bool)element.GetValue(IsVisibleProperty);
        }


        public static void SetIsDialogVisible(DependencyObject element, bool value) {
            element.SetValue(IsDialogVisibleProperty, value);
        }

        public static bool GetIsDialogVisible(DependencyObject element) {
            return (bool)element.GetValue(IsDialogVisibleProperty);
        }


        public static void SetCloseByHidden(DependencyObject element, bool value) {
            element.SetValue(CloseByHiddenProperty, value);
        }

        public static bool GetCloseByHidden(DependencyObject element) {
            return (bool)element.GetValue(CloseByHiddenProperty);
        }
    }
}
