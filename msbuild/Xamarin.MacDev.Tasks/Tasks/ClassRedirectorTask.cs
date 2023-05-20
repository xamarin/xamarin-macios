using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using ClassRedirector;

using Xamarin.Messaging.Build.Client;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public class ClassRedirector : XamarinTask, ITaskCallback {
		[Required]
		public string InputDirectory { get; set; } = string.Empty;

		[Required]
		public string OutputDirectory { get; set; } = string.Empty;

		[Required]
		public ITaskItem? ClassMapPath { get; set; }

		[Required]
		public string PlatformAssembly { get; set; } = string.Empty;

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ()) {
				var taskRunner = new TaskRunner (SessionId, BuildEngine4);
				return taskRunner.RunAsync (this).Result;
			}

			return ExecuteLocally ();
		}

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied ()
		{
			if (!Directory.Exists (InputDirectory))
				return Enumerable.Empty<ITaskItem> ();

			return Directory.GetFiles (InputDirectory, "*", SearchOption.AllDirectories)
				.Select (f => new TaskItem (f));
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => true;

		public bool ShouldCreateOutputFile (ITaskItem item) => true;

		bool ExecuteLocally ()
		{
			if (!Directory.Exists (InputDirectory)) {
				Log.LogError ($"InputDirectory {InputDirectory} doesn't exist.");
				return false;
			}

			if (InputDirectory == OutputDirectory) {
				Log.LogError ($"OutputDirectory {OutputDirectory} must be difference from InputDirectory.");
				return false;
			}

			if (!Directory.Exists (OutputDirectory)) {
				try {
					Directory.CreateDirectory (OutputDirectory);
				} catch (Exception directoryException) {
					Log.LogErrorFromException (directoryException);
				}
			}

			if (!DirectoryIsWritable (OutputDirectory)) {
				Log.LogError ($"OutputDirectory {OutputDirectory} is not writable.");
				return false;
			}

			var classMapPath = ClassMapPath!.ItemSpec;
			if (!File.Exists (classMapPath)) {
				Log.LogError ($"ClassMapPath file {classMapPath} does not exist.");
				return false;
			}

			var xamarinDll = PlatformAssembly;

			if (!File.Exists (xamarinDll))
				xamarinDll = Path.Combine (InputDirectory, PlatformAssembly);

			if (!File.Exists (xamarinDll)) {
				Log.LogError ($"PlatformAssembly {PlatformAssembly} does not exist as is or in {InputDirectory}");
				return false;
			}



			var dllsToProcess = CollectDlls (InputDirectory);

			var map = ReadRegistrarFile (classMapPath);

			try {
				Log.LogMessage (MessageImportance.Low, $"Redirecting class_handle usage from directory {InputDirectory} in the following dlls: {string.Join (",", dllsToProcess)}");
				Log.LogMessage (MessageImportance.Low, $"Redirecting class_handle usage with the platform dll {xamarinDll}");
				Log.LogMessage (MessageImportance.Low, $"Redirecting class_handle usage with the following {nameof (ClassMapPath)}: {classMapPath}");
				var rewriter = new Rewriter (map, xamarinDll, dllsToProcess, OutputDirectory);
				rewriter.Process ();
			} catch (Exception e) {
				Log.LogErrorFromException (e);
				return false;
			}

			return true;
		}

		static bool DirectoryIsWritable (string path)
		{
			var info = new DirectoryInfo (path);
			return !info.Attributes.HasFlag (FileAttributes.ReadOnly);
		}

		static string [] CollectDlls (string dir)
		{
			return Directory.GetFiles (dir, "*.dll"); // GetFiles returns full paths
		}

		static CSToObjCMap ReadRegistrarFile (string path)
		{
			var doc = XDocument.Load (path);
			var map = CSToObjCMap.FromXDocument (doc);
			if (map is null)
				throw new Exception ($"Unable to read static registrar map file {path}");
			return map;
		}
	}
}

