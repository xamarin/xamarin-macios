using Xamarin.Utils;

namespace Microsoft.Macios.Bindings.Analyzer.Extensions;

public static class PlatformNameExtensions {

	public static ApplePlatform ToApplePlatform (this PlatformName platformName)
		=> platformName switch {
			PlatformName.iOS => ApplePlatform.iOS,
			PlatformName.MacCatalyst => ApplePlatform.MacCatalyst,
			PlatformName.MacOSX => ApplePlatform.MacOSX,
			PlatformName.TvOS => ApplePlatform.TVOS,
			_ => ApplePlatform.None,
		};

}
