using NTW.Moduls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NTW.Data.Context//.Presentation
{
    public partial class TaskFile
    {
        private StateFile _state = StateFile.Exists;
        public StateFile State {
            get { return _state; }
            set { _state = value; this.OnPropertyChanged(nameof(State)); }
        }

        private int _progress;
        public int Progress
        {
            get { return _progress; }
            set { _progress = value; this.OnPropertyChanged(nameof(Progress)); }
        }

        public TaskFile Inicial(string pathToDB, string pathByFile)
        {
            //копирование файла в директорию загрузки ресурсов БД
            if (!Directory.Exists(Path.Combine(pathToDB, "Resources")))
                Directory.CreateDirectory(Path.Combine(pathToDB, "Resources"));

            if (!File.Exists(Path.Combine(pathToDB, "Resources", this.StoreName)) &&
                File.Exists(pathByFile)) {
                this.State = StateFile.Load;
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    CopyEx.Copy(pathByFile, Path.Combine(pathToDB, "Resources", StoreName), true, true, (d, pce) =>
                    {
                        Progress = pce.ProgressPercentage;
                    });

                    this.State = StateFile.Exists;
                });
            }
            return this;
        }

        public TaskFile Disposing(string pathToDB) {
            if (!Directory.Exists(Path.Combine(pathToDB, "Resources")))
                Directory.CreateDirectory(Path.Combine(pathToDB, "Resources"));
            if (File.Exists(Path.Combine(pathToDB, "Resources", this.StoreName))) {
                File.Delete(Path.Combine(pathToDB, "Resources", StoreName));
                this.State = StateFile.NotExists;
            }
            return this;
        }
    }
}
