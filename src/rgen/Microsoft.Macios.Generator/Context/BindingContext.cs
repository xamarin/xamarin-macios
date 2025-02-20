// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Microsoft.Macios.Generator.DataModel;
using Microsoft.Macios.Generator.Extensions;
using Microsoft.Macios.Generator.IO;

namespace Microsoft.Macios.Generator.Context;

readonly struct BindingContext {

	/// <summary>
	/// The root context of the current code generation.
	/// </summary>
	public RootContext RootContext { get; }

	/// <summary>
	/// Tabbed string builder that can be used to write the generated code.
	/// </summary>
	public TabbedStringBuilder Builder { get; }

	/// <summary>
	/// Current code changes for the binding context.
	/// </summary>
	public Binding Changes { get; }

	/// <summary>
	/// True if we need to insert thread checks in the generated methods/properties.
	/// </summary>
	public bool NeedsThreadChecks { get; init; }

	/// <summary>
	/// Dictionary that contains the names of the selectors that are being used in the generated code.
	/// </summary>
	public Dictionary<string, string> SelectorNames { get; } = new ();

	public BindingContext (RootContext rootContext, TabbedStringBuilder builder, Binding changes)
	{
		RootContext = rootContext;
		Builder = builder;
		Changes = changes;

		// calculate if the current binding needs to have a thread check
		if (Changes.BindingInfo.IsThreadSafe) {
			NeedsThreadChecks = false;
		} else {
			// the thread check is needed if the current namespace is one of the UI namespaces for the compilation
			var uiNamespaces = RootContext.Compilation.GetUINamespaces ();
			var ns = string.Join ('.', Changes.Namespace);
			NeedsThreadChecks = uiNamespaces.Contains (ns);
		}
	}

}
