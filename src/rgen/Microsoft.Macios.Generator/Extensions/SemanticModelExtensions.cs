using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.DataModel;

namespace Microsoft.Macios.Generator.Extensions;

static class SemanticModelExtensions {

	public static void GetSymbolData (ISymbol? symbol,
		out string name,
		out string? baseClass,
		out ImmutableArray<string> interfaces,
		out ImmutableArray<string> namespaces,
		out SymbolAvailability symbolAvailability)
	{
		name = symbol?.Name ?? string.Empty;
		baseClass = null;
		var interfacesBucket = ImmutableArray.CreateBuilder<string> ();
		var bucket = ImmutableArray.CreateBuilder<string> ();
		var ns = symbol?.ContainingNamespace;
		while (ns is not null) {
			if (!string.IsNullOrWhiteSpace (ns.Name))
				// prepend the namespace so that we can read from top to bottom
				bucket.Insert (0, ns.Name);
			ns = ns.ContainingNamespace;
		}

		if (symbol is INamedTypeSymbol namedTypeSymbol) {
			baseClass = namedTypeSymbol.BaseType?.ToDisplayString ().Trim ();
			foreach (var symbolInterface in namedTypeSymbol.Interfaces) {
				interfacesBucket.Add (symbolInterface.ToDisplayString ().Trim ());
			}
		}
		symbolAvailability = symbol?.GetSupportedPlatforms () ?? new SymbolAvailability ();
		interfaces = interfacesBucket.ToImmutable ();
		namespaces = bucket.ToImmutableArray ();
	}

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
