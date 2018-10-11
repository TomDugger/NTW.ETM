using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NTW.Controls{
    /// <summary>
    /// Логика взаимодействия для ColorControl.xaml
    /// </summary>
    public partial class SizeChangeControl : UserControl {

        public SizeChangeControl()
        {
            DefinitionTemplates = new DefinitionKeySizeCollection();

            InitializeComponent();
        }

        public DefinitionKeySizeCollection DefinitionTemplates {
            get {
                if (GetValue(DefinitionTemplatesProperty) == null)
                    SetValue(DefinitionTemplatesProperty, new DefinitionKeySizeCollection());
                return (DefinitionKeySizeCollection)GetValue(DefinitionTemplatesProperty);
            }
            set { SetValue(DefinitionTemplatesProperty, value); }
        }

        public static readonly DependencyProperty DefinitionTemplatesProperty =
            DependencyProperty.Register("DefinitionTemplates", typeof(DefinitionKeySizeCollection), typeof(SizeChangeControl), new UIPropertyMetadata(null, new PropertyChangedCallback((d, a) =>
            {
                if (a.NewValue != null)
                {
                    SizeChangeControl scc = (SizeChangeControl)d;
                    scc.SizeChanged += Scc_SizeChanged;
                }
            })));

        private static void Scc_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var item in ((SizeChangeControl)sender).DefinitionTemplates)
            {
                if (item.Identification(e.NewSize.Width, e.NewSize.Height))
                {
                    if (((SizeChangeControl)sender).CurrentTemplate != item.Template)
                        ((SizeChangeControl)sender).CurrentTemplate = item.Template;
                    break;
                }
            }
        }


        public ControlTemplate CurrentTemplate {
            get { return (ControlTemplate)GetValue(CurrentTemplateProperty); }
            set { SetValue(CurrentTemplateProperty, value); }
        }

        public static readonly DependencyProperty CurrentTemplateProperty =
            DependencyProperty.Register("CurrentTemplate", typeof(ControlTemplate), typeof(SizeChangeControl), new PropertyMetadata(null));
    }

    public sealed class DefinitionKeySizeCollection : IList<DefinitionKeySize>, ICollection<DefinitionKeySize>, IEnumerable<DefinitionKeySize>, IList, ICollection, IEnumerable
    {
        public DefinitionKeySizeCollection()
        {
            _hash = new List<DefinitionKeySize>();
        }

        private List<DefinitionKeySize> _hash;

        public DefinitionKeySize this[int index] {
            get { return _hash[index]; }
            set { _hash[index] = value; }
        }
        public int Count {
            get { return _hash.Count; }
        }
        public bool IsReadOnly {
            get { return false; }
        }
        public bool IsSynchronized {
            get { return true; }
        }
        public object SyncRoot {
            get;
        }
        public void Add(DefinitionKeySize value)
        {
            _hash.Add(value);
        }
        public void Clear()
        {
            _hash.Clear();
        }
        public bool Contains(DefinitionKeySize value)
        {
            return _hash.Contains(value);
        }
        public void CopyTo(DefinitionKeySize[] array, int index)
        {
            //?
        }
        public int IndexOf(DefinitionKeySize value)
        {
            return _hash.IndexOf(value);
        }
        public void Insert(int index, DefinitionKeySize value)
        {
            _hash.Insert(index, value);
        }
        public bool Remove(DefinitionKeySize value)
        {
            return _hash.Remove(value);
        }
        public void RemoveAt(int index)
        {
            _hash.RemoveAt(index);
        }
        public void RemoveRange(int index, int count)
        {
            _hash.RemoveRange(index, count);
        }


        object IList.this[int index] {
            get { return _hash[index]; }
            set { _hash[index] = (DefinitionKeySize)value; }
        }

        public bool IsFixedSize => false;

        public int Add(object value)
        {
            _hash.Add((DefinitionKeySize)value);
            return _hash.IndexOf((DefinitionKeySize)value);
        }

        public bool Contains(object value)
        {
            return _hash.Contains((DefinitionKeySize)value);
        }

        public void CopyTo(Array array, int index)
        {
            //?
        }

        public IEnumerator<DefinitionKeySize> GetEnumerator()
        {
            return _hash.GetEnumerator();
        }

        public int IndexOf(object value)
        {
            return _hash.IndexOf((DefinitionKeySize)value);
        }

        public void Insert(int index, object value)
        {
            _hash.Insert(index, (DefinitionKeySize)value);
        }

        public void Remove(object value)
        {
            _hash.Remove((DefinitionKeySize)value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _hash.GetEnumerator();
        }
    }

    public struct DefinitionKeySize
    {
        public double Value { get; set; }
        public TypeSizeDifinition TypeKey { get; set; }
        public OrientationKeyDefinition Orientation { get; set; }

        public bool Identification(double width, double height)
        {
            bool result = false;
            switch (Orientation)
            {
                case OrientationKeyDefinition.Left:
                    switch (TypeKey)
                    {
                        case TypeSizeDifinition.Width:
                            if (width < Value)
                                result = true;
                            break;
                        case TypeSizeDifinition.Height:
                            if (height < Value)
                                result = true;
                            break;
                        case TypeSizeDifinition.FullSize:
                            if (height < Value || width < Value)
                                result = true;
                            break;
                    }
                    break;
                case OrientationKeyDefinition.Right:
                    switch (TypeKey)
                    {
                        case TypeSizeDifinition.Width:
                            if (width > Value)
                                result = true;
                            break;
                        case TypeSizeDifinition.Height:
                            if (height > Value)
                                result = true;
                            break;
                        case TypeSizeDifinition.FullSize:
                            if (height > Value || width > Value)
                                result = true;
                            break;
                    }
                    break;
                case OrientationKeyDefinition.Equal:
                    switch (TypeKey)
                    {
                        case TypeSizeDifinition.Width:
                            if (width == Value)
                                result = true;
                            break;
                        case TypeSizeDifinition.Height:
                            if (height == Value)
                                result = true;
                            break;
                        case TypeSizeDifinition.FullSize:
                            if (height == Value || width == Value)
                                result = true;
                            break;
                    }
                    break;
            }
            return result;
        }

        public ControlTemplate Template { get; set; }
    }

    public enum TypeSizeDifinition
    {
        Height,
        Width,
        FullSize
    }

    public enum OrientationKeyDefinition
    {
        Left,
        Right,
        Equal
    }
}
