using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev;
using Xamarin.Localization.MSBuild;
using Xamarin.Messaging.Build.Client;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.MacDev.Tasks {
	public class CreateAssetPackManifest : XamarinTask, ICancelableTask {
		const double DownloadPriorityInterval = 0.90;
		const double TopDownloadPriority = 0.95;

		[Required]
		public ITaskItem AppBundleDir { get; set; }

		public string InitialInstallTags { get; set; }

		public string PrefetchOrder { get; set; }

		[Required]
		public string OutputPath { get; set; }

		static double GetDownloadPriority (int index, int length)
		{
			if (length == 1)
				return 0.5;

			if (index == 0)
				return TopDownloadPriority;

			var step = Math.Round (DownloadPriorityInterval / (length - 1), 3);

			return TopDownloadPriority - step * index;
		}

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			var manifestPath = Path.Combine (AppBundleDir.ItemSpec, "AssetPackManifestTemplate.plist");
			var onDemandResourcesPath = Path.Combine (AppBundleDir.ItemSpec, "OnDemandResources.plist");
			var onDemandResourcesDir = Path.Combine (OutputPath, "OnDemandResources");
			var onDemandResourcesStamp = File.GetLastWriteTimeUtc (onDemandResourcesPath);
			var initialInstallTags = new HashSet<string> (AssetPackUtils.ParseTags (InitialInstallTags));
			var prefetchOrder = AssetPackUtils.ParseTags (PrefetchOrder);
			var manifestStamp = File.GetLastWriteTimeUtc (manifestPath);
			var onDemandResources = new PDictionary ();
			var requestTags = new PDictionary ();
			bool updateOnDemandResources = false;
			var assetPacks = new PDictionary ();
			var manifest = new PDictionary ();
			var resources = new PArray ();
			bool updateManifest = false;

			if (!Directory.Exists (onDemandResourcesDir))
				return !Log.HasLoggedErrors;

			onDemandResources.Add ("NSBundleResourceRequestAssetPacks", assetPacks);
			onDemandResources.Add ("NSBundleResourceRequestTags", requestTags);

			manifest.Add ("resources", resources);

			foreach (var dir in Directory.EnumerateDirectories (onDemandResourcesDir)) {
				var path = Path.Combine (dir, "Info.plist");
				PDictionary info;

				if (!File.Exists (path))
					continue;

				var mtime = File.GetLastWriteTimeUtc (path);

				updateOnDemandResources = updateOnDemandResources || mtime > onDemandResourcesStamp;
				updateManifest = updateManifest || mtime > manifestStamp;

				try {
					info = PDictionary.FromFile (path);
				} catch {
					continue;
				}

				var bundleIdentifier = info.GetCFBundleIdentifier ();
				var primaryContentHash = new PDictionary ();
				var resource = new PDictionary ();
				var items = new PArray ();
				long size = 0;

				// update OnDemandResources.plist:NSBundleResourceRequestAssetPacks
				foreach (var file in Directory.EnumerateFiles (dir)) {
					var name = Path.GetFileName (file);

					if (name != "Info.plist")
						items.Add (new PString (name));

					size += new FileInfo (file).Length;
				}

				assetPacks.Add (bundleIdentifier, items);

				// update OnDemandResources.plist:NSBundleResourceRequestTags
				var tags = info.GetArray ("Tags").OfType<PString> ().Select (x => x.Value);
				var priority = double.NaN;

				foreach (var tag in tags) {
					PDictionary dict;
					PArray packs;

					if (initialInstallTags.Contains (tag)) {
						priority = 1.0f;
					} else {
						for (int i = 0; i < prefetchOrder.Length; i++) {
							if (tag == prefetchOrder [i]) {
								var value = GetDownloadPriority (i, prefetchOrder.Length);

								priority = double.IsNaN (priority) ? value : Math.Max (priority, value);
								break;
							}
						}
					}

					if (!requestTags.TryGetValue (tag, out dict)) {
						dict = new PDictionary ();
						dict.Add ("NSAssetPacks", new PArray ());

						requestTags.Add (tag, dict);
					}

					packs = dict.GetArray ("NSAssetPacks");

					packs.Add (new PString (bundleIdentifier));
				}

				// update AssetPackManifestTemplate.plist
				resource.Add ("URL", new PString ("http://127.0.0.1" + Uri.EscapeUriString (Path.GetFullPath (dir))));
				resource.Add ("bundleKey", new PString (bundleIdentifier));

				if (!double.IsNaN (priority))
					resource.Add ("downloadPriority", new PReal (priority));

				resource.Add ("isStreamable", new PBoolean (true));
				primaryContentHash.Add ("hash", mtime.ToString ("yyyy-MM-dd HH:mm:ss.000"));
				primaryContentHash.Add ("strategy", "modtime");
				resource.Add ("primaryContentHash", primaryContentHash);
				resource.Add ("uncompressedSize", new PNumber ((int) ((size + 8191) & ~8191)));

				resources.Add (resource);
			}

			if (updateOnDemandResources) {
				try {
					onDemandResources.Save (onDemandResourcesPath, true, true);
				} catch (Exception ex) {
					Log.LogError (MSBStrings.E0120, onDemandResourcesPath, ex.Message);
				}
			}

			if (updateManifest) {
				try {
					manifest.Save (manifestPath, true, true);
				} catch (Exception ex) {
					Log.LogError (MSBStrings.E0120, manifestPath, ex.Message);
				}
			}

			return !Log.HasLoggedErrors;
		}

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}
	}
}
