using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.DataModel;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class MethodTests : BaseGeneratorTestClass {
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
					returnType: "void",
					symbolAvailability: new (),
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
		public string MyMethod () => string.Empty;
	}
}
";

			yield return [
				stringMethodNoParams,
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: "string",
					symbolAvailability: new (),
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
					returnType: "NS.CustomType",
					symbolAvailability: new (),
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
					returnType: "string",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (0, "string", "input")
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
					returnType: "string",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (0, "string", "input") {
							IsArray = true
						}
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
					returnType: "string",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (0, "string", "input") {
							IsArray = true,
							IsNullable = true,
						}
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
					returnType: "string",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (0, "string?", "input") {
							IsArray = true,
							IsNullable = false,
						}
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
					returnType: "string",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (0, "string?", "input") {
							IsArray = true,
							IsNullable = true,
						}
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
					returnType: "string",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (0, "string[]", "input") {
							IsArray = true
						}
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
					returnType: "void",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (0, "NS.CustomType", "input")
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
					returnType: "string",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (0, "string", "input"),
						new (1, "string", "second") {
							IsNullable = true,
						}
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
					returnType: "bool",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (0, "string", "example") {
							IsNullable = true,
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
					returnType: "void",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (0, "NS.MyStruct", "data") {
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
					returnType: "void",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (0, "NS.MyStruct", "data") {
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
					returnType: "string",
					symbolAvailability: builder.ToImmutable (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (0, "string", "input"),
						new (1, "string", "second") {
							IsNullable = true,
						}
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
					returnType: "bool",
					symbolAvailability: new (),
					attributes: [
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (0, "string", "example") {
							IsNullable = true,
							ReferenceKind = ReferenceKind.Out,
							Attributes = [
								new ("System.Diagnostics.CodeAnalysis.NotNullWhenAttribute", ["true"])
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
