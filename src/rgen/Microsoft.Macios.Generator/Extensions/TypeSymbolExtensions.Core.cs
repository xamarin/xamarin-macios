// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Availability;

namespace Microsoft.Macios.Generator.Extensions;

static partial class TypeSymbolExtensions {
	/// <summary>
	/// Retrieve a dictionary with the attribute data of all the attributes attached to a symbol. Because
	/// an attribute can appear more than once, the valus are a collection of attribute data.
	/// </summary>
	/// <param name="symbol">The symbol whose attributes we want to retrieve.</param>
	/// <returns>A dictionary with the attribute names as keys and the related attribute data.</returns>
	public static Dictionary<string, List<AttributeData>> GetAttributeData (this ISymbol symbol)
	{
		var boundAttributes = symbol.GetAttributes ();
		if (boundAttributes.Length == 0) {
			// return an empty dictionary if there are no attributes
			return new ();
		}

		var attributes = new Dictionary<string, List<AttributeData>> ();
		foreach (var attributeData in boundAttributes) {
			var attrName = attributeData.AttributeClass?.ToDisplayString ();
			if (string.IsNullOrEmpty (attrName))
				continue;
			if (!attributes.TryGetValue (attrName, out var attributeDataList)) {
				attributeDataList = new List<AttributeData> ();
				attributes.Add (attrName, attributeDataList);
			}

			attributeDataList.Add (attributeData);
		}

		return attributes;
	}

	/// <summary>
	/// Return the list of parent symbols in ascending order.
	/// </summary>
	/// <param name="symbol">The symbol whose parents to retrieve.</param>
	/// <returns>Array with all the parent symbols until we reach the containing namespace.</returns>
	public static ImmutableArray<ISymbol> GetParents (this ISymbol symbol)
	{
		var result = new List<ISymbol> ();
		// when looking for the parents of a symbol we need to make a distinction between a general ISymbol such as
		// a INamedType symbol and a IMethodSymbol that represents a property accessor. Properties are NOT treated as
		// containing symbols of their accessors. Accessors are related to their property symbols via the AssociatedSymbol
		// property. In our code generator we want to include the property symbol as a parent of the accessor to combine
		// correctly the availability attributes.
		var current = (symbol is IMethodSymbol { AssociatedSymbol: not null } methodSymbol)
			? methodSymbol.AssociatedSymbol
			: symbol.ContainingSymbol;
		if (current is null)
			return [];

		while (current is not INamespaceSymbol) {
			result.Add (current);
			current = current.ContainingSymbol;
		}

		return [.. result];
	}

	public static bool HasAttribute (this ISymbol symbol, string attribute)
	{
		var boundAttributes = symbol.GetAttributes ();
		if (boundAttributes.Length == 0) {
			return false;
		}

		foreach (var attributeData in boundAttributes) {
			var attrName = attributeData.AttributeClass?.ToDisplayString ();
			if (attrName == attribute) {
				return true;
			}
		}

		return false;
	}

	internal delegate string? GetAttributeNames ();

	internal delegate bool TryParse<T> (AttributeData data, [NotNullWhen (true)] out T? value) where T : struct;

	internal static T? GetAttribute<T> (this ISymbol symbol, GetAttributeNames getAttributeNames, TryParse<T> tryParse)
		where T : struct
	{
		var attributes = symbol.GetAttributeData ();
		if (attributes.Count == 0)
			return null;

		// retrieve the name of the attribute based on the flag
		var attrName = getAttributeNames ();
		if (attrName is null)
			return null;
		if (!attributes.TryGetValue (attrName, out var exportAttrDataList) ||
			exportAttrDataList.Count != 1)
			return null;

		var exportAttrData = exportAttrDataList [0];
		var fieldSyntax = exportAttrData.ApplicationSyntaxReference?.GetSyntax ();
		if (fieldSyntax is null)
			return null;

		if (tryParse (exportAttrData, out var exportData))
			return exportData.Value;
		return null;
	}

	internal static T? GetAttribute<T> (this ISymbol symbol, string attributeName, TryParse<T> tryParse) where T : struct
		=> GetAttribute (symbol, () => attributeName, tryParse);

