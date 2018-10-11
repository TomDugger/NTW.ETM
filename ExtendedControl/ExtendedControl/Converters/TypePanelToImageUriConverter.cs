using ExtendedControl.Views.Panels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace NTW.Controls.Converters {
    public class TypePanelToImageUriConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            Uri result = null;
            Type onType = (Type)value;
            if (onType == typeof(AdminControlPanel))
                return new Uri("pack://application:,,,/ExtendedControl;component/Images/ControlPanel.png");
            else if (onType == typeof(AppControlPanel))
                return new Uri("pack://application:,,,/ExtendedControl;component/Images/Settings.png");
            else if (onType == typeof(HookKeyControlPanel))
                return new Uri("pack://application:,,,/ExtendedControl;component/Images/HookKeys.png");
            else if (onType == typeof(ReportControlPanel))
                return new Uri("pack://application:,,,/ExtendedControl;component/Images/Report.png");

            else if (onType == typeof(SettingsControlPanel))
                return new Uri("pack://application:,,,/ExtendedControl;component/Images/UserSettings.png");

            else if (onType == typeof(EventsControlPanel))
                return new Uri("pack://application:,,,/ExtendedControl;component/Images/Events.png");
            else if (onType == typeof(TrackingStateControlPanel))
                return new Uri("pack://application:,,,/ExtendedControl;component/Images/Tracking.png");

            else if (onType == typeof(NotesControlPanel))
                return new Uri("pack://application:,,,/ExtendedControl;component/Images/Notes.png");
            else if (onType == typeof(TaskControlPanel))
                return new Uri("pack://application:,,,/ExtendedControl;component/Images/Task.png");
            else if (onType == typeof(ProcesControlPanel))
                return new Uri("pack://application:,,,/ExtendedControl;component/Images/Process.png");

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
