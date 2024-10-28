using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Context;
using Microsoft.Macios.Generator.Extensions;
using ObjCBindings;

namespace Microsoft.Macios.Generator.Emitters;

using ExportedConstructor = ExportedMember<IMethodSymbol, ExportData<Constructor>>;

class ConstructorEmitter (SymbolBindingContext context) {

	public bool TryEmit (IEnumerable<ExportedConstructor> constructorInfo, TabbedStringBuilder classBlock)
	{
		var s = context.Symbol.Name;
		return true;
	}
}
