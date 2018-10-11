using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace NTW.Controls.Presentation
{
    public class PresentGrid : Grid
    {
        public static readonly DependencyProperty PresentContextProperty =
            DependencyProperty.Register("PresentContext", typeof(object), typeof(PresentGrid), new UIPropertyMetadata(null));

        public object PresentContext
        {
            get { return this.GetValue(PresentContextProperty); }
            set { this.SetValue(PresentContextProperty, value); }
        }
    }
}
