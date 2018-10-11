using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace NTW.Controls
{
    public class AddControlMenuItem: INotifyPropertyChanged
    {
        public  AddControlMenuItem(object content, ObservableCollection<ControlMenuItem> addItems = null, string resourceToolTip = null) {
            this._content = content;
            this._addItems = addItems == null ? new ObservableCollection<ControlMenuItem>() : addItems;
            this._resourceToolTip = resourceToolTip;
        }

        #region Members
        private object _content;
        public object Content
        {
            get { return _content; }
        }

        private ObservableCollection<ControlMenuItem> _addItems;
        public ObservableCollection<ControlMenuItem> AddItems {
            get { return _addItems; }
        }

        private string _resourceToolTip;
        public string ResourceToolTip {
            get { return _resourceToolTip; }
        }
        #endregion}

        #region Helps
        public void AddItem(ControlMenuItem item)
        {
            item.SetParent(this);
            if (_addItems == null)
                _addItems = new ObservableCollection<ControlMenuItem>();

            _addItems.Add(item);
        }

        public void RemoveItem(ControlMenuItem item) {
            if (_addItems != null)
                _addItems.Remove(item);
        }
        #endregion

        #region INotifyPropertyChanged 
        public event PropertyChangedEventHandler PropertyChanged;
        public void SendPropertyChanged(string propertyName = "") {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
