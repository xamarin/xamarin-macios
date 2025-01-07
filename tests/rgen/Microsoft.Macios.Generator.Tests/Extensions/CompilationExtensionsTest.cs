using Microsoft.Macios.Generator.Extensions;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.Extensions;

public class CompilationExtensionsTest : BaseGeneratorTestClass {

	[Theory]
	[PlatformInlineData (ApplePlatform.iOS, PlatformName.iOS)]
	[PlatformInlineData (ApplePlatform.TVOS, PlatformName.TvOS)]
	[PlatformInlineData (ApplePlatform.MacOSX, PlatformName.MacOSX)]
	[PlatformInlineData (ApplePlatform.MacCatalyst, PlatformName.MacCatalyst)]
	public void GetCurrentPlatformTests (ApplePlatform platform, PlatformName expectedPlatform)
	{
		// get the current compilation for the platform and assert we return the correct one from
		// the compilation
		var (compilation, _) = CreateCompilation (platform);
		Assert.Equal (expectedPlatform, compilation.GetCurrentPlatform ());
	}
}
