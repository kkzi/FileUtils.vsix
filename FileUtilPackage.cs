using EnvDTE;
using EnvDTE80;
using Microsoft;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace FileUtil
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class FileUtilPackage : AsyncPackage
    {
        public const string PackageGuidString = "e4242fd4-6c82-4213-83bd-592a5903ad35";
        internal static DTE2 Dte { get; private set; }
        internal static IVsMonitorSelection MonitorSelection { get; private set; }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            Dte = await GetServiceAsync(typeof(EnvDTE.DTE)) as DTE2;
            //DTE2 dte = (DTE2)Package.GetGlobalService(typeof(DTE));
            Assumes.Present(Dte);

            MonitorSelection = await GetServiceAsync(typeof(IVsMonitorSelection)) as IVsMonitorSelection;
            Assumes.Present(MonitorSelection);

            await NewItemCmd.InitializeAsync(this);
            await AlignByCmd.InitializeAsync(this);
        }
    }
}
