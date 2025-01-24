// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Context;
using Microsoft.Macios.Generator.Extensions;

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
}
