using NTW.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace NTW.Controls.Behaviours
{
    public class MenuActivationBehaviour
    {
        public static readonly DependencyProperty MouseOverProperty = DependencyProperty.RegisterAttached(
            "MouseOver", typeof(bool), typeof(MenuActivationBehaviour), new PropertyMetadata(false));

        public static readonly DependencyProperty CommandActivationProperty = DependencyProperty.RegisterAttached(
            "CommandActivation", typeof(ICommand), typeof(MenuActivationBehaviour), new PropertyMetadata(null, new PropertyChangedCallback((d, a) => {
                if (d is Border)
                {
                    Border b = (Border)d;

                    bool mouseOver = GetMouseOver(b);
                    if (mouseOver) {//использовать mouseEnter и mouseLeave
                        System.Windows.Interactivity.EventTrigger triggerMouseEnter = new System.Windows.Interactivity.EventTrigger();
                        triggerMouseEnter.EventName = "MouseEnter";
                        InvokeCommandAction actionMouseEnter = new InvokeCommandAction { Command = (ICommand)a.NewValue };
                        triggerMouseEnter.Actions.Add(actionMouseEnter);
                        triggerMouseEnter.Attach(b);

                        System.Windows.Interactivity.EventTrigger triggerMouseLeave = new System.Windows.Interactivity.EventTrigger();
                        triggerMouseLeave.EventName = "MouseLeave";
                        InvokeCommandAction actionMouseLeave = new InvokeCommandAction { Command = (ICommand)a.NewValue };
                        triggerMouseLeave.Actions.Add(actionMouseLeave);
                        triggerMouseLeave.Attach(b);
                    }
                    else {//использовать MouseUp
                        System.Windows.Interactivity.EventTrigger triggerMouseLeave = new System.Windows.Interactivity.EventTrigger();
                        triggerMouseLeave.EventName = "MouseUp";
                        InvokeCommandAction actionMouseLeave = new InvokeCommandAction { Command = (ICommand)a.NewValue };
                        triggerMouseLeave.Actions.Add(actionMouseLeave);
                        triggerMouseLeave.Attach(b);
                    }
                }
            })));

        public static readonly DependencyProperty PositionProperty = DependencyProperty.RegisterAttached(
            "Position", typeof(WindowPosition), typeof(MenuActivationBehaviour), new PropertyMetadata(WindowPosition.Non, new PropertyChangedCallback((d, a) => {
                if (d is Border)
                {
                    Border b = (Border)d;

                    switch ((WindowPosition)a.NewValue) {
                        case WindowPosition.Left:
                            b.BorderThickness = new Thickness(2, 0, 0, 0);
                            break;
                        case WindowPosition.Right:
                            b.BorderThickness = new Thickness(0, 0, 2, 0);
                            break;
                        case WindowPosition.Top:
                            b.BorderThickness = new Thickness(0, 2, 0, 0);
                            break;
                        case WindowPosition.Bottom:
                            b.BorderThickness = new Thickness(0, 0, 0, 2);
                            break;

                        case WindowPosition.LeftTop:
                            b.BorderThickness = new Thickness(2, 2, 0, 0);
                            break;
                        case WindowPosition.RightTop:
                            b.BorderThickness = new Thickness(0, 2, 2, 0);
                            break;
                        case WindowPosition.RightBottom:
                            b.BorderThickness = new Thickness(0, 0, 2, 2);
                            break;
                        case WindowPosition.LeftBottom:
                            b.BorderThickness = new Thickness(2, 0, 0, 2);
                            break;

                        case WindowPosition.None:
                            b.BorderThickness = new Thickness(0, 0, 0, 0);
                            break;
                    }
                }
            })));


        public static void SetMouseOver(DependencyObject element, bool value) {
            element.SetValue(MouseOverProperty, value);
        }

        public static bool GetMouseOver(DependencyObject element) {
            return (bool)element.GetValue(MouseOverProperty);
        }


        public static void SetCommandActivation(DependencyObject element, ICommand value) {
            element.SetValue(CommandActivationProperty, value);
        }

        public static ICommand GetCommandActivation(DependencyObject element) {
            return (ICommand)element.GetValue(CommandActivationProperty);
        }

        public static void SetPosition(DependencyObject element, WindowPosition value) {
            element.SetValue(PositionProperty, value);
        }

        public static WindowPosition GetPosition(DependencyObject element) {
            return (WindowPosition)element.GetValue(PositionProperty);
        }
    }
}
