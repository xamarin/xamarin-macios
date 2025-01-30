// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Extensions;
using Microsoft.Macios.Transformer.Attributes;
using Xamarin.Tests;
using Xamarin.Utils;

namespace Microsoft.Macios.Transformer.Tests.Attributes;

public class AsyncDataTests : BaseTransformerTestClass {

	class TestDataTryCreate : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string path = "/some/random/path.cs";

			const string simpleAsyncMethod = @"
using System;
using AppKit;
using Foundation;
using ObjCRuntime;

namespace Test;

[NoMacCatalyst]
[BaseType (typeof (NSObject))]
[DisableDefaultCtor]
interface NSTableViewDiffableDataSource {

	[Export (""applySnapshot:animatingDifferences:completion:"")]
	[Async]
	void ApplySnapshot (NSObject snapshot, bool animatingDifferences, [NullAllowed] Action completion);
}
";
			yield return [(Source: simpleAsyncMethod, Path: path), new AsyncData ()];

			const string asyncResultTypeName = @"
using System;
using AppKit;
using Foundation;
using ObjCRuntime;

namespace Test;

[NoMacCatalyst]
[BaseType (typeof (NSObject))]
[DisableDefaultCtor]
interface NSTableViewDiffableDataSource {

	[Export (""applySnapshot:animatingDifferences:completion:"")]
	[Async (ResultTypeName=""NSSpellCheckerCandidates"")]
	void ApplySnapshot (NSObject snapshot, bool animatingDifferences, [NullAllowed] Action completion);
}
";

			yield return [(Source: asyncResultTypeName, Path: path),
				new AsyncData {
					ResultTypeName = "NSSpellCheckerCandidates"
				}];

			const string asyncMethodName = @"
using System;
using AppKit;
using Foundation;
using ObjCRuntime;

namespace Test;

[NoMacCatalyst]
[BaseType (typeof (NSObject))]
[DisableDefaultCtor]
interface NSTableViewDiffableDataSource {

	[Export (""applySnapshot:animatingDifferences:completion:"")]
	[Async (""ApplyTheSnapshotAsync"")]
	void ApplySnapshot (NSObject snapshot, bool animatingDifferences, [NullAllowed] Action completion);
}
";

			yield return [(Source: asyncMethodName, Path: path),
				new AsyncData {
					MethodName = "ApplyTheSnapshotAsync"
				}];

			const string asyncTypeOf = @"
using System;
using AppKit;
using Foundation;
using ObjCRuntime;

namespace Test;

public class SampleResult {}

[NoMacCatalyst]
[BaseType (typeof (NSObject))]
[DisableDefaultCtor]
interface NSTableViewDiffableDataSource {

	[Export (""applySnapshot:animatingDifferences:completion:"")]
	[Async (ResultType = typeof (SampleResult))]
	void ApplySnapshot (NSObject snapshot, bool animatingDifferences, [NullAllowed] Action completion);
}
";

			yield return [(Source: asyncTypeOf, Path: path),
				new AsyncData {
					ResultType = "Test.SampleResult"
				}];

			const string postResult = @"
using System;
using AppKit;
using Foundation;
using ObjCRuntime;

namespace Test;

public class SampleResult {}

[NoMacCatalyst]
[BaseType (typeof (NSObject))]
[DisableDefaultCtor]
interface NSTableViewDiffableDataSource {

	[Export (""applySnapshot:animatingDifferences:completion:"")]
	[Async (ResultTypeName = ""NSUrlSessionDataTaskRequest"", PostNonResultSnippet = ""result.Resume ();"")]
	void ApplySnapshot (NSObject snapshot, bool animatingDifferences, [NullAllowed] Action completion);
}
";

			yield return [(Source: postResult, Path: path),
				new AsyncData {
					ResultTypeName = "NSUrlSessionDataTaskRequest",
					PostNonResultSnippet = "result.Resume ();"
				}];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataTryCreate>]
	void TryCreateTests (ApplePlatform platform, (string Source, string Path) source, AsyncData expectedData)
	{
		// create a compilation used to create the transformer
		var compilation = CreateCompilation (platform, sources: source);
		var syntaxTree = compilation.SyntaxTrees.ForSource (source);
		Assert.NotNull (syntaxTree);

		var semanticModel = compilation.GetSemanticModel (syntaxTree);
		Assert.NotNull (semanticModel);

		var declaration = syntaxTree.GetRoot ()
			.DescendantNodes ().OfType<MethodDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);

		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);
		var attribute = symbol.GetAttribute<AsyncData> (AttributesNames.AsyncAttribute, AsyncData.TryParse);
		Assert.Equal (expectedData, attribute);
	}
}
