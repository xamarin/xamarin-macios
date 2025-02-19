// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.DataModel;
using Xamarin.Tests;
using Xamarin.Utils;
using static Microsoft.Macios.Generator.Tests.TestDataFactory;

namespace Microsoft.Macios.Transformer.Tests.DataModel;

public class ParameterTests : BaseTransformerTestClass {

	class TestDataTryCreate : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			var path = "/some/random/path.cs";

			const string simpleValueParameter = @"
using System;

namespace Test;

public class MyClass () {

	public void Hello (int value) { }
}
";

			yield return [
				(Source: simpleValueParameter, Path: path),
				new Parameter (0, ReturnTypeForInt (), "value")
			];

			const string nullableValueParameter = @"
using System;
using ObjCRuntime;

namespace Test;

public class MyClass () {

	public void Hello ([NullAllowed] int value) { }
}
";

			yield return [
				(Source: nullableValueParameter, Path: path),
				new Parameter (
					position: 0,
					type: ReturnTypeForInt (isNullable: true, keepInterfaces: true),
					name: "value") {
					Attributes = [
						new (name: "NullAllowedAttribute"),
					]
				}
			];

			const string arrayValueParameter = @"
using System;
using ObjCRuntime;

namespace Test;

public class MyClass () {

	public void Hello (int[] value) { }
}
";
			yield return [
				(Source: arrayValueParameter, Path: path),
				new Parameter (
					position: 0,
					type: ReturnTypeForArray ("int", isBlittable: true),
					name: "value")
			];

			const string nullableArrayValueParameter = @"
using System;
using ObjCRuntime;

namespace Test;

public class MyClass () {

	public void Hello ([NullAllowed] int[] value) { }
}
";
			yield return [
				(Source: nullableArrayValueParameter, Path: path),
				new Parameter (
					position: 0,
					type: ReturnTypeForArray ("int", isBlittable: false, isNullable: true),
					name: "value") {
					Attributes = [
						new (name: "NullAllowedAttribute"),
					],
				}
			];

			const string referenceParameter = @"
using System;
using ObjCRuntime;

namespace Test;

public class MyClass () {

	public void Hello (string value) { }
}
";
			yield return [
				(Source: referenceParameter, Path: path),
				new Parameter (0, ReturnTypeForString (), "value")
			];

			const string nullableReferenceParameter = @"
using System;
using ObjCRuntime;

namespace Test;

public class MyClass () {

	public void Hello ([NullAllowed] string value) { }
}
";
			yield return [
				(Source: nullableReferenceParameter, Path: path),
				new Parameter (
					position: 0,
					type: ReturnTypeForString (isNullable: true),
					name: "value") {
					Attributes = [
						new (name: "NullAllowedAttribute"),
					]
				}
			];

			const string nsobjectParameter = @"
using System;
using Foundation;
using ObjCRuntime;

namespace Test;

public class MyClass () {

	public void Hello (NSObject value) { }
}
";

			yield return [
				(Source: nsobjectParameter, Path: path),
				new Parameter (
					position: 0,
					type: ReturnTypeForNSObject (isApiDefinition: true),
					name: "value")
			];


			const string enumParameter = @"
using System;
using Foundation;
using ObjCRuntime;

namespace Test;

public enum MyEnum {
	One,
	Two,
	Three
}

public class MyClass () {

	public void Hello (MyEnum value) { }
}
";
			yield return [
				(Source: enumParameter, Path: path),
				new Parameter (
					position: 0,
					type: ReturnTypeForEnum ("Test.MyEnum", underlyingType: SpecialType.System_Int32),
					name: "value")
			];

			const string nullableEnumParameter = @"
using System;
using Foundation;
using ObjCRuntime;

namespace Test;

public enum MyEnum {
	One,
	Two,
	Three
}

public class MyClass () {
	public void Hello ([NullAllowed] MyEnum value) { }
}
";
			yield return [
				(Source: nullableEnumParameter, Path: path),
				new Parameter (
					position: 0,
					type: ReturnTypeForEnum ("Test.MyEnum", isNullable: true, isBlittable: false, underlyingType: SpecialType.System_Int32),
					name: "value") {
					Attributes = [
						new (name: "NullAllowedAttribute"),
					]
				}
			];

			const string nativeEnumParameter = @"
using System;
using Foundation;
using ObjCRuntime;

namespace Test;

[Native]
public enum MyEnum {
	One,
	Two,
	Three
}

public class MyClass () {
	public void Hello (MyEnum value) { }
}
";
			yield return [
				(Source: nativeEnumParameter, Path: path),
				new Parameter (
					position: 0,
					type: ReturnTypeForEnum ("Test.MyEnum", isNativeEnum: true, underlyingType: SpecialType.System_Int32),
					name: "value")
			];

			const string blittableStructParam = @"
using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;

namespace Test;

[StructLayout(LayoutKind.Sequential)]
public struct MyEnum {
	public int First { get; }
	public int Second { get; }
}

public class MyClass () {
	public void Hello (MyEnum value) { }
}
";
			yield return [
				(Source: blittableStructParam, Path: path),
				new Parameter (0, ReturnTypeForStruct ("Test.MyEnum", isBlittable: true), "value")
			];


			const string nonBlittableStructParam = @"
using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;

namespace Test;

public struct MyEnum {
	public int First { get; }
	public int Second { get; }
}

public class MyClass () {
	public void Hello (MyEnum value) { }
}
";
			yield return [
				(Source: nonBlittableStructParam, Path: path),
				new Parameter (0, ReturnTypeForStruct ("Test.MyEnum"), "value")
			];

			const string interfaceParameter = @"
using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;

namespace Test;

public interface IMyInterface { }

public class MyClass () {
	public void Hello (IMyInterface value) { }
}
";
			yield return [
				(Source: interfaceParameter, Path: path),
				new Parameter (0, ReturnTypeForInterface ("Test.IMyInterface"), "value")
			];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataTryCreate>]
	void TryCreateTests (ApplePlatform platform, (string Source, string Path) source, Parameter expectedData)
	{
		var compilation = CreateCompilation (platform, sources: source);
		var syntaxTree = compilation.SyntaxTrees.ForSource (source);
		var trees = compilation.SyntaxTrees.Where (s => s.FilePath == source.Path).ToArray ();
		Assert.Single (trees);
		Assert.NotNull (syntaxTree);

		var semanticModel = compilation.GetSemanticModel (syntaxTree);
		Assert.NotNull (semanticModel);

		var declaration = syntaxTree.GetRoot ()
				.DescendantNodes ().OfType<ParameterSyntax> ()
				.LastOrDefault ();

		Assert.NotNull (declaration);

		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);
		Assert.True (Parameter.TryCreate (symbol, declaration, semanticModel, out var parameter));
		Assert.Equal (expectedData, parameter);
	}
}
