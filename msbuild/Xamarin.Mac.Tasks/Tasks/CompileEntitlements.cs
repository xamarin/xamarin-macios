using System;
using System.IO;
using System.Collections.Generic;

using Xamarin.MacDev;
using Xamarin.MacDev.Tasks;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace Xamarin.Mac.Tasks
{
	public class CompileEntitlements : CompileEntitlementsTaskBase
	{
		public bool Debug { get; set; }

		[Required]
		public string SdkDevPath { get; set; }

		static readonly HashSet<string> allowedProvisioningKeys = new HashSet<string> {
			"com.apple.application-identifier",
			"com.apple.developer.aps-environment",
			"com.apple.developer.default-data-protection",
			//"com.apple.developer.icloud-container-development-container-identifiers",
			//"com.apple.developer.icloud-container-identifiers",
			//"com.apple.developer.icloud-container-environment",
			//"com.apple.developer.icloud-services",
			"com.apple.developer.pass-type-identifiers",
			"com.apple.developer.team-identifier",
			//"com.apple.developer.ubiquity-container-identifiers",
			"get-task-allow",
		};

		protected override string ApplicationIdentifierKey {
			get { return "com.apple.application-identifier"; }
		}

		protected override HashSet<string> AllowedProvisioningKeys {
			get { return allowedProvisioningKeys; }
		}

		protected override string DefaultEntitlementsPath {
			get { return Path.Combine (SdkDevPath, "Platforms/MacOSX.platform/Entitlements.plist"); }
		}

		protected override string EntitlementBundlePath { get { return Path.Combine (AppBundleDir, "Contents", "Resources"); } }

		protected override PDictionary GetCompiledEntitlements (MobileProvision profile, PDictionary template)
		{
			var entitlements = base.GetCompiledEntitlements (profile, template);
			PBoolean sandbox;

			if (Debug && entitlements.TryGetValue ("com.apple.security.app-sandbox", out sandbox) && sandbox.Value)
				entitlements["com.apple.security.network.client"] = new PBoolean (true);

			return entitlements;
		}
	}
}