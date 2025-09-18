using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace FileUtil
{

    [Export(typeof(IWpfTextViewMarginProvider))]
    [Name(MarginName)]
    [Order(Before = PredefinedMarginNames.Spacer, After = PredefinedMarginNames.LineNumber)]
    [MarginContainer(PredefinedMarginNames.BottomRightCorner)]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal sealed class FileEncodingFactory : IWpfTextViewMarginProvider
    {
        public const string MarginName = "FileEncoding";
        public IWpfTextViewMargin CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer)
        {
            ITextDocument document;
            wpfTextViewHost.TextView.TextBuffer.Properties.TryGetProperty(typeof(ITextDocument), out document);
            return new SimpleButtonMargin(MarginName, new FileEncodingButton(document));
        }
    }

    internal class EncodingUtil
    {
        internal static bool HasBom(Encoding encoding)
        {
            return encoding.GetPreamble().Length != 0;
        }
        internal static bool IsSameEncoding(Encoding left, Encoding right)
        {
            return left.CodePage == right.CodePage && HasBom(left) == HasBom(right);
        }

        internal static string GetEncodingName(Encoding enc)
        {
            int codepage = enc.CodePage;
            if (codepage == Encoding.UTF8.CodePage)
            {
                return HasBom(enc) ? "UTF-8 (BOM)" : "UTF-8";
            }
            else
            {
                return enc.HeaderName.ToUpper();
            }
        }
    }

    internal class ConvertCommand : ICommand
    {
        private readonly Encoding encoding;
        public ConvertCommand(Encoding encoding)
        {
            this.encoding = encoding;
        }

        event EventHandler ICommand.CanExecuteChanged
        {
            add { }
            remove { }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var target = parameter as FileEncodingButton;
            var doc = target.Document;
            if (!EncodingUtil.IsSameEncoding(doc.Encoding, encoding))
            {
                doc.Encoding = encoding;
                doc.UpdateDirtyState(true, DateTime.Now);
                target.Content = EncodingUtil.GetEncodingName(encoding);
            }
        }
    }


    public partial class FileEncodingButton : SimpleButton
    {
        public ITextDocument Document;

        public FileEncodingButton(ITextDocument document)
        {
            this.Document = document;
            this.Document.FileActionOccurred += (sender, e) => Content = EncodingUtil.GetEncodingName(document.Encoding);
            Content = EncodingUtil.GetEncodingName(document.Encoding);

            InitContextMenu();
            MouseUp += OnClicked;
        }

        private void InitContextMenu()
        {
            ContextMenu = new ContextMenu();
            ContextMenu.PlacementTarget = this;
            ContextMenu.Placement = PlacementMode.Top;

            var encodings = new Dictionary<string, Encoding> {
                {"UTF8", new UTF8Encoding(false)},
                {"UTF8-BOM", Encoding.UTF8 },
                {"GB2312", Encoding.GetEncoding("GB2312") },
            };
            foreach (var it in encodings)
            {
                ContextMenu.Items.Add(new MenuItem
                {
                    Header = it.Key,
                    Command = new ConvertCommand(it.Value),
                    CommandParameter = this,
                });
            }
        }

        private void OnClicked(object sender, RoutedEventArgs e)
        {
            foreach (MenuItem item in ContextMenu.Items)
            {
                item.IsChecked = Content.Equals(item.Header);
            }
            ContextMenu.IsOpen = true;
        }
    }
}
