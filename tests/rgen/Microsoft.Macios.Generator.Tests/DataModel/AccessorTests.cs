using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.DataModel;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class AccessorTests {
	[Fact]
	public void CompareDiffKind ()
	{
		var x = new Accessor (AccessorKind.Getter, new (), [], []);
		var y = new Accessor (AccessorKind.Setter, new (), [], []);
		Assert.False (x.Equals (y));
		Assert.False (y.Equals (x));
		Assert.False (x == y);
		Assert.True (x != y);
	}

	[Fact]
	public void CompareSameKindDiffAttrCount ()
	{
		var x = new Accessor (AccessorKind.Getter, new (), [
			new ("First"),
			new ("Second"),
		], []);
		var y = new Accessor (AccessorKind.Getter, new (), [
			new ("First"),
		], []);
		Assert.False (x.Equals (y));
		Assert.False (y.Equals (x));
		Assert.False (x == y);
		Assert.True (x != y);
	}

	[Fact]
	public void CompareSameKindDiffAttr ()
	{
		var x = new Accessor (AccessorKind.Getter, new (), [
			new ("First"),
			new ("Second"),
		], []);
		var y = new Accessor (AccessorKind.Getter, new (), [
			new ("Third"),
			new ("Fourth"),
		], []);
		Assert.False (x.Equals (y));
		Assert.False (y.Equals (x));
		Assert.False (x == y);
		Assert.True (x != y);
	}

	[Fact]
	public void CompareSameKindDiffAttrOrder ()
	{
		var x = new Accessor (AccessorKind.Getter, new (), [
			new ("First"),
			new ("Second"),
		], []);
		var y = new Accessor (AccessorKind.Getter, new (), [
			new ("Second"),
			new ("First"),
		], []);
		Assert.True (x.Equals (y));
		Assert.True (y.Equals (x));
		Assert.True (x == y);
		Assert.False (x != y);
	}

	[Fact]
	public void CompareSameKindSameAttrDiffModifiersCount ()
	{
		var x = new Accessor (AccessorKind.Getter, new (), [
			new ("First"),
			new ("Second"),
		], [
			SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			SyntaxFactory.Token (SyntaxKind.PrivateKeyword)
		]);
		var y = new Accessor (AccessorKind.Getter, new (), [
			new ("Second"),
			new ("First"),
		], [
			SyntaxFactory.Token (SyntaxKind.PrivateKeyword)
		]);

		Assert.False (x.Equals (y));
		Assert.False (y.Equals (x));
		Assert.False (x == y);
		Assert.True (x != y);
	}

	[Fact]
	public void CompareSameKindSameAttrDiffModifiers ()
	{
		var x = new Accessor (AccessorKind.Getter, new (), [
			new ("First"),
			new ("Second"),
		], [
			SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			SyntaxFactory.Token (SyntaxKind.PrivateKeyword)
		]);
		var y = new Accessor (AccessorKind.Getter, new (), [
			new ("Second"),
			new ("First"),
		], [
			SyntaxFactory.Token (SyntaxKind.PrivateKeyword),
			SyntaxFactory.Token (SyntaxKind.ProtectedKeyword)
		]);

		Assert.False (x.Equals (y));
		Assert.False (y.Equals (x));
		Assert.False (x == y);
		Assert.True (x != y);
	}

	[Fact]
	public void CompareSameKindSameAttrDiffModifiersOrder ()
	{
		var x = new Accessor (AccessorKind.Getter, new (), [
			new ("First"),
			new ("Second"),
		], [
			SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			SyntaxFactory.Token (SyntaxKind.PrivateKeyword)
		]);
		var y = new Accessor (AccessorKind.Getter, new (), [
			new ("Second"),
			new ("First"),
		], [
			SyntaxFactory.Token (SyntaxKind.PrivateKeyword),
			SyntaxFactory.Token (SyntaxKind.PublicKeyword),
		]);

		Assert.True (x.Equals (y));
		Assert.True (y.Equals (x));
		Assert.True (x == y);
		Assert.False (x != y);
	}
	
	[Fact]
	public void CompareSameKindSameAttrDiffAvailability ()
	{
		var builder = SymbolAvailability.CreateBuilder ();
		builder.Add (new SupportedOSPlatformData ("ios17.0"));
		var x = new Accessor (AccessorKind.Getter, builder.ToImmutable (), [
			new ("First"),
			new ("Second"),
		], [
			SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			SyntaxFactory.Token (SyntaxKind.PrivateKeyword)
		]);
		builder.Clear ();
		builder.Add (new SupportedOSPlatformData ("tvos17.0"));
		var y = new Accessor (AccessorKind.Getter, builder.ToImmutable (), [
			new ("Second"),
			new ("First"),
		], [
			SyntaxFactory.Token (SyntaxKind.PrivateKeyword),
			SyntaxFactory.Token (SyntaxKind.PublicKeyword),
		]);

		Assert.False (x.Equals (y));
		Assert.False (y.Equals (x));
		Assert.False (x == y);
		Assert.True (x != y);
	}
	
	[Fact]
	public void CompareSameKindSameAttrSameAvailability ()
	{
		var builder = SymbolAvailability.CreateBuilder ();
		builder.Add (new SupportedOSPlatformData ("ios17.0"));
		var x = new Accessor (AccessorKind.Getter, builder.ToImmutable (), [
			new ("First"),
			new ("Second"),
		], [
			SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			SyntaxFactory.Token (SyntaxKind.PrivateKeyword)
		]);
		var y = new Accessor (AccessorKind.Getter, builder.ToImmutable (), [
			new ("Second"),
			new ("First"),
		], [
			SyntaxFactory.Token (SyntaxKind.PrivateKeyword),
			SyntaxFactory.Token (SyntaxKind.PublicKeyword),
		]);

		Assert.True (x.Equals (y));
		Assert.True (y.Equals (x));
		Assert.True (x == y);
		Assert.False (x != y);
	}
}
