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
    /// Логика взаимодействия для AppControlPanel.xaml
    /// </summary>
    public partial class AppControlPanel : Window
    {
        public AppControlPanel()
        {
            InitializeComponent();
        }

        public static Size StartSize { get { return new Size(500, 360); } }

        public static Size MinSize { get { return new Size(500, 360); } }
    }
}
