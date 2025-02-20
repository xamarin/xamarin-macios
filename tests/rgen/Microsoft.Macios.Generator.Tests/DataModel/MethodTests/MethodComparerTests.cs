// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Macios.Generator.DataModel;
using Xunit;
using static Microsoft.Macios.Generator.Tests.TestDataFactory;

namespace Microsoft.Macios.Generator.Tests.DataModel.MethodTests;

public class MethodComparerTests {
	readonly MethodComparer comparer = new ();

	[Fact]
	public void CompareDiffType ()
	{
		var x = new Method (
			type: "MyType",
			name: "MyMethod",
			returnType: new ("void"),
			symbolAvailability: new (),
			exportMethodData: new (),
			attributes: [],
			modifiers: [],
			parameters: []
		);

		var y = new Method (
			type: "MyOtherType",
			name: "MyMethod",
			returnType: new ("void"),
			symbolAvailability: new (),
			exportMethodData: new (),
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
			returnType: new ("void"),
			symbolAvailability: new (),
			exportMethodData: new (),
			attributes: [],
			modifiers: [],
			parameters: []
		);

		var y = new Method (
			type: "MyType",
			name: "MyOtherMethod",
			returnType: new ("void"),
			symbolAvailability: new (),
			exportMethodData: new (),
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
			returnType: new ("void"),
			symbolAvailability: new (),
			exportMethodData: new (),
			attributes: [],
			modifiers: [],
			parameters: []
		);

		var y = new Method (
			type: "MyType",
			name: "MyMethod",
			returnType: new (
				name: "int",
				isNullable: false,
				isBlittable: false,
				isSmartEnum: false,
				isArray: true,
				isReferenceType: false
			),
			symbolAvailability: new (),
			exportMethodData: new (),
			attributes: [],
			modifiers: [],
			parameters: []
		);
		var returnTypeComparer = new TypeInfoComparer ();
		Assert.Equal (returnTypeComparer.Compare (x.ReturnType, y.ReturnType), comparer.Compare (x, y));
	}

	[Fact]
	public void CompareModifierDiffLength ()
	{
		var x = new Method (
			type: "MyType",
			name: "MyMethod",
			returnType: new ("void"),
			symbolAvailability: new (),
			exportMethodData: new (),
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
			returnType: new ("void"),
			symbolAvailability: new (),
			exportMethodData: new (),
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
			returnType: new ("void"),
			symbolAvailability: new (),
			exportMethodData: new (),
			attributes: [],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PartialKeyword),
			],
			parameters: []
		);

		var y = new Method (
			type: "MyType",
			name: "MyMethod",
			returnType: new ("void"),
			symbolAvailability: new (),
			exportMethodData: new (),
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
			returnType: new ("void"),
			symbolAvailability: new (),
			exportMethodData: new (),
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
			returnType: new ("void"),
			symbolAvailability: new (),
			exportMethodData: new (),
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
			returnType: new ("void"),
			symbolAvailability: new (),
			exportMethodData: new (),
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
			returnType: new ("void"),
			symbolAvailability: new (),
			exportMethodData: new (),
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
			returnType: new ("void"),
			symbolAvailability: new (),
			exportMethodData: new (),
			attributes: [
				new ("FirstAttr"),
			],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			],
			parameters: [
				new (position: 0, type: ReturnTypeForString (), name: "name"),
			]
		);

		var y = new Method (
			type: "MyType",
			name: "MyMethod",
			returnType: new ("void"),
			symbolAvailability: new (),
			exportMethodData: new (),
			attributes: [
				new ("FirstAttr"),
			],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			],
			parameters: [
				new (position: 0, type: ReturnTypeForString (), name: "name"),
				new (position: 1, type: ReturnTypeForString (), name: "surname"),
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
			returnType: new ("void"),
			symbolAvailability: new (),
			exportMethodData: new (),
			attributes: [
				new ("FirstAttr"),
			],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			],
			parameters: [
				new (position: 0, type: ReturnTypeForString (), name: "name"),
			]
		);

		var y = new Method (
			type: "MyType",
			name: "MyMethod",
			returnType: new ("void"),
			symbolAvailability: new (),
			exportMethodData: new (),
			attributes: [
				new ("FirstAttr"),
			],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			],
			parameters: [
				new (position: 1, type: ReturnTypeForString (), name: "surname"),
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
			returnType: new ("void"),
			symbolAvailability: new (),
			exportMethodData: new (),
			attributes: [
				new ("FirstAttr"),
			],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			],
			parameters: [
				new (position: 0, type: ReturnTypeForEnum ("MyEnum", isSmartEnum: true), name: "name"),
			]
		);

		var y = new Method (
			type: "MyType",
			name: "MyMethod",
			returnType: new ("void"),
			symbolAvailability: new (),
			exportMethodData: new (),
			attributes: [
				new ("FirstAttr"),
			],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			],
			parameters: [
				new (position: 0, type: ReturnTypeForEnum ("MyEnum"), name: "name"),
			]
		);
		var parameterCompare = new ParameterComparer ();
		Assert.Equal (parameterCompare.Compare (x.Parameters [0], y.Parameters [0]), comparer.Compare (x, y));
	}
}
