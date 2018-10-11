using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;

namespace NTW.Data.Context
{
    public partial class Task : IDataErrorInfo
    {
        public static Task New() {
            Task result = new Task();
            result.Caption = "Task";
            FlowDocument fd = new FlowDocument { AllowDrop = true };
            fd.Blocks.Add(new Paragraph(new Run("item")));
            result.Text = XamlWriter.Save(fd);
            result.TypeTask = 1;
            result.PriorityTask = 1;
            return result;
        }

        public string DText {
            get { return this.Text; }
        }

        private string _createrName;
        public string CreaterName {
            get {
                return _createrName ?? (_createrName = GetCreaterName());
            }
        }

        public bool ThereAreFiles {
            get { return this.TaskFiles.Count > 0; }
        }

        private ICollectionView _perfomerCollectionView;
        public ICollectionView PerfomerCollectionView {
            get { return _perfomerCollectionView ?? (_perfomerCollectionView = GetPerfomerCollectionView()); }
        }

        private string _searchTextPerfomers;
        public string SearchTextPerfomers {
            get { return this._searchTextPerfomers; }
            set {
                this._searchTextPerfomers = value;
                string h = value.ToUpper();
                if (PerfomerCollectionView.CanFilter)
                    PerfomerCollectionView.Filter = new Predicate<object>((x) => ((Perfomer)x).FullNamePerfomer.ToUpper().Contains(h));

                this.OnPropertyChanged(nameof(SearchTextPerfomers));
            }
        }

        private ICollectionView _fileCollectionView;
        public ICollectionView FileCollectionView {
            get { return _fileCollectionView ?? (_fileCollectionView = GetFileCollectionView()); }
        }

        private string _searchTextFiles;
        public string SearchTextFiles {
            get { return this._searchTextFiles; }
            set { this._searchTextFiles = value;
                string h = value.ToUpper();
                Application.Current.Dispatcher.BeginInvoke((Action)(() => {
                    if (FileCollectionView.CanFilter)
                        FileCollectionView.Filter = new Predicate<object>((x) => ((TaskFile)x).OriginalName.ToUpper().Contains(h));
                }));

                this.OnPropertyChanged(nameof(SearchTextPerfomers));
            }
        }

        private ObservableCollection<TaskComment> _comments;
        public ObservableCollection<TaskComment> Comments {
            get { return _comments ?? (_comments = new ObservableCollection<TaskComment>(GetComment())); }
        }

        private IEnumerable<TaskComment> _verticalComments;
        public IEnumerable<TaskComment> VerticalComments {
            get { return _verticalComments; }
            set { _verticalComments = value; this.OnPropertyChanged(nameof(VerticalComments)); }
        }

        private ICollectionView _commentCollection;
        public ICollectionView CommentCollection {
            get { return _commentCollection ?? (_commentCollection = GetCommentCollectionView()); }
        }

        private string _searchTextComments;
        public string SearchTextComments {
            get { return _searchTextComments; }
            set {
                _searchTextComments = value;
                string h = value.ToUpper();
                using (DBContext context = new DBContext(false))
                    CommentCollection.Filter = new Predicate<object>((x) => ((TaskComment)x).Commentary.ToUpper().Contains(h) ||
                    ((TaskComment)x).CreaterName.ToUpper().Contains(h));
                this.OnPropertyChanging(nameof(SearchTextComments));
            }
        }

        public string PText {
            get { return this.Text; }
            set { this.Text = value; this.OnPropertyChanged(nameof(RText)); this.OnPropertyChanged(nameof(PText)); }
        }

        public string RText {
            get { return this.Text; }
            set { this.Text = value; this.OnPropertyChanged(nameof(PText)); this.OnPropertyChanged(nameof(RText)); }
        }

        public int PersonOfChargeCount {
            get { return this.Perfomers.Count(x => x.PersonInCharge); }
        }

        public int OtherPerfomerCount {
            get { return this.Perfomers.Count(x => !x.PersonInCharge); }
        }

        private bool _isAccessMore;
        public bool IsAccessMore {
            get { return _isAccessMore; }
            set { _isAccessMore = value; this.OnPropertyChanged(nameof(IsAccessMore)); }
        }

        private int _countComment;
        public int CountComment {
            get { return _countComment; }
            set { _countComment = value; this.OnPropertyChanged(nameof(CountComment)); }
        }

        #region Helps
        public Task Disposing(string pathToDB) {
            if (this.TaskFiles != null || this.TaskFiles.Count != 0)
                foreach (var item in this.TaskFiles)
                    item.Disposing(pathToDB);
            return this;
        }

        protected string GetCreaterName() {
            string result = string.Empty;
            using (DBContext context = new DBContext(false))
            {
                User u = context.Users.FirstOrDefault(x => x.ID == this.Creater);
                if (u != null)
                    result = u.FullName;
            }
            return result;
        }

