// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Availability;

namespace Microsoft.Macios.Generator.Extensions;

static partial class TypeSymbolExtensions {

	const string nativeObjectInterface = "ObjCRuntime.INativeObject";
	const string nsObjectClass = "Foundation.NSObject";
	const string dictionaryContainerClass = "Foundation.DictionaryContainer";

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

		// if we are dealing with a method, we want to also return the attributes used for the return type
		if (symbol is not IMethodSymbol methodSymbol)
			return attributes;

		var returnAttributes = methodSymbol.GetReturnTypeAttributes ();
		foreach (var attributeData in returnAttributes) {
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

	internal static T? GetAttribute<T> (this ISymbol symbol, string attributeName, TryParse<T> tryParse)
		where T : struct
		=> GetAttribute (symbol, () => attributeName, tryParse);

	/// <summary>
	/// Return the layout kind used by the symbol.
	/// </summary>
	/// <param name="symbol">The struct symbol whose layout we need to retrieve.</param>
	/// <returns>The layout kind used by the symbol.</returns>
	public static LayoutKind GetStructLayout (this ITypeSymbol symbol)
	{
		// Check for StructLayout attribute with LayoutKind.Sequential
		var layoutAttribute = symbol.GetAttributes ()
			.FirstOrDefault (attr =>
				attr.AttributeClass?.ToString () == typeof (StructLayoutAttribute).FullName);

		if (layoutAttribute is not null) {
			return (LayoutKind) layoutAttribute.ConstructorArguments [0].Value!;
		}

		// the default is auto, another lyaout would have been set in the attr
		return LayoutKind.Auto;
	}

	/// <summary>
	/// Return all the fields that determine the size of a struct. 
	/// </summary>
	/// <param name="symbol">The symbol whose fields to retireve.</param>
	/// <returns>an array with all the none static fields of a struct.</returns>
	public static IFieldSymbol [] GetStructFields (this ITypeSymbol symbol)
		=> symbol.GetMembers ()
			.OfType<IFieldSymbol> ()
			.Where (field => !field.IsStatic)
			.ToArray ();

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
				if (symbol.GetStructLayout () == LayoutKind.Auto) {
					return false;
				}

				// Recursively check all fields of the struct
				var instanceFields = symbol.GetStructFields ();
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
	/// Return the offset of a field in a Explicitly layout struct.
	/// </summary>
	/// <param name="symbol">A Field symbol.</param>
	/// <returns></returns>
	public static int GetFieldOffset (this IFieldSymbol symbol)
	{
		var offsetAttribute = symbol.GetAttributes ()
			.FirstOrDefault (attr =>
				attr.AttributeClass?.ToString () == typeof (FieldOffsetAttribute).FullName);

		return offsetAttribute is not null
			? (int) offsetAttribute.ConstructorArguments [0].Value!
			: 0;
	}

	/// <summary>
	/// Return the value of the MarshallAs attribute.
	/// </summary>
	/// <param name="symbol">The symbol under test.</param>
	/// <returns>The type to use for the symbol marshaling.</returns>
	public static (UnmanagedType Type, int SizeConst)? GetMarshalAs (this ISymbol symbol)
	{
		var marshalAsAttribute = symbol.GetAttributes ()
			.FirstOrDefault (attr =>
				attr.AttributeClass?.ToString () == typeof (MarshalAsAttribute).FullName);
		if (marshalAsAttribute is null)
			return null;
		var type = (UnmanagedType) marshalAsAttribute.ConstructorArguments [0].Value!;
		int sizeConst = 0;
		foreach (var (name, value) in marshalAsAttribute.NamedArguments) {
			if (name == "SizeConst")
				sizeConst = (int) value.Value!;
		}

		return (type, sizeConst);
	}

	/// <summary>
	/// Indicate whether the current symbol represents a type whose definition is nested inside the definition of
	/// another symbol.
	/// </summary>
	/// <param name="symbol">The symbol under test.</param>
	/// <returns>True if the symbol is nested.</returns>
	internal static bool IsNested (this ITypeSymbol symbol) => symbol.ContainingType is not null;

	/// <summary>
	/// Try to get the size of a built-in type.
	/// </summary>
	/// <param name="symbol">The symbol under test.</param>
	/// <param name="is64bits">The platform target.</param>
	/// <param name="size">The size of the native type.</param>
	/// <returns>True if we could calculate the size.</returns>
	internal static bool TryGetBuiltInTypeSize (this ITypeSymbol symbol, bool is64bits, out int size)
	{
		if (symbol.IsNested ()) {
			size = 0;
			return false;
		}
		
#pragma warning disable format

		var symbolInfo = (
			ContainingNamespace: symbol.ContainingNamespace.ToDisplayString (),
			Name: symbol.Name,
			SpecialType: symbol.SpecialType
		);

		var (currentSize, result) = symbolInfo switch { 
			{ SpecialType: SpecialType.System_Void } => (0, true), 
			{ ContainingNamespace: "ObjCRuntime", Name: "NativeHandle" } => (is64bits ? 8 : 4, true), 
			{ ContainingNamespace: "System.Runtime.InteropServices", Name: "NFloat" } => (is64bits ? 8 : 4, true), 
			{ ContainingNamespace: "System", Name: "Char" or "Boolean" or "SByte" or "Byte" } => (1, true), 
			{ ContainingNamespace: "System", Name: "Int16" or "UInt16" } => (2, true), 
			{ ContainingNamespace: "System", Name: "Single" or "Int32" or "UInt32" } => (4, true), 
			{ ContainingNamespace: "System", Name: "Double" or "Int64" or "UInt64" } => (8, true), 
			{ ContainingNamespace: "System", Name: "IntPtr" or "UIntPtr" or "nuint" or "nint" } => (is64bits ? 8 : 4, true),
			_ => (0, false)
		};
#pragma warning restore format
		size = currentSize;
		return result;
	}

	static bool TryGetBuiltInTypeSize (this ITypeSymbol type)
		=> TryGetBuiltInTypeSize (type, true /* doesn't matter */, out _);

	static int AlignAndAdd (int size, int add, ref int maxElementSize)
	{
		maxElementSize = Math.Max (maxElementSize, add);
		if (size % add != 0)
			size += add - size % add;
		return size + add;
	}


	static void GetValueTypeSize (this ITypeSymbol originalSymbol, ITypeSymbol type, List<ITypeSymbol> fieldSymbols,
		bool is64Bits, ref int size,
		ref int maxElementSize)
	{
		// FIXME:
		// SIMD types are not handled correctly here (they need 16-bit alignment).
		// However we don't annotate those types in any way currently, so first we'd need to 
		// add the proper attributes so that the generator can distinguish those types from other types.

		if (type.TryGetBuiltInTypeSize (is64Bits, out var typeSize) && typeSize > 0) {
			fieldSymbols.Add (type);
			size = AlignAndAdd (size, typeSize, ref maxElementSize);
			return;
		}

		// composite struct
		foreach (var field in type.GetStructFields ()) {
			var marshalAs = field.GetMarshalAs ();
			if (marshalAs is null) {
				GetValueTypeSize (originalSymbol, field.Type, fieldSymbols, is64Bits, ref size, ref maxElementSize);
				continue;
			}

			var (marshalAsType, sizeConst) = marshalAs.Value;
			var multiplier = 1;
			switch (marshalAsType) {
			case UnmanagedType.ByValArray:
				var types = new List<ITypeSymbol> ();
				var arrayTypeSymbol = (field as IArrayTypeSymbol)!;
				GetValueTypeSize (originalSymbol, arrayTypeSymbol.ElementType, types, is64Bits, ref typeSize, ref maxElementSize);
				multiplier = sizeConst;
				break;
			case UnmanagedType.U1:
			case UnmanagedType.I1:
				typeSize = 1;
				break;
			case UnmanagedType.U2:
			case UnmanagedType.I2:
				typeSize = 2;
				break;
			case UnmanagedType.U4:
			case UnmanagedType.I4:
			case UnmanagedType.R4:
				typeSize = 4;
				break;
			case UnmanagedType.U8:
			case UnmanagedType.I8:
			case UnmanagedType.R8:
				typeSize = 8;
				break;
			default:
				throw new Exception ($"Unhandled MarshalAs attribute: {marshalAs.Value} on field {field.ToDisplayString ()}");
			}
			fieldSymbols.Add (field.Type);
			size = AlignAndAdd (size, typeSize, ref maxElementSize);
			size += (multiplier - 1) * size;
		}
	}

	/// <summary>
	/// Return the size of a blittable structure that can be used in a PInvoke
	/// </summary>
	/// <param name="type">The type symbol whose size we want to get.</param>
	/// <param name="fieldTypes">The fileds of the struct.</param>
	/// <param name="is64Bits">If the calculation is for a 64b machine.</param>
	/// <returns></returns>
	internal static int GetValueTypeSize (this ITypeSymbol type, List<ITypeSymbol> fieldTypes, bool is64Bits)
	{
		int size = 0;
		int maxElementSize = 1;

		if (type.GetStructLayout () == LayoutKind.Explicit) {
			// Find the maximum of "field size + field offset" for each field.
			foreach (var field in type.GetStructFields ()) {
				var fieldOffset = field.GetFieldOffset ();
				var elementSize = 0;
				GetValueTypeSize (type, field.Type, fieldTypes, is64Bits, ref elementSize, ref maxElementSize);
				size = Math.Max (size, elementSize + fieldOffset);
			}
		} else {
			GetValueTypeSize (type, type, fieldTypes, is64Bits, ref size, ref maxElementSize);
		}

		if (size % maxElementSize != 0)
			size += (maxElementSize - size % maxElementSize);

		return size;
	}

	/// <summary>
	/// Returns the parents and all the implemented interfaces (including those of the parents).
	/// </summary>
	/// <param name="symbol">The symbol whose inheritance we want to retrieve.</param>
	/// <param name="isNativeObject">If the type implements the INativeObject interface.</param>
	/// <param name="isDictionaryContainer">If the type inherits from Foundation.DictionaryContainer.</param>
	/// <param name="parents">An immutable array of the parents in order from closest to furthest.</param>
	/// <param name="interfaces">All implemented interfaces by the type and its parents.</param>
	/// <param name="isNSObject">If the type inherits from NSObject.</param>
	public static void GetInheritance (
		this ITypeSymbol symbol, out bool isNSObject, out bool isNativeObject, out bool isDictionaryContainer,
		out ImmutableArray<string> parents,
		out ImmutableArray<string> interfaces)
	{
		isNSObject = symbol.ToDisplayString ().Trim () == nsObjectClass;
		isNativeObject = false;
		isDictionaryContainer = false;

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
			isDictionaryContainer |= parentName == dictionaryContainerClass;
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
