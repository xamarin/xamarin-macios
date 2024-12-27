using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Extensions;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.Extensions;

public class SemanticModelExtensionsTests : BaseGeneratorTestClass {

	ListComparer<string> comparer = new ();
	class TestDataGetNameAndNamespaceTests : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{

			const string filescopedNamespaceClass = @"
namespace Test;
public class Foo {
}
";
			ImmutableArray<string> ns = ImmutableArray.Create ("Test");
			yield return [filescopedNamespaceClass, "Foo", ns];

			const string filescopedNamespaceNestedClass = @"
namespace Test;
public class Foo {
	public class Bar {
	}	
}
";
			yield return [filescopedNamespaceNestedClass, "Bar", ns];

			const string namespaceClass = @"
namespace Test {
	public class Foo {
	}
}
";

			yield return [namespaceClass, "Foo", ns];

			const string nestedNamespaces = @"
namespace Foo {
	namespace Bar {
		public class Test {}
	}
}
";
			ns = ImmutableArray.Create ("Foo", "Bar");
			yield return [nestedNamespaces, "Test", ns];

		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataGetNameAndNamespaceTests>]
	public void GetNameAndNamespaceTests (ApplePlatform platform, string inputText, string expectedName,
		ImmutableArray<string> expectedNamespace)
	{
		var (compilation, syntaxTrees) = CreateCompilation (platform, sources: inputText);
		Assert.Single (syntaxTrees);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<BaseTypeDeclarationSyntax> ()
			.LastOrDefault ();
		Assert.NotNull (declaration);
		var (name, @namespace) = semanticModel.GetNameAndNamespace (declaration);
		Assert.Equal (expectedName, name);
		Assert.Equal (expectedNamespace, @namespace, comparer);
	}
}
