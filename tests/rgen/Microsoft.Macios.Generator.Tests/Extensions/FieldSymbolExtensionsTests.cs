#pragma warning disable APL0003
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Extensions;
using ObjCBindings;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.Extensions;

public class FieldSymbolExtensionsTests : BaseGeneratorTestClass {


	[Theory]
	[AllSupportedPlatforms]
	public void GetFieldDataMissingAttribute (ApplePlatform platform)
	{
		const string inputString = @"
using ObjCBindings;

namespace Test;
public enum MyEnum {
	First,
	Second,
	Last,
}
";

		var (compilation, syntaxTrees) = CreateCompilation (nameof (GetFieldDataMissingAttribute), platform, inputString);
		Assert.Single (syntaxTrees);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<BaseTypeDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);
		var enumValue = symbol.GetMembers ().FirstOrDefault () as IFieldSymbol;
		Assert.NotNull (enumValue);
		var fieldData = enumValue.GetFieldData ();
		Assert.Null (fieldData);
	}

	[Theory]
	[AllSupportedPlatforms]
	public void GetFieldDataPresentAttributeWithField (ApplePlatform platform)
	{

		const string inputString = @"
using ObjCBindings;

namespace Test;
public enum MyEnum {
	[Field<EnumValue> (""First"")]
	First,
}
";
		var (compilation, syntaxTrees) = CreateCompilation (nameof (GetFieldDataMissingAttribute), platform, inputString);
		Assert.Single (syntaxTrees);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<BaseTypeDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);
		var enumValue = symbol.GetMembers ().FirstOrDefault () as IFieldSymbol;
		Assert.NotNull (enumValue);
		var fieldData = enumValue.GetFieldData ();
		Assert.NotNull (fieldData);
		Assert.Equal ("First", fieldData.Value.SymbolName);
		Assert.Null (fieldData.Value.LibraryName);
		Assert.Equal (EnumValue.None, fieldData.Value.Flags);
	}

	[Theory]
	[AllSupportedPlatforms]
	public void GetFieldDataPresentAttributeWithLibraryName (ApplePlatform platform)
	{
		const string inputString = @"
using ObjCBindings;

namespace Test;
public enum MyEnum {
	[Field<EnumValue> (""First"", ""Lib"")]
	First,
}
";
		var (compilation, syntaxTrees) = CreateCompilation (nameof (GetFieldDataMissingAttribute), platform, inputString);
		Assert.Single (syntaxTrees);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<BaseTypeDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);
		var enumValue = symbol.GetMembers ().FirstOrDefault () as IFieldSymbol;
		Assert.NotNull (enumValue);
		var fieldData = enumValue.GetFieldData ();
		Assert.NotNull (fieldData);
		Assert.Equal ("First", fieldData.Value.SymbolName);
		Assert.Equal ("Lib", fieldData.Value.LibraryName);
		Assert.Equal (EnumValue.None, fieldData.Value.Flags);
	}

	class TestDataGetFieldDataPresentAttributeNotValid : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string missingFieldAttributes = @"
using ObjCBindings;

namespace Test;
public enum MyEnum {
	First,
	Second,
	Last,
}
";
			yield return [missingFieldAttributes];


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
	[AllSupportedPlatformsClassData<TestDataGetFieldDataPresentAttributeNotValid>]
	public void GetFieldDataPresentAttributeNotValid (ApplePlatform platform, string inputString)
	{
		var (compilation, syntaxTrees) = CreateCompilation (nameof (GetFieldDataPresentAttributeNotValid), platform, inputString);
		Assert.Single (syntaxTrees);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<BaseTypeDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);
		var enumValue = symbol.GetMembers ().FirstOrDefault () as IFieldSymbol;
		Assert.NotNull (enumValue);
		var fieldData = enumValue.GetFieldData ();
		Assert.Null (fieldData);
	}
}
