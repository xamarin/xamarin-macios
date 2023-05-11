using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Framework;
using Xamarin.Messaging.Build.Client;

namespace Xamarin.MacDev.Tasks {
	public class ComputeBundleResourceOutputPaths : ComputeBundleResourceOutputPathsTaskBase, ITaskCallback, ICancelableTask {
		public override bool Execute ()
		{
			if (!ShouldExecuteRemotely ())
				return base.Execute ();

			var result = new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			RemoveDuplicates ();

			return result;
		}

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied () => Enumerable.Empty<ITaskItem> ();

		public bool ShouldCopyToBuildServer (ITaskItem item) => false;

		public bool ShouldCreateOutputFile (ITaskItem item) => false;

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}

		void RemoveDuplicates ()
		{
			// Remove duplicated files 
			var filteredOutput = new List<ITaskItem> ();

			foreach (var item in BundleResourcesWithOutputPaths) {
				var itemIsFromProjectReference = IsFromProjectReference (item);
				var duplicate = filteredOutput.FirstOrDefault (i => i.GetMetadata ("OutputPath") == item.GetMetadata ("OutputPath"));

				if (duplicate is not null) {
					if (!IsFromProjectReference (duplicate) && itemIsFromProjectReference) {
						filteredOutput.Remove (duplicate);
					}
				}

				if (itemIsFromProjectReference || duplicate is null)
					filteredOutput.Add (item);
			}

			BundleResourcesWithOutputPaths = filteredOutput.ToArray ();
		}

		bool IsFromProjectReference (ITaskItem item) => !string.IsNullOrEmpty (item.GetMetadata ("MSBuildSourceProjectFile"));
	}
}
