using System.Collections.Immutable;
using Microsoft.Macios.Generator.DataModel;
using Xunit;

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
				returnType: "void",
				symbolAvailability: new (),
				attributes: [],
				modifiers: [],
				parameters: []
			)
		];
		ImmutableArray<Method> y = [
			new (
				type: "MyTypeName",
				name: "Test",
				returnType: "void",
				symbolAvailability: new (),
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
				returnType: "void",
				symbolAvailability: new (),
				attributes: [],
				modifiers: [],
				parameters: []
			)
		];
		ImmutableArray<Method> y = [
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: "int",
				symbolAvailability: new (),
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
				returnType: "void",
				symbolAvailability: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (0, "string", "name"),
					new (1, "string", "surname")
				]
			)
		];
		ImmutableArray<Method> y = [
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: "void",
				symbolAvailability: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (0, "string", "name"),
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
				returnType: "void",
				symbolAvailability: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (0, "string", "name"),
					new (1, "string", "surname")
				]
			)
		];
		ImmutableArray<Method> y = [
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: "void",
				symbolAvailability: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (0, "int", "name"),
					new (1, "int", "surname")
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
				returnType: "void",
				symbolAvailability: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (0, "string", "name"),
					new (1, "string", "surname")
				]
			),
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: "void",
				symbolAvailability: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (0, "int", "name"),
					new (1, "int", "surname")
				]
			)
		];
		ImmutableArray<Method> y = [
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: "void",
				symbolAvailability: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (0, "int", "name"),
					new (1, "int", "surname")
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
				returnType: "string",
				symbolAvailability: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (0, "int", "name"),
					new (1, "int", "surname")
				]
			)
		];
		ImmutableArray<Method> y = [
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: "void",
				symbolAvailability: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (0, "int", "name"),
					new (1, "int", "surname")
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
				returnType: "void",
				symbolAvailability: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (0, "string", "name"),
					new (1, "string", "surname")
				]
			),
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: "void",
				symbolAvailability: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (0, "int", "name"),
					new (1, "int", "surname")
				]
			)
		];
		ImmutableArray<Method> y = [
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: "void",
				symbolAvailability: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (0, "string", "name"),
					new (1, "string", "surname")
				]
			),
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: "void",
				symbolAvailability: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (0, "int", "name"),
					new (1, "int", "surname")
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
				returnType: "void",
				symbolAvailability: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (0, "string", "name"),
					new (1, "string", "surname")
				]
			),
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: "void",
				symbolAvailability: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (0, "int", "name"),
					new (1, "int", "surname")
				]
			)
		];
		ImmutableArray<Method> y = [
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: "void",
				symbolAvailability: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (0, "int", "name"),
					new (1, "int", "surname")
				]
			),
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: "void",
				symbolAvailability: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (0, "string", "name"),
					new (1, "string", "surname")
				]
			),
		];

		Assert.True (equalityComparer.Equals (x, y));
	}
}
