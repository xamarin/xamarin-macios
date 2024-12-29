#pragma warning disable APL0003
using System.Collections;
using System.Collections.Generic;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.DataModel;
using ObjCBindings;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class EnumMemberCodeChangesTests {
	[Fact]
	public void EqualsNoParams ()
	{
		var memberCodeChange1 = new EnumMember ("name", new (), new (), []);
		var memberCodeChange2 = new EnumMember ("name", new (), new (), []);
		Assert.True (memberCodeChange1.Equals (memberCodeChange2));
		Assert.True (memberCodeChange1 == memberCodeChange2);
		Assert.False (memberCodeChange1 != memberCodeChange2);
	}

	[Fact]
	public void EqualsWithArgumentParams ()
	{
		var memberCodeChange1 = new EnumMember ("name", new (), new (), [
			new AttributeCodeChange ("name", ["arg1", "arg2"])
		]);
		var memberCodeChange2 = new EnumMember ("name", new (), new (), [
			new AttributeCodeChange ("name", ["arg1", "arg2"])
		]);
		Assert.True (memberCodeChange1.Equals (memberCodeChange2));
		Assert.True (memberCodeChange1 == memberCodeChange2);
		Assert.False (memberCodeChange1 != memberCodeChange2);
	}

	[Fact]
	public void NotEqualsDifferentName ()
	{
		var memberCodeChange1 = new EnumMember ("name", new (), new (), []);
		var memberCodeChange2 = new EnumMember ("name2", new (), new (), []);
		Assert.False (memberCodeChange1.Equals (memberCodeChange2));
		Assert.False (memberCodeChange1 == memberCodeChange2);
		Assert.True (memberCodeChange1 != memberCodeChange2);
	}

	[Fact]
	public void NotEqualsDifferentAttributeNames ()
	{
		var memberCodeChange1 = new EnumMember ("name", new (), new (), [
			new AttributeCodeChange ("name", ["arg1", "arg2"])
		]);
		var memberCodeChange2 = new EnumMember ("name", new (), new (), [
			new AttributeCodeChange ("name2", ["arg1", "arg2"])
		]);
		Assert.False (memberCodeChange1.Equals (memberCodeChange2));
		Assert.False (memberCodeChange1 == memberCodeChange2);
		Assert.True (memberCodeChange1 != memberCodeChange2);
	}

	[Fact]
	public void NotEqualsDifferentAttributeParams ()
	{
		var memberCodeChange1 = new EnumMember ("name", new (), new (), [
			new AttributeCodeChange ("name", ["arg1", "arg2"])
		]);
		var memberCodeChange2 = new EnumMember ("name", new (), new (), [
			new AttributeCodeChange ("name", ["arg1", "arg3"])
		]);
		Assert.False (memberCodeChange1.Equals (memberCodeChange2));
		Assert.False (memberCodeChange1 == memberCodeChange2);
		Assert.True (memberCodeChange1 != memberCodeChange2);
	}

	[Fact]
	public void NotEqualsDifferentAttributeParamsOrder ()
	{
		var memberCodeChange1 = new EnumMember ("name", new (), new (), [
			new AttributeCodeChange ("name", ["arg1", "arg2"])
		]);
		var memberCodeChange2 = new EnumMember ("name", new (), new (), [
			new AttributeCodeChange ("name", ["arg2", "arg1"])
		]);
		Assert.False (memberCodeChange1.Equals (memberCodeChange2));
		Assert.False (memberCodeChange1 == memberCodeChange2);
		Assert.True (memberCodeChange1 != memberCodeChange2);
	}

	[Fact]
	public void NotEqualsDifferentAttributesCount ()
	{
		var memberCodeChange1 = new EnumMember ("name", new (), new (), [
			new AttributeCodeChange ("name", ["arg1", "arg2"]),
			new AttributeCodeChange ("name2", [])
		]);
		var memberCodeChange2 = new EnumMember ("name", new (), new (), [
			new AttributeCodeChange ("name", ["arg2", "arg1"])
		]);
		Assert.False (memberCodeChange1.Equals (memberCodeChange2));
		Assert.False (memberCodeChange1 == memberCodeChange2);
		Assert.True (memberCodeChange1 != memberCodeChange2);
	}

	[Fact]
	public void NotEqualsDifferentPlatformAvailability ()
	{
		var builder = SymbolAvailability.CreateBuilder ();
		builder.Add (new SupportedOSPlatformData ("ios"));
		builder.Add (new SupportedOSPlatformData ("tvos"));
		builder.Add (new UnsupportedOSPlatformData ("tvos"));
		var availability = builder.ToImmutable ();

		var memberCodeChange1 = new EnumMember ("name", new (), availability, [
			new AttributeCodeChange ("name", ["arg1", "arg2"]),
			new AttributeCodeChange ("name2", [])
		]);
		var memberCodeChange2 = new EnumMember ("name", new (), new (), [
			new AttributeCodeChange ("name", ["arg2", "arg1"])
		]);
		Assert.False (memberCodeChange1.Equals (memberCodeChange2));
		Assert.False (memberCodeChange1 == memberCodeChange2);
		Assert.True (memberCodeChange1 != memberCodeChange2);
	}

	[Fact]
	public void NotEqualsDifferentFieldData ()
	{
		var memberCodeChange1 = new EnumMember ("name", new ("x", "libName", EnumValue.None), new (), [
			new AttributeCodeChange ("name", ["arg1", "arg2"]),
			new AttributeCodeChange ("name2", [])
		]);
		var memberCodeChange2 = new EnumMember ("name", new ("x", "yLibName", EnumValue.None), new (), [
			new AttributeCodeChange ("name", ["arg2", "arg1"])
		]);
		Assert.False (memberCodeChange1.Equals (memberCodeChange2));
		Assert.False (memberCodeChange1 == memberCodeChange2);
		Assert.True (memberCodeChange1 != memberCodeChange2);
	}

	class TestDataToString : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			var simpleEnum = new EnumMember (
				name: "EnumValue",
				fieldData: null,
				symbolAvailability: new (),
				attributes: []);
			yield return [simpleEnum, "{ Name: 'EnumValue' SymbolAvailability: [] FieldData:  Attributes: [] }"];

			var fieldDataEnum = new EnumMember (
				name: "EnumValue",
				fieldData: new ("x", "libName", EnumValue.None),
				symbolAvailability: new (),
				attributes: []);
			yield return [fieldDataEnum, "{ Name: 'EnumValue' SymbolAvailability: [] FieldData: { SymbolName: 'x' LibraryName: 'libName', Flags: 'None' } Attributes: [] }"];

			var builder = SymbolAvailability.CreateBuilder ();
			builder.Add (new SupportedOSPlatformData ("ios"));

			var availabilityEnum = new EnumMember (
				name: "EnumValue",
				fieldData: new ("x", "libName", EnumValue.None),
				symbolAvailability: builder.ToImmutable (),
				attributes: []);
			yield return [availabilityEnum, "{ Name: 'EnumValue' SymbolAvailability: [{ Platform: 'iOS', Supported: '0.0', Unsupported: [], Obsoleted: [] }] FieldData: { SymbolName: 'x' LibraryName: 'libName', Flags: 'None' } Attributes: [] }"];

			var attrsEnum = new EnumMember (
				name: "EnumValue",
				fieldData: new ("x", "libName", EnumValue.None),
				symbolAvailability: builder.ToImmutable (),
				attributes: [
					new ("Attribute1"),
					new ("Attribute2"),
				]);
			yield return [attrsEnum, "{ Name: 'EnumValue' SymbolAvailability: [{ Platform: 'iOS', Supported: '0.0', Unsupported: [], Obsoleted: [] }] FieldData: { SymbolName: 'x' LibraryName: 'libName', Flags: 'None' } Attributes: [{ Name: Attribute1, Arguments: [] }, { Name: Attribute2, Arguments: [] }] }"];
		}

		IEnumerator IEnumerable.GetEnumerator ()
			=> GetEnumerator ();
	}

	[Theory]
	[ClassData (typeof (TestDataToString))]
	void TestFieldDataToString (EnumMember x, string expected)
		=> Assert.Equal (expected, x.ToString ());

}
