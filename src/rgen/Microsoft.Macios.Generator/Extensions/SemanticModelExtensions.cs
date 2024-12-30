using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.DataModel;

namespace Microsoft.Macios.Generator.Extensions;

static class SemanticModelExtensions {
	
	public static void GetSymbolData (ISymbol? symbol, 
		out string name, 
		out ImmutableArray<string> namespaces,
		out SymbolAvailability symbolAvailability)
	{
		name = symbol?.Name ?? string.Empty;
		var bucket = ImmutableArray.CreateBuilder<string> ();
		var ns = symbol?.ContainingNamespace;
		while (ns is not null) {
			if (!string.IsNullOrWhiteSpace (ns.Name))
				// prepend the namespace so that we can read from top to bottom
				bucket.Insert (0, ns.Name);
			ns = ns.ContainingNamespace;
		}

		symbolAvailability = symbol?.GetSupportedPlatforms () ?? new SymbolAvailability ();
		namespaces = bucket.ToImmutableArray ();
	}

	public static void GetSymbolData (this SemanticModel self, BaseTypeDeclarationSyntax declaration,
		BindingType bindingType,
		out string name,
		out ImmutableArray<string> namespaces,
		out SymbolAvailability symbolAvailability,
		out BindingData bindingData)
	{
		var symbol = self.GetDeclaredSymbol (declaration);
		GetSymbolData (symbol, out name, out namespaces, out symbolAvailability);
		if (symbol is null)
			bindingData = default;
		else {
			bindingData = bindingType switch {
				BindingType.Category => new BindingData(symbol.GetBindingData<ObjCBindings.Category> ()),
				BindingType.Class => new BindingData(symbol.GetBindingData<ObjCBindings.Class> ()),
				BindingType.Protocol => new BindingData(symbol.GetBindingData<ObjCBindings.Protocol> ()),
				BindingType.SmartEnum => new BindingData(BindingType.SmartEnum, symbol.GetBindingData ()),
				_ => default,
			};
		}
	}

}
