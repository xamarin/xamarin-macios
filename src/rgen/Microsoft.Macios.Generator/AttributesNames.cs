using System.Collections.Generic;

namespace Microsoft.Macios.Generator;

/// <summary>
/// Contains all the names of the attributes that are used by the binding generator.
/// </summary>
public static class AttributesNames {
	public const string BindingAttribute = "ObjCBindings.BindingTypeAttribute";
	public const string EnumFieldAttribute = "ObjCBindings.FieldAttribute<ObjCBindings.EnumValue>";
	public const string ConstructorAttribute = "ObjCBindings.ExportAttribute<ObjCBindings.Constructor>";
	public const string FieldAttribute = "ObjCBindings.ExportAttribute<ObjCBindings.Field>";
	public const string MethodAttribute = "ObjCBindings.ExportAttribute<ObjCBindings.Method>";
	public const string PropertyAttribute = "ObjCBindings.ExportAttribute<ObjCBindings.Property>";

	public static readonly HashSet<string> MethodAttributes =
		[ConstructorAttribute, FieldAttribute, MethodAttribute, PropertyAttribute];
}
