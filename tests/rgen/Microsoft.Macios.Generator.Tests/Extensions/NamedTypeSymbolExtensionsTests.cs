using System.Collections;
using System.Collections.Generic;
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

	class TestDataTryGetEnumFieldsNoFields : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{

			foreach (var platform in Configuration.GetIncludedPlatforms (true)) {

				const string emptyEnum = @"
namespace Test;
public enum MyEnum {
}
";
				yield return [platform, emptyEnum];

				const string missingFieldAttributes = @"
namespace Test;
public enum MyEnum {
	First,
	Second,
	Last,
}
";
				yield return [platform, missingFieldAttributes];


				const string fieldWithQuotes = @"
namespace Test;
public enum MyEnum {
	[Field<EnumValue> (""Field\""With\""Quotes"")]
	First,
}
";
				yield return [platform, fieldWithQuotes];

				const string leadingWithNumber = @"
namespace Test;
public enum MyEnum {
	[Field<EnumValue> (""42Tries"")]
	First,
}
";
				yield return [platform, leadingWithNumber];

				const string fieldWithNewLines = @"
namespace Test;
public enum MyEnum {
	[Field<EnumValue> (""With\nNew\nLine"")]
	First,
}
";
				yield return [platform, fieldWithNewLines];

				const string fieldWithKeyword = @"
namespace Test;
public enum MyEnum {
	[Field<EnumValue> (""class"")]
	First,
}
";
				yield return [platform, fieldWithKeyword];

				const string fieldWithTabs = @"
namespace Test;
public enum MyEnum {
	[Field<EnumValue> ("" \tSecondBackendField\t \n"")]
	First,
}
";

				yield return [platform, fieldWithTabs];
			}
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}


	[Theory]
	[ClassData (typeof (TestDataTryGetEnumFieldsNoFields))]
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
