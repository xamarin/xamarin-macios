using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Xamarin.Localization.MSBuild;
using Xamarin.Messaging.Build.Client;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.MacDev.Tasks {
	public class CollectBundleResources : XamarinTask, ICancelableTask {
		#region Inputs

		public ITaskItem [] BundleResources { get; set; }

		public bool OptimizePropertyLists { get; set; }

		public bool OptimizePNGs { get; set; }

		[Required]
		public string ProjectDir { get; set; }

		[Required]
		public string ResourcePrefix { get; set; }

		#endregion

		#region Outputs

		[Output]
		public ITaskItem [] BundleResourcesWithLogicalNames { get; set; }

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
			try {
				if (ShouldExecuteRemotely ()) {
					// Copy the bundle files to the build server
					new TaskRunner (SessionId, BuildEngine4).CopyInputsAsync (this).Wait ();
				}

				// But execute locally
				return ExecuteImpl ();
			} catch (Exception ex) {
				Log.LogErrorFromException (ex);

				return false;
			}
		}

		bool ExecuteImpl ()
		{
			var prefixes = BundleResource.SplitResourcePrefixes (ResourcePrefix);
			var bundleResources = new List<ITaskItem> ();

			if (BundleResources is not null) {
				foreach (var item in BundleResources) {
					// Skip anything with the PublishFolderType metadata, these are copied directly to the ResolvedFileToPublish item group instead.
					var publishFolderType = item.GetMetadata ("PublishFolderType");
					if (!string.IsNullOrEmpty (publishFolderType))
						continue;

					var logicalName = BundleResource.GetLogicalName (ProjectDir, prefixes, item, !string.IsNullOrEmpty (SessionId));
					// We need a physical path here, ignore the Link element
					var path = item.GetMetadata ("FullPath");
					string illegal;

					if (!File.Exists (path)) {
						Log.LogError (MSBStrings.E0099, logicalName, path);
						continue;
					}

					if (logicalName.StartsWith (".." + Path.DirectorySeparatorChar, StringComparison.Ordinal)) {
						Log.LogError (null, null, null, item.ItemSpec, 0, 0, 0, 0, MSBStrings.E0100, logicalName);
						continue;
					}

					if (logicalName == "Info.plist") {
						Log.LogWarning (null, null, null, item.ItemSpec, 0, 0, 0, 0, MSBStrings.E0101);
						continue;
					}

					if (BundleResource.IsIllegalName (logicalName, out illegal)) {
						Log.LogError (null, null, null, item.ItemSpec, 0, 0, 0, 0, MSBStrings.E0102, illegal);
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

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}
	}
}
