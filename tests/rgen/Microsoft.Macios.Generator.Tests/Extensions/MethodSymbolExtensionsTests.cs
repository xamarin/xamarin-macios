using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Extensions;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.Extensions;

public class MethodSymbolExtensionsTests : BaseGeneratorTestClass {

	[Theory]
	[AllSupportedPlatforms]
	public void TestTryGetDeclarationPartialMethod (ApplePlatform platform)
	{
		const string inputText = @"
public class TestClass {
	public partial void TestMethod (int firstParam, string? secondParam, string? optionalParam = null); // this method should be found
}
";
		var (compilation, syntaxTrees) =
			CreateCompilation (nameof(TestTryGetDeclarationPartialMethod), platform, inputText);
		Assert.Single (syntaxTrees);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<MethodDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		// get the symbol and use the symbol as the parameter to TryGetDeclaration
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);
		Assert.True (symbol.TryGetDeclaration (out var declarationText));
		Assert.NotNull (declarationText);
		Assert.Equal ("public partial void TestMethod (int firstParam, string? secondParam, string? optionalParam = null)", declarationText);
	}

	[Theory]
	[AllSupportedPlatforms]
	public void TestTryGetDeclarationNonPartialMethod (ApplePlatform platform)
	{
		const string inputText = @"
public class TestClass {
	// this method should NOT be found
	public void TestMethod (int firstParam, string? secondParam, string? optionalParam = null) { } 
}
";
		var (compilation, syntaxTrees) =
			CreateCompilation (nameof(TestTryGetDeclarationPartialMethod), platform, inputText);
		Assert.Single (syntaxTrees);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<MethodDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);
		Assert.False (symbol.TryGetDeclaration (out var declarationText));
		Assert.Null (declarationText);
	}
}
