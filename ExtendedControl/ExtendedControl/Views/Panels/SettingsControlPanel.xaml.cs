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
using System.Windows.Shapes;

namespace ExtendedControl.Views.Panels
{
    /// <summary>
    /// Логика взаимодействия для SettingsControlPanel.xaml
    /// </summary>
    public partial class SettingsControlPanel : Window
    {
        public SettingsControlPanel()
        {
            InitializeComponent();
        }

        public static Size StartSize { get { return new Size(300, 180); } }

        public static Size MinSize { get { return new Size(300, 180); } }
    }
}
