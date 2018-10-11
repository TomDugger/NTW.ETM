using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace NTW.Controls{
    /// <summary>
    /// Логика взаимодействия для ColorControl.xaml
    /// </summary>
    public partial class ColorControl : UserControl, INotifyPropertyChanged{
        public ColorControl() {
            InitializeComponent();

            //this.DataContext = this;
        }

        public Color? Color {
            get { return (Color?)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color?), typeof(ColorControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback((d, a) => {
                if (a.NewValue != null)
                {
                    Color c = (Color)a.NewValue;
                    Color border = Colors.White;
                    if (c != null)
                    {
                        if (c.R + c.G + c.B > 382.5)
                            border = Colors.Black;
                        else
                            border = Colors.White;
                    }

                    ColorControl cc = (ColorControl)d;
                    cc.BorderColor = border;
                }
            })));

        private string _searchText;
        public string SearchText {
            get { return _searchText; }
            set { _searchText = value; SendPropertyChanged(nameof(Filter));  }
        }

        public Predicate<object> Filter {
            get {  return _searchText == null || _searchText == string.Empty ? null : new Predicate<object>((x) => x.ToString().ToUpper().Contains(_searchText.ToUpper())); }
        }

        public Color? BorderColor {
            get { return (Color?)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }
        
        public static readonly DependencyProperty BorderColorProperty =
            DependencyProperty.Register("BorderColor", typeof(Color?), typeof(ColorControl), new PropertyMetadata(Colors.White));

        public event PropertyChangedEventHandler PropertyChanged;

        protected void SendPropertyChanged(string properyName = "") {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(properyName));
        }
    }
}
