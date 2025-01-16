// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
#pragma warning disable APL0003
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.Extensions;
using ObjCBindings;
using ObjCRuntime;
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

			yield return [classText, attributesText, classAttrs];

			var interfaceText = @"
using System;

namespace Test;

[Simple]
public interface ITestInterface {
}
";
			var interfaceAttrs = new HashSet<string> { "Test.SimpleAttribute" };
			yield return [interfaceText, attributesText, interfaceAttrs];

			var enumText = @"
namespace Test;

public enum TestEnum {
	First,
	Second,
}
";
			HashSet<string> enumAttrs = new ();
			yield return [enumText, attributesText, enumAttrs];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataGetAttributeDataPresent>]
	public void GetAttributeDataPresent (ApplePlatform platform, string inputText, string attributesText,
		HashSet<string> expectedAttributes)
	{
		var (compilation, syntaxTrees) = CreateCompilation (platform, sources: [inputText, attributesText]);
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

	class TestDataGetParents : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string nestedMethodNestedClass = @"
using System;

namespace Test;

public class ParentClass{
	public class ChildClass{
		public void Method(){}
	}
}
";
			Func<SyntaxNode, CSharpSyntaxNode?> getNestedMethod =
				rootNode => rootNode.DescendantNodes ().OfType<MethodDeclarationSyntax> ().LastOrDefault ();
			var nestedMethodNestedClassParents = new [] { "ChildClass", "ParentClass" };
			yield return [nestedMethodNestedClass, getNestedMethod, nestedMethodNestedClassParents];

			const string methodInClass = @"
using System;

namespace Test;

public class ParentClass{
	public void Method(){}
}
";
			var methodParents = new [] { "ParentClass" };
			yield return [methodInClass, getNestedMethod, methodParents];

			const string nestedNamespacesNestedClass = @"
using System;

namespace Test {
	namespace Foo {
		namespace Bar {
			public class ParentClass{
				public void Method(){}
			}
		}
	}
}
";

			var nestedNamespacesParents = new [] { "ParentClass" };
			yield return [nestedNamespacesNestedClass, getNestedMethod, nestedNamespacesParents];


			Func<SyntaxNode, CSharpSyntaxNode?> getEnumValue =
				rootNode => rootNode.DescendantNodes ().OfType<EnumMemberDeclarationSyntax> ().LastOrDefault ();
			const string enumValueNested = @"
using System;

namespace Test;

public class ParentClass {
	public class ChildClass {
		public enum MyEnum {
			First,
		}
	}
}
";
			var enumParensts = new [] { "MyEnum", "ChildClass", "ParentClass" };
			yield return [enumValueNested, getEnumValue, enumParensts];
			
			Func<SyntaxNode, CSharpSyntaxNode?> getGetterValue =
				rootNode => rootNode.DescendantNodes ().OfType<AccessorDeclarationSyntax> ().LastOrDefault ();
			const string propertyGetter = @"
using System;

namespace Test;

public class ParentClass {
	public class ChildClass {
		public int Property {
			get {
				return 0;
			}
		}
	}
}
";
			var getterParents = new [] { "Property", "ChildClass", "ParentClass" };
			yield return [propertyGetter, getGetterValue, getterParents];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataGetParents>]
	public void GetParentTests (ApplePlatform platform, string inputText,
		Func<SyntaxNode, CSharpSyntaxNode?> getNode, string [] expectedParents)
	{
		var (compilation, syntaxTrees) = CreateCompilation (platform, sources: inputText);
		Assert.Single (syntaxTrees);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		var declaration = getNode (syntaxTrees [0].GetRoot ());
		Assert.NotNull (declaration);
		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);
		var parents = symbol.GetParents ().Select (p => p.Name).ToArray ();
		Assert.Equal (expectedParents, parents);
	}

	class TestDataGetSupportedPlatforms : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{

			var builder = SymbolAvailability.CreateBuilder ();
			Func<SyntaxNode, MemberDeclarationSyntax?> getNestedMethod =
				rootNode => rootNode.DescendantNodes ().OfType<MethodDeclarationSyntax> ().LastOrDefault ();

			// base member decorated, not child
			const string decoratedParent = @"
using System;
using System.Runtime.Versioning;

namespace Test;

[SupportedOSPlatform (""ios12.0"")]
public class ParentClass{
	public void Method(){}
}
";
			builder.Add (new SupportedOSPlatformData ("ios12.0"));
			yield return [decoratedParent, getNestedMethod, builder.ToImmutable ()];
			builder.Clear ();

			// base member decorated, child decorated
			const string decoratedParentAndChild = @"
using System;
using System.Runtime.Versioning;

namespace Test;

[SupportedOSPlatform (""ios"")]
public class ParentClass{
	[UnsupportedOSPlatform (""tvos"")]
	[SupportedOSPlatform (""ios12.0"")]
	public void Method(){}
}
";
			builder.Add (new SupportedOSPlatformData ("ios12.0"));
			builder.Add (new UnsupportedOSPlatformData ("tvos"));
			yield return [decoratedParentAndChild, getNestedMethod, builder.ToImmutable ()];
			builder.Clear ();

			// base member, child, granchild not decorated
			const string grandChild = @"
using System;
using System.Runtime.Versioning;

namespace Test;

[SupportedOSPlatform (""ios"")]
public class ParentClass{
	[UnsupportedOSPlatform (""tvos"")]
	public class ChildClass {
		public void Method(){}
	}
}
";
			builder.Add (new SupportedOSPlatformData ("ios"));
			builder.Add (new UnsupportedOSPlatformData ("tvos"));
			yield return [grandChild, getNestedMethod, builder.ToImmutable ()];
			builder.Clear ();

			// all decorated
			const string allDecorated = @"
using System;
using System.Runtime.Versioning;

namespace Test;

[SupportedOSPlatform (""ios"")]
public class ParentClass{
	[UnsupportedOSPlatform (""tvos"")]
	public class ChildClass {
		[SupportedOSPlatform (""ios13.0"")]
		public void Method(){}
	}
}
";
			builder.Add (new SupportedOSPlatformData ("ios13.0"));
			builder.Add (new UnsupportedOSPlatformData ("tvos"));
			yield return [allDecorated, getNestedMethod, builder.ToImmutable ()];
			builder.Clear ();

			// enum decorated
			Func<SyntaxNode, MemberDeclarationSyntax?> getEnumValue =
				rootNode => rootNode.DescendantNodes ().OfType<EnumMemberDeclarationSyntax> ().LastOrDefault ();
			const string enumDecorated = @"
using System;
using System.Runtime.Versioning;

namespace Test;

[SupportedOSPlatform (""ios"")]
public class ParentClass{
	[UnsupportedOSPlatform (""tvos"")]
	public enum MyEnum {
		[SupportedOSPlatform (""ios13.0"")]
		First,
	}
}
";

			builder.Add (new SupportedOSPlatformData ("ios13.0"));
			builder.Add (new UnsupportedOSPlatformData ("tvos"));
			yield return [enumDecorated, getEnumValue, builder.ToImmutable ()];
			builder.Clear ();

		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataGetSupportedPlatforms>]
	void GetSupportedPlatforms (ApplePlatform platform, string inputText,
		Func<SyntaxNode, MemberDeclarationSyntax?> getNode,
		SymbolAvailability expectedAvailability)
	{
		var (compilation, syntaxTrees) = CreateCompilation (platform, sources: inputText);
		Assert.Single (syntaxTrees);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		var declaration = getNode (syntaxTrees [0].GetRoot ());
		Assert.NotNull (declaration);
		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);
		var availability = symbol.GetSupportedPlatforms ();
		Assert.Equal (availability, expectedAvailability);
	}

	class TestDataHasAttribute : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string supportedOS = @"
