using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Xamarin.Localization.MSBuild;

namespace Xamarin.MacDev.Tasks {
	public class PackLibraryResources : XamarinTask, ICancelableTask {
		#region Inputs

		[Required]
		public string Prefix { get; set; } = string.Empty;

		public ITaskItem [] BundleResourcesWithLogicalNames { get; set; } = Array.Empty<ITaskItem> ();

		public ITaskItem [] BundleOriginalResourcesWithLogicalNames { get; set; } = Array.Empty<ITaskItem> ();

		#endregion

		#region Outputs

		[Output]
		public ITaskItem [] EmbeddedResources { get; set; } = Array.Empty<ITaskItem> ();

		#endregion

		// The opposite function is UnpackLibraryResources.UnmangleResource
		public static string EscapeMangledResource (string name)
		{
			var mangled = new StringBuilder ();

			for (int i = 0; i < name.Length; i++) {
				switch (name [i]) {
				case '\\':
				case '/': mangled.Append ("_s"); break;
				case '_': mangled.Append ("__"); break;
				default: mangled.Append (name [i]); break;
				}
			}

			return mangled.ToString ();
		}

		public override bool Execute ()
		{
			var results = new List<ITaskItem> ();

			var hashOriginalResources = new HashSet<string> (BundleOriginalResourcesWithLogicalNames.Select (v => v.ItemSpec));

			foreach (var item in BundleResourcesWithLogicalNames) {
				if (hashOriginalResources.Contains (item.ItemSpec)) {
					Log.LogMessage (MessageImportance.Low, $"Skipping BundleResourcesWithLogicalNames={item.ItemSpec}, because it's also specified in BundleOriginalResourcesWithLogicalNames");
					continue;
				}

				var logicalName = item.GetMetadata ("LogicalName");

				if (string.IsNullOrEmpty (logicalName)) {
					Log.LogError (null, null, null, item.ItemSpec, 0, 0, 0, 0, MSBStrings.E0161);
					continue;
				}

				var embedded = new TaskItem (item);

				embedded.SetMetadata ("LogicalName", "__" + Prefix + "_content_" + EscapeMangledResource (logicalName));

				results.Add (embedded);
			}

			foreach (var item in BundleOriginalResourcesWithLogicalNames) {
				var originalItemGroup = item.GetMetadata ("OriginalItemGroup");
				if (!TryGetMangledLogicalName (item, originalItemGroup, out var mangledLogicalName))
					continue;
				var embedded = new TaskItem (item);
				embedded.SetMetadata ("LogicalName", mangledLogicalName);
				results.Add (embedded);
			}

			EmbeddedResources = results.ToArray ();

			return !Log.HasLoggedErrors;
		}

		bool TryGetMangledLogicalName (ITaskItem item, string itemName, [NotNullWhen (true)] out string? mangled)
		{
			var logicalName = item.GetMetadata ("LogicalName");
			if (string.IsNullOrEmpty (logicalName)) {
				Log.LogError (null, null, null, item.ItemSpec, 0, 0, 0, 0, MSBStrings.E0161);
				mangled = null;
				return false;
			}
			mangled = "__" + Prefix + "_item_" + itemName + "_" + EscapeMangledResource (logicalName);
			return true;
		}

		public void Cancel ()
		{
		}
	}
}
