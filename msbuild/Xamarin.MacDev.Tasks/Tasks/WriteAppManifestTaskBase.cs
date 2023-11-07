#nullable enable

using System;
using System.IO;
using System.Linq;

using Microsoft.Build.Framework;

using Xamarin.Localization.MSBuild;

namespace Xamarin.MacDev.Tasks {
	public abstract class WriteAppManifestTaskBase : XamarinTask {
		#region Inputs

		[Required]
		[Output] // This is required so that the file is created on Windows in a remote build.
		public ITaskItem? AppBundleManifest { get; set; }

		[Required]
		public ITaskItem []? AppManifests { get; set; }

		#endregion

		public override bool Execute ()
		{
			PDictionary plist;

			var firstManifest = AppManifests! [0].ItemSpec;
			try {
				plist = PDictionary.FromFile (firstManifest)!;
			} catch (Exception ex) {
				Log.LogError (null, null, null, firstManifest, 0, 0, 0, 0, MSBStrings.E0010, ex.Message);
				return false;
			}

			if (AppManifests.Length > 1)
				CompileAppManifestTaskBase.MergePartialPLists (this, plist, AppManifests.Skip (1));

			// Remove any IDE specific keys we don't want in the final app manifest.
			plist.Remove (ManifestKeys.XSLaunchImageAssets);
			plist.Remove (ManifestKeys.XSAppIconAssets);

			// write the resulting app manifest
			var appBundleManifestPath = AppBundleManifest!.ItemSpec;
			Directory.CreateDirectory (Path.GetDirectoryName (appBundleManifestPath));
			plist.Save (appBundleManifestPath, true, true);

			return !Log.HasLoggedErrors;
		}
	}
}
