using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Build.Framework;

using Xamarin.Messaging.Build.Client;
using Xamarin.Utils;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public class InstallNameTool : XamarinTask, ITaskCallback {
		public string InstallNameToolPath { get; set; } = "";

		[Required]
		public ITaskItem [] DynamicLibrary { get; set; } = [];

		// The intended output directory for reidentified native libraries. Used to make sure we never
		// write outside this directory, even if the reidentified path was influenced by metadata that
		// originates from a (passive) binding resource package manifest.
		[Required]
		public string IntermediateNativeLibraryDir { get; set; } = "";

		// This isn't consumed from the targets files, but it's needed for VSX to create corresponding
		// files on Windows.
		[Output]
		public ITaskItem [] ReidentifiedDynamicLibrary { get; set; } = [];

		// Whether references between libraries in the list of dynamic libraries should be updated.
		public bool FixupLibraryReferences { get; set; }

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return ExecuteRemotely ();

			var processes = new List<Task> ();
			var reidentified = new List<ITaskItem> ();

			var changes = new List<string> ();
			if (FixupLibraryReferences) {
				foreach (var lib in DynamicLibrary) {
					var name = Path.GetFileName (lib.ItemSpec);
					var fw = Path.GetFileNameWithoutExtension (name);
					changes.Add ("-change");
					changes.Add ($"@rpath/{name}");
					changes.Add ($"@rpath/{fw}.framework/{fw}");
				}
			}

			for (var i = 0; i < DynamicLibrary.Length; i++) {
				var input = DynamicLibrary [i];
				var src = Path.GetFullPath (input.ItemSpec);
				// Make sure we use the correct path separator, these are relative paths, so it doesn't look
				// like MSBuild does the conversion automatically.
				var target = input.GetMetadata ("ReidentifiedPath").Replace ('\\', Path.DirectorySeparatorChar);

				// Defense-in-depth: the 'ReidentifiedPath' can be influenced by metadata that originates
				// from a (passive) binding resource package manifest. Make sure we
				// never create directories or write files outside the intended intermediate output
				// directory, even if the path contains traversal segments, is absolute, or uses symlinks.
				if (!PathUtils.IsPathContained (IntermediateNativeLibraryDir, target)) {
					Log.LogError (MSBStrings.E7181 /* The native library can't be reidentified to '{0}' because that path is outside the intended output directory '{1}'. */, target, IntermediateNativeLibraryDir);
					continue;
				}

				var temporaryTarget = target + ".tmp";

				var arguments = new List<string> ();
				arguments.Add ("-id");
				arguments.Add (input.GetMetadata ("UpdatedId"));
				arguments.AddRange (changes);

				arguments.Add (temporaryTarget);
				var executable = GetExecutable (arguments, "install_name_tool", InstallNameToolPath);

				var stampFile = input.GetMetadata ("StampFile").Replace ('\\', Path.DirectorySeparatorChar);
				var stampContents = "";
				if (!string.IsNullOrEmpty (stampFile) && IsUpToDate (src, target, executable, arguments, stampFile, out stampContents))
					continue;

				// install_name_tool modifies the file in-place, so copy it to a temporary file first.
				Directory.CreateDirectory (Path.GetDirectoryName (temporaryTarget)!);
				File.Copy (src, temporaryTarget, true);
				processes.Add (ExecuteAsync (executable, arguments).ContinueWith ((v) => {
					if (v.IsFaulted) {
						// install_name_tool faulted; don't leave the temporary copy behind.
						File.Delete (temporaryTarget);
						throw v.Exception;
					}
					if (v.Status == TaskStatus.RanToCompletion && v.Result.ExitCode == 0) {
						File.Delete (target);
						File.Move (temporaryTarget, target);

						if (!string.IsNullOrEmpty (stampFile)) {
							Directory.CreateDirectory (Path.GetDirectoryName (stampFile)!);
							File.WriteAllText (stampFile, stampContents);
						}
					} else {
						// install_name_tool failed; don't leave the temporary copy behind.
						File.Delete (temporaryTarget);
					}
				}));

				reidentified.Add (new Microsoft.Build.Utilities.TaskItem (target));
			}

			if (processes.Count > 0)
				Task.WaitAll (processes.ToArray ());

			ReidentifiedDynamicLibrary = reidentified.ToArray ();

			return !Log.HasLoggedErrors;
		}

		bool IsUpToDate (string sourceLibrary, string destinationLibrary, string executable, List<string> arguments, string stampFile, out string stampFileContents)
		{
			stampFileContents = executable + "\n" + string.Join ("\n", arguments);

			if (!File.Exists (destinationLibrary)) {
				Log.LogMessage (MessageImportance.Low, "The destination library '{0}' is not up-to-date, because it doesn't exist.", destinationLibrary);
				return false;
			}

			var srcDate = File.GetLastWriteTimeUtc (sourceLibrary);
			var destDate = File.GetLastWriteTimeUtc (destinationLibrary);
			if (destDate < srcDate) {
				Log.LogMessage (MessageImportance.Low, "The destination library '{0}' is not up-to-date, its timestamp ({1}) is earlier than the timestamp of the source library '{2}: {3}.", destinationLibrary, destDate, sourceLibrary, srcDate);
				return false;
			}

			if (!File.Exists (stampFile)) {
				Log.LogMessage (MessageImportance.Low, "The destination library '{0}' is not up-to-date, its stamp file ({1}) does not exist.", destinationLibrary, stampFile);
				return false;
			}

			var stampContents = File.ReadAllText (stampFile);
			if (stampContents != stampFileContents) {
				Log.LogMessage (MessageImportance.Low, "The destination library '{0}' is not up-to-date, because the contents of the stamp file ({1}) changed.", destinationLibrary, stampFile);
				return false;
			}

			Log.LogMessage (MessageImportance.Low, "The destination library '{0}' is up-to-date.", destinationLibrary);
			return true;
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => true;
		public bool ShouldCreateOutputFile (ITaskItem item) => true;
		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied () => Enumerable.Empty<ITaskItem> ();
	}
}
