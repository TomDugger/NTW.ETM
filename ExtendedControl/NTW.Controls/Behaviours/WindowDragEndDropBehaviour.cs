using NTW.Controls.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NTW.Controls.Behaviours
{
    public class WindowDragEndDropBehaviour
    {
        #region Drag
        public static readonly DependencyProperty StaysProperty = DependencyProperty.RegisterAttached(
            "Stays", typeof(bool), typeof(WindowDragEndDropBehaviour), new PropertyMetadata(false));

        public static void SetStays(DependencyObject element, bool value)
        {
            element.SetValue(StaysProperty, value);
        }

        public static bool GetStays(DependencyObject element)
        {
            return (bool)element.GetValue(StaysProperty);
        }


        public static readonly DependencyProperty SetItemsCommandProperty = DependencyProperty.RegisterAttached(
            "SetItemsCommand", typeof(ICommand), typeof(WindowDragEndDropBehaviour), new PropertyMetadata(null));

        public static void SetSetItemsCommand(DependencyObject element, ICommand value)
        {
            element.SetValue(SetItemsCommandProperty, value);
        }

        public static ICommand GetSetItemsCommand(DependencyObject element)
        {
            return (ICommand)element.GetValue(SetItemsCommandProperty);
        }


        public static readonly DependencyProperty UseDragProperty = DependencyProperty.RegisterAttached(
            "UseDrag", typeof(Type), typeof(WindowDragEndDropBehaviour), new PropertyMetadata(null, new PropertyChangedCallback((d, a) =>
            {
                if (d is Border)
                {
                    Border b = d as Border;

                    //по факту нужно регистрировать связки типа объекта к перемещаемой область и в соответствии с ним регистрировать список зависимых областей
                    if (a.NewValue != null)
                    {
                        b.PreviewMouseLeftButtonDown += b_MouseLeftButtonDown;
                        b.PreviewMouseMove += b_MouseMove;
                    }
                    else
                    {
                        b.PreviewMouseLeftButtonDown -= b_MouseLeftButtonDown;
                        b.PreviewMouseMove -= b_MouseMove;
                        //стоит предусматреть вариацию при нуле перехват файлов из параметров строки

                    }
                }
                else
                    throw new Exception("UIElement is not type \"Border\"!");
            })));

        private static Point startPoint;
        private static object initObject;

        static void b_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(null);
            initObject = sender;
        }

        static void b_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.MouseDevice.LeftButton == MouseButtonState.Pressed && initObject == sender)
            {
                var currenPoint = e.GetPosition(null);

                if ((startPoint.X > currenPoint.X + 10 || startPoint.X < currenPoint.X - 10) || (startPoint.Y > currenPoint.Y + 10 || startPoint.Y < currenPoint.Y - 10))
                {
                    startPoint = currenPoint;

                    Type typePresentary = GetUseDrag(sender as DependencyObject);

                    //var present = DynamicModuleLambdaCompiler.CreateWindow(typePresentary);
                    var present = (Window)DynamicModuleLambdaCompiler.CreateInstance(typePresentary);

                    if (present != null)
                    {
                        Point p = (sender as Visual).PointToScreen(new Point());
                        //понадобится метрика для расчета текущего положения нового окна


                        //и команда обработки текущего положения
                        ICommand cmdPoint = GetOnPoint((DependencyObject)sender);
                        if (cmdPoint != null)
                            cmdPoint.Execute(new Tuple<object, Point>(((FrameworkElement)sender).DataContext, p));
                        else
                        {
                            present.Top = p.Y;
                            present.Left = p.X;
                        }

                        present.DataContext = (sender as FrameworkElement).DataContext;

                        present.Width = present.Height = (sender as FrameworkElement).ActualWidth;

                        //подключение проверки перемещения окна
                        present.LocationChanged += new EventHandler(present_LocationChanged);

                        present.Focus();
                        present.Topmost = true;

                        if (GetStays(sender as DependencyObject))
                            present.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(present_MouseLeftButtonUp);

                        present.Show();
                        present.DragMove();
                        //получается что сдесь мыш была опущена следовательно объект нужно переместить в область управления
                        foreach (var item in ReceivingElements.Where(x => (x.Value == typePresentary || x.Value == null) && GetIsMouseOver(x.Key)))
                        {
                            IDropViewModel dropViewModel = item.Key.DataContext as IDropViewModel;
                            dropViewModel.SetValues(typePresentary, new object[] { (sender as FrameworkElement).DataContext });
                            dropViewModel.SetValues(item.Key, new object[] { (sender as FrameworkElement).DataContext });

                            SetIsMouseOver(item.Key, false);

                            ICommand cmdSet = GetSetItemsCommand(item.Key);
                            if (cmdSet != null)
                                cmdSet.Execute((sender as FrameworkElement).DataContext);

                            ICommand cmdSet1 = GetSetItemsCommand((DependencyObject)sender);
                            if (cmdSet1 != null)
                                cmdSet1.Execute((sender as FrameworkElement).DataContext);

                            present.Close();
                            break;
                        }

                        //так же забываем про последний сет (не понятно зачем это)
                        //ICommand cmdSetFine = GetSetItemsCommand((DependencyObject)sender);
                        //if (cmdSetFine != null)
                        //    cmdSetFine.Execute(((FrameworkElement)sender).DataContext);

                        //и в итоге если ниодного объекта управления нет проверяем состояние
                        if (!GetStays(sender as DependencyObject))
                            present.Close();
                        else {
                            ICommand cmdSetFine = GetSetItemsCommand((DependencyObject)sender);
                            if (cmdSetFine != null)
                                cmdSetFine.Execute(((FrameworkElement)sender).DataContext);
                        }
                        initObject = null;

                        Console.WriteLine(present.Left + ";" + present.Top);
                    }
                }
            }
        }

        static void present_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            foreach (var item in ReceivingElements.Where(x => (x.Value == sender.GetType() || x.Value == null) && GetIsMouseOver(x.Key)))
            {
                IDropViewModel dropViewModel = item.Key.DataContext as IDropViewModel;
                dropViewModel.SetValues(sender.GetType(), new object[] { (sender as FrameworkElement).DataContext });

                SetIsMouseOver(item.Key, false);

                ICommand cmdSet = GetSetItemsCommand(item.Key);
                if (cmdSet != null)
                    cmdSet.Execute((sender as FrameworkElement).DataContext);

                (sender as Window).Close();
            }
        }

        static void present_LocationChanged(object sender, EventArgs e)
        {
            //просто проверяем соответствие расположения окна относительно типавых родителей размещения
            Window w = sender as Window;

            Rect rect = new Rect(w.Left, w.Top, w.Width, w.Height);

            bool istry = false;

            foreach (var item in ReceivingElements.Where(x => x.Value == w.GetType() || x.Value == null).ToArray())
            {
                if (PresentationSource.FromVisual(item.Key) != null)
                {
                    Rect parent = new Rect(item.Key.PointToScreen(new Point()), new Size((item.Key as FrameworkElement).ActualWidth, (item.Key as FrameworkElement).ActualHeight));

                    if (parent.IntersectsWith(rect))
                    {
                        if (!istry)
                        {
                            SetIsMouseOver(item.Key, true);
                            istry = true;
                        }
                        else
                        {
                            SetIsMouseOver(item.Key, false);
                        }
                    }
                    else
                    {
                        SetIsMouseOver(item.Key, false);
                    }
                }
                else
                    ReceivingElements.Remove(item.Key);
                // подумай как можно убирать элементы без регистрации
            }
        }

        public static void SetUseDrag(DependencyObject element, Type value)
        {
            element.SetValue(UseDragProperty, value);
        }

        public static Type GetUseDrag(DependencyObject element)
        {
            return (Type)element.GetValue(UseDragProperty);
        }

        private static readonly DependencyProperty OnPointCommand = DependencyProperty.RegisterAttached(
            "OnPoint", typeof(ICommand), typeof(WindowDragEndDropBehaviour), new PropertyMetadata(null));

        public static void SetOnPoint(DependencyObject element, ICommand value) {
            element.SetValue(OnPointCommand, value);
        }

        public static ICommand GetOnPoint(DependencyObject element) {
            return (ICommand)element.GetValue(OnPointCommand);
        }
        #endregion

        #region Drop
        private static Dictionary<FrameworkElement, Type> ReceivingElements = new Dictionary<FrameworkElement, Type>();

        public static readonly DependencyProperty UseDropProperty = DependencyProperty.RegisterAttached(
            "UseDrop", typeof(Type), typeof(WindowDragEndDropBehaviour), new PropertyMetadata(typeof(object), new PropertyChangedCallback((d, a) =>
            {
                if (d is UIElement)
                {
                    UIElement ui = (UIElement)d;
                    if ((Type)a.NewValue == typeof(string)) {
                        ui.AllowDrop = true;
                        ui.PreviewDrop += new DragEventHandler(ui_Drop);
                        ui.PreviewDragEnter += new DragEventHandler(ui_DragEnter);
                        ui.PreviewDragLeave += new DragEventHandler(ui_DragLeave);
                    } else //в противном случае выставляем параметры
                        ReceivingElements[d as FrameworkElement] = (Type)a.NewValue;
                }
                else
                    throw new Exception("\"UIElement\"!");
            })));

        private static void ui_DragLeave(object sender, DragEventArgs e)
        {
            SetIsDragEnter(sender as DependencyObject, false);
        }

        private static void ui_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(GetDataFormats(sender as DependencyObject))) e.Effects = DragDropEffects.Copy;
            if (!GetIsDragEnter(sender as DependencyObject))
                SetIsDragEnter(sender as DependencyObject, true);
        }

        static void ui_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            ((sender as FrameworkElement).DataContext as IDropViewModel).SetValues(typeof(string), files);

            SetIsDragEnter(sender as DependencyObject, false);
        }


        public static readonly DependencyProperty IsDragEnterProperty = DependencyProperty.RegisterAttached(
            "IsDragEnter", typeof(bool), typeof(WindowDragEndDropBehaviour), new PropertyMetadata(default(bool)));

        public static void SetIsDragEnter(DependencyObject element, bool value)
        {
            element.SetValue(IsDragEnterProperty, value);
        }

        public static bool GetIsDragEnter(DependencyObject element)
        {
            return (bool)element.GetValue(IsDragEnterProperty);
        }


        public static readonly DependencyProperty DataFormatsProperty = DependencyProperty.RegisterAttached(
            "DataFormats", typeof(string), typeof(WindowDragEndDropBehaviour), new PropertyMetadata(DataFormats.FileDrop));

        public static void SetDataFormats(DependencyObject element, string value)
        {
            element.SetValue(DataFormatsProperty, value);
        }

        public static string GetDataFormats(DependencyObject element)
        {
            return (string)element.GetValue(DataFormatsProperty);
        }


        public static void SetUseDrop(DependencyObject element, Type value)
        {
            element.SetValue(UseDropProperty, value);
        }

        public static Type GetUseDrop(DependencyObject element)
        {
            return (Type)element.GetValue(UseDropProperty);
        }


        public static readonly DependencyProperty IsMouseOverProperty = DependencyProperty.RegisterAttached(
            "IsMouseOver", typeof(bool), typeof(WindowDragEndDropBehaviour), new PropertyMetadata(false));

        public static void SetIsMouseOver(DependencyObject element, bool value)
        {
            element.SetValue(IsMouseOverProperty, value);
        }

        public static bool GetIsMouseOver(DependencyObject element)
        {
            return (bool)element.GetValue(IsMouseOverProperty);
        }
        #endregion

        #region Helps
        public static void ClearParents() {
            ReceivingElements.Clear();
        }
        #endregion

        #region Extentional
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.RegisterAttached(
            "HeaderTemplate", typeof(DataTemplate), typeof(WindowDragEndDropBehaviour), new PropertyMetadata(default(DataTemplate)));

        public static void SetHeaderTemplate(DependencyObject element, DataTemplate value) {
            element.SetValue(HeaderTemplateProperty, value);
        }

        public static DataTemplate GetHeaderTemplate(DependencyObject element) {
            return (DataTemplate)element.GetValue(HeaderTemplateProperty);
        }
        #endregion
    }

    internal static class DynamicModuleLambdaCompiler
    {
        internal static Window CreateWindow(Type typeContent)
        {
            if (typeContent == typeof(Window))
                return new Window();
            //if (typeContent == typeof(IntDrop))
            //    return new IntDrop();
            //else if (typeContent == typeof(StringDrop))
            //    return new StringDrop();
            //else if (typeContent == typeof(ObjectDrop))
            //    return new ObjectDrop();
            else return null;
        }

        public static Func<T> GenerateFactory<T>() where T : new()
        {
            Expression<Func<T>> expr = () => new T();
            NewExpression newExpr = (NewExpression)expr.Body;

            var method = new DynamicMethod(
                name: "lambda",
                returnType: newExpr.Type,
                parameterTypes: new Type[0],
                m: typeof(DynamicModuleLambdaCompiler).Module,
                skipVisibility: true);

            ILGenerator ilGen = method.GetILGenerator();

            if (newExpr.Constructor != null)
            {
                ilGen.Emit(OpCodes.Newobj, newExpr.Constructor);
            }
            else
            {
                LocalBuilder temp = ilGen.DeclareLocal(newExpr.Type);
                ilGen.Emit(OpCodes.Ldloca, temp);
                ilGen.Emit(OpCodes.Initobj, newExpr.Type);
                ilGen.Emit(OpCodes.Ldloc, temp);
            }

            ilGen.Emit(OpCodes.Ret);

            return (Func<T>)method.CreateDelegate(typeof(Func<T>));
        }

        internal static object CreateInstance(Type typeContent) {
            return Activator.CreateInstance(typeContent);
        }
    }
}
