using NTW.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NTW.Controls
{
    /// <summary>
    /// Логика взаимодействия для EditableTextBox.xaml
    /// </summary>
    public partial class EditableTextBox : UserControl
    {
        public EditableTextBox()
        {
            InitializeComponent();

            EditablePanel.Focus();
            Keyboard.Focus(EditablePanel);
            Mouse.Capture(EditablePanel);

        }

        #region Collections
        public IEnumerable<FontFamily> FontsCollection
        {
            get { return Fonts.SystemFontFamilies.OrderBy(f => f.Source); }
        }

        public IEnumerable<double> SizeCollection
        {
            get { return new double[] { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 }; }
        }

        public IEnumerable<string> PageFormats
        {
            get { return new string[] { "A4" }; }
        }

        public Array MarkerStyle
        {
            get { return Enum.GetValues(typeof(TextMarkerStyle)); }
        }
        #endregion

        #region Manipulation
        public bool VisibilityPanel {
            get { return EditPanel.Height != 0; }
            set {
                if (value)
                    EditPanel.Height = double.NaN;
                else
                    EditPanel.Height = 0;
            }
        }

        public bool ControlFilePanelVisible {
            get { return ControlFilePanel.Width == 0 && ControlFilePanel.Height == 0; }
            set { ControlFilePanel.Width = ControlFilePanel.Height = value ? double.NaN : 0; }
        }

        public bool PageSizePanelVisible {
            get { return PageSizePanel.Width == 0 && PageSizePanel.Height == 0; }
            set { PageSizePanel.Width = PageSizePanel.Height = value ? double.NaN : 0; }
        }

        public bool AligenmentTextPanelVisible {
            get { return AligenmentTextPanel.Width == 0 && AligenmentTextPanel.Height == 0; }
            set { AligenmentTextPanel.Width = AligenmentTextPanel.Height = value ? double.NaN : 0; }
        }

        public bool FontTextPanelVisible {
            get { return FontTextPanel.Width == 0 && FontTextPanel.Height == 0; }
            set { FontTextPanel.Width = FontTextPanel.Height = value ? double.NaN : 0; }
        }

        public bool PrimitivyObjectPanelVisible {
            get { return PrimitivyObjectPanel.Width == 0 && PrimitivyObjectPanel.Height == 0; }
            set { PrimitivyObjectPanel.Width = PrimitivyObjectPanel.Height = value ? double.NaN : 0; }
        }

        public bool ObjectControlPanelVisible {
            get { return ObjectControlPanel.Width == 0 && ObjectControlPanel.Height == 0; }
            set { ObjectControlPanel.Width = ObjectControlPanel.Height = value ? double.NaN : 0; }
        }

        public bool MarkerPanelVisible {
            get { return MarkerPanel.Width == 0 && MarkerPanel.Height == 0; }
            set { MarkerPanel.Width = MarkerPanel.Height = value ? double.NaN : 0; }
        }

        public double PageWidth {
            get { return EditablePanel.Document.PageWidth; }
            set { EditablePanel.Document.PageWidth = value; }
        }

        public double PageHeight {
            get { return EditablePanel.Document.PageHeight; }
            set { EditablePanel.Document.PageHeight = value; }
        }
        #endregion

        #region Indication
        private static readonly DependencyProperty CountColumnsTableProperty = DependencyProperty.Register(
            "CountColumnsTable", typeof(int), typeof(EditableTextBox), new PropertyMetadata(2));
        public int CountColumnsTable
        {
            get { return (int)this.GetValue(CountColumnsTableProperty); }
            set { this.SetValue(CountColumnsTableProperty, value); }
        }

        private static readonly DependencyProperty TypeMarkerTextProperty = DependencyProperty.Register(
            "TypeMarkerText", typeof(TextMarkerStyle), typeof(EditableTextBox), new PropertyMetadata(TextMarkerStyle.Decimal));
        public TextMarkerStyle TypeMarkerText
        {
            get { return (TextMarkerStyle)this.GetValue(TypeMarkerTextProperty); }
            set { this.SetValue(TypeMarkerTextProperty, value); }
        }

        private static readonly DependencyProperty StringDocumentProperty = DependencyProperty.Register(
            "StringDocument", typeof(string), typeof(EditableTextBox));
        public string StringDocument
        {
            get { return (string)this.GetValue(StringDocumentProperty); }
            set { this.SetValue(StringDocumentProperty, value); }
        }

        private static readonly DependencyProperty DocumentProperty = DependencyProperty.Register(
            "Document", typeof(FlowDocument), typeof(EditableTextBox), new PropertyMetadata((d, a) => {
                if (d is EditableTextBox)
                {
                    EditableTextBox etb = (EditableTextBox)d;
                    etb.EditablePanel.Document = (FlowDocument)a.NewValue;
                }
            }));
        public FlowDocument Document
        {
            get { return (FlowDocument)this.GetValue(DocumentProperty); }
            set { this.SetValue(DocumentProperty, value); }
        }
        #endregion

        private void EditablePanel_SelectionChanged(object sender, RoutedEventArgs e)
        {
            object temp = EditablePanel.Selection.GetPropertyValue(Inline.FontWeightProperty);
            btnBold.IsChecked = temp != null && (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontWeights.Bold));
            temp = EditablePanel.Selection.GetPropertyValue(Inline.FontStyleProperty);
            btnItalic.IsChecked = temp != null && (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontStyles.Italic));
            temp = EditablePanel.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            btnUnderline.IsChecked = temp != null && (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextDecorations.Underline));

            temp = EditablePanel.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            cmbFontFamily.SelectedItem = (FontFamily)temp;
            temp = EditablePanel.Selection.GetPropertyValue(Inline.FontSizeProperty);
            if (temp != null && temp != DependencyProperty.UnsetValue)
                cmbFontSize.SelectedValue = (double)temp;

            temp = EditablePanel.Selection.GetPropertyValue(TextBlock.TextAlignmentProperty);
            btnAlignLeft.IsChecked = temp != null && (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextAlignment.Left));
            btnAlignCenter.IsChecked = temp != null && (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextAlignment.Center));
            btnAlignRight.IsChecked = temp != null && (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextAlignment.Right));
            btnAlignJustify.IsChecked = temp != null && (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextAlignment.Justify));
        }

        private void cmbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbFontFamily.SelectedItem != null)
                EditablePanel.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, cmbFontFamily.SelectedItem);
        }

        private void cmbFontSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                EditablePanel.Selection.ApplyPropertyValue(Inline.FontSizeProperty, cmbFontSize.Text);
            }
            catch { }
        }

        private void CreateTable(object sender, RoutedEventArgs e)
        {
            EditablePanel.Document.Blocks.Add(new Paragraph());
            Table t = new Table();
            t.BorderBrush = new SolidColorBrush(Colors.Black);
            t.BorderThickness = new Thickness(1);
            for (int i = 0; i < CountColumnsTable; i++)
            {
                t.Columns.Add(new TableColumn());
            }
            
            TableRow row = new TableRow();
            for (int j = 0; j < CountColumnsTable; j++)
            {
                row.Cells.Add(new TableCell(new Paragraph(new Run("Head " + j.ToString()))) { BorderBrush = new SolidColorBrush(Colors.Black), BorderThickness = new Thickness(1), Padding = new Thickness(2) });
            }

            var rg = new TableRowGroup();
            rg.Rows.Add(row);

            TableRow row1 = new TableRow();
            for (int j = 0; j < CountColumnsTable; j++)
            {
                row1.Cells.Add(new TableCell(new Paragraph(new Run(""))) { BorderBrush = new SolidColorBrush(Colors.Black), BorderThickness = new Thickness(1), Padding = new Thickness(2) });
            }
            
            rg.Rows.Add(row1);

            t.RowGroups.Add(rg);

            EditablePanel.Document.Blocks.Add(t);

            EditablePanel.Document.Blocks.Add(new Paragraph());
        }

        private void AddImage(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "Image (png)|*.png |Image (jpg)|*.jpg|Image (jpeg)|*.jpeg";
            switch (ofd.ShowDialog())
            {
                case System.Windows.Forms.DialogResult.OK:
                    Image img = new Image();
                    using (var stream = File.OpenRead(ofd.FileName)) {
                        BitmapImage bi = new BitmapImage();
                        bi.BeginInit();
                        bi.CacheOption = BitmapCacheOption.OnLoad;
                        bi.StreamSource = stream;
                        bi.EndInit();

                        img.Source = bi;
                    }
                    EditablePanel.Document.Blocks.Add(new BlockUIContainer(img));
                    EditablePanel.Document.Blocks.Add(new Paragraph());
                    break;
            }
        }

        private void AddMarkerList(object sender, RoutedEventArgs e)
        {
            List list = new List();
            list.MarkerStyle = TypeMarkerText;
            list.StartIndex = 1;
            list.ListItems.Add(new ListItem(new Paragraph(new Run(""))));
            EditablePanel.Document.Blocks.Add(list);
        }

        private void cmbFormatPage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (cmbFormatPage.SelectedItem != null)
            //    switch (cmbFormatPage.SelectedItem.ToString())
            //    {
            //        case "A4":
            //            EditablePanel.Document.PageHeight = 842;
            //            EditablePanel.Document.PageWidth = 595;
            //            break;
            //    }
        }

        private void HiperLink(object sender, RoutedEventArgs e)
        {
            Hyperlink hl = new Hyperlink();
            hl.NavigateUri = new Uri("https://stackoverflow.com/questions/2644382/construct-hyperlink-in-wpf-c-sharp");
            EditablePanel.Document.Blocks.Add(new Paragraph(hl));
            EditablePanel.Document.Blocks.Add(new Paragraph());
        }

        private void ShowHideEditPanel(object sender, RoutedEventArgs e) {
            if (EditPanel.Height != 0)
                EditPanel.Height = 0;
            else
                EditPanel.Height = double.NaN;
        }

        #region Temp
        private void SaveDocument(object sender, RoutedEventArgs e)
        {
            string path = "TextDoc.xml";
            TextRange range = new TextRange(EditablePanel.Document.ContentStart, EditablePanel.Document.ContentEnd);
            FileStream fs = new FileStream(path, FileMode.Create);
            range.Save(fs, DataFormats.XamlPackage);
            fs.Close();
        }

        private void LoadedDocument(object sender, RoutedEventArgs e)
        {
            string path = "TextDoc.xml";
            TextRange range;
            FileStream fs;
            if (File.Exists(path)) {
                range = new TextRange(EditablePanel.Document.ContentStart, EditablePanel.Document.ContentEnd);
                fs = new FileStream(path, FileMode.Open);
                range.Load(fs, DataFormats.XamlPackage);
                fs.Close();
            }
        }
        #endregion

        private void btnAlignLeft_Checked(object sender, RoutedEventArgs e)
        {
            switch (((ToggleButton)sender).Name) {
                case "btnAlignLeft":
                    btnAlignCenter.IsChecked = false;
                    btnAlignRight.IsChecked = false;
                    btnAlignJustify.IsChecked = false;
                    break;
                case "btnAlignCenter":
                    btnAlignLeft.IsChecked = false;
                    btnAlignRight.IsChecked = false;
                    btnAlignJustify.IsChecked = false;
                    break;
                case "btnAlignRight":
                    btnAlignLeft.IsChecked = false;
                    btnAlignCenter.IsChecked = false;
                    btnAlignJustify.IsChecked = false;
                    break;
                case "btnAlignJustify":
                    btnAlignLeft.IsChecked = false;
                    btnAlignRight.IsChecked = false;
                    btnAlignCenter.IsChecked = false;
                    break;
            }
        }

        private void ActFontPanel_Checked(object sender, RoutedEventArgs e)
        {
            switch (((ToggleButton)sender).Name)
            {
                case "ActFilesPanel":
                    ActFontPanel.IsChecked = false;
                    ActAlignmentTextPanel.IsChecked = false;
                    ActPageSizePanel.IsChecked = false;
                    ActPrimitivyPanel.IsChecked = false;
                    ActObjectPanel.IsChecked = false;
                    ActMarkerPanel.IsChecked = false;
                    break;
                case "ActFontPanel":
                    ActFilesPanel.IsChecked = false;
                    ActAlignmentTextPanel.IsChecked = false;
                    ActPageSizePanel.IsChecked = false;
                    ActPrimitivyPanel.IsChecked = false;
                    ActObjectPanel.IsChecked = false;
                    ActMarkerPanel.IsChecked = false;
                    break;
                case "ActAlignmentTextPanel":
                    ActFilesPanel.IsChecked = false;
                    ActFontPanel.IsChecked = false;
                    ActPageSizePanel.IsChecked = false;
                    ActPrimitivyPanel.IsChecked = false;
                    ActObjectPanel.IsChecked = false;
                    ActMarkerPanel.IsChecked = false;
                    break;
                case "ActPageSizePanel":
                    ActFilesPanel.IsChecked = false;
                    ActFontPanel.IsChecked = false;
                    ActAlignmentTextPanel.IsChecked = false;
                    ActPrimitivyPanel.IsChecked = false;
                    ActObjectPanel.IsChecked = false;
                    ActMarkerPanel.IsChecked = false;
                    break;
                case "ActPrimitivyPanel":
                    ActFilesPanel.IsChecked = false;
                    ActFontPanel.IsChecked = false;
                    ActAlignmentTextPanel.IsChecked = false;
                    ActPageSizePanel.IsChecked = false;
                    ActObjectPanel.IsChecked = false;
                    ActMarkerPanel.IsChecked = false;
                    break;
                case "ActObjectPanel":
                    ActFilesPanel.IsChecked = false;
                    ActFontPanel.IsChecked = false;
                    ActAlignmentTextPanel.IsChecked = false;
                    ActPageSizePanel.IsChecked = false;
                    ActPrimitivyPanel.IsChecked = false;
                    ActMarkerPanel.IsChecked = false;
                    break;
                case "ActMarkerPanel":
                    ActFontPanel.IsChecked = false;
                    ActAlignmentTextPanel.IsChecked = false;
                    ActPageSizePanel.IsChecked = false;
                    ActPrimitivyPanel.IsChecked = false;
                    ActObjectPanel.IsChecked = false;
                    ActObjectPanel.IsChecked = false;
                    break;
            }
        }

        private void TableCell_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            ((FrameworkContentElement)sender).ContextMenu.IsOpen = true;
        }


        #region Static commands


        private static Command _SplitCommand;
        public static Command SplitCommand {
            get {
                return _SplitCommand ?? (_SplitCommand = new Command(obj =>
                {
                    string h = "";
                }));
            }
        }


        #endregion
    }
}
