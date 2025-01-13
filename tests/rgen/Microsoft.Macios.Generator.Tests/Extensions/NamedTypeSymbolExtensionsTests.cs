// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
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
		var (compilation, syntaxTrees) = CreateCompilation (platform, sources: inputString);
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
			const string emptyEnum = @"
using ObjCBindings;

namespace Test;
public enum MyEnum {
}
";
			yield return [emptyEnum];

			const string missingFieldAttributes = @"
namespace Test;
public enum MyEnum {
	First,
	Second,
	Last,
}
";
			yield return [missingFieldAttributes];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}


	[Theory]
	[AllSupportedPlatformsClassData<TestDataTryGetEnumFieldsNoFields>]
	public void TryGetEnumFieldsNoFields (ApplePlatform platform, string inputString)
	{
		var (compilation, syntaxTrees) = CreateCompilation (platform, sources: inputString);
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

	class TestDataTryGetEnumFieldsInvalidFields : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{

			const string fieldWithQuotes = @"
using ObjCBindings;

namespace Test;
public enum MyEnum {
	[Field<EnumValue> (""Field\""With\""Quotes"")]
	First,
}
";
			yield return [fieldWithQuotes];

			const string leadingWithNumber = @"
using ObjCBindings;

namespace Test;
public enum MyEnum {
	[Field<EnumValue> (""42Tries"")]
	First,
}
";
			yield return [leadingWithNumber];

			const string fieldWithNewLines = @"
using ObjCBindings;

namespace Test;
public enum MyEnum {
	[Field<EnumValue> (""With\nNew\nLine"")]
	First,
}
";
			yield return [fieldWithNewLines];

			const string fieldWithKeyword = @"
using ObjCBindings;

namespace Test;
public enum MyEnum {
	[Field<EnumValue> (""class"")]
	First,
}
";
			yield return [fieldWithKeyword];

			const string fieldWithTabs = @"
using ObjCBindings;

namespace Test;
public enum MyEnum {
	[Field<EnumValue> ("" \tSecondBackendField\t \n"")]
	First,
}
";

			yield return [fieldWithTabs];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataTryGetEnumFieldsInvalidFields>]
	public void TryGetEnumFieldsInvalidFields (ApplePlatform platform, string inputString)
	{
		var (compilation, syntaxTrees) = CreateCompilation (platform, sources: inputString);
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
		Assert.NotNull (diagnostics);
		Assert.Single (diagnostics);
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
		var (compilation, syntaxTrees) = CreateCompilation (platform, sources: inputString);
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
