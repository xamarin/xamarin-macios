// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Macios.Transformer.Attributes;

namespace Microsoft.Macios.Generator.DataModel;

readonly partial struct Parameter {

	/// <summary>
	/// Returns the bind from data if present in the binding.
	/// </summary>
	public BindAsData? BindAs => BindAsAttribute;
}
