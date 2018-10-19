using ExtendedControl.ViewModels;
using ExtendedControl.Views;
using ExtendedControl.Views.ControlWindow;
using ExtendedControl.Views.DragDrop;
using ExtendedControl.Views.Panels;
using NTW.Communication.Beginers;
using NTW.Communication.Services;
using NTW.Controls;
using NTW.Controls.Behaviours;
using NTW.Core;
using NTW.Data;
using NTW.Data.Context;
using NTW.Data.Context.Connections;
using NTW.Data.Models;
using NTW.Moduls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Media;

namespace ExtendedControl
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private CommandExecution _activatePanel;
        protected CommandExecution ActivePanel {
            get { return _activatePanel ?? (_activatePanel = new CommandExecution((obj) => {

                if (obj.Element == null)
                {
                    obj.SetElement(CreateInstanceWindow(obj.TypeElement));

                    //извлекаем параметры для обратного построения
                    var settings = PanelSettings.Load();
                    var item = PanelSettings.GetItem(settings, obj.TypeElement);
                    var panel = PanelSettings.GetPanel(settings, obj.TypeElement);

                    WindowPositionBehaviour.SetWindowScreen(obj.Element, panel.IndexScreen);
                    WindowPositionBehaviour.SetWindowPosition(obj.Element, item.Position);
                    PanelItemParametry pip = PanelSettings.GetParametry(obj.TypeElement);
                    Window ww = (Window)obj.Element;
                    switch (item.Position)
                    {
                        case WindowPosition.None:
                            ww.Left = pip.Bounds.Left;
                            ww.Top = pip.Bounds.Top;
                            ww.Width = pip.Bounds.Width;
                            ww.Height = pip.Bounds.Height;
                            break;
                        case WindowPosition.Left:
                        case WindowPosition.Right:
                            ww.Width = pip.Bounds.Width;
                            break;
                        case WindowPosition.Top:
                        case WindowPosition.Bottom:
                            ww.Height = pip.Bounds.Height;
                            break;
                    }

                    HideButtonBehaviour.SetCommand(obj.Element, new Command((obj1) =>
                    {
                        obj.Active(obj);
                    }, obj1 => obj1 is FrameworkElement));

                    UntieButtonBehaviour.SetCommand(obj.Element, new Command((obj2) =>
                    {
                        WindowPosition wp = WindowPositionBehaviour.GetWindowPosition((Window)obj2);
                        if (wp != WindowPosition.None)
                        {
                            WindowPositionBehaviour.SetWindowPosition((Window)obj2, WindowPosition.None);
                            //сохраняем новое положение
                            PanelSettings.SingleBuild(WindowPosition.None, obj.TypeElement);

                            //и выставляем сохраненные параметры если были
                            Window w = (Window)obj2;
                            w.Left = pip.Bounds.Left;
                            w.Top = pip.Bounds.Top;
                            w.Width = pip.Bounds.Width;
                            w.Height = pip.Bounds.Height;
                        }
                        else
                        {
                            //извлекаем параметры для обратного построения
                            var itemPanel = PanelSettings.Load().Panels.FirstOrDefault(x => x.Items.Count(y => y.Element == obj.TypeElement) != 0);
                            WindowPositionBehaviour.SetWindowPosition((Window)obj2, itemPanel.Position);

                            PanelSettings.SingleBuild(itemPanel.Position, obj.TypeElement);
                        }
                    }, obj2 => obj2 is FrameworkElement));

                    WindowVisibilityBehaviour.SetIsVisible(obj.Element, true);
                }
                else
                {
                    WindowVisibilityBehaviour.SetIsVisible(obj.Element, false);
                    obj.SetElement(null);
                }
            })); }
        }

        private CommandExecution _unactiveCommand;
        public CommandExecution UnactiveCommand {
            get {
                return _unactiveCommand ?? (_unactiveCommand = new CommandExecution((obj) =>
                {
                    if(obj.Element != null)
                        WindowVisibilityBehaviour.SetIsVisible(obj.Element, false);
                    obj.SetElement(null);
                }));
            }
        }

        public ServerBeginer Server { get; set; }

        #region Localization
        private static List<CultureInfo> _languages;

        public static List<CultureInfo> Languages {
            get {
                return _languages ?? (_languages = new List<CultureInfo>());
            }
        }

        protected static IEnumerable<string> LocalizationResourcesNames {
            get { return new string[] { "Panel",
                "DBSettings",// +
                "Authorization",// +
                "Process",// +
                "Tasks",// +
                "Notes",// +
                "Settings",// +
                "Admin",// +
                "UserSettings",// +
                "HookKeys",// +
                "Messages",//
            };
            }
        }

        public static CultureInfo Language
        {
            get
            {
                return System.Threading.Thread.CurrentThread.CurrentUICulture;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                if (value == System.Threading.Thread.CurrentThread.CurrentUICulture) return;

                //1. Меняем язык приложения:
                System.Threading.Thread.CurrentThread.CurrentUICulture = value;
                //обрабатываем значения ресурсов локализации
                foreach (var item in LocalizationResourcesNames)
                {
                    ResourceDictionary dict = new ResourceDictionary
                    {
                        Source = new Uri(string.Format("/NTW.Resources;component/Localisation/{0}.{1}.xaml", item, value.Name), UriKind.Relative)
                    };
                    //3. ищем предыдущий ресурс с таким именем
                    ResourceDictionary oldDict = (from d in Application.Current.Resources.MergedDictionaries
                                                  where d.Source != null && d.Source.OriginalString.StartsWith(string.Format("/NTW.Resources;component/Localisation/{0}.", item))
                                                  select d).FirstOrDefault();
                    if (oldDict != null)
                    {
                        int ind = Application.Current.Resources.MergedDictionaries.IndexOf(oldDict);
                        Application.Current.Resources.MergedDictionaries.Remove(oldDict);
                        Application.Current.Resources.MergedDictionaries.Insert(ind, dict);
                    }
                    else
                        Application.Current.Resources.MergedDictionaries.Insert(0, dict);
                }
            }
        }
        #endregion

        public App() {
            Languages.Clear();
            Languages.Add(new CultureInfo("en"));
            Languages.Add(new CultureInfo("ru"));
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            this.MainWindow = new Window();

            base.OnStartup(e);

            bool active = false;
            // -2. подгрузка параметров приложения
            if (AppSettings.ExistFileSettings())
                AppSettings = AppSettings.Load();
            ((AppSettingsViewModel)GetViewModel(typeof(AppSettingsViewModel))).StateColorsResource(AppSettings.Theme);
            // -1. проверка настроек БД
            if (DBSettings.ExistFileSettings()) {
                Application.Current.Resources["DBSettings"] = DBSettings.Load(new DBSettings { Connection = new SqlConnection() });
                //test to the connection
                bool resultTestConnection = false;
                StartUpWindow startWindow = new StartUpWindow
                {
                    OnCloseAction = () =>
                    {
                        resultTestConnection = DBSettingsViewModel.TestConnection(App.DBSettings);
                    }
                };
                WindowVisibilityBehaviour.SetIsDialogVisible(startWindow, true);

                if (!resultTestConnection){
                    if (!(bool)new DBSettingsWindow().ShowDialog()) {
                        Application.Current.Shutdown();
                        return;
                    }
                };
            }

            if (!DBSettings.ExistFileSettings() && !(bool)new DBSettingsWindow().ShowDialog()) {
                Application.Current.Shutdown();
                return;
            }
            else if (UserSettings.Load(new UserSettings()).AutoEnter)
            {
                //формируем запись с параметрами пользователя
                User user;
                UserSettings s = UserSettings.Load();
                using (DBContext context = new DBContext(true))
                {
                    if ((user = context.Users.FirstOrDefault(i => i.UserName == s.UserName)) != null)
                    {
                        //проверка пароля
                        string password = s.Password;
                        if (Security.VerifyHashedPassword(user.Password, password))
                        {
                            //регистрируем пользователя и перестраиваем панели
                            int h = user.Role.ID;
                            h = user.Setting.ID;
                            App.CurrentUser = user;
                            App.Language = new CultureInfo(user.Language);
                            active = true;
                            // set data journal
                            AdminViewModel.SendDataJournal(TypeMessage.UserEnter, user);
                        }
                        else if (!(bool)new AuthorizationWindow().ShowDialog()) //0. проверка авторизации пользователя
                            Application.Current.Shutdown();
                        else active = true;
                    }
                    else if (!(bool)new AuthorizationWindow().ShowDialog()) //0. проверка авторизации пользователя
                        Application.Current.Shutdown();
                    else active = true;
                }
            }
            else if (!(bool)new AuthorizationWindow().ShowDialog()) // 0. проверка авторизации пользователя
                Application.Current.Shutdown();
            else
                active = true;

            if(active)
            {

                // 1. предворительная проверка и формирование панелей
                if (!PanelSettings.Exists())
                {
                    ControlPanelsWindow cpw = new ControlPanelsWindow();
                    WindowVisibilityBehaviour.SetIsDialogVisible(cpw, true);
                    if (!(bool)cpw.DialogResult) {
                        Application.Current.Shutdown();
                        return;
                    }
                }

                // 2. производим корректировку панелей
                //получаем список панелей и выводим их общее представление
                RebuildPanels();

                // 3. как внешние стоит обработать модель представления заметок
                NotesViewModel.ShowTopWindow();

                // 4. запуск на чтение горячих клавиш
                KeyListner.KeyUp += Global;

                // 5. запуск сервисов прослущивания сообщений
                string strHostName = Dns.GetHostName();
                IPHostEntry ipEntry = Dns.GetHostByName(strHostName);
                IPAddress[] addr = ipEntry.AddressList;
                string ip = "localhost";
                if (addr.Length > 0)
                    ip = addr[0].ToString();

                Server = new ServerBeginer(typeof(CommandService), typeof(ICommandService), ip);
                Server.Start(()=> {
                    using (DBContext context = new DBContext(false)) {
                        var u = context.Users.FirstOrDefault(x => x.ID == CurrentUser.ID);
                        u.IpAdress = ip;
                        context.SaveChanges();
                    }
                });
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            using (DBContext context = new DBContext(false))
            {
                var u = context.Users.FirstOrDefault(x => x.ID == CurrentUser.ID);
                u.IpAdress = string.Empty;
                context.SaveChanges();

                // set data journal
                AdminViewModel.SendDataJournal(TypeMessage.UserExit, u);
            }

            base.OnExit(e);
        }

        private void Global(object sender, RawKeyEventArgs args) {
            if (!ActiveDifinition) {
                //обрабатываем комбинации читая какие нужно использовать с проверкой на блокировку
                //по условиям сначала получаем список сапастовления клавиш. Далее ищем подходящую комбинацию в БД и в случае успешного сопостовления выполняем команду
                if (args.Keys.Length != 0) {
                    System.Windows.Input.Key[] Key = args.Keys;
                    Console.WriteLine(string.Join(" -> ", Key.Select(x => x)));
                    using (DBContext context = new DBContext(false)) {
                        var result = context.GlobalHookKeys.ToArray().FirstOrDefault(x => x.PKey.Intersect(Key).Count() == x.PKey.Count());
                        if(result != null)
                            switch (result.IDFucnk) {
                                case 0://Выйти из программы
                                    #region MyRegion
                                    Application.Current.Shutdown();
                                    break; 
                                #endregion

                                case 1://Администраторская панель
                                    #region MyRegion
                                    foreach (var item in Application.Current.Windows)
                                        if (item is MenuWindow) ((MenuWindow)item).VisibilityPanel<AdminControlPanel>();
                                    break; 
                                #endregion
                                case 2://Найстройки приложения
                                    #region MyRegion
                                    foreach (var item in Application.Current.Windows)
                                        if (item is MenuWindow) ((MenuWindow)item).VisibilityPanel<AppControlPanel>();
                                    break; 
                                #endregion
                                case 3://Панель событий
                                    #region MyRegion
                                    foreach (var item in Application.Current.Windows)
                                        if (item is MenuWindow) ((MenuWindow)item).VisibilityPanel<EventsControlPanel>();
                                    break; 
                                #endregion
                                case 4://Панель горячих клавиш
                                    #region MyRegion
                                    foreach (var item in Application.Current.Windows)
                                        if (item is MenuWindow) ((MenuWindow)item).VisibilityPanel<HookKeyControlPanel>();
                                    break; 
                                #endregion
                                case 5://Заметки
                                    #region MyRegion
                                    foreach (var item in Application.Current.Windows)
                                        if (item is MenuWindow) ((MenuWindow)item).VisibilityPanel<NotesControlPanel>();
                                    break; 
                                #endregion
                                case 6://Процессы
                                    #region MyRegion
                                    foreach (var item in Application.Current.Windows)
                                        if (item is MenuWindow) ((MenuWindow)item).VisibilityPanel<ProcesControlPanel>();
                                    break; 
                                #endregion
                                case 7://Отчеты
                                    #region MyRegion
                                    foreach (var item in Application.Current.Windows)
                                        if (item is MenuWindow) ((MenuWindow)item).VisibilityPanel<ReportControlPanel>();
                                    break; 
                                #endregion
                                case 8://Пользовательские настройки
                                    #region MyRegion
                                    foreach (var item in Application.Current.Windows)
                                        if (item is MenuWindow) ((MenuWindow)item).VisibilityPanel<SettingsControlPanel>();
                                    break; 
                                #endregion
                                case 9://Задачи
                                    #region MyRegion
                                    foreach (var item in Application.Current.Windows)
                                        if (item is MenuWindow) ((MenuWindow)item).VisibilityPanel<TaskControlPanel>();
                                    break; 
                                #endregion
                                case 10://Панель отслеживания процессов
                                    #region MyRegion
                                    foreach (var item in Application.Current.Windows)
                                        if (item is MenuWindow) ((MenuWindow)item).VisibilityPanel<TrackingStateControlPanel>();
                                    break; 
                                #endregion

                                case 11://Процессы Создать
                                    #region MyRegion
                                    App.GetViewModel<ProcessViewModel>().CreateProcess();
                                    #endregion
                                    break;
                                case 12://Процессы Создать из буфера обмена
                                    #region MyRegion
                                    App.GetViewModel<ProcessViewModel>().CreateProcessOnClipboard();
                                    break; 
                                #endregion
                                case 13://Процессы Удалить
                                    break;
                                case 14://Процессы Изменить
                                    break;
                                case 15://Процессы Копия
                                    break;

                                case 16://Задачи Создать
                                    #region MyRegion
                                    KeyListner.Look = true;
                                    bool create = true;
                                    foreach (var item in App.Current.Windows)
                                        if (item is TaskControlWindow)
                                        {
                                            create = false; break;
                                        }
                                    if (create)
                                        App.GetViewModel<TasksViewModel>().CreateTask();
                                    break; 
                                #endregion
                                case 17://Задачи Создать из буфера обмена
                                    #region MyRegion
                                    KeyListner.Look = true;
                                    bool createC = true;
                                    foreach (var item in App.Current.Windows)
                                        if (item is TaskControlWindow)
                                        {
                                            createC = false; break;
                                        }
                                    if (createC)
                                        App.GetViewModel<TasksViewModel>().CreateTaskOfClipboard();
                                    break; 
                                #endregion
                                case 18://Задачи Копия
                                    #region MyRegion
                                    var vmrr = App.GetViewModel<TasksViewModel>();
                                    KeyListner.Look = true;
                                    bool createrr = true;
                                    foreach (var item in App.Current.Windows)
                                        if (item is TaskControlWindow)
                                        {
                                            createrr = false; break;
                                        }
                                    if (createrr && vmrr.SelectedTask != null)
                                    {
                                        var panelTasks = new Window();
                                        foreach (var item in App.Current.Windows)
                                            if (item is TaskControlPanel)
                                            {
                                                panelTasks = (TaskControlPanel)item; break;
                                            }
                                        vmrr.CopyTaskCommand.Execute(new Tuple<object, object>(vmrr.SelectedTask, panelTasks));
                                    }
                                    break; 
                                #endregion
                                case 19://Задачи Удалить
                                    #region MyRegion
                                    var vmr = App.GetViewModel<TasksViewModel>();
                                    KeyListner.Look = true;
                                    bool creater = true;
                                    foreach (var item in App.Current.Windows)
                                        if (item is TaskControlWindow)
                                        {
                                            creater = false; break;
                                        }
                                    if (creater && vmr.SelectedTask != null)
                                    {
                                        var panelTasks = new Window();
                                        foreach (var item in App.Current.Windows)
                                            if (item is TaskControlPanel)
                                            {
                                                panelTasks = (TaskControlPanel)item; break;
                                            }
                                        vmr.RemoveTaskCommand.Execute(vmr.SelectedTask);
                                    }
                                    break;
                                #endregion
                                case 20://Задачи Изменить
                                    #region MyRegion
                                    var vm = App.GetViewModel<TasksViewModel>();
                                    KeyListner.Look = true;
                                    bool createE = true;
                                    foreach (var item in App.Current.Windows)
                                        if (item is TaskControlWindow)
                                        {
                                            createE = false; break;
                                        }
                                    if (createE && vm.SelectedTask != null)
                                    {
                                        var panelTasks = new Window();
                                        foreach (var item in App.Current.Windows)
                                            if (item is TaskControlPanel)
                                            {
                                                panelTasks = (TaskControlPanel)item; break;
                                            }
                                        vm.EditTaskCommand.Execute(new Tuple<object, object>(vm.SelectedTask, panelTasks));
                                    }
                                    break;
                                #endregion

                                case 21://Заметки Создать
                                    #region MyRegion
                                    App.GetViewModel<NotesViewModel>().CreateNote();
                                    break; 
                                #endregion
                                case 22://Заметки Создать из буфера обмена
                                    #region MyRegion
                                    if (Clipboard.GetText() != null && Clipboard.GetText() != "")
                                        App.GetViewModel<NotesViewModel>().CreateNoteOfClipBoard();
                                    break; 
                                #endregion
                                case 23://Заметки Удалить
                                    break;
                                case 24://Заметки Изменить
                                    break;
                                case 25://Заметки Копия
                                    break;
                            }
                    }
                }
            }
        }

        #region Static members
        public static User CurrentUser {
            get { return (User)Application.Current.Resources["CurrentUser"]; }
            set {

                Application.Current.Resources["CurrentUser"] = value;
                App.Language = new CultureInfo(value.Language); }
        }

        public static AppSettings AppSettings {
            get { return (AppSettings)Application.Current.Resources["AppSettings"]; }
            set { Application.Current.Resources["AppSettings"] = value; }
        }

        public static DBSettings DBSettings {
            get { return (DBSettings)Application.Current.Resources["DBSettings"]; }
            set { Application.Current.Resources["DBSettings"] = value; }
        }

        public static Color GetColorResource(string colorName) {
            return (Color)Application.Current.Resources[colorName];
        }

        public static void SetColorResource(string colorName, Color value, bool save = false) {
            Application.Current.Resources[colorName] = value;

            if (save) {
                switch (colorName) {
                    case nameof(App.AppSettings.MainBackColor):
                        App.AppSettings.MainBackColor = value.ToString(); App.AppSettings.Save();
                        break;
                    case nameof(App.AppSettings.MainForeColor):
                        App.AppSettings.MainForeColor = value.ToString(); App.AppSettings.Save();
                        break;
                    case nameof(App.AppSettings.FBackColor):
                        App.AppSettings.FBackColor = value.ToString(); App.AppSettings.Save();
                        break;
                    case nameof(App.AppSettings.FForeColor):
                        App.AppSettings.FForeColor = value.ToString(); App.AppSettings.Save();
                        break;
                    case nameof(App.AppSettings.SBackColor):
                        App.AppSettings.SBackColor = value.ToString(); App.AppSettings.Save();
                        break;
                    case nameof(App.AppSettings.SForeColor):
                        App.AppSettings.SForeColor = value.ToString(); App.AppSettings.Save();
                        break;
                    case nameof(App.AppSettings.TBackColor):
                        App.AppSettings.TBackColor = value.ToString(); App.AppSettings.Save();
                        break;
                    case nameof(App.AppSettings.TForeColor):
                        App.AppSettings.TForeColor = value.ToString(); App.AppSettings.Save();
                        break;

                }
            }
        }

        #region Listners
        private static KeyboardListener _keyListner;
        public static KeyboardListener KeyListner {
            get { return _keyListner ?? (_keyListner = new KeyboardListener()); }
        }

        public static bool ActiveDifinition = false;
        #endregion
        #endregion

        #region Helps
        private static Window CreateInstanceWindow(Type onType)
        {
            try
            {
                return (Window)Activator.CreateInstance(onType);
            }
            catch { return null; }
        }

        public static object GetViewModel(Type onType)
        {
            return ViewModelContainer.Instance.Register(onType);
        }

        public static T GetViewModel<T>() {
            return (T)ViewModelContainer.Instance.Register(typeof(T));
        }

        public static ControlMenuItem GetPresentItem(Type onType, 
            AddControlMenuItem parentControl, 
            AddControlMenuItem parentUser, 
            AddControlMenuItem parentEvent, 
            AddControlMenuItem parentWork,
            CommandExecution active, CommandExecution unactive) {
            if (onType == typeof(AdminControlPanel))
                return new ControlMenuItem(new Uri("pack://application:,,,/ExtendedControl;component/Images/ControlPanel.png"), resourceToolTip: "AdminControl", typeElement: onType, parent: parentControl, active: active, canActive: unactive);
            else if (onType == typeof(AppControlPanel))
                return new ControlMenuItem(new Uri("pack://application:,,,/ExtendedControl;component/Images/Settings.png"), resourceToolTip: "AppControl", typeElement: onType, parent: parentControl, active: active, canActive: unactive);
            else if (onType == typeof(HookKeyControlPanel))
                return new ControlMenuItem(new Uri("pack://application:,,,/ExtendedControl;component/Images/HookKeys.png"), resourceToolTip: "HookKeyControl", typeElement: onType, parent: parentControl, active: active, canActive: unactive);
            else if (onType == typeof(ReportControlPanel))
                return new ControlMenuItem(new Uri("pack://application:,,,/ExtendedControl;component/Images/Report.png"), resourceToolTip: "ReportsControl", typeElement: onType, parent: parentControl, active: active, canActive: unactive);

            else if (onType == typeof(SettingsControlPanel))
                return new ControlMenuItem(new Uri("pack://application:,,,/ExtendedControl;component/Images/UserSettings.png"), resourceToolTip: "UserSettings", typeElement: onType, parent: parentUser, active: active, canActive: unactive);

            else if (onType == typeof(EventsControlPanel))
                return new ControlMenuItem(new Uri("pack://application:,,,/ExtendedControl;component/Images/Events.png"), resourceToolTip: "EventsControl", typeElement: onType, parent: parentEvent, active: active, canActive: unactive);
            else if (onType == typeof(TrackingStateControlPanel))
                return new ControlMenuItem(new Uri("pack://application:,,,/ExtendedControl;component/Images/Tracking.png"), resourceToolTip: "TrackingControl", typeElement: onType, parent: parentEvent, active: active, canActive: unactive);

            else if (onType == typeof(NotesControlPanel))
                return new ControlMenuItem(new Uri("pack://application:,,,/ExtendedControl;component/Images/Notes.png"), resourceToolTip: "NotesControl", typeElement: onType, parent: parentWork, active: active, canActive: unactive);
            else if (onType == typeof(TaskControlPanel))
                return new ControlMenuItem(new Uri("pack://application:,,,/ExtendedControl;component/Images/Task.png"), resourceToolTip: "TaskControl", typeElement: onType, parent: parentWork, active: active, canActive: unactive);
            else if (onType == typeof(ProcesControlPanel))
                return new ControlMenuItem(new Uri("pack://application:,,,/ExtendedControl;component/Images/Process.png"), resourceToolTip: "ProcessControl", typeElement: onType, parent: parentWork, active: active, canActive: unactive);

            else return null;
        }

        protected static void SetPresentItem(Type onType,
                AddControlMenuItem parentControl,
                AddControlMenuItem parentUser,
                AddControlMenuItem parentEvent,
                AddControlMenuItem parentWork,
                CommandExecution active)
        {
            if (onType == typeof(AdminControlPanel))
                parentControl.AddItem(new ControlMenuItem(new Uri("pack://application:,,,/ExtendedControl;component/Images/ControlPanel.png"), resourceToolTip: "AdminControl", typeElement: onType, parent: parentControl, active: active));
            else if (onType == typeof(AppControlPanel))
                parentControl.AddItem(new ControlMenuItem(new Uri("pack://application:,,,/ExtendedControl;component/Images/Settings.png"), resourceToolTip: "AppControl", typeElement: onType, parent: parentControl, active: active));
            else if (onType == typeof(HookKeyControlPanel))
                parentControl.AddItem(new ControlMenuItem(new Uri("pack://application:,,,/ExtendedControl;component/Images/HookKeys.png"), resourceToolTip: "HookKeyControl", typeElement: onType, parent: parentControl, active: active));
            else if (onType == typeof(ReportControlPanel))
                parentControl.AddItem(new ControlMenuItem(new Uri("pack://application:,,,/ExtendedControl;component/Images/Report.png"), resourceToolTip: "ReportsControl", typeElement: onType, parent: parentControl, active: active));

            else if (onType == typeof(SettingsControlPanel))
                parentUser.AddItem(new ControlMenuItem(new Uri("pack://application:,,,/ExtendedControl;component/Images/Settings.png"), resourceToolTip: "UserSettings", typeElement: onType, parent: parentUser, active: active));

            else if (onType == typeof(EventsControlPanel))
                parentEvent.AddItem( new ControlMenuItem(new Uri("pack://application:,,,/ExtendedControl;component/Images/Events.png"), resourceToolTip: "EventsControl", typeElement: onType, parent: parentEvent, active: active));
            else if (onType == typeof(TrackingStateControlPanel))
                parentEvent.AddItem(new ControlMenuItem(new Uri("pack://application:,,,/ExtendedControl;component/Images/Tracking.png"), resourceToolTip: "TrackingControl", typeElement: onType, parent: parentEvent, active: active));

            else if (onType == typeof(NotesControlPanel))
                parentWork.AddItem(new ControlMenuItem(new Uri("pack://application:,,,/ExtendedControl;component/Images/Notes.png"), resourceToolTip: "NotesControl", typeElement: onType, parent: parentWork, active: active));
            else if (onType == typeof(TaskControlPanel))
                parentWork.AddItem(new ControlMenuItem(new Uri("pack://application:,,,/ExtendedControl;component/Images/Task.png"), resourceToolTip: "TaskControl", typeElement: onType, parent: parentWork, active: active));
            else if (onType == typeof(ProcesControlPanel))
                parentWork.AddItem(new ControlMenuItem(new Uri("pack://application:,,,/ExtendedControl;component/Images/Process.png"), resourceToolTip: "ProcessControl", typeElement: onType, parent: parentWork, active: active));
        }

        protected void RebuildPanels()
        {
            if (App.Current != null)
            {
                foreach (Window window in App.Current.Windows)
                {
                    if (App.Current.MainWindow != window)
                        window.Close();
                }

                WindowDragEndDropBehaviour.ClearParents();

                //построение панелей
                ObservableCollection<AddControlMenuItem> addItems = new ObservableCollection<AddControlMenuItem>();

                AddControlMenuItem ControlGroup = new AddControlMenuItem(new Uri("pack://application:,,,/ExtendedControl;component/Images/ControlGroup.png"), resourceToolTip: "ControlGroup");
                AddControlMenuItem UserGroup = new AddControlMenuItem(new Uri("pack://application:,,,/ExtendedControl;component/Images/UserGroup.png"), resourceToolTip: "UserGroup");
                AddControlMenuItem EventGroup = new AddControlMenuItem(new Uri("pack://application:,,,/ExtendedControl;component/Images/EventGroup.png"), resourceToolTip: "EventGroup");
                AddControlMenuItem WorkGroup = new AddControlMenuItem(new Uri("pack://application:,,,/ExtendedControl;component/Images/WorkGroup.png"), resourceToolTip: "WorkGroup");

                addItems.Add(ControlGroup);
                addItems.Add(UserGroup);
                addItems.Add(EventGroup);
                addItems.Add(WorkGroup);

                IEnumerable<Type> typesElement = new Type[] {
                    typeof(AdminControlPanel),
                    typeof(AppControlPanel),
                    typeof(HookKeyControlPanel),
                    typeof(ReportControlPanel),

                    typeof(SettingsControlPanel),

                    typeof(EventsControlPanel),
                    typeof(TrackingStateControlPanel),

                    typeof(NotesControlPanel),
                    typeof(TaskControlPanel),
                    typeof(ProcesControlPanel)
                };

                foreach (var item in PanelSettings.Load().Panels)
                {
                    //исходя из параметров общего представления формируем область обработки панелей и вложенных элементов
                    MenuWindow mw = new MenuWindow();

                    switch (item.Position)
                    {
                        case WindowPosition.Left:
                        case WindowPosition.Right:
                            WindowPositionBehaviour.SetWindowThickness(mw, new Thickness(0, 10, 0, 10));
                            break;
                        case WindowPosition.Top:
                        case WindowPosition.Bottom:
                            WindowPositionBehaviour.SetWindowThickness(mw, new Thickness(10, 0, 10, 0));
                            break;
                    }
                    mw.Left = System.Windows.Forms.Screen.AllScreens[item.IndexScreen].Bounds.Left;
                    mw.Top = System.Windows.Forms.Screen.AllScreens[item.IndexScreen].Bounds.Top;

                    WindowPositionBehaviour.SetWindowScreen(mw, item.IndexScreen);
                    WindowPositionBehaviour.SetWindowPosition(mw, item.Position);
                    mw.SizeToContent = SizeToContent.WidthAndHeight;
                    mw.SetDragDropElement(typeof(ItemPanelView));
                    mw.SetUseAnimation(true);

                    //первичное представление
                    if (item.IsPrimary)
                    {

                        mw.AddItem(new ControlMenuItem(new Uri("pack://application:,,,/ExtendedControl;component/Images/Shotdown.png"), resourceToolTip: "Shotdown", active: (obj) =>
                        {
                            NotesViewModel.HideAllNote();
                            Application.Current.Shutdown();
                        }));

                        mw.AddItem(new ControlMenuItem(new Uri("pack://application:,,,/ExtendedControl;component/Images/EnterUser.png"), resourceToolTip: "LogOut", active: (obj) =>
                        {

                            foreach (Window window in App.Current.Windows)
                            {
                                if (App.Current.MainWindow != window)
                                    window.Close();
                            }

                            if ((bool)new AuthorizationWindow().ShowDialog())
                                RebuildPanels();
                            else
                                Application.Current.Shutdown();
                        }));

                        mw.AddItem(new ControlMenuItem(new Uri("pack://application:,,,/ExtendedControl;component/Images/PanelsSettings.png"), resourceToolTip: "PanelSettings", active: (obj) =>
                        {
                            ControlPanelsWindow cpw = new ControlPanelsWindow();
                            cpw.ShowDialog();

                            RebuildPanels();
                        }));
                    }

                    mw.SetAddItems(addItems);

                    //элементы

                    if (item.Items != null)
                    {
                        foreach (var itemPanel in item.Items)
                        {
                            mw.AddItem((ControlMenuItem)GetPresentItem(itemPanel.Element, ControlGroup, UserGroup, EventGroup, WorkGroup, this.ActivePanel, this.UnactiveCommand));
                        }

                        typesElement = typesElement.Except(item.Items.Select(x => x.Element));
                    }

                    mw.Show();
                }

                foreach (var itemv in typesElement)
                {
                    SetPresentItem(itemv, ControlGroup, UserGroup, EventGroup, WorkGroup, this.ActivePanel);
                }
            }
        }

        public static void BeginInvoke(Action action)
        {
            Application.Current.Dispatcher.BeginInvoke(action);
        }

        public static void Invoke(Action action) {
            Application.Current.Dispatcher.Invoke(action);
        }

        public static string GetString(string Name) {
            var res = App.Current.FindResource(Name);
            if (res != null)
                return res.ToString();
            else
                return Name;
        }
        #endregion
    }
}
