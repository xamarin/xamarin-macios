using System;
using System.Xml.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

#nullable enable

namespace ClassRedirector
{
	public class ClassRedirectorTask : Microsoft.Build.Utilities.Task
	{
		[Required]
		public string InputDirectory { get; set; } = string.Empty;

		[Required]
		public string ClassMapPath { get; set; } = string.Empty;

		[Required]
		public string PlatformAssembly { get; set; } = string.Empty;

		public override bool Execute ()
		{
			if (!Directory.Exists (InputDirectory)) {
				Log.LogError ($"InputDirectory {InputDirectory} doesn't exist.");
				return false;
			}

			if (!DirectoryIsWritable (InputDirectory)) {
				Log.LogError ($"InputDirectory {InputDirectory} is not writable.");
				return false;
			}

			if (!File.Exists (ClassMapPath)) {
				Log.LogError ($"ClassMapPath file {ClassMapPath} does not exist.");
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

			var map = ReadRegistrarFile (ClassMapPath);

			try {
				Log.LogMessage (MessageImportance.Low, $"Redirecting class_handle usage from directory {InputDirectory} in the following dlls: {string.Join (",", dllsToProcess)}");
				Log.LogMessage (MessageImportance.Low, $"Redirecting class_handle usage with the platform dll {xamarinDll}");
				Log.LogMessage (MessageImportance.Low, $"Redirecting class_handle usage with the following {nameof (ClassMapPath)}: {ClassMapPath}");
				var rewriter = new Rewriter (map, xamarinDll, dllsToProcess);
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

