// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.DataModel;
using Microsoft.Macios.Generator.Emitters;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.Emitters;

public class EmitterFactoryTests : BaseGeneratorTestClass {
	Binding CreateSymbol<T> (ApplePlatform platform, string inputText) where T : BaseTypeDeclarationSyntax
	{
		var (compilation, syntaxTrees) = CreateCompilation (platform, sources: inputText);
		Assert.Single (syntaxTrees);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<T> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);

		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);
		var changes = Binding.FromDeclaration (declaration, semanticModel);
		Assert.NotNull (changes);
		return changes.Value;
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
		var changes = CreateSymbol<EnumDeclarationSyntax> (platform, inputText);
		Assert.True (EmitterFactory.TryCreate (changes, out var emitter));
		Assert.IsType<EnumEmitter> (emitter);
		Assert.True (EmitterFactory.TryCreate (changes, out var secondEmitter));
		Assert.Same (emitter, secondEmitter);
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
		var changes = CreateSymbol<ClassDeclarationSyntax> (platform, inputText);
		Assert.True (EmitterFactory.TryCreate (changes, out var emitter));
		Assert.IsType<ClassEmitter> (emitter);
		Assert.True (EmitterFactory.TryCreate (changes, out var secondEmitter));
		Assert.Same (emitter, secondEmitter);
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
		var changes = CreateSymbol<InterfaceDeclarationSyntax> (platform, inputText);
		Assert.True (EmitterFactory.TryCreate (changes, out var emitter));
		Assert.IsType<InterfaceEmitter> (emitter);
		Assert.True (EmitterFactory.TryCreate (changes, out var secondEmitter));
		Assert.Same (emitter, secondEmitter);
	}
}
