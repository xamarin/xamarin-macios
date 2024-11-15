namespace Microsoft.Macios.Generator.DataModel;

/// <summary>
/// Enum that represents the binding type needed for an class/interface/enum. This allows the code generator
/// differentiate between a code changes that has the exact same qualified name but different type.
/// </summary>
enum BindingType {
	/// <summary>
	/// Unknown binding type.
	/// </summary>
	Unknown = 0,
	/// <summary>
	/// Binding type for a enum with backing fields.
	/// </summary>
	SmartEnum,
}
