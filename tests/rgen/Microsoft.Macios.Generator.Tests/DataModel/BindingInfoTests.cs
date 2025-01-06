#pragma warning disable APL0003
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.DataModel;
using ObjCBindings;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class BindingInfoTests {

	[Fact]
	public void ConstructorBindingTypeData ()
	{

		var bindingTypeData = new BindingTypeData ("BindingType");
		BindingInfo bindingInfo = new BindingInfo (BindingType.SmartEnum, bindingTypeData);
		Assert.Equal (bindingTypeData, (BindingTypeData) bindingInfo);
		Assert.Equal (bindingTypeData.Name, ((BindingTypeData) bindingInfo).Name);
		Assert.Equal (BindingType.SmartEnum, bindingInfo.BindingType);
	}

	[Fact]
	public void ConstructorBindingTypeClassData ()
	{
		var bindingTypeData = new BindingTypeData<Class> ("BindingType", Class.DisableDefaultCtor);
		BindingInfo bindingInfo = new BindingInfo (bindingTypeData);
		Assert.Equal (bindingTypeData, (BindingTypeData<Class>) bindingInfo);
		Assert.Equal (bindingTypeData.Name, ((BindingTypeData<Class>) bindingInfo).Name);
		Assert.Equal (bindingTypeData.Flags, ((BindingTypeData<Class>) bindingInfo).Flags);
		Assert.Equal (BindingType.Class, bindingInfo.BindingType);
	}

	[Fact]
	public void ConstructorBindingProtocolData ()
	{
		var bindingTypeData = new BindingTypeData<Protocol> ("BindingType");
		BindingInfo bindingInfo = new BindingInfo (bindingTypeData);
		Assert.Equal (bindingTypeData, (BindingTypeData<Protocol>) bindingInfo);
		Assert.Equal (bindingTypeData.Name, ((BindingTypeData<Protocol>) bindingInfo).Name);
		Assert.Equal (bindingTypeData.Flags, ((BindingTypeData<Protocol>) bindingInfo).Flags);
		Assert.Equal (BindingType.Protocol, bindingInfo.BindingType);
	}

	[Fact]
	public void ConstructorBindingCategoryData ()
	{
		var bindingTypeData = new BindingTypeData<Category> ("BindingType");
		BindingInfo bindingInfo = new BindingInfo (bindingTypeData);
		Assert.Equal (bindingTypeData, (BindingTypeData<Category>) bindingInfo);
		Assert.Equal (bindingTypeData.Name, ((BindingTypeData<Category>) bindingInfo).Name);
		Assert.Equal (bindingTypeData.Flags, ((BindingTypeData<Category>) bindingInfo).Flags);
		Assert.Equal (BindingType.Category, bindingInfo.BindingType);
	}

	[Fact]
	public void EqualsDiffBindingType ()
	{
		var xBinding = new BindingTypeData ("Name");
		var x = new BindingInfo (BindingType.SmartEnum, xBinding);
		var y = new BindingInfo (BindingType.Unknown, xBinding);
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
		var x = new BindingInfo (BindingType.SmartEnum, xBinding);
		var y = new BindingInfo (yBinding);
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
		var x = new BindingInfo (BindingType.SmartEnum, xBinding);
		var y = new BindingInfo (BindingType.SmartEnum, yBinding);
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
		var x = new BindingInfo (xBinding);
		var y = new BindingInfo (yBinding);
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
		var x = new BindingInfo (xBinding);
		var y = new BindingInfo (yBinding);
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
		var x = new BindingInfo (xBinding);
		var y = new BindingInfo (yBinding);
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
		var x = new BindingInfo (xBinding);
		var y = new BindingInfo (yBinding);
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
		var x = new BindingInfo (xBinding);
		var y = new BindingInfo (yBinding);
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
		var x = new BindingInfo (xBinding);
		var y = new BindingInfo (yBinding);
		Assert.True (x.Equals (y));
		Assert.True (y.Equals (x));
		Assert.True (x == y);
		Assert.False (x != y);
	}

	class TestDataToString : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			yield return [
				new BindingInfo (BindingType.SmartEnum, new BindingTypeData ("Name")),
				"{ BindingType: SmartEnum, BindingData: { Name: 'Name' } }",
			];

			yield return [
				new BindingInfo (new BindingTypeData<Class> ("Name")),
				"{ BindingType: Class, BindingData: { Name: 'Name', Flags: 'Default' } }"
			];

			yield return [
				new BindingInfo (new BindingTypeData<Class> ("Name", Class.DisableDefaultCtor)),
				"{ BindingType: Class, BindingData: { Name: 'Name', Flags: 'DisableDefaultCtor' } }"
			];

			yield return [
				new BindingInfo (new BindingTypeData<Protocol> ("Name")),
				"{ BindingType: Protocol, BindingData: { Name: 'Name', Flags: 'Default' } }"
			];

			yield return [
				new BindingInfo (new BindingTypeData<Category> ("Name")),
				"{ BindingType: Category, BindingData: { Name: 'Name', Flags: 'Default' } }"
			];
		}

		IEnumerator IEnumerable.GetEnumerator ()
			=> GetEnumerator ();
	}

	[Theory]
	[ClassData (typeof (TestDataToString))]
	void TestFieldDataToString (BindingInfo x, string expected)
		=> Assert.Equal (expected, x.ToString ());

	[Fact]
	void TestCasting ()
	{
		// failures
		Assert.Throws<InvalidCastException> (() =>
			(BindingTypeData<Class>) new BindingInfo (new BindingTypeData<Category> ("name")));
		Assert.Throws<InvalidCastException> (() =>
			(BindingTypeData<Protocol>) new BindingInfo (new BindingTypeData<Category> ("name")));
		Assert.Throws<InvalidCastException> (() =>
			(BindingTypeData<Category>) new BindingInfo (new BindingTypeData<Class> ("name")));
		
		var classBinding = new BindingTypeData<Class> ("Name");
		Assert.Equal (classBinding, (BindingTypeData<Class>) new BindingInfo (classBinding));
		var protocolBinding = new BindingTypeData<Protocol> ("Name");
		Assert.Equal (protocolBinding, (BindingTypeData<Protocol>) new BindingInfo (protocolBinding));	
		var categoryBinding = new BindingTypeData<Category> ("Name");
		Assert.Equal (categoryBinding, (BindingTypeData<Category>)new BindingInfo (categoryBinding));
	}
}
