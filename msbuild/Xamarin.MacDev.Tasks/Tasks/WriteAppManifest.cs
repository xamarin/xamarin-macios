#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Build.Framework;

using Xamarin.Localization.MSBuild;
using Xamarin.Messaging.Build.Client;

namespace Xamarin.MacDev.Tasks {
	public class WriteAppManifest : XamarinTask, ICancelableTask, ITaskCallback {
		#region Inputs

		[Required]
		[Output] // This is required so that the file is created on Windows in a remote build.
		public ITaskItem? AppBundleManifest { get; set; }

		[Required]
		public ITaskItem []? AppManifests { get; set; }

		#endregion

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			PDictionary plist;

			var firstManifest = AppManifests! [0].ItemSpec;
			try {
				plist = PDictionary.FromFile (firstManifest)!;
			} catch (Exception ex) {
				Log.LogError (null, null, null, firstManifest, 0, 0, 0, 0, MSBStrings.E0010, ex.Message);
				return false;
			}

			if (AppManifests.Length > 1)
				CompileAppManifest.MergePartialPLists (this, plist, AppManifests.Skip (1));

			// Remove any IDE specific keys we don't want in the final app manifest.
			plist.Remove (ManifestKeys.XSLaunchImageAssets);
			plist.Remove (ManifestKeys.XSAppIconAssets);

			// write the resulting app manifest
			var appBundleManifestPath = AppBundleManifest!.ItemSpec;
			Directory.CreateDirectory (Path.GetDirectoryName (appBundleManifestPath));
			plist.Save (appBundleManifestPath, true, true);

			return !Log.HasLoggedErrors;
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => false;

		public bool ShouldCreateOutputFile (ITaskItem item) => false;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied () => Enumerable.Empty<ITaskItem> ();

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}
	}
}
