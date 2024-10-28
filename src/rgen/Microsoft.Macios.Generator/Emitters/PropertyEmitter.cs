using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Context;
using Microsoft.Macios.Generator.Extensions;
using ObjCBindings;

namespace Microsoft.Macios.Generator.Emitters;

using ExportedProperty = ExportedMember<IPropertySymbol, ExportData<Property>>;

class PropertyEmitter (SymbolBindingContext context) {
	public bool TryEmit (IEnumerable<ExportedProperty> propertyInfo, TabbedStringBuilder classBlock)
	{
		var name = context.GetUniqueVariableName ("test");
		return true;
	}
}
