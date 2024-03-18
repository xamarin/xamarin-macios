using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.Localization.MSBuild;
using Xamarin.Messaging.Build.Client;
using Xamarin.Utils;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public class CompileAppManifest : XamarinTask, ITaskCallback, ICancelableTask {
		#region Inputs

		// Single-project property that maps to CFBundleIdentifier for Apple platforms
		public string ApplicationId { get; set; } = String.Empty;

		// Single-project property that maps to CFBundleShortVersionString for Apple platforms
		public string ApplicationDisplayVersion { get; set; } = String.Empty;

		// Single-project property that maps to CFBundleDisplayName for Apple platforms
		public string ApplicationTitle { get; set; } = String.Empty;

		// Single-project property that maps to CFBundleVersion for Apple platforms
		public string ApplicationVersion { get; set; } = String.Empty;

		[Required]
		public string AppBundleName { get; set; } = String.Empty;

		// This must be an ITaskItem to copy the file to Windows for remote builds.
		public ITaskItem? AppManifest { get; set; }

		[Required]
		public string AssemblyName { get; set; } = String.Empty;

		[Required]
		[Output] // This is required to create an empty file on Windows for the Input/Outputs check.
		public ITaskItem? CompiledAppManifest { get; set; }

		[Required]
		public bool Debug { get; set; }

		public string DebugIPAddresses { get; set; } = String.Empty;

		public string DefaultSdkVersion { get; set; } = String.Empty;

		public ITaskItem [] FontFilesToRegister { get; set; } = Array.Empty<ITaskItem> ();

		// Single-project property that determines whether other single-project properties should have any effect
		public bool GenerateApplicationManifest { get; set; }

		[Required]
		public bool IsAppExtension { get; set; }

		public bool IsXPCService { get; set; }

		public bool IsWatchApp { get; set; }

		public bool IsWatchExtension { get; set; }

		public ITaskItem [] PartialAppManifests { get; set; } = Array.Empty<ITaskItem> ();

		[Required]
		public string ProjectDir { get; set; } = String.Empty;

		[Required]
		public string ResourcePrefix { get; set; } = String.Empty;

		public string ResourceRules { get; set; } = String.Empty;

		[Required]
		public bool SdkIsSimulator { get; set; }

		public string SdkVersion { get; set; } = String.Empty;

		public string SupportedOSPlatformVersion { get; set; } = String.Empty;

		public string TargetArchitectures { get; set; } = String.Empty;

		public bool Validate { get; set; }
		#endregion

		protected TargetArchitecture architectures;
		IPhoneDeviceType supportedDevices;
		AppleSdkVersion sdkVersion;

		public string SdkPlatform {
			get {
				return GetSdkPlatform (SdkIsSimulator);
			}
		}

		bool OnWindows {
			get => Environment.OSVersion.Platform == PlatformID.Win32NT;
		}

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			PDictionary plist;

			var appManifest = AppManifest?.ItemSpec;
			if (appManifest is null || string.IsNullOrEmpty (appManifest)) {
				plist = new PDictionary ();
			} else if (File.Exists (appManifest)) {
				try {
					plist = PDictionary.FromFile (appManifest)!;
				} catch (Exception ex) {
					LogAppManifestError (MSBStrings.E0010, appManifest, ex.Message);
					return false;
				}
			} else {
				LogAppManifestWarning (MSBStrings.W7108 /* The file '{0}' does not exist. */, appManifest);
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

			string? defaultBundleShortVersion = null;
			if (GenerateApplicationManifest) {
				if (!string.IsNullOrEmpty (ApplicationDisplayVersion))
					defaultBundleShortVersion = ApplicationDisplayVersion;
				else if (!string.IsNullOrEmpty (ApplicationVersion))
					defaultBundleShortVersion = ApplicationVersion;
			}
			if (string.IsNullOrEmpty (defaultBundleShortVersion))
				defaultBundleShortVersion = plist.GetCFBundleVersion ();
			plist.SetIfNotPresent (ManifestKeys.CFBundleShortVersionString, defaultBundleShortVersion);

			RegisterFonts (plist);

			if (!SetMinimumOSVersion (plist))
				return false;

			if (!Compile (plist))
				return false;

			AddXamarinVersionNumber (plist);

			// Merge with any partial plists...
			MergePartialPlistTemplates (plist);

			Validation (plist);

			// write the resulting app manifest
			if (FileUtils.UpdateFile (CompiledAppManifest!.ItemSpec, (tmpfile) => plist.Save (tmpfile, true, true)))
				Log.LogMessage (MessageImportance.Low, "The file {0} is up-to-date.", CompiledAppManifest.ItemSpec);

			return !Log.HasLoggedErrors;
		}

		void AddXamarinVersionNumber (PDictionary plist)
		{
			// Add our own version number
			if (IsWatchApp)
				return;

			string name;
			string value;

			// This key is our supported way of determining if an app
			// was built with Xamarin, so it needs to be present in all apps.
			if (TargetFramework.IsDotNet) {
				value = DotNetVersion;
				name = "com.microsoft." + Platform.AsString ().ToLowerInvariant ();
			} else if (Platform != ApplePlatform.MacOSX) {
				var version = Sdks.XamIOS.ExtendedVersion;
				value = string.Format ("{0} ({1}: {2})", version.Version, version.Branch, version.Hash);
				name = "com.xamarin.ios";
			} else {
				return;
			}

			var dict = new PDictionary ();
			dict.Add ("Version", new PString (value));
			plist.Add (name, dict);
		}

		void RegisterFonts (PDictionary plist)
		{
			if (FontFilesToRegister is null || FontFilesToRegister.Length == 0)
				return;

			// https://developer.apple.com/documentation/swiftui/applying-custom-fonts-to-text

			// Compute the relative location in the app bundle for each font file
			var prefixes = BundleResource.SplitResourcePrefixes (ResourcePrefix);
			const string logicalNameKey = "_ComputedLogicalName_";
			foreach (var item in FontFilesToRegister) {
				var logicalName = BundleResource.GetLogicalName (ProjectDir, prefixes, item, !string.IsNullOrEmpty (SessionId));
				item.SetMetadata (logicalNameKey, logicalName);
			}

			switch (Platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
			case ApplePlatform.WatchOS:
			case ApplePlatform.MacCatalyst:
				// Fonts are listed in the Info.plist in a UIAppFonts entry for iOS, tvOS, watchOS and Mac Catalyst.
				var uiAppFonts = plist.GetArray ("UIAppFonts");
				if (uiAppFonts is null) {
					uiAppFonts = new PArray ();
					plist ["UIAppFonts"] = uiAppFonts;
				}
				foreach (var item in FontFilesToRegister)
					uiAppFonts.Add (new PString (item.GetMetadata (logicalNameKey)));
				break;
			case ApplePlatform.MacOSX:
				// The directory where the fonts are located is in the Info.plist in the ATSApplicationFontsPath entry for macOS.
				// It's relative to the Resources directory.
				// Make sure that all the fonts are in the same directory in the app bundle
				var allSubdirectories = FontFilesToRegister.Select (v => Path.GetDirectoryName (v.GetMetadata (logicalNameKey)));
				var distinctSubdirectories = allSubdirectories.Distinct ().ToArray ();
				if (distinctSubdirectories.Length > 1) {
					Log.LogError (MSBStrings.E7083 /* "All font files must be located in the same directory in the app bundle. The following font files have different target directories in the app bundle:" */, CompiledAppManifest!.ItemSpec);
					foreach (var fonts in FontFilesToRegister)
						Log.LogError (null, null, null, fonts.ItemSpec, 0, 0, 0, 0, MSBStrings.E7084 /* "The target directory is {0}" */, fonts.GetMetadata (logicalNameKey));
				} else {
					plist.SetIfNotPresent ("ATSApplicationFontsPath", string.IsNullOrEmpty (distinctSubdirectories [0]) ? "." : distinctSubdirectories [0]);
				}
				break;
			default:
				throw new InvalidOperationException (string.Format (MSBStrings.InvalidPlatform, Platform));
			}
		}

		bool SetMinimumOSVersion (PDictionary plist)
		{
			var minimumVersionKey = PlatformFrameworkHelper.GetMinimumOSVersionKey (Platform);
			var minimumOSVersionInManifest = plist.Get<PString> (minimumVersionKey)?.Value;
			string convertedSupportedOSPlatformVersion;
			string minimumOSVersion;

			if (Platform == ApplePlatform.MacCatalyst && !string.IsNullOrEmpty (SupportedOSPlatformVersion)) {
				// SupportedOSPlatformVersion is the iOS version for Mac Catalyst.
				// But we need to store the macOS version in the app manifest, so convert it to the macOS version here.
				if (!MacCatalystSupport.TryGetMacOSVersion (Sdks.GetAppleSdk (Platform).GetSdkPath (SdkVersion), SupportedOSPlatformVersion, out var convertedVersion, out var knowniOSVersions)) {
					Log.LogError (MSBStrings.E0188, SupportedOSPlatformVersion, string.Join (", ", knowniOSVersions));
					return false;
				}
				convertedSupportedOSPlatformVersion = convertedVersion;
			} else {
				convertedSupportedOSPlatformVersion = SupportedOSPlatformVersion;
			}

			if (Platform == ApplePlatform.MacCatalyst && string.IsNullOrEmpty (minimumOSVersionInManifest)) {
				// If there was no value for the macOS min version key, then check the iOS min version key.
				var minimumiOSVersionInManifest = plist.Get<PString> (ManifestKeys.MinimumOSVersion)?.Value;
				if (!string.IsNullOrEmpty (minimumiOSVersionInManifest)) {
					// Convert to the macOS version
					if (!MacCatalystSupport.TryGetMacOSVersion (Sdks.GetAppleSdk (Platform).GetSdkPath (SdkVersion), minimumiOSVersionInManifest!, out var convertedVersion, out var knowniOSVersions)) {
						Log.LogError (MSBStrings.E0188, minimumiOSVersionInManifest, string.Join (", ", knowniOSVersions));
						return false;
					}
					minimumOSVersionInManifest = convertedVersion;
				}
			}

			if (string.IsNullOrEmpty (minimumOSVersionInManifest)) {
				// Nothing is specified in the Info.plist - use SupportedOSPlatformVersion, and if that's not set, then use the sdkVersion
				if (!string.IsNullOrEmpty (convertedSupportedOSPlatformVersion)) {
					minimumOSVersion = convertedSupportedOSPlatformVersion;
				} else if (OnWindows && string.IsNullOrEmpty (SdkVersion)) {
					// When building on Windows (Hot Restart), we're not using any Xcode version, so there's no SdkVersion either, so use the min OS version we support if the project doesn't specify anything.
					minimumOSVersion = Xamarin.SdkVersions.GetMinVersion (Platform).ToString ();
				} else {
					minimumOSVersion = SdkVersion;
				}
			} else if (!IAppleSdkVersion_Extensions.TryParse (minimumOSVersionInManifest, out var _)) {
				LogAppManifestError (MSBStrings.E0011, minimumOSVersionInManifest!);
				return false;
			} else if (!string.IsNullOrEmpty (convertedSupportedOSPlatformVersion) && convertedSupportedOSPlatformVersion != minimumOSVersionInManifest) {
				// SupportedOSPlatformVersion and the value in the Info.plist are not the same. This is an error.
				LogAppManifestError (MSBStrings.E7082, minimumVersionKey, minimumOSVersionInManifest!, SupportedOSPlatformVersion);
				return false;
			} else {
				minimumOSVersion = minimumOSVersionInManifest!;
			}

			// Write out our value
			plist [minimumVersionKey] = minimumOSVersion;

			return true;
		}

		protected string? GetMinimumOSVersion (PDictionary plist, out Version version)
		{
			var rv = plist.Get<PString> (PlatformFrameworkHelper.GetMinimumOSVersionKey (Platform))?.Value;
			Version.TryParse (rv, out version);
			return rv;
		}

		bool Compile (PDictionary plist)
		{
			if (!OnWindows) {
				if (string.IsNullOrEmpty (DefaultSdkVersion)) {
					Log.LogError (MSBStrings.E7114 /* The "{0}" task was not given a value for the parameter "{1}", which is required when building on this platform. */, GetType ().Name, "DefaultSdkVersion");
					return false;
				}

				var currentSDK = Sdks.GetAppleSdk (Platform);

				sdkVersion = AppleSdkVersion.Parse (DefaultSdkVersion);
				if (!currentSDK.SdkIsInstalled (sdkVersion, SdkIsSimulator)) {
					Log.LogError (null, null, null, null, 0, 0, 0, 0, MSBStrings.E0013, Platform, sdkVersion);
					return false;
				}
				SetXcodeValues (plist, currentSDK);
			}

			switch (Platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
			case ApplePlatform.WatchOS:
			case ApplePlatform.MacCatalyst:
				return CompileMobile (plist);
			case ApplePlatform.MacOSX:
				return CompileMac (plist);
			default:
				throw new InvalidOperationException (string.Format (MSBStrings.InvalidPlatform, Platform));
			}
		}

		protected void LogAppManifestError (string format, params object [] args)
		{
			// Log an error linking to the Info.plist file
			if (AppManifest is not null) {
				Log.LogError (null, null, null, AppManifest.ItemSpec, 0, 0, 0, 0, format, args);
			} else {
				Log.LogError (format, args);
			}

		}

		protected void LogAppManifestWarning (string format, params object [] args)
		{
			// Log a warning linking to the Info.plist file
			if (AppManifest is not null) {
				Log.LogWarning (null, null, null, AppManifest.ItemSpec, 0, 0, 0, 0, format, args);
			} else {
				Log.LogWarning (format, args);
			}
		}

		protected void SetValue (PDictionary dict, string key, string value)
		{
			if (dict.ContainsKey (key))
				return;

			if (string.IsNullOrEmpty (value))
				LogAppManifestWarning (MSBStrings.W0106, key);
			else
				dict [key] = value;
		}

		public static void MergePartialPlistDictionary (PDictionary plist, PDictionary partial)
		{
			foreach (var property in partial) {
				var key = property.Key!;
				if (plist.ContainsKey (key)) {
					var value = plist [key];

					if (value is PDictionary && property.Value is PDictionary) {
						MergePartialPlistDictionary ((PDictionary) value, (PDictionary) property.Value);
					} else {
						plist [key] = property.Value.Clone ();
					}
				} else {
					plist [key] = property.Value.Clone ();
				}
			}
		}

		public static void MergePartialPLists (Task task, PDictionary plist, IEnumerable<ITaskItem> partialLists)
		{
			if (partialLists is null)
				return;

			foreach (var template in partialLists) {
				PDictionary partial;

				try {
					partial = PDictionary.FromFile (template.ItemSpec)!;
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

		void Validation (PDictionary plist)
		{
			if (!Validate)
				return;

			var supportedDevices = plist.GetUIDeviceFamily ();
			var macCatalystOptimizedForMac = (supportedDevices & IPhoneDeviceType.MacCatalystOptimizedForMac) == IPhoneDeviceType.MacCatalystOptimizedForMac;
			if (macCatalystOptimizedForMac && !OnWindows) {
				if (Platform != ApplePlatform.MacCatalyst) {
					LogAppManifestError (MSBStrings.E7098 /* The UIDeviceFamily value '6' is not valid for this platform. It's only valid for Mac Catalyst. */);
					return; // no need to look for more errors, they will probably not make much sense.
				}

				GetMinimumOSVersion (plist, out var minimumOSVersion);
				if (minimumOSVersion < new Version (11, 0)) {
					string miniOSVersion = "?";
					if (MacCatalystSupport.TryGetiOSVersion (Sdks.GetAppleSdk (Platform).GetSdkPath (SdkVersion), minimumOSVersion, out var iOSVersion, out var _))
						miniOSVersion = iOSVersion?.ToString () ?? "?";
					LogAppManifestError (MSBStrings.E7099 /* The UIDeviceFamily value '6' requires macOS 11.0. Please set the 'SupportedOSPlatformVersion' in the project file to at least 14.0 (the Mac Catalyst version equivalent of macOS 11.0). The current value is {0} (equivalent to macOS {1}). */, miniOSVersion, minimumOSVersion);
				}
			}

			switch (Platform) {
			case ApplePlatform.iOS:
				var supportsIPhone = (supportedDevices & IPhoneDeviceType.IPhone) != 0
									 || supportedDevices == IPhoneDeviceType.NotSet;
				var supportsIPad = (supportedDevices & IPhoneDeviceType.IPad) != 0;

				if (!IsAppExtension) {
					IPhoneOrientation orientation;

					if (supportsIPhone) {
						orientation = plist.GetUISupportedInterfaceOrientations (false);
						if (orientation == IPhoneOrientation.None) {
							LogAppManifestWarning (MSBStrings.W0019);
						} else if (!orientation.IsValidPair ()) {
							LogAppManifestWarning (MSBStrings.W0020);
						}
					}

					if (supportsIPad) {
						orientation = plist.GetUISupportedInterfaceOrientations (true);
						if (orientation == IPhoneOrientation.None) {
							LogAppManifestWarning (MSBStrings.W0021);
						} else if (!orientation.IsValidPair ()) {
							LogAppManifestWarning (MSBStrings.W0022);
						}
					}
				}
				break;
			}
		}

		bool CompileMac (PDictionary plist)
		{
			if (!IsAppExtension || (IsAppExtension && IsXPCService))
				plist.SetIfNotPresent ("MonoBundleExecutable", AssemblyName + ".exe");

			return !Log.HasLoggedErrors;
		}

		bool CompileMobile (PDictionary plist)
		{
			supportedDevices = plist.GetUIDeviceFamily ();

			// We have an issue here, this is for consideration by the platform:
			// CFLocaleCopyCurrent(), used in the mono code to get the current locale (locale.c line 421), will return the value of the application's CFBundleDevelopmentRegion Info.plist key if all of the following conditions are true:
			// 
			// * CFBundleDevelopmentRegion is present in the Info.plist
			// * The CFBundleDevelopmentRegion language is in the list of preferred languages on the iOS device, but isn't the first one
			// * There are no localized resources (i.e. no .lproj directory) in the app for the first preferred locale
			//
			// This differs from iOS 10 where the presence of the CFBundleDevelopmentRegion key had no effect. Commenting this line out, ensures that CurrentCulture is correct and behaves like the iOS 10 version.
			// plist.SetIfNotPresent (ManifestKeys.CFBundleDevelopmentRegion, "en");

			if (Platform == ApplePlatform.iOS) {
				var executable = plist.GetCFBundleExecutable ();
				if (executable.EndsWith (".app", StringComparison.Ordinal))
					LogAppManifestError (MSBStrings.E0014, executable);
			}

			if (!string.IsNullOrEmpty (ResourceRules))
				plist.SetIfNotPresent (ManifestKeys.CFBundleResourceSpecification, Path.GetFileName (ResourceRules));
			if (!plist.ContainsKey (ManifestKeys.CFBundleSupportedPlatforms))
				plist [ManifestKeys.CFBundleSupportedPlatforms] = new PArray { SdkPlatform };

			SetDeviceFamily (plist);

			if (IsWatchExtension) {
				if (Debug)
					SetAppTransportSecurity (plist);
			}

			SetRequiredArchitectures (plist);

			return !Log.HasLoggedErrors;
		}

		void SetXcodeValues (PDictionary plist, IAppleSdk currentSDK)
		{
			var sdkSettings = currentSDK.GetSdkSettings (sdkVersion, SdkIsSimulator);
			var dtSettings = currentSDK.GetAppleDTSettings ();

			SetValueIfNotNull (plist, ManifestKeys.BuildMachineOSBuild, dtSettings.BuildMachineOSBuild);
			SetValueIfNotNull (plist, "DTCompiler", sdkSettings.DTCompiler);
			SetValueIfNotNull (plist, "DTPlatformBuild", dtSettings.DTPlatformBuild);
			SetValueIfNotNull (plist, "DTSDKBuild", sdkSettings.DTSDKBuild);
			SetValueIfNotNull (plist, "DTPlatformName", SdkPlatform.ToLowerInvariant ());
			SetValueIfNotNull (plist, "DTPlatformVersion", dtSettings.DTPlatformVersion);
			SetValueIfNotNull (plist, "DTSDKName", sdkSettings.CanonicalName);
			SetValueIfNotNull (plist, "DTXcode", AppleSdkSettings.DTXcode);
			SetValueIfNotNull (plist, "DTXcodeBuild", dtSettings.DTXcodeBuild);
		}

		void SetValueIfNotNull (PDictionary dict, string key, string? value)
		{
			if (value is null)
				return;
			SetValue (dict, key, value);
		}

		void SetRequiredArchitectures (PDictionary plist)
		{
			PObject? capabilities;

			if (plist.TryGetValue (ManifestKeys.UIRequiredDeviceCapabilities, out capabilities)) {
				if (capabilities is PArray) {
					var architectureValues = new HashSet<string> (new [] { "armv6", "armv7", "arm64" });
					var array = (PArray) capabilities;

					// Remove any architecture values
					for (int i = 0; i < array.Count; i++) {
						var value = array [i] as PString;

						if (value is null || !architectureValues.Contains (value.Value))
							continue;

						array.RemoveAt (i);
					}

					// If-and-only-if the TargetArchitecture is a single architecture, set it as a required device capability
					switch (architectures) {
					case TargetArchitecture.ARM64:
						array.Add (new PString ("arm64"));
						break;
					case TargetArchitecture.ARMv7:
						array.Add (new PString ("armv7"));
						break;
					}
				} else if (capabilities is PDictionary) {
					var dict = (PDictionary) capabilities;

					switch (architectures) {
					case TargetArchitecture.ARM64:
						dict ["arm64"] = new PBoolean (true);
						dict.Remove ("armv6");
						dict.Remove ("armv7");
						break;
					case TargetArchitecture.ARMv7:
						dict ["armv7"] = new PBoolean (true);
						dict.Remove ("armv6");
						dict.Remove ("arm64");
						break;
					default:
						dict.Remove ("armv6");
						dict.Remove ("armv7");
						dict.Remove ("arm64");
						break;
					}
				}
			} else {
				var array = new PArray ();

				// If-and-only-if the TargetArchitecture is a single architecture, set it as a required device capability
				switch (architectures) {
				case TargetArchitecture.ARM64:
					array.Add (new PString ("arm64"));
					break;
				case TargetArchitecture.ARMv7:
					array.Add (new PString ("armv7"));
					break;
				}

				if (array.Count > 0)
					plist.Add (ManifestKeys.UIRequiredDeviceCapabilities, array);
			}
		}

		void SetDeviceFamily (PDictionary plist)
		{
			var uiDeviceFamily = IPhoneDeviceType.NotSet;
			switch (Platform) {
			case ApplePlatform.iOS:
				if (!IsAppExtension)
					plist.SetIfNotPresent (ManifestKeys.LSRequiresIPhoneOS, true);

				uiDeviceFamily = IPhoneDeviceType.IPhone;
				break;
			case ApplePlatform.WatchOS:
				uiDeviceFamily = IPhoneDeviceType.Watch;
				break;
			case ApplePlatform.TVOS:
				uiDeviceFamily = IPhoneDeviceType.TV;
				break;
			}

			// Don't set UIDeviceFamily if the plist already contains it
			if (uiDeviceFamily != IPhoneDeviceType.NotSet && supportedDevices == IPhoneDeviceType.NotSet)
				plist.SetUIDeviceFamily (uiDeviceFamily);
		}

		void SetAppTransportSecurity (PDictionary plist)
		{
			// Debugging over http has a couple of gotchas:
			// * We can't use https, because that requires a valid server certificate,
			//   which we can't ensure.
			//   It would also require a hostname for the mac, which it might not have either.
			// * NSAppTransportSecurity/NSExceptionDomains does not allow exceptions based
			//   on IP address (only hostname).
			// * Which means the only way to make sure watchOS allows connections from 
			//   the app on device to the mac is to disable App Transport Security altogether.
			// Good news: watchOS 3 will apparently not apply ATS when connecting
			// directly to IP addresses, which means we won't have to do this at all
			// (sometime in the future).

			PDictionary? ats;

			if (!plist.TryGetValue (ManifestKeys.NSAppTransportSecurity, out ats))
				plist.Add (ManifestKeys.NSAppTransportSecurity, ats = new PDictionary ());

			if (ats.GetBoolean (ManifestKeys.NSAllowsArbitraryLoads)) {
				Log.LogMessage (MessageImportance.Low, MSBStrings.M0017);
			} else {
				Log.LogMessage (MessageImportance.Low, MSBStrings.M0018);
				ats.SetBooleanOrRemove (ManifestKeys.NSAllowsArbitraryLoads, true);
			}
		}

		public bool ShouldCopyToBuildServer (ITaskItem item)
		{
			// We don't want to copy partial generated manifest files unless they exist and have a non-zero length
			if (PartialAppManifests is not null && PartialAppManifests.Contains (item)) {
				var finfo = new FileInfo (item.ItemSpec);
				if (!finfo.Exists || finfo.Length == 0)
					return false;
			}

			return true;
		}

		public bool ShouldCreateOutputFile (ITaskItem item) => true;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied () => Enumerable.Empty<ITaskItem> ();

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}
	}
}
