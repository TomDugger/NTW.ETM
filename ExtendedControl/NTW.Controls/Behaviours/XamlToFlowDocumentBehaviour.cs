using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace NTW.Controls.Behaviours
{
    public class XamlToFlowDocumentBehaviour: DependencyObject
    {
        private static HashSet<Thread> _recursionProtection = new HashSet<Thread>();
        //private static object CurrentElement;

        public static string GetDocumentXaml(DependencyObject obj)
        {
            return (string)obj.GetValue(DocumentXamlProperty);
        }

        public static void SetDocumentXaml(DependencyObject obj, string value)
        {
            _recursionProtection.Add(Thread.CurrentThread);
            obj.SetValue(DocumentXamlProperty, value);
            _recursionProtection.Remove(Thread.CurrentThread);
        }

        public static readonly DependencyProperty DocumentXamlProperty = DependencyProperty.RegisterAttached(
            "DocumentXaml", typeof(string), typeof(XamlToFlowDocumentBehaviour), new FrameworkPropertyMetadata(string.Empty,
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (obj, e) =>
                {
                    if (_recursionProtection.Contains(Thread.CurrentThread))
                       return;
                        //if (CurrentElement == obj)
                        //    return;

                        if (obj is EditableTextBox) {
                        var parent = (EditableTextBox)obj;

                        try {
                            if (GetDocumentXaml(parent) != "" && GetDocumentXaml(parent) != null) {
                                var stream = new MemoryStream(Encoding.UTF8.GetBytes(GetDocumentXaml(parent)));
                                FlowDocument doc = (FlowDocument)XamlReader.Load(stream);
                                parent.EditablePanel.Document = doc;
                            }
                        }
                        catch (Exception ex) {
                            parent.EditablePanel.Document = new FlowDocument();
                        }

                        parent.EditablePanel.TextChanged += (obj2, e2) => {
                            RichTextBox richTextBox2 = obj2 as RichTextBox;
                            if (richTextBox2 != null) {
                                //if (CurrentElement != parent)
                                //    CurrentElement = parent;
                                SetDocumentXaml(parent, XamlWriter.Save(richTextBox2.Document));
                            }
                        };
                    }
                    else if (obj is FlowDocumentPageViewer) {
                        var parent = (FlowDocumentPageViewer)obj;

                        try {
                            if (GetDocumentXaml(parent) != "" && GetDocumentXaml(parent) != null) {
                                var stream = new MemoryStream(Encoding.UTF8.GetBytes(GetDocumentXaml(parent)));
                                var doc = (FlowDocument)XamlReader.Load(stream);

                                parent.Document = doc;
                            }
                        }
                        catch (Exception) {
                            parent.Document = new FlowDocument();
                        }
                    }
                    else if (obj is FlowDocumentScrollViewer) {
                        var parent = (FlowDocumentScrollViewer)obj;

                        try {
                            if (GetDocumentXaml(parent) != "" && GetDocumentXaml(parent) != null) {
                                var stream = new MemoryStream(Encoding.UTF8.GetBytes(GetDocumentXaml(parent)));
                                var doc = (FlowDocument)XamlReader.Load(stream);

                                parent.Document = doc;
                            }
                        }
                        catch (Exception) {
                            parent.Document = new FlowDocument();
                        }
                    }
                }
            )
        );
    }
}
