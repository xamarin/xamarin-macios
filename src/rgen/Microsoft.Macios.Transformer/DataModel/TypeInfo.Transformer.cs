// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Generator.DataModel;

readonly partial struct TypeInfo {

	internal TypeInfo (ITypeSymbol symbol) :
		this (
			symbol is IArrayTypeSymbol arrayTypeSymbol
				? arrayTypeSymbol.ElementType.ToDisplayString ()
				: symbol.ToDisplayString ().Trim ('?', '[', ']'),
			symbol.SpecialType)
	{
		throw new NotImplementedException ();
	}
}
