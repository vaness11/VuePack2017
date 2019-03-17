using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using VuePack.Helpers;

namespace VuePack
{
	[Export(typeof(IVsTextViewCreationListener))]
	[ContentType("json")]
	[ContentType("htmlx")]
	[TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
	class ComponentsCache : IVsTextViewCreationListener
	{
		private static bool _hasRun, _isProcessing;
		private static ConcurrentDictionary<string, ComponentData> _cache = 
			new ConcurrentDictionary<string, ComponentData>();
		public static ConcurrentDictionary<string, ComponentData> GetCache() => _cache;

		[Import]
		public IVsEditorAdaptersFactoryService EditorAdaptersFactoryService { get; set; }

		[Import]
		public ITextDocumentFactoryService TextDocumentFactoryService { get; set; }

		public void VsTextViewCreated(IVsTextView textViewAdapter)
		{
			if (_hasRun || _isProcessing)
				return;

			var textView = EditorAdaptersFactoryService.GetWpfTextView(textViewAdapter);

			ITextDocument doc;

			if (TextDocumentFactoryService.TryGetTextDocument(textView.TextDataModel.DocumentBuffer, out doc))
			{
				if (Path.IsPathRooted(doc.FilePath) && File.Exists(doc.FilePath))
				{
					_isProcessing = true;
					var dte = (DTE2)Package.GetGlobalService(typeof(DTE));
					var item = dte.Solution?.FindProjectItem(doc.FilePath);
					//System.Threading.Tasks.Task.Run(() =>
					{
						EnsureInitialized(item);
						_hasRun = _isProcessing = false;
					}//);
				}
			}
		}

		public static void Clear()
		{
			_hasRun = false;
			_cache.Clear();
		}

		private void EnsureInitialized(ProjectItem item)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			if (item == null || item.ContainingProject == null)
				return;
			ProjectHelpers.Log("Looking for Vue component definitions...");

			string folder = item.ContainingProject.GetRootFolder();
			var allFiles = GetFiles(folder, "*.vdef.json");

			foreach (string file in allFiles.ToArray())
			{
				try
				{
					ProcessFile(file);
					ProjectHelpers.Log("Processed file: " + file);
				}
				catch (Exception ex)
				{
					ProjectHelpers.Log(
						"Couldn't parse a file: " + file
						+ Environment.NewLine
						+ ex.ToString()
					);
				}
			}
		}

		public static void ProcessFile(string file)
		{
			string content = File.ReadAllText(file);
			var data = ComponentDataParser.Parse(content);
			foreach(var item in data)
				_cache[item.Name] = item;
		}

		private static List<string> GetFiles(string path, string pattern)
		{
			var files = new List<string>();

			if (path.Contains("node_modules"))
				return files;

			try
			{
				files.AddRange(Directory.GetFiles(path, pattern, SearchOption.TopDirectoryOnly));
				foreach (var directory in Directory.GetDirectories(path))
					files.AddRange(GetFiles(directory, pattern));
			}
			catch { }

			return files;
		}
	}
}
