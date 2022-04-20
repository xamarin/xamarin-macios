using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

#nullable enable

namespace Xamarin.MacDev.Tasks
{
	public abstract class DSymUtilTaskBase : XamarinTask
	{
		#region Inputs

		// This can also be specified as metadata on the Executable item (as 'DSymDir')
		public string DSymDir { get; set; } = string.Empty;

		[Output]
		[Required]
		public ITaskItem [] Executable { get; set; } = Array.Empty<ITaskItem> ();

		#endregion

		#region Outputs

		// This property is required for XVS to work properly, even though it's not used for anything in the targets.
		[Output]
		public ITaskItem[] DsymContentFiles { get; set; } = Array.Empty<ITaskItem> ();

		#endregion

		public override bool Execute ()
		{
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
			args.Add ("-z");
			args.Add ("-o");
			args.Add (dSymDir);

			args.Add (Path.GetFullPath (item.ItemSpec));
			ExecuteAsync ("xcrun", args).Wait ();

			var contentsDir = Path.Combine (dSymDir, "Contents");
			if (Directory.Exists (contentsDir))
				contentFiles.AddRange (Directory.EnumerateFiles (contentsDir).Select (x => new TaskItem (x)).ToArray ());
		}
	}
}
