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
					type: "NS.TestClass",
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
					type: "NS.TestClass",
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (0, "string", "inName"),
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
					type: "NS.TestClass",
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (0, "string", "inName"),
						new (1, "int", "inAge"),
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
					type: "NS.TestClass",
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (0, "string?", "inName") { IsNullable = true, },
						new (1, "int", "inAge"),
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
					type: "NS.TestClass",
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (0, "string?", "inName") { IsNullable = true, },
						new (1, "int", "inAge"),
						new (2, "string[]", "inSurnames") { IsParams = true, },
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
					type: "NS.TestClass",
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (0, "string?", "inName") { IsNullable = true, IsOptional = true, },
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
					type: "NS.TestClass<T>",
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (0, "T?", "inName") { IsOptional = true, IsNullable = true, },
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
		var (compilation, syntaxTrees) = CreateCompilation (nameof (FromConstructorDeclaration),
			platform, inputText);
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
