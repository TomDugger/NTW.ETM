using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace NTW.Data.Models
{
    public class TemporaryValue: DependencyObject
    {
        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(TemporaryValue), new PropertyMetadata(null, new PropertyChangedCallback((d, a) => {
                string h = "";
            })));
    }
}
