// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Extensions;
using Microsoft.Macios.Transformer.Attributes;
using Xamarin.Tests;
using Xamarin.Utils;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Microsoft.Macios.Transformer.Tests.Attributes;

public class BindAsDataTests : BaseTransformerTestClass {
	
	class TestDataTryCreate : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			var path = "/some/random/path.cs";

			const string bindAsProperty = @"
using System;
using Foundation;
using ObjCRuntime;
using CoreMedia;

namespace Test;

[NoTV]
[MacCatalyst (13, 1)]
[DisableDefaultCtor]
[Abstract]
[BaseType (typeof (NSObject))]
interface UIFeedbackGenerator {
	[BindAs (typeof (CMVideoDimensions []))]
	[Export (""supportedMaxPhotoDimensions"")]
	NSValue [] SupportedMaxPhotoDimensions { get; }
}
";
			
			yield return [
				PropertyDeclaration (IdentifierName ("string"), Identifier ("Hello")), 
				(Source: bindAsProperty, Path: path), 
				new BindAsData("CoreMedia.CMVideoDimensions[]")];
				
			
			const string bindAsReturnMethod = @"
using System;
using Foundation;
using ObjCRuntime;
using CoreMedia;

namespace Test;

[NoTV]
[MacCatalyst (13, 1)]
[DisableDefaultCtor]
[Abstract]
[BaseType (typeof (NSObject))]
interface UIFeedbackGenerator {
	[return: BindAs (typeof (NSLinguisticTag []))]
	[Export (""linguisticTagsInRange:scheme:options:orthography:tokenRanges:"")]
	NSString [] GetLinguisticTags (NSRange range, NSString scheme, NSLinguisticTaggerOptions options, [NullAllowed] NSOrthography orthography, [NullAllowed] out NSValue [] tokenRanges);
}
";
			
			yield return [
				MethodDeclaration (PredefinedType(Token(SyntaxKind.StringKeyword)), Identifier ("Hello")),
				(Source: bindAsReturnMethod, Path: path), 
				new BindAsData("Foundation.NSLinguisticTag[]")];


			const string parameterBindAs = @"
using System;
using Foundation;
using ObjCRuntime;
using CoreMedia;

namespace Test;

[NoTV]
[MacCatalyst (13, 1)]
[DisableDefaultCtor]
[Abstract]
[BaseType (typeof (NSObject))]
interface UIFeedbackGenerator {
	[Export (""linguisticTagsInRange:scheme:options:orthography:tokenRanges:"")]
	NSString [] GetLinguisticTags ([BindAs (typeof (ushort?))] NSString scheme);
}
";
			var parameter = Parameter (Identifier ("variable"))
					.WithType (PredefinedType (Token (SyntaxKind.StringKeyword)));
			yield return [
				parameter,
				(Source: parameterBindAs, Path: path), 
				new BindAsData("ushort?")];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataTryCreate>]
	void TryCreateTests<T> (ApplePlatform platform, T _, (string Source, string Path) source, BindAsData expectedData)
		where T : CSharpSyntaxNode 
	{
		// create a compilation used to create the transformer
		var compilation = CreateCompilation (platform, sources: source);
		var syntaxTree = compilation.SyntaxTrees.ForSource (source);
		var trees = compilation.SyntaxTrees.Where (s => s.FilePath == source.Path).ToArray ();
		Assert.Single (trees);
		Assert.NotNull (syntaxTree);

		var semanticModel = compilation.GetSemanticModel (syntaxTree);
		Assert.NotNull (semanticModel);

		var declaration = syntaxTree.GetRoot ()
			.DescendantNodes ().OfType<T> ()
			.LastOrDefault ();

		Assert.NotNull (declaration);

		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);
		var attribute = symbol.GetAttribute<BindAsData> (AttributesNames.BindAsAttribute, BindAsData.TryParse);
		Assert.NotNull (attribute);
		Assert.Equal (expectedData, attribute.Value);
	}
}
