#pragma warning disable APL0003
using System.Collections;
using System.Collections.Generic;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.DataModel;
using ObjCBindings;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class BindingDataTests {

	[Fact]
	public void ConstructorBindingTypeData ()
	{

		var bindingTypeData = new BindingTypeData ("BindingType");
		BindingData bindingData = new BindingData (BindingType.SmartEnum, bindingTypeData);
		Assert.Equal (bindingTypeData, (BindingTypeData) bindingData);
		Assert.Equal (bindingTypeData.Name, ((BindingTypeData) bindingData).Name);
		Assert.Equal (BindingType.SmartEnum, bindingData.BindingType);
	}

	[Fact]
	public void ConstructorBindingTypeClassData ()
	{
		var bindingTypeData = new BindingTypeData<Class> ("BindingType", Class.DisableDefaultCtor);
		BindingData bindingData = new BindingData (bindingTypeData);
		Assert.Equal (bindingTypeData, (BindingTypeData<Class>) bindingData);
		Assert.Equal (bindingTypeData.Name, ((BindingTypeData<Class>) bindingData).Name);
		Assert.Equal (bindingTypeData.Flags, ((BindingTypeData<Class>) bindingData).Flags);
		Assert.Equal (BindingType.Class, bindingData.BindingType);
	}

	[Fact]
	public void ConstructorBindingProtocolData ()
	{
		var bindingTypeData = new BindingTypeData<Protocol> ("BindingType");
		BindingData bindingData = new BindingData (bindingTypeData);
		Assert.Equal (bindingTypeData, (BindingTypeData<Protocol>) bindingData);
		Assert.Equal (bindingTypeData.Name, ((BindingTypeData<Protocol>) bindingData).Name);
		Assert.Equal (bindingTypeData.Flags, ((BindingTypeData<Protocol>) bindingData).Flags);
		Assert.Equal (BindingType.Protocol, bindingData.BindingType);
	}

	[Fact]
	public void ConstructorBindingCategoryData ()
	{
		var bindingTypeData = new BindingTypeData<Category> ("BindingType");
		BindingData bindingData = new BindingData (bindingTypeData);
		Assert.Equal (bindingTypeData, (BindingTypeData<Category>) bindingData);
		Assert.Equal (bindingTypeData.Name, ((BindingTypeData<Category>) bindingData).Name);
		Assert.Equal (bindingTypeData.Flags, ((BindingTypeData<Category>) bindingData).Flags);
		Assert.Equal (BindingType.Category, bindingData.BindingType);
	}

	[Fact]
	public void EqualsDiffBindingType ()
	{
		var xBinding = new BindingTypeData ("Name");
		var x = new BindingData (BindingType.SmartEnum, xBinding);
		var y = new BindingData (BindingType.Unknown, xBinding);
		Assert.False (x.Equals (y));
		Assert.False (y.Equals (x));
		Assert.False (x == y);
		Assert.True (x != y);
	}

	[Fact]
	public void EqualsDiffEnumBindingType ()
	{
		var xBinding = new BindingTypeData ("Name1");
		var yBinding = new BindingTypeData<Class> ("Name2");
		var x = new BindingData (BindingType.SmartEnum, xBinding);
		var y = new BindingData (yBinding);
		Assert.False (x.Equals (y));
		Assert.False (y.Equals (x));
		Assert.False (x == y);
		Assert.True (x != y);
	}

	[Fact]
	public void EqualsSameEnumBindingType ()
	{
		var xBinding = new BindingTypeData ("Name");
		var yBinding = new BindingTypeData ("Name");
		var x = new BindingData (BindingType.SmartEnum, xBinding);
		var y = new BindingData (BindingType.SmartEnum, yBinding);
		Assert.True (x.Equals (y));
		Assert.True (y.Equals (x));
		Assert.True (x == y);
		Assert.False (x != y);
	}

	[Fact]
	public void EqualsDiffClassBindingType ()
	{
		var xBinding = new BindingTypeData<Class> ("Name1");
		var yBinding = new BindingTypeData<Class> ("Name2");
		var x = new BindingData (xBinding);
		var y = new BindingData (yBinding);
		Assert.False (x.Equals (y));
		Assert.False (y.Equals (x));
		Assert.False (x == y);
		Assert.True (x != y);
	}

	[Fact]
	public void EqualsSameClassBindingType ()
	{
		var xBinding = new BindingTypeData<Class> ("Name");
		var yBinding = new BindingTypeData<Class> ("Name");
		var x = new BindingData (xBinding);
		var y = new BindingData (yBinding);
		Assert.True (x.Equals (y));
		Assert.True (y.Equals (x));
		Assert.True (x == y);
		Assert.False (x != y);
	}

	[Fact]
	public void EqualsDiffProtocolBindingType ()
	{
		var xBinding = new BindingTypeData<Protocol> ("Name1");
		var yBinding = new BindingTypeData<Protocol> ("Name2");
		var x = new BindingData (xBinding);
		var y = new BindingData (yBinding);
		Assert.False (x.Equals (y));
		Assert.False (y.Equals (x));
		Assert.False (x == y);
		Assert.True (x != y);
	}

	[Fact]
	public void EqualsSameProtocolBindingType ()
	{
		var xBinding = new BindingTypeData<Protocol> ("Name");
		var yBinding = new BindingTypeData<Protocol> ("Name");
		var x = new BindingData (xBinding);
		var y = new BindingData (yBinding);
		Assert.True (x.Equals (y));
		Assert.True (y.Equals (x));
		Assert.True (x == y);
		Assert.False (x != y);
	}

	[Fact]
	public void EqualsDiffCategoryBindingType ()
	{
		var xBinding = new BindingTypeData<Category> ("Name1");
		var yBinding = new BindingTypeData<Category> ("Name2");
		var x = new BindingData (xBinding);
		var y = new BindingData (yBinding);
		Assert.False (x.Equals (y));
		Assert.False (y.Equals (x));
		Assert.False (x == y);
		Assert.True (x != y);
	}

	[Fact]
	public void EqualsSameCategoryBindingType ()
	{
		var xBinding = new BindingTypeData<Category> ("Name");
		var yBinding = new BindingTypeData<Category> ("Name");
		var x = new BindingData (xBinding);
		var y = new BindingData (yBinding);
		Assert.True (x.Equals (y));
		Assert.True (y.Equals (x));
		Assert.True (x == y);
		Assert.False (x != y);
	}

	class TestDataToString : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			yield return [
				new BindingData (BindingType.SmartEnum, new BindingTypeData ("Name")),
				"{ BindingType: SmartEnum, BindingData: { Name: 'Name' } }",
			];

			yield return [
				new BindingData (new BindingTypeData<Class> ("Name")),
				"{ BindingType: Class, BindingData: { Name: 'Name', Flags: 'Default' } }"
			];

			yield return [
				new BindingData (new BindingTypeData<Class> ("Name", Class.DisableDefaultCtor)),
				"{ BindingType: Class, BindingData: { Name: 'Name', Flags: 'DisableDefaultCtor' } }"
			];

			yield return [
				new BindingData (new BindingTypeData<Protocol> ("Name")),
				"{ BindingType: Protocol, BindingData: { Name: 'Name', Flags: 'Default' } }"
			];

			yield return [
				new BindingData (new BindingTypeData<Category> ("Name")),
				"{ BindingType: Category, BindingData: { Name: 'Name', Flags: 'Default' } }"
			];
		}

		IEnumerator IEnumerable.GetEnumerator ()
			=> GetEnumerator ();
	}

	[Theory]
	[ClassData (typeof (TestDataToString))]
	void TestFieldDataToString (BindingData x, string expected)
		=> Assert.Equal (expected, x.ToString ());
}
