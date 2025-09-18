using EnvDTE;
using EnvDTE80;
using Microsoft;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.IO;
using Task = System.Threading.Tasks.Task;
using static FileUtil.FileUtilPackage;

namespace FileUtil
{
    internal sealed class NewItemCmd
    {
        public const int CommandId = 0x0100;
        public static readonly Guid CommandSet = new Guid("96d5b6c2-b20e-4bb4-872f-1f7dd49f16b2");

        private NewItemCmd(AsyncPackage package, OleMenuCommandService commandService)
        {
            package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        public static NewItemCmd Instance
        {
            get;
            private set;
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new NewItemCmd(package, commandService);
        }

        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            string current_file = Dte.ActiveWindow.Kind == "Document" ? Dte.ActiveDocument.FullName : SolutionExplorer.TryGetSingleSelectedFile();
            if (current_file.Length == 0)
                current_file = Dte.Solution.FileName;

            var dialog = new NewItemDialog(current_file);
            if (!dialog.ShowDialog().GetValueOrDefault(false))
                return;

            var dirpath = dialog.DirPath.Text;
            var itemname = dialog.ItemName.Text;
            var isdir = itemname.EndsWith("/") || itemname.EndsWith("\\");
            var path = Path.Combine(dirpath, itemname);
            if (isdir) CreateNewDir(path);
            else CreateAndOpenFile(path);
        }

        private void CreateAndOpenFile(string path)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            CreateNewDir(Path.GetDirectoryName(path));
            File.Create(path).Close();
            _ = Dte.ItemOperations.OpenFile(path);
        }

        private void CreateNewDir(string path)
        {
            Directory.CreateDirectory(path);
        }
    }
}
