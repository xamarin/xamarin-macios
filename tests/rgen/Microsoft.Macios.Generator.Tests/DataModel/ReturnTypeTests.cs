using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.DataModel;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class ReturnTypeTests : BaseGeneratorTestClass {

	class TestDataFromMethodDeclaration : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string voidMethodNoParams = @"
using System;

namespace NS {
	public class MyClass {
		public void MyMethod () {}
	}
}
";

			yield return [
				voidMethodNoParams,
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: new ("void"),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: []
				)
			];

			const string intMethodNoParams = @"
using System;

namespace NS {
	public class MyClass {
		public int MyMethod () {}
	}
}
";
			yield return [
				intMethodNoParams,
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: new (
						type: "int",
						isNullable: false,
						isBlittable: true,
						isSmartEnum: false,
						isArray: false,
						isReferenceType: false
					),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: []
				)
			];

			const string nullableIntMethodNoParams = @"
using System;

namespace NS {
	public class MyClass {
		public int? MyMethod () {}
	}
}
";
			yield return [
				nullableIntMethodNoParams,
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: new (
						type: "int",
						isNullable: true,
						isBlittable: false,
						isSmartEnum: false,
						isArray: false,
						isReferenceType: false
					),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: []
				)
			];

			const string intArrayMethodNoParams = @"
using System;

namespace NS {
	public class MyClass {
		public int[] MyMethod () {}
	}
}
";
			yield return [
				intArrayMethodNoParams,
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: new (
						type: "int",
						isNullable: false,
						isBlittable: true,
						isSmartEnum: false,
						isArray: true,
						isReferenceType: true
					),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: []
				)
			];

			const string nullableIntArrayMethodNoParams = @"
using System;

namespace NS {
	public class MyClass {
		public int[]? MyMethod () {}
	}
}
";
			yield return [
				nullableIntArrayMethodNoParams,
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: new (
						type: "int",
						isNullable: true,
						isBlittable: false,
						isSmartEnum: false,
						isArray: true,
						isReferenceType: true
					),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: []
				)
			];

			const string intNullArrayMethodNoParams = @"
using System;

namespace NS {
	public class MyClass {
		public int?[] MyMethod () {}
	}
}
";
			yield return [
				intNullArrayMethodNoParams,
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: new (
						type: "int?",
						isNullable: false,
						isBlittable: false,
						isSmartEnum: false,
						isArray: true,
						isReferenceType: true
					),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: []
				)
			];

			const string int2DArrayMethodNoParams = @"
using System;

namespace NS {
	public class MyClass {
		public int[][] MyMethod () {}
	}
}
";
			yield return [
				int2DArrayMethodNoParams,
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: new (
						type: "int[]",
						isNullable: false,
						isBlittable: true,
						isSmartEnum: false,
						isArray: true,
						isReferenceType: true
					),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: []
				)
			];

			const string stringMethodNoParams = @"
using System;

namespace NS {
	public class MyClass {
		public string MyMethod () {}
	}
}
";
			yield return [
				stringMethodNoParams,
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: new (
						type: "string",
						isNullable: false,
						isBlittable: false,
						isSmartEnum: false,
						isArray: false,
						isReferenceType: true
					),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: []
				)
			];

			const string customClassMethodNoParams = @"
using System;

namespace NS {
	public class ReturnClass {}

	public class MyClass {
		public ReturnClass MyMethod () {}
	}
}
";
			yield return [
				customClassMethodNoParams,
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: new (
						type: "NS.ReturnClass",
						isNullable: false,
						isBlittable: false,
						isSmartEnum: false,
						isArray: false,
						isReferenceType: true
					),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: []
				)
			];

			const string customStructMethodNoParams = @"
using System;

namespace NS {
	public struct ReturnClass {}

	public class MyClass {
		public ReturnClass MyMethod () {}
	}
}
";
			yield return [
				customStructMethodNoParams,
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: new (
						type: "NS.ReturnClass",
						isNullable: false,
						isBlittable: false,
						isSmartEnum: false,
						isArray: false,
						isReferenceType: false
					),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: []
				)
			];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataFromMethodDeclaration>]
	void FromMethodDeclaration (ApplePlatform platform, string inputText, Method expected)
	{
		var (compilation, syntaxTrees) = CreateCompilation (platform, sources: inputText);
		Assert.Single (syntaxTrees);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ().OfType<MethodDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		Assert.True (Method.TryCreate (declaration, semanticModel, out var changes));
		Assert.NotNull (changes);
		Assert.Equal (expected, changes);
	}
}