	/// <summary>
	/// Returns if a type is blittable or not.
	/// </summary>
	/// <param name="symbol"></param>
	/// <returns></returns>
	/// <seealso cref="https://learn.microsoft.com/en-us/dotnet/framework/interop/blittable-and-non-blittable-types"/>
	public static bool IsBlittable (this ITypeSymbol symbol)
	{
		if (symbol.NullableAnnotation == NullableAnnotation.Annotated)
			return false;

		while (true) {
			// per the documentation, the following system types are blittable
			switch (symbol.SpecialType) {
			case SpecialType.System_Byte:
			case SpecialType.System_SByte:
			case SpecialType.System_Int16:
			case SpecialType.System_UInt16:
			case SpecialType.System_Int32:
			case SpecialType.System_UInt32:
			case SpecialType.System_Int64:
			case SpecialType.System_UInt64:
			case SpecialType.System_Single:
			case SpecialType.System_Double:
			case SpecialType.System_IntPtr:
			case SpecialType.System_UIntPtr:
				return true;
			case SpecialType.System_Array:
				// if we are dealing with an array, an array of blittable elements is blittable
				symbol = symbol.ContainingType;
				continue;
			}

			if (symbol is IArrayTypeSymbol arrayTypeSymbol) {
				symbol = arrayTypeSymbol.ElementType;
				continue;
			}

			if (symbol.TypeKind == TypeKind.Enum && symbol is INamedTypeSymbol enumSymbol) {
				// an enum is blittable based on its backing field
				symbol = enumSymbol.EnumUnderlyingType!;
				continue;
			}

			// if we are dealing with a structure, we have to check the layout type and all its children
			if (symbol.TypeKind == TypeKind.Struct) {
				// Check for StructLayout attribute with LayoutKind.Sequential
				var layoutAttribute = symbol.GetAttributes ()
					.FirstOrDefault (attr =>
						attr.AttributeClass?.ToString () == typeof (StructLayoutAttribute).FullName);

				if (layoutAttribute is not null) {
					var layoutKind = (LayoutKind) layoutAttribute.ConstructorArguments [0].Value!;
					if (layoutKind == LayoutKind.Auto) {
						return false;
					}
				} else {
					return false;
				}

				// Recursively check all fields of the struct
				var instanceFields = symbol.GetMembers ()
					.OfType<IFieldSymbol> ()
					.Where (field => !field.IsStatic);
				foreach (var member in instanceFields) {
					if (!member.Type.IsBlittable ()) {
						return false;
					}
				}

				return true;
			}

			// any other types are not blittable
			return false;
		}
	}

	/// <summary>
	/// Returns the parents and all the implemented interfaces (including those of the parents).
	/// </summary>
	/// <param name="symbol">The symbol whose inheritance we want to retrieve.</param>
	/// <param name="isNativeObject">If the type implements the INativeObject interface.</param>
	/// <param name="parents">An immutable array of the parents in order from closest to furthest.</param>
	/// <param name="interfaces">All implemented interfaces by the type and its parents.</param>
	/// <param name="isNSObject">If the type inherits from NSObject.</param>
	public static void GetInheritance (
		this ITypeSymbol symbol, out bool isNSObject, out bool isNativeObject, out ImmutableArray<string> parents,
		out ImmutableArray<string> interfaces)
	{
		const string nativeObjectInterface = "ObjCRuntime.INativeObject";
		const string nsObjectClass = "Foundation.NSObject";

		isNSObject = false;
		isNativeObject = false;

		// parents will be returned directly in a Immutable array via a builder since the order is important
		// interfaces will use a hash set because we do not want duplicates.
		var parentsBuilder = ImmutableArray.CreateBuilder<string> ();
		// init the set with all the interfaces of the current symbol
		var interfacesSet = new HashSet<string> (symbol.Interfaces.Select (i => i.ToDisplayString ()));

		var currentType = symbol.BaseType;
		while (currentType is not null) {
			// check if we reach the NSObject as a parent
			var parentName = currentType.ToDisplayString ().Trim ();
			isNSObject |= parentName == nsObjectClass;
			parentsBuilder.Add (parentName);

			// union with the current interfaces
			interfacesSet.UnionWith (currentType.Interfaces.Select (i => i.ToDisplayString ()));

			currentType = currentType.BaseType;
		}

		isNativeObject = interfacesSet.Contains (nativeObjectInterface);
		parents = parentsBuilder.ToImmutable ();
		interfaces = [.. interfacesSet];
	}
	
	/// <summary>
	/// Returns the symbol availability taking into account the parent symbols availability.
	///
	/// That means that the attributes used on the current symbol are merged with the attributes used
	/// in all the symbol parents following the correct child-parent order.
	/// </summary>
	/// <param name="symbol">The symbol whose availability we want to retrieve.</param>
	/// <returns>A symbol availability structure for the symbol.</returns>
	public static SymbolAvailability GetSupportedPlatforms (this ISymbol symbol)
	{
		var availability = GetAvailabilityForSymbol (symbol);
		// get the parents and return the merge
		foreach (var parent in GetParents (symbol)) {
			availability = availability.MergeWithParent (GetAvailabilityForSymbol (parent));
		}

		return availability;
	}
}
