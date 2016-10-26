using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.MacDev.Tasks
{
	public abstract class CollectBundleResourcesTaskBase : Task
	{
		#region Inputs

		public string SessionId { get; set; }

		public ITaskItem[] BundleResources { get; set; }

		public bool OptimizePropertyLists { get; set; }

		public bool OptimizePNGs { get; set; }

		[Required]
		public string ProjectDir { get; set; }

		[Required]
		public string ResourcePrefix { get; set; }

		#endregion

		#region Outputs

		[Output]
		public ITaskItem[] BundleResourcesWithLogicalNames { get; set; }

		#endregion

		static bool CanOptimize (string path)
		{
			switch (Path.GetExtension (path).ToLowerInvariant ()) {
			case ".png": case ".plist": case ".strings": return true;
			default: return false;
			}
		}

		public override bool Execute ()
		{
			Log.LogTaskName ("CollectBundleResources");
			Log.LogTaskProperty ("BundleResources", BundleResources);
			Log.LogTaskProperty ("OptimizePropertyLists", OptimizePropertyLists);
			Log.LogTaskProperty ("OptimizePNGs", OptimizePNGs);
			Log.LogTaskProperty ("ProjectDir", ProjectDir);
			Log.LogTaskProperty ("ResourcePrefix", ResourcePrefix);

			var prefixes = BundleResource.SplitResourcePrefixes (ResourcePrefix);
			var bundleResources = new List<ITaskItem> ();

			if (BundleResources != null) {
				foreach (var item in BundleResources) {
					var logicalName = BundleResource.GetLogicalName (ProjectDir, prefixes, item, !string.IsNullOrEmpty(SessionId));
					// We need a physical path here, ignore the Link element
					var path = item.GetMetadata ("FullPath");
					string illegal;

					if (!File.Exists (path)) {
						Log.LogError ("  Bundle Resource '{0}' not found on disk (should be at '{1}')", logicalName, path);
						continue;
					}

					if (logicalName.StartsWith (".." + Path.DirectorySeparatorChar, StringComparison.Ordinal)) {
						Log.LogError (null, null, null, item.ItemSpec, 0, 0, 0, 0, "The path '{0}' would result in a file outside of the app bundle and cannot be used.", logicalName);
						continue;
					}

					if (logicalName == "Info.plist") {
						Log.LogWarning (null, null, null, item.ItemSpec, 0, 0, 0, 0, "Info.plist files should have a Build Action of 'None'.");
						continue;
					}

					if (BundleResource.IsIllegalName (logicalName, out illegal)) {
						Log.LogError (null, null, null, item.ItemSpec, 0, 0, 0, 0, "The name '{0}' is reserved and cannot be used.", illegal);
						continue;
					}

					var bundleResource = new TaskItem (item);
					bundleResource.SetMetadata ("LogicalName", logicalName);

					bool optimize = false;

					if (CanOptimize (item.ItemSpec)) {
						var metadata = item.GetMetadata ("Optimize");

						// fall back to old metadata name
						if (string.IsNullOrEmpty (metadata))
							metadata = item.GetMetadata ("OptimizeImage");

						if (string.IsNullOrEmpty (metadata) || !bool.TryParse (metadata, out optimize)) {
							switch (Path.GetExtension (item.ItemSpec).ToLowerInvariant ()) {
							case ".plist": case ".strings": optimize = OptimizePropertyLists; break;
							case ".png": optimize = OptimizePNGs; break;
							}
						}
					}

					bundleResource.SetMetadata ("Optimize", optimize.ToString ());

					bundleResources.Add (bundleResource);
				}
			}

			BundleResourcesWithLogicalNames = bundleResources.ToArray ();

			return !Log.HasLoggedErrors;
		}
	}
}
