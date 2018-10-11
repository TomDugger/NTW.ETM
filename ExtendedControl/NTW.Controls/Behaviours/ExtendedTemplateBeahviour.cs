using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace NTW.Controls.Behaviours {
    public class ExtendedTemplateBeahviour {


        public static DataTemplate GetTemplate(DependencyObject obj) {
            return (DataTemplate)obj.GetValue(TemplateProperty);
        }

        public static void SetTemplate(DependencyObject obj, DataTemplate value) {
            obj.SetValue(TemplateProperty, value);
        }

        // Using a DependencyProperty as the backing store for Template.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.RegisterAttached("Template", typeof(DataTemplate), typeof(ExtendedTemplateBeahviour), new PropertyMetadata(null));


    }
}
