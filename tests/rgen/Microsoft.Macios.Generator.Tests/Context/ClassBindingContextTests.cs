using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Context;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.Context;

public class ClassBindingContextTests : BaseGeneratorTestClass {
	const string symbolNameRegisterName = @"
using System;
using Foundation;
using ObjCBindings;
namespace TestNamespace;

[BindingType]
public partial class ExampleClass {
}
";

	const string customNameRegisterName = @"
using System;
using Foundation;
using ObjCBindings;
namespace TestNamespace;

[BindingType (Name = ""AVAudioPCMBuffer"")]
public partial class AVAudioPcmBuffer {
}
";

	[Theory]
	[AllSupportedPlatforms (symbolNameRegisterName, "ExampleClass")]
	[AllSupportedPlatforms (customNameRegisterName, "AVAudioPCMBuffer")]
	public void RegisterNameTest (ApplePlatform platform, string inputString, string expectedRegisterName)
	{
		var (compilation, syntaxTrees) = CreateCompilation (nameof(ClassBindingContextTests), platform, inputString);
		Assert.Single (syntaxTrees);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		var classDeclaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<ClassDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (classDeclaration);
		var symbol = semanticModel.GetDeclaredSymbol (classDeclaration);
		Assert.NotNull (symbol);
		var rootContext = new RootBindingContext (compilation);
		var classBindingContext = new ClassBindingContext (rootContext, semanticModel, symbol, classDeclaration);
		Assert.Equal (expectedRegisterName, classBindingContext.RegisterName);
	}
}
