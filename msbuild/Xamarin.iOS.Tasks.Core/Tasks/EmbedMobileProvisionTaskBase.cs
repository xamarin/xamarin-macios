using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;
using Xamarin.MacDev;

namespace Xamarin.iOS.Tasks
{
	public abstract class EmbedMobileProvisionTaskBase : Task
	{
		#region Inputs

		public string SessionId { get; set; }

		[Required]
		public string AppBundleDir { get; set; }

		[Required]
		public string ProvisioningProfile { get; set; }

		[Required]
		public string SdkPlatform { get; set; }

		#endregion

		public override bool Execute ()
		{
			MobileProvisionPlatform platform;

			switch (SdkPlatform) {
			case "AppleTVSimulator":
			case "AppleTVOS":
				platform = MobileProvisionPlatform.tvOS;
				break;
			case "iPhoneSimulator":
			case "WatchSimulator":
			case "iPhoneOS":
			case "WatchOS":
				platform = MobileProvisionPlatform.iOS;
				break;
			default:
				Log.LogError ("Unknown SDK platform: {0}", SdkPlatform);
				return false;
			}

			var profile = MobileProvisionIndex.GetMobileProvision (platform, ProvisioningProfile);

			if (profile == null) {
				Log.LogError ("Could not locate the provisioning profile with a Name or UUID of {0}.", ProvisioningProfile);
				return false;
			}

			var embedded = Path.Combine (AppBundleDir, "embedded.mobileprovision");

			if (File.Exists (embedded)) {
				var embeddedProfile = MobileProvision.LoadFromFile (embedded);

				if (embeddedProfile.Uuid == profile.Uuid)
					return true;
			}

			Directory.CreateDirectory (AppBundleDir);
			profile.Save (embedded);

			return true;
		}
	}
}
