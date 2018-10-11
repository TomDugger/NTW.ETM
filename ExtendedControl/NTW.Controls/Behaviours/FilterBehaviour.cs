﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace NTW.Controls.Behaviours {
    public class FilterBehaviour {

        public static readonly DependencyProperty ByProperty = DependencyProperty.RegisterAttached(
               "By", typeof(Predicate<object>), typeof(FilterBehaviour), new PropertyMetadata(default(Predicate<object>), OnWithChanged));

        public static void SetBy(ItemsControl element, Predicate<object> value) {
            element.SetValue(ByProperty, value);
        }

        public static Predicate<object> GetBy(ItemsControl element) {
            return (Predicate<object>)element.GetValue(ByProperty);
        }

        private static void OnWithChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            UpdateFilter((ItemsControl)d, (Predicate<object>)e.NewValue);
        }

        private static void UpdateFilter(ItemsControl itemsControl, Predicate<object> filter) {
            if (itemsControl.Items.CanFilter) {
                itemsControl.Items.Filter = filter;
            }
        }
    }
}
