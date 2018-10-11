using ExtendedControl.Views.Panels;
using Microsoft.Win32;
using NTW.Controls;
using NTW.Controls.Behaviours;
using NTW.Controls.ViewModels;
using NTW.Core;
using NTW.Data;
using NTW.Data.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ExtendedControl.ViewModels
{
    public class ControlPanelViewModel: ViewModel, IDropViewModel {
        #region Commands
        private Command _addNewPanelCommand;
        public Command AddNewPanelCommand {
            get { return _addNewPanelCommand ?? (_addNewPanelCommand = new Command(obj => {
                if (SelectedPosition != null)
                    Panels.Panels.Add(new PanelParametry { Position = (WindowPosition)SelectedPosition, IsPrimary = Panels.Panels.Count == 0, IndexScreen = IndexScreen });
                this.SendPropertyChanged(nameof(AffordableAccommodation));
                this.SendPropertyChanged(nameof(Screens));
            })); }
        }

        private Command _removePanelCommand;
        public Command RemovePanelCommand {
            get {
                return _removePanelCommand ?? (_removePanelCommand = new Command(obj => {

                    if (((PanelParametry)obj).IsPrimary && Panels.Panels.Count > 0)
                        Panels.Panels[0].IsPrimary = true;
                    
                    Panels.Panels.Remove((PanelParametry)obj);

                    _panelsList = null;
                    this.SendPropertyChanged(nameof(PanelsList));

                    this.SendPropertyChanged(nameof(AffordableAccommodation));

                    this.SendPropertyChanged(nameof(Screens));

                }, obj => obj != null && obj is PanelParametry));
            }
        }

        private Command _acceptedCommand;
        public Command AcceptedCommand {
            get { return _acceptedCommand ?? (_acceptedCommand = new Command(obj => {
                if (Panels.Panels.Count > 0) {
                    Panels.Save();
                    ((Window)obj).DialogResult = true;
                    ((Window)obj).Close();
                }
                else {
                    WindowMessageBehaviour.SetMessage((Window)obj, "Not create panels!", Colors.Maroon, Colors.White);
                }
            }, obj => obj is Window)); }
        }

        private Command _primaryCommand;
        public Command PrimaryCommand
        {
            get { return _primaryCommand ?? (_primaryCommand = new Command(obj => {
                CheckBox cb = (CheckBox)obj;
                if (cb.IsChecked == true)
                {
                    PanelParametry pp = (PanelParametry)cb.DataContext;
                    foreach (var item in Panels.Panels) 
                        item.IsPrimary = false;

                    pp.IsPrimary = true;
                }
                else
                    cb.IsChecked = true;
            }, obj => obj is CheckBox)); }
        }

        private Command _removeOnPanelCommand;
        public Command RemoveOnPanelsCommand {
            get { return _removeOnPanelCommand ?? (_removeOnPanelCommand = new Command(obj => {
                PanelItemParametry pip = (PanelItemParametry)obj;
                PanelParametry ps = Panels.Panels.FirstOrDefault(x => x.Items.Contains(pip));
                if (ps != null) {
                    ps.Items.Remove(pip);
                    CollectionViewSource.GetDefaultView(ps.Items).Refresh();
                    _panelsList = null;
                    this.SendPropertyChanged(nameof(PanelsList));
                    this.SendPropertyChanged(nameof(Screens));
                }
            }, obj => obj != null)); }
        }
        #endregion

        #region Members
        private double? _maxSize;
        protected double? MaxSize {
            get { return _maxSize ?? (_maxSize = Math.Max(System.Windows.Forms.Screen.AllScreens.Max(x => x.Bounds.Width), System.Windows.Forms.Screen.AllScreens.Max(x => x.Bounds.Height))); }
        }

        private int _indexScreen;
        public int IndexScreen {
            get { return _indexScreen; }
            set { _indexScreen = value; this.SendPropertyChanged(nameof(IndexScreen)); this.SendPropertyChanged(nameof(AffordableAccommodation)); }
        }

        private double _maxWidth;
        public double MaxWidth {
            get { return _maxWidth; }
            set { _maxWidth = value; this.SendPropertyChanged(nameof(MaxWidth)); }
        }

        private double _maxHeight;
        public double MaxHeight {
            get { return _maxHeight; }
            set { _maxHeight = value; this.SendPropertyChanged(nameof(MaxHeight)); }
        }


        private PanelSettings _panels;
        public PanelSettings Panels {
            get { return _panels ?? (_panels = PanelSettings.Load()); }
        }

        private IEnumerable<Tuple<System.Drawing.Rectangle, ObservableCollection<PanelParametry>>> _screens;
        public IEnumerable<Tuple<System.Drawing.Rectangle, ObservableCollection<PanelParametry>>> Screens {
            get {
                var scs = System.Windows.Forms.Screen.AllScreens;
                double size = 200;
                double maxSize = Math.Max(scs.Max(x => x.Bounds.Width), scs.Max(x => x.Bounds.Height));
                double leftStep = scs.Min(y => y.Bounds.X) * -1;
                double topStep = scs.Min(y => y.Bounds.Y) * -1;
                var kof = size / maxSize;

                var res = scs.Select(x => new Tuple<System.Drawing.Rectangle, ObservableCollection<PanelParametry>>(Rect(x.Bounds, kof, leftStep, topStep), 
                    new ObservableCollection<PanelParametry>(Panels.Panels.Where(y => y.IndexScreen == System.Windows.Forms.Screen.AllScreens.ToList().IndexOf(x))))).ToArray();
                _screens = res;

                MaxWidth = res.Max(x => x.Item1.Width + x.Item1.X);
                MaxHeight = res.Max(y => y.Item1.Height + y.Item1.Y);

                return _screens;
            }
        }

        public ImageBrush DesctopSource {
            get {
                ImageBrush result = new ImageBrush();

                RegistryKey key = Registry.CurrentUser;
 
                key = key.OpenSubKey("Control Panel\\Desktop");
 
                string w = (string)key.GetValue("Wallpaper");  
 
                BitmapImage desctop = new BitmapImage();
                desctop.BeginInit();
                desctop.UriSource = new Uri(w, UriKind.Absolute);
                desctop.EndInit();

                result.ImageSource = desctop;
                return result;
            }
        }

        private WindowPosition? _selectedPosition;
        public WindowPosition? SelectedPosition {
            get { return _selectedPosition; }
            set { _selectedPosition = value; this.SendPropertyChanged(nameof(SelectedPosition)); }
        }

        public IEnumerable<WindowPosition> AffordableAccommodation {
            get {
                IEnumerable<WindowPosition> result = null;
                result = ((WindowPosition[])Enum.GetValues(typeof(WindowPosition))).Where(x => x != WindowPosition.Non && 
                x != WindowPosition.None && 
                Panels.Panels.Count(y => y.Position == x && y.IndexScreen == IndexScreen) == 0);
                SelectedPosition = result.FirstOrDefault();
                return result;
            }
        }

        private IEnumerable<ControlMenuItem> _panelsList;
        public IEnumerable<ControlMenuItem> PanelsList {
            get { return _panelsList ?? (_panelsList = GetAvailablePanels()); }
        }

        public IEnumerable<string> ScreenName {
            get { return System.Windows.Forms.Screen.AllScreens.Select(x => x.DeviceName); }
        }
        #endregion

        #region Helps
        private IEnumerable<ControlMenuItem> GetAvailablePanels() {
            List<ControlMenuItem> result = new List<ControlMenuItem>();
            IEnumerable<Type> typesElement = new Type[] {
                    typeof(AdminControlPanel),
                    typeof(AppControlPanel),
                    typeof(HookKeyControlPanel),
                    typeof(ReportControlPanel),

                    typeof(SettingsControlPanel),

                    typeof(EventsControlPanel),
                    typeof(TrackingStateControlPanel),

                    typeof(NotesControlPanel),
                    typeof(TaskControlPanel),
                    typeof(ProcesControlPanel)
                };
            foreach (var item in typesElement)
                if (Panels.Panels.Count(x => x.Items.Count(y => y.Element == item) != 0) == 0)
                    result.Add(App.GetPresentItem(item, null, null, null, null, null, null));
            return result;
        }

        #region IDropViewModel implementation
        public void SetValues(Type typeContent, IEnumerable values) {
            
        }

        public void SetValues(UIElement element, IEnumerable values) {
            var sender = (FrameworkElement)element;
            PanelParametry parametry = (PanelParametry)((FrameworkElement)sender.Parent).DataContext;

            foreach (ControlMenuItem item in values)
                if (item.ToString() != "{DisconnectedItem}") {
                    WindowPosition wp = parametry.Position;
                    PanelItemParametry pp = new PanelItemParametry { Element = item.TypeElement, Position= wp, Bounds = new Rect(GetStartSizeByTypePanel(item.TypeElement)) };
                    parametry.Items.Add(pp);
                    CollectionViewSource.GetDefaultView(parametry.Items).Refresh();

                    _panelsList = null;
                    this.SendPropertyChanged(nameof(PanelsList));
                }
        }
        #endregion

        private Size GetStartSizeByTypePanel(Type typePanel) {
            PropertyInfo startSizePr = (typePanel).GetProperty("StartSize");
            return (Size)startSizePr.GetValue(null, null);
        }

        public System.Drawing.Rectangle Rect(System.Drawing.Rectangle rect, double kof, double leftStep, double topStep)
        {
            System.Drawing.Rectangle result = new System.Drawing.Rectangle
            {
                X = (int)(rect.X * kof + (leftStep * kof)),
                Y = (int)(rect.Y * kof + (topStep * kof)),
                Width = (int)(rect.Width * kof),
                Height = (int)(rect.Height * kof)
            };

            return result;
        }
        #endregion
    }
}
