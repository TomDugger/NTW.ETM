using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ExtendedControl.Behaviour
{
    public class ViewModelIjection
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.RegisterAttached(
            "ViewModel", typeof(Type), typeof(ViewModelIjection), new PropertyMetadata(null, new PropertyChangedCallback((d, a) =>
            {
                d.SetValue(FrameworkElement.DataContextProperty, App.GetViewModel((Type)a.NewValue));
            })));

        public static void SetViewModel(DependencyObject element, Type value)
        {
            element.SetValue(ViewModelProperty, value);
        }

        public static Type GetViewModel(DependencyObject element)
        {
            return element.GetValue(ViewModelProperty).GetType();
        }
    }
}
