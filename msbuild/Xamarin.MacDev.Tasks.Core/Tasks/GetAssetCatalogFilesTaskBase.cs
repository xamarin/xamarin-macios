using System.IO;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.MacDev.Tasks
{
	public abstract class GetAssetCatalogFilesTaskBase : Task
	{
		public string SessionId { get; set; }

		public ITaskItem[] AssetCatalogs { get; set; }

		[Required]
		public string ProjectDir { get; set; }

		[Output]
		public ITaskItem[] AssetCatalogFiles { get; set; }

		public override bool Execute ()
		{
			Log.LogTaskName ("GetAssetCatalogFiles");
			Log.LogTaskProperty ("AssetCatalogs", AssetCatalogs);
			Log.LogTaskProperty ("ProjectDir", ProjectDir);

			var files = new List<ITaskItem> ();

			if (AssetCatalogs != null) {
				foreach (var catalog in AssetCatalogs) {
					foreach (var path in Directory.EnumerateFiles (catalog.ItemSpec, "*.*", SearchOption.AllDirectories)) {
						var vpath = PathUtils.AbsoluteToRelative (ProjectDir, path);

						files.Add (new TaskItem (vpath));
					}
				}
			}

			AssetCatalogFiles = files.ToArray ();

			return !Log.HasLoggedErrors;
		}
	}
}
