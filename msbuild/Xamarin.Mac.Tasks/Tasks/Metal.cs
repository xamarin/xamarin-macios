using System.IO;

using Xamarin.MacDev;
using Xamarin.MacDev.Tasks;

namespace Xamarin.Mac.Tasks
{
	public class Metal : MetalTaskBase
	{
		protected override string OperatingSystem {
			get { return "macosx"; }
		}

		protected override string MinimumDeploymentTargetKey {
			get { return ManifestKeys.LSMinimumSystemVersion; }
		}

		protected override string DevicePlatformBinDir {
			get { return Path.Combine (SdkDevPath, "Platforms", "MacOSX.platform", "usr", "bin"); }
		}
	}
}
