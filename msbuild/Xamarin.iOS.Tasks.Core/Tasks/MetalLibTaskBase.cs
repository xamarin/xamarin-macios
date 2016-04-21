using System.IO;

namespace Xamarin.iOS.Tasks
{
	public abstract class MetalLibTaskBase : Xamarin.MacDev.Tasks.MetalLibTaskBase
	{
		protected override string DevicePlatformBinDir {
			get { return Path.Combine (SdkDevPath, "Platforms", "iPhoneOS.platform", "usr", "bin"); }
		}
	}
}
