using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Macios.Generator.DataModel;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class PropertyAccessorTests {
	[Fact]
	public void CompareDiffKind ()
	{
		var x = new PropertyAccessor (AccessorKind.Getter, [], []);
		var y = new PropertyAccessor (AccessorKind.Setter, [], []);
		Assert.False (x.Equals (y));
		Assert.False (y.Equals (x));
		Assert.False (x == y);
		Assert.True (x != y);
	}

	[Fact]
	public void CompareSameKindDiffAttrCount ()
	{
		var x = new PropertyAccessor (AccessorKind.Getter, [
			new ("First"),
			new ("Second"),
		], []);
		var y = new PropertyAccessor (AccessorKind.Getter, [
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
		var x = new PropertyAccessor (AccessorKind.Getter, [
			new ("First"),
			new ("Second"),
		], []);
		var y = new PropertyAccessor (AccessorKind.Getter, [
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
		var x = new PropertyAccessor (AccessorKind.Getter, [
			new ("First"),
			new ("Second"),
		], []);
		var y = new PropertyAccessor (AccessorKind.Getter, [
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
		var x = new PropertyAccessor (AccessorKind.Getter, [
			new ("First"),
			new ("Second"),
		], [
			SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			SyntaxFactory.Token (SyntaxKind.PrivateKeyword)
		]);
		var y = new PropertyAccessor (AccessorKind.Getter, [
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
		var x = new PropertyAccessor (AccessorKind.Getter, [
			new ("First"),
			new ("Second"),
		], [
			SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			SyntaxFactory.Token (SyntaxKind.PrivateKeyword)
		]);
		var y = new PropertyAccessor (AccessorKind.Getter, [
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
		var x = new PropertyAccessor (AccessorKind.Getter, [
			new ("First"),
			new ("Second"),
		], [
			SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			SyntaxFactory.Token (SyntaxKind.PrivateKeyword)
		]);
		var y = new PropertyAccessor (AccessorKind.Getter, [
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
