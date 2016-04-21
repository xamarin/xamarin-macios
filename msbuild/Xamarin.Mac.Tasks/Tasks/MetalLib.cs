using System.IO;

using Xamarin.MacDev.Tasks;

namespace Xamarin.Mac.Tasks
{
	public class MetalLib : MetalLibTaskBase
	{
		protected override string DevicePlatformBinDir {
			get { return Path.Combine (SdkDevPath, "Platforms", "MacOSX.platform", "usr", "bin"); }
		}
	}
}
