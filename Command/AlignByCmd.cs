using Microsoft;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.ComponentModel.Design;
using Task = System.Threading.Tasks.Task;

namespace FileUtil
{
    internal sealed class AlignByCmd
    {
        public const int CommandId = 0x0101;
        public static readonly Guid CommandSet = new Guid("96d5b6c2-b20e-4bb4-872f-1f7dd49f16b2");
        private static IVsTextManager txtmgr = null;
        private static IComponentModel model = null;

        public string ScopeSelectorLineValues { get; set; }
        public string ScopeSelectorLineEnds { get; set; }

        private AlignByCmd(AsyncPackage package, OleMenuCommandService commandService)
        {
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        public static AlignByCmd Instance
        {
            get;
            private set;
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            txtmgr = await package.GetServiceAsync(typeof(SVsTextManager)) as IVsTextManager;
            Assumes.Present(txtmgr);

            model = await package.GetServiceAsync(typeof(SComponentModel)) as IComponentModel;
            Assumes.Present(model);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new AlignByCmd(package, commandService);
        }

        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var view = GetCurrentView();
            if (view == null) return;

            var dialog = new AlignByDialog(view);
            dialog.ShowDialog();
        }

        private IWpfTextView GetCurrentView()
        {
            if (txtmgr == null || model == null) return null;

            IVsTextView view;
            txtmgr.GetActiveView(1, null, out view);
            if (view == null) return null;

            var factory = model.GetService<IVsEditorAdaptersFactoryService>();
            return factory.GetWpfTextView(view);
        }
    }
}
