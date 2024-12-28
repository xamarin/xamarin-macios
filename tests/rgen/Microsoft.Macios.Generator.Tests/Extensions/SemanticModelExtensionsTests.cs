using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.Extensions;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.Extensions;

public class SemanticModelExtensionsTests : BaseGeneratorTestClass {

	CollectionComparer<string> comparer = new ();
	class TestDataGetNameAndNamespaceTests : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{

			var builder = SymbolAvailability.CreateBuilder ();
			const string filescopedNamespaceClass = @"
namespace Test;
public class Foo {
}
";
			ImmutableArray<string> ns = ImmutableArray.Create ("Test");
			yield return [filescopedNamespaceClass, "Foo", ns, builder.ToImmutable ()];

			const string filescopedNamespaceNestedClass = @"
namespace Test;
public class Foo {
	public class Bar {
	}	
}
";
			yield return [filescopedNamespaceNestedClass, "Bar", ns, builder.ToImmutable ()];

			const string namespaceClass = @"
namespace Test {
	public class Foo {
	}
}
";

			yield return [namespaceClass, "Foo", ns, builder.ToImmutable ()];

			const string nestedNamespaces = @"
namespace Foo {
	namespace Bar {
		public class Test {}
	}
}
";
			ns = ImmutableArray.Create ("Foo", "Bar");
			yield return [nestedNamespaces, "Test", ns, builder.ToImmutable ()];

		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	class TestDataGetNameAndNamespaceAndAvailabilityTests : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{

			var builder = SymbolAvailability.CreateBuilder ();
			builder.Add (new SupportedOSPlatformData ("ios17.0"));
			builder.Add (new SupportedOSPlatformData ("tvos17.0"));
			builder.Add (new UnsupportedOSPlatformData ("macos"));
			const string filescopedNamespaceClass = @"
using System.Runtime.Versioning;
namespace Test;

[SupportedOSPlatform (""ios17.0"")]
[SupportedOSPlatform (""tvos17.0"")]
[UnsupportedOSPlatform (""macos"")]
public class Foo {
}
";
			ImmutableArray<string> ns = ImmutableArray.Create ("Test");
			yield return [filescopedNamespaceClass, "Foo", ns, builder.ToImmutable ()];

			const string filescopedNamespaceNestedClass = @"
using System.Runtime.Versioning;
namespace Test;
public class Foo {

	[SupportedOSPlatform (""ios17.0"")]
	[SupportedOSPlatform (""tvos17.0"")]
	[UnsupportedOSPlatform (""macos"")]
	public class Bar {
	}	
}
";
			yield return [filescopedNamespaceNestedClass, "Bar", ns, builder.ToImmutable ()];

			const string namespaceClass = @"
using System.Runtime.Versioning;
namespace Test {

	[SupportedOSPlatform (""ios17.0"")]
	[SupportedOSPlatform (""tvos17.0"")]
	[UnsupportedOSPlatform (""macos"")]
	public class Foo {
	}
}
";

			yield return [namespaceClass, "Foo", ns, builder.ToImmutable ()];

			const string nestedNamespaces = @"
using System.Runtime.Versioning;
namespace Foo {
	namespace Bar {
		[SupportedOSPlatform (""ios17.0"")]
		[SupportedOSPlatform (""tvos17.0"")]
		[UnsupportedOSPlatform (""macos"")]
		public class Test {}
	}
}
";
			ns = ImmutableArray.Create ("Foo", "Bar");
			yield return [nestedNamespaces, "Test", ns, builder.ToImmutable ()];

		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataGetNameAndNamespaceTests>]
	[AllSupportedPlatformsClassData<TestDataGetNameAndNamespaceAndAvailabilityTests>]
	internal void GetNameAndNamespaceTests (ApplePlatform platform, string inputText, string expectedName,
		ImmutableArray<string> expectedNamespace, SymbolAvailability expectedAvailability)
	{
		var (compilation, syntaxTrees) = CreateCompilation (platform, sources: inputText);
		Assert.Single (syntaxTrees);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<BaseTypeDeclarationSyntax> ()
			.LastOrDefault ();
		Assert.NotNull (declaration);
		var (name, @namespace, symbolAvailability) = semanticModel.GetSymbolData (declaration);
		Assert.Equal (expectedName, name);
		Assert.Equal (expectedNamespace, @namespace, comparer);
		Assert.Equal (expectedAvailability, symbolAvailability);
	}
}
