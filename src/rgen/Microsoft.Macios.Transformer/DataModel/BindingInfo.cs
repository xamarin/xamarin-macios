// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Macios.Generator.DataModel;
using Microsoft.Macios.Transformer.Attributes;

namespace Microsoft.Macios.Transformer.DataModel;

/// <summary>
/// This struct works as a union to store the possible BindingTypeData that can be present in the bindings.
/// </summary>
readonly record struct BindingInfo {
	public BaseTypeData? BaseTypeData { get; init; }
	public BindingType BindingType { get; init; }

	public BindingInfo (BaseTypeData? baseTypeData, BindingType bindingType)
	{
		// we have to calculate the type of the binding based on the attributes that the user provided
		BaseTypeData = baseTypeData;
		BindingType = bindingType;
	}
}
