﻿using System;
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
    /// Логика взаимодействия для NotesControlPanel.xaml
    /// </summary>
    public partial class NotesControlPanel : Window
    {
        public NotesControlPanel()
        {
            InitializeComponent();
        }

        public static Size StartSize { get { return new Size(300, 180); } }

        public static Size MinSize { get { return new Size(300, 180); } }
    }
}
