using System;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.MacDev.Tasks
{
	public abstract class FindItemWithLogicalNameTaskBase : Task
	{
		#region Inputs

		public string SessionId { get; set; }

		[Required]
		public string ProjectDir { get; set; }

		[Required]
		public string ResourcePrefix { get; set; }

		[Required]
		public string LogicalName { get; set; }

		public ITaskItem[] Items { get; set; }

		#endregion Inputs

		#region Outputs

		[Output]
		public ITaskItem Item { get; set; }

		#endregion Outputs

		public override bool Execute ()
		{
			Log.LogTaskName ("FindItemWithLogicalName");
			Log.LogTaskProperty ("Items", Items);
			Log.LogTaskProperty ("LogicalName", LogicalName);
			Log.LogTaskProperty ("ProjectDir", ProjectDir);
			Log.LogTaskProperty ("ResourcePrefix", ResourcePrefix);

			if (Items != null) {
				var prefixes = BundleResource.SplitResourcePrefixes (ResourcePrefix);

				foreach (var item in Items) {
					var logical = BundleResource.GetLogicalName (ProjectDir, prefixes, item, !string.IsNullOrEmpty(SessionId));

					if (logical == LogicalName) {
						Log.LogMessage (MessageImportance.Low, "  {0} found at: {1}", LogicalName, item.ItemSpec);
						Item = item;
						break;
					}
				}
			}

			return !Log.HasLoggedErrors;
		}
	}
}

