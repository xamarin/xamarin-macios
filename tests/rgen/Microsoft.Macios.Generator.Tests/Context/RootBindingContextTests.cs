using Microsoft.Macios.Generator.Context;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.Context;

public class RootBindingContextTests : BaseGeneratorTestClass {
	[Theory]
	[PlatformInlineData (ApplePlatform.iOS, "+CoreImage", "CoreImage", "CoreImage")]
	[PlatformInlineData (ApplePlatform.iOS, "+CoreServices", "CoreServices", "MobileCoreServices")]
	[PlatformInlineData (ApplePlatform.iOS, "+PDFKit", "PDFKit", "PdfKit")]
	[PlatformInlineData (ApplePlatform.MacOSX, "+CoreImage", "CoreImage", "Quartz")]
	[PlatformInlineData (ApplePlatform.MacOSX, "+CoreServices", "CoreServices", "CoreServices")]
	[PlatformInlineData (ApplePlatform.MacOSX, "+PDFKit", "PDFKit", "PdfKit")]
	[PlatformInlineData (ApplePlatform.TVOS, "+CoreImage", "CoreImage", "CoreImage")]
	[PlatformInlineData (ApplePlatform.TVOS, "+CoreServices", "CoreServices", "MobileCoreServices")]
	[PlatformInlineData (ApplePlatform.TVOS, "+PDFKit", "PDFKit", "PdfKit")]
	public void TryComputeLibraryNamePlusLibsTests (ApplePlatform platform, string attributeLibName, string ns,
		string expectedLibraryName)
	{
		const string inputText = @"
namespace MyNamespace {
	public class MyClass {
	}
}
";
		var (compilation, _) = CreateCompilation (nameof(TryComputeLibraryNamePlusLibsTests), platform, inputText);
		var rootContext = new RootBindingContext (compilation);
		Assert.True (rootContext.TryComputeLibraryName (attributeLibName, ns, out var libName, out var libPath));
		Assert.Equal (expectedLibraryName, libName);
	}

	[Theory]
	[AllSupportedPlatforms ("MyOwnLib", false)]
	[AllSupportedPlatforms ("Test", false)]
	[AllSupportedPlatforms ("CoreData", true)]
	[AllSupportedPlatforms ("GameKit", true)]
	[AllSupportedPlatforms ("NetworkExtension", true)]
	[AllSupportedPlatforms ("MobileCoreServices", true)]
	public void IsSystemLibraryTests (ApplePlatform platform, string lib, bool expectedResult)
	{
		const string inputText = @"
namespace MyNamespace {
	public class MyClass {
	}
}
";
		var (compilation, _) = CreateCompilation (nameof(TryComputeLibraryNamePlusLibsTests), platform, inputText);
		var rootContext = new RootBindingContext (compilation);
		Assert.Equal (rootContext.IsSystemLibrary (lib), expectedResult);
	}
}
