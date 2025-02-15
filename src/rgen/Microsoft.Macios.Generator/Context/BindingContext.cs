// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
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

	/// <summary>
	/// Dictionary that contains the names of the selectors that are being used in the generated code.
	/// </summary>
	public Dictionary<string, string> SelectorNames { get; } = new ();

	public BindingContext (TabbedStringBuilder builder, Binding changes)
	{
		Builder = builder;
		Changes = changes;
	}

}
