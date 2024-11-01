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

public class TypeSymbolExtensionsTests : BaseGeneratorTestClass {

	class TestDataGetAttributeDataPresent : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			var attributesText = @"
using System;

namespace Test;

public class SimpleAttribute : Attribute {
}

public class AttributeWithParamsAttribute : Attribute {
        public AttributeWithParams (string name, int value) {
        }
}

";

			var classText = @"
using System;

namespace Test;

[Simple, AttributeWithParams (""test"", 2)]
public class TestClass {
}
";
			var classAttrs = new HashSet<string> { "Test.SimpleAttribute", "Test.AttributeWithParamsAttribute" };

			foreach (var platform in Configuration.GetAllPlatforms (true)) {
				if (Configuration.IsEnabled (platform))
					yield return [platform, classText, attributesText, classAttrs];
			}

			var interfaceText = @"
using System;

namespace Test;

[Simple]
public interface ITestInterface {
}
";
			var interfaceAttrs = new HashSet<string> { "Test.SimpleAttribute" };
			foreach (var platform in Configuration.GetAllPlatforms (true)) {
				if (Configuration.IsEnabled (platform))
					yield return [platform, interfaceText, attributesText, interfaceAttrs];
			}

			var enumText = @"
namespace Test;

public enum TestEnum {
	First,
	Second,
}
";
			HashSet<string> enumAttrs = new ();
			foreach (var platform in Configuration.GetAllPlatforms (true)) {
				if (Configuration.IsEnabled (platform))
					yield return [platform, enumText, attributesText, enumAttrs];
			}
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();

	}

	[Theory]
	[ClassData (typeof (TestDataGetAttributeDataPresent))]
	public void GetAttributeDataPresent (ApplePlatform platform, string inputText, string attributesText, HashSet<string> expectedAttributes)
	{
		var (compilation, syntaxTrees) = CreateCompilation (nameof (GetAttributeDataPresent),
			platform, inputText, attributesText);
		Assert.Equal (2, syntaxTrees.Length);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<BaseTypeDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);
		var attrs = symbol.GetAttributeData ();
		Assert.Equal (expectedAttributes.Count, attrs.Keys.Count);
		Assert.Multiple (() => {
			foreach (var attrName in attrs.Keys) {
				Assert.Contains (attrName, expectedAttributes);
				Assert.Single (attrs [attrName]);
			}
		});
	}
}
