#pragma warning disable APL0003
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.DataModel;
using Microsoft.Macios.Generator.Extensions;
using ObjCBindings;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.Extensions;

public class SemanticModelExtensionsTests : BaseGeneratorTestClass {

	ListComparer<string> comparer = new ();
	class TestDataGetNameAndNamespaceTests : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			var bindingData = new BindingData (new BindingTypeData<Class> ());
			var builder = SymbolAvailability.CreateBuilder ();
			const string filescopedNamespaceClass = @"
using ObjCBindings;

namespace Test;

[BindingType<Class>]
public class Foo {
}
";
			ImmutableArray<string> ns = ImmutableArray.Create ("Test");
			yield return [BindingType.Class, filescopedNamespaceClass, "Foo", ns, builder.ToImmutable (), bindingData];

			const string filescopedNamespaceNestedClass = @"
using ObjCBindings;

namespace Test;

[BindingType<Class>]
public class Foo {
	public class Bar {
	}	
}
";
			yield return [BindingType.Class, filescopedNamespaceNestedClass, "Bar", ns, builder.ToImmutable (), bindingData];

			const string namespaceClass = @"
using ObjCBindings;

namespace Test {

	[BindingType<Class>]
	public class Foo {
	}
}
";

			yield return [BindingType.Class, namespaceClass, "Foo", ns, builder.ToImmutable (), bindingData];

			const string nestedNamespaces = @"
using ObjCBindings;

namespace Foo {
	namespace Bar {
		[BindingType<Class>]
		public class Test {}
	}
}
";
			ns = ImmutableArray.Create ("Foo", "Bar");
			yield return [BindingType.Class, nestedNamespaces, "Test", ns, builder.ToImmutable (), bindingData];

		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	class TestDataGetNameAndNamespaceAndAvailabilityTests : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			var bindingData = new BindingData (new BindingTypeData<Class> ());
			var builder = SymbolAvailability.CreateBuilder ();
			builder.Add (new SupportedOSPlatformData ("ios17.0"));
			builder.Add (new SupportedOSPlatformData ("tvos17.0"));
			builder.Add (new UnsupportedOSPlatformData ("macos"));
			const string filescopedNamespaceClass = @"
using System.Runtime.Versioning;
using ObjCBindings;

namespace Test;

[BindingType<Class>]
[SupportedOSPlatform (""ios17.0"")]
[SupportedOSPlatform (""tvos17.0"")]
[UnsupportedOSPlatform (""macos"")]
public class Foo {
}
";
			ImmutableArray<string> ns = ImmutableArray.Create ("Test");
			yield return [BindingType.Class, filescopedNamespaceClass, "Foo", ns, builder.ToImmutable (), bindingData];

			const string filescopedNamespaceNestedClass = @"
using System.Runtime.Versioning;
using ObjCBindings;

namespace Test;

[BindingType<Class>]
public class Foo {

	[SupportedOSPlatform (""ios17.0"")]
	[SupportedOSPlatform (""tvos17.0"")]
	[UnsupportedOSPlatform (""macos"")]
	public class Bar {
	}	
}
";
			yield return [BindingType.Class, filescopedNamespaceNestedClass, "Bar", ns, builder.ToImmutable (), bindingData];

			const string namespaceClass = @"
using System.Runtime.Versioning;
using ObjCBindings;

namespace Test {

	[BindingType<Class>]
	[SupportedOSPlatform (""ios17.0"")]
	[SupportedOSPlatform (""tvos17.0"")]
	[UnsupportedOSPlatform (""macos"")]
	public class Foo {
	}
}
";

			yield return [BindingType.Class, namespaceClass, "Foo", ns, builder.ToImmutable (), bindingData];

			const string nestedNamespaces = @"
using System.Runtime.Versioning;
using ObjCBindings;

namespace Foo {
	namespace Bar {

