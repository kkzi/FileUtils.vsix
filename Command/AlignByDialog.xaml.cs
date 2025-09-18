using Microsoft.VisualStudio.Text.Editor;
using System.Windows;
using System.Windows.Input;

namespace FileUtil
{
    public partial class AlignByDialog : Window
    {
        private IWpfTextView view;

        public AlignByDialog(IWpfTextView view, string input = "")
        {
            this.view = view;
            InitializeComponent();

            InputText.Text = input;
            InputText.CaretIndex = InputText.Text.Length;
            InputText.Focus();

            PreviewKeyDown += (object sender, KeyEventArgs e) =>
            {
                if (e.Key == Key.Escape)
                    Close();
            };

        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (InputText.Text.Length == 0)
            {
                InputText.Focus();
                return;
            }
            new Alignment(view).ByString(InputText.Text);
            Close();
        }
    }
}
