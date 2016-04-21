using System.IO;

using Xamarin.MacDev.Tasks;

namespace Xamarin.Mac.Tasks
{
	public class Metal : MetalTaskBase
	{
		protected override string OperatingSystem {
			get { return "osx"; }
		}

		protected override string DevicePlatformBinDir {
			get { return Path.Combine (SdkDevPath, "Platforms", "MacOSX.platform", "usr", "bin"); }
		}
	}
}
