// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Macios.Generator.DataModel;

readonly partial struct Binding {

	/// <summary>
	/// Represents the type of binding that the code changes are for.
	/// </summary>
	public BindingType BindingType => throw new NotImplementedException ();

	public BindingInfo BindingInfo => throw new NotImplementedException ();

	public Binding ()
	{
		FullyQualifiedSymbol = "";
		IsStatic = false;
		IsPartial = false;
		IsAbstract = false;
	}
}
