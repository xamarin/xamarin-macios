using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests;

// Unit test that ensures that all the generator attributes are correctly added in the compilation initialization
public class BindingSourceGeneratorGeneratorTests : BaseGeneratorTestClass {

	const string SampleBindingType = @"
namespace TestNamespace;

[BindingType (Name = ""AVAudioPCMBuffer"")]
interface AVAudioPcmBuffer : AVAudioBuffer {
}
";

	[Theory]
	[PlatformInlineData (ApplePlatform.iOS)]
	[PlatformInlineData (ApplePlatform.TVOS)]
	[PlatformInlineData (ApplePlatform.MacOSX)]
	[PlatformInlineData (ApplePlatform.MacCatalyst)]
	public void AttributesAreNotPresent (ApplePlatform platform)
	{
		// We need to create a compilation with the required source code.
		var compilation = CreateCompilation (nameof (AttributesAreNotPresent),
			platform, SampleBindingType);

		// Run generators and retrieve all results.
		var runResult = _driver.RunGenerators (compilation).GetRunResult ();

		// ensure that we do have all the needed attributes present
		var expectedGeneratedAttributes = new [] {
			"BindingTypeAttribute.g.cs",
		};

		foreach (string generatedAttribute in expectedGeneratedAttributes) {
			var generatedFile = runResult.GeneratedTrees.SingleOrDefault (t => t.FilePath.EndsWith (generatedAttribute));
			Assert.Null (generatedFile);
		}
	}
}
