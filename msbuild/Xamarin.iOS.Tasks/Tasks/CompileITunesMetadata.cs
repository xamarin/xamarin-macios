using System;
using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;
using Xamarin.MacDev;
using Xamarin.Localization.MSBuild;
using Xamarin.Messaging.Build.Client;

#nullable enable

namespace Xamarin.iOS.Tasks {
	public class CompileITunesMetadata : XamarinTask, ICancelableTask {
		#region Inputs

		[Required]
		public string BundleIdentifier { get; set; } = string.Empty;

		public string BundleDisplayName { get; set; } = string.Empty;

		public string BundleVersion { get; set; } = string.Empty;

		public ITaskItem [] ITunesMetadata { get; set; } = Array.Empty<ITaskItem> ();

		[Output]
		[Required]
		public ITaskItem? OutputPath { get; set; }

		#endregion

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			PDictionary metadata;

			if (ITunesMetadata is not null) {
				if (ITunesMetadata.Length > 1) {
					Log.LogError (MSBStrings.E0023);
					return false;
				}

				var path = ITunesMetadata [0].GetMetadata ("FullPath");

				try {
					metadata = PDictionary.FromFile (path)!;
				} catch (Exception ex) {
					Log.LogError (null, null, null, path, 0, 0, 0, 0, MSBStrings.E0010, path, ex.Message);
					return false;
				}
			} else {
				var displayName = BundleDisplayName;
				var bundleVersion = BundleVersion;

				metadata = new PDictionary ();

				metadata.Add ("genre", new PString ("Application"));
				if (!string.IsNullOrEmpty (bundleVersion))
					metadata.Add ("bundleVersion", (PString) bundleVersion);
				if (!string.IsNullOrEmpty (displayName))
					metadata.Add ("itemName", (PString) displayName);
				metadata.Add ("kind", (PString) "software");
				if (displayName is not null)
					metadata.Add ("playlistName", (PString) displayName);
				metadata.Add ("softwareIconNeedsShine", (PBoolean) true);
				metadata.Add ("softwareVersionBundleId", (PString) BundleIdentifier);
			}

			Directory.CreateDirectory (Path.GetDirectoryName (OutputPath!.ItemSpec));
			metadata.Save (OutputPath.ItemSpec, true);

			return !Log.HasLoggedErrors;
		}

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}
	}
}