        public void RefreshErrorsState() {
            this.OnPropertyChanged(nameof(this.Caption));
            this.OnPropertyChanged(nameof(this.Perfomers));
        }

        protected ICollectionView GetPerfomerCollectionView() {
            return CollectionViewSource.GetDefaultView(this.Perfomers.ToList());
        }

        protected ICollectionView GetFileCollectionView() {
            return CollectionViewSource.GetDefaultView(this.TaskFiles.ToList());
        }

        protected IEnumerable<TaskComment> GetComment()
        {
            IEnumerable<TaskComment> result = null;
            using (DBContext context = new DBContext(false))
            {
                result = context.TaskComments.Where(x => !x.IsDelete && x.IdTask == this.ID).ToArray().Reverse().Take(5).Reverse().ToArray();

                CountComment = context.TaskComments.Where(x => !x.IsDelete && x.IdTask == this.ID).Count();

                IsAccessMore = CountComment >= result.Count();
            }
            VerticalComments = result;
            return result;
        }

        protected ICollectionView GetCommentCollectionView() {
            var view = CollectionViewSource.GetDefaultView(Comments);

            //view.GroupDescriptions.Clear();
            //view.GroupDescriptions.Add(new PropertyGroupDescription(nameof(TaskComment.OnlyDateCreateDate)));

            return view;
        }

        public void LoadMoreComment(int countMore = 5) {
            IEnumerable<TaskComment> result = null;
            using (DBContext context = new DBContext(false))
            {
                result = context.TaskComments.Where(x => !x.IsDelete && x.IdTask == this.ID).ToArray().Reverse().Skip(Comments.Count).Take(5).ToArray();

                IsAccessMore = result.Count() != 0;
            }
            foreach (var item in result)
                Comments.Insert(0, item);

            VerticalComments = null;
            VerticalComments = Comments;
        }

        public void AddComment(TaskComment comment) {

            Comments.Add(comment);

            VerticalComments = null;
            VerticalComments = Comments;

            CountComment++;
        }

        public TaskComment AddSystemComment(string commentary, int idUser)
        {
            TaskComment newComment = new TaskComment();
            newComment.IDUser = idUser;
            newComment.IdTask = this.ID;
            newComment.CreateDate = DateTime.Now;
            newComment.Commentary = commentary;
            newComment.TypeCommentary = 1;

            Comments.Add(newComment);

            VerticalComments = null;
            VerticalComments = Comments;

            CountComment++;

            return newComment;
        }

        public void DisposeComments() {
            _comments = null;
            _commentCollection = null;
            _searchTextComments = "";
        }

        public Task Copy() {
            Task result = new Task();
            result.Caption = "Copy_" + this.Caption;
            result.Text = this.Text;
            result.IsPersonal = this.IsPersonal;
            result.OpenDate = DateTime.Now;
            result.Creater = this.Creater;

            result.TypeTask = this.TypeTask;
            result.PriorityTask = this.PriorityTask;
            result.IdProject = this.IdProject;

            result.EndDate = this.OpenDate.AddDays(1);

            foreach (var item in this.Perfomers) {
                result.Perfomers.Add(new Perfomer { IDUser = item.IDUser, PersonInCharge = item.PersonInCharge, StartDate = result.OpenDate, CloseDate = result.OpenDate, OpenDateOnTask = result.OpenDate });
            }
            //файлы не копируются
            return result;
        }

        public void OnChangeCount() {
            this.OnPropertyChanged(nameof(PersonOfChargeCount));
            this.OnPropertyChanged(nameof(OtherPerfomerCount));
        }
        #endregion

        #region IDataErrorInfo
        private Dictionary<string, string> _errors;
        public Dictionary<string, string> Errors {
            get { return _errors ?? (_errors = new Dictionary<string, string>()); }
        }

        public string Error {
            get { return string.Join(Environment.NewLine, _errors.Select(x => x.Value)); ; }
        }

        public bool HasError { get { return _errors.Count == 0 ? false : true; } }

        public string this[string columnName] {
            get {
                string _error = string.Empty;
                switch (columnName) {
                    case nameof(this.Caption):
                        if (this.Caption == string.Empty)
                            Errors[nameof(this.Caption)] = _error = "TaskControlErrorCaptionIsEmpty";
                        else
                            Errors.Remove(nameof(this.Caption));
                        break;
                    case nameof(this.Perfomers):
                        if (this.Perfomers.Count == 0)
                            Errors[nameof(this.Perfomers)] = _error = "TaskControlErrorPerfomerCount";
                        else
                            Errors.Remove(nameof(this.Perfomers));
                        break;
                }
                this.OnPropertyChanged(nameof(Error));
                this.OnPropertyChanged(nameof(HasError));
                return _error;
            }
        }
        #endregion
    }
}
