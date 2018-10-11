using NTW.Controls.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ExtendedControl.Views
{
    /// <summary>
    /// Логика взаимодействия для StartUpWindow.xaml
    /// </summary>
    public partial class StartUpWindow : Window
    {
        public StartUpWindow()
        {
            InitializeComponent();
            this.Loaded += StartUpWindow_Loaded;
        }

        private void StartUpWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if(OnCloseAction != null)
                Task.Factory.StartNew(() => {
                    OnCloseAction();
                    App.BeginInvoke(() =>
                    {
                        WindowVisibilityBehaviour.SetIsDialogVisible(this, false);
                    });
                });
            else
                WindowVisibilityBehaviour.SetIsDialogVisible(this, false);
        }

        public Action OnCloseAction { get; set; }
    }
}
