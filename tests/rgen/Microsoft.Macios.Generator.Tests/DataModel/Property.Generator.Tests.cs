#pragma warning disable APL0003
using Microsoft.Macios.Generator.DataModel;
using ObjCRuntime;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class PropertyGeneratorTests {
	[Fact]
	public void GetExportDataUnknownKind ()
	{
		var property = new Property (
			name: "Name",
			type: "string",
			isBlittable: false,
			isSmartEnum: false,
			symbolAvailability: new(),
			attributes: [],
			modifiers: [],
			accessors: [
				new(
					accessorKind: AccessorKind.Remove,
					symbolAvailability: new(),
					exportPropertyData: new("myName"),
					attributes: [],
					modifiers: []
				)
			])
		{
			ExportPropertyData = new("name")
		};
		Assert.Null (property.GetExportData (property.Accessors[0]));
	}

	[Fact]
	public void GetExportDataPropertyNullGetterNull ()
	{
		var property = new Property (
			name: "Name",
			type: "string",
			isBlittable: false,
			isSmartEnum: false,
			symbolAvailability: new(),
			attributes: [],
			modifiers: [],
			accessors: [
				new(
					accessorKind: AccessorKind.Getter,
					symbolAvailability: new(),
					exportPropertyData: new("myName"),
					attributes: [],
					modifiers: []
				)
			])
		{
			ExportPropertyData = null
		};
		
		Assert.Null (property.GetExportData (property.Accessors[0]));
	}

	[Fact]
	public void GetExportDataPropertyGetterNull ()
	{
		var property = new Property (
			name: "Name",
			type: "string",
			isBlittable: false,
			isSmartEnum: false,
			symbolAvailability: new(),
			attributes: [],
			modifiers: [],
			accessors: [
				new(
					accessorKind: AccessorKind.Getter,
					symbolAvailability: new(),
					exportPropertyData: null,
					attributes: [],
					modifiers: []
				)
			])
		{
			ExportPropertyData = new("name")
		};
		
		Assert.Equal (property.ExportPropertyData, property.GetExportData (property.Accessors[0]));
	}

	[Fact]
	public void GetExportDataPropertyGetter ()
	{
		var property = new Property (
			name: "Name",
			type: "string",
			isBlittable: false,
			isSmartEnum: false,
			symbolAvailability: new(),
			attributes: [],
			modifiers: [],
			accessors: [
				new(
					accessorKind: AccessorKind.Getter,
					symbolAvailability: new(),
					exportPropertyData: new("myName"),
					attributes: [],
					modifiers: []
				)
			])
		{
			ExportPropertyData = new("name")
		};
		
		Assert.Equal (property.Accessors[0].ExportPropertyData, property.GetExportData (property.Accessors[0]));
	}

	[Theory]
	[InlineData(ArgumentSemantic.None, ObjCBindings.Property.Default)]
	[InlineData(ArgumentSemantic.Assign, ObjCBindings.Property.Default)]
	[InlineData(ArgumentSemantic.Copy, ObjCBindings.Property.Default)]
	[InlineData(ArgumentSemantic.Retain, ObjCBindings.Property.Default)]
	[InlineData(ArgumentSemantic.Weak, ObjCBindings.Property.Default)]
	public void GetExportDataPropertySetterNull (ArgumentSemantic semantic, ObjCBindings.Property flags)
	{
		var property = new Property (
			name: "Name",
			type: "string",
			isBlittable: false,
			isSmartEnum: false,
			symbolAvailability: new(),
			attributes: [],
			modifiers: [],
			accessors: [
				new(
					accessorKind: AccessorKind.Setter,
					symbolAvailability: new(),
					exportPropertyData: null,
					attributes: [],
					modifiers: []
				)
			])
		{
			ExportPropertyData = new("name", semantic, flags)
		};
		
		var exportPropertyData = property.GetExportData (property.Accessors[0]);
		Assert.NotNull (exportPropertyData);
		// the flags and the argument semantic have to be the same as the property ones
		Assert.Equal (property.ExportPropertyData.Value.ArgumentSemantic, exportPropertyData.Value.ArgumentSemantic);
		Assert.Equal (property.ExportPropertyData.Value.Flags, exportPropertyData.Value.Flags);
		// setter must be the default setter one from ObjC
		Assert.Equal ("setName:", exportPropertyData.Value.Selector);
	}

	[Fact]
	public void GetExportDataPropertySetter ()
	{
		var property = new Property (
			name: "Name",
			type: "string",
			isBlittable: false,
			isSmartEnum: false,
			symbolAvailability: new(),
			attributes: [],
			modifiers: [],
			accessors: [
				new(
					accessorKind: AccessorKind.Setter,
					symbolAvailability: new(),
					exportPropertyData: new("specialSetMyName"),
					attributes: [],
					modifiers: []
				)
			])
		{
			ExportPropertyData = new("name")
		};
		
		// should be the one present in the setter
		Assert.Equal (property.Accessors[0].ExportPropertyData, property.GetExportData (property.Accessors[0]));
	}
}
