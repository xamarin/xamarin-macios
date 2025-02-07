// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
using ObjCRuntime;

namespace Microsoft.Macios.Generator.DataModel;

readonly partial struct Property {

	/// <summary>
	/// The data of the field attribute used to mark the value as a field binding. 
	/// </summary>
	public FieldInfo<ObjCBindings.Property>? ExportFieldData { get; init; }

	/// <summary>
	/// True if the property represents a Objc field.
	/// </summary>
	[MemberNotNullWhen (true, nameof (ExportFieldData))]
	public bool IsField => ExportFieldData is not null;

	/// <summary>
	/// Returns if the field was marked as a notification.
	/// </summary>
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
	/// Returns if the property was marked as thread safe.
	/// </summary>
	public bool IsThreadSafe =>
		IsProperty && ExportPropertyData.Value.Flags.HasFlag (ObjCBindings.Property.IsThreadSafe);

	/// <summary>
	/// True if the method was exported with the MarshalNativeExceptions flag allowing it to support native exceptions.
	/// </summary>
	public bool MarshalNativeExceptions
		=> IsProperty && ExportPropertyData.Value.Flags.HasFlag (ObjCBindings.Property.MarshalNativeExceptions);

	/// <summary>
	/// Returns the bind from data if present in the binding.
	/// </summary>
	public BindFromData? BindAs { get; init; }

	/// <summary>
	/// True if the property should be generated without a backing field.
	/// </summary>
	public bool IsTransient => IsProperty && ExportPropertyData.Value.Flags.HasFlag (ObjCBindings.Property.Transient);

	/// <summary>
	/// True if the property was marked to DisableZeroCopy.
	/// </summary>
	public bool DisableZeroCopy
		=> IsProperty && ExportPropertyData.Value.Flags.HasFlag (ObjCBindings.Property.DisableZeroCopy);

	/// <summary>
	/// True if the generator should not use a NSString for marshalling.
	/// </summary>
	public bool UsePlainString
		=> IsProperty && ExportPropertyData.Value.Flags.HasFlag (ObjCBindings.Property.PlainString);

	/// <summary>
	/// Return if the method invocation should be wrapped by a NSAutoReleasePool.
	/// </summary>
	public bool AutoRelease => IsProperty && ExportPropertyData.Value.Flags.HasFlag (ObjCBindings.Method.AutoRelease);

	readonly bool? needsBackingField = null;
	/// <summary>
	/// States if the property, when generated, needs a backing field.
	/// </summary>
	public bool NeedsBackingField {
		get {
			if (needsBackingField is not null)
				return needsBackingField.Value;
			var isWrapped = ReturnType.IsWrapped ||
							ReturnType is { IsArray: true, ArrayElementTypeIsWrapped: true };
			return isWrapped && !IsTransient;
		}
		// Added to allow testing. This way we can set the correct expectation in the test factory
		init => needsBackingField = value;
	}

	readonly bool? requiresDirtyCheck = null;
	/// <summary>
	/// States if the property, when generated, should have a dirty check.
	/// </summary>
	public bool RequiresDirtyCheck {
		get {
			if (requiresDirtyCheck is not null)
				return requiresDirtyCheck.Value;
			if (!IsProperty)
				return false;
			switch (ExportPropertyData.Value.ArgumentSemantic) {
			case ArgumentSemantic.Copy:
			case ArgumentSemantic.Retain:
			case ArgumentSemantic.None:
				return NeedsBackingField;
			default:
				return false;
			}
		}
		// Added to allow testing. This way we can set the correct expectation in the test factory
		init => requiresDirtyCheck = value;
	}

	static FieldInfo<ObjCBindings.Property>? GetFieldInfo (RootContext context, IPropertySymbol propertySymbol)
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

	internal Property (string name, TypeInfo returnType,
		SymbolAvailability symbolAvailability,
		ImmutableArray<AttributeCodeChange> attributes,
		ImmutableArray<SyntaxToken> modifiers,
		ImmutableArray<Accessor> accessors)
	{
		Name = name;
		BackingField = $"_{Name}";
		ReturnType = returnType;
		SymbolAvailability = symbolAvailability;
		Attributes = attributes;
		Modifiers = modifiers;
		Accessors = accessors;
	}

	public static bool TryCreate (PropertyDeclarationSyntax declaration, RootContext context,
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
			BindAs = propertySymbol.GetBindFromData (),
			ExportFieldData = GetFieldInfo (context, propertySymbol),
			ExportPropertyData = propertySymbol.GetExportData<ObjCBindings.Property> (),
		};
		return true;
	}

	/// <inheritdoc />
	public bool Equals (Property other)
	{
		if (!CoreEquals (other))
			return false;
		if (IsTransient != other.IsTransient)
			return false;
		if (NeedsBackingField != other.NeedsBackingField)
			return false;
		return RequiresDirtyCheck == other.RequiresDirtyCheck;
	}

	/// <inheritdoc />
	public override string ToString ()
	{
		var sb = new StringBuilder (
			$"Name: '{Name}', Type: {ReturnType}, Supported Platforms: {SymbolAvailability}, ExportFieldData: '{ExportFieldData?.ToString () ?? "null"}', ExportPropertyData: '{ExportPropertyData?.ToString () ?? "null"}', ");
		sb.Append ($"IsTransient: '{IsTransient}', ");
		sb.Append ($"NeedsBackingField: '{NeedsBackingField}', ");
		sb.Append ($"RequiresDirtyCheck: '{RequiresDirtyCheck}', ");
		sb.Append ($"BindAs: '{BindAs}', ");
		sb.Append ("Attributes: [");
		sb.AppendJoin (",", Attributes);
		sb.Append ("], Modifiers: [");
		sb.AppendJoin (",", Modifiers.Select (x => x.Text));
		sb.Append ("], Accessors: [");
		sb.AppendJoin (",", Accessors);
		sb.Append (']');
		return sb.ToString ();
	}
}
