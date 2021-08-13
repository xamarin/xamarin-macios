using System;
using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.Localization.MSBuild;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks
{
	public abstract class CompileAppManifestTaskBase : XamarinTask
	{
		#region Inputs

		// Single-project property that maps to CFBundleShortVersionString for Apple platforms
		public string AppleShortVersion { get; set; }

		// Single-project property that maps to CFBundleDisplayName for Apple platforms
		public string ApplicationTitle { get; set; }

		// Single-project property that maps to CFBundleVersion for Apple platforms
		public string ApplicationVersion { get; set; }

		[Required]
		public string AppBundleName { get; set; }

		[Required]
		public string AppManifest { get; set; }

		[Required]
		public string AssemblyName { get; set; }

		[Required]
		public string BundleIdentifier { get; set; }

		[Required]
		[Output] // This is required to create an empty file on Windows for the Input/Outputs check.
		public string CompiledAppManifest { get; set; }

		[Required]
		public bool Debug { get; set; }

		public string DebugIPAddresses { get; set; }

		[Required]
		public string DefaultSdkVersion { get; set; }

		// Single-project property that determines whether other single-project properties should have any effect
		public bool GenerateApplicationManifest { get; set; }

		[Required]
		public bool IsAppExtension { get; set; }

		public bool IsXPCService { get; set; }

		public bool IsWatchApp { get; set; }

		public bool IsWatchExtension { get; set; }

		[Required]
		public string MinimumOSVersion { get; set; }

		public ITaskItem[] PartialAppManifests { get; set; }

		public string ResourceRules { get; set; }

		[Required]
		public string SdkPlatform { get; set; }

		[Required]
		public bool SdkIsSimulator { get; set; }

		public string TargetArchitectures { get; set; }
		#endregion

		protected TargetArchitecture architectures;

		public override bool Execute ()
		{
			PDictionary plist;

			try {
				plist = PDictionary.FromFile (AppManifest);
			} catch (Exception ex) {
				LogAppManifestError (MSBStrings.E0010, AppManifest, ex.Message);
				return false;
			}

			plist.SetIfNotPresent (PlatformFrameworkHelper.GetMinimumOSVersionKey (Platform), MinimumOSVersion);

			if (!string.IsNullOrEmpty (TargetArchitectures) && !Enum.TryParse (TargetArchitectures, out architectures)) {
				LogAppManifestError (MSBStrings.E0012, TargetArchitectures);
				return false;
			}

			plist.SetCFBundleIdentifier (BundleIdentifier); // no ifs and buts, we've computed the final bundle identifier (BundleIdentifier) in DetectSigningIdentityTask.
			plist.SetIfNotPresent (ManifestKeys.CFBundleInfoDictionaryVersion, "6.0");
			plist.SetIfNotPresent (ManifestKeys.CFBundlePackageType, IsAppExtension ? "XPC!" : "APPL");
			plist.SetIfNotPresent (ManifestKeys.CFBundleSignature, "????");
			plist.SetIfNotPresent (ManifestKeys.CFBundleExecutable, AssemblyName);
			plist.SetIfNotPresent (ManifestKeys.CFBundleName, AppBundleName);

			if (GenerateApplicationManifest && !string.IsNullOrEmpty (ApplicationTitle))
				plist.SetIfNotPresent (ManifestKeys.CFBundleDisplayName, ApplicationTitle);

			string defaultBundleVersion = "1.0";
			if (GenerateApplicationManifest && !string.IsNullOrEmpty (ApplicationVersion))
				defaultBundleVersion = ApplicationVersion;
			plist.SetIfNotPresent (ManifestKeys.CFBundleVersion, defaultBundleVersion);

			string defaultBundleShortVersion = null;
			if (GenerateApplicationManifest) {
				if (!string.IsNullOrEmpty (AppleShortVersion))
					defaultBundleShortVersion = AppleShortVersion;
				else if (!string.IsNullOrEmpty (ApplicationVersion))
					defaultBundleShortVersion = ApplicationVersion;
			}
			if (string.IsNullOrEmpty (defaultBundleShortVersion))
				defaultBundleShortVersion = plist.GetCFBundleVersion ();
			plist.SetIfNotPresent (ManifestKeys.CFBundleShortVersionString, defaultBundleShortVersion);
			
			if (!Compile (plist))
				return false;

			// Merge with any partial plists...
			MergePartialPlistTemplates (plist);

			plist.Save (CompiledAppManifest, true, true);

			return !Log.HasLoggedErrors;
		}

		protected abstract bool Compile (PDictionary plist);

		protected void LogAppManifestError (string format, params object[] args)
		{
			// Log an error linking to the Info.plist file
			Log.LogError (null, null, null, AppManifest, 0, 0, 0, 0, format, args);
		}

		protected void LogAppManifestWarning (string format, params object[] args)
		{
			// Log a warning linking to the Info.plist file
			Log.LogWarning (null, null, null, AppManifest, 0, 0, 0, 0, format, args);
		}

		protected void SetValue (PDictionary dict, string key, string value)
		{
			if (dict.ContainsKey (key))
				return;

			if (string.IsNullOrEmpty (value))
				LogAppManifestWarning (MSBStrings.W0106, key);
			else
				dict[key] = value;
		}

		protected void MergePartialPlistDictionary (PDictionary plist, PDictionary partial)
		{
			foreach (var property in partial) {
				if (plist.ContainsKey (property.Key)) {
					var value = plist[property.Key];

					if (value is PDictionary && property.Value is PDictionary) {
						MergePartialPlistDictionary ((PDictionary) value, (PDictionary) property.Value);
					} else {
						plist[property.Key] = property.Value.Clone ();
					}
				} else {
					plist[property.Key] = property.Value.Clone ();
				}
			}
		}

		protected void MergePartialPlistTemplates (PDictionary plist)
		{
			if (PartialAppManifests == null)
				return;

			foreach (var template in PartialAppManifests) {
				PDictionary partial;

				try {
					partial = PDictionary.FromFile (template.ItemSpec);
				} catch (Exception ex) {
					Log.LogError (MSBStrings.E0107, template.ItemSpec, ex.Message);
					continue;
				}

				MergePartialPlistDictionary (plist, partial);
			}
		}
	}
}
