// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.DataModel;

namespace Microsoft.Macios.Generator.Extensions;

static partial class SemanticModelExtensions {

	public static void GetSymbolData (this SemanticModel self, BaseTypeDeclarationSyntax declaration,
		BindingType bindingType,
		out string name,
		out string? baseClass,
		out ImmutableArray<string> interfaces,
		out ImmutableArray<string> namespaces,
		out SymbolAvailability symbolAvailability,
		out BindingInfo bindingInfo)
	{
		var symbol = self.GetDeclaredSymbol (declaration);
		GetSymbolData (symbol, out name, out baseClass, out interfaces, out namespaces, out symbolAvailability);
		if (symbol is null)
			bindingInfo = default;
		else {
			bindingInfo = bindingType switch {
				BindingType.Category => new BindingInfo (symbol.GetBindingData<ObjCBindings.Category> ()),
				BindingType.Class => new BindingInfo (symbol.GetBindingData<ObjCBindings.Class> ()),
				BindingType.Protocol => new BindingInfo (symbol.GetBindingData<ObjCBindings.Protocol> ()),
				BindingType.SmartEnum => new BindingInfo (BindingType.SmartEnum, symbol.GetBindingData ()),
				_ => default,
			};
		}
	}

}
