using System;
using System.Xml.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

#nullable enable

namespace ClassRedirector
{
	public class ClassRedirectorTask : Microsoft.Build.Utilities.Task
	{
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

			var dllsToProcess = CollectDlls (InputDirectory);
			var xamarinDll = FindXamarinDll (dllsToProcess);

			if (xamarinDll is null) {
				Log.LogError ($"unable to find platform dll in {InputDirectory}");
				return false;
			}

			var map = ReadRegistrarFile (ClassMapPath);

			try {
				Log.LogMessage ($"Redirecting class_handle usage from directory {InputDirectory} in the following dlls: {string.Join (",", dllsToProcess)}");
				Log.LogMessage ($"Redirecting class_handle usage with the platform dll {xamarinDll}");
				Log.LogMessage ($"Redirecting class_handle usage with the following {nameof (ClassMapPath)}: {ClassMapPath}");
				var rewriter = new Rewriter (map, xamarinDll, dllsToProcess);
				rewriter.Process ();
			} catch (Exception e) {
				Log.LogErrorFromException (e);
			}

			return true;
		}

		[Required]
		public string InputDirectory { get; set; } = "";

		[Required]
		public string ClassMapPath { get; set; } = "";

		static bool DirectoryIsWritable (string path)
		{
			var info = new DirectoryInfo (path);
			return !info.Attributes.HasFlag (FileAttributes.ReadOnly);
		}

		static string [] CollectDlls (string dir)
		{
			return Directory.GetFiles (dir, "*.dll"); // GetFiles returns full paths
		}

		static string [] xamarinDlls = new string [] {
			"Microsoft.iOS.dll",
			"Microsoft.macOS.dll",
			"Microsoft.tvOS.dll",
		};

		static bool IsXamarinDll (string p)
		{
			return xamarinDlls.FirstOrDefault (dll => p.EndsWith (dll, StringComparison.Ordinal)) is not null;
		}

		static string? FindXamarinDll (string [] paths)
		{
			return paths.FirstOrDefault (IsXamarinDll);
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

