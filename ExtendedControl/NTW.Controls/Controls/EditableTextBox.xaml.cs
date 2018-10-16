using NTW.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class EditableTextBox : UserControl, INotifyPropertyChanged
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

        private TableCellPresent[] _tableSample;
        public TableCellPresent[] TableSample {
            get {
                return _tableSample ?? (_tableSample = new TableCellPresent[] {
                    new TableCellPresent(1, 1), new TableCellPresent(1, 2), new TableCellPresent(1, 3), new TableCellPresent(1, 4), new TableCellPresent(1, 5), new TableCellPresent(1, 6), new TableCellPresent(1, 7), new TableCellPresent(1, 8) ,
                    new TableCellPresent(2, 1), new TableCellPresent(2, 2), new TableCellPresent(2, 3), new TableCellPresent(2, 4), new TableCellPresent(2, 5), new TableCellPresent(2, 6), new TableCellPresent(2, 7), new TableCellPresent(2, 8) ,
                    new TableCellPresent(3, 1), new TableCellPresent(3, 2), new TableCellPresent(3, 3), new TableCellPresent(3, 4), new TableCellPresent(3, 5), new TableCellPresent(3, 6), new TableCellPresent(3, 7), new TableCellPresent(3, 8) ,
                    new TableCellPresent(4, 1), new TableCellPresent(4, 2), new TableCellPresent(4, 3), new TableCellPresent(4, 4), new TableCellPresent(4, 5), new TableCellPresent(4, 6), new TableCellPresent(4, 7), new TableCellPresent(4, 8) ,
                    new TableCellPresent(5, 1), new TableCellPresent(5, 2), new TableCellPresent(5, 3), new TableCellPresent(5, 4), new TableCellPresent(5, 5), new TableCellPresent(5, 6), new TableCellPresent(5, 7), new TableCellPresent(5, 8) ,
                    new TableCellPresent(6, 1), new TableCellPresent(6, 2), new TableCellPresent(6, 3), new TableCellPresent(6, 4), new TableCellPresent(6, 5), new TableCellPresent(6, 6), new TableCellPresent(6, 7), new TableCellPresent(6, 8) ,
                    new TableCellPresent(7, 1), new TableCellPresent(7, 2), new TableCellPresent(7, 3), new TableCellPresent(7, 4), new TableCellPresent(7, 5), new TableCellPresent(7, 6), new TableCellPresent(7, 7), new TableCellPresent(7, 8) ,
                    new TableCellPresent(8, 1), new TableCellPresent(8, 2), new TableCellPresent(8, 3), new TableCellPresent(8, 4), new TableCellPresent(8, 5), new TableCellPresent(8, 6), new TableCellPresent(8, 7), new TableCellPresent(8, 8)
                });
            }
        }

        public TableCellPresent Present { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void SendPropertyChanged(string propertyName = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
            ((FrameworkContentElement)sender).ContextMenu.Tag = sender;
            ((FrameworkContentElement)sender).ContextMenu.IsOpen = true;
        }

        #region Dependency property
        public object FileContent {
            get { return (object)GetValue(FileContentProperty); }
            set { SetValue(FileContentProperty, value); }
        }

        public static readonly DependencyProperty FileContentProperty =
            DependencyProperty.Register("FileContent", typeof(object), typeof(EditableTextBox), new PropertyMetadata("File"));


        public object FontContent {
            get { return (object)GetValue(FontContentProperty); }
            set { SetValue(FontContentProperty, value); }
        }

        public static readonly DependencyProperty FontContentProperty =
            DependencyProperty.Register("FontContent", typeof(object), typeof(EditableTextBox), new PropertyMetadata("Font"));


        public object AligenmentContent {
            get { return (object)GetValue(AligenmentContentProperty); }
            set { SetValue(AligenmentContentProperty, value); }
        }
        
        public static readonly DependencyProperty AligenmentContentProperty =
            DependencyProperty.Register("AligenmentContent", typeof(object), typeof(EditableTextBox), new PropertyMetadata("Aligenment"));


        public object PageContent {
            get { return (object)GetValue(PageContentProperty); }
            set { SetValue(PageContentProperty, value); }
        }
        
        public static readonly DependencyProperty PageContentProperty =
            DependencyProperty.Register("PageContent", typeof(object), typeof(EditableTextBox), new PropertyMetadata("Page"));


        public object PrimitivyObjectContent {
            get { return (object)GetValue(PrimitivyObjectContentProperty); }
            set { SetValue(PrimitivyObjectContentProperty, value); }
        }
        
        public static readonly DependencyProperty PrimitivyObjectContentProperty =
            DependencyProperty.Register("PrimitivyObjectContent", typeof(object), typeof(EditableTextBox), new PropertyMetadata("Primitivy"));


        public object ObjectContent {
            get { return (object)GetValue(ObjectContentProperty); }
            set { SetValue(ObjectContentProperty, value); }
        }
        
        public static readonly DependencyProperty ObjectContentProperty =
            DependencyProperty.Register("ObjectContent", typeof(object), typeof(EditableTextBox), new PropertyMetadata("Object"));


        public object MarkerContent {
            get { return (object)GetValue(MarkerContentProperty); }
            set { SetValue(MarkerContentProperty, value); }
        }
        
        public static readonly DependencyProperty MarkerContentProperty =
            DependencyProperty.Register("MarkerContent", typeof(object), typeof(EditableTextBox), new PropertyMetadata("Marker"));


        #endregion

        #region Commands
        private Command _mouseOverTableCommand;
        public Command MouseOverTableCommand {
            get {
                return _mouseOverTableCommand ?? (_mouseOverTableCommand = new Command(obj =>
                {
                    TableCellPresent pr = obj as TableCellPresent;
                    foreach (var item in TableSample)
                        item.State = false;

                    foreach (var item in TableSample.Where(x => x.X <= pr.X && x.Y <= pr.Y))
                        item.State = true;

                    Present = pr;
                    this.SendPropertyChanged(nameof(Present));
                }, obj => obj != null));
            }
        }
        
        private Command _mouseLeaveTableCommand;
        public Command MouseLeaveTableCommand {
            get {
                return _mouseLeaveTableCommand ?? (_mouseLeaveTableCommand = new Command(obj =>
                {
                    foreach (var item in TableSample)
                        item.State = false;

                    Present = null;
                    this.SendPropertyChanged(nameof(Present));
                }));
            }
        }


        private Command _createTableCommand;
        public Command CreateTableCommand {
            get {
                return _createTableCommand ?? (_createTableCommand = new Command(obj =>
                {
                    TableCellPresent pr = obj as TableCellPresent;

                    EditablePanel.Document.Blocks.Add(new Paragraph());
                    Table t = new Table();
                    t.BorderBrush = new SolidColorBrush(Colors.Black);
                    t.BorderThickness = new Thickness(1);
                    for (int i = 0; i < pr.Y; i++)
                    {
                        t.Columns.Add(new TableColumn());
                    }

                    var rg = new TableRowGroup();
                    for (int i = 0; i < pr.X; i++)
                    {
                        TableRow row1 = new TableRow();
                        for (int j = 0; j < pr.Y; j++)
                        {
                            row1.Cells.Add(new TableCell(new Paragraph(new Run(""))) { BorderBrush = new SolidColorBrush(Colors.Black), BorderThickness = new Thickness(1), Padding = new Thickness(2) });
                        }

                        rg.Rows.Add(row1);
                    }

                    t.RowGroups.Add(rg);

                    EditablePanel.Document.Blocks.Add(t);

                    EditablePanel.Document.Blocks.Add(new Paragraph());
                }, obj => obj != null));
            }
        }


        #endregion

        #region Static commands
        private static Command _addColumnLeftCommand;
        public static Command AddColumnLeftCommand {
            get { return _addColumnLeftCommand ?? (_addColumnLeftCommand = new Command(obj => {
                var cell = obj as TableCell;
                var row = cell.Parent as TableRow;
                int index = row.Cells.IndexOf(cell);
                var table = row.Parent as TableRowGroup;
                foreach (var drow in table.Rows) {
                    drow.Cells.Insert(index, new TableCell(new Paragraph(new Run(""))) { BorderBrush = new SolidColorBrush(Colors.Black), BorderThickness = new Thickness(1), Padding = new Thickness(2) });
                }
            })); }
        }

        private static Command _addColumnRightCommand;
        public static Command AddColumnRightCommand {
            get {
                return _addColumnRightCommand ?? (_addColumnRightCommand = new Command(obj => {
                    var cell = obj as TableCell;
                    var row = cell.Parent as TableRow;
                    int index = row.Cells.IndexOf(cell) + 1;
                    var table = row.Parent as TableRowGroup;
                    foreach (var drow in table.Rows)
                    {
                        drow.Cells.Insert(index, new TableCell(new Paragraph(new Run(""))) { BorderBrush = new SolidColorBrush(Colors.Black), BorderThickness = new Thickness(1), Padding = new Thickness(2) });
                    }
                }));
            }
        }

        private static Command _addRowTopCommand;
        public static Command AddRowTopCommand {
            get {
                return _addRowTopCommand ?? (_addRowTopCommand = new Command(obj => {
                    var cell = obj as TableCell;
                    var row = cell.Parent as TableRow;
                    var rowGroup = row.Parent as TableRowGroup;
                    int index = rowGroup.Rows.IndexOf(row);
                    var newrow = new TableRow();
                    for (int j = 0; j < row.Cells.Count; j++)
                    {
                        newrow.Cells.Add(new TableCell(new Paragraph(new Run(""))) { BorderBrush = new SolidColorBrush(Colors.Black), BorderThickness = new Thickness(1), Padding = new Thickness(2) });
                    }
                    rowGroup.Rows.Insert(index, newrow);
                }));
            }
        }

        private static Command _addRowBottomCommand;
        public static Command AddRowBottomCommand {
            get {
                return _addRowBottomCommand ?? (_addRowBottomCommand = new Command(obj => {
                    var cell = obj as TableCell;
                    var row = cell.Parent as TableRow;
                    var rowGroup = row.Parent as TableRowGroup;
                    int index = rowGroup.Rows.IndexOf(row) + 1;
                    var newrow = new TableRow();
                    for (int j = 0; j < row.Cells.Count; j++)
                    {
                        newrow.Cells.Add(new TableCell(new Paragraph(new Run(""))) { BorderBrush = new SolidColorBrush(Colors.Black), BorderThickness = new Thickness(1), Padding = new Thickness(2) });
                    }
                    rowGroup.Rows.Insert(index, newrow);
                }));
            }
        }


        private static Command _removeFillRowCommand;
        public static Command RemoveFillRowCommand {
            get { return _removeFillRowCommand ?? (_removeFillRowCommand = new Command(obj => {
                var cell = obj as TableCell;
                var row = cell.Parent as TableRow;
                var rowGroup = row.Parent as TableRowGroup;
                rowGroup.Rows.Remove(row);
            })); }
        }

        private static Command _removeFillColumnCommand;
        public static Command RemoveFillColumnCommand {
            get { return _removeFillColumnCommand ?? (_removeFillColumnCommand = new Command(obj => {
                var cell = obj as TableCell;
                var row = cell.Parent as TableRow;
                int index = row.Cells.IndexOf(cell);
                var rowGroup = row.Parent as TableRowGroup;
                foreach (var item in rowGroup.Rows) {
                    item.Cells.RemoveAt(index);
                }
                var table = rowGroup.Parent as Table;
                table.Columns.RemoveAt(index);
            })); }
        }


        private static Command _removeTableCommand;
        public static Command RemoveTableCommand {
            get {
                return _removeTableCommand ?? (_removeTableCommand = new Command(obj =>
                {
                    var cell = obj as TableCell;
                    var row = cell.Parent as TableRow;
                    var rowGroup = row.Parent as TableRowGroup;
                    var table = rowGroup.Parent as Table;
                    FlowDocument document = table.Parent as FlowDocument;
                    document.Blocks.Remove(table);
                }, obj => obj != null));
            }
        }



        private static Command _SplitCommand;
        public static Command SplitCommand {
            get {
                return _SplitCommand ?? (_SplitCommand = new Command(obj =>
                {
                    var g = obj as TableCell;
                }));
            }
        }


        #endregion

        #region Models
        public class TableCellPresent:INotifyPropertyChanged {

            public TableCellPresent(int x, int y) {
                X = x;
                Y = y;
                State = false;
            }
            public int X { get; set; }
            public int Y { get; set; }

            private bool _state;
            public bool State { get { return _state; } set { _state = value; this.SendPropertyChanged(nameof(State)); } }

            public event PropertyChangedEventHandler PropertyChanged;

            private void SendPropertyChanged(string propertyName = "") {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
