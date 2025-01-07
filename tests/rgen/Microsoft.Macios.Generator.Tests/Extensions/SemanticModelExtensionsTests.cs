#pragma warning disable APL0003
using System;
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

	CollectionComparer<string> nameSpaceComparer = new (); // order matters in namespaces
	CollectionComparer<string> interfacesComparer = new (StringComparer.InvariantCulture); // order does not matter in interfaces
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
			yield return [
				BindingType.Class,
				filescopedNamespaceClass,
				"Foo",
				"object",
				ImmutableArray<string>.Empty,
				ns,
				builder.ToImmutable (),
				bindingData];

			const string filescopedNamespaceNestedClass = @"
using ObjCBindings;

namespace Test;

[BindingType<Class>]
public class Foo {
	public class Bar {
	}	
}
";
			yield return [
				BindingType.Class,
				filescopedNamespaceNestedClass,
				"Bar",
				"object",
				ImmutableArray<string>.Empty,
				ns,
				builder.ToImmutable (),
				bindingData];

			const string namespaceClass = @"
using ObjCBindings;

namespace Test {

	[BindingType<Class>]
	public class Foo {
	}
}
";

			yield return [
				BindingType.Class,
				namespaceClass,
				"Foo",
				"object",
				ImmutableArray<string>.Empty,
				ns,
				builder.ToImmutable (),
				bindingData];

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
			yield return [
				BindingType.Class,
				nestedNamespaces,
				"Test",
				"object",
				ImmutableArray<string>.Empty,
				ns,
				builder.ToImmutable (),
				bindingData];

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
			yield return [
				BindingType.Class,
				filescopedNamespaceClass,
				"Foo",
				"object",
				ImmutableArray<string>.Empty,
				ns,
				builder.ToImmutable (),
				bindingData];

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
			yield return [
				BindingType.Class,
				filescopedNamespaceNestedClass,
				"Bar",
				"object",
				ImmutableArray<string>.Empty,
				ns,
				builder.ToImmutable (),
				bindingData];

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

			yield return [
				BindingType.Class,
				namespaceClass,
				"Foo",
				"object",
				ImmutableArray<string>.Empty,
				ns,
				builder.ToImmutable (),
				bindingData];

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
			yield return [
				BindingType.Class,
				nestedNamespaces,
				"Test",
				"object",
				ImmutableArray<string>.Empty,
				ns,
				builder.ToImmutable (),
				bindingData];

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
			yield return [BindingType.Class,
				classBindingType,
				"Foo",
				"object",
				ImmutableArray<string>.Empty,
				ns,
				builder.ToImmutable (),
				new BindingData (new BindingTypeData<Class> ())];

			const string classBindingDisableConstructorsType = @"
using ObjCBindings;

namespace Test;

[BindingType<Class> (Flags = Class.DisableDefaultCtor)]
public class Foo {
}
";
			yield return [BindingType.Class,
				classBindingDisableConstructorsType,
				"Foo",
				"object",
				ImmutableArray<string>.Empty,
				ns,
				builder.ToImmutable (),
				new BindingData (new BindingTypeData<Class> (Class.DisableDefaultCtor))];

			const string classBindingName = @"
using ObjCBindings;

namespace Test;

[BindingType<Class> (Name = ""ObjcFoo"")]
public class Foo {
}
";
			yield return [BindingType.Class,
				classBindingName,
				"Foo",
				"object",
				ImmutableArray<string>.Empty,
				ns,
				builder.ToImmutable (),
				new BindingData (new BindingTypeData<Class> ("ObjcFoo"))];

			const string classBindingNameAndFlags = @"
using ObjCBindings;

namespace Test;

[BindingType<Class> (Name = ""ObjcFoo"", Flags = Class.DisableDefaultCtor)]
public class Foo {
}
";
			yield return [BindingType.Class,
				classBindingNameAndFlags,
				"Foo",
				"object",
				ImmutableArray<string>.Empty,
				ns,
				builder.ToImmutable (),
				new BindingData (new BindingTypeData<Class> ("ObjcFoo", Class.DisableDefaultCtor))];

			const string categoryBindingType = @"
using ObjCBindings;

namespace Test;

[BindingType<Category>]
public static class Foo {
}
";
			yield return [BindingType.Category,
				categoryBindingType,
				"Foo",
				"object",
				ImmutableArray<string>.Empty,
				ns,
				builder.ToImmutable (),
				new BindingData (new BindingTypeData<Category> ())];

			const string protocolBindingType = @"
using ObjCBindings;

namespace Test;

[BindingType<Protocol>]
public interface IFoo {
}
";
			yield return [BindingType.Protocol,
				protocolBindingType,
				"IFoo",
				null!,
				ImmutableArray<string>.Empty,
				ns,
				builder.ToImmutable (),
				new BindingData (new BindingTypeData<Protocol> ())];

			const string smartEnumBindingType = @"
using ObjCBindings;

namespace Test;

[BindingType]
public enum Foo {
	First,
}
";
			yield return [BindingType.SmartEnum,
				smartEnumBindingType,
				"Foo",
				"System.Enum",
				ImmutableArray<string>.Empty,
				ns,
				builder.ToImmutable (),
				new BindingData (BindingType.SmartEnum, new ())];

		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	class TestDataGetNameAndNamespaceInheritanceTests : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			var classBindingData = new BindingData (new BindingTypeData<Class> ());
			var protocolBindingData = new BindingData (new BindingTypeData<Protocol> ());
			var builder = SymbolAvailability.CreateBuilder ();
			ImmutableArray<string> ns = ImmutableArray.Create ("Test");

			const string classBindingNoBase = @"
using Foundation;
using ObjCBindings;

namespace Test;

