using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.Extensions;
using ObjCBindings;

namespace Microsoft.Macios.Generator.DataModel;

/// <summary>
/// Readonly struct that represent the changes that a user has made in a property.
/// </summary>
readonly struct Property : IEquatable<Property> {
	/// <summary>
	/// Name of the property.
	/// </summary>
	public string Name { get; } = string.Empty;

	/// <summary>
	/// String representation of the property type.
	/// </summary>
	public string Type { get; } = string.Empty;

	/// <summary>
	/// Returns if the property type is bittable.
	/// </summary>
	public bool IsBlittable { get; }

	/// <summary>
	/// Returns if the property type is a smart enum.
	/// </summary>
	public bool IsSmartEnum { get; }

	/// <summary>
	/// Returns if the property type is a reference type.
	/// </summary>
	public bool IsReferenceType { get; }

	/// <summary>
	/// The platform availability of the property.
	/// </summary>
	public SymbolAvailability SymbolAvailability { get; }

	/// <summary>
	/// The data of the field attribute used to mark the value as a field binding. 
	/// </summary>
	public ExportData<Field>? ExportFieldData { get; init; }

	/// <summary>
	/// True if the property represents a Objc field.
	/// </summary>
	public bool IsField => ExportFieldData is not null;

	/// <summary>
	/// The data of the field attribute used to mark the value as a property binding. 
	/// </summary>
	public ExportData<ObjCBindings.Property>? ExportPropertyData { get; init; }

	/// <summary>
	/// True if the property represents a Objc property.
	/// </summary>
	public bool IsProperty => ExportPropertyData is not null;

	/// <summary>
	/// Get the attributes added to the member.
	/// </summary>
	public ImmutableArray<AttributeCodeChange> Attributes { get; } = [];

	/// <summary>
	/// Get the modifiers of the property.
	/// </summary>
	public ImmutableArray<SyntaxToken> Modifiers { get; } = [];

	/// <summary>
	/// Get the list of accessor changes of the property.
	/// </summary>
	public ImmutableArray<Accessor> Accessors { get; } = [];

	internal Property (string name, string type,
		bool isBlittable,
		bool isSmartEnum,
		bool isReferenceType,
		SymbolAvailability symbolAvailability,
		ImmutableArray<AttributeCodeChange> attributes,
		ImmutableArray<SyntaxToken> modifiers, ImmutableArray<Accessor> accessors)
	{
		Name = name;
		Type = type;
		IsBlittable = isBlittable;
		IsSmartEnum = isSmartEnum;
		IsReferenceType = isReferenceType;
		SymbolAvailability = symbolAvailability;
		Attributes = attributes;
		Modifiers = modifiers;
		Accessors = accessors;
	}

	/// <inheritdoc />
	public bool Equals (Property other)
	{
		// this could be a large && but ifs are more readable
		if (Name != other.Name)
			return false;
		if (Type != other.Type)
			return false;
		if (IsBlittable != other.IsBlittable)
			return false;
		if (IsSmartEnum != other.IsSmartEnum)
			return false;
		if (IsReferenceType != other.IsReferenceType)
			return false;
		if (SymbolAvailability != other.SymbolAvailability)
			return false;
		if (ExportFieldData != other.ExportFieldData)
			return false;
		if (ExportPropertyData != other.ExportPropertyData)
			return false;

		var attrsComparer = new AttributesEqualityComparer ();
		if (!attrsComparer.Equals (Attributes, other.Attributes))
			return false;

		var modifiersComparer = new ModifiersEqualityComparer ();
		if (!modifiersComparer.Equals (Modifiers, other.Modifiers))
			return false;

		var accessorComparer = new AccessorsEqualityComparer ();
		return accessorComparer.Equals (Accessors, other.Accessors);
	}

	/// <inheritdoc />
	public override bool Equals (object? obj)
	{
		return obj is Property other && Equals (other);
	}

	/// <inheritdoc />
	public override int GetHashCode ()
	{
		return HashCode.Combine (Name, Type, IsSmartEnum, Attributes, Modifiers, Accessors);
	}

	public static bool operator == (Property left, Property right)
	{
		return left.Equals (right);
	}

	public static bool operator != (Property left, Property right)
	{
		return !left.Equals (right);
	}

	public static bool TryCreate (PropertyDeclarationSyntax declaration, SemanticModel semanticModel,
		[NotNullWhen (true)] out Property? change)
	{
		var memberName = declaration.Identifier.ToFullString ().Trim ();
		// get the symbol from the property declaration
		if (semanticModel.GetDeclaredSymbol (declaration) is not IPropertySymbol propertySymbol) {
			change = null;
			return false;
		}

		var propertySupportedPlatforms = propertySymbol.GetSupportedPlatforms ();

		var type = propertySymbol.Type.ToDisplayString ().Trim ();
		var attributes = declaration.GetAttributeCodeChanges (semanticModel);
		ImmutableArray<Accessor> accessorCodeChanges = [];
		if (declaration.AccessorList is not null && declaration.AccessorList.Accessors.Count > 0) {
			// calculate any possible changes in the accessors of the property
			var accessorsBucket = ImmutableArray.CreateBuilder<Accessor> ();
			foreach (var accessorDeclaration in declaration.AccessorList.Accessors) {
				if (semanticModel.GetDeclaredSymbol (accessorDeclaration) is not ISymbol accessorSymbol)
					continue;
				var kind = accessorDeclaration.Kind ().ToAccessorKind ();
				var accessorAttributeChanges = accessorDeclaration.GetAttributeCodeChanges (semanticModel);
				accessorsBucket.Add (new (kind, accessorSymbol.GetSupportedPlatforms (), accessorAttributeChanges,
					[.. accessorDeclaration.Modifiers]));
			}

			accessorCodeChanges = accessorsBucket.ToImmutable ();
		}

		if (declaration.ExpressionBody is not null) {
			// an expression body == a getter with no attrs or modifiers; that means that the accessor does not have
			// extra availability, but the ones form the property
			accessorCodeChanges = [
				new (AccessorKind.Getter, propertySupportedPlatforms, [], [])
			];
		}

		change = new (
			name: memberName,
			type: type,
			isBlittable: propertySymbol.Type.IsBlittable (),
			isSmartEnum: propertySymbol.Type.IsSmartEnum (),
			isReferenceType: propertySymbol.Type.IsReferenceType,
			symbolAvailability: propertySupportedPlatforms,
			attributes: attributes,
			modifiers: [.. declaration.Modifiers],
			accessors: accessorCodeChanges) {
			ExportFieldData = propertySymbol.GetExportData<Field> (),
			ExportPropertyData = propertySymbol.GetExportData<ObjCBindings.Property> (),
		};
		return true;
	}

	/// <inheritdoc />
	public override string ToString ()
	{
		var sb = new StringBuilder (
			$"Name: '{Name}', Type: '{Type}', IsBlittable: {IsBlittable}, IsSmartEnum: {IsSmartEnum}, IsReferenceType: {IsReferenceType} Supported Platforms: {SymbolAvailability}, ExportFieldData: '{ExportFieldData?.ToString () ?? "null"}', ExportPropertyData: '{ExportPropertyData?.ToString () ?? "null"}' Attributes: [");
		sb.AppendJoin (",", Attributes);
		sb.Append ("], Modifiers: [");
		sb.AppendJoin (",", Modifiers.Select (x => x.Text));
		sb.Append ("], Accessors: [");
		sb.AppendJoin (",", Accessors);
		sb.Append (']');
		return sb.ToString ();
	}
}
