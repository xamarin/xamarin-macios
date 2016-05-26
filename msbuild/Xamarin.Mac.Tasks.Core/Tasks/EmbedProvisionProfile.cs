using System;
using System.IO;
using System.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;
using Xamarin.MacDev;

namespace Xamarin.Mac.Tasks
{
	public class EmbedProvisionProfile : Task
	{
		#region Inputs

		[Required]
		public string AppBundleDir { get; set; }

		[Required]
		public string ProvisioningProfile { get; set; }

		#endregion

		static MobileProvision GetMobileProvision (MobileProvisionPlatform platform, string uuid)
		{
			var extension = MobileProvision.GetFileExtension (platform);
			var path = Path.Combine (MobileProvision.ProfileDirectory, uuid + extension);

			if (File.Exists (path))
				return MobileProvision.LoadFromFile (path);

			return MobileProvision.GetAllInstalledProvisions (platform, true).FirstOrDefault (x => x.Uuid == uuid);
		}

		public override bool Execute ()
		{
			Log.LogTaskName ("EmbedProvisionProfile");
			Log.LogTaskProperty ("AppBundleDir", AppBundleDir);
			Log.LogTaskProperty ("ProvisioningProfile", ProvisioningProfile);

			var profile = GetMobileProvision (MobileProvisionPlatform.MacOS, ProvisioningProfile);

			if (profile == null) {
				Log.LogError ("Could not locate the provisioning profile with a UUID of {0}.", ProvisioningProfile);
				return false;
			}

			var embedded = Path.Combine (AppBundleDir, "Contents", "embedded.provisionprofile");

			Directory.CreateDirectory (AppBundleDir);
			profile.Save (embedded);

			return true;
		}
	}
}
