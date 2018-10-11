﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace NTW.Controls.Behaviours
{
    public class ComboBoxSelectionBoxAltTemplateBehaviour
    {
        public static readonly DependencyProperty SelectionBoxAltTemplateProperty = DependencyProperty.RegisterAttached(
            "SelectionBoxAltTemplate", typeof(DataTemplate), typeof(ComboBoxSelectionBoxAltTemplateBehaviour), new PropertyMetadata(default(DataTemplate)));

        public static void SetSelectionBoxAltTemplate(DependencyObject element, DataTemplate value)
        {
            element.SetValue(SelectionBoxAltTemplateProperty, value);
        }

        public static DataTemplate GetSelectionBoxAltTemplate(DependencyObject element)
        {
            return (DataTemplate)element.GetValue(SelectionBoxAltTemplateProperty);
        }

    }
}
