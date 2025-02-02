// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.Extensions;
using Microsoft.Macios.Transformer.Attributes;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Microsoft.Macios.Generator.DataModel;

readonly partial struct Property {

	readonly FieldData? overrideFieldData;

	/// <summary>
	/// The data of the field attribute used to mark the value as a field binding. 
	/// </summary>
	public FieldData? ExportFieldData {
		get => overrideFieldData ?? FieldAttribute;
		init => overrideFieldData = value;
	}

	/// <summary>
	/// True if the property represents a Objc field.
	/// </summary>
	[MemberNotNullWhen (true, nameof (ExportFieldData))]
	public bool IsField => ExportFieldData is not null;

	/// <summary>
	/// Returns if the field was marked as a notification.
	/// </summary>
	public bool IsNotification => HasNotificationAttribute;


	readonly ExportData? overrideExportData;

	/// <summary>
	/// The data of the field attribute used to mark the value as a property binding. 
	/// </summary>
	public ExportData? ExportPropertyData {
		get => overrideExportData ?? ExportAttribute;
		init => overrideExportData = value;
	}

	/// <summary>
	/// True if the property represents a Objc property.
	/// </summary>
	[MemberNotNullWhen (true, nameof (ExportPropertyData))]
	public bool IsProperty => ExportPropertyData is not null;

	/// <summary>
	/// True if the method was exported with the MarshalNativeExceptions flag allowing it to support native exceptions.
	/// </summary>
	public bool MarshalNativeExceptions => HasMarshalNativeExceptionsFlag;

	/// <summary>
	/// True if the property should be generated without a backing field.
	/// </summary>
	public bool IsTransient => IsProperty && HasTransientFlag;

	/// <summary>
	/// Returns the bind from data if present in the binding.
	/// </summary>
	public BindAsData? BindAs => BindAsAttribute;

	/// <inheritdoc />
	public bool Equals (Property other) => CoreEquals (other);

	internal Property (string name,
		TypeInfo returnType,
		SymbolAvailability symbolAvailability,
		Dictionary<string, List<AttributeData>> attributes,
		ImmutableArray<Accessor> accessors)
	{
		Name = name;
		BackingField = $"_{Name}";
		ReturnType = returnType;
		SymbolAvailability = symbolAvailability;
		AttributesDictionary = attributes;
		Accessors = accessors;

		// the modifiers depend on whether we are talking about a field or a property. If we are talking
		// about a field, we always have the same modifiers, public static. With a property we need to 
		// respect the flags that are set in the attributes.
		if (IsField) {
			// public static partial
			Modifiers = [Token (SyntaxKind.PublicKeyword), Token (SyntaxKind.StaticKeyword), Token (SyntaxKind.PartialKeyword)];
		} else {
			// create a helper struct to retrieve the modifiers
			var flags = new ModifiersFlags (HasAbstractFlag, HasInternalFlag, HasNewFlag, HasOverrideFlag, HasStaticFlag);
			Modifiers = flags.ToModifiersArray ();
		}
	}

	public static bool TryCreate (PropertyDeclarationSyntax declaration, SemanticModel semanticModel,
		[NotNullWhen (true)] out Property? property)
	{
		var memberName = declaration.Identifier.ToFullString ().Trim ();
		// get the symbol from the property declaration
		if (semanticModel.GetDeclaredSymbol (declaration) is not IPropertySymbol propertySymbol) {
			property = null;
			return false;
		}

		var propertySupportedPlatforms = propertySymbol.GetAvailabilityForSymbol ();
		ImmutableArray<Accessor> accessors = [];
		if (declaration.AccessorList is not null && declaration.AccessorList.Accessors.Count > 0) {
			// calculate any possible changes in the accessors of the property
			var accessorsBucket = ImmutableArray.CreateBuilder<Accessor> ();
			foreach (var accessorDeclaration in declaration.AccessorList.Accessors) {
				if (semanticModel.GetDeclaredSymbol (accessorDeclaration) is not ISymbol accessorSymbol)
					continue;
				var kind = accessorDeclaration.Kind ().ToAccessorKind ();
				accessorsBucket.Add (new (
					accessorKind: kind,
					// just for the current symbol, not the parents etc..
					symbolAvailability: accessorSymbol.GetAvailabilityForSymbol (),
					attributes: accessorSymbol.GetAttributeData ()
				));
			}

			accessors = accessorsBucket.ToImmutable ();
		}

		var propertyAttributes = propertySymbol.GetAttributeData ();
		property = new (
			name: memberName,
			returnType: new (propertySymbol.Type, propertyAttributes),
			symbolAvailability: propertySupportedPlatforms,
			attributes: propertyAttributes,
			accessors: accessors);
		return true;
	}

	/// <inheritdoc />
	public override string ToString ()
	{
		var sb = new StringBuilder ($"Name: '{Name}', Type: {ReturnType}, ");
		sb.Append ($"Supported Platforms: {SymbolAvailability}, ");
		sb.Append ($"ExportFieldData: '{ExportFieldData?.ToString () ?? "null"}', ");
		sb.Append ($"ExportPropertyData: '{ExportPropertyData?.ToString () ?? "null"}' Attributes: [");
		sb.AppendJoin (",", Attributes);
		sb.Append ("], Modifiers: [");
		sb.AppendJoin (",", Modifiers.Select (x => x.Text));
		sb.Append ("], Accessors: [");
		sb.AppendJoin (",", Accessors);
		sb.Append (']');
		return sb.ToString ();
	}
}
