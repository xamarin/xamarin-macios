using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Macios.Generator.DataModel;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class MethodComparerTests {
	readonly MethodComparer comparer = new ();

	[Fact]
	public void CompareDiffType ()
	{
		var x = new Method (
			type: "MyType",
			name: "MyMethod",
			returnType: "void",
			symbolAvailability: new (),
			attributes: [],
			modifiers: [],
			parameters: []
		);

		var y = new Method (
			type: "MyOtherType",
			name: "MyMethod",
			returnType: "void",
			symbolAvailability: new (),
			attributes: [],
			modifiers: [],
			parameters: []
		);

		Assert.Equal (String.Compare (x.Type, y.Type, StringComparison.Ordinal), comparer.Compare (x, y));
	}

	[Fact]
	public void CompareDiffName ()
	{
		var x = new Method (
			type: "MyType",
			name: "MyMethod",
			returnType: "void",
			symbolAvailability: new (),
			attributes: [],
			modifiers: [],
			parameters: []
		);

		var y = new Method (
			type: "MyType",
			name: "MyOtherMethod",
			returnType: "void",
			symbolAvailability: new (),
			attributes: [],
			modifiers: [],
			parameters: []
		);
		Assert.Equal (String.Compare (x.Name, y.Name, StringComparison.Ordinal), comparer.Compare (x, y));
	}

	[Fact]
	public void CompareDiffReturnType ()
	{
		var x = new Method (
			type: "MyType",
			name: "MyMethod",
			returnType: "void",
			symbolAvailability: new (),
			attributes: [],
			modifiers: [],
			parameters: []
		);

		var y = new Method (
			type: "MyType",
			name: "MyMethod",
			returnType: "int",
			symbolAvailability: new (),
			attributes: [],
			modifiers: [],
			parameters: []
		);
		Assert.Equal (String.Compare (x.ReturnType, y.ReturnType, StringComparison.Ordinal), comparer.Compare (x, y));
	}

	[Fact]
	public void CompareModifierDiffLength ()
	{
		var x = new Method (
			type: "MyType",
			name: "MyMethod",
			returnType: "void",
			symbolAvailability: new (),
			attributes: [],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
				SyntaxFactory.Token (SyntaxKind.PartialKeyword),
			],
			parameters: []
		);

		var y = new Method (
			type: "MyType",
			name: "MyMethod",
			returnType: "void",
			symbolAvailability: new (),
			attributes: [],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			],
			parameters: []
		);
		Assert.Equal (x.Modifiers.Length.CompareTo (y.Modifiers.Length), comparer.Compare (x, y));
	}

	[Fact]
	public void CompareDiffModifier ()
	{
		var x = new Method (
			type: "MyType",
			name: "MyMethod",
			returnType: "void",
			symbolAvailability: new (),
			attributes: [],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PartialKeyword),
			],
			parameters: []
		);

		var y = new Method (
			type: "MyType",
			name: "MyMethod",
			returnType: "void",
			symbolAvailability: new (),
			attributes: [],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			],
			parameters: []
		);
		Assert.Equal (String.Compare (x.Modifiers [0].Text, y.Modifiers [0].Text, StringComparison.Ordinal),
			comparer.Compare (x, y));
	}


	[Fact]
	public void CompareAttrsDiffLength ()
	{
		var x = new Method (
			type: "MyType",
			name: "MyMethod",
			returnType: "void",
			symbolAvailability: new (),
			attributes: [
				new ("FirstAttr"),
				new ("SecondAttr", ["first"]),
			],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			],
			parameters: []
		);

		var y = new Method (
			type: "MyType",
			name: "MyMethod",
			returnType: "void",
			symbolAvailability: new (),
			attributes: [
				new ("FirstAttr"),
			],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			],
			parameters: []
		);
		Assert.Equal (x.Attributes.Length.CompareTo (y.Attributes.Length), comparer.Compare (x, y));
	}

	[Fact]
	public void CompareDiffAttrs ()
	{
		var x = new Method (
			type: "MyType",
			name: "MyMethod",
			returnType: "void",
			symbolAvailability: new (),
			attributes: [
				new ("FirstAttr"),
			],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			],
			parameters: []
		);

		var y = new Method (
			type: "MyType",
			name: "MyMethod",
			returnType: "void",
			symbolAvailability: new (),
			attributes: [
				new ("SecondAttr", ["first"]),
			],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			],
			parameters: []
		);
		var attrCompare = new AttributeComparer ();
		Assert.Equal (attrCompare.Compare (x.Attributes [0], y.Attributes [0]), comparer.Compare (x, y));
	}

	[Fact]
	public void CompareParameterDiffLength ()
	{
		var x = new Method (
			type: "MyType",
			name: "MyMethod",
			returnType: "void",
			symbolAvailability: new (),
			attributes: [
				new ("FirstAttr"),
			],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			],
			parameters: [
				new (0, "string", "name"),
			]
		);

		var y = new Method (
			type: "MyType",
			name: "MyMethod",
			returnType: "void",
			symbolAvailability: new (),
			attributes: [
				new ("FirstAttr"),
			],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			],
			parameters: [
				new (0, "string", "name"),
				new (1, "string", "surname"),
			]
		);
		Assert.Equal (x.Parameters.Length.CompareTo (y.Parameters.Length), comparer.Compare (x, y));
	}

	[Fact]
	public void CompareDiffParameters ()
	{
		var x = new Method (
			type: "MyType",
			name: "MyMethod",
			returnType: "void",
			symbolAvailability: new (),
			attributes: [
				new ("FirstAttr"),
			],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			],
			parameters: [
				new (0, "string", "name"),
			]
		);

		var y = new Method (
			type: "MyType",
			name: "MyMethod",
			returnType: "void",
			symbolAvailability: new (),
			attributes: [
				new ("FirstAttr"),
			],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			],
			parameters: [
				new (1, "string", "surname"),
			]
		);
		var parameterCompare = new ParameterComparer ();
		Assert.Equal (parameterCompare.Compare (x.Parameters [0], y.Parameters [0]), comparer.Compare (x, y));
	}
	
	[Fact]
	public void CompareDiffParametersSmartEnum ()
	{
		var x = new Method (
			type: "MyType",
			name: "MyMethod",
			returnType: "void",
			symbolAvailability: new (),
			attributes: [
				new ("FirstAttr"),
			],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			],
			parameters: [
				new (0, "MyEnum", "name") {
					IsSmartEnum = true
				},
			]
		);

		var y = new Method (
			type: "MyType",
			name: "MyMethod",
			returnType: "void",
			symbolAvailability: new (),
			attributes: [
				new ("FirstAttr"),
			],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			],
			parameters: [
				new (0, "MyEnum", "name") {
					IsSmartEnum = false 
				},
			]
		);
		var parameterCompare = new ParameterComparer ();
		Assert.Equal (parameterCompare.Compare (x.Parameters [0], y.Parameters [0]), comparer.Compare (x, y));
	}
}
