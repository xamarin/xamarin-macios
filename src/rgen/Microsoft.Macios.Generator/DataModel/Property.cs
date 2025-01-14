// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.Context;
using Microsoft.Macios.Generator.Extensions;

namespace Microsoft.Macios.Generator.DataModel;

/// <summary>
/// Readonly struct that represent the changes that a user has made in a property.
/// </summary>
readonly struct Property : IEquatable<Property> {
	/// <summary>
	/// Name of the property.
	/// </summary>
	public string Name { get; } = string.Empty;

	public string BackingField { get; private init; }

	/// <summary>
	/// Representation of the property type.
	/// </summary>
	public ReturnType ReturnType { get; } = default;

	/// <summary>
	/// Returns if the property type is bittable.
	/// </summary>
	public bool IsBlittable => ReturnType.IsBlittable;

	/// <summary>
	/// Returns if the property type is a smart enum.
	/// </summary>
	public bool IsSmartEnum => ReturnType.IsSmartEnum;

	/// <summary>
	/// Returns if the property type is a reference type.
	/// </summary>
	public bool IsReferenceType => ReturnType.IsReferenceType;

	/// <summary>
	/// The platform availability of the property.
	/// </summary>
	public SymbolAvailability SymbolAvailability { get; }

	/// <summary>
	/// The data of the field attribute used to mark the value as a field binding. 
	/// </summary>
	public FieldInfo<ObjCBindings.Property>? ExportFieldData { get; init; }

	/// <summary>
	/// True if the property represents a Objc field.
	/// </summary>
	[MemberNotNullWhen (true, nameof (ExportFieldData))]
	public bool IsField => ExportFieldData is not null;

	public bool IsNotification
		=> IsField && ExportFieldData.Value.FieldData.Flags.HasFlag (ObjCBindings.Property.Notification);

	/// <summary>
	/// The data of the field attribute used to mark the value as a property binding. 
	/// </summary>
	public ExportData<ObjCBindings.Property>? ExportPropertyData { get; init; }

	/// <summary>
	/// True if the property represents a Objc property.
	/// </summary>
	[MemberNotNullWhen (true, nameof (ExportPropertyData))]
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

	internal Property (string name, ReturnType returnType,
		SymbolAvailability symbolAvailability,
		ImmutableArray<AttributeCodeChange> attributes,
		ImmutableArray<SyntaxToken> modifiers, ImmutableArray<Accessor> accessors)
	{
		Name = name;
		BackingField = $"_{Name}";
		ReturnType = returnType;
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
		if (ReturnType != other.ReturnType)
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
		return HashCode.Combine (Name, ReturnType, IsSmartEnum, Attributes, Modifiers, Accessors);
	}

	public static bool operator == (Property left, Property right)
	{
		return left.Equals (right);
	}

	public static bool operator != (Property left, Property right)
	{
		return !left.Equals (right);
	}

	static FieldInfo<ObjCBindings.Property>? GetFieldInfo (RootBindingContext context, IPropertySymbol propertySymbol)
	{
		// grab the last port of the namespace
		var ns = propertySymbol.ContainingNamespace.Name.Split ('.') [^1];
		var fieldData = propertySymbol.GetFieldData<ObjCBindings.Property> ();
		FieldInfo<ObjCBindings.Property>? fieldInfo = null;
		if (fieldData is not null && context.TryComputeLibraryName (fieldData.Value.LibraryName, ns,
				out string? libraryName, out string? libraryPath)) {
			fieldInfo = new FieldInfo<ObjCBindings.Property> (fieldData.Value, libraryName, libraryPath);
		}

		return fieldInfo;
	}

	public static bool TryCreate (PropertyDeclarationSyntax declaration, RootBindingContext context,
		[NotNullWhen (true)] out Property? change)
	{
		var memberName = declaration.Identifier.ToFullString ().Trim ();
		// get the symbol from the property declaration
		if (context.SemanticModel.GetDeclaredSymbol (declaration) is not IPropertySymbol propertySymbol) {
			change = null;
			return false;
		}

		var propertySupportedPlatforms = propertySymbol.GetSupportedPlatforms ();
		var attributes = declaration.GetAttributeCodeChanges (context.SemanticModel);

		ImmutableArray<Accessor> accessorCodeChanges = [];
		if (declaration.AccessorList is not null && declaration.AccessorList.Accessors.Count > 0) {
			// calculate any possible changes in the accessors of the property
			var accessorsBucket = ImmutableArray.CreateBuilder<Accessor> ();
			foreach (var accessorDeclaration in declaration.AccessorList.Accessors) {
				if (context.SemanticModel.GetDeclaredSymbol (accessorDeclaration) is not ISymbol accessorSymbol)
					continue;
				var kind = accessorDeclaration.Kind ().ToAccessorKind ();
				var accessorAttributeChanges =
					accessorDeclaration.GetAttributeCodeChanges (context.SemanticModel);
				accessorsBucket.Add (new (
					accessorKind: kind,
					exportPropertyData: accessorSymbol.GetExportData<ObjCBindings.Property> (),
					symbolAvailability: accessorSymbol.GetSupportedPlatforms (),
					attributes: accessorAttributeChanges,
					modifiers: [.. accessorDeclaration.Modifiers]));
			}

			accessorCodeChanges = accessorsBucket.ToImmutable ();
		}

		if (declaration.ExpressionBody is not null) {
			// an expression body == a getter with no attrs or modifiers; that means that the accessor does not have
			// extra availability, but the ones form the property
			accessorCodeChanges = [new (
				accessorKind: AccessorKind.Getter,
				symbolAvailability: propertySupportedPlatforms,
				exportPropertyData: null,
				attributes: [],
				modifiers: [])
			];
		}

		change = new (
			name: memberName,
			returnType: new (propertySymbol.Type),
			symbolAvailability: propertySupportedPlatforms,
			attributes: attributes,
			modifiers: [.. declaration.Modifiers],
			accessors: accessorCodeChanges) {
			ExportFieldData = GetFieldInfo (context, propertySymbol),
			ExportPropertyData = propertySymbol.GetExportData<ObjCBindings.Property> (),
		};
		return true;
	}

	/// <inheritdoc />
	public override string ToString ()
	{
		var sb = new StringBuilder (
			$"Name: '{Name}', Type: {ReturnType}, Supported Platforms: {SymbolAvailability}, ExportFieldData: '{ExportFieldData?.ToString () ?? "null"}', ExportPropertyData: '{ExportPropertyData?.ToString () ?? "null"}' Attributes: [");
		sb.AppendJoin (",", Attributes);
		sb.Append ("], Modifiers: [");
		sb.AppendJoin (",", Modifiers.Select (x => x.Text));
		sb.Append ("], Accessors: [");
		sb.AppendJoin (",", Accessors);
		sb.Append (']');
		return sb.ToString ();
	}
}
