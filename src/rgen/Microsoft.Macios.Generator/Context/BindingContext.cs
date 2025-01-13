// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Microsoft.Macios.Generator.DataModel;

namespace Microsoft.Macios.Generator.Context;

readonly struct BindingContext {

	/// <summary>
	/// The root context of the current binding operation.
	/// </summary>
	public RootBindingContext RootContext { get; }

	/// <summary>
	/// Tabbed string builder that can be used to write the generated code.
	/// </summary>
	public TabbedStringBuilder Builder { get; }

	/// <summary>
	/// Current code changes for the binding context.
	/// </summary>
	public CodeChanges Changes { get; }

	public BindingContext (RootBindingContext rootContext, TabbedStringBuilder builder, CodeChanges changes)
	{
		RootContext = rootContext;
		Builder = builder;
		Changes = changes;
	}

}
