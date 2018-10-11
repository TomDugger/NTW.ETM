using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace NTW.Data.Context
{
    public partial class NoteGroup
    {
        public int DIdColor
        {
            get { return this.IDColor; }
            set
            {
                if (this.IDColor != value)
                {
                    using (DBContext context = new DBContext(false))
                    {
                        NoteGroup n = context.NoteGroups.FirstOrDefault(x => x.ID == this.ID);
                        if (n != null)
                        {
                            n.IDColor = value;
                            context.SaveChanges();
                            this.IDColor = value;
                            this.OnPropertyChanged(nameof(Color));
                        }
                    }
                }
            }
        }

        public static NoteGroup New() {
            NoteGroup result = new NoteGroup();
            result.Caption = "Group";
            return result;
        }

        private bool _showNotes;
        public bool ShowNotes
        {
            get { return _showNotes; }
            set {
                _showNotes = value;
                CollectionNotes.Clear();
                if (value) {
                    foreach (var item in GetNote())
                        CollectionNotes.Add(item);
                }
                //this.OnPropertyChanged(nameof(ShowNotes));
            }
        }

        private ObservableCollection<Note> _collectionNotes;
        protected ObservableCollection<Note> CollectionNotes {
            get { return _collectionNotes ?? (_collectionNotes = new ObservableCollection<Note>()); }
        }

        public string NameGroup {
            get { return this.Caption; }
            set {
                if (this.Caption != value) {
                    using (DBContext context = new DBContext(false)) {
                        NoteGroup ng = context.NoteGroups.FirstOrDefault(x => x.ID == this.ID);
                        if (ng != null) {
                            ng.Caption = value;
                            context.SaveChanges();
                            this.Caption = value;
                        }
                    }
                }
            }
        }


        private ICollectionView _notesView;
        public ICollectionView NotesView {
            get { return _notesView ?? (_notesView = CollectionViewSource.GetDefaultView(CollectionNotes)); }
        }


        protected IEnumerable<Note> GetNote() {
            IEnumerable<Note> result = null;
            using (DBContext context = new DBContext(false)) {
                result = context.Notes.Where(x => x.Group == this.ID && !x.IsShowin).ToArray();
            }
            return result;
        }

        public bool GetContainsTextOnNotes(DBContext context, string value) {
            bool result = context.Notes.Count(x => x.Group == this.ID && x.Text.ToUpper().Contains(value.ToUpper())) > 0;
            //паралельно выставляем фильтр на соответствие
            NotesView.Filter = new Predicate<object>((x) => ((Note)x).Text.ToLower().Contains(value.ToLower()));
            return result;
        }

        public bool ContainsNoteOnGroup(Note note) {
            return this.CollectionNotes.Contains(note);
        }

        public void AddNewNote(Note note) {
            if (this.ShowNotes)
                CollectionNotes.Add(note);
            this.OnPropertyChanged("NotesCount");
        }

        public void RemoveNote(Note note) {
            CollectionNotes.Remove(note);
            this.OnPropertyChanged("NotesCount");
        }

        public void Refresh() {
            CollectionNotes.Clear();
            foreach (var item in GetNote()) {
                CollectionNotes.Add(item);
            }
            this.OnPropertyChanged(nameof(NotesCount));
        }

        public IEnumerable<Note> GetCurrentNotes() {
            return CollectionNotes.ToArray();
        }

        public NoteGroup Copy() {
            NoteGroup result = new NoteGroup();
            result.IDUser = this.IDUser;
            result.IDColor = this.IDColor;
            result.Caption = "Copy_" + this.Caption;
            return result;
        }
    }
}
