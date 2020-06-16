using System.IO;

using Xamarin.MacDev;

namespace Xamarin.iOS.Tasks
{
	public abstract class MetalTaskBase : Xamarin.MacDev.Tasks.MetalTaskBase
	{
		protected override string DevicePlatformBinDir {
			get {
				return AppleSdkSettings.XcodeVersion.Major >= 11
					? Path.Combine (SdkDevPath, "Toolchains", "XcodeDefault.xctoolchain", "usr", "bin")
					: Path.Combine (SdkDevPath, "Platforms", "iPhoneOS.platform", "usr", "bin");
			}
		}
	}
}
