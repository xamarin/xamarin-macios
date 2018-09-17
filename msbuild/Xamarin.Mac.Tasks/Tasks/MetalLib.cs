using System.IO;
using Xamarin.MacDev;
using Xamarin.MacDev.Tasks;

namespace Xamarin.Mac.Tasks
{
	public class MetalLib : MetalLibTaskBase
	{
		protected override string DevicePlatformBinDir {
			get {
				return AppleSdkSettings.XcodeVersion.Major >= 10
					? Path.Combine (SdkDevPath, "Toolchains", "XcodeDefault.xctoolchain", "usr", "bin")
					: Path.Combine (SdkDevPath, "Platforms", "MacOSX.platform", "usr", "bin");
			}
		}
	}
}
