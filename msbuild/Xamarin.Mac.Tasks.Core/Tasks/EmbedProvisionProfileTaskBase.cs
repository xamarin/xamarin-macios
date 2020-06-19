using System;
using System.IO;
using System.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;
using Xamarin.MacDev;
using Xamarin.Localization.MSBuild;

namespace Xamarin.Mac.Tasks
{
	public abstract class EmbedProvisionProfileTaskBase : XamarinTask
	{
		#region Inputs

		[Required]
		public string AppBundleDir { get; set; }

		[Required]
		public string ProvisioningProfile { get; set; }

		#endregion

		public override bool Execute ()
		{
			var profile = MobileProvisionIndex.GetMobileProvision (MobileProvisionPlatform.MacOS, ProvisioningProfile);

			if (profile == null) {
				Log.LogError (MSBStrings.E0049, ProvisioningProfile);
				return false;
			}

			var embedded = Path.Combine (AppBundleDir, "Contents", "embedded.provisionprofile");

			Directory.CreateDirectory (AppBundleDir);
			profile.Save (embedded);

			return true;
		}
	}
}
