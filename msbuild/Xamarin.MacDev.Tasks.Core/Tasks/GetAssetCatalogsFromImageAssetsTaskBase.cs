using System.IO;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.MacDev.Tasks
{
	public abstract class GetAssetCatalogsFromImageAssetsTaskBase : Task
	{
		public string SessionId { get; set; }

		public ITaskItem[] ImageAssets { get; set; }

		[Required]
		public string ProjectDir { get; set; }

		[Output]
		public ITaskItem[] AssetCatalogs { get; set; }

		public override bool Execute ()
		{
			Log.LogTaskName ("GetAssetCatalogsFromImageAssets");
			Log.LogTaskProperty ("ImageAssets", ImageAssets);
			Log.LogTaskProperty ("ProjectDir", ProjectDir);

			var catalogs = new List<ITaskItem> ();
			var unique = new HashSet<string> ();

			foreach (var asset in ImageAssets) {
				var vpath = BundleResource.GetVirtualProjectPath (ProjectDir, asset);
				if (Path.GetFileName (vpath) != "Contents.json")
					continue;

				// get the parent (which will typically be .appiconset, .launchimage, .imageset, .iconset, etc)
				var catalog = Path.GetDirectoryName (vpath);

				// keep walking up the directory structure until we get to the .xcassets directory
				while (!string.IsNullOrEmpty (catalog) && Path.GetExtension (catalog) != ".xcassets")
					catalog = Path.GetDirectoryName (catalog);

				if (string.IsNullOrEmpty (catalog)) {
					Log.LogWarning (null, null, null, asset.ItemSpec, 0, 0, 0, 0, "Asset not part of an asset catalog: {0}", asset.ItemSpec);
					continue;
				}

				if (unique.Add (catalog))
					catalogs.Add (new TaskItem (catalog));
			}

			AssetCatalogs = catalogs.ToArray ();

			return !Log.HasLoggedErrors;
		}
	}
}
