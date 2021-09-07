using System;
using System.Collections.Generic;
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

		// Single-project property that maps to CFBundleIdentifier for Apple platforms
		public string ApplicationId { get; set; }

		// Single-project property that maps to CFBundleShortVersionString for Apple platforms
		public string AppleShortVersion { get; set; }

		// Single-project property that maps to CFBundleDisplayName for Apple platforms
		public string ApplicationTitle { get; set; }

		// Single-project property that maps to CFBundleVersion for Apple platforms
		public string ApplicationVersion { get; set; }

		[Required]
		public string AppBundleName { get; set; }

		public string AppManifest { get; set; }

		[Required]
		public string AssemblyName { get; set; }

		[Required]
		[Output] // This is required to create an empty file on Windows for the Input/Outputs check.
		public ITaskItem CompiledAppManifest { get; set; }

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

		public ITaskItem[] PartialAppManifests { get; set; }

		public string ResourceRules { get; set; }

		[Required]
		public string SdkPlatform { get; set; }

		[Required]
		public bool SdkIsSimulator { get; set; }

		[Required]
		public string SdkVersion { get; set; }

		public string SupportedOSPlatformVersion { get; set; }

		public string TargetArchitectures { get; set; }

		public bool Validate { get; set; }
		#endregion

		protected TargetArchitecture architectures;

		public override bool Execute ()
		{
			PDictionary plist = null;

			if (File.Exists (AppManifest)) {
				try {
					plist = PDictionary.FromFile (AppManifest);
				} catch (Exception ex) {
					LogAppManifestError (MSBStrings.E0010, AppManifest, ex.Message);
					return false;
				}
			} else {
				plist = new PDictionary ();
			}

			if (!string.IsNullOrEmpty (TargetArchitectures) && !Enum.TryParse (TargetArchitectures, out architectures)) {
				LogAppManifestError (MSBStrings.E0012, TargetArchitectures);
				return false;
			}

			if (GenerateApplicationManifest && !string.IsNullOrEmpty (ApplicationId))
				plist.SetIfNotPresent (ManifestKeys.CFBundleIdentifier, ApplicationId);
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

			if (!SetMinimumOSVersion (plist))
				return false;

			if (!Compile (plist))
				return false;

			// Merge with any partial plists...
			MergePartialPlistTemplates (plist);

			// write the resulting app manifest
			if (FileUtils.UpdateFile (CompiledAppManifest.ItemSpec, (tmpfile) => plist.Save (tmpfile, true, true)))
				Log.LogMessage (MessageImportance.Low, "The file {0} is up-to-date.", CompiledAppManifest.ItemSpec);

			return !Log.HasLoggedErrors;
		}

		bool SetMinimumOSVersion (PDictionary plist)
		{
			var minimumVersionKey = PlatformFrameworkHelper.GetMinimumOSVersionKey (Platform);
			var minimumOSVersionInManifest = plist?.Get<PString> (minimumVersionKey)?.Value;
			string convertedSupportedOSPlatformVersion;
			string minimumOSVersion;

			if (Platform == ApplePlatform.MacCatalyst && !string.IsNullOrEmpty (SupportedOSPlatformVersion)) {
				// SupportedOSPlatformVersion is the iOS version for Mac Catalyst.
				// But we need to store the macOS version in the app manifest, so convert it to the macOS version here.
				if (!MacCatalystSupport.TryGetMacOSVersion (Sdks.GetAppleSdk (Platform).GetSdkPath (SdkVersion, false), SupportedOSPlatformVersion, out var convertedVersion))
					Log.LogError (MSBStrings.E0188, SupportedOSPlatformVersion);
				convertedSupportedOSPlatformVersion = convertedVersion;
			} else {
				convertedSupportedOSPlatformVersion = SupportedOSPlatformVersion;
			}

			if (Platform == ApplePlatform.MacCatalyst && string.IsNullOrEmpty (minimumOSVersionInManifest)) {
				// If there was no value for the macOS min version key, then check the iOS min version key.
				var minimumiOSVersionInManifest = plist?.Get<PString> (ManifestKeys.MinimumOSVersion)?.Value;
				if (!string.IsNullOrEmpty (minimumiOSVersionInManifest)) {
					// Convert to the macOS version
					if (!MacCatalystSupport.TryGetMacOSVersion (Sdks.GetAppleSdk (Platform).GetSdkPath (SdkVersion, false), minimumiOSVersionInManifest, out var convertedVersion))
						Log.LogError (MSBStrings.E0188, minimumiOSVersionInManifest);
					minimumOSVersionInManifest = convertedVersion;
				}
			}

			if (string.IsNullOrEmpty (minimumOSVersionInManifest)) {
				// Nothing is specified in the Info.plist - use SupportedOSPlatformVersion, and if that's not set, then use the sdkVersion
				if (!string.IsNullOrEmpty (convertedSupportedOSPlatformVersion)) {
					minimumOSVersion = convertedSupportedOSPlatformVersion;
				} else {
					minimumOSVersion = SdkVersion;
				}
			} else if (!IAppleSdkVersion_Extensions.TryParse (minimumOSVersionInManifest, out var _)) {
				Log.LogError (null, null, null, AppManifest, 0, 0, 0, 0, MSBStrings.E0011, minimumOSVersionInManifest);
				return false;
			} else if (!string.IsNullOrEmpty (convertedSupportedOSPlatformVersion) && convertedSupportedOSPlatformVersion != minimumOSVersionInManifest) {
				// SupportedOSPlatformVersion and the value in the Info.plist are not the same. This is an error.
				Log.LogError (null, null, null, AppManifest, 0, 0, 0, 0, MSBStrings.E7082, minimumVersionKey, minimumOSVersionInManifest, SupportedOSPlatformVersion);
				return false;
			} else {
				minimumOSVersion = minimumOSVersionInManifest;
			}

			// Write out our value
			plist [minimumVersionKey] = minimumOSVersion;

			return true;
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

		public static void MergePartialPlistDictionary (PDictionary plist, PDictionary partial)
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

		public static void MergePartialPLists (Task task, PDictionary plist, IEnumerable<ITaskItem> partialLists)
		{
			if (partialLists == null)
				return;

			foreach (var template in partialLists) {
				PDictionary partial;

				try {
					partial = PDictionary.FromFile (template.ItemSpec);
				} catch (Exception ex) {
					task.Log.LogError (MSBStrings.E0107, template.ItemSpec, ex.Message);
					continue;
				}

				MergePartialPlistDictionary (plist, partial);
			}
		}

		protected void MergePartialPlistTemplates (PDictionary plist)
		{
			MergePartialPLists (this, plist, PartialAppManifests);
		}
	}
}
