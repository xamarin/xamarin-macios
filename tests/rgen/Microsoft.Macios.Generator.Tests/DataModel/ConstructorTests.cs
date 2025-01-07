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

public class ConstructorTests : BaseGeneratorTestClass {
	class TestDataFromConstructorDeclaration : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string emptyConstructor = @"
using System;

namespace NS {
	public class TestClass {
		string name;	
		public TestClass () {
			name = ""Test"";
		}
	}
}
";

			yield return [
				emptyConstructor,
				new Constructor (
					type: "TestClass",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: []
				)
			];

			const string singleParameter = @"
using System;

namespace NS {
	public class TestClass {
		string name;	
		public TestClass (string inName) {
			name = inName;
		}
	}
}
";
			yield return [
				singleParameter,
				new Constructor (
					type: "TestClass",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: "string", name: "inName", isBlittable: false),
					]
				)
			];

			const string multiParameter = @"
using System;

namespace NS {
	public class TestClass {
		string name;	
		int age;
		public TestClass (string inName, int inAge) {
			name = inName;
			age = inAge;
		}
	}
}
";

			yield return [
				multiParameter,
				new Constructor (
					type: "TestClass",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: "string", name: "inName", isBlittable: false),
						new (position: 1, type: "int", name: "inAge", isBlittable: true),
					]
				)
			];

			const string nullableParameter = @"
using System;

namespace NS {
	public class TestClass {
		string name;	
		int age;
		public TestClass (string? inName, int inAge) {
			name = inName ?? string.Empty;
			age = inAge;
		}
	}
}
";

			yield return [
				nullableParameter,
				new Constructor (
					type: "TestClass",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: "string", name: "inName", isBlittable: false) {
							IsNullable = true,
						},
						new (position: 1, type: "int", name: "inAge", isBlittable: true),
					]
				)
			];

			const string paramsCollectionParameter = @"
using System;

namespace NS {
	public class TestClass {
		string name;	
		int age;
		string [] surnames;

		public TestClass (string? inName, int inAge, params string[] inSurnames) {
			name = inName ?? string.Empty;
			age = inAge;
			surnames = inSurnames;	
		}
	}
}
";

			yield return [
				paramsCollectionParameter,
				new Constructor (
					type: "TestClass",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: "string", name: "inName", isBlittable: false) {
							IsNullable = true,
						},
						new (position: 1, type: "int", name: "inAge", isBlittable: true),
						new (position: 2, type: "string", name: "inSurnames", isBlittable: false) {
							IsParams = true,
							IsArray = true,
						},
					]
				)
			];

			const string arrayParameter = @"
using System;

namespace NS {
	public class TestClass {
		string name;	
		int age;
		string [] surnames;

		public TestClass (string? inName, int inAge, string[] inSurnames) {
			name = inName ?? string.Empty;
			age = inAge;
			surnames = inSurnames;	
		}
	}
}
";

			yield return [
				arrayParameter,
				new Constructor (
					type: "TestClass",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: "string", name: "inName", isBlittable: false) {
							IsNullable = true,
						},
						new (position: 1, type: "int", name: "inAge", isBlittable: true),
						new (position: 2, type: "string", name: "inSurnames", isBlittable: false) {
							IsParams = false,
							IsArray = true
						},
					]
				)
			];

			const string nullableArrayParameter = @"
using System;

namespace NS {
	public class TestClass {
		string name;	
		int age;
		string [] surnames;

		public TestClass (string? inName, int inAge, string[]? inSurnames) {
			name = inName ?? string.Empty;
			age = inAge;
			surnames = inSurnames;	
		}
	}
}
";

			yield return [
				nullableArrayParameter,
				new Constructor (
					type: "TestClass",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: "string", name: "inName", isBlittable: false) {
							IsNullable = true,
						},
						new (position: 1, type: "int", name: "inAge", isBlittable: true),
						new (position: 2, type: "string", name: "inSurnames", isBlittable: false) {
							IsNullable = true,
							IsArray = true
						},
					]
				)
			];

			const string arrayOfNullableParameter = @"
using System;

namespace NS {
	public class TestClass {
		string name;	
		int age;
		string [] surnames;

