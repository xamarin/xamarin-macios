// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#pragma warning disable APL0003
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.DataModel;
using ObjCRuntime;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;
using static Microsoft.Macios.Generator.Tests.TestDataFactory;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class MethodTests : BaseGeneratorTestClass {

	[Fact]
	public void IsThreadSafe ()
	{
		var method = new Method (
			type: "NS.MyClass",
			name: "MyMethod",
			returnType: ReturnTypeForVoid (),
			symbolAvailability: new (),
			exportMethodData: new ("selector", ArgumentSemantic.None, ObjCBindings.Method.IsThreadSafe),
			attributes: [],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			],
			parameters: []
		);
		Assert.True (method.IsThreadSafe);

		method = new Method (
			type: "NS.MyClass",
			name: "MyMethod",
			returnType: ReturnTypeForVoid (),
			symbolAvailability: new (),
			exportMethodData: new ("selector", ArgumentSemantic.None, ObjCBindings.Method.Default),
			attributes: [],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			],
			parameters: []
		);
		Assert.False (method.IsThreadSafe);
	}
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
					returnType: ReturnTypeForVoid (),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: []
				)
			];

			const string voidMethodNoParamsExportData = @"
using System;
using ObjCBindings;

namespace NS {
	public class MyClass {
		[Export<Method>(""myMethod"")]
		public void MyMethod () {}
	}
}
";

			yield return [
				voidMethodNoParamsExportData,
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: ReturnTypeForVoid (),
					symbolAvailability: new (),
					exportMethodData: new ("myMethod"),
					attributes: [
						new ("ObjCBindings.ExportAttribute<ObjCBindings.Method>", ["myMethod"]),
					],
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
		public string MyMethod () => string.Empty;
	}
}
";

			yield return [
				stringMethodNoParams,
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: ReturnTypeForString (),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: []
				)
			];

			const string customTypeNoParams = @"
using System;

namespace NS {
	public class CustomType {
	}

	public class MyClass {
		public CustomType MyMethod () => new ();
	}
}
";

			yield return [
				customTypeNoParams,
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: ReturnTypeForClass ("NS.CustomType"),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: []
				)
			];

			const string singleParameterMethod = @"
using System;

namespace NS {
	public class MyClass {
		public string MyMethod (string input) => $""{input}_test"";
	}
}
";

			yield return [
				singleParameterMethod,
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: ReturnTypeForString (),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: ReturnTypeForString (), name: "input"),
					]
				)
			];

			const string singleArrayParameterMethod = @"
using System;

namespace NS {
	public class MyClass {
		public string MyMethod (string[] input) => $""{input}_test"";
	}
}
";

			yield return [
				singleArrayParameterMethod,
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: ReturnTypeForString (),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: ReturnTypeForArray ("string"), name: "input")
					]
				)
			];

			const string nullableSingleArrayParameterMethod = @"
using System;

namespace NS {
	public class MyClass {
		public string MyMethod (string[]? input) => $""{input}_test"";
	}
}
";

			yield return [
				nullableSingleArrayParameterMethod,
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: ReturnTypeForString (),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: ReturnTypeForArray ("string", isNullable: true), name: "input")
					]
				)
			];

			const string singleArrayNullableParameterMethod = @"
using System;

namespace NS {
	public class MyClass {
		public string MyMethod (string?[] input) => $""{input}_test"";
	}
}
";

			yield return [
				singleArrayNullableParameterMethod,
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: ReturnTypeForString (),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: ReturnTypeForArray ("string?"), name: "input")
					]
				)
			];

			const string nullableSingleArrayNullableParameterMethod = @"
using System;

namespace NS {
	public class MyClass {
		public string MyMethod (string?[]? input) => $""{input}_test"";
	}
}
";

			yield return [
				nullableSingleArrayNullableParameterMethod,
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: ReturnTypeForString (),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: ReturnTypeForArray ("string?", isNullable: true), name: "input")
					]
				)
			];

			const string twoDimensionArrayParameterMethod = @"
using System;

