using System;
using System.IO;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace VuePack
{
    static class ProjectHelpers
    {
        public static string GetRootFolder(this Project project)
        {
            if (string.IsNullOrEmpty(project.FullName))
                return null;

            string fullPath;

            try
            {
                fullPath = project.Properties.Item("FullPath").Value as string;
            }
            catch (ArgumentException)
            {
                try
                {
                    // MFC projects don't have FullPath, and there seems to be no way to query existence
                    fullPath = project.Properties.Item("ProjectDirectory").Value as string;
                }
                catch (ArgumentException)
                {
                    // Installer projects have a ProjectPath.
                    fullPath = project.Properties.Item("ProjectPath").Value as string;
                }
            }

            if (string.IsNullOrEmpty(fullPath))
                return File.Exists(project.FullName) ? Path.GetDirectoryName(project.FullName) : null;

            if (Directory.Exists(fullPath))
                return fullPath;

            if (File.Exists(fullPath))
                return Path.GetDirectoryName(fullPath);

            return null;
        }

		public static void Log(string text)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			const int VISIBLE = 1;
			const int DO_NOT_CLEAR_WITH_SOLUTION = 0;

			// Get the output window
			var outputWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
			int hr;

			var guidPane = Microsoft.VisualStudio.VSConstants.OutputWindowPaneGuid.GeneralPane_guid;
			hr = outputWindow.CreatePane(guidPane, "General", VISIBLE, DO_NOT_CLEAR_WITH_SOLUTION);
			Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr);

			// Get the pane
			IVsOutputWindowPane outputWindowPane = null;
			hr = outputWindow.GetPane(guidPane, out outputWindowPane);
			Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr);

			// Output the text
			if (outputWindowPane != null)
			{
				outputWindowPane.Activate();
				outputWindowPane.OutputString("Vue2Pack: " + text + Environment.NewLine);
			}
		}
	}
}