		public TestClass (string? inName, int inAge, string?[] inSurnames) {
			name = inName ?? string.Empty;
			age = inAge;
			surnames = inSurnames;	
		}
	}
}
";

			yield return [
				arrayOfNullableParameter,
				new Constructor (
					type: "TestClass",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: "string", name: "inName", isBlittable: false) {
							IsNullable = true,
						},
						new (position: 1, type: "int", name: "inAge", isBlittable: true),
						new (position: 2, type: "string?", name: "inSurnames", isBlittable: false) {
							IsNullable = false,
							IsArray = true
						},
					]
				)
			];

			const string nullableArrayOfNullableParameter = @"
using System;

namespace NS {
	public class TestClass {
		string name;	
		int age;
		string [] surnames;

		public TestClass (string? inName, int inAge, string?[]? inSurnames) {
			name = inName ?? string.Empty;
			age = inAge;
			surnames = inSurnames;	
		}
	}
}
";

			yield return [
				nullableArrayOfNullableParameter,
				new Constructor (
					type: "TestClass",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (0, "string", "inName", false) {
							IsNullable = true,
						},
						new (1, "int", "inAge", true),
						new (2, "string?", "inSurnames", false) {
							IsNullable = true,
							IsArray = true
						},
					]
				)
			];

			const string twoDimensionalArrayParameter = @"
using System;

namespace NS {
	public class TestClass {
		string name;	
		int age;
		string [] surnames;

		public TestClass (string? inName, int inAge, string[][] inSurnames) {
			name = inName ?? string.Empty;
			age = inAge;
			surnames = inSurnames;	
		}
	}
}
";

			yield return [
				twoDimensionalArrayParameter,
				new Constructor (
					type: "TestClass",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: "string", name: "inName", isBlittable: false) {
							IsNullable = true,
						},
						new (position: 1, type: "int", name: "inAge", isBlittable: true),
						new (position: 2, type: "string[]", name: "inSurnames", isBlittable: false) {
							IsParams = false,
							IsArray = true
						},
					]
				)
			];

			const string optionalParameter = @"
using System;

namespace NS {
	public class TestClass {
		string name;	
		public TestClass (string? inName = null) {
			name = inName ?? string.Empty;
		}
	}
}
";

			yield return [
				optionalParameter,
				new Constructor (
					type: "TestClass",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: "string", name: "inName", isBlittable: false) {
							IsNullable = true,
							IsOptional = true,
						},
					]
				)
			];

			const string genericParameter = @"
using System;

namespace NS {
	public class TestClass<T> {
		T name;	
		public TestClass (T? inName = null) {
			name = T; 
		}
	}
}
";

			yield return [
				genericParameter,
				new Constructor (
					type: "TestClass",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: "T", name: "inName", isBlittable: false) {
							IsOptional = true,
							IsNullable = true,
						},
					]
				)
			];

			const string availabilityPresent = @"
using System.Runtime.Versioning;
using System;

namespace NS {
	[SupportedOSPlatform (""ios"")]
	[SupportedOSPlatform (""tvos"")]
	public class TestClass {
		string name;	
		public TestClass (string? inName = null) {
			name = inName ?? string.Empty;
		}
	}
}
";
			var builder = SymbolAvailability.CreateBuilder ();
			builder.Add (new SupportedOSPlatformData ("ios"));
			builder.Add (new SupportedOSPlatformData ("tvos"));

			yield return [
				availabilityPresent,
				new Constructor (
					type: "TestClass",
					symbolAvailability: builder.ToImmutable (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: "string", name: "inName", isBlittable: false) {
							IsNullable = true,
							IsOptional = true,
						},
					]
				)
			];

		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataFromConstructorDeclaration>]
	void FromConstructorDeclaration (ApplePlatform platform, string inputText, Constructor expected)
	{
		var (compilation, syntaxTrees) = CreateCompilation (platform, sources: inputText);
		Assert.Single (syntaxTrees);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ().OfType<ConstructorDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		Assert.True (Constructor.TryCreate (declaration, semanticModel, out var changes));
		Assert.NotNull (changes);
		Assert.Equal (expected, changes);
	}
}
