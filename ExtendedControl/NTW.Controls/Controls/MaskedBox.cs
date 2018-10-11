﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace NTW.Controls {
    public class MaskedBox:TextBox {

        #region Members
        private MaskedTextProvider _mprovider = null;
        public string Mask {
            get {
                if (_mprovider != null) return _mprovider.Mask;
                else return "";
            }
            set {
                _mprovider = new MaskedTextProvider(value);
                this.Text = _mprovider.ToDisplayString();
            }
        }

        private bool _ignoreSpace = true;
        public bool IgnoreSpace {
            get { return _ignoreSpace; }
            set { _ignoreSpace = value; }
        }

        private bool _InsertIsON = false;

        private bool _NewTextIsOk = false;
        public bool NewTextIsOk {
            get { return _NewTextIsOk; }
            set { _NewTextIsOk = value; }
        }

        private bool _stayInFocusUntilValid;
        public bool StayInFocusUntilValid {
            get { return _stayInFocusUntilValid; }
            set { _stayInFocusUntilValid = value; }
        }
        #endregion

        #region Helps
        private void PressKey(Key key) {
            KeyEventArgs eInsertBack = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, key);
            eInsertBack.RoutedEvent = KeyDownEvent;
            InputManager.Current.ProcessInput(eInsertBack);
        }
        #endregion

        #region Overrides
        protected override void OnPreviewKeyDown(KeyEventArgs e) {
            if (this.SelectionLength > 1) {
                this.SelectionLength = 0;
                e.Handled = true;
            }
            if (e.Key == Key.Insert ||
                e.Key == Key.Delete ||
                e.Key == Key.Back ||
               (e.Key == Key.Space && _ignoreSpace)) {
                e.Handled = true;
            }
            base.OnPreviewKeyDown(e);
        }

        protected override void OnGotFocus(RoutedEventArgs e) {
            base.OnGotFocus(e);
            if (!_InsertIsON) {
                PressKey(Key.Insert);
                _InsertIsON = true;
            }
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e) {
            System.ComponentModel.MaskedTextResultHint hint;
            int TestPosition;

            if (e.Text.Length == 1)
                this._NewTextIsOk = _mprovider.VerifyChar(e.Text[0], this.CaretIndex, out hint);
            else
                this._NewTextIsOk = _mprovider.VerifyString(e.Text, out TestPosition, out hint);

            base.OnPreviewTextInput(e);
        }

        protected override void OnTextInput(System.Windows.Input.TextCompositionEventArgs e) {
            string PreviousText = this.Text;
            if (NewTextIsOk) {
                base.OnTextInput(e);
                if (_mprovider.VerifyString(this.Text) == false) this.Text = PreviousText;
                while (!_mprovider.IsEditPosition(this.CaretIndex) && _mprovider.Length > this.CaretIndex) this.CaretIndex++;

            }
            else
                e.Handled = true;
        }

        protected override void OnPreviewLostKeyboardFocus(KeyboardFocusChangedEventArgs e) {
            if (StayInFocusUntilValid) {
                _mprovider.Clear();
                _mprovider.Add(this.Text);
                if (!_mprovider.MaskFull) e.Handled = true;
            }

            base.OnPreviewLostKeyboardFocus(e);
        }
        #endregion
    }
}