using System;
using System.Runtime.Versioning;

namespace Test;

[SupportedOSPlatform (""ios12.0"")]
public class ParentClass{
	public void Method(){}
}
";
			yield return [supportedOS, AttributesNames.SupportedOSPlatformAttribute, true];
			yield return [supportedOS, AttributesNames.UnsupportedOSPlatformAttribute, false];
		}
		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataHasAttribute>]
	void HasAttributeTests (ApplePlatform platform, string inputText, string attrName, bool expected)
	{
		var (compilation, syntaxTrees) = CreateCompilation (platform, sources: inputText);
		Assert.Single (syntaxTrees);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<BaseTypeDeclarationSyntax> ()
			.LastOrDefault ();
		Assert.NotNull (declaration);
		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);
		Assert.Equal (expected, symbol.HasAttribute (attrName));
	}

	class TestDataIsSmartEnum : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string classExample = @"
using System;
namespace Test;

public class ClassExample {
}";
			yield return [classExample, false];

			const string interfaceExample = @"
using System;
namespace Test;

public interface InterfaceExample {
}";

			yield return [interfaceExample, false];

			const string enumExample = @"
using System;
namespace Test;

public enum MyEnum {
	None,
}";

			yield return [enumExample, false];

			const string smartEnumExample = @"
