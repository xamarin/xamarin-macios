using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Tasks;
using Microsoft.Build.Utilities;

using Xamarin.Messaging.Build.Client;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public class MobileILStrip : global::ILStrip, ITaskCallback {
		public string SessionId { get; set; } = string.Empty;

		[Output]
		public ITaskItem [] StrippedAssemblies { get; set; } = Array.Empty<ITaskItem> ();

		public override bool Execute ()
		{
			if (this.ShouldExecuteRemotely (SessionId))
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			var stripedItems = new List<ITaskItem> ();
			foreach (var item in Assemblies) {
				var outputPath = item.GetMetadata ("OutputPath");
				stripedItems.Add (new TaskItem (outputPath, item.CloneCustomMetadata ()));
				Directory.CreateDirectory (Path.GetDirectoryName (outputPath));
			}

			var result = base.Execute ();

			if (result)
				StrippedAssemblies = stripedItems.ToArray ();

			return result;
		}

		public bool ShouldCopyToBuildServer (ITaskItem item)
		{
			// Some assemblies are already on the Mac, and we have a 0-length
			// output file on Windows. We don't want to copy these files.
			// However, some assemblies have to be copied, because they don't
			// already exist on the Mac (typically resource assemblies). So
			// filter to assemblies with a non-zero length.

			var finfo = new FileInfo (item.ItemSpec);
			if (!finfo.Exists || finfo.Length == 0)
				return false;

			return true;
		}

		public bool ShouldCreateOutputFile (ITaskItem item) => true;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied () => Enumerable.Empty<ITaskItem> ();
	}
}
