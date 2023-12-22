using System;
using System.IO;

using Microsoft.Build.Framework;

using Xamarin.Localization.MSBuild;
using Xamarin.Messaging.Build.Client;
using Xamarin.Utils;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.MacDev.Tasks {
	public class EmbedProvisionProfile : XamarinTask {
		#region Inputs

		[Required]
		public string AppBundleDir { get; set; }

		[Required]
		public string ProvisioningProfile { get; set; }

		[Required]
		public string EmbeddedProvisionProfilePath { get; set; }

		#endregion

		MobileProvisionPlatform GetMobileProvisionPlatform ()
		{
			switch (Platform) {
			case ApplePlatform.iOS:
				return MobileProvisionPlatform.iOS;
			case ApplePlatform.TVOS:
				return MobileProvisionPlatform.tvOS;
			case ApplePlatform.WatchOS:
				return MobileProvisionPlatform.iOS;
			case ApplePlatform.MacOSX:
			case ApplePlatform.MacCatalyst:
				return MobileProvisionPlatform.MacOS;
			default:
				throw new InvalidOperationException (string.Format (MSBStrings.InvalidPlatform, Platform));
			}
		}

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			var profile = MobileProvisionIndex.GetMobileProvision (GetMobileProvisionPlatform (), ProvisioningProfile);

			if (profile is null) {
				Log.LogError (MSBStrings.E0049, ProvisioningProfile);
				return false;
			}

			var embedded = EmbeddedProvisionProfilePath;

			if (File.Exists (embedded)) {
				var embeddedProfile = MobileProvision.LoadFromFile (embedded);

				if (embeddedProfile.Uuid == profile.Uuid)
					return true;
			}

			Directory.CreateDirectory (Path.GetDirectoryName (embedded));
			profile.Save (embedded);

			return true;
		}
	}
}
