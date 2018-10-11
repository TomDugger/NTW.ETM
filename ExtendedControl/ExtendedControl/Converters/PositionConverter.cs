using NTW.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace ExtendedControl.Converters
{
    public class PositionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((WindowPosition)value) {
                case WindowPosition.Left:
                    return App.GetString("ConteolPanelPositionLeft");
                case WindowPosition.Right:
                    return App.GetString("ConteolPanelPositionRight");
                case WindowPosition.Top:
                    return App.GetString("ConteolPanelPositionTop");
                case WindowPosition.Bottom:
                    return App.GetString("ConteolPanelPositionBottom");
                case WindowPosition.LeftTop:
                    return App.GetString("ConteolPanelPositionLeftTop");
                case WindowPosition.RightTop:
                    return App.GetString("ConteolPanelPositionRightTop");
                case WindowPosition.LeftBottom:
                    return App.GetString("ConteolPanelPositionLeftBottom");
                case WindowPosition.RightBottom:
                    return App.GetString("ConteolPanelPositionRightBottom");
                default: return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
