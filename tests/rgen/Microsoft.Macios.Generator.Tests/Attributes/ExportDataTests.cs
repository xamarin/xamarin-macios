#pragma warning disable APL0003
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Macios.Generator.Attributes;
using ObjCBindings;
using ObjCRuntime;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.Attributes;

public class ExportDataTests {

	[Fact]
	public void TestExportDataEqualsDiffSelector ()
	{
		var x = new ExportData<Field> ("field1");
		var y = new ExportData<Field> ("field2");
		Assert.False (x.Equals (y));
		Assert.False (y.Equals (x));
		Assert.False (x == y);
		Assert.True (x != y);
	}

	[Fact]
	public void TestExportDataEqualsDiffArgumentSemantic ()
	{
		var x = new ExportData<Field> ("property", ArgumentSemantic.None);
		var y = new ExportData<Field> ("property", ArgumentSemantic.Retain);
		Assert.False (x.Equals (y));
		Assert.False (y.Equals (x));
		Assert.False (x == y);
		Assert.True (x != y);
	}

	[Theory]
	[InlineData (Property.Default, Property.Default, true)]
	[InlineData (Property.Notification, Property.Default, false)]
	public void TestExportDataEqualsDiffFlag (Property xFlag, Property yFlag, bool expected)
	{
		var x = new ExportData<Property> ("property", ArgumentSemantic.None, xFlag);
		var y = new ExportData<Property> ("property", ArgumentSemantic.None, yFlag);
		Assert.Equal (expected, x.Equals (y));
		Assert.Equal (expected, y.Equals (x));
		Assert.Equal (expected, x == y);
		Assert.Equal (!expected, x != y);
	}
	
	class TestDataToString : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			yield return [
				Field.Default,
				new ExportData<Field> ("symbol", ArgumentSemantic.None, Field.Default),
				"{ Type: 'ObjCBindings.Field', Selector: 'symbol', ArgumentSemantic: 'None', Flags: 'Default' }"
			];
			yield return [
				Field.Default,
				new ExportData<Field> ("symbol"),
				"{ Type: 'ObjCBindings.Field', Selector: 'symbol', ArgumentSemantic: 'None', Flags: 'Default' }"
			];
			yield return [
				Property.Default,
				new ExportData<Property> ("symbol", ArgumentSemantic.Retain, Property.Default),
				"{ Type: 'ObjCBindings.Property', Selector: 'symbol', ArgumentSemantic: 'Retain', Flags: 'Default' }"
			];
			yield return [
				Property.Default,
				new ExportData<Property> ("symbol"),
				"{ Type: 'ObjCBindings.Property', Selector: 'symbol', ArgumentSemantic: 'None', Flags: 'Default' }"
			];
		}

		IEnumerator IEnumerable.GetEnumerator ()
			=> GetEnumerator ();
	}

	[Theory]
	[ClassData (typeof(TestDataToString))]
	void TestFieldDataToString<T> (T @enum, ExportData<T> x, string expected) where T : Enum
	{
		Assert.NotNull (@enum);
		Assert.Equal (expected, x.ToString ());
	}
}