using ObjCBindings;
namespace Test;

[BindingType]
public enum MyEnum {
	None,
}";

			yield return [smartEnumExample, true];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataIsSmartEnum>]
	void IsSmartEnumTests (ApplePlatform platform, string inputText, bool expected)
	{
		var (compilation, syntaxTrees) = CreateCompilation (platform, sources: inputText);
		Assert.Single (syntaxTrees);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<BaseTypeDeclarationSyntax> ()
			.LastOrDefault ();
		Assert.NotNull (declaration);
		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);
		Assert.Equal (expected, symbol.IsSmartEnum ());
	}

	class TestDataGetExportData : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string noAttrPropertyClass = @"
using ObjCBindings;

namespace NS;

[BindingType]
public partial class MyClass {
	public static partial string Name { get; set; } = string.Empty;
}
";
			yield return [noAttrPropertyClass, Property.Default, null!];

			const string singlePropertyClass = @"
using ObjCBindings;

namespace NS;

[BindingType]
public partial class MyClass {
	[Export<Property> (""name"")]
	public partial string Name { get; set; } = string.Empty;
}
";
			yield return [singlePropertyClass, Property.Default, new ExportData<Property> ("name")];

			const string notificationPropertyClass = @"
using ObjCBindings;

namespace NS;

[BindingType]
public partial class MyClass {
	[Export<Property> (""name"", Property.Notification)]
	public partial string Name { get; set; } = string.Empty;
}
";
			yield return [
				notificationPropertyClass,
				Property.Default,
				new ExportData<Property> ("name", ArgumentSemantic.None, Property.Notification)
			];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataGetExportData>]
	void GetExportData<T> (ApplePlatform platform, string inputText, T @enum, ExportData<T>? expectedData)
		where T : Enum
	{
		Assert.NotNull (@enum);
		var (compilation, syntaxTrees) = CreateCompilation (platform, sources: inputText);
		Assert.Single (syntaxTrees);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<PropertyDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		Assert.NotNull (semanticModel);
		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);
		var exportData = symbol.GetExportData<T> ();
		Assert.Equal (expectedData, exportData);
	}

	class TestDataGetFieldData : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string fieldPropertyClass = @"
using ObjCBindings;

namespace NS;

