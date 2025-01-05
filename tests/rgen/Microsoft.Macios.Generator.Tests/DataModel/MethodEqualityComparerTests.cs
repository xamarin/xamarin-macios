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
				returnType: "void",
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
				returnType: "void",
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
				returnType: "void",
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
				returnType: "void",
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
				returnType: "int",
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
				returnType: "void",
				symbolAvailability: new (),
				exportMethodData: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (position: 0, type: "string", name: "name", isBlittable: false),
					new (position: 1, type: "string", name: "surname", isBlittable: false),
				]
			)
		];
		ImmutableArray<Method> y = [
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: "void",
				symbolAvailability: new (),
				exportMethodData: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (position: 0, type: "string", name: "name", isBlittable: false),
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
				exportMethodData: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (position: 0, type: "string", name: "name", isBlittable: false),
					new (position: 1, type: "string", name: "surname", isBlittable: false),
				]
			)
		];
		ImmutableArray<Method> y = [
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: "void",
				symbolAvailability: new (),
				exportMethodData: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (position: 0, type: "int", name: "name", isBlittable: false),
					new (position: 1, type: "int", name: "surname", isBlittable: false),
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
				exportMethodData: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (position: 0, type: "string", name: "name", isBlittable: false),
					new (position: 1, type: "string", name: "surname", isBlittable: false),
				]
			),
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: "void",
				symbolAvailability: new (),
				exportMethodData: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (position: 0, type: "int", name: "name", isBlittable: false),
					new (position: 1, type: "int", name: "surname", isBlittable: false),
				]
			)
		];
		ImmutableArray<Method> y = [
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: "void",
				symbolAvailability: new (),
				exportMethodData: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (position: 0, type: "int", name: "name", isBlittable: false),
					new (position: 1, type: "int", name: "surname", isBlittable: false),
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
				exportMethodData: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (position: 0, type: "int", name: "name", isBlittable: false),
					new (position: 1, type: "int", name: "surname", isBlittable: false),
				]
			)
		];
		ImmutableArray<Method> y = [
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: "void",
				symbolAvailability: new (),
				exportMethodData: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (position: 0, type: "int", name: "name", isBlittable: false),
					new (position: 1, type: "int", name: "surname", isBlittable: false),
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
				exportMethodData: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (position: 0, type: "string", name: "name", isBlittable: false),
					new (position: 1, type: "string", name: "surname", isBlittable: false),
				]
			),
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: "void",
				symbolAvailability: new (),
				exportMethodData: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (position: 0, type: "int", name: "name", isBlittable: false),
					new (position: 1, type: "int", name: "surname", isBlittable: false),
				]
			)
		];
		ImmutableArray<Method> y = [
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: "void",
				symbolAvailability: new (),
				exportMethodData: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (position: 0, type: "string", name: "name", isBlittable: false),
					new (position: 1, type: "string", name: "surname", isBlittable: false),
				]
			),
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: "void",
				symbolAvailability: new (),
				exportMethodData: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (position: 0, type: "int", name: "name", isBlittable: false),
					new (position: 1, type: "int", name: "surname", isBlittable: false),
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
				exportMethodData: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (position: 0, type: "string", name: "name", isBlittable: false),
					new (position: 1, type: "string", name: "surname", isBlittable: false),
				]
			),
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: "void",
				symbolAvailability: new (),
				exportMethodData: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (position: 0, type: "int", name: "name", isBlittable: false),
					new (position: 1, type: "int", name: "surname", isBlittable: false),
				]
			)
		];
		ImmutableArray<Method> y = [
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: "void",
				symbolAvailability: new (),
				exportMethodData: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (position: 0, type: "int", name: "name", isBlittable: false),
					new (position: 1, type: "int", name: "surname", isBlittable: false),
				]
			),
			new (
				type: "MyTypeName",
				name: "MyMethod",
				returnType: "void",
				symbolAvailability: new (),
				exportMethodData: new (),
				attributes: [],
				modifiers: [],
				parameters: [
					new (position: 0, type: "string", name: "name", isBlittable: false),
					new (position: 1, type: "string", name: "surname", isBlittable: false),
				]
			),
		];

		Assert.True (equalityComparer.Equals (x, y));
	}
}
