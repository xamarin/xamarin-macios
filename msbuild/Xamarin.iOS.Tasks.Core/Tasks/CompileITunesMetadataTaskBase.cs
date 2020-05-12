using System;
using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;
using Xamarin.MacDev;
using Xamarin.Localization.MSBuild;

namespace Xamarin.iOS.Tasks
{
	public abstract class CompileITunesMetadataTaskBase : XamarinTask
	{
		#region Inputs

		[Required]
		public string AppBundleDir { get; set; }

		public ITaskItem[] ITunesMetadata { get; set; }

		[Output]
		[Required]
		public ITaskItem OutputPath { get; set; }

		#endregion

		public override bool Execute ()
		{
			PDictionary metadata;

			if (ITunesMetadata != null) {
				if (ITunesMetadata.Length > 1) {
					Log.LogError (MSBStrings.E0023);
					return false;
				}

				var path = ITunesMetadata[0].GetMetadata ("FullPath");

				try {
					metadata = PDictionary.FromFile (path);
				} catch (Exception ex) {
					Log.LogError (null, null, null, path, 0, 0, 0, 0, MSBStrings.E0010, path, ex.Message);
					return false;
				}
			} else {
				var manifest = Path.Combine (AppBundleDir, "Info.plist");
				PDictionary plist;

				try {
					plist = PDictionary.FromFile (manifest);
				} catch (Exception ex) {
					Log.LogError (null, null, null, manifest, 0, 0, 0, 0, MSBStrings.E0010, manifest, ex.Message);
					return false;
				}

				var displayName = plist.GetCFBundleDisplayName ();
				var bundleVersion = plist.GetCFBundleVersion ();

				metadata = new PDictionary ();

				metadata.Add ("genre", new PString ("Application"));
				if (bundleVersion != null)
					metadata.Add ("bundleVersion", (PString) bundleVersion);
				if (displayName != null)
					metadata.Add ("itemName", (PString) displayName);
				metadata.Add ("kind", (PString) "software");
				if (displayName != null)
					metadata.Add ("playlistName", (PString) displayName);
				metadata.Add ("softwareIconNeedsShine", (PBoolean) true);
				metadata.Add ("softwareVersionBundleId", (PString) plist.GetCFBundleIdentifier ());
			}

			Directory.CreateDirectory (Path.GetDirectoryName (OutputPath.ItemSpec));
			metadata.Save (OutputPath.ItemSpec, true);

			return !Log.HasLoggedErrors;
		}
	}
}
