using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace NTW.Data.Models
{
    [Serializable]
    public class PanelSettings {

        #region Members

        private ObservableCollection<PanelParametry> _panels;
        public ObservableCollection<PanelParametry> Panels
        {
            get { return _panels ?? (_panels = new ObservableCollection<PanelParametry>()); }
            set { _panels = value; }
        }

        private ObservableCollection<PanelItemParametry> _controlWindow;
        public ObservableCollection<PanelItemParametry> ControlWindow {
            get { return _controlWindow ?? (_controlWindow = new ObservableCollection<PanelItemParametry>()); }
            set { _controlWindow = value; }
        }
        #endregion

        #region Helps


        public void Save() {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(Path.Combine(Environment.CurrentDirectory, "panels.cpf"), FileMode.Create))
            {
                formatter.Serialize(fs, this);
            }
        }

        public static void Build(WindowPosition position, int screen, IEnumerable<PanelItemParametry> items) {
            PanelSettings temp = PanelSettings.Load();

            temp.Panels.First(x => x.Position == position && x.IndexScreen == screen).Items = new List<PanelItemParametry>(items);

            temp.Save();
        }

        public static void Attached(Type typeElement) {
            PanelSettings temp = PanelSettings.Load();

            temp.Panels.FirstOrDefault(x => x.Items.Count(y => y.Element == typeElement) != 0).Items.Remove(temp.Panels.FirstOrDefault(x => x.Items.Count(y => y.Element == typeElement) != 0).Items.FirstOrDefault(w => w.Element == typeElement));

            temp.Save();
        }

        public static void SingleBuild(WindowPosition position, Type typeElement)
        {
            PanelSettings temp = PanelSettings.Load();

            var item = temp.Panels.First(x => x.Items.Count(y => y.Element == typeElement) != 0).Items.FirstOrDefault(w => w.Element == typeElement);
            int index = temp.Panels.First(x => x.Items.Count(y => y.Element == typeElement) != 0).Items.IndexOf(item);
            item.Position = position;

            temp.Panels.First(x => x.Items.Count(y => y.Element == typeElement) != 0).Items[index] = item;

            temp.Save();
        }

        public static void SingleBuild(Type typeElement, PanelItemParametry value) {
                PanelSettings temp = PanelSettings.Load();

                var item = temp.Panels.FirstOrDefault(x => x.Items.Count(y => y.Element == typeElement) != 0).Items.FirstOrDefault(w => w.Element == typeElement);
                int index = temp.Panels.FirstOrDefault(x => x.Items.Count(y => y.Element == typeElement) != 0).Items.IndexOf(item);

                temp.Panels.First(x => x.Items.Count(y => y.Element == typeElement) != 0).Items[index] = value;

                temp.Save();
        }

        public static PanelItemParametry GetParametry(Type typeElement) {
            return PanelSettings.Load().Panels.FirstOrDefault(x => x.Items.Count(y => y.Element == typeElement) != 0).Items.FirstOrDefault(w => w.Element == typeElement);
        }

        public static bool Exists() {
            return File.Exists(Path.Combine(Environment.CurrentDirectory, "panels.cpf"));
        } 

        public static PanelSettings Load() {
            PanelSettings settings;
            BinaryFormatter formatter = new BinaryFormatter();
            if (Exists())
                using (FileStream fs = new FileStream(Path.Combine(Environment.CurrentDirectory, "panels.cpf"), FileMode.Open))
                {
                    settings = (PanelSettings)formatter.Deserialize(fs);
                }
            else
                settings = new PanelSettings();

            return settings;
        }

        public static PanelItemParametry GetItem(PanelSettings parent, Type typeElement) {
            var result = parent.Panels.FirstOrDefault(x => x.Items.Count(y => y.Element == typeElement) != 0).Items.FirstOrDefault(x => x.Element == typeElement);
            return result;
        }

        public static PanelParametry GetPanel(PanelSettings parent, Type typeElement) {
            var result = parent.Panels.FirstOrDefault(x => x.Items.Count(y => y.Element == typeElement) != 0);
            return result;
        }
        #endregion
    }

    [Serializable]
    public class PanelParametry: INotifyPropertyChanged {

        public PanelParametry()
        {
            Random rnd = new Random();

            _color = System.Windows.Media.Color.FromArgb(255, (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255)).ToString();
        }

        private bool _isPrimary;
        public bool IsPrimary {
            get { return _isPrimary; }
            set { _isPrimary = value; this.SendPropertyChnaged(nameof(IsPrimary)); }
        }

        private WindowPosition _position;
        public WindowPosition Position
        {
            get { return _position; }
            set { _position = value; this.SendPropertyChnaged(nameof(Position)); }
        }

        private int _indexScreen;
        public int IndexScreen {
            get { return _indexScreen; }
            set { _indexScreen = value; this.SendPropertyChnaged(nameof(IndexScreen)); }
        }

        public int TextScreen {
            get { return IndexScreen + 1; }
        }

        private List<PanelItemParametry> _items;
        public List<PanelItemParametry> Items {
            get { return _items ?? (_items = new List<PanelItemParametry>()); }
            set { _items = value; }
        }

        private string _color;
        public string Color {
            get { return _color; }
            set { _color = value; }
        }

        [field: NonSerialized]
        public SolidColorBrush Brush {
            get { return new SolidColorBrush((Color)ColorConverter.ConvertFromString(Color)); }
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        public void SendPropertyChnaged(string propertyName = "") {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [Serializable]
    public struct PanelItemParametry {

        private Type _element;
        public Type Element {
            get { return _element; }
            set { _element = value; }
        }

        private WindowPosition _position;
        public WindowPosition Position {
            get { return _position; }
            set { _position = value; }
        }

        private Rect _bounds;
        public Rect Bounds {
            get { return _bounds; }
            set { _bounds = value; }
        }
    }
}
