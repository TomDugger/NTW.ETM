using NTW.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace NTW.Controls.Behaviours
{
    public class WindowMessageBehaviour
    {
        public static readonly DependencyProperty RegisterMessageOnJournalProperty = DependencyProperty.RegisterAttached(
            "RegisterMessageOnJournal", typeof(bool), typeof(WindowMessageBehaviour), new PropertyMetadata(false));

        public static readonly DependencyProperty SetMessageOnClipBoardProperty = DependencyProperty.RegisterAttached(
            "SetMessageOnClipBoard", typeof(bool), typeof(WindowMessageBehaviour), new PropertyMetadata(false));

        public static readonly DependencyProperty UseMessagesProperty = DependencyProperty.RegisterAttached(
            "UseMessages", typeof(bool), typeof(WindowMessageBehaviour), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty OriginalPointProperty = DependencyProperty.RegisterAttached(
            "OriginalPoint", typeof(Point), typeof(WindowMessageBehaviour), new PropertyMetadata(new Point(0, 0)));

        public static readonly DependencyProperty VerticalPositionProperty = DependencyProperty.RegisterAttached(
            "VerticalPosition", typeof(VerticalAlignment), typeof(WindowMessageBehaviour), new PropertyMetadata(VerticalAlignment.Top, new PropertyChangedCallback((d, a) =>
            {
                if ((VerticalAlignment)a.NewValue == VerticalAlignment.Top)
                    SetOriginalPoint(d, new Point(0, 0));
                else if ((VerticalAlignment)a.NewValue == VerticalAlignment.Bottom)
                    SetOriginalPoint(d, new Point(0, 1));
            })));

        public static readonly DependencyProperty MessagesProperty = DependencyProperty.RegisterAttached(
            "Messages", typeof(ObservableCollection<ItemMessage>), typeof(WindowMessageBehaviour), new PropertyMetadata(null));


        public static void SetRegisterMessageOnJournal(DependencyObject element, bool value) {
            element.SetValue(RegisterMessageOnJournalProperty, value);
        }

        public static bool GetRegisterMessageOnJournal(DependencyObject element) {
            return (bool)element.GetValue(RegisterMessageOnJournalProperty);
        }


        public static void SetSetMessageOnClipBoard(DependencyObject element, bool value) {
            element.SetValue(SetMessageOnClipBoardProperty, value);
        }

        public static bool GetSetMessageOnClipBoard(DependencyObject element) {
            return (bool)element.GetValue(SetMessageOnClipBoardProperty);
        }


        public static void SetUseMessages(DependencyObject element, bool value) {
            element.SetValue(UseMessagesProperty, value);
        }

        public static bool GetUseMessages(DependencyObject element) {
            return (bool)element.GetValue(UseMessagesProperty);
        }


        public static void SetOriginalPoint(DependencyObject element, Point value)
        {
            element.SetValue(OriginalPointProperty, value);
        }

        public static Point GetOriginalPoint(DependencyObject element)
        {
            return (Point)element.GetValue(OriginalPointProperty);
        }


        public static void SetVerticalPosition(DependencyObject element, VerticalAlignment value)
        {
            element.SetValue(VerticalPositionProperty, value);
        }

        public static VerticalAlignment GetVerticalPosition(DependencyObject element)
        {
            return (VerticalAlignment)element.GetValue(VerticalPositionProperty);
        }

        //only get messages
        public static ObservableCollection<ItemMessage> GetMessages(DependencyObject element)
        {
            return (ObservableCollection<ItemMessage>)element.GetValue(MessagesProperty);
        }

        public static void SetMessage(Window target, 
                                      object message,
                                      Color? backColor = null,
                                      Color? foreColor = null,
                                      VerticalAlignment position = VerticalAlignment.Bottom,
                                      TimeSpan? timeOut = null) {
            if (GetMessages(target) == null)
                target.SetValue(MessagesProperty, new ObservableCollection<ItemMessage>());

            var item = new ItemMessage(backColor ?? Colors.DimGray, foreColor ?? Colors.Red, message);

            GetMessages(target).Add(item);

            var items = GetMessages(target);

            DispatcherTimer dt = new DispatcherTimer();
            dt.Interval = timeOut == null ? new TimeSpan(0, 0, 3) : (TimeSpan)timeOut;
            dt.Tick += new EventHandler((s, e) =>
            {
                dt.Stop();
                GetMessages(target).Remove(item);
            });
            dt.Start();

            if (GetSetMessageOnClipBoard(target) && message != null) {
                Clipboard.SetText(message.ToString());
            }

            if (GetRegisterMessageOnJournal(target) && message != null) {
                //сдесь регистрируются события в журнал (нужно подумать как подвязать их основным журналом)
            }
        }
    }

    public class ItemMessage {
        public ItemMessage(Color backColor, Color foreColor, object content, Command clickCommand = null) {
            BackColor = backColor;
            ForeColor = foreColor;
            Content = content;
            _clickMessageCommand = clickCommand;
        }

        public Color BackColor { get; private set; }

        public Color ForeColor { get; private set; }

        public object Content { get; private set; }

        private Command _clickMessageCommand;
        public Command ClickMessageCommand {
            get { return _clickMessageCommand ?? (_clickMessageCommand = new Command(obj => {
                WindowMessageBehaviour.GetMessages((Window)obj).Remove(this);
            }, obj => obj is Window)); }
        }
    }
}
