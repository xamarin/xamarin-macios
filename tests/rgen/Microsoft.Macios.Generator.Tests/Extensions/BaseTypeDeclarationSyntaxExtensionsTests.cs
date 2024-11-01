using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Extensions;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.Extensions;

public class BaseTypeDeclarationSyntaxExtensionsTests : BaseGeneratorTestClass, IEnumerable<object []> {

	const string filescopedNamespaceClass = @"
namespace Test;
public class Foo {
}
";

	const string filescopedNamespaceNestedEnum = @"
namespace Test;
public class Foo {
	public enum Bar {
	}	
}
";

	const string namespaceClass = @"
namespace Test {
	public class Foo {
	}
}
";

	const string severalNamespaces = @"
namespace Test {
	public class Foo {}
}
namespace Test2 {
	public class Bar {}
}
";

	const string nestedNamespaces = @"
namespace Foo {
	namespace Bar {
		public class Test {}
	}
}
";

	const string nestedEnum = @"
namespace Foo {
	namespace Bar {
		public class Test {
			public enum Final {
				None,
			}
		}
	}
}
";
	T GetDeclaration<T> (ApplePlatform platform, string inputText) where T : BaseTypeDeclarationSyntax
	{
		var (_, sourceTrees) = CreateCompilation (nameof (BaseTypeDeclarationSyntaxExtensionsTests), platform, inputText);
		Assert.Single (sourceTrees);
		var declaration = sourceTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<T> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		return declaration;
	}

	public IEnumerator<object []> GetEnumerator ()
	{
		foreach (var platform in Configuration.GetIncludedPlatforms (true)) {
			yield return [GetDeclaration<ClassDeclarationSyntax> (platform, filescopedNamespaceClass),
				"Test.Foo"];
			yield return [GetDeclaration<EnumDeclarationSyntax> (platform, filescopedNamespaceNestedEnum),
				"Test.Foo.Bar"];
			yield return [GetDeclaration<ClassDeclarationSyntax> (platform, namespaceClass),
				"Test.Foo"];
			yield return [GetDeclaration<ClassDeclarationSyntax> (platform, severalNamespaces),
				"Test.Foo"];
			yield return [GetDeclaration<ClassDeclarationSyntax> (platform, nestedNamespaces),
				"Foo.Bar.Test"];
			yield return [GetDeclaration<EnumDeclarationSyntax> (platform, nestedEnum),
				"Foo.Bar.Test.Final"];
		}
	}

	IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();

	[Theory]
	[ClassData (typeof (BaseTypeDeclarationSyntaxExtensionsTests))]
	public void GetFullyQualifiedIdentifier<T> (T declaration, string expected)
		where T : BaseTypeDeclarationSyntax
	{
		Assert.Equal (expected, declaration.GetFullyQualifiedIdentifier ());
	}
}
