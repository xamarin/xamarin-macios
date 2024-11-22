using Microsoft.Macios.Generator.DataModel;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class ConstructorsEqualityComparerTests {
	readonly ConstructorsEqualityComparer compare = new();

	[Fact]
	public void CompareEmpty ()
		=> Assert.True (compare.Equals ([], []));

	[Fact]
	public void CompareSingleElementDiffParameterCount ()
	{
		var x = new Constructor ("MyClass",
			attributes: [],
			modifiers: [],
			parameters: [
				new(0, "string", "name"),
				new(0, "string", "surname"),
			]);
		var y = new Constructor ("MyClass",
			attributes: [],
			modifiers: [],
			parameters: [
				new(0, "string", "name"),
			]);
		Assert.False (compare.Equals ([x], [y]));
	}

	[Fact]
	public void CompareSingleElementSameParameterCountDifferentParams ()
	{
		var x = new Constructor ("MyClass",
			attributes: [],
			modifiers: [],
			parameters: [
				new(0, "string", "surname"),
			]);
		var y = new Constructor ("MyClass",
			attributes: [],
			modifiers: [],
			parameters: [
				new(0, "string", "name"),
			]);
		Assert.False (compare.Equals ([x], [y]));
	}

	[Fact]
	public void CompareDifferentConstructorCount ()
	{
		var x = new Constructor ("MyClass",
			attributes: [],
			modifiers: [],
			parameters: [
				new(0, "string", "surname"),
			]);
		var y = new Constructor ("MyClass",
			attributes: [],
			modifiers: [],
			parameters: [
				new(0, "string", "name"),
			]);
		Assert.False (compare.Equals ([x, y], [y]));
	}

	[Fact]
	public void CompareSameConstructorsDifferentOrder ()
	{
		var x = new Constructor ("MyClass",
			attributes: [],
			modifiers: [],
			parameters: [
				new(0, "string", "surname"),
			]);
		var y = new Constructor ("MyClass",
			attributes: [],
			modifiers: [],
			parameters: [
				new(0, "string", "name"),
			]);
		Assert.True (compare.Equals ([x, y], [y, x]));
	}
}
