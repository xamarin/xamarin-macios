using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Context;
using Microsoft.Macios.Generator.DataModel;
using Microsoft.Macios.Generator.Emitters;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.Emitters;

public class EmitterFactoryTests : BaseGeneratorTestClass {
	TabbedStringBuilder sb = new (new ());

	(CodeChanges Changes, RootBindingContext Context, SemanticModel SemanticModel, INamedTypeSymbol Symbol)
		CreateSymbol<T> (ApplePlatform platform, string inputText) where T : BaseTypeDeclarationSyntax
	{
		var (compilation, syntaxTrees) = CreateCompilation (platform, sources: inputText);
		Assert.Single (syntaxTrees);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<T> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);

		var rootContext = new RootBindingContext (compilation);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);
		var changes = CodeChanges.FromDeclaration (declaration, semanticModel);
		Assert.NotNull (changes);
		return (changes.Value, rootContext, semanticModel, symbol);
	}

	[Theory]
	[AllSupportedPlatforms]
	public void TryCreateEmitterEnum (ApplePlatform platform)
	{
		const string inputText = @"
namespace Test;

public enum TestEnum {
	One,
	Two,
	Three
}
";
		var (changes, rootContext, semanticModel, symbol) = CreateSymbol<EnumDeclarationSyntax> (platform, inputText);
		Assert.True (EmitterFactory.TryCreate (changes, rootContext, semanticModel, symbol, sb, out var emitter));
		Assert.IsType<EnumEmitter> (emitter);
	}

	[Theory]
	[AllSupportedPlatforms]
	public void TryCreateEmitterClass (ApplePlatform platform)
	{
		const string inputText = @"
namespace Test;

public class TestClass {
}
";
		var (changes, rootContext, semanticModel, symbol) = CreateSymbol<ClassDeclarationSyntax> (platform, inputText);
		Assert.True (EmitterFactory.TryCreate (changes, rootContext, semanticModel, symbol, sb, out var emitter));
		Assert.IsType<ClassEmitter> (emitter);
	}

	[Theory]
	[AllSupportedPlatforms]
	public void TryCreateEmitterInterface (ApplePlatform platform)
	{
		const string inputText = @"
namespace Test;

public interface ITestInterface {
}
";
		var (changes, rootContext, semanticModel, symbol) = CreateSymbol<InterfaceDeclarationSyntax> (platform, inputText);
		Assert.True (EmitterFactory.TryCreate (changes, rootContext, semanticModel, symbol, sb, out var emitter));
		Assert.IsType<InterfaceEmitter> (emitter);
	}
}
