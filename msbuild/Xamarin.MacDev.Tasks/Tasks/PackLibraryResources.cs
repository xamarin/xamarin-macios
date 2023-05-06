using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Framework;
using Xamarin.Messaging.Build.Client;

namespace Xamarin.MacDev.Tasks {
	public class PackLibraryResources : PackLibraryResourcesTaskBase, ITaskCallback, ICancelableTask {
		public override bool Execute ()
		{
			if (!ShouldExecuteRemotely ())
				return base.Execute ();

			// Fix LogicalName path for the Mac
			if (BundleResourcesWithLogicalNames is not null) {
				foreach (var resource in BundleResourcesWithLogicalNames) {
					var logicalName = resource.GetMetadata ("LogicalName");

					if (!string.IsNullOrEmpty (logicalName)) {
						resource.SetMetadata ("LogicalName", logicalName.Replace ("\\", "/"));
					}
				}
			}

			var runner = new TaskRunner (SessionId, BuildEngine4);

			try {
				var result = runner.RunAsync (this).Result;

				if (result && EmbeddedResources is not null) {
					// We must get the "real" file that will be embedded in the
					// compiled assembly in Windows
					foreach (var embeddedResource in EmbeddedResources.Where (x => runner.ShouldCopyItemAsync (task: this, item: x).Result)) {
						runner.GetFileAsync (this, embeddedResource.ItemSpec).Wait ();
					}
				}

				return result;
			} catch (Exception ex) {
				Log.LogErrorFromException (ex);

				return false;
			}
		}

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => false;

		public bool ShouldCreateOutputFile (ITaskItem item) => false;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied () => Enumerable.Empty<ITaskItem> ();
	}
}
