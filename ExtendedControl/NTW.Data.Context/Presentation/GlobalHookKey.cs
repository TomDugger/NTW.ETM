using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace NTW.Data.Context//.Presentation 
    {
    public partial class GlobalHookKey {

        public string PresentKeys {
            get {
                string result = string.Empty;

                string[] keys = this.Key.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < keys.Length; i++) {
                    if (i == 0)
                        result = ((Key)Convert.ToInt32(keys[i])).ToString();
                    else
                        result += " + " + ((Key)Convert.ToInt32(keys[i])).ToString();
                }

                return result;
            }
        }

        public int Funck {
            get { return this.IDFucnk; }
            set {
                if (this.IDFucnk != value) {
                    this.IDFucnk = value;
                    using (DBContext context = new DBContext(false)) {
                        GlobalHookKey g = context.GlobalHookKeys.FirstOrDefault(x => x.ID == this.ID);
                        g.IDFucnk = value;
                        context.SaveChanges();
                    }
                    //нужно будет заставить перестроится обработчик глобального перехвата клавиш
                }
            }
        }

        public Key[] PKey {
            get {
                string[] keys = this.Key.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                Key[] result = new Key[keys.Length];
                for (int i = 0; i < keys.Length; i++) {
                    result[i] = (Key)Convert.ToInt32(keys[i]);
                }

                return result;
            }
            set {
                string result = string.Empty;
                for (int i = 0; i < value.Length; i++) {
                    result += " " + (int)value[i];
                }
                if (this.Key != result) {
                    this.Key = result;
                    using (DBContext context = new DBContext(false)) {
                        GlobalHookKey g = context.GlobalHookKeys.FirstOrDefault(x => x.ID == this.ID);
                        g.Key = result;
                        context.SaveChanges();
                    }
                    this.OnPropertyChanged("Key");
                    this.OnPropertyChanged("PKey");
                    this.OnPropertyChanged("PresentKeys");
                }
            }
        }
    }
}
