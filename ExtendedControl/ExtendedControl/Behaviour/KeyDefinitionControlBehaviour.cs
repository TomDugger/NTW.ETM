using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ExtendedControl.Behaviour {
    public class KeyDefinitionControlBehaviour {
        public static readonly DependencyProperty KeysProperty = DependencyProperty.RegisterAttached(
            "Keys", typeof(System.Windows.Input.Key[]), typeof(KeyDefinitionControlBehaviour), new PropertyMetadata(null));

        public static readonly DependencyProperty KeysPresentProperty = DependencyProperty.RegisterAttached(
            "KeysPresent", typeof(string), typeof(KeyDefinitionControlBehaviour), new PropertyMetadata("<Keys>"));

        public static readonly DependencyProperty UseKeyDefenitionProperty = DependencyProperty.RegisterAttached(
            "UseKeyDefenition", typeof(bool), typeof(KeyDefinitionControlBehaviour), new PropertyMetadata(false, new PropertyChangedCallback((d, a) => {
                if (d is Window) {
                    Window w = d as Window;

                    if ((bool)a.NewValue) {
                        App.KeyListner.KeyUp += KeyListner_KeyUp;
                        ParentControl = d;
                    }
                    else {
                        App.KeyListner.KeyUp -= KeyListner_KeyUp;
                        ParentControl = null;
                    }
                }
            })));

        private static DependencyObject ParentControl;

        private static void KeyListner_KeyUp(object sender, NTW.Moduls.RawKeyEventArgs args) {
            if (args.Keys.Length != 0) {
                System.Windows.Input.Key[] Key = args.Keys;
                if (ParentControl != null)
                    SetKeys(ParentControl, Key);
                string present = string.Empty;
                for (int i = 0; i < Key.Length; i++) {
                    if (i == 0)
                        present = Key[i].ToString();
                    else
                        present += " + " + Key[i].ToString();
                }

                if (ParentControl != null) {
                    SetKeysPresent(ParentControl, present);
                    //NTW.Controls.Behaviours.WindowStateBehaviour.SetState(ParentControl as Window, false);
                }
            }
        }

        public static void SetUseKeyDefenition(DependencyObject element, bool value) {
            element.SetValue(UseKeyDefenitionProperty, value);
        }

        public static bool GetUseKeyDefenition(DependencyObject element) {
            return (bool)element.GetValue(UseKeyDefenitionProperty);
        }


        public static void SetKeysPresent(DependencyObject element, string value) {
            element.SetValue(KeysPresentProperty, value);
        }

        public static string GetKeysPresent(DependencyObject element) {
            return (string)element.GetValue(KeysPresentProperty);
        }


        public static void SetKeys(DependencyObject element, System.Windows.Input.Key[] value) {
            element.SetValue(KeysProperty, value);
        }

        public static System.Windows.Input.Key[] GetKeys(DependencyObject element) {
            return (System.Windows.Input.Key[])element.GetValue(KeysProperty);
        }
    }
}
