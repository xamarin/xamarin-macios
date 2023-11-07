using System;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Xamarin.Localization.MSBuild;

namespace Xamarin.MacDev.Tasks {
	public abstract class FindItemWithLogicalNameTaskBase : XamarinTask {
		#region Inputs

		[Required]
		public string ProjectDir { get; set; }

		[Required]
		public string ResourcePrefix { get; set; }

		[Required]
		public string LogicalName { get; set; }

		public ITaskItem [] Items { get; set; }

		#endregion Inputs

		#region Outputs

		[Output]
		public ITaskItem Item { get; set; }

		#endregion Outputs

		public override bool Execute ()
		{
			if (Items is not null) {
				var prefixes = BundleResource.SplitResourcePrefixes (ResourcePrefix);

				foreach (var item in Items) {
					var logical = BundleResource.GetLogicalName (ProjectDir, prefixes, item, !string.IsNullOrEmpty (SessionId));

					if (logical == LogicalName) {
						Log.LogMessage (MessageImportance.Low, MSBStrings.M0149, LogicalName, item.ItemSpec);
						Item = item;
						break;
					}
				}
			}

			return !Log.HasLoggedErrors;
		}
	}
}
