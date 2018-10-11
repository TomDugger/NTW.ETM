using NTW.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ExtendedControl.Selectors
{
    public class TaskCommentSelector: DataTemplateSelector
    {
        public DataTemplate CommonTemplate { get; set; }
        public DataTemplate SystemTemplate { get; set; }

        public DataTemplate MyCommonTemplate { get; set; }
        public DataTemplate MySystemTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            switch (((TaskComment)item).TypeCommentary + (((TaskComment)item).IDUser == App.CurrentUser.ID? 2 : 0)) {
                case 0:
                    return CommonTemplate;
                case 1:
                    return SystemTemplate;
                case 2:
                    return MyCommonTemplate;
                case 3:
                    return MySystemTemplate;
            }
            return base.SelectTemplate(item, container);
        }
    }
}
