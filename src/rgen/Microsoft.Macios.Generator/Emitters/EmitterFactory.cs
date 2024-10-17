using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Context;

namespace Microsoft.Macios.Generator.Emitters;

/// <summary>
/// Returns the emitter that is related to the provided declaration type.
/// </summary>
static class EmitterFactory {
	public static bool TryCreate (SymbolBindingContext context, TabbedStringBuilder builder,
		[NotNullWhen (true)] out ICodeEmitter? emitter)
	{
		emitter = context.DeclarationSyntax switch {
			ClassDeclarationSyntax => new ClassEmitter (context, builder),
			EnumDeclarationSyntax => new EnumEmitter (context, builder),
			InterfaceDeclarationSyntax => new InterfaceEmitter (context, builder),
			_ => null
		};
		return emitter is not null;
	}
}
