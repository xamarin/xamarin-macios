using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Context;

namespace Microsoft.Macios.Generator.Emitters;

/// <summary>
/// Returns the emitter that is related to the provided declaration type.
/// </summary>
static class EmitterFactory {
	public static bool TryCreate<T> (ISymbolBindingContext<T> context, TabbedStringBuilder builder,
		[NotNullWhen (true)] out ICodeEmitter? emitter)
		where T : BaseTypeDeclarationSyntax
	{
		emitter = context switch {
			ClassBindingContext classContext => new ClassEmitter (classContext, builder),
			ISymbolBindingContext<EnumDeclarationSyntax> enumContext => new EnumEmitter (enumContext, builder),
			ISymbolBindingContext<InterfaceDeclarationSyntax> interfaceContext => new InterfaceEmitter (interfaceContext, builder),
			_ => null
		};
		return emitter is not null;
	}
}
