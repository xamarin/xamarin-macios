using Microsoft.Macios.Generator.Attributes;

namespace Microsoft.Macios.Generator.DataModel;

// Partial struct implementation so that other projects that use the data model do not need to reference the
// registrar.core if not needed.
readonly partial struct Property {
	
	public ExportData<ObjCBindings.Property>? GetExportData (in Accessor accessor)
	{
		if (!IsProperty)
			return null;
		
		// make a decision based on the type of accessor:
		switch (accessor.Kind) {
		case AccessorKind.Getter:
			// Simplest case. If there is export data, return the export data in it, else the current one.
			return accessor.ExportPropertyData ?? ExportPropertyData;
		case AccessorKind.Setter:
			// if the user defined an export attr in the getter, return the value in it
			if (accessor.ExportPropertyData is not null)
				return accessor.ExportPropertyData;
			// create an attribute with a setter version provided by the registrar
			var setterSelector = Registrar.Registrar.CreateSetterSelector (ExportPropertyData.Value.Selector);
			return new (
				selector: setterSelector, 
				argumentSemantic: ExportPropertyData.Value.ArgumentSemantic,
				flags: ExportPropertyData.Value.Flags);
		default:
			return null;
		}
	}
}
