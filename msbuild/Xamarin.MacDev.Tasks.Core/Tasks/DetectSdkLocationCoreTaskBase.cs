using System;
using System.IO;
using System.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;
using Xamarin.MacDev;
using Xamarin.Localization.MSBuild;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	public abstract class DetectSdkLocationsCoreTaskBase : XamarinTask {
		#region Inputs

		#endregion Inputs

		#region Outputs

		[Output]
		public string SdkRoot {
			get; set;
		}

		[Output]
		public string SdkBinPath {
			get; set;
		}

		[Output]
		public string SdkDevPath {
			get; set;
		}

		[Output]
		public string SdkUsrPath {
			get; set;
		}

		[Output]
		public bool SdkIsSimulator {
			get; set;
		}

		[Output]
		public string SdkPlatform {
			get; set;
		}

		// This is also an input
		[Output]
		public string SdkVersion {
			get; set;
		}

		[Output]
		public bool IsXcode8 {
			get; set;
		}

		// This is also an input
		[Output]
		public string XamarinSdkRoot {
			get; set;
		}

		#endregion Outputs

		protected abstract IAppleSdk CurrentSdk { get; }
		protected abstract string GetDefaultXamarinSdkRoot ();
		protected abstract string GetSdkPlatform ();
		protected abstract IAppleSdkVersion GetDefaultSdkVersion ();

		protected void EnsureSdkPath ()
		{
			SdkPlatform = GetSdkPlatform ();

			var currentSdk = CurrentSdk;
			IAppleSdkVersion requestedSdkVersion;

			if (string.IsNullOrEmpty (SdkVersion)) {
				requestedSdkVersion = GetDefaultSdkVersion ();
			} else if (!currentSdk.TryParseSdkVersion (SdkVersion, out requestedSdkVersion)) {
			//} else if (!IPhoneSdkVersion.TryParse (SdkVersion, out requestedSdkVersion)) {
				Log.LogError (MSBStrings.E0025, SdkVersion);
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
			SdkVersion = sdkVersion.ToString ();

			SdkRoot = currentSdk.GetSdkPath (SdkVersion, SdkIsSimulator);
			if (string.IsNullOrEmpty (SdkRoot))
				Log.LogError (MSBStrings.E0084 /* Could not locate the {0} '{1}' SDK at path '{2}' */, PlatformName, SdkVersion, SdkRoot);

			SdkUsrPath = DirExists ("SDK usr directory", Path.Combine (currentSdk.DeveloperRoot, "usr"));
			if (string.IsNullOrEmpty (SdkUsrPath))
				Log.LogError (MSBStrings.E0085 /* Could not locate the {0} '{1}' SDK usr path at '{2}' */, PlatformName, SdkVersion, SdkRoot);

			SdkBinPath = DirExists ("SDK bin directory", Path.Combine (SdkUsrPath, "bin"));
			if (string.IsNullOrEmpty (SdkBinPath))
				Log.LogError (MSBStrings.E0032 /* Could not locate SDK bin directory */);
		}

		void EnsureXamarinSdkRoot ()
		{
			if (string.IsNullOrEmpty (XamarinSdkRoot))
				XamarinSdkRoot = GetDefaultXamarinSdkRoot ();

			if (string.IsNullOrEmpty (XamarinSdkRoot))
				Log.LogError (MSBStrings.E0046, Product);
			else if (!Directory.Exists (XamarinSdkRoot))
				Log.LogError (MSBStrings.E0170, Product, XamarinSdkRoot);
		}

		public override bool Execute ()
		{
			if (EnsureAppleSdkRoot ())
				EnsureSdkPath ();
			EnsureXamarinSdkRoot ();
			return !Log.HasLoggedErrors;
		}

		protected bool EnsureAppleSdkRoot ()
		{
			var currentSdk = CurrentSdk;
			if (!currentSdk.IsInstalled) {
				string ideSdkPath;
				if (string.IsNullOrEmpty (SessionId))
					// SessionId is only and always defined on windows.
					// We can't check 'Environment.OSVersion.Platform' since the base tasks are always executed on the Mac.
					ideSdkPath = "(Projects > SDK Locations > Apple > Apple SDK)";
				else
					ideSdkPath = "(Tools > Options > Xamarin > iOS Settings > Apple SDK)";
				Log.LogError (MSBStrings.E0044, AppleSdkSettings.InvalidDeveloperRoot, ideSdkPath);
				return false;
			}
			Log.LogMessage (MessageImportance.Low, "DeveloperRoot: {0}", currentSdk.DeveloperRoot);
			Log.LogMessage (MessageImportance.Low, "GetPlatformPath: {0}", currentSdk.GetPlatformPath (SdkIsSimulator));

			SdkDevPath = currentSdk.DeveloperRoot;
			if (string.IsNullOrEmpty (SdkDevPath)) {
				Log.LogError (MSBStrings.E0086);
				return false;
			}
			return true;
		}

		protected string DirExists (string checkingFor, params string [] paths)
		{
			try {
				if (paths.Any (p => string.IsNullOrEmpty (p)))
					return null;

				var path = Path.GetFullPath (Path.Combine (paths));
				Log.LogMessage (MessageImportance.Low, MSBStrings.M0047, checkingFor, path);
				return Directory.Exists (path) ? path : null;
			} catch {
				return null;
			}
		}
	}
}
