using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NTW.Data.Context//Manipulation
{
    public partial class Note
    {
        public string DText {
            get { return this.Text; }
            set {
                if (this.Text != value && value != null && value != string.Empty) {
                    using (DBContext context = new DBContext(false)) {
                        Note n = context.Notes.FirstOrDefault(x => x.ID == this.ID);
                        if (n != null) {
                            n.Text = value;
                            context.SaveChanges();
                            this.Text = value;
                            this.OnPropertyChanged("DText");
                        }
                    }
                }
            }
        }

        public bool DIsTop {
            get { return this.IsTop; }
            set {
                if (this.IsTop != value)
                    using (DBContext context = new DBContext(false)) {
                        Note n = context.Notes.FirstOrDefault(x => x.ID == this.ID);
                        if (n != null) {
                            n.IsTop = value;
                            context.SaveChanges();
                            this.IsTop = value;
                        }
                    }
            }
        }

        public int DIdColor {
            get { return this.IDColor; }
            set
            {
                if (this.IDColor != value) {
                    using (DBContext context = new DBContext(false)) {
                        Note n = context.Notes.FirstOrDefault(x => x.ID == this.ID);
                        if (n != null) {
                            n.IDColor = value;
                            context.SaveChanges();
                            this.IDColor = value;
                            this.OnPropertyChanged(nameof(Color));
                        }
                    }
                }
            }
        }

        public Note Copy(int idGroup) {
            Note result = new Note();
            result.Text = this.Text;
            result.Owner = this.Owner;
            result.Group = idGroup;
            result.CreateDate = DateTime.Now;
            result.Width = this.Width;
            result.Height = this.Height;
            result.IDColor = this.IDColor;

            return result;
        }
    }
}
