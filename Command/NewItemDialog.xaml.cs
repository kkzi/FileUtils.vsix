using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FileUtil
{
    /// <summary>
    /// Interaction logic for NewItemDialog.xaml
    /// </summary>
    public partial class NewItemDialog : Window
    {
        private static string[] HEADER_EXTS = { ".h", ".hh", ".hpp", ".hxx", ".ixx" };
        private static string[] SOURCE_EXTS = { ".cc", ".c++", ".cpp" };


        public NewItemDialog(string filepath)
        {
            InitializeComponent();
            if (Directory.Exists(filepath))
            {
                DirPath.Text = filepath;
                ItemName.Text = "";
            }
            else
            {
                DirPath.Text = Path.GetDirectoryName(filepath);
                var name = Path.GetFileNameWithoutExtension(filepath);
                var ext = Path.GetExtension(filepath);
                if (HEADER_EXTS.Contains(ext))
                    ItemName.Text = $"{name}.cpp";
                else if (SOURCE_EXTS.Contains(ext))
                    ItemName.Text = $"{name}.h";
                else
                    ItemName.Text = name;
            }

            ItemName.CaretIndex = ItemName.Text.Length;
            ItemName.Focus();

            PreviewKeyDown += (object sender, KeyEventArgs e) =>
            {
                if (e.Key == Key.Escape)
                    Close();
            };
        }

        public void OkButton_Click(object sender, RoutedEventArgs args)
        {
            if (ItemName.Text.Length == 0)
            {
                ItemName.Focus();
                return;
            }

            DialogResult = true;
            Message.Text = "";
            Close();
        }

        private void ItemName_TextChanged(object sender, TextChangedEventArgs e)
        {
            var path = $"{DirPath.Text}/{ItemName.Text}";
            var exist = Directory.Exists(path) || File.Exists(path);
            Message.Text = exist ? "Directory or file exists" : "";
            Ok.IsEnabled = !exist;
        }
    }
}
