using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.Messaging.Build.Client;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public class DSymUtil : XamarinTask, ITaskCallback {
		#region Inputs

		// This can also be specified as metadata on the Executable item (as 'DSymDir')
		public string DSymDir { get; set; } = string.Empty;

		[Output]
		[Required]
		public ITaskItem [] Executable { get; set; } = Array.Empty<ITaskItem> ();

		[Required]
		public string SdkDevPath { get; set; } = string.Empty;

		#endregion

		#region Outputs

		// This property is required for XVS to work properly, even though it's not used for anything in the targets.
		[Output]
		public ITaskItem [] DsymContentFiles { get; set; } = Array.Empty<ITaskItem> ();

		#endregion

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			var contentFiles = new List<ITaskItem> ();

			// We're not executing multiple dsymutil processes in parallel, because
			// we're asking dsymutil to do parallel processing (we're passing '-num-threads 4' to dsymutil)
			foreach (var item in Executable) {
				ExecuteDSymUtil (item, contentFiles);
			}

			DsymContentFiles = contentFiles.ToArray ();

			return !Log.HasLoggedErrors;
		}

		void ExecuteDSymUtil (ITaskItem item, List<ITaskItem> contentFiles)
		{
			var dSymDir = GetNonEmptyStringOrFallback (item, "DSymDir", DSymDir, required: true);

			var args = new List<string> ();

			args.Add ("dsymutil");
			args.Add ("-num-threads");
			args.Add ("4");
			if (AppleSdkSettings.XcodeVersion < new Version (13, 3)) {
				// Apple removed the -z / --minimize option in Xocde 13.3, so now if you use it you get a warning: "ignoring unknown option: -z"
				// So just don't pass -z when Xcode >= 13.3
				// Ref: https://github.com/llvm/llvm-project/commit/5d07dc897707f877c45cab6c7e4b65dad7d3ff6d
				// Ref: https://github.com/dotnet/runtime/issues/66770
				args.Add ("-z");
			}
			args.Add ("-o");
			args.Add (dSymDir);

			args.Add (Path.GetFullPath (item.ItemSpec));
			ExecuteAsync ("xcrun", args, sdkDevPath: SdkDevPath).Wait ();

			var contentsDir = Path.Combine (dSymDir, "Contents");
			if (Directory.Exists (contentsDir))
				contentFiles.AddRange (Directory.EnumerateFiles (contentsDir).Select (x => new TaskItem (x)).ToArray ());
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => false;

		public bool ShouldCreateOutputFile (ITaskItem item) => Executable.Contains (item);

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied () => Enumerable.Empty<ITaskItem> ();
	}
}
