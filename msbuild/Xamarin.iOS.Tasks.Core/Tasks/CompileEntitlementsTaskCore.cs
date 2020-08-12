using System.IO;
using System.Collections.Generic;

using Xamarin.MacDev.Tasks;

namespace Xamarin.iOS.Tasks
{
	public abstract class CompileEntitlementsTaskCore : CompileEntitlementsTaskBase
	{
		static readonly HashSet<string> allowedProvisioningKeys = new HashSet<string> {
			"application-identifier",
			"aps-environment",
			"beta-reports-active",
			"com.apple.developer.default-data-protection",

			"com.apple.developer.icloud-container-environment",
			"com.apple.developer.icloud-container-identifiers",
			"com.apple.developer.pass-type-identifiers",
			"com.apple.developer.team-identifier",
			"com.apple.developer.ubiquity-container-identifiers",
			"get-task-allow"
		};

		protected override string ApplicationIdentifierKey {
			get { return "application-identifier"; }
		}

		protected override HashSet<string> AllowedProvisioningKeys {
			get { return allowedProvisioningKeys; }
		}

		protected override string DefaultEntitlementsPath {
			get {
				return Path.Combine (IPhoneSdks.GetSdk (TargetFrameworkMoniker).GetSdkPath (SdkVersion, false), "Entitlements.plist");
			}
		}

		protected override string EntitlementBundlePath { get { return AppBundleDir; } }
	}
}

