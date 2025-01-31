// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Extensions;
using Xamarin.Tests;
using Xamarin.Utils;

namespace Microsoft.Macios.Transformer.Tests.Extensions;

public class TypeSymbolExtensionsTests : BaseTransformerTestClass {

	class TestDataIsSmartEnum : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			var path = "/some/random/path.cs";

			const string normalEnum = @"
using System;

namespace Test;

public enum MyEnum {
	First,
	Second,
	Last,	
}
";
			yield return [(Source: normalEnum, Path: path), false];

			const string traditionalSmartEnum = @"
using System;
using Foundation;
using ObjCRuntime;

namespace Test;

public enum MyEnum {
	[Field (""FirstEnum""]
	First,
	[Field (""Second""]
	Second,
	[Field (""Last""]
	Last,	
}
";

			yield return [(Source: traditionalSmartEnum, Path: path), true];

			const string partialSmartEnum = @"
using System;
using Foundation;
using ObjCRuntime;

namespace Test;

public enum MyEnum {
	First,
	Second,
	[Field (""Last""]
	Last,	
}
";

			// should not be very common, nevertheless test for it.
			yield return [(Source: partialSmartEnum, Path: path), true];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataIsSmartEnum>]
	void IsSmartEnumTests (ApplePlatform platform, (string Source, string Path) source, bool expectedResult)
	{
		var compilation = CreateCompilation (platform, sources: source);
		var syntaxTree = compilation.SyntaxTrees.ForSource (source);
		var trees = compilation.SyntaxTrees.Where (s => s.FilePath == source.Path).ToArray ();
		Assert.Single (trees);
		Assert.NotNull (syntaxTree);

		var semanticModel = compilation.GetSemanticModel (syntaxTree);
		Assert.NotNull (semanticModel);

		var declaration = syntaxTree.GetRoot ()
				.DescendantNodes ().OfType<EnumDeclarationSyntax> ()
				.LastOrDefault ();

		Assert.NotNull (declaration);

		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);
		var isSmartEnum = symbol.IsSmartEnum ();
		Assert.Equal (expectedResult, symbol.IsSmartEnum ());
	}
}