		[BindingType<Class>]
		[SupportedOSPlatform (""ios17.0"")]
		[SupportedOSPlatform (""tvos17.0"")]
		[UnsupportedOSPlatform (""macos"")]
		public class Test {}
	}
}
";
			ns = ImmutableArray.Create ("Foo", "Bar");
			yield return [BindingType.Class, nestedNamespaces, "Test", ns, builder.ToImmutable (), bindingData];

		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	class TestDataGetNameAndNamespaceDiffBindingType : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			var builder = SymbolAvailability.CreateBuilder ();
			ImmutableArray<string> ns = ImmutableArray.Create ("Test");
			
			const string classBindingType = @"
using ObjCBindings;

namespace Test;

[BindingType<Class>]
public class Foo {
}
";
			yield return [BindingType.Class, classBindingType, "Foo", ns, builder.ToImmutable (), 
				new BindingData(new BindingTypeData<Class> ())];
			
			const string classBindingDisableConstructorsType = @"
using ObjCBindings;

namespace Test;

[BindingType<Class> (Flags = Class.DisableDefaultCtor)]
public class Foo {
}
";
			yield return [BindingType.Class, classBindingDisableConstructorsType, "Foo", ns, builder.ToImmutable (), 
				new BindingData(new BindingTypeData<Class> (Class.DisableDefaultCtor))];
			
			const string classBindingName = @"
using ObjCBindings;

namespace Test;

[BindingType<Class> (Name = ""ObjcFoo"")]
public class Foo {
}
";
			yield return [BindingType.Class, classBindingName, "Foo", ns, builder.ToImmutable (), 
				new BindingData(new BindingTypeData<Class> ("ObjcFoo"))];
			
			const string classBindingNameAndFlags = @"
using ObjCBindings;

namespace Test;

[BindingType<Class> (Name = ""ObjcFoo"", Flags = Class.DisableDefaultCtor)]
public class Foo {
}
";
			yield return [BindingType.Class, classBindingNameAndFlags, "Foo", ns, builder.ToImmutable (), 
				new BindingData(new BindingTypeData<Class> ("ObjcFoo", Class.DisableDefaultCtor))];
			
			const string categoryBindingType = @"
using ObjCBindings;

namespace Test;

[BindingType<Category>]
public static class Foo {
}
";
			yield return [BindingType.Category, categoryBindingType, "Foo", ns, builder.ToImmutable (),
				new BindingData(new BindingTypeData<Category> ())];
			
			const string protocolBindingType = @"
using ObjCBindings;

namespace Test;

[BindingType<Protocol>]
public interface IFoo {
}
";
			yield return [BindingType.Protocol, protocolBindingType, "IFoo", ns, builder.ToImmutable (), 
				new BindingData(new BindingTypeData<Protocol> ())];
			
			const string smartEnumBindingType = @"
using ObjCBindings;

namespace Test;

[BindingType]
public enum Foo {
	First,
}
";
			yield return [BindingType.SmartEnum, smartEnumBindingType, "Foo", ns, builder.ToImmutable (), 
				new BindingData(BindingType.SmartEnum, new ())];
		}
		
		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataGetNameAndNamespaceTests>]
	[AllSupportedPlatformsClassData<TestDataGetNameAndNamespaceAndAvailabilityTests>]
	[AllSupportedPlatformsClassData<TestDataGetNameAndNamespaceDiffBindingType>]
	internal void GetNameAndNamespaceTests (ApplePlatform platform, BindingType bindingType, string inputText, string expectedName,
		ImmutableArray<string> expectedNamespace, SymbolAvailability expectedAvailability, BindingData expectedData)
	{
		var (compilation, syntaxTrees) = CreateCompilation (platform, sources: inputText);
		Assert.Single (syntaxTrees);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<BaseTypeDeclarationSyntax> ()
			.LastOrDefault ();
		Assert.NotNull (declaration);
		semanticModel.GetSymbolData (declaration, bindingType,
			out var name, 
			out var @namespace, 
			out var symbolAvailability, 
			out var bindingData);
		Assert.Equal (expectedName, name);
		Assert.Equal (expectedNamespace, @namespace, comparer);
		Assert.Equal (expectedAvailability, symbolAvailability);
		Assert.Equal (expectedData, bindingData);
	}
}
