using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NTW.Controls {
    /// <summary>
    /// Логика взаимодействия для TimeControl.xaml
    /// </summary>
    public partial class TimeControl : UserControl {
        public TimeControl() {
            InitializeComponent();
        }

        public TimeSpan Value {
            get { return (TimeSpan)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register("Value", typeof(TimeSpan), typeof(TimeControl),
        new UIPropertyMetadata(DateTime.Now.TimeOfDay, new PropertyChangedCallback(OnValueChanged)));

        private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
            TimeControl control = obj as TimeControl;
            control.Hours = ((TimeSpan)e.NewValue).Hours;
            control.Minutes = ((TimeSpan)e.NewValue).Minutes;
            control.Seconds = ((TimeSpan)e.NewValue).Seconds;
        }

        public int Hours {
            get { return (int)GetValue(HoursProperty); }
            set { if (value > 23) value = 23; else if (value < 0) value = 0; SetValue(HoursProperty, value); }
        }
        public static readonly DependencyProperty HoursProperty =
        DependencyProperty.Register("Hours", typeof(int), typeof(TimeControl),
        new UIPropertyMetadata(0, new PropertyChangedCallback(OnTimeChanged)));

        public int Minutes {
            get { return (int)GetValue(MinutesProperty); }
            set { if (value > 59) value = 59; else if (value < 0) value = 0; SetValue(MinutesProperty, value); }
        }
        public static readonly DependencyProperty MinutesProperty =
        DependencyProperty.Register("Minutes", typeof(int), typeof(TimeControl),
        new UIPropertyMetadata(0, new PropertyChangedCallback(OnTimeChanged)));

        public int Seconds {
            get { return (int)GetValue(SecondsProperty); }
            set { if (value > 59) value = 59; else if (value < 0) value = 0; SetValue(SecondsProperty, value); }
        }

        public static readonly DependencyProperty SecondsProperty =
        DependencyProperty.Register("Seconds", typeof(int), typeof(TimeControl),
        new UIPropertyMetadata(0, new PropertyChangedCallback(OnTimeChanged)));


        private static void OnTimeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
            TimeControl control = obj as TimeControl;
            control.Value = new TimeSpan(control.Hours, control.Minutes, control.Seconds);
        }

        private void Down(object sender, KeyEventArgs args) {
            switch (((TextBox)sender).Name) {
                case "sec":
                    if (args.Key == Key.Up)
                        if (this.Seconds+1 > 59)
                            this.Seconds = 0;
                        else
                            this.Seconds++;
                    else if (args.Key == Key.Down)
                        if (this.Seconds-1 < 0)
                            this.Seconds = 59;
                        else
                            this.Seconds--;
                    else if (args.Key == Key.Right)
                        hour.Focus();
                    else if (args.Key == Key.Left)
                        min.Focus();
                    break;

                case "min":
                    if (args.Key == Key.Up)
                        if (this.Minutes+1 > 59)
                            this.Minutes = 0;
                        else
                            this.Minutes++;
                    else if (args.Key == Key.Down)
                        if (this.Minutes-1 < 0)
                            this.Minutes = 59;
                        else
                            this.Minutes--;
                    else if (args.Key == Key.Right)
                        sec.Focus();
                    else if (args.Key == Key.Left)
                        hour.Focus();
                    break;

                case "hour":
                    if (args.Key == Key.Up)
                        if (this.Hours+1 > 23)
                            this.Hours = 0;
                        else
                            this.Hours++;
                    else if (args.Key == Key.Down)
                        if (this.Hours-1 < 0)
                            this.Hours = 23;
                        else
                            this.Hours--;
                    else if (args.Key == Key.Right)
                        min.Focus();
                    else if (args.Key == Key.Left)
                        sec.Focus();
                    break;
            }
        }
    }
}
