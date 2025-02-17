// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Microsoft.Macios.Generator.DataModel;

/// <summary>
/// Enum that represents the binding type needed for an class/interface/enum. This allows the code generator
/// to differentiate between code changes that have the exact same qualified name but different type.
/// </summary>
enum BindingType : ulong {
	/// <summary>
	/// Unknown binding type.
	/// </summary>
	Unknown = 0,
	/// <summary>
	/// Binding type for an objc category.
	/// </summary>
	Category,
	/// <summary>
	/// Binding type for an objc class.
	/// </summary>
	Class,
	/// <summary>
	/// Binding type for an objc protocol.
	/// </summary>
	Protocol,
	/// <summary>
	/// Binding type for a enum with backing fields.
	/// </summary>
	SmartEnum,
	/// <summary>
	/// Binding type for a dictionary with strong value.
	/// </summary>
	StrongDictionary,
	/// <summary>
	/// Binding type for a core image filter.
	/// </summary>
	CoreImageFilter,
}

