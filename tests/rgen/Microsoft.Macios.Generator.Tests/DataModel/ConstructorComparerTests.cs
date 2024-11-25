using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Macios.Generator.DataModel;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class ConstructorComparerTests {
	readonly ConstructorComparer comparer = new ();

	[Fact]
	public void CompareDiffType ()
	{
		var x = new Constructor ("MyClass", [], [], []);
		var y = new Constructor ("MyClass2", [], [], []);
		Assert.Equal (String.Compare (x.Type, y.Type, StringComparison.Ordinal), comparer.Compare (x, y));
	}

	[Fact]
	public void CompareModifierDiffLength ()
	{
		var x = new Constructor ("MyClass",
			attributes: [],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
				SyntaxFactory.Token (SyntaxKind.PartialKeyword),
			],
			parameters: []);
		var y = new Constructor ("MyClass",
			attributes: [],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			],
			parameters: []);
		Assert.Equal (x.Modifiers.Length.CompareTo (y.Modifiers.Length), comparer.Compare (x, y));
	}

	[Fact]
	public void CompareDiffModifier ()
	{
		var x = new Constructor ("MyClass",
			attributes: [],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PartialKeyword),
			],
			parameters: []);
		var y = new Constructor ("MyClass",
			attributes: [],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			],
			parameters: []);
		Assert.Equal (String.Compare (x.Modifiers [0].Text, y.Modifiers [0].Text, StringComparison.Ordinal),
			comparer.Compare (x, y));
	}

	[Fact]
	public void CompareAttrsDiffLength ()
	{
		var x = new Constructor ("MyClass",
			attributes: [
				new ("FirstAttr"),
				new ("SecondAttr", ["first"]),
			],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PartialKeyword),
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			],
			parameters: []);
		var y = new Constructor ("MyClass",
			attributes: [
				new ("FirstAttr"),
			],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PartialKeyword),
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			],
			parameters: []);
		Assert.Equal (x.Attributes.Length.CompareTo (y.Attributes.Length), comparer.Compare (x, y));
	}

	[Fact]
	public void CompareDiffAttrs ()
	{
		var x = new Constructor ("MyClass",
			attributes: [
				new ("FirstAttr"),
			],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PartialKeyword),
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			],
			parameters: []);
		var y = new Constructor ("MyClass",
			attributes: [
				new ("SecondAttr", ["first"]),
			],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PartialKeyword),
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			],
			parameters: []);
		var attrCompare = new AttributeComparer ();
		Assert.Equal (attrCompare.Compare (x.Attributes [0], y.Attributes [0]), comparer.Compare (x, y));
	}

	[Fact]
	public void CompareParameterDiffLength ()
	{

		var x = new Constructor ("MyClass",
			attributes: [
				new ("FirstAttr"),
			],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PartialKeyword),
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			],
			parameters: [
				new (0, "string", "name"),
			]);
		var y = new Constructor ("MyClass",
			attributes: [
				new ("FirstAttr"),
			],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PartialKeyword),
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			],
			parameters: [
				new (0, "string", "name"),
				new (1, "string", "surname"),
			]);
		Assert.Equal (x.Parameters.Length.CompareTo (y.Parameters.Length), comparer.Compare (x, y));
	}

	[Fact]
	public void CompareDiffParameters ()
	{
		var x = new Constructor ("MyClass",
			attributes: [
				new ("FirstAttr"),
			],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PartialKeyword),
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			],
			parameters: [
				new (0, "string", "name"),
			]);
		var y = new Constructor ("MyClass",
			attributes: [
				new ("FirstAttr"),
			],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PartialKeyword),
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			],
			parameters: [
				new (1, "string", "surname"),
			]);
		var parameterCompare = new ParameterComparer ();
		Assert.Equal (parameterCompare.Compare (x.Parameters [0], y.Parameters [0]), comparer.Compare (x, y));
	}
}
