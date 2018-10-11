using NTW.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace NTW.Controls.Behaviours
{
    public class GridDefinitionBehaviour
    {
        public static readonly DependencyProperty ParentPositionProperty = DependencyProperty.RegisterAttached(
            "ParentPosition", typeof(WindowPosition), typeof(GridDefinitionBehaviour), new PropertyMetadata(WindowPosition.Non, new PropertyChangedCallback((d, a) => {
                //в зависимости от списка и элемента выравнивания строить колонки и строки
                if (d is Grid) {
                    Grid g = (Grid)d;
                    g.RowDefinitions.Clear();
                    g.ColumnDefinitions.Clear();
                    WindowPosition wp = (WindowPosition)a.NewValue;
                    bool isnegative = GetIsNegativePosition(d);
                    IEnumerable<GridLength> gls = GetGridDefinitions(d);
                    switch (wp) {
                        case WindowPosition.None:
                            if (!isnegative)
                                foreach (var item in gls)
                                {
                                    g.RowDefinitions.Add(new RowDefinition { Height = item });
                                }
                            else foreach (var item in gls)
                                {
                                    g.ColumnDefinitions.Add(new ColumnDefinition { Width = item });
                                }
                            break;
                        case WindowPosition.Left:
                        case WindowPosition.Right:
                        case WindowPosition.LeftTop:
                        case WindowPosition.LeftBottom:
                        case WindowPosition.RightTop:
                        case WindowPosition.RightBottom:
                            if (!isnegative)
                                foreach (var item in gls)
                                {
                                    g.RowDefinitions.Add(new RowDefinition { Height = item });
                                }
                            else foreach (var item in gls)
                                {
                                    g.ColumnDefinitions.Add(new ColumnDefinition { Width = item });
                                }
                            break;
                        case WindowPosition.Top:
                        case WindowPosition.Bottom:
                            if (!isnegative)
                                foreach (var item in gls)
                                {
                                    g.ColumnDefinitions.Add(new ColumnDefinition { Width = item });
                                }
                            else foreach (var item in gls)
                                {
                                    g.RowDefinitions.Add(new RowDefinition { Height = item });
                                }
                            break;
                    }
                }
            })));

        public static readonly DependencyProperty IsNegativePositionBehaviour = DependencyProperty.RegisterAttached(
            "IsNegativePosition", typeof(bool), typeof(GridDefinitionBehaviour), new PropertyMetadata(false, new PropertyChangedCallback((d, a) => {
                if (d is Grid)
                {
                    Grid g = (Grid)d;
                    g.RowDefinitions.Clear();
                    g.ColumnDefinitions.Clear();
                    bool isnegative = (bool)a.NewValue;
                    WindowPosition wp = GetParentPosition(d);
                    IEnumerable<GridLength> gls = GetGridDefinitions(d);
                    switch (wp)
                    {
                        case WindowPosition.None:
                        case WindowPosition.LeftTop:
                        case WindowPosition.RightTop:
                        case WindowPosition.RightBottom:
                        case WindowPosition.LeftBottom:
                            //пофакту стоим много строничную панель (хз как это поможет)
                            //но пака данный момент опустим
                            if (!isnegative)
                                foreach (var item in gls)
                                {
                                    g.RowDefinitions.Add(new RowDefinition { Height = item });
                                }
                            else foreach (var item in gls)
                                {
                                    g.ColumnDefinitions.Add(new ColumnDefinition { Width = item });
                                }
                            break;
                        case WindowPosition.Left:
                        case WindowPosition.Right:
                            if (!isnegative)
                                foreach (var item in gls)
                                {
                                    g.RowDefinitions.Add(new RowDefinition { Height = item });
                                }
                            else foreach (var item in gls)
                                {
                                    g.ColumnDefinitions.Add(new ColumnDefinition { Width = item });
                                }
                            break;
                        case WindowPosition.Top:
                        case WindowPosition.Bottom:
                            if (!isnegative)
                                foreach (var item in gls)
                                {
                                    g.ColumnDefinitions.Add(new ColumnDefinition { Width = item });
                                }
                            else foreach (var item in gls)
                                {
                                    g.RowDefinitions.Add(new RowDefinition { Height = item });
                                }
                            break;
                    }
                }
            })));

        public static readonly DependencyProperty GridDefinitionsProperty = DependencyProperty.Register(
            "GridDefinitions", typeof(Collection<GridLength>), typeof(Grid));


        public static void SetParentPosition(DependencyObject element, WindowPosition value) {
            element.SetValue(ParentPositionProperty, value);
        }

        public static WindowPosition GetParentPosition(DependencyObject element) {
            return (WindowPosition)element.GetValue(ParentPositionProperty);
        }


        public static void SetIsNegativePosition(DependencyObject element, bool value) {
            element.SetValue(IsNegativePositionBehaviour, value);
        }

        public static bool GetIsNegativePosition(DependencyObject element) {
            return (bool)element.GetValue(IsNegativePositionBehaviour);
        }


        public static void SetGridDefinitions(DependencyObject element, Collection<GridLength> value) {
            element.SetValue(GridDefinitionsProperty, value);
        }

        public static Collection<GridLength> GetGridDefinitions(DependencyObject element) {

            Collection<GridLength> temp = (Collection<GridLength>)element.GetValue(GridDefinitionsProperty);
            if (temp == null)
                element.SetValue(GridDefinitionsProperty, new Collection<GridLength>());

            return (Collection<GridLength>)element.GetValue(GridDefinitionsProperty);
        }
    }
}
