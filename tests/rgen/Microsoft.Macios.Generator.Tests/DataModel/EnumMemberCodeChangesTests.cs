using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.DataModel;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class EnumMemberCodeChangesTests {
	[Fact]
	public void EqualsNoParams ()
	{
		var memberCodeChange1 = new EnumMember ("name", new(),[]);
		var memberCodeChange2 = new EnumMember ("name", new(),[]);
		Assert.True (memberCodeChange1.Equals (memberCodeChange2));
		Assert.True (memberCodeChange1 == memberCodeChange2);
		Assert.False (memberCodeChange1 != memberCodeChange2);
	}

	[Fact]
	public void EqualsWithArgumentParams ()
	{
		var memberCodeChange1 = new EnumMember ("name", new(),[
			new AttributeCodeChange ("name", ["arg1", "arg2"])
		]);
		var memberCodeChange2 = new EnumMember ("name", new(),[
			new AttributeCodeChange ("name", ["arg1", "arg2"])
		]);
		Assert.True (memberCodeChange1.Equals (memberCodeChange2));
		Assert.True (memberCodeChange1 == memberCodeChange2);
		Assert.False (memberCodeChange1 != memberCodeChange2);
	}

	[Fact]
	public void NotEqualsDifferentName ()
	{
		var memberCodeChange1 = new EnumMember ("name", new(),[]);
		var memberCodeChange2 = new EnumMember ("name2", new(),[]);
		Assert.False (memberCodeChange1.Equals (memberCodeChange2));
		Assert.False (memberCodeChange1 == memberCodeChange2);
		Assert.True (memberCodeChange1 != memberCodeChange2);
	}

	[Fact]
	public void NotEqualsDifferentAttributeNames ()
	{
		var memberCodeChange1 = new EnumMember ("name", new(),[
			new AttributeCodeChange ("name", ["arg1", "arg2"])
		]);
		var memberCodeChange2 = new EnumMember ("name", new(),[
			new AttributeCodeChange ("name2", ["arg1", "arg2"])
		]);
		Assert.False (memberCodeChange1.Equals (memberCodeChange2));
		Assert.False (memberCodeChange1 == memberCodeChange2);
		Assert.True (memberCodeChange1 != memberCodeChange2);
	}

	[Fact]
	public void NotEqualsDifferentAttributeParams ()
	{
		var memberCodeChange1 = new EnumMember ("name", new(),[
			new AttributeCodeChange ("name", ["arg1", "arg2"])
		]);
		var memberCodeChange2 = new EnumMember ("name", new(),[
			new AttributeCodeChange ("name", ["arg1", "arg3"])
		]);
		Assert.False (memberCodeChange1.Equals (memberCodeChange2));
		Assert.False (memberCodeChange1 == memberCodeChange2);
		Assert.True (memberCodeChange1 != memberCodeChange2);
	}

	[Fact]
	public void NotEqualsDifferentAttributeParamsOrder ()
	{
		var memberCodeChange1 = new EnumMember ("name", new(),[
			new AttributeCodeChange ("name", ["arg1", "arg2"])
		]);
		var memberCodeChange2 = new EnumMember ("name", new(),[
			new AttributeCodeChange ("name", ["arg2", "arg1"])
		]);
		Assert.False (memberCodeChange1.Equals (memberCodeChange2));
		Assert.False (memberCodeChange1 == memberCodeChange2);
		Assert.True (memberCodeChange1 != memberCodeChange2);
	}

	[Fact]
	public void NotEqualsDifferentAttributesCount ()
	{
		var memberCodeChange1 = new EnumMember ("name", new(),[
			new AttributeCodeChange ("name", ["arg1", "arg2"]),
			new AttributeCodeChange ("name2", [])
		]);
		var memberCodeChange2 = new EnumMember ("name", new(),[
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
		builder.Add (new SupportedOSPlatformData("ios"));
		builder.Add (new SupportedOSPlatformData("tvos"));
		builder.Add (new UnsupportedOSPlatformData("tvos"));
		var availability = builder.ToImmutable ();
		
		var memberCodeChange1 = new EnumMember ("name", availability,[
			new AttributeCodeChange ("name", ["arg1", "arg2"]),
			new AttributeCodeChange ("name2", [])
		]);
		var memberCodeChange2 = new EnumMember ("name", new(),[
			new AttributeCodeChange ("name", ["arg2", "arg1"])
		]);
		Assert.False (memberCodeChange1.Equals (memberCodeChange2));
		Assert.False (memberCodeChange1 == memberCodeChange2);
		Assert.True (memberCodeChange1 != memberCodeChange2);
	}

}
