using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace FileUtil
{
    [Export(typeof(IWpfTextViewMarginProvider))]
    [Name(MarginName)]
    [Order(After = PredefinedMarginNames.Suggestion)]
    [MarginContainer(PredefinedMarginNames.BottomControl)]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal sealed class FilePathFactory : IWpfTextViewMarginProvider
    {
        public const string MarginName = "FilePath";
        public IWpfTextViewMargin CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer)
        {
            ITextDocument document;
            wpfTextViewHost.TextView.TextBuffer.Properties.TryGetProperty(typeof(ITextDocument), out document);
            return new SimpleButtonMargin(MarginName, new FilePathButton(document));
        }
    }

    public partial class FilePathButton : SimpleButton
    {
        private readonly ITextDocument document;
        public FilePathButton(ITextDocument document)
        {
            this.document = document;
            Content = document.FilePath;

            var max = 64;
            if (document.FilePath.Length > max)
            {
                string path = "";
                var parts = document.FilePath.Split('\\');
                for (var i = parts.Length - 1; i > 1; --i)
                {
                    var it = parts[i];
                    if (it.Length > max)
                    {
                        path = "...\\" + it.Substring(0, 18) + "..." + it.Substring(it.Length - 18);
                        break;
                    }
                    path = path.Length == 0 ? it : it + "\\" + path;
                    if (path.Length > max && i > 2)
                    {
                        path = "...\\" + path;
                        break;
                    }
                }
                Content = parts[0] + "\\" + path;
            }


            //FontFamily = 
            MouseUp += OnClicked;
            MouseRightButtonUp += OnRightClicked;
            MouseDoubleClick += OnDoubleClicked;
        }

        private void OnClicked(object sender, RoutedEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                Process.Start(new ProcessStartInfo
                {
                    WorkingDirectory = System.IO.Path.GetDirectoryName(document.FilePath),
                    FileName = "cmd.exe",
                });
            }
            else
            {
                System.Windows.Clipboard.SetText(document.FilePath);
            }
        }

        private void OnRightClicked(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", string.Format("/select, \"{0}\"", document.FilePath));
        }

        private void OnDoubleClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Process.Start("explorer.exe", string.Format("/select, \"{0}\"", document.FilePath));
        }
    }
}