[BindingType]
public partial class MyClass {
	[Field<Property> (""CONSTANT"")]
	public static partial string Name { get; set; } = string.Empty;
}
";
			yield return [fieldPropertyClass, Property.Default, new FieldData<Property> ("CONSTANT")];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataGetFieldData>]
	void GetFieldData<T> (ApplePlatform platform, string inputText, T @enum, FieldData<T>? expectedData)
		where T : Enum
	{
		Assert.NotNull (@enum);
		var (compilation, syntaxTrees) = CreateCompilation (platform, sources: inputText);
		Assert.Single (syntaxTrees);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<PropertyDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		Assert.NotNull (semanticModel);
		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);
		var fieldData = symbol.GetFieldData<T> ();
		Assert.Equal (expectedData, fieldData);
	}

	class TestDataIsBlittablePrimitiveType : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string byteProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public byte Property { get; set; }
}
";
			yield return [byteProperty, true];

			const string otherByteProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public Byte Property { get; set; }
}
";
			yield return [otherByteProperty, true];

			const string sbyteProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public sbyte Property { get; set; }
}
";
			yield return [sbyteProperty, true];

			const string otherSByteProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public SByte Property { get; set; }
}
";
			yield return [otherSByteProperty, true];

			const string intProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public int Property { get; set; }
}
";
			yield return [intProperty, true];

			const string int16Property = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public Int16 Property { get; set; }
}
";
			yield return [int16Property, true];

			const string shortProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public short Property { get; set; }
}
";
			yield return [shortProperty, true];

			const string uint16Property = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public UInt16 Property { get; set; }
}
";
			yield return [uint16Property, true];

			const string ushortProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public ushort Property { get; set; }
}
";
			yield return [ushortProperty, true];

			const string int32Property = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public Int32 Property { get; set; }
}
";
			yield return [int32Property, true];

			const string uint32Property = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public UInt32 Property { get; set; }
}
";
			yield return [uint32Property, true];

			const string uintProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public uint Property { get; set; }
}
";
			yield return [uintProperty, true];

			const string int64Property = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public Int64 Property { get; set; }
}
";
			yield return [int64Property, true];

			const string longProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public long Property { get; set; }
}
";
			yield return [longProperty, true];

			const string uint64Property = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public UInt64 Property { get; set; }
}
";
			yield return [uint64Property, true];

			const string ulongProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public ulong Property { get; set; }
}
";
			yield return [ulongProperty, true];

			const string intPtrProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public IntPtr Property { get; set; }
}
";
			yield return [intPtrProperty, true];

			const string uintPtrProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public UIntPtr Property { get; set; }
}
";
			yield return [uintPtrProperty, true];

			const string singleProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public Single Property { get; set; }
}
";
			yield return [singleProperty, true];

			const string floatProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public float Property { get; set; }
}
";
			yield return [floatProperty, true];

			const string doubleProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public double Property { get; set; }
}
";
			yield return [doubleProperty, true];

			const string otherDoubleProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public Double Property { get; set; }
}
";
			yield return [otherDoubleProperty, true];

			const string decimalProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public Decimal Property { get; set; }
}
";
			yield return [decimalProperty, false];

			const string charProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public char Property { get; set; }
}
";
			yield return [charProperty, false];

			const string otherCharProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public Char Property { get; set; }
}
";
			yield return [otherCharProperty, false];

			const string stringProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public string Property { get; set; }
}
";
			yield return [stringProperty, false];

			const string otherStringProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public String Property { get; set; }
}
";
			yield return [otherStringProperty, false];

		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	class TestDataIsBlittableStructs : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string emptyStructNoLayout = @"
using System;
using System.Runtime.InteropServices;
using ObjCBindings;

namespace NS;

public struct MyStruct {}

[BindingType<Class>]
public partial class MyClass {
	public MyStruct Property { get; set; }
}
";
			yield return [emptyStructNoLayout, false];

			const string emptyStructExplicitLayout = @"
using System;
using System.Runtime.InteropServices;
using ObjCBindings;

namespace NS;

[StructLayout(LayoutKind.Explicit)]
public struct MyStruct {}

[BindingType<Class>]
public partial class MyClass {
	public MyStruct Property { get; set; }
}
";
			yield return [emptyStructExplicitLayout, true];

			const string emptyStructSequentialLayout = @"
using System;
using System.Runtime.InteropServices;
using ObjCBindings;

namespace NS;

[StructLayout(LayoutKind.Sequential)]
public struct MyStruct {}

[BindingType<Class>]
public partial class MyClass {
	public MyStruct Property { get; set; }
}
";
			yield return [emptyStructSequentialLayout, true];

			const string emptyStructAutoLayout = @"
using System;
using System.Runtime.InteropServices;
using ObjCBindings;

namespace NS;

[StructLayout(LayoutKind.Auto)]
public struct MyStruct {}

[BindingType<Class>]
public partial class MyClass {
	public MyStruct Property { get; set; }
}
";
			yield return [emptyStructAutoLayout, false];

			const string structBlittableContent = @"
using System;
using System.Runtime.InteropServices;
using ObjCBindings;

namespace NS;

[StructLayout(LayoutKind.Sequential)]
public struct MyStruct {
	int a;
	double b;
}

[BindingType<Class>]
public partial class MyClass {
	public MyStruct Property { get; set; }
}
";
			yield return [structBlittableContent, true];

			const string structNonBlittableContent = @"
using System;
using System.Runtime.InteropServices;
using ObjCBindings;

namespace NS;

[StructLayout(LayoutKind.Sequential)]
public struct MyStruct {
	int a;
	string b;
}

[BindingType<Class>]
public partial class MyClass {
	public MyStruct Property { get; set; }
}
";
			yield return [structNonBlittableContent, false];

			const string nestedStructBlittableContent = @"
using System;
using System.Runtime.InteropServices;
using ObjCBindings;

namespace NS;

[StructLayout(LayoutKind.Sequential)]
public struct MyInnerStruct {
	int a;
	double b;
}

