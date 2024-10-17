using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Extensions;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.Extensions;

public class NamedTypeSymbolExtensionsTests : BaseGeneratorTestClass {

	[Theory]
	[AllSupportedPlatforms]
	public void TryGetEnumFieldsNotEnum (ApplePlatform platform)
	{
		const string inputString = @"
namespace Test;
public class NotEnum {
}
";
		var (compilation, syntaxTrees) = CreateCompilation (nameof (TryGetEnumFieldsNotEnum), platform, inputString);
		Assert.Single (syntaxTrees);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<BaseTypeDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);
		Assert.False (symbol.TryGetEnumFields (out var fields, out var diagnostics));
		Assert.Null (fields);
		Assert.Single (diagnostics);
	}

	const string emptyEnum = @"
namespace Test;
public enum MyEnum {
}
";

	const string missingFieldAttributes = @"
namespace Test;
public enum MyEnum {
	First,
	Second,
	Last,
}
";

	[Theory]
	[AllSupportedPlatforms (emptyEnum)]
	[AllSupportedPlatforms (missingFieldAttributes)]
	public void TryGetEnumFieldsNoFields (ApplePlatform platform, string inputString)
	{
		var (compilation, syntaxTrees) = CreateCompilation (nameof (TryGetEnumFieldsNotEnum), platform, inputString);
		Assert.Single (syntaxTrees);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<BaseTypeDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);
		Assert.True (symbol.TryGetEnumFields (out var fields, out var diagnostics));
		Assert.NotNull (fields);
		Assert.Empty (fields);
		Assert.Null (diagnostics);
	}

	[Theory]
	[AllSupportedPlatforms]
	public void TryGetEnumFieldsWithAttr (ApplePlatform platform)
	{
		const string inputString = @"
using ObjCRuntime;
using ObjCBindings;

namespace Test;
public enum MyEnum {
	[Field<EnumValue> (""FirstBackendField"")]
	First,
	[Field<EnumValue> (""SecondBackendField"")]
	Second,
	// should be ignored because it does not have the FieldAttribute
	Last,
}
";
		var (compilation, syntaxTrees) = CreateCompilation (nameof (TryGetEnumFieldsNotEnum), platform, inputString);
		Assert.Single (syntaxTrees);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<BaseTypeDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);
		Assert.True (symbol.TryGetEnumFields (out var fields, out var diagnostics));
		// we should get no fields because there are no attributes
		Assert.Null (diagnostics);
		Assert.NotNull (fields);
		Assert.Equal (2, fields.Value.Length);
		// assert the data from the field attr
		Assert.Equal ("FirstBackendField", fields.Value [0].FieldData.SymbolName);
		Assert.Equal ("SecondBackendField", fields.Value [1].FieldData.SymbolName);
	}
}
