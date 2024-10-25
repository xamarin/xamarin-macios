using Microsoft.Macios.Generator.DataModel;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class MemberCodeChangesTests {
	[Fact]
	public void EqualsNoParams ()
	{
		var memberCodeChange1 = new MemberCodeChange ("name", []);
		var memberCodeChange2 = new MemberCodeChange ("name", []);
		Assert.True (memberCodeChange1.Equals (memberCodeChange2));
		Assert.True (memberCodeChange1 == memberCodeChange2);
		Assert.False (memberCodeChange1 != memberCodeChange2);
	}

	[Fact]
	public void EqualsWithArgumentParams ()
	{
		var memberCodeChange1 = new MemberCodeChange ("name", [
			new AttributeCodeChange ("name", ["arg1", "arg2"])
		]);
		var memberCodeChange2 = new MemberCodeChange ("name", [
			new AttributeCodeChange ("name", ["arg1", "arg2"])
		]);
		Assert.True (memberCodeChange1.Equals (memberCodeChange2));
		Assert.True (memberCodeChange1 == memberCodeChange2);
		Assert.False (memberCodeChange1 != memberCodeChange2);
	}

	[Fact]
	public void NotEqualsDifferentName ()
	{
		var memberCodeChange1 = new MemberCodeChange ("name", []);
		var memberCodeChange2 = new MemberCodeChange ("name2", []);
		Assert.False (memberCodeChange1.Equals (memberCodeChange2));
		Assert.False (memberCodeChange1 == memberCodeChange2);
		Assert.True (memberCodeChange1 != memberCodeChange2);
	}

	[Fact]
	public void NotEqualsDiffenretAttributeNames ()
	{
		var memberCodeChange1 = new MemberCodeChange ("name", [
			new AttributeCodeChange ("name", ["arg1", "arg2"])
		]);
		var memberCodeChange2 = new MemberCodeChange ("name", [
			new AttributeCodeChange ("name2", ["arg1", "arg2"])
		]);
		Assert.False (memberCodeChange1.Equals (memberCodeChange2));
		Assert.False (memberCodeChange1 == memberCodeChange2);
		Assert.True (memberCodeChange1 != memberCodeChange2);
	}

	[Fact]
	public void NotEqualsDifferentAttributeParams ()
	{
		var memberCodeChange1 = new MemberCodeChange ("name", [
			new AttributeCodeChange ("name", ["arg1", "arg2"])
		]);
		var memberCodeChange2 = new MemberCodeChange ("name", [
			new AttributeCodeChange ("name", ["arg1", "arg3"])
		]);
		Assert.False (memberCodeChange1.Equals (memberCodeChange2));
		Assert.False (memberCodeChange1 == memberCodeChange2);
		Assert.True (memberCodeChange1 != memberCodeChange2);
	}

	[Fact]
	public void NotEqualsDifferentAttributeParamsOrder ()
	{
		var memberCodeChange1 = new MemberCodeChange ("name", [
			new AttributeCodeChange ("name", ["arg1", "arg2"])
		]);
		var memberCodeChange2 = new MemberCodeChange ("name", [
			new AttributeCodeChange ("name", ["arg2", "arg1"])
		]);
		Assert.False (memberCodeChange1.Equals (memberCodeChange2));
		Assert.False (memberCodeChange1 == memberCodeChange2);
		Assert.True (memberCodeChange1 != memberCodeChange2);
	}

	[Fact]
	public void NotEqualsDifferentAttributesCount ()
	{

		var memberCodeChange1 = new MemberCodeChange ("name", [
			new AttributeCodeChange ("name", ["arg1", "arg2"]),
			new AttributeCodeChange ("name2", [])
		]);
		var memberCodeChange2 = new MemberCodeChange ("name", [
			new AttributeCodeChange ("name", ["arg2", "arg1"])
		]);
		Assert.False (memberCodeChange1.Equals (memberCodeChange2));
		Assert.False (memberCodeChange1 == memberCodeChange2);
		Assert.True (memberCodeChange1 != memberCodeChange2);
	}

}
