using NTW.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace NTW.Controls.Behaviours
{
    public class ItemsControlExtantionalBehaviour {
        public static readonly DependencyProperty ParentPositionProperty = DependencyProperty.RegisterAttached(
            "ParentPosition", typeof(WindowPosition), typeof(ItemsControlExtantionalBehaviour), new PropertyMetadata(WindowPosition.Non, (d, a) => {
                WindowPosition wp = (WindowPosition)a.NewValue;
                bool state = GetState(d);

                SetParametry(wp, state, d);

            }));

        public static readonly DependencyProperty StateProperty = DependencyProperty.RegisterAttached(
            "State", typeof(bool), typeof(ItemsControlExtantionalBehaviour), new PropertyMetadata((d, a) => {
                WindowPosition wp = GetParentPosition(d);
                bool state = (bool)a.NewValue;

                SetParametry(wp, state, d);

            }));

        public static readonly DependencyProperty ItemsPanelProperty = DependencyProperty.RegisterAttached(
            "ItemsPanel", typeof(ItemsPanelTemplate), typeof(ItemsControlExtantionalBehaviour));

        public static readonly DependencyProperty FlowProperty = DependencyProperty.RegisterAttached(
            "Flow", typeof(FlowDirection), typeof(ItemsControlExtantionalBehaviour));

        public static readonly DependencyProperty VScrollBarVisibilityProperty = DependencyProperty.RegisterAttached(
            "VScrollBarVisibility", typeof(ScrollBarVisibility), typeof(ItemsControlExtantionalBehaviour), new FrameworkPropertyMetadata(ScrollBarVisibility.Auto));

        public static readonly DependencyProperty HScrollBarVisibilityProperty = DependencyProperty.RegisterAttached(
            "HScrollBarVisibility", typeof(ScrollBarVisibility), typeof(ItemsControlExtantionalBehaviour), new FrameworkPropertyMetadata(ScrollBarVisibility.Auto));


        public static void SetParametry(WindowPosition wp, bool state, DependencyObject element) {
            FrameworkElementFactory root;

            if (!state) {
                root = new FrameworkElementFactory(typeof(StackPanel));

                switch (wp) {

                    case WindowPosition.Left:
                    case WindowPosition.LeftTop:
                    case WindowPosition.LeftBottom:
                        root.SetValue(StackPanel.OrientationProperty, Orientation.Vertical);
                        SetFlow(element, FlowDirection.RightToLeft);
                        SetVScrollBarVisibility(element, ScrollBarVisibility.Auto);
                        SetHScrollBarVisibility(element, ScrollBarVisibility.Disabled);
                        break;
                    case WindowPosition.Right:
                    case WindowPosition.RightTop:
                    case WindowPosition.RightBottom:
                        root.SetValue(StackPanel.OrientationProperty, Orientation.Vertical);
                        SetFlow(element, FlowDirection.LeftToRight);
                        SetVScrollBarVisibility(element, ScrollBarVisibility.Auto);
                        SetHScrollBarVisibility(element, ScrollBarVisibility.Disabled);
                        break;
                    case WindowPosition.Top:
                        root.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
                        SetFlow(element, FlowDirection.LeftToRight);
                        SetVScrollBarVisibility(element, ScrollBarVisibility.Disabled);
                        SetHScrollBarVisibility(element, ScrollBarVisibility.Auto);
                        break;
                    case WindowPosition.Bottom:
                        root.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
                        SetFlow(element, FlowDirection.LeftToRight);
                        SetVScrollBarVisibility(element, ScrollBarVisibility.Disabled);
                        SetHScrollBarVisibility(element, ScrollBarVisibility.Auto);
                        break;
                    case WindowPosition.None:
                        root = new FrameworkElementFactory(typeof(WrapPanel));
                        root.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
                        SetFlow(element, FlowDirection.LeftToRight);
                        SetVScrollBarVisibility(element, ScrollBarVisibility.Auto);
                        SetHScrollBarVisibility(element, ScrollBarVisibility.Disabled);
                        break;
                }
            }
            else {
                root = new FrameworkElementFactory(typeof(WrapPanel));

                switch (wp) {
                    case WindowPosition.Left:
                    case WindowPosition.LeftTop:
                    case WindowPosition.LeftBottom:
                        root.SetValue(WrapPanel.OrientationProperty, Orientation.Vertical);
                        SetFlow(element, FlowDirection.LeftToRight);
                        SetVScrollBarVisibility(element, ScrollBarVisibility.Disabled);
                        SetHScrollBarVisibility(element, ScrollBarVisibility.Auto);
                        break;
                    case WindowPosition.Right:
                    case WindowPosition.RightTop:
                    case WindowPosition.RightBottom:
                        root.SetValue(WrapPanel.OrientationProperty, Orientation.Vertical);
                        SetFlow(element, FlowDirection.RightToLeft);
                        SetVScrollBarVisibility(element, ScrollBarVisibility.Disabled);
                        SetHScrollBarVisibility(element, ScrollBarVisibility.Auto);
                        break;
                    case WindowPosition.Top:
                        root.SetValue(WrapPanel.OrientationProperty, Orientation.Horizontal);
                        SetFlow(element, FlowDirection.LeftToRight);
                        SetVScrollBarVisibility(element, ScrollBarVisibility.Auto);
                        SetHScrollBarVisibility(element, ScrollBarVisibility.Disabled);
                        break;
                    case WindowPosition.Bottom:
                        root.SetValue(WrapPanel.OrientationProperty, Orientation.Horizontal);
                        SetFlow(element, FlowDirection.LeftToRight);
                        SetVScrollBarVisibility(element, ScrollBarVisibility.Auto);
                        SetHScrollBarVisibility(element, ScrollBarVisibility.Disabled);
                        break;
                    case WindowPosition.None:
                        root.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
                        SetFlow(element, FlowDirection.LeftToRight);
                        SetVScrollBarVisibility(element, ScrollBarVisibility.Auto);
                        SetHScrollBarVisibility(element, ScrollBarVisibility.Disabled);
                        break;
                }
            }

            SetItemsPanel(element, new ItemsPanelTemplate(root));
        }


        public static void SetParentPosition(DependencyObject element, WindowPosition value) {
            element.SetValue(ParentPositionProperty, value);
        }

        public static WindowPosition GetParentPosition(DependencyObject element) {
            return (WindowPosition)element.GetValue(ParentPositionProperty);
        }


        public static void SetState(DependencyObject element, bool value) {
            element.SetValue(StateProperty, value);
        }

        public static bool GetState(DependencyObject element) {
            return (bool)element.GetValue(StateProperty);
        }


        protected static void SetItemsPanel(DependencyObject element, ItemsPanelTemplate value) {
            element.SetValue(ItemsPanelProperty, value);
        }

        public static ItemsPanelTemplate GetItemsPanel(DependencyObject element) {
            return (ItemsPanelTemplate)element.GetValue(ItemsPanelProperty);
        }


        protected static void SetFlow(DependencyObject element, FlowDirection value) {
            element.SetValue(FlowProperty, value);
        }

        public static FlowDirection GetFlow(DependencyObject element) {
            return (FlowDirection)element.GetValue(FlowProperty);
        }


        public static void SetVScrollBarVisibility(DependencyObject element, ScrollBarVisibility value) {
            element.SetValue(VScrollBarVisibilityProperty, value);
        }

        public static ScrollBarVisibility GetVScrollBarVisibility(DependencyObject element) {
            return (ScrollBarVisibility)element.GetValue(VScrollBarVisibilityProperty);
        }


        public static void SetHScrollBarVisibility(DependencyObject element, ScrollBarVisibility value) {
            element.SetValue(HScrollBarVisibilityProperty, value);
        }

        public static ScrollBarVisibility GetHScrollBarVisibility(DependencyObject element) {
            return (ScrollBarVisibility)element.GetValue(HScrollBarVisibilityProperty);
        }

        //extended listbox
        public static bool GetUseUnselection(DependencyObject obj) {
            return (bool)obj.GetValue(UseUnselectionProperty);
        }

        public static void SetUseUnselection(DependencyObject obj, bool value) {
            obj.SetValue(UseUnselectionProperty, value);
        }

        public static readonly DependencyProperty UseUnselectionProperty =
            DependencyProperty.RegisterAttached("UseUnselection", typeof(bool), typeof(ItemsControlExtantionalBehaviour), new PropertyMetadata(false, new PropertyChangedCallback((d, a) => {
                if (d is ListBoxItem) {
                    var lb = (ListBoxItem)d;
                    if ((bool)a.NewValue) {
                        lb.PreviewMouseLeftButtonDown += Lb_PreviewMouseLeftButtonDown;
                    }
                    else
                        lb.PreviewMouseLeftButtonDown -= Lb_PreviewMouseLeftButtonDown;
                }
                })));

        private static void Lb_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            var orSource = (DependencyObject)e.OriginalSource;
            if (GetOnActiveUnselection(orSource)) {
                var lbi = (ListBoxItem)sender;
                if (lbi.IsSelected)
                    lbi.IsSelected = false;
            }
        }



        public static bool GetUseUnselectionNon(DependencyObject obj) {
            return (bool)obj.GetValue(UseUnselectionNonProperty);
        }

        public static void SetUseUnselectionNon(DependencyObject obj, bool value) {
            obj.SetValue(UseUnselectionNonProperty, value);
        }

        public static readonly DependencyProperty UseUnselectionNonProperty =
            DependencyProperty.RegisterAttached("UseUnselectionNon", typeof(bool), typeof(ItemsControlExtantionalBehaviour), new PropertyMetadata(false, new PropertyChangedCallback((d, a) => {
                if (d is ListBoxItem) {
                    var lb = (ListBoxItem)d;
                    if ((bool)a.NewValue) {
                        lb.MouseLeftButtonDown += Lb_MouseLeftButtonDown;
                    }
                    else
                        lb.MouseLeftButtonDown -= Lb_MouseLeftButtonDown;
                }
            })));

        private static void Lb_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            var lbi = (ListBoxItem)sender;
            if (lbi.IsSelected)
                lbi.IsSelected = false;
        }


        public static bool GetOnActiveUnselection(DependencyObject obj) {
            return (bool)obj.GetValue(OnActiveUnselectionProperty);
        }

        public static void SetOnActiveUnselection(DependencyObject obj, bool value) {
            obj.SetValue(OnActiveUnselectionProperty, value);
        }

        // Using a DependencyProperty as the backing store for OnActiveUnselection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OnActiveUnselectionProperty =
            DependencyProperty.RegisterAttached("OnActiveUnselection", typeof(bool), typeof(ItemsControlExtantionalBehaviour), new PropertyMetadata(false));

    }
}
