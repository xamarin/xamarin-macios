// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Macios.Generator.DataModel;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class ModifiersEqualityComparerTests {
	readonly ModifiersEqualityComparer equalityComparer = new ModifiersEqualityComparer ();

	[Fact]
	public void CompareEmpty ()
	{
		var x = ImmutableArray<SyntaxToken>.Empty;
		var y = ImmutableArray<SyntaxToken>.Empty;
		Assert.True (equalityComparer.Equals (x, y));
	}

	[Fact]
	public void CompareDifferentSize ()
	{
		ImmutableArray<SyntaxToken> x = [
			SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			SyntaxFactory.Token (SyntaxKind.PrivateKeyword),
		];
		ImmutableArray<SyntaxToken> y = [
			SyntaxFactory.Token (SyntaxKind.PublicKeyword),
		];
		Assert.False (equalityComparer.Equals (x, y));
	}

	[Fact]
	public void CompareDifferentModifiers ()
	{
		ImmutableArray<SyntaxToken> x = [
			SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			SyntaxFactory.Token (SyntaxKind.PrivateKeyword),
		];
		ImmutableArray<SyntaxToken> y = [
			SyntaxFactory.Token (SyntaxKind.ProtectedKeyword),
			SyntaxFactory.Token (SyntaxKind.PublicKeyword),
		];
		Assert.False (equalityComparer.Equals (x, y));
	}

	[Fact]
	public void CompareSameModifiersDiffOrder ()
	{
		ImmutableArray<SyntaxToken> x = [
			SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			SyntaxFactory.Token (SyntaxKind.PrivateKeyword),
		];
		ImmutableArray<SyntaxToken> y = [
			SyntaxFactory.Token (SyntaxKind.PrivateKeyword),
			SyntaxFactory.Token (SyntaxKind.PublicKeyword),
		];

		Assert.True (equalityComparer.Equals (x, y));
	}

	[Fact]
	public void CompareSameModifiers ()
	{
		ImmutableArray<SyntaxToken> x = [
			SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			SyntaxFactory.Token (SyntaxKind.PrivateKeyword),
		];
		ImmutableArray<SyntaxToken> y = [
			SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			SyntaxFactory.Token (SyntaxKind.PrivateKeyword),
		];

		Assert.True (equalityComparer.Equals (x, y));
	}
}
