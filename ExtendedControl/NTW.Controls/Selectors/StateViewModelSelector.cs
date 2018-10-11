using NTW.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace NTW.Controls.Selectors {
    public class StateViewModelSelector: DataTemplateSelector {

        public DataTemplate CreateTemplate { get; set; }
        public DataTemplate EditTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container) {
            if (item != null)
                switch ((TypeControl)item) {
                    case TypeControl.Create:
                        return CreateTemplate;
                    case TypeControl.Edit:
                        return EditTemplate;
                    default:
                        return null;
                }
            else return base.SelectTemplate(item, container);
        }
    }
}
