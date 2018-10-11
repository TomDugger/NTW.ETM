using NTW.Controls.Behaviours;
using NTW.Core;
using NTW.Data;
using NTW.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace NTW.Controls
{
    public delegate void CommandExecution(ControlMenuItem item);

    public class ControlMenuItem: INotifyPropertyChanged
    {
        public ControlMenuItem(object content, string resourceToolTip = null, Type typeElement = null, AddControlMenuItem parent = null, CommandExecution active = null, CommandExecution canActive = null)
        {
            this._content = content;
            this._element = null;
            this._typeElement = typeElement;
            this._active = active;
            this._canActive = canActive;
            this._parent = parent;
            this._resourceToolTip = resourceToolTip;
        }

        #region Members
        private object _content;
        public object Content {
            get { return _content; }
        }

        private string _resourceToolTip;
        public string ResourceToolTip
        {
            get { return _resourceToolTip; }
        }

        private Type _typeElement;
        public Type TypeElement {
            get { return _typeElement; }
        }

        private FrameworkElement _element;
        public FrameworkElement Element {
            get { return _element; }
        }

        private AddControlMenuItem _parent;
        public AddControlMenuItem Parent {
            get { return _parent; }
        }

        private CommandExecution _active;
        public CommandExecution Active {
            get { return _active; }
        }

        private CommandExecution _canActive;
        public CommandExecution CanActive {
            get { return _canActive; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void SendPropertyChanged(string propertyName = "") {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Helps

        public void SetElement(FrameworkElement element) {
            this._element = element;
            SendPropertyChanged("Element");
        }

        public ControlMenuItem SetParent(AddControlMenuItem parent) {
            this._parent = parent;
            return this;
        }
        #endregion
    }
}
