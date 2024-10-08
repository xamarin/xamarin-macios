using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Microsoft.Macios.Generator.Tests;

// Unit test that ensures that all the generator attributes are correctly added in the compilation initialization
public class BindingSourceGeneratorTests : BaseTestClass {

	const string SampleBindingType = @"
namespace TestNamespace;

[BidingType (Name = ""AVAudioPCMBuffer"")]
interface AVAudioPcmBuffer : AVAudioBuffer {
}
";

	[Fact]
	public void AttributesArePresent ()
	{
		// We need to create a compilation with the required source code.
		var compilation = CSharpCompilation.Create(nameof(AttributesArePresent ),
			new[] { CSharpSyntaxTree.ParseText(SampleBindingType) },
			_references);

		// Run generators and retrieve all results.
		var runResult = _driver.RunGenerators(compilation).GetRunResult();

		// ensure that we do have all the needed attributes present
		var expectedGeneratedAttributes = new [] {
			"Foundation.BindingTypeAttribute.g.cs",
		};

		foreach (string generatedAttribute in expectedGeneratedAttributes) {
			var generatedFile = runResult.GeneratedTrees.SingleOrDefault(t => t.FilePath.EndsWith(generatedAttribute));
			Assert.NotNull (generatedFile);
		}
	}
}
