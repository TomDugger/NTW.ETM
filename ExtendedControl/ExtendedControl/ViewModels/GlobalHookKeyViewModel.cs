using ExtendedControl.Behaviour;
using ExtendedControl.Views.ControlWindow;
using NTW.Core;
using NTW.Data.Context;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace ExtendedControl.ViewModels {
    public class GlobalHookKeyViewModel: ControlViewModel {
        #region Commands
        private Command _createHookKeyCommand;
        public Command CreateHookKeyCommand {
            get {
                return _createHookKeyCommand ?? (_createHookKeyCommand = new Command(obj => {
                    Window w = (Window)obj;
                    using (DBContext context = new DBContext(true)) {
                        GlobalHookKey chk = new GlobalHookKey();
                        chk.Key = ((int)System.Windows.Forms.Keys.F1).ToString();
                        chk.IDFucnk = 0;
                        chk.IDUser = CurrentUser.ID;

                        context.AddToGlobalHookKeys(chk);
                        context.SaveChanges();

                        HookKeys.Add(chk);
                    }
                }, obj => obj is Window));
            }
        }

        private Command _removeHookKeyCommad;
        public Command RemoveHookKeyCommand {
            get {
                return _removeHookKeyCommad ?? (_removeHookKeyCommad = new Command(obj => {
                    GlobalHookKey g = obj as GlobalHookKey;
                    using (DBContext context = new DBContext(true)) {
                        context.GlobalHookKeys.DeleteObject(context.GlobalHookKeys.FirstOrDefault(x => x.ID == g.ID));
                        context.SaveChanges();
                    }
                    HookKeys.Remove(g);
                }, obj => obj != null));
            }
        }


        private Command _definitionKeysCommand;
        public Command DefinitionKeysCommand {
            get {
                return _definitionKeysCommand ?? (_definitionKeysCommand = new Command(obj => {
                    App.ActiveDifinition = true;
                    App.KeyListner.def = true;
                    DefinitionKeysControlWindow dkw = new DefinitionKeysControlWindow();
                    KeyDefinitionControlBehaviour.SetKeys(dkw, (obj as GlobalHookKey).PKey);
                    NTW.Controls.Behaviours.WindowVisibilityBehaviour.SetIsDialogVisible(dkw, true);
                    var k = KeyDefinitionControlBehaviour.GetKeys(dkw);
                    if (k != null && k.Length != 0)
                        (obj as GlobalHookKey).PKey = k;

                    App.ActiveDifinition = false;
                    App.KeyListner.def = false;
                }, obj => obj != null));
            }
        }
        #endregion

        #region Members
        private ObservableCollection<GlobalHookKey> _hookKeys;
        public ObservableCollection<GlobalHookKey> HookKeys {
            get { return _hookKeys ?? (_hookKeys = new ObservableCollection<GlobalHookKey>(GetKeys())); }
        }

        private ICollectionView _hookKeysView;
        public ICollectionView HookKeysView {
            get { return _hookKeysView ?? (_hookKeysView = CollectionViewSource.GetDefaultView(HookKeys)); }
        }

        private string _searchText;
        public string SearchText {
            get { return _searchText; }
            set {
                _searchText = value;
                string h = value.ToUpper();
                HookKeysView.Filter = new Predicate<object>((x) => ((GlobalHookKey)x).Key.Contains(h));
                this.SendPropertyChanged(nameof(SearchText));
            }
        }

        public IEnumerable<Tuple<string, string, int, string>> Functions {
            get {
                List<Tuple<string, string, int, string>> result = new List<Tuple<string, string, int, string>>();

                result.Add(new Tuple<string, string, int, string>(App.GetString("HookKeyExitApplication"), App.GetString("HookKeyGroupApp"), 0, "ToolTip"));

                if (CurrentUser.Role.RestoreTask ||
                    CurrentUser.Role.CreateUser || CurrentUser.Role.DeleteUser || CurrentUser.Role.UpdateInfoUser ||
                    CurrentUser.Role.CreateNewRole || CurrentUser.Role.DeleteRole || CurrentUser.Role.UpdateRole ||
                    CurrentUser.Role.CreateSetting || CurrentUser.Role.DeleteSetting || CurrentUser.Role.UpdateSetting ||
                    CurrentUser.Role.CreateProject || CurrentUser.Role.DeleteProject || CurrentUser.Role.UpdateProject ||
                    CurrentUser.Role.VisibleJournal)
                    result.Add(new Tuple<string, string, int, string>(App.GetString("HookKeyAdminPanel"), App.GetString("HookKeyGroupPanels"), 1, "ToolTip"));
                result.Add(new Tuple<string, string, int, string>(App.GetString("HookKeyAppSettings"), App.GetString("HookKeyGroupPanels"), 2, "ToolTip"));
                result.Add(new Tuple<string, string, int, string>(App.GetString("HookKeyEventsPanel"), App.GetString("HookKeyGroupPanels"), 3, "ToolTip"));
                result.Add(new Tuple<string, string, int, string>(App.GetString("HookKeyHookKeyPanel"), App.GetString("HookKeyGroupPanels"), 4, "ToolTip"));
                result.Add(new Tuple<string, string, int, string>(App.GetString("HookKeyNotesPanel"), App.GetString("HookKeyGroupPanels"), 5, "ToolTip"));
                result.Add(new Tuple<string, string, int, string>(App.GetString("HookKeyProcessPanel"), App.GetString("HookKeyGroupPanels"), 6, "ToolTip"));
                result.Add(new Tuple<string, string, int, string>(App.GetString("HookKeyReports"), App.GetString("HookKeyGroupPanels"), 7, "ToolTip"));
                result.Add(new Tuple<string, string, int, string>(App.GetString("HookKeyUserSettings"), App.GetString("HookKeyGroupPanels"), 8, "ToolTip"));
                result.Add(new Tuple<string, string, int, string>(App.GetString("HookKeyTasksPanel"), App.GetString("HookKeyGroupPanels"), 9, "ToolTip"));
                result.Add(new Tuple<string, string, int, string>(App.GetString("HookKeyTrackingPanel"), App.GetString("HookKeyGroupPanels"), 10, "ToolTip"));

                result.Add(new Tuple<string, string, int, string>(App.GetString("HookKeyCreateProcess"), App.GetString("HookKeyGroupProcess"), 11, "ToolTip"));
                result.Add(new Tuple<string, string, int, string>(App.GetString("HookKeyCreateProcessOnClipboard"), App.GetString("HookKeyGroupProcess"), 12, "ToolTip"));
                //result.Add(new Tuple<string, string, int, string>(App.GetString("Процессы Удалить"), App.GetString("Process"), 13, "ToolTip"));
                //result.Add(new Tuple<string, string, int, string>(App.GetString("Процессы Изменить"), App.GetString("Process"), 14, "ToolTip"));
                //result.Add(new Tuple<string, string, int, string>(App.GetString("Процессы Копия"), App.GetString("Process"), 15, "ToolTip"));

                if (CurrentUser.Role.CreateTask) {
                    result.Add(new Tuple<string, string, int, string>(App.GetString("HookKeyCreateTask"), App.GetString("HookKeyGroupTasks"), 16, "ToolTip"));
                    result.Add(new Tuple<string, string, int, string>(App.GetString("HookKeyCreateTaskOnClipboard"), App.GetString("HookKeyGroupTasks"), 17, "ToolTip"));
                    result.Add(new Tuple<string, string, int, string>(App.GetString("HookKeyCopyTask"), App.GetString("HookKeyGroupTasks"), 18, "ToolTip"));
                }
                if (CurrentUser.Role.DeleteTask)
                    result.Add(new Tuple<string, string, int, string>(App.GetString("HookKeyRemoveTask"), App.GetString("HookKeyGroupTasks"), 19, "ToolTip"));
                if (CurrentUser.Role.UpdateInfoTask)
                    result.Add(new Tuple<string, string, int, string>(App.GetString("HookKeyEditTask"), App.GetString("HookKeyGroupTasks"), 20, "ToolTip"));

                result.Add(new Tuple<string, string, int, string>(App.GetString("HookKeyCreateNote"), App.GetString("HookKeyGroupNotes"), 21, "ToolTip"));
                result.Add(new Tuple<string, string, int, string>(App.GetString("HookKeyCreateNotenClipboard"), App.GetString("HookKeyGroupNotes"), 22, "ToolTip"));
                //result.Add(new Tuple<string, string, int, string>(App.GetString("Заметки Удалить"), App.GetString("Notes"), 23, "ToolTip"));
                //result.Add(new Tuple<string, string, int, string>(App.GetString("Заметки Изменить"), App.GetString("Notes"), 24, "ToolTip"));
                //result.Add(new Tuple<string, string, int, string>(App.GetString("Заметки Копия"), App.GetString("Notes"), 25, "ToolTip"));
                return result;
            }
        }
        #endregion

        #region Helps
        protected IEnumerable<GlobalHookKey> GetKeys() {
            IEnumerable<GlobalHookKey> result = null;
            using (DBContext context = new DBContext(true))
                result = context.GlobalHookKeys.Where(x => x.IDUser == CurrentUser.ID).ToArray();
            return result;
        }
        #endregion
    }
}
