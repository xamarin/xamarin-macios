using System;
using System.IO;
using System.Diagnostics;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev;
using Xamarin.MacDev.Tasks;

namespace Xamarin.Mac.Tasks
{
	public abstract class ArchiveTaskBase : Xamarin.MacDev.Tasks.ArchiveTaskBase
	{

		[Required]
		public string CustomBundleName { get; set; } // default is `MonoBundle` but that can be changed with `_CustomBundleName`

		public override bool Execute ()
		{
			var archiveDir = CreateArchiveDirectory ();

			try {
				var plist = PDictionary.FromFile (PlatformFrameworkHelper.GetAppManifestPath (Platform, AppBundleDir.ItemSpec));
				var productsDir = Path.Combine (archiveDir, "Products");

				// Archive the Applications...
				var appDestDir = Path.Combine (productsDir, "Applications", Path.GetFileName (AppBundleDir.ItemSpec));
				Ditto (AppBundleDir.ItemSpec, appDestDir);

				// Archive the main dSYM...
				ArchiveDSym (DSYMDir, archiveDir);

				// for each `.dylib` (file) inside `MonoBundle` there could be a corresponding `.dSYM` - e.g. when using an AOT mode
				foreach (var dylib in Directory.GetFiles (Path.Combine (AppBundleDir.ItemSpec, "Contents", CustomBundleName), "*.dylib")) {
					var dsym = Path.Combine (AppBundleDir.ItemSpec, "..", Path.GetFileName (dylib) + ".dSYM");
					ArchiveDSym (dsym, archiveDir);
				}

				// for each user framework (directory) that is bundled inside the app we must also archive their dSYMs, if available
				var fks = Path.Combine (AppBundleDir.ItemSpec, "Contents", "Frameworks");
				if (Directory.Exists (fks)) {
					foreach (var fx in Directory.GetDirectories (fks, "*.framework")) {
						var dsym = Path.Combine (AppBundleDir.ItemSpec, "..", Path.GetFileName (fx) + ".dSYM");
						ArchiveDSym (dsym, archiveDir);
					}
				}

				// Generate an archive Info.plist
				var arInfo = new PDictionary ();
				var props = new PDictionary ();
				props.Add ("ApplicationPath", new PString (string.Format ("Applications/{0}", Path.GetFileName (AppBundleDir.ItemSpec))));
				props.Add ("CFBundleIdentifier", new PString (plist.GetCFBundleIdentifier ()));

				var version = plist.GetCFBundleShortVersionString ();
				var build = plist.GetCFBundleVersion ();
				props.Add ("CFBundleShortVersionString", new PString (version ?? (build ?? "1.0")));
				props.Add ("CFBundleVersion", new PString (build ?? "1.0"));
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
	}
}
