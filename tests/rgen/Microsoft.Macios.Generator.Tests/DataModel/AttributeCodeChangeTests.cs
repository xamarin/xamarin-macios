using Microsoft.Macios.Generator.DataModel;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class AttributeCodeChangeTests {

	[Fact]
	public void EqualsNoParams ()
	{
		var attributeCodeChange1 = new AttributeCodeChange ("name", []);
		var attributeCodeChange2 = new AttributeCodeChange ("name", []);
		Assert.True (attributeCodeChange1.Equals (attributeCodeChange2));
		Assert.True (attributeCodeChange1 == attributeCodeChange2);
		Assert.False (attributeCodeChange1 != attributeCodeChange2);
	}

	[Fact]
	public void EqualsWithParams ()
	{
		var attributeCodeChange1 = new AttributeCodeChange ("name", ["arg1", "arg2"]);
		var attributeCodeChange2 = new AttributeCodeChange ("name", ["arg1", "arg2"]);
		Assert.True (attributeCodeChange1.Equals (attributeCodeChange2));
		Assert.True (attributeCodeChange1 == attributeCodeChange2);
		Assert.False (attributeCodeChange1 != attributeCodeChange2);
	}

	[Fact]
	public void NotEqualsDifferentName ()
	{
		var attributeCodeChange1 = new AttributeCodeChange ("name", []);
		var attributeCodeChange2 = new AttributeCodeChange ("name2", []);
		Assert.False (attributeCodeChange1.Equals (attributeCodeChange2));
		Assert.False (attributeCodeChange1 == attributeCodeChange2);
		Assert.True (attributeCodeChange1 != attributeCodeChange2);
	}

	[Fact]
	public void NotEqualsDifferentParams ()
	{
		var attributeCodeChange1 = new AttributeCodeChange ("name", ["arg1", "arg2"]);
		var attributeCodeChange2 = new AttributeCodeChange ("name", ["arg1", "arg3"]);
		Assert.False (attributeCodeChange1.Equals (attributeCodeChange2));
		Assert.False (attributeCodeChange1 == attributeCodeChange2);
		Assert.True (attributeCodeChange1 != attributeCodeChange2);
	}

	[Fact]
	public void NotEqualsDifferentParamsLength ()
	{
		var attributeCodeChange1 = new AttributeCodeChange ("name", ["arg1", "arg2"]);
		var attributeCodeChange2 = new AttributeCodeChange ("name", ["arg1"]);
		Assert.False (attributeCodeChange1.Equals (attributeCodeChange2));
		Assert.False (attributeCodeChange1 == attributeCodeChange2);
		Assert.True (attributeCodeChange1 != attributeCodeChange2);
	}

	[Fact]
	public void NotEqualsDifferentParamsOrder ()
	{
		var attributeCodeChange1 = new AttributeCodeChange ("name", ["arg1", "arg2"]);
		var attributeCodeChange2 = new AttributeCodeChange ("name", ["arg2", "arg1"]);
		Assert.False (attributeCodeChange1.Equals (attributeCodeChange2));
		Assert.False (attributeCodeChange1 == attributeCodeChange2);
		Assert.True (attributeCodeChange1 != attributeCodeChange2);
	}

}
