using NTW.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ExtendedControl.Selectors
{
    public class EventItemSelector: DataTemplateSelector
    {
        public DataTemplate TextMessage { get; set; }

        public DataTemplate EnterTextMessage { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is EventItem eventItem)
            {
                if(eventItem.Type == TypeMessage.NonMessage)
                    return base.SelectTemplate(item, container);
                else if (eventItem.Type == TypeMessage.AddComment)
                    return EnterTextMessage;
                else
                    return TextMessage;
            }
            else return base.SelectTemplate(item, container);
        }
    }
}
