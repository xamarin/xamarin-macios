using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Extensions;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.Extensions;

public class PropertyDeclarationSyntaxExtensionsTests : BaseGeneratorTestClass {

	[Theory]
	[AllSupportedPlatforms]
	public void TestTryGetDeclarationPartialMethod (ApplePlatform platform)
	{
		const string inputText = @"
public class TestClass {
	public virtual partial uint FrameLength { get; set; }
}
";
		var (compilation, syntaxTrees) =
			CreateCompilation (nameof(TestTryGetDeclarationPartialMethod), platform, inputText);
		Assert.Single (syntaxTrees);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<PropertyDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		Assert.True (declaration.TryGetDeclaration (out var declarationText));
		Assert.NotNull (declarationText);
		Assert.Equal ("public virtual partial uint FrameLength", declarationText);
	}

	[Theory]
	[AllSupportedPlatforms]
	public void TestTryGetDeclarationNonPartialMethod (ApplePlatform platform)
	{
		const string inputText = @"
public class TestClass {
	// should return false, it is not partial
	public virtual uint FrameLength { get; set; }
}
";
		var (compilation, syntaxTrees) =
			CreateCompilation (nameof(TestTryGetDeclarationPartialMethod), platform, inputText);
		Assert.Single (syntaxTrees);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<PropertyDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		Assert.False (declaration.TryGetDeclaration (out var declarationText));
		Assert.Null (declarationText);
	}

}
