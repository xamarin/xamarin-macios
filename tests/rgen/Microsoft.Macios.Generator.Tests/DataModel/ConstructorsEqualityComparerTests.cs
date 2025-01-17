// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.DataModel;
using Xunit;
using static Microsoft.Macios.Generator.Tests.TestDataFactory;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class ConstructorsEqualityComparerTests {
	readonly ConstructorsEqualityComparer compare = new ();

	[Fact]
	public void CompareEmpty ()
		=> Assert.True (compare.Equals ([], []));

	[Fact]
	public void CompareSingleElementDiffParameterCount ()
	{
		var x = new Constructor ("MyClass",
			symbolAvailability: new (),
			attributes: [],
			modifiers: [],
			parameters: [
				new (position: 0, type: ReturnTypeForString (), name: "name"),
				new (position: 0, type: ReturnTypeForString (), name: "surname"),
			]);
		var y = new Constructor ("MyClass",
			symbolAvailability: new (),
			attributes: [],
			modifiers: [],
			parameters: [
				new (position: 0, type: ReturnTypeForString (), name: "name"),
			]);
		Assert.False (compare.Equals ([x], [y]));
	}

	[Fact]
	public void CompareSingleElementSameParameterCountDifferentParams ()
	{
		var x = new Constructor ("MyClass",
			symbolAvailability: new (),
			attributes: [],
			modifiers: [],
			parameters: [
				new (position: 0, type: ReturnTypeForString (), name: "surname"),
			]);
		var y = new Constructor ("MyClass",
			symbolAvailability: new (),
			attributes: [],
			modifiers: [],
			parameters: [
				new (position: 0, type: ReturnTypeForString (), name: "name"),
			]);
		Assert.False (compare.Equals ([x], [y]));
	}

	[Fact]
	public void CompareDifferentConstructorCount ()
	{
		var x = new Constructor ("MyClass",
			symbolAvailability: new (),
			attributes: [],
			modifiers: [],
			parameters: [
				new (position: 0, type: ReturnTypeForString (), name: "surname"),
			]);
		var y = new Constructor ("MyClass",
			symbolAvailability: new (),
			attributes: [],
			modifiers: [],
			parameters: [
				new (position: 0, type: ReturnTypeForString (), name: "name"),
			]);
		Assert.False (compare.Equals ([x, y], [y]));
	}

	[Fact]
	public void CompareSameConstructorsDifferentOrder ()
	{
		var x = new Constructor ("MyClass",
			symbolAvailability: new (),
			attributes: [],
			modifiers: [],
			parameters: [
				new (position: 0, type: ReturnTypeForString (), name: "surname"),
			]);
		var y = new Constructor ("MyClass",
			symbolAvailability: new (),
			attributes: [],
			modifiers: [],
			parameters: [
				new (position: 0, type: ReturnTypeForString (), name: "name"),
			]);
		Assert.True (compare.Equals ([x, y], [y, x]));
	}

	[Fact]
	public void CompareSameConstructorsDifferentAvailability ()
	{
		var xBuilder = SymbolAvailability.CreateBuilder ();
		xBuilder.Add (new SupportedOSPlatformData ("ios"));
		xBuilder.Add (new SupportedOSPlatformData ("tvos"));
		xBuilder.Add (new UnsupportedOSPlatformData ("macos"));

		var x = new Constructor ("MyClass",
			symbolAvailability: xBuilder.ToImmutable (),
			attributes: [],
			modifiers: [],
			parameters: [
				new (position: 0, type: ReturnTypeForString (), name: "surname"),
			]);

		var yBuilder = SymbolAvailability.CreateBuilder ();
		yBuilder.Add (new SupportedOSPlatformData ("ios"));
		yBuilder.Add (new UnsupportedOSPlatformData ("tvos"));

		var y = new Constructor ("MyClass",
			symbolAvailability: yBuilder.ToImmutable (),
			attributes: [],
			modifiers: [],
			parameters: [
				new (position: 0, type: ReturnTypeForString (), name: "name"),
			]);
		Assert.False (compare.Equals ([x], [y]));
	}

	[Fact]
	public void CompareSameConstructorsSameAvailability ()
	{

		var builder = SymbolAvailability.CreateBuilder ();
		builder.Add (new SupportedOSPlatformData ("ios"));
		builder.Add (new SupportedOSPlatformData ("tvos"));

		var x = new Constructor ("MyClass",
			symbolAvailability: builder.ToImmutable (),
			attributes: [],
			modifiers: [],
			parameters: [
				new (position: 0, type: ReturnTypeForString (), name: "surname"),
			]);

		var y = new Constructor ("MyClass",
			symbolAvailability: builder.ToImmutable (),
			attributes: [],
			modifiers: [],
			parameters: [
				new (position: 0, type: ReturnTypeForString (), name: "surname"),
			]);
		Assert.True (compare.Equals ([x], [y]));

	}
}
