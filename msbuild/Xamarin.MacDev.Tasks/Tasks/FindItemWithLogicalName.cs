using System;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Xamarin.Localization.MSBuild;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.MacDev.Tasks {
	public class FindItemWithLogicalName : XamarinTask {
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
					var logical = BundleResource.GetLogicalName (this, ProjectDir, prefixes, item);

					if (logical == LogicalName) {
						Log.LogMessage (MessageImportance.Low, MSBStrings.M0149, LogicalName, item.ItemSpec);
						Item = item;
						break;
					}
					Log.LogMessage (MessageImportance.Low, MSBStrings.M0149b /* "Discarded '{0}' with logical name '{1}' because the logical name does not match '{2}'" */, item.ItemSpec, logical, LogicalName);
				}
			}

			return !Log.HasLoggedErrors;
		}
	}
}
