using System;
using System.IO;
using System.Diagnostics;
using System.Linq;

using Microsoft.Build.Framework;

using System.Globalization;

using Xamarin.Localization.MSBuild;
using Xamarin.iOS.Tasks;
using Xamarin.Messaging.Build.Client;
using Xamarin.Utils;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.MacDev.Tasks {
	public class Archive : XamarinTask, ICancelableTask {
		protected readonly DateTime Now = DateTime.Now;

		#region Inputs

		[Required]
		public ITaskItem AppBundleDir { get; set; }

		public ITaskItem [] AppExtensionReferences { get; set; }

		// default is `MonoBundle` but that can be changed with `_CustomBundleName`
		public string CustomBundleName { get; set; }

		public string InsightsApiKey { get; set; }

		public ITaskItem [] ITunesSourceFiles { get; set; }

		[Required]
		public string OutputPath { get; set; }

		[Required]
		public string ProjectName { get; set; }

		public string ProjectGuid { get; set; }

		public string ProjectTypeGuids { get; set; }

		public string SolutionPath { get; set; }

		public string SigningKey { get; set; }

		public ITaskItem [] WatchAppReferences { get; set; }
		#endregion

		#region Outputs

		[Output]
		public string ArchiveDir { get; set; }

		#endregion

		protected string DSYMDir {
			get { return AppBundleDir.ItemSpec + ".dSYM"; }
		}

		protected string MSYMDir {
			get { return AppBundleDir.ItemSpec + ".mSYM"; }
		}

		protected string XcodeArchivesDir {
			get {
				var home = Environment.GetFolderPath (Environment.SpecialFolder.UserProfile);

				return Path.Combine (home, "Library", "Developer", "Xcode", "Archives");
			}
		}

		string UserFrameworksPath {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
				case ApplePlatform.WatchOS:
					return Path.Combine (AppBundleDir.ItemSpec, "Frameworks");
				case ApplePlatform.MacOSX:
				case ApplePlatform.MacCatalyst:
					return Path.Combine (AppBundleDir.ItemSpec, "Contents", "Frameworks");
				default:
					throw new InvalidOperationException (string.Format (MSBStrings.InvalidPlatform, Platform));
				}
			}
		}

		string UserDylibPath {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
				case ApplePlatform.WatchOS:
					return AppBundleDir.ItemSpec;
				case ApplePlatform.MacOSX:
				case ApplePlatform.MacCatalyst:
					return Path.Combine (AppBundleDir.ItemSpec, "Contents", CustomBundleName);
				default:
					throw new InvalidOperationException (string.Format (MSBStrings.InvalidPlatform, Platform));
				}
			}
		}

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ()) {
				if (AppExtensionReferences is not null)
					TaskItemFixer.ReplaceItemSpecsWithBuildServerPath (AppExtensionReferences, SessionId);

				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;
			}

			var archiveDir = CreateArchiveDirectory ();
			try {
				var plist = PDictionary.FromFile (PlatformFrameworkHelper.GetAppManifestPath (Platform, AppBundleDir.ItemSpec));
				var productsDir = Path.Combine (archiveDir, "Products");

				// Archive the OnDemandResources...
				var resourcesDestDir = Path.Combine (productsDir, "OnDemandResources");
				var resourcesSrcDir = Path.Combine (OutputPath, "OnDemandResources");

				if (Directory.Exists (resourcesSrcDir))
					Ditto (resourcesSrcDir, resourcesDestDir);

				// Archive the Applications...
				var appDestDir = Path.Combine (productsDir, "Applications", Path.GetFileName (AppBundleDir.ItemSpec));
				Ditto (AppBundleDir.ItemSpec, appDestDir);

				// Archive the main dSYM...
				ArchiveDSym (DSYMDir, archiveDir);

				// for each `.dylib` (file) inside `MonoBundle` there could be a corresponding `.dSYM` - e.g. when using an AOT mode
				foreach (var dylib in Directory.GetFiles (UserDylibPath, "*.dylib")) {
					var dsym = Path.Combine (AppBundleDir.ItemSpec, "..", Path.GetFileName (dylib) + ".dSYM");
					ArchiveDSym (dsym, archiveDir);
				}

				// for each user framework (directory) that is bundled inside the app we must also archive their dSYMs, if available
				var fks = UserFrameworksPath;
				if (Directory.Exists (fks)) {
					foreach (var fx in Directory.GetDirectories (fks, "*.framework")) {
						var dsym = Path.Combine (AppBundleDir.ItemSpec, "..", Path.GetFileName (fx) + ".dSYM");
						ArchiveDSym (dsym, archiveDir);
					}
				}

				// Archive the mSYMs...
				ArchiveMSym (MSYMDir, archiveDir);

				// Archive the Bitcode symbol maps
				var bcSymbolMaps = Directory.GetFiles (Path.GetDirectoryName (DSYMDir), "*.bcsymbolmap");
				if (bcSymbolMaps.Length > 0) {
					var bcSymbolMapsDir = Path.Combine (archiveDir, "BCSymbolMaps");

					Directory.CreateDirectory (bcSymbolMapsDir);

					for (int i = 0; i < bcSymbolMaps.Length; i++)
						File.Copy (bcSymbolMaps [i], Path.Combine (bcSymbolMapsDir, Path.GetFileName (bcSymbolMaps [i])));
				}

				if (AppExtensionReferences is not null) {
					// Archive the dSYMs, mSYMs, etc for each of the referenced App Extensions as well...
					for (int i = 0; i < AppExtensionReferences.Length; i++)
						ArchiveAppExtension (AppExtensionReferences [i], archiveDir);
				}

				if (WatchAppReferences is not null) {
					// Archive the dSYMs, mSYMs, etc for each of the referenced WatchOS2 Apps as well...
					for (int i = 0; i < WatchAppReferences.Length; i++)
						ArchiveWatchApp (WatchAppReferences [i], archiveDir);
				}

				if (ITunesSourceFiles is not null) {
					// Archive the iTunesMetadata.plist and iTunesArtwork files...
					var iTunesMetadataDir = Path.Combine (archiveDir, "iTunesMetadata", Path.GetFileName (AppBundleDir.ItemSpec));
					for (int i = 0; i < ITunesSourceFiles.Length; i++) {
						var archivedMetaFile = Path.Combine (iTunesMetadataDir, Path.GetFileName (ITunesSourceFiles [i].ItemSpec));

						Directory.CreateDirectory (iTunesMetadataDir);
						File.Copy (ITunesSourceFiles [i].ItemSpec, archivedMetaFile, true);
					}
				}

				// Generate an archive Info.plist
				var arInfo = new PDictionary ();
				// FIXME: figure out this value
				//arInfo.Add ("AppStoreFileSize", new PNumber (65535));
				var props = new PDictionary ();
				props.Add ("ApplicationPath", new PString (string.Format ("Applications/{0}", Path.GetFileName (AppBundleDir.ItemSpec))));
				props.Add ("CFBundleIdentifier", new PString (plist.GetCFBundleIdentifier ()));

				var version = plist.GetCFBundleShortVersionString ();
				var build = plist.GetCFBundleVersion ();
				props.Add ("CFBundleShortVersionString", new PString (version ?? (build ?? "1.0")));
				props.Add ("CFBundleVersion", new PString (build ?? "1.0"));

				var iconFiles = plist.GetCFBundleIconFiles ();
				var iconDict = plist.GetCFBundleIcons ();
				var icons = new PArray ();

				if (iconFiles is not null)
					AddIconPaths (icons, iconFiles, Path.Combine (archiveDir, "Products"));

				if (iconDict is not null) {
					var primary = iconDict.Get<PDictionary> (ManifestKeys.CFBundlePrimaryIcon);
					if (primary is not null && (iconFiles = primary.GetCFBundleIconFiles ()) is not null)
						AddIconPaths (icons, iconFiles, Path.Combine (archiveDir, "Products"));
				}

				if (icons.Count > 0)
					props.Add ("IconPaths", icons);

				if (!string.IsNullOrEmpty (SigningKey))
					props.Add ("SigningIdentity", SigningKey);

				arInfo.Add ("ApplicationProperties", props);
				arInfo.Add ("ArchiveVersion", new PNumber (2));
				arInfo.Add ("CreationDate", new PDate (Now.ToUniversalTime ()));
				arInfo.Add ("Name", new PString (plist.GetCFBundleName () ?? plist.GetCFBundleDisplayName ()));

				if (!string.IsNullOrEmpty (ProjectGuid))
					arInfo.Add ("ProjectGuid", new PString (ProjectGuid));

				if (!string.IsNullOrEmpty (ProjectTypeGuids))
					arInfo.Add ("ProjectTypeGuids", new PString (ProjectTypeGuids));

				if (!string.IsNullOrEmpty (SolutionPath)) {
					arInfo.Add ("SolutionName", new PString (Path.GetFileNameWithoutExtension (SolutionPath)));
					arInfo.Add ("SolutionPath", new PString (SolutionPath));
				}

				if (!string.IsNullOrEmpty (InsightsApiKey))
					arInfo.Add ("InsightsApiKey", new PString (InsightsApiKey));

				arInfo.Save (Path.Combine (archiveDir, "Info.plist"));
				ArchiveDir = archiveDir;
			} catch (Exception ex) {
				Log.LogErrorFromException (ex);
				Directory.Delete (archiveDir, true);
			}

			return !Log.HasLoggedErrors;
		}

		protected string CreateArchiveDirectory ()
		{
			var timestamp = Now.ToString ("M-dd-yy h.mm tt", CultureInfo.InvariantCulture);
			var folder = Now.ToString ("yyyy-MM-dd");
			var baseArchiveDir = XcodeArchivesDir;
			string archiveDir, name;
			int unique = 1;

			do {
				if (unique > 1)
					name = string.Format ("{0} {1} {2}.xcarchive", ProjectName, timestamp, unique);
				else
					name = string.Format ("{0} {1}.xcarchive", ProjectName, timestamp);

				archiveDir = Path.Combine (baseArchiveDir, folder, name);
				unique++;
			} while (Directory.Exists (archiveDir));

			Directory.CreateDirectory (archiveDir);

			return archiveDir;
		}

		void ArchiveAppExtension (ITaskItem appex, string archiveDir)
		{
			var plist = PDictionary.FromFile (Path.Combine (appex.ItemSpec, "Info.plist"));
			string watchAppBundleDir;

			if (IsWatchAppExtension (appex, plist, out watchAppBundleDir)) {
				var wk = Path.Combine (watchAppBundleDir, "_WatchKitStub", "WK");
				var supportDir = Path.Combine (archiveDir, "WatchKitSupport");

				if (File.Exists (wk) && !Directory.Exists (supportDir)) {
					Directory.CreateDirectory (supportDir);
					File.Copy (wk, Path.Combine (supportDir, "WK"), true);
				}
			}

			// Note: App Extension dSYM dirs exist alongside the main app bundle now that they are generated from the main app's MSBuild targets
			var dsymDir = Path.Combine (Path.GetDirectoryName (AppBundleDir.ItemSpec), Path.GetFileName (appex.ItemSpec) + ".dSYM");
			ArchiveDSym (dsymDir, archiveDir);

			var msymDir = appex.ItemSpec + ".mSYM";
			ArchiveMSym (msymDir, archiveDir);
		}

		protected void ArchiveDSym (string dsymDir, string archiveDir)
		{
			if (Directory.Exists (dsymDir)) {
				var destDir = Path.Combine (archiveDir, "dSYMs", Path.GetFileName (dsymDir));
				Ditto (dsymDir, destDir);
			}
		}

		protected void ArchiveMSym (string msymDir, string archiveDir)
		{
			if (Directory.Exists (msymDir)) {
				var destDir = Path.Combine (archiveDir, "mSYMs", Path.GetFileName (msymDir));
				Ditto (msymDir, destDir);
			}
		}

		void ArchiveWatchApp (ITaskItem watchApp, string archiveDir)
		{
			var wk = Path.Combine (watchApp.ItemSpec, "_WatchKitStub", "WK");
			var supportDir = Path.Combine (archiveDir, "WatchKitSupport2");

			if (File.Exists (wk) && !Directory.Exists (supportDir)) {
				Directory.CreateDirectory (supportDir);
				File.Copy (wk, Path.Combine (supportDir, "WK"), true);
			}

			var dsymDir = watchApp.ItemSpec + ".dSYM";
			ArchiveDSym (dsymDir, archiveDir);

			var msymDir = watchApp.ItemSpec + ".mSYM";
			ArchiveMSym (msymDir, archiveDir);
		}

		protected static int Ditto (string source, string destination)
		{
			var args = new CommandLineArgumentBuilder ();

			args.AddQuoted (source);
			args.AddQuoted (destination);

			var psi = new ProcessStartInfo ("/usr/bin/ditto", args.ToString ()) {
				RedirectStandardOutput = false,
				RedirectStandardError = false,
				UseShellExecute = false,
				CreateNoWindow = true,
			};

			using (var process = Process.Start (psi)) {
				process.WaitForExit ();

				return process.ExitCode;
			}
		}

		static bool IsWatchAppExtension (ITaskItem appex, PDictionary plist, out string watchAppBundleDir)
		{
			PString expectedBundleIdentifier, bundleIdentifier, extensionPoint;
			PDictionary extension, attributes;

			watchAppBundleDir = null;

			if (!plist.TryGetValue ("NSExtension", out extension))
				return false;

			if (!extension.TryGetValue ("NSExtensionPointIdentifier", out extensionPoint))
				return false;

			if (extensionPoint.Value != "com.apple.watchkit")
				return false;

			// Okay, we've found the WatchKit App Extension...
			if (!extension.TryGetValue ("NSExtensionAttributes", out attributes))
				return false;

			if (!attributes.TryGetValue ("WKAppBundleIdentifier", out expectedBundleIdentifier))
				return false;

			var pwd = PathUtils.ResolveSymbolicLinks (Environment.CurrentDirectory);

			// Scan the *.app subdirectories to find the WatchApp bundle...
			foreach (var bundle in Directory.GetDirectories (appex.ItemSpec, "*.app")) {
				if (!File.Exists (Path.Combine (bundle, "Info.plist")))
					continue;

				plist = PDictionary.FromFile (Path.Combine (bundle, "Info.plist"));

				if (!plist.TryGetValue ("CFBundleIdentifier", out bundleIdentifier))
					continue;

				if (bundleIdentifier.Value != expectedBundleIdentifier.Value)
					continue;

				watchAppBundleDir = PathUtils.AbsoluteToRelative (pwd, PathUtils.ResolveSymbolicLinks (bundle));

				return true;
			}

			return false;
		}

		void AddIconPaths (PArray icons, PArray iconFiles, string productsDir)
		{
			foreach (var icon in iconFiles.Cast<PString> ().Where (p => p.Value is not null)) {
				var path = string.Format ("Applications/{0}/{1}", Path.GetFileName (AppBundleDir.ItemSpec), icon.Value);
				bool addDefault = true;

				if (path.EndsWith (".png", StringComparison.Ordinal)) {
					icons.Add (new PString (path));
					continue;
				}

				if (File.Exists (Path.Combine (productsDir, path + "@3x.png"))) {
					icons.Add (new PString (path + "@3x.png"));
					addDefault = false;
				}

				if (File.Exists (Path.Combine (productsDir, path + "@2x.png"))) {
					icons.Add (new PString (path + "@2x.png"));
					addDefault = false;
				}

				if (addDefault || File.Exists (Path.Combine (productsDir, path + ".png")))
					icons.Add (new PString (path + ".png"));
			}
		}

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}
	}
}
