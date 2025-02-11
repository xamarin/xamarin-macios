// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Microsoft.Macios.Generator.DataModel;
using Xunit;
using static Microsoft.Macios.Generator.Tests.TestDataFactory;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class MethodEqualityComparerTests {
	readonly MethodsEqualityComparer equalityComparer = new ();

	[Fact]
	public void CompareEmpty ()
		=> Assert.True (equalityComparer.Equals ([], []));

	[Fact]
	public void CompareSingleElementDifferentMethodName ()
	{
		ImmutableArray<Method> x = [
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: new ("void"),
				symbolAvailability: new (),
				exportMethodData: new (),
				attributes: [],
				modifiers: [],
				parameters: []
			)
		];
		ImmutableArray<Method> y = [
			new (
				type: "MyTypeName",
				name: "Test",
				returnType: new ("void"),
				symbolAvailability: new (),
				exportMethodData: new (),
				attributes: [],
				modifiers: [],
				parameters: []
			)
		];
		Assert.False (equalityComparer.Equals (x, y));
	}

	[Fact]
	public void CompareSingleElementDifferentExportData ()
	{
		ImmutableArray<Method> x = [
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: new ("void"),
				symbolAvailability: new (),
				exportMethodData: new ("myMethod"),
				attributes: [],
				modifiers: [],
				parameters: []
			)
		];
		ImmutableArray<Method> y = [
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: new ("void"),
				symbolAvailability: new (),
				exportMethodData: new ("objcMethod"),
				attributes: [],
				modifiers: [],
				parameters: []
			)
		];
		Assert.False (equalityComparer.Equals (x, y));
	}

	[Fact]
	public void CompareSingleElementDifferentReturnType ()
	{
		ImmutableArray<Method> x = [
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: new ("void"),
				symbolAvailability: new (),
				exportMethodData: new (),
				attributes: [],
				modifiers: [],
				parameters: []
			)
		];
		ImmutableArray<Method> y = [
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: new ("int"),
				symbolAvailability: new (),
				exportMethodData: new (),
				attributes: [],
				modifiers: [],
				parameters: []
			)
		];
		Assert.False (equalityComparer.Equals (x, y));
	}

	[Fact]
	public void CompareSingleElementDifferentParameterCount ()
	{
		ImmutableArray<Method> x = [
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: new ("void"),
				symbolAvailability: new (),
				exportMethodData: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (position: 0, type: ReturnTypeForString (), name: "name"),
					new (position: 1, type: ReturnTypeForString (), name: "surname"),
				]
			)
		];
		ImmutableArray<Method> y = [
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: new ("void"),
				symbolAvailability: new (),
				exportMethodData: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (position: 0, type: ReturnTypeForString (), name: "name"),
				]
			)
		];
		Assert.False (equalityComparer.Equals (x, y));
	}

	[Fact]
	public void CompareSingleElementDifferentParameterType ()
	{
		ImmutableArray<Method> x = [
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: new ("void"),
				symbolAvailability: new (),
				exportMethodData: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (position: 0, type: ReturnTypeForString (), name: "name"),
					new (position: 1, type: ReturnTypeForString (), name: "surname"),
				]
			)
		];
		ImmutableArray<Method> y = [
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: new ("void"),
				symbolAvailability: new (),
				exportMethodData: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (position: 0, type: ReturnTypeForInt (), name: "name"),
					new (position: 1, type: ReturnTypeForInt (), name: "surname"),
				]
			)
		];
		Assert.False (equalityComparer.Equals (x, y));
	}

	[Fact]
	public void CompareDifferentMethodCount ()
	{
		ImmutableArray<Method> x = [
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: new ("void"),
				symbolAvailability: new (),
				exportMethodData: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (position: 0, type: ReturnTypeForString (), name: "name"),
					new (position: 1, type: ReturnTypeForString (), name: "surname"),
				]
			),
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: new ("void"),
				symbolAvailability: new (),
				exportMethodData: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (position: 0, type: ReturnTypeForInt (), name: "name"),
					new (position: 1, type: ReturnTypeForInt (), name: "surname"),
				]
			)
		];
		ImmutableArray<Method> y = [
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: new ("void"),
				symbolAvailability: new (),
				exportMethodData: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (position: 0, type: ReturnTypeForInt (), name: "name"),
					new (position: 1, type: ReturnTypeForInt (), name: "surname"),
				]
			)
		];
		Assert.False (equalityComparer.Equals (x, y));
	}

	[Fact]
	public void CompareSameMethodNamesDiffReturnTypes ()
	{
		ImmutableArray<Method> x = [
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: new ("void"),
				symbolAvailability: new (),
				exportMethodData: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (position: 0, type: ReturnTypeForInt (), name: "name"),
					new (position: 1, type: ReturnTypeForInt (), name: "surname"),
				]
			)
		];
		ImmutableArray<Method> y = [
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: new ("string"),
				symbolAvailability: new (),
				exportMethodData: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (position: 0, type: ReturnTypeForInt (), name: "name"),
					new (position: 1, type: ReturnTypeForInt (), name: "surname"),
				]
			)
		];
		Assert.False (equalityComparer.Equals (x, y));
	}

	[Fact]
	public void CompareSameMethods ()
	{
		ImmutableArray<Method> x = [
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: new ("void"),
				symbolAvailability: new (),
				exportMethodData: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (position: 0, type: ReturnTypeForString (), name: "name"),
					new (position: 1, type: ReturnTypeForString (), name: "surname"),
				]
			),
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: new ("void"),
				symbolAvailability: new (),
				exportMethodData: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (position: 0, type: ReturnTypeForInt (), name: "name"),
					new (position: 1, type: ReturnTypeForInt (), name: "surname"),
				]
			)
		];
		ImmutableArray<Method> y = [
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: new ("void"),
				symbolAvailability: new (),
				exportMethodData: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (position: 0, type: ReturnTypeForString (), name: "name"),
					new (position: 1, type: ReturnTypeForString (), name: "surname"),
				]
			),
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: new ("void"),
				symbolAvailability: new (),
				exportMethodData: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (position: 0, type: ReturnTypeForInt (), name: "name"),
					new (position: 1, type: ReturnTypeForInt (), name: "surname"),
				]
			)
		];

		Assert.True (equalityComparer.Equals (x, y));
	}

	[Fact]
	public void CompareSameMethodsDifferentOrder ()
	{
		ImmutableArray<Method> x = [
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: new ("void"),
				symbolAvailability: new (),
				exportMethodData: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (position: 0, type: ReturnTypeForString (), name: "name"),
					new (position: 1, type: ReturnTypeForString (), name: "surname"),
				]
			),
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: new ("void"),
				symbolAvailability: new (),
				exportMethodData: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (position: 0, type: ReturnTypeForInt (), name: "name"),
					new (position: 1, type: ReturnTypeForInt (), name: "surname"),
				]
			)
		];
		ImmutableArray<Method> y = [
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: new ("void"),
				symbolAvailability: new (),
				exportMethodData: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (position: 0, type: ReturnTypeForInt (), name: "name"),
					new (position: 1, type: ReturnTypeForInt (), name: "surname"),
				]
			),
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: new ("void"),
				symbolAvailability: new (),
				exportMethodData: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (position: 0, type: ReturnTypeForString (), name: "name"),
					new (position: 1, type: ReturnTypeForString (), name: "surname"),
				]
			),
		];

		Assert.True (equalityComparer.Equals (x, y));
	}
}
