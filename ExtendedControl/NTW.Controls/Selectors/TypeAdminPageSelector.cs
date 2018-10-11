using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace NTW.Controls.Selectors
{
    public class TypeAdminPageSelector: DataTemplateSelector
    {
        public DataTemplate TasksTemplate { get; set; }
        public DataTemplate UsersTemplate { get; set; }
        public DataTemplate RuleTemplate { get; set; }
        public DataTemplate UsersSettingsTemplate { get; set; }
        public DataTemplate ProjectsTemplate { get; set; }
        public DataTemplate JournslTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if(item != null)
                switch ((int)item)
                {
                    case 1:
                        return TasksTemplate;
                    case 2:
                        return UsersTemplate;
                    case 3:
                        return RuleTemplate;
                    case 4:
                        return UsersSettingsTemplate;
                    case 5:
                        return ProjectsTemplate;
                    case 6:
                        return JournslTemplate;
                    default:
                        return base.SelectTemplate(item, container);
                }
            else
                return base.SelectTemplate(item, container);

        }
    }
}
