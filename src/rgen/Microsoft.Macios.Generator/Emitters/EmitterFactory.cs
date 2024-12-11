using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Context;
using Microsoft.Macios.Generator.DataModel;

namespace Microsoft.Macios.Generator.Emitters;

/// <summary>
/// Returns the emitter that is related to the provided declaration type.
/// </summary>
static class EmitterFactory {
	public static bool TryCreate (CodeChanges changes, RootBindingContext context, SemanticModel semanticModel,
		INamedTypeSymbol symbol, TabbedStringBuilder builder,
		[NotNullWhen (true)] out ICodeEmitter? emitter)
	{
		switch (changes.BindingType) {
		case BindingType.Class: {
			var ctx = new ClassBindingContext (context, semanticModel, symbol);
			emitter = new ClassEmitter (ctx, builder);
			break;
		}
		case BindingType.SmartEnum: {
			var ctx = new SymbolBindingContext (context, semanticModel, symbol);
			emitter = new EnumEmitter (ctx, builder);
			break;
		}
		case BindingType.Protocol: {
			var ctx = new SymbolBindingContext (context, semanticModel, symbol);
			emitter = new InterfaceEmitter (ctx, builder);
			break;
		}
		default:
			emitter = null;
			break;
		}

		return emitter is not null;
	}
}
