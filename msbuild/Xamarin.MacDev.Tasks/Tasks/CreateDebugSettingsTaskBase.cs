using System;
using System.IO;
using System.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;
using Xamarin.MacDev;
using Xamarin.Localization.MSBuild;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.MacDev.Tasks {
	public abstract class CreateDebugSettingsTaskBase : XamarinTask {
		#region Inputs

		[Required]
		public string AppBundleDir { get; set; }

		[Required]
		public string DebuggerPort { get; set; }

		#endregion

		public override bool Execute ()
		{
			PDictionary plist;

			var path = Path.Combine (AppBundleDir, "Settings.bundle", "Root.plist");

			if (File.Exists (path)) {
				try {
					plist = PDictionary.FromFile (path);
				} catch (Exception ex) {
					Log.LogError (MSBStrings.E0024, Path.GetFileName (AppBundleDir), ex.Message);
					return false;
				}
			} else {
				plist = new PDictionary ();
			}

			plist.SetIfNotPresent ("Title", "AppSettings");
			plist.SetIfNotPresent ("StringsTable", "Root");

			var specifiers = plist.GetArray ("PreferenceSpecifiers");

			// check that we haven't already merged in our debug settings...
			foreach (var specifier in specifiers.OfType<PDictionary> ()) {
				if (specifier.Get<PString> ("Type") == "PSToggleSwitchSpecifier" && specifier.Get<PString> ("Key") == "__monotouch_debug_enabled")
					return !Log.HasLoggedErrors;
			}

			specifiers.Add (new PDictionary {
				{ "Type", "PSGroupSpecifier" },
				{ "Title", "Debug Settings" }
			});

			specifiers.Add (new PDictionary {
				{ "Type", "PSToggleSwitchSpecifier" },
				{ "Title", "Enabled" },
				{ "Key", "__monotouch_debug_enabled" },
				{ "DefaultValue", "1" },
				{ "TrueValue", "1" },
				{ "FalseValue", "0" }
			});

			specifiers.Add (new PDictionary {
				{ "Type", "PSTextFieldSpecifier" },
				{ "Title","Xamarin Studio Host" },
				{ "Key", "__monodevelop_host" },
				{ "AutocapitalizationType", "None" },
				{ "AutocorrectionType", "No" },
				{ "DefaultValue", "automatic" }
			});

			if (!Directory.Exists (Path.GetDirectoryName (path)))
				Directory.CreateDirectory (Path.GetDirectoryName (path));

			plist.Save (path, true, true);

			return !Log.HasLoggedErrors;
		}
	}
}
