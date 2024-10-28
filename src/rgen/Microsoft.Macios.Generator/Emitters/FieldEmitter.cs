using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Context;
using Microsoft.Macios.Generator.Extensions;
using ObjCBindings;

namespace Microsoft.Macios.Generator.Emitters;

using ExportedField = ExportedMember<IPropertySymbol, FieldData<Field>>;

class FieldEmitter (SymbolBindingContext context) {


	public bool TryEmit (IEnumerable<ExportedField> fieldInfo, TabbedStringBuilder classBlock)
	{
		var s = context.Symbol.Name;
		return true;
	}

}
