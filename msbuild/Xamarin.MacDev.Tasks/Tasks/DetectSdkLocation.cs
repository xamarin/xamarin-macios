using System;
using System.IO;
using System.Linq;

using Microsoft.Build.Framework;

using Xamarin.Localization.MSBuild;
using Xamarin.Messaging.Build.Client;
using Xamarin.Utils;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public class DetectSdkLocations : XamarinTask, ICancelableTask {
		const string SdkVersionDefaultValue = "default";
		#region Inputs

		[Required]
		public bool SdkIsSimulator {
			get; set;
		}

		#endregion Inputs

		#region Outputs

		[Output]
		public string SdkRoot {
			get; set;
		} = "";

		// this is input too (the variable 'XcodeLocation')
		[Output]
		public new string SdkDevPath {
			get => base.SdkDevPath;
			set => base.SdkDevPath = value;
		}

		[Output]
		public string SdkPlatform {
			get; set;
		} = "";

		// This is also an input
		[Output]
		public string SdkVersion {
			get; set;
		} = "";

		// This is also an input
		[Output]
		public string XamarinSdkRoot {
			get; set;
		} = "";

		[Output]
		public string XcodeVersion {
			get; set;
		} = "";

		#endregion Outputs

		IAppleSdkVersion GetDefaultSdkVersion ()
		{
			switch (Platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
			case ApplePlatform.MacCatalyst:
				return AppleSdkVersion.UseDefault;
			case ApplePlatform.MacOSX:
				var v = CurrentSdk.GetInstalledSdkVersions (false);
				return v.Count > 0 ? v [v.Count - 1] : AppleSdkVersion.UseDefault;
			default:
				throw new InvalidOperationException (string.Format (MSBStrings.InvalidPlatform, Platform));
			}
		}

		protected void EnsureSdkPath ()
		{
			SdkPlatform = GetSdkPlatform (SdkIsSimulator);

			var currentSdk = CurrentSdk;
			IAppleSdkVersion requestedSdkVersion;

			if (string.IsNullOrEmpty (SdkVersion)) {
				requestedSdkVersion = GetDefaultSdkVersion ();
			} else if (!currentSdk.TryParseSdkVersion (SdkVersion, out requestedSdkVersion)) {
				Log.LogError (MSBStrings.E0025 /* Could not parse the SDK version '{0}' */, SdkVersion);
				return;
			}

			var sdkVersion = requestedSdkVersion.ResolveIfDefault (currentSdk, SdkIsSimulator);
			if (!currentSdk.SdkIsInstalled (sdkVersion, SdkIsSimulator)) {
				sdkVersion = currentSdk.GetClosestInstalledSdk (sdkVersion, SdkIsSimulator);

				if (sdkVersion.IsUseDefault || !currentSdk.SdkIsInstalled (sdkVersion, SdkIsSimulator)) {
					if (requestedSdkVersion.IsUseDefault) {
						Log.LogError (MSBStrings.E0171 /* The {0} SDK is not installed. */, PlatformName);
					} else {
						Log.LogError (MSBStrings.E0172 /* The {0} SDK version '{0}' is not installed, and no newer version was found. */, PlatformName, requestedSdkVersion.ToString ());
					}
					return;
				}
				Log.LogWarning (MSBStrings.E0173 /* The {0} SDK version '{1}' is not installed. Using newer version '{2}' instead'. */, PlatformName, requestedSdkVersion, sdkVersion);
			}
			SdkVersion = sdkVersion.ToString () ?? "";

			SdkRoot = currentSdk.GetSdkPath (SdkVersion, SdkIsSimulator);
			if (string.IsNullOrEmpty (SdkRoot))
				Log.LogError (MSBStrings.E0084 /* Could not locate the {0} '{1}' SDK at path '{2}' */, PlatformName, SdkVersion, SdkRoot);
		}

		void EnsureXamarinSdkRoot ()
		{
			if (string.IsNullOrEmpty (XamarinSdkRoot))
				Log.LogError (MSBStrings.E0046 /* Could not find '{0}' */, Product);
			else if (!Directory.Exists (XamarinSdkRoot))
				Log.LogError (MSBStrings.E0170 /* Could not find {0} in {1}. */, Product, XamarinSdkRoot);
		}

		public override bool Execute ()
		{
			try {
				LoggingService.SetCustomLogger (this);
				return ExecuteImpl () && !Log.HasLoggedErrors;
			} finally {
				LoggingService.SetCustomLogger (null);
			}
		}

		bool ExecuteImpl ()
		{
			if (ShouldExecuteRemotely ()) {
				// The new targets do not support the "default" value for the MtouchSdkVersion
				// So we fix it to not break existing projects that has this value defined in the .csproj
				if (!string.IsNullOrEmpty (SdkVersion) && SdkVersionDefaultValue.Equals (SdkVersion, StringComparison.OrdinalIgnoreCase))
					SdkVersion = string.Empty;

				return ExecuteRemotely ();
			}

			var isNet11OrNewer = TargetFramework.Version.Major >= 11;
			var appleSdkSettings = GetXcodeLocator (initialDiscovery: true, (locator) => {
				locator.SupportEnvironmentVariableLookup = !isNet11OrNewer;
				locator.SupportSettingsFileLookup = !isNet11OrNewer;
			});
			SetXcodeLocator (appleSdkSettings);
			SdkDevPath = appleSdkSettings.DeveloperRoot;
			XcodeVersion = appleSdkSettings.XcodeVersion.ToString ();

			if (appleSdkSettings.SystemHasEnvironmentVariable) {
				if (isNet11OrNewer) {
					Log.LogWarning (MSBStrings.W7172 /* The environment variable '{0}' is deprecated, and will be ignored. Use the 'DEVELOPER_DIR' environment variable or the 'XcodeLocation' MSBuild property to use an Xcode other than the system's current default Xcode. */, XcodeLocator.EnvironmentVariableName);
				} else {
					Log.LogWarning (MSBStrings.W7171 /* The environment variable '{0}' is deprecated, and will be ignored in .NET 11+. Use the 'DEVELOPER_DIR' environment variable or the 'XcodeLocation' MSBuild property to use an Xcode other than the system's current default Xcode. */, XcodeLocator.EnvironmentVariableName);
				}
			}
			foreach (var file in appleSdkSettings.SystemExistingSettingsFiles) {
				if (isNet11OrNewer) {
					Log.LogWarning (MSBStrings.W7174 /* The settings file '{0}' is deprecated, and will be ignored. Use the 'DEVELOPER_DIR' environment variable or the 'XcodeLocation' MSBuild property to use an Xcode other than the system's current default Xcode. */, file);
				} else {
					Log.LogWarning (MSBStrings.W7173 /* The settings file '{0}' is deprecated, and will be ignored in .NET 11+. Use the 'DEVELOPER_DIR' environment variable or the 'XcodeLocation' MSBuild property to use an Xcode other than the system's current default Xcode. */, file);
				}
			}

			if (Log.HasLoggedErrors)
				return false;

			Log.LogMessage (MessageImportance.Low, "DeveloperRoot: {0}", CurrentSdk.DeveloperRoot);
			Log.LogMessage (MessageImportance.Low, "GetPlatformPath: {0}", CurrentSdk.GetPlatformPath (SdkIsSimulator));

			EnsureSdkPath ();
			EnsureXamarinSdkRoot ();

			return !Log.HasLoggedErrors;
		}

		protected string? DirExists (string checkingFor, params string [] paths)
		{
			try {
				if (paths.Any (p => string.IsNullOrEmpty (p)))
					return null;

				var path = Path.GetFullPath (Path.Combine (paths));
				Log.LogMessage (MessageImportance.Low, MSBStrings.M0047 /* Searching for '{0}' in '{1}' */, checkingFor, path);
				return Directory.Exists (path) ? path : null;
			} catch {
				return null;
			}
		}

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}
	}
}
