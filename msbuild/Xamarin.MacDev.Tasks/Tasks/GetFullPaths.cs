using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;
using Xamarin.Messaging.Build.Client;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public class GetFullPaths : XamarinTask, ICancelableTask, ITaskCallback {
		[Required]
		public ITaskItem [] Items { get; set; } = Array.Empty<ITaskItem> ();

		public string [] Metadata { get; set; } = Array.Empty<string> ();

		[Output]
		public ITaskItem [] Output { get; set; } = Array.Empty<ITaskItem> ();

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			return ExecuteLocally ();
		}

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}

		bool ExecuteLocally ()
		{
			var rv = new List<ITaskItem> ();

			foreach (var item in Items) {
				var identity = item.ItemSpec;
				if (Metadata.Length == 0 || Array.IndexOf (Metadata, "Identity") >= 0)
					identity = Path.GetFullPath (identity);
				var newItem = new TaskItem (identity);
				item.CopyMetadataTo (newItem);
				foreach (var md in Metadata) {
					if (string.IsNullOrEmpty (md))
						continue;
					if (md == "Identity")
						continue;
					newItem.SetMetadata (md, Path.GetFullPath (newItem.GetMetadata (md)));
				}
				rv.Add (newItem);
			}

			Output = rv.ToArray ();

			return !Log.HasLoggedErrors;
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => true;

		public bool ShouldCreateOutputFile (ITaskItem item) => false;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied () => Enumerable.Empty<ITaskItem> ();
	}
}
