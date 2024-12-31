using System;

namespace Microsoft.Macios.Generator;

/// <summary>
/// Contains all the names of the attributes that are used by the binding generator.
/// </summary>
static class AttributesNames {

	public const string BindingAttribute = "ObjCBindings.BindingTypeAttribute";
	public const string BindingCategoryAttribute = "ObjCBindings.BindingTypeAttribute<ObjCBindings.Category>";
	public const string BindingClassAttribute = "ObjCBindings.BindingTypeAttribute<ObjCBindings.Class>";
	public const string BindingProtocolAttribute = "ObjCBindings.BindingTypeAttribute<ObjCBindings.Protocol>";
	public const string FieldAttribute = "ObjCBindings.FieldAttribute";
	public const string EnumFieldAttribute = "ObjCBindings.FieldAttribute<ObjCBindings.EnumValue>";
	public const string ExportFieldAttribute = "ObjCBindings.ExportAttribute<ObjCBindings.Field>";
	public const string ExportPropertyAttribute = "ObjCBindings.ExportAttribute<ObjCBindings.Property>";
	public const string ExportMethodAttribute = "ObjCBindings.ExportAttribute<ObjCBindings.Method>";
	public const string SupportedOSPlatformAttribute = "System.Runtime.Versioning.SupportedOSPlatformAttribute";
	public const string UnsupportedOSPlatformAttribute = "System.Runtime.Versioning.UnsupportedOSPlatformAttribute";
	public const string ObsoletedOSPlatformAttribute = "System.Runtime.Versioning.ObsoletedOSPlatformAttribute";


	public static string? GetBindingTypeAttributeName<T> () where T : Enum
	{
		var type = typeof (T);
		if (type == typeof (ObjCBindings.Category)) {
			return BindingCategoryAttribute;
		}
		if (type == typeof (ObjCBindings.Class)) {
			return BindingClassAttribute;
		}
		if (type == typeof (ObjCBindings.Protocol)) {
			return BindingProtocolAttribute;
		}

		return null;
	}

	public static string? GetFieldAttributeName<T> () where T : Enum
	{
		// we cannot use a switch statement because typeof is not a constant value
		var type = typeof (T);
		if (type == typeof (ObjCBindings.Field)) {
			return ExportFieldAttribute;
		}
		if (type == typeof (ObjCBindings.Property)) {
			return ExportPropertyAttribute;
		}
		if (type == typeof (ObjCBindings.Method)) {
			return ExportMethodAttribute;
		}
		return null;
	}

}