[StructLayout(LayoutKind.Sequential)]
public struct MyStruct {
	int a;
	double b;
	MyInnerStruct c;
}

[BindingType<Class>]
public partial class MyClass {
	public MyStruct Property { get; set; }
}
";
			yield return [nestedStructBlittableContent, true];

			const string nestedStructNoBlittableContent = @"
using System;
using System.Runtime.InteropServices;
using ObjCBindings;

namespace NS;

[StructLayout(LayoutKind.Sequential)]
public struct MyInnerStruct {
	int a;
	string b;
}

[StructLayout(LayoutKind.Sequential)]
public struct MyStruct {
	int a;
	double b;
	MyInnerStruct c;
}

[BindingType<Class>]
public partial class MyClass {
	public MyStruct Property { get; set; }
}
";
			yield return [nestedStructNoBlittableContent, false];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	class TestDataIsBlittableArrays : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string byteProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public byte[] Property { get; set; }
}
";
			yield return [byteProperty, true];

			const string otherByteProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public Byte[] Property { get; set; }
}
";
			yield return [otherByteProperty, true];

			const string sbyteProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public sbyte[] Property { get; set; }
}
";
			yield return [sbyteProperty, true];

			const string otherSByteProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public SByte[] Property { get; set; }
}
";
			yield return [otherSByteProperty, true];

			const string intProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public int[] Property { get; set; }
}
";
			yield return [intProperty, true];

			const string int16Property = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public Int16[] Property { get; set; }
}
";
			yield return [int16Property, true];

			const string shortProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public short[] Property { get; set; }
}
";
			yield return [shortProperty, true];

			const string uint16Property = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public UInt16[] Property { get; set; }
}
";
			yield return [uint16Property, true];

			const string ushortProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public ushort[] Property { get; set; }
}
";
			yield return [ushortProperty, true];

			const string int32Property = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public Int32[] Property { get; set; }
}
";
			yield return [int32Property, true];

			const string uint32Property = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public UInt32[] Property { get; set; }
}
";
			yield return [uint32Property, true];

			const string uintProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public uint[] Property { get; set; }
}
";
			yield return [uintProperty, true];

			const string int64Property = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public Int64[] Property { get; set; }
}
";
			yield return [int64Property, true];

			const string longProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public long[] Property { get; set; }
}
";
			yield return [longProperty, true];

			const string uint64Property = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public UInt64[] Property { get; set; }
}
";
			yield return [uint64Property, true];

			const string ulongProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public ulong[] Property { get; set; }
}
";
			yield return [ulongProperty, true];

			const string intPtrProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public IntPtr[] Property { get; set; }
}
";
			yield return [intPtrProperty, true];

			const string uintPtrProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public UIntPtr[] Property { get; set; }
}
";
			yield return [uintPtrProperty, true];

			const string singleProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public Single[] Property { get; set; }
}
";
			yield return [singleProperty, true];

			const string floatProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public float[] Property { get; set; }
}
";
			yield return [floatProperty, true];

			const string doubleProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public double[] Property { get; set; }
}
";
			yield return [doubleProperty, true];

			const string otherDoubleProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public Double[] Property { get; set; }
}
";
			yield return [otherDoubleProperty, true];

			const string decimalProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public Decimal[] Property { get; set; }
}
";
			yield return [decimalProperty, false];

			const string charProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public char[] Property { get; set; }
}
";
			yield return [charProperty, false];

			const string otherCharProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public Char[] Property { get; set; }
}
";
			yield return [otherCharProperty, false];

			const string stringProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public string[] Property { get; set; }
}
";
			yield return [stringProperty, false];

			const string otherStringProperty = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public String[] Property { get; set; }
}
";
			yield return [otherStringProperty, false];

			const string structNonBlittableContent = @"
using System;
using System.Runtime.InteropServices;
using ObjCBindings;

namespace NS;

[StructLayout(LayoutKind.Sequential)]
public struct MyStruct {
	int a;
	string b;
}

[BindingType<Class>]
public partial class MyClass {
	public MyStruct[] Property { get; set; }
}
";
			yield return [structNonBlittableContent, false];

			const string nestedStructBlittableContent = @"
using System;
using System.Runtime.InteropServices;
using ObjCBindings;

namespace NS;