namespace NS {
	public class MyClass {
		public string MyMethod (string[][] input) => $""{input}_test"";
	}
}
";

			yield return [
				twoDimensionArrayParameterMethod,
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: ReturnTypeForString (),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: ReturnTypeForArray ("string[]"), name: "input")
					]
				)
			];

			const string customTypeParameter = @"
using System;

namespace NS {
	public class CustomType {
	}

	public class MyClass {
		public void MyMethod (CustomType input) {}
	}
}
";

			yield return [
				customTypeParameter,
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: ReturnTypeForVoid (),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: ReturnTypeForClass ("NS.CustomType"), name: "input"),
					]
				)
			];


			const string severalParametersMethod = @"
using System;

namespace NS {
	public class MyClass {
		public string MyMethod (string input, string? second) => $""{input}_test{second}"";
	}
}
";

			yield return [
				severalParametersMethod,
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: ReturnTypeForString (),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: ReturnTypeForString (), name: "input"),
						new (position: 1, type: ReturnTypeForString (isNullable: true), name: "second")
					]
				)
			];

			const string outParameterMethod = @"
using System;

namespace NS {
	public class MyClass {
		public bool TryGetString (out string? example) {
			example = null;
			return false;
		}
	}
}
";
			yield return [
				outParameterMethod,
				new Method (
					type: "NS.MyClass",
					name: "TryGetString",
					returnType: ReturnTypeForBool (),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: ReturnTypeForString (isNullable: true), name: "example") {
							ReferenceKind = ReferenceKind.Out,
						},
					]
				)
			];

			const string inParameterMethod = @"
using System;

namespace NS {
	public readonly struct MyStruct {
		public string Name { get; }
		public MyStruct(string name) {
			Name = name;
		}
	}
	public class MyClass {
		public void MyMethod (in MyStruct data) {
			var s = data.Name;
		}
	}
}
";

			yield return [
				inParameterMethod,
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: ReturnTypeForVoid (),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: ReturnTypeForStruct ("NS.MyStruct"), name: "data") {
							ReferenceKind = ReferenceKind.In,
						},
					]
				)
			];

			const string refParameterMethod = @"
using System;

namespace NS {
	public readonly struct MyStruct {
		public string Name { get; }
		public MyStruct(string name) {
			Name = name;
		}
	}
	public class MyClass {
		public void MyMethod (ref MyStruct data) {
		}
	}
}
";

			yield return [
				refParameterMethod,
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: ReturnTypeForVoid (),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: ReturnTypeForStruct ("NS.MyStruct"), name: "data") {
							ReferenceKind = ReferenceKind.Ref,
						},
					]
				)
			];

			const string methodWithAttribute = @"
using System;
using System.Runtime.Versioning;

namespace NS {
	public class MyClass {
		[SupportedOSPlatform (""ios""]
		public string MyMethod (string input, string? second) => $""{input}_test{second}"";
	}
}
";
			var builder = SymbolAvailability.CreateBuilder ();
			builder.Add (new SupportedOSPlatformData ("ios"));

			yield return [
				methodWithAttribute,
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: ReturnTypeForString (),
					symbolAvailability: builder.ToImmutable (),
					exportMethodData: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: ReturnTypeForString (), name: "input"),
						new (position: 1, type: ReturnTypeForString (isNullable: true), name: "second")
					]
				)
			];

			const string parameterWithAttribute = @"
using System;
using System.Diagnostics.CodeAnalysis;

namespace NS {
	public class MyClass {
		public bool TryGetString ([NotNullWhen (true)] out string? example) {
			example = null;
			return false;
		}
	}
}
";

			yield return [
				parameterWithAttribute,
				new Method (
					type: "NS.MyClass",
					name: "TryGetString",
					returnType: ReturnTypeForBool (),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: ReturnTypeForString (isNullable: true), name: "example") {
							ReferenceKind = ReferenceKind.Out,
							Attributes = [
								new (name: "System.Diagnostics.CodeAnalysis.NotNullWhenAttribute", arguments: ["true"])
							],
						},
					]
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
