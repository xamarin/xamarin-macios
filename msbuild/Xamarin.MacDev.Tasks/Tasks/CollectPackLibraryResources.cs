using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Xamarin.Localization.MSBuild;
using Xamarin.Messaging.Build.Client;

namespace Xamarin.MacDev.Tasks {
	// This task will collect several item groups with various types of assets/resources,
	// add/compute the LogicalName value for each of them, and then add them to the
	// ItemsWithLogicalNames item group. The items in this item group will have the
	// 'OriginalItemGroup' metadata set indicating where they came from.
	public class CollectPackLibraryResources : XamarinTask {
		#region Inputs

		public ITaskItem [] AtlasTextures { get; set; } = Array.Empty<ITaskItem> ();

		public ITaskItem [] BundleResources { get; set; } = Array.Empty<ITaskItem> ();

		public ITaskItem [] ImageAssets { get; set; } = Array.Empty<ITaskItem> ();

		public ITaskItem [] InterfaceDefinitions { get; set; } = Array.Empty<ITaskItem> ();

		public ITaskItem [] ColladaAssets { get; set; } = Array.Empty<ITaskItem> ();

		public ITaskItem [] CoreMLModels { get; set; } = Array.Empty<ITaskItem> ();

		public ITaskItem [] PartialAppManifests { get; set; } = Array.Empty<ITaskItem> ();

		public ITaskItem [] SceneKitAssets { get; set; } = Array.Empty<ITaskItem> ();

		[Required]
		public string ProjectDir { get; set; } = string.Empty;

		[Required]
		public string ResourcePrefix { get; set; } = string.Empty;

		#endregion

		#region Outputs

		// These items will have the following metadata set:
		// * LogicalName
		// * OriginalItemGroup: the name of the originating item group
		[Output]
		public ITaskItem [] ItemsWithLogicalNames { get; set; } = Array.Empty<ITaskItem> ();

		#endregion

		public override bool Execute ()
		{
			var prefixes = BundleResource.SplitResourcePrefixes (ResourcePrefix);
			var rv = new List<ITaskItem> ();

			var resources = new [] {
				new { Name = "AtlasTexture", Items = AtlasTextures },
				new { Name = "BundleResource", Items = BundleResources },
				new { Name = "Collada", Items = ColladaAssets },
				new { Name = "CoreMLModel", Items = CoreMLModels },
				new { Name = "ImageAsset", Items = ImageAssets },
				new { Name = "InterfaceDefinition", Items = InterfaceDefinitions },
				new { Name = "PartialAppManifest", Items = PartialAppManifests },
				new { Name = "SceneKitAsset", Items = SceneKitAssets },
			};

			foreach (var kvp in resources) {
				var itemName = kvp.Name;
				var items = kvp.Items;

				foreach (var item in items) {
					if (!CollectBundleResources.TryCreateItemWithLogicalName (this, item, ProjectDir, prefixes, SessionId, out var itemWithLogicalName))
						continue;

					itemWithLogicalName.SetMetadata ("OriginalItemGroup", itemName);
					rv.Add (itemWithLogicalName);
				}
			}

			ItemsWithLogicalNames = rv.ToArray ();

			return !Log.HasLoggedErrors;
		}
	}
}
