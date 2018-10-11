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

namespace NTW.Controls
{
    /// <summary>
    /// Логика взаимодействия для SearchBox.xaml
    /// </summary>
    public partial class SearchBox : UserControl
    {
        public SearchBox()
        {
            InitializeComponent();
        }

        public string SearchText
        {
            get { return (string)GetValue(SearchTextProperty); }
            set { SetValue(SearchTextProperty, value); }
        }

        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register("SearchText", typeof(string), typeof(SearchBox), new PropertyMetadata(null));



        public bool IsVisibleTextBox {
            get { return (bool)GetValue(IsVisibleTextBoxProperty); }
            set { SetValue(IsVisibleTextBoxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsVisibleTextBox.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsVisibleTextBoxProperty =
            DependencyProperty.Register("IsVisibleTextBox", typeof(bool), typeof(SearchBox), new PropertyMetadata(false));


        private void Button_Click(object sender, RoutedEventArgs e) {
            SearchText = string.Empty;
        }
    }
}