[StructLayout(LayoutKind.Sequential)]
public struct MyInnerStruct {
	int a;
	double b;
}

[StructLayout(LayoutKind.Sequential)]
public struct MyStruct {
	int a;
	double b;
	MyInnerStruct c;
}

[BindingType<Class>]
public partial class MyClass {
	public MyStruct[] Property { get; set; }
}
";
			yield return [nestedStructBlittableContent, true];

			const string blittableWithNonInstanceContent = @"
using System;
using System.Runtime.InteropServices;
using ObjCBindings;

namespace NS;

[StructLayout(LayoutKind.Sequential)]
public struct MyStruct {
	public static string Data = ""test"";
	int a;
	double b;
}

[BindingType<Class>]
public partial class MyClass {
	public MyStruct[] Property { get; set; }
}
";
			yield return [blittableWithNonInstanceContent, true];

		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	class TestDataIsBlittableEnums : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string defaultBackedEnum = @"
using System;
using System.Runtime.InteropServices;
using ObjCBindings;

namespace NS;

public enum MyEnum {
	First,
	Second,
}

[BindingType<Class>]
public partial class MyClass {
	public MyEnum Property { get; set; }
}
";
			yield return [defaultBackedEnum, true];

			const string byteEnum = @"
using System;
using System.Runtime.InteropServices;
using ObjCBindings;

namespace NS;

public enum MyEnum : byte {
	First,
	Second,
}

[BindingType<Class>]
public partial class MyClass {
	public MyEnum Property { get; set; }
}
";
			yield return [byteEnum, true];

			const string sbyteEnum = @"
using System;
using System.Runtime.InteropServices;
using ObjCBindings;

namespace NS;

public enum MyEnum : sbyte {
	First,
	Second,
}

[BindingType<Class>]
public partial class MyClass {
	public MyEnum Property { get; set; }
}
";
			yield return [sbyteEnum, true];

			const string shortEnum = @"
using System;
using System.Runtime.InteropServices;
using ObjCBindings;

namespace NS;

public enum MyEnum : short {
	First,
	Second,
}

[BindingType<Class>]
public partial class MyClass {
	public MyEnum Property { get; set; }
}
";
			yield return [shortEnum, true];

			const string ushortEnum = @"
using System;
using System.Runtime.InteropServices;
using ObjCBindings;

namespace NS;

public enum MyEnum : ushort {
	First,
	Second,
}

[BindingType<Class>]
public partial class MyClass {
	public MyEnum Property { get; set; }
}
";
			yield return [ushortEnum, true];

			const string intEnum = @"
using System;
using System.Runtime.InteropServices;
using ObjCBindings;

namespace NS;

public enum MyEnum : int {
	First,
	Second,
}

[BindingType<Class>]
public partial class MyClass {
	public MyEnum Property { get; set; }
}
";
			yield return [intEnum, true];

			const string uintEnum = @"
using System;
using System.Runtime.InteropServices;
using ObjCBindings;

namespace NS;

public enum MyEnum : uint {
	First,
	Second,
}

[BindingType<Class>]
public partial class MyClass {
	public MyEnum Property { get; set; }
}
";
			yield return [uintEnum, true];

			const string longEnum = @"
using System;
using System.Runtime.InteropServices;
using ObjCBindings;

namespace NS;

public enum MyEnum : long {
	First,
	Second,
}

[BindingType<Class>]
public partial class MyClass {
	public MyEnum Property { get; set; }
}
";
			yield return [longEnum, true];

			const string ulongEnum = @"
using System;
using System.Runtime.InteropServices;
using ObjCBindings;

namespace NS;

public enum MyEnum : ulong {
	First,
	Second,
}

[BindingType<Class>]
public partial class MyClass {
	public MyEnum Property { get; set; }
}
";
			yield return [ulongEnum, true];

		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataIsBlittablePrimitiveType>]
	[AllSupportedPlatformsClassData<TestDataIsBlittableStructs>]
	[AllSupportedPlatformsClassData<TestDataIsBlittableEnums>]
	void IsBlittable (ApplePlatform platform, string inputText, bool expectedResult)
	{
		var (compilation, syntaxTrees) = CreateCompilation (platform, sources: inputText);
		Assert.Single (syntaxTrees);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<PropertyDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		Assert.NotNull (semanticModel);
		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);
		Assert.Equal (expectedResult, symbol.Type.IsBlittable ());
	}

}
