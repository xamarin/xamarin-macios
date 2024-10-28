using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Context;
using Microsoft.Macios.Generator.Extensions;
using ObjCBindings;

namespace Microsoft.Macios.Generator.Emitters;

using ExportedMethod = ExportedMember<IMethodSymbol, ExportData<Method>>;

class MethodEmitter (SymbolBindingContext context) {

	public bool TryEmit (IEnumerable<ExportedMethod> methodInfo, TabbedStringBuilder classBlock)
	{
		var s = context.Symbol.Name;
		return true;
	}

}
