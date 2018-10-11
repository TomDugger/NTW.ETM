using NTW.Controls.Behaviours;
using NTW.Controls.ViewModels;
using NTW.Core;
using NTW.Data;
using NTW.Data.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NTW.Controls
{
    /// <summary>
    /// Логика взаимодействия для MenuWindow.xaml
    /// </summary>
    public partial class MenuWindow : Window, INotifyPropertyChanged, IDropViewModel
    {
        public MenuWindow()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        #region Commands
        private Command _addCommand;
        public Command AddCommand {
            get { return _addCommand ?? (_addCommand = new Command(obj => {
                this.EntryAddItem((ControlMenuItem)obj);

                foreach (var item in AddItems)
                {
                    item.RemoveItem((ControlMenuItem)obj);
                }

                this.Activate();

                WindowPosition wp = WindowPositionBehaviour.GetWindowPosition(this);

            }, obj => obj is ControlMenuItem)); }
        }

        private Command _removeCommand;
        public Command RemoveCommand {
            get { return _removeCommand ?? (_removeCommand = new Command(obj => {
                ControlMenuItem ci = (ControlMenuItem)obj;
                //предворительно закрываем окно
                if (ci.CanActive != null)
                    ci.CanActive(ci);

                if (ci.Parent != null)
                    ci.Parent.AddItem((ControlMenuItem)obj);
                Items.Remove(ci);

                this.Activate();

                WindowPosition wp = WindowPositionBehaviour.GetWindowPosition(this);
                //по факту нужно просто открепить
                PanelSettings.Attached(((ControlMenuItem)obj).TypeElement);

            }, obj => obj is ControlMenuItem)); }
        }

        private Command _stateBeginCommand;
        public Command StateBeginCommand {
            get { return _stateBeginCommand ?? (_stateBeginCommand = new Command(obj => {
                ControlMenuItem cmi = (ControlMenuItem)obj;
                if (cmi.Element != null) {
                    UntieButtonBehaviour.GetCommand(cmi.Element).Execute(cmi.Element);
                }
            }, obj => obj is ControlMenuItem)); }
        }

        private Command _visibleItems;
        public Command VisibleItems {
            get { return _visibleItems ?? (_visibleItems = new Command(obj => {
                _isVisibleItems = !_isVisibleItems;
                this.SendPropertyChanged("IsVisibleItems");

                this.Activate();
            })); }
        }

        private Command _setItemDropCommand;
        public Command SetItemDropCommand {
            get { return _setItemDropCommand ?? (_setItemDropCommand = new Command(obj => {
                if(obj.ToString() != "{DisconnectedItem}")
                Items.Remove((ControlMenuItem)obj);

                WindowPosition wp = WindowPositionBehaviour.GetWindowPosition(this);
                int screen = WindowPositionBehaviour.GetWindowScreen(this);

                PanelSettings.Build(wp, screen, this.Items.Where(x => x.Parent != null).Select(x => new PanelItemParametry() { Element = x.TypeElement, Position = wp, Bounds = new Rect(0, 0, 200, 200) }));
            })); }
        }

        private Command _activeCommand;
        public Command ActiveCommand {
            get { return _activeCommand ?? (_activeCommand = new Command(obj => {
                ControlMenuItem cmi = (ControlMenuItem)obj;

                if (cmi.Active != null)
                    cmi.Active(cmi);

                foreach (var item in this.Items)
                    if (item != cmi && item.CanActive != null)
                        item.CanActive(item);

                //пака замарозим 
                //а не разморозим
                VisibleItems.Execute(null);

                this.Activate();
            })); }
        }

        private Command _mouseMoveCommand;
        public Command MouseMoveCommand {
            get { return _mouseMoveCommand ?? (_mouseMoveCommand = new Command(obj => {
                this.Activate();
            })); }
        }
        #endregion

        #region Propertys
        #region Members
        private bool _isVisibleItems;
        public bool IsVisibleItems {
            get { return _isVisibleItems; }
        }

        private bool _useAnimation;
        public bool UseAnimation {
            get { return _useAnimation; }
        }

        private Type _dragDropElement = typeof(Window);
        public Type DragDropElement {
            get { return _dragDropElement; }
        }
        #endregion

        private ObservableCollection<AddControlMenuItem> _addItems;
        public ObservableCollection<AddControlMenuItem> AddItems {
            get { return _addItems; }
        }

        private ObservableCollection<ControlMenuItem> _items;

        public ObservableCollection<ControlMenuItem> Items {
            get { return _items ?? (_items = new ObservableCollection<ControlMenuItem>());
            }
        }
        #endregion

        #region Helps
        public void SetAddItems(ObservableCollection<AddControlMenuItem> items) {
            _addItems = items;
        }

        public void SetDragDropElement(Type element) {
            _dragDropElement = element;
            this.SendPropertyChanged("DragDropElement");
        }

        public void SetUseAnimation(bool value) {
            _useAnimation = value;
            this.SendPropertyChanged("UseAnimation");
        }

        public void AddItem(ControlMenuItem item) {
            WindowPosition wp = WindowPositionBehaviour.GetWindowPosition(this);

            if (item.Element != null) {
                switch (wp)
                {
                    case WindowPosition.LeftTop:
                    case WindowPosition.LeftBottom:
                        wp = WindowPosition.Left;
                        break;
                    case WindowPosition.RightTop:
                    case WindowPosition.RightBottom:
                        wp = WindowPosition.Right;
                        break;
                }
                WindowPositionBehaviour.SetWindowPosition(item.Element, wp);
            }

            this.Items.Add(item);
        }

        public void EntryAddItem(ControlMenuItem item)
        {
            int screen = WindowPositionBehaviour.GetWindowScreen(this);

            WindowPosition wp = WindowPositionBehaviour.GetWindowPosition(this);

            WindowPosition wpe = wp;

            if (item.Element != null)
            {
                switch (wp)
                {
                    case WindowPosition.LeftTop:
                    case WindowPosition.LeftBottom:
                        wpe = WindowPosition.Left;
                        break;
                    case WindowPosition.RightTop:
                    case WindowPosition.RightBottom:
                        wpe = WindowPosition.Right;
                        break;
                }
                if (WindowPositionBehaviour.GetWindowPosition(item.Element) != WindowPosition.None)
                    WindowPositionBehaviour.SetWindowPosition(item.Element, wpe);

                WindowPositionBehaviour.SetWindowScreen(item.Element, WindowPositionBehaviour.GetWindowScreen(this));
            }

            this.Items.Add(item);

            PanelSettings.Build(wp, screen, this.Items.Where(x => x.Parent != null).Select(x => new PanelItemParametry() { Element = x.TypeElement, Position = wpe, Bounds = new Rect(GetStartSizeByTypePanel(x.TypeElement)) }));
        }

        private Size GetStartSizeByTypePanel(Type typePanel) {
            PropertyInfo startSizePr = (typePanel).GetProperty("StartSize");
            return (Size)startSizePr.GetValue(null, null);
        }

        public bool VisibilityPanel<T>() {
            bool result = false;
            ControlMenuItem cm = Items.FirstOrDefault(x => x.TypeElement == typeof(T));

            _isVisibleItems = true;
            //VisibleItems.Execute(null);

            //foreach (var item in this.Items)
            //    if (item != cm && item.CanActive != null)
            //        item.CanActive(item);

            if (cm != null) {
                this.ActiveCommand.Execute(cm);
                result = true;
            }

            return result;
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void SendPropertyChanged(string propertyName = "") {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region IDropViewModel
        public void SetValues(Type typeContent, IEnumerable values)
        {
            foreach (var value in values)
                if (value.ToString() != "{DisconnectedItem}")
                {
                    this.EntryAddItem((ControlMenuItem)value);
                }

            this.Activate();

            _isVisibleItems = true;
            this.SendPropertyChanged("IsVisibleItems");
        }

        public void SetValues(UIElement element, IEnumerable values) { }
        #endregion
    }
}
