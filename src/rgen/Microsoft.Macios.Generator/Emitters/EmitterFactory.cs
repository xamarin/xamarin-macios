using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Context;
using Microsoft.Macios.Generator.DataModel;

namespace Microsoft.Macios.Generator.Emitters;

/// <summary>
/// Returns the emitter that is related to the provided declaration type.
/// </summary>
static class EmitterFactory {
	public static bool TryCreate (CodeChanges changes, RootBindingContext context, TabbedStringBuilder builder,
		[NotNullWhen (true)] out ICodeEmitter? emitter)
	{
		emitter = changes.BindingType switch {
			BindingType.Class => new ClassEmitter (context, builder),
			BindingType.SmartEnum => new EnumEmitter (context, builder),
			BindingType.Protocol => new InterfaceEmitter (context, builder),
			_ => null
		};
		return emitter is not null;
	}
}
