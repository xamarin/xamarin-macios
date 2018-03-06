using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using Parallel = System.Threading.Tasks.Parallel;
using ParallelOptions = System.Threading.Tasks.ParallelOptions;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev;
using Xamarin.MacDev.Tasks;

namespace Xamarin.iOS.Tasks
{
	public abstract class CodesignNativeLibrariesTaskBase : Task
	{
		const string ToolName = "codesign";
		string toolExe;

		#region Inputs

		public string SessionId { get; set; }

		[Required]
		public string AppBundleDir { get; set; }

		[Required]
		public string IntermediateOutputPath { get; set; }

		[Required]
		public string CodesignAllocate { get; set; }

		public bool DisableTimestamp { get; set; }

		public string Keychain { get; set; }

		[Required]
		public string SigningKey { get; set; }

		public string ExtraArgs { get; set; }

		public string ToolExe {
			get { return toolExe ?? ToolName; }
			set { toolExe = value; }
		}

		public string ToolPath { get; set; }

		#endregion

		string GetFullPathToTool ()
		{
			if (!string.IsNullOrEmpty (ToolPath))
				return Path.Combine (ToolPath, ToolExe);

			var path = Path.Combine ("/usr/bin", ToolExe);

			return File.Exists (path) ? path : ToolExe;
		}

		ProcessStartInfo GetProcessStartInfo (string tool, string args)
		{
			var startInfo = new ProcessStartInfo (tool, args);

			startInfo.WorkingDirectory = Environment.CurrentDirectory;
			startInfo.EnvironmentVariables["CODESIGN_ALLOCATE"] = CodesignAllocate;

			startInfo.CreateNoWindow = true;

			return startInfo;
		}

		string GenerateCommandLineArguments (string dylib)
		{
			var args = new CommandLineArgumentBuilder ();

			args.Add ("-v");
			args.Add ("--force");

			args.Add ("--sign");
			args.AddQuoted (SigningKey);

			if (!string.IsNullOrEmpty (Keychain)) {
				args.Add ("--keychain");
				args.AddQuoted (Path.GetFullPath (Keychain));
			}

			if (DisableTimestamp)
				args.Add ("--timestamp=none");

			if (!string.IsNullOrEmpty (ExtraArgs))
				args.Add (ExtraArgs);

			args.AddQuoted (dylib);

			return args.ToString ();
		}

		void Codesign (string dylib)
		{
			var startInfo = GetProcessStartInfo (GetFullPathToTool (), GenerateCommandLineArguments (dylib));
			var messages = new StringBuilder ();
			var errors = new StringBuilder ();
			int exitCode;

			try {
				Log.LogMessage (MessageImportance.Normal, "Tool {0} execution started with arguments: {1}", startInfo.FileName, startInfo.Arguments);

				using (var stdout = new StringWriter (messages)) {
					using (var stderr = new StringWriter (errors)) {
						var process = ProcessUtils.StartProcess (startInfo, stdout, stderr);

						process.Wait ();

						exitCode = process.Result;
					}

					Log.LogMessage (MessageImportance.Low, "Tool {0} execution finished (exit code = {1}).", startInfo.FileName, exitCode);
				}
			} catch (Exception ex) {
				Log.LogError ("Error executing tool '{0}': {1}", startInfo.FileName, ex.Message);
				return;
			}

			if (messages.Length > 0)
				Log.LogMessage (MessageImportance.Normal, "{0}", messages.ToString ());

			if (exitCode != 0) {
				if (errors.Length > 0)
					Log.LogError ("Failed to codesign '{0}': {1}", dylib, errors);
				else
					Log.LogError ("Failed to codesign '{0}'", dylib);
			} else {
				var output = GetOutputPath (dylib);
				var dir = Path.GetDirectoryName (output);

				Directory.CreateDirectory (dir);

				File.WriteAllText (output, string.Empty);
			}
		}

		string GetOutputPath (string path)
		{
			var rpath = PathUtils.AbsoluteToRelative (AppBundleDir, path);

			return Path.Combine (IntermediateOutputPath, rpath);
		}

		bool NeedsCodesign (string path)
		{
			var output = GetOutputPath (path);

			return !File.Exists (output) || File.GetLastWriteTimeUtc (path) >= File.GetLastWriteTimeUtc (output);
		}

		public override bool Execute ()
		{
			if (!Directory.Exists (AppBundleDir))
				return true;

			var subdirs = new List<string> ();
			var dylibs = new List<string> ();

			foreach (var path in Directory.EnumerateFileSystemEntries (AppBundleDir)) {
				if (Directory.Exists (path)) {
					var name = Path.GetFileName (path);

					if (name != "PlugIns" && name != "Watch")
						subdirs.Add (path);
				} else {
					if (path.EndsWith (".metallib", StringComparison.Ordinal) || path.EndsWith (".dylib", StringComparison.Ordinal)) {
						if (NeedsCodesign (path))
							dylibs.Add (path);
					}
				}
			}

			foreach (var subdir in subdirs) {
				foreach (var dylib in Directory.EnumerateFiles (subdir, "*.*", SearchOption.AllDirectories)) {
					if (dylib.EndsWith (".metallib", StringComparison.Ordinal) || dylib.EndsWith (".dylib", StringComparison.Ordinal)) {
						if (NeedsCodesign (dylib))
							dylibs.Add (dylib);
					}
				}
			}

			if (dylibs.Count == 0)
				return true;

			Parallel.ForEach (dylibs, new ParallelOptions { MaxDegreeOfParallelism = Math.Max (Environment.ProcessorCount / 2, 1) }, (dylib) => {
				Codesign (dylib);
			});

			return !Log.HasLoggedErrors;
		}
	}
}
