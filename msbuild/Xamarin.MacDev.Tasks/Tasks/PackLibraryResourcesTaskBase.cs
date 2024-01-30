using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Xamarin.Localization.MSBuild;
using Xamarin.Messaging.Build.Client;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public class PackLibraryResources : XamarinTask, ITaskCallback, ICancelableTask {
		#region Inputs

		[Required]
		public string Prefix { get; set; } = string.Empty;

		public ITaskItem [] BundleResourcesWithLogicalNames { get; set; } = Array.Empty<ITaskItem> ();

		#endregion

		#region Outputs

		[Output]
		public ITaskItem [] EmbeddedResources { get; set; } = Array.Empty<ITaskItem> ();

		#endregion

		public static string EscapeMangledResource (string name)
		{
			var mangled = new StringBuilder ();

			for (int i = 0; i < name.Length; i++) {
				switch (name [i]) {
				case '\\':
				case '/': mangled.Append ("_f"); break;
				case '_': mangled.Append ("__"); break;
				default: mangled.Append (name [i]); break;
				}
			}

			return mangled.ToString ();
		}

		bool ExecuteRemotely ()
		{
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

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return ExecuteRemotely ();

			var results = new List<ITaskItem> ();

			if (BundleResourcesWithLogicalNames is not null) {
				foreach (var item in BundleResourcesWithLogicalNames) {
					var logicalName = item.GetMetadata ("LogicalName");

					if (string.IsNullOrEmpty (logicalName)) {
						Log.LogError (MSBStrings.E0161);
						return false;
					}

					var embedded = new TaskItem (item);

					embedded.SetMetadata ("LogicalName", "__" + Prefix + "_content_" + EscapeMangledResource (logicalName));

					results.Add (embedded);
				}
			}

			EmbeddedResources = results.ToArray ();

			return !Log.HasLoggedErrors;
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
