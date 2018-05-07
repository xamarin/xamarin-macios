using System.IO;

using Xamarin.MacDev;

namespace Xamarin.iOS.Tasks
{
	public abstract class MetalTaskBase : Xamarin.MacDev.Tasks.MetalTaskBase
	{
		protected override string OperatingSystem {
			get { return "ios"; }
		}

		protected override string MinimumDeploymentTargetKey {
			get { return ManifestKeys.MinimumOSVersion; }
		}

		protected override string DevicePlatformBinDir {
			get { return Path.Combine (SdkDevPath, "Platforms", "iPhoneOS.platform", "usr", "bin"); }
		}
	}
}
