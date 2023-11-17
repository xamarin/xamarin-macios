using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Build.Framework;

using Xamarin.Messaging.Build.Client;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.MacDev.Tasks {
	public class InstallNameTool : XamarinTask, ITaskCallback {
		[Required]
		public ITaskItem [] DynamicLibrary { get; set; }

		[Required]
		public string SdkDevPath { get; set; }

		// This isn't consumed from the targets files, but it's needed for VSX to create corresponding
		// files on Windows.
		[Output]
		public ITaskItem [] ReidentifiedDynamicLibrary { get; set; }

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			var processes = new Task [DynamicLibrary.Length];
			ReidentifiedDynamicLibrary = new ITaskItem [DynamicLibrary.Length];

			for (var i = 0; i < DynamicLibrary.Length; i++) {
				var input = DynamicLibrary [i];
				var src = Path.GetFullPath (input.ItemSpec);
				// Make sure we use the correct path separator, these are relative paths, so it doesn't look
				// like MSBuild does the conversion automatically.
				var target = input.GetMetadata ("ReidentifiedPath").Replace ('\\', Path.DirectorySeparatorChar);
				var temporaryTarget = target + ".tmp";

				// install_name_tool modifies the file in-place, so copy it first to a temporary file first.
				Directory.CreateDirectory (Path.GetDirectoryName (temporaryTarget));
				File.Copy (src, temporaryTarget, true);

				var arguments = new List<string> ();

				arguments.Add ("install_name_tool");
				arguments.Add ("-id");
				arguments.Add (input.GetMetadata ("DynamicLibraryId"));
				arguments.Add (temporaryTarget);

				processes [i] = ExecuteAsync ("xcrun", arguments, sdkDevPath: SdkDevPath).ContinueWith ((v) => {
					if (v.IsFaulted)
						throw v.Exception;
					if (v.Status == TaskStatus.RanToCompletion) {
						File.Delete (target);
						File.Move (temporaryTarget, target);
					}
				});

				ReidentifiedDynamicLibrary [i] = new Microsoft.Build.Utilities.TaskItem (target);
			}

			Task.WaitAll (processes);

			return !Log.HasLoggedErrors;
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => true;
		public bool ShouldCreateOutputFile (ITaskItem item) => true;
		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied () => Enumerable.Empty<ITaskItem> ();
	}
}
