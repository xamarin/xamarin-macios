// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Availability;

namespace Microsoft.Macios.Generator.DataModel;

readonly partial struct Accessor {

	/// <summary>
	/// The data of the field attribute used to mark the value as a property binding. 
	/// </summary>
	public ExportData<ObjCBindings.Property>? ExportPropertyData { get; init; }

	public bool MarshalNativeExceptions
		=> ExportPropertyData is not null && ExportPropertyData.Value.Flags.HasFlag (ObjCBindings.Property.MarshalNativeExceptions);

	/// <summary>
	/// Create a new code change in a property accessor.
	/// </summary>
	/// <param name="accessorKind">The kind of accessor.</param>
	/// <param name="symbolAvailability">The os availability of the symbol.</param>
	/// <param name="exportPropertyData">The data of the export attribute found in the accessor.</param>
	/// <param name="attributes">The list of attributes attached to the accessor.</param>
	/// <param name="modifiers">The list of visibility modifiers of the accessor.</param>
	public Accessor (AccessorKind accessorKind,
		SymbolAvailability symbolAvailability,
		ExportData<ObjCBindings.Property>? exportPropertyData,
		ImmutableArray<AttributeCodeChange> attributes,
		ImmutableArray<SyntaxToken> modifiers)
	{
		Kind = accessorKind;
		SymbolAvailability = symbolAvailability;
		ExportPropertyData = exportPropertyData;
		Attributes = attributes;
		Modifiers = modifiers;
	}

	/// <summary>
	/// Retrieve the selector to be used with the associated property.
	/// </summary>
	/// <param name="associatedProperty">The property associated with the accessor.</param>
	/// <returns>The selector to use for the accessor.</returns>
	public string? GetSelector (in Property associatedProperty)
	{
		// this is not a property but a field, we cannot retrieve a selector.
		if (!associatedProperty.IsProperty)
			return null;

		// There are two possible cases, the current accessor has an export attribute, if that
		// is the case, we will use the selector in that attribute. Otherwise, we have:
		//
		// * getter: return the property selector.
		// * setter: use the registrar code (it has the right logic) to get the setter.
		if (ExportPropertyData is null) {
			return Kind == AccessorKind.Getter
				? associatedProperty.ExportPropertyData.Value.Selector
				: Registrar.Registrar.CreateSetterSelector (associatedProperty.ExportPropertyData.Value.Selector);
		}

		return ExportPropertyData.Value.Selector;
	}

	/// <summary>
	/// Returns if the accessor should marshal native exceptions with the associated property.
	/// </summary>
	/// <param name="property">The property associated with the accessor.</param>
	/// <returns>True if either the accessor or the property were marked with the MarshalNativeExceptions flag.</returns>
	public bool ShouldMarshalNativeExceptions (in Property property)
		=> MarshalNativeExceptions || property.MarshalNativeExceptions;

}
