// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Microsoft.Macios.Generator.DataModel;
using Microsoft.Macios.Generator.IO;

namespace Microsoft.Macios.Generator.Context;

readonly struct BindingContext {

	/// <summary>
	/// Tabbed string builder that can be used to write the generated code.
	/// </summary>
	public TabbedStringBuilder Builder { get; }

	/// <summary>
	/// Current code changes for the binding context.
	/// </summary>
	public Binding Changes { get; }

	public BindingContext (TabbedStringBuilder builder, Binding changes)
	{
		Builder = builder;
		Changes = changes;
	}

}
