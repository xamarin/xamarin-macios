using System.Collections;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Macios.Generator.DataModel;
using Xunit;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class ReferenceKindTests {

	[Theory]
	[InlineData (RefKind.Ref, ReferenceKind.Ref)]
	[InlineData (RefKind.Out, ReferenceKind.Out)]
	[InlineData (RefKind.In, ReferenceKind.In)]
	[InlineData (RefKind.RefReadOnlyParameter, ReferenceKind.RefReadOnlyParameter)]
	[InlineData ((RefKind) 100, ReferenceKind.None)]
	void ToReferenceKind (RefKind refKind, ReferenceKind expected)
		=> Assert.Equal (expected, refKind.ToReferenceKind ());


	class TestDataToTokens : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			yield return [ReferenceKind.Ref, new SyntaxTokenList (Token (SyntaxKind.RefKeyword))];
			yield return [ReferenceKind.Out, new SyntaxTokenList (Token (SyntaxKind.OutKeyword))];
			yield return [ReferenceKind.In, new SyntaxTokenList (Token (SyntaxKind.InKeyword))];
			yield return [
				ReferenceKind.RefReadOnlyParameter,
				new SyntaxTokenList (Token (SyntaxKind.RefKeyword), Token (SyntaxKind.ReadOnlyKeyword))
			];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[ClassData (typeof (TestDataToTokens))]
	void ToTokens (ReferenceKind referenceKind, SyntaxTokenList expected)
	{
		var comparer = new CollectionComparer<SyntaxToken> ();
		Assert.Equal (expected, referenceKind.ToTokens (), comparer);
	}

}
