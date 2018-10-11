using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace NTW.Controls {
    public class LastItemFillPanel : Panel {
        class MeasureArrangeHelper {
            public Size TotalSize = new Size();
            public Size CurrentLineSize = new Size();
            public List<UIElement> CurrentLine;

            double availableWidth;
            bool keepChildren;

            public MeasureArrangeHelper(double availableWidth, bool keepChildren) {
                this.availableWidth = availableWidth;
                this.keepChildren = keepChildren;
                if (keepChildren)
                    CurrentLine = new List<UIElement>();
            }

            public void AddToCurrentLine(UIElement child) {
                var size = child.DesiredSize;
                CurrentLineSize.Width += size.Width;
                CurrentLineSize.Height = Math.Max(size.Height, CurrentLineSize.Height);
                if (keepChildren)
                    CurrentLine.Add(child);
            }

            public bool CanAddToCurrentLine(UIElement child) {
                return CurrentLineSize.Width + child.DesiredSize.Width <= availableWidth;
            }

            public void FinishLine(bool stretchLast = false) {
                if (keepChildren)
                    ArrangeLine(TotalSize.Height, CurrentLineSize.Height, CurrentLine, stretchLast);
                TotalSize.Width = Math.Max(CurrentLineSize.Width, TotalSize.Width);
                TotalSize.Height += CurrentLineSize.Height;
                CurrentLineSize = new Size();
                if (keepChildren)
                    CurrentLine.Clear();
            }

            void ArrangeLine(double height, double lineHeight, List<UIElement> children, bool stretchLast) {
                double width = 0;

                var childToStretch = stretchLast ? children.LastOrDefault() : null;
                foreach (var child in children) {
                    double layoutSlotWidth = child.DesiredSize.Width;
                    if (child == childToStretch)
                        layoutSlotWidth = Math.Max(layoutSlotWidth, availableWidth - width);
                    child.Arrange(new Rect(width, height, layoutSlotWidth, lineHeight));
                    width += layoutSlotWidth;
                }
            }
        }

        protected override Size MeasureOverride(Size availableSize) {
            return CommonMeasureArrange(availableSize, false);
        }

        protected override Size ArrangeOverride(Size finalSize) {
            CommonMeasureArrange(finalSize, true);
            return finalSize;
        }

        Size CommonMeasureArrange(Size size, bool arrange) {
            var helper = new MeasureArrangeHelper(size.Width, keepChildren: arrange);

            foreach (var child in InternalChildren.OfType<UIElement>()) {
                if (!arrange)
                    child.Measure(size);

                if (helper.CanAddToCurrentLine(child)) {
                    helper.AddToCurrentLine(child);
                }
                else // переходим на следующую строку
                {
                    helper.FinishLine();

                    var overflow = !helper.CanAddToCurrentLine(child);
                    helper.AddToCurrentLine(child);
                    if (overflow) // больше заданной ширины -- получает отдельную строку
                        helper.FinishLine();
                }
            }

            // не забываем последнюю строку
            if (helper.CurrentLine == null || helper.CurrentLine.Count > 0)
                helper.FinishLine(true);

            return helper.TotalSize;
        }
    }

    class LWrapPanel : WrapPanel {
        
        protected override Size ArrangeOverride(Size finalSize) {
            return base.ArrangeOverride(finalSize);
        }
    }
}