[BindingType<Class>]
public class Foo {
}
";
			yield return [
				BindingType.Class,
				classBindingNoBase,
				"Foo",
				"object",
				ImmutableArray<string>.Empty,
				ns,
				builder.ToImmutable (),
				classBindingData
			];

			const string classBindingNoBaseAndInterface = @"
using Foundation;
using ObjCBindings;

namespace Test;

public interface IFoo {}

[BindingType<Class>]
public class Foo : IFoo {
}
";
			yield return [
				BindingType.Class,
				classBindingNoBaseAndInterface,
				"Foo",
				"object",
				ImmutableArray.Create ("Test.IFoo"),
				ns,
				builder.ToImmutable (),
				classBindingData
			];

			const string classBindingNoBaseAndSeveralInterface = @"
using Foundation;
using ObjCBindings;

namespace Test;

public interface IFoo {}

public interface IBar {}

[BindingType<Class>]
public class Foo : IFoo, IBar {
}
";
			yield return [
				BindingType.Class,
				classBindingNoBaseAndSeveralInterface,
				"Foo",
				"object",
				ImmutableArray.Create ("Test.IFoo", "Test.IBar"),
				ns,
				builder.ToImmutable (),
				classBindingData
			];

			const string classBindingWithBase = @"
using Foundation;
using ObjCBindings;

namespace Test;

[BindingType<Class>]
public class Foo : NSObject {
}
";
			yield return [
				BindingType.Class,
				classBindingWithBase,
				"Foo",
				"Foundation.NSObject",
				ImmutableArray<string>.Empty,
				ns,
				builder.ToImmutable (),
				classBindingData
			];

			const string classBindingWithBaseInterface = @"
using Foundation;
using ObjCBindings;

namespace Test;

public interface IFoo {}

[BindingType<Class>]
public class Foo : NSObject, IFoo {
}
";
			yield return [
				BindingType.Class,
				classBindingWithBaseInterface,
				"Foo",
				"Foundation.NSObject",
				ImmutableArray.Create ("Test.IFoo"),
				ns,
				builder.ToImmutable (),
				classBindingData
			];

			const string classBindingWithBaseSeveralInterface = @"
using Foundation;
using ObjCBindings;

namespace Test;

public interface IFoo {}

public interface IBar {}

[BindingType<Class>]
public class Foo : NSObject, IFoo, IBar {
}
";
			yield return [
				BindingType.Class,
				classBindingWithBaseSeveralInterface,
				"Foo",
				"Foundation.NSObject",
				ImmutableArray.Create ("Test.IFoo", "Test.IBar"),
				ns,
				builder.ToImmutable (),
				classBindingData
			];

			const string interfaceBinding = @"
using Foundation;
using ObjCBindings;

namespace Test;

[BindingType<Protocol>]
public interface IFoo {
}
";
			yield return [
				BindingType.Protocol,
				interfaceBinding,
				"IFoo",
				null!,
				ImmutableArray<string>.Empty,
				ns,
				builder.ToImmutable (),
				protocolBindingData
			];

			const string interfaceWithBaseBinding = @"
using Foundation;
using ObjCBindings;

namespace Test;

public interface IBar {}

[BindingType<Protocol>]
public interface IFoo : IBar {
}
";
			yield return [
				BindingType.Protocol,
				interfaceWithBaseBinding,
				"IFoo",
				null!,
				ImmutableArray.Create ("Test.IBar"),
				ns,
				builder.ToImmutable (),
				protocolBindingData
			];

			const string interfaceWithSeveralBaseBinding = @"
using Foundation;
using ObjCBindings;

namespace Test;

public interface IBar {}
public interface IBaz {}

[BindingType<Protocol>]
public interface IFoo : IBar, IBaz {
}
";
			yield return [
				BindingType.Protocol,
				interfaceWithSeveralBaseBinding,
				"IFoo",
				null!,
				ImmutableArray.Create ("Test.IBar", "Test.IBaz"),
				ns,
				builder.ToImmutable (),
				protocolBindingData
			];

			const string interfaceWithBaseListGenericBinding = @"
using System.Collections.Generic;
using Foundation;
using ObjCBindings;

namespace Test;


[BindingType<Protocol>]
public interface IFoo : IList<string> {
}
";
			yield return [
				BindingType.Protocol,
				interfaceWithBaseListGenericBinding,
				"IFoo",
				null!,
				ImmutableArray.Create ("System.Collections.Generic.IList<string>"),
				ns,
				builder.ToImmutable (),
				protocolBindingData
			];
		}
		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataGetNameAndNamespaceTests>]
	[AllSupportedPlatformsClassData<TestDataGetNameAndNamespaceAndAvailabilityTests>]
	[AllSupportedPlatformsClassData<TestDataGetNameAndNamespaceDiffBindingType>]
	[AllSupportedPlatformsClassData<TestDataGetNameAndNamespaceInheritanceTests>]
	internal void GetNameAndNamespaceTests (ApplePlatform platform, BindingType bindingType,
		string inputText, string expectedName, string? expectedBaseClass, ImmutableArray<string> expectedInterfaces,
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
		semanticModel.GetSymbolData (
			declaration: declaration,
			bindingType: bindingType,
			name: out var name,
			baseClass: out var baseClass,
			interfaces: out var interfaces,
			namespaces: out var @namespace,
			symbolAvailability: out var symbolAvailability,
			bindingData: out var bindingData);
		Assert.Equal (expectedName, name);
		Assert.Equal (expectedBaseClass, baseClass);
		Assert.Equal (expectedInterfaces, interfaces, interfacesComparer);
		Assert.Equal (expectedNamespace, @namespace, nameSpaceComparer);
		Assert.Equal (expectedAvailability, symbolAvailability);
		Assert.Equal (expectedData, bindingData);
	}
}
