// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Availability;

namespace Microsoft.Macios.Generator.Extensions;

static partial class TypeSymbolExtensions {

	/// <summary>
	/// Return the symbol availability WITHOUT taking into account the parent symbols availability.
	/// </summary>
	/// <param name="symbol">The symbols whose availability attributes we want to retrieve.</param>
	/// <returns>The symbol availability WITHOUT taking into account the parent symbols.</returns>
	/// <remarks>This is a helper method, you probably don't want to use it.</remarks>
	static SymbolAvailability GetAvailabilityForSymbol (this ISymbol symbol)
	{
		//get the attribute of the symbol and look for the Supported and Unsupported attributes and
		// add the different platforms to the result hashsets
		var builder = SymbolAvailability.CreateBuilder ();
		var boundAttributes = symbol.GetAttributes ();
		if (boundAttributes.Length == 0) {
			// no attrs in the symbol, therefore the symbol is supported in all platforms
			return builder.ToImmutable ();
		}

		foreach (var attributeData in boundAttributes) {
			var attrName = attributeData.AttributeClass?.ToDisplayString ();
			if (string.IsNullOrEmpty (attrName))
				continue;
			// we only care in this case about the support and unsupported attrs, ignore any other
			switch (attrName) {
			case AttributesNames.SupportedOSPlatformAttribute:
				if (SupportedOSPlatformData.TryParse (attributeData, out var supportedPlatform)) {
					builder.Add (supportedPlatform.Value);
				}

				break;
			case AttributesNames.UnsupportedOSPlatformAttribute:
				if (UnsupportedOSPlatformData.TryParse (attributeData, out var unsupportedPlatform)) {
					builder.Add (unsupportedPlatform.Value);
				}

				break;
			case AttributesNames.ObsoletedOSPlatformAttribute:
				if (ObsoletedOSPlatformData.TryParse (attributeData, out var obsoletedOsPlatform)) {
					builder.Add (obsoletedOsPlatform.Value);
				}

				break;
			default:
				continue;
			}
		}

		return builder.ToImmutable ();
	}

	public static bool IsSmartEnum (this ITypeSymbol symbol)
	{
		// a type is a smart enum if its type is a enum one AND it was decorated with the
		// binding type attribute
		return symbol.TypeKind == TypeKind.Enum
			   && symbol.HasAttribute (AttributesNames.BindingAttribute);
	}

	public static BindingTypeData GetBindingData (this ISymbol symbol)
	{
		var boundAttributes = symbol.GetAttributes ();
		if (boundAttributes.Length == 0) {
			// no attrs in the symbol, therefore the symbol is supported in all platforms
			return default;
		}

		// we are looking for the basic BindingAttribute attr
		foreach (var attributeData in boundAttributes) {
			var attrName = attributeData.AttributeClass?.ToDisplayString ();
			if (string.IsNullOrEmpty (attrName) || attrName != AttributesNames.BindingAttribute)
				continue;
			if (BindingTypeData.TryParse (attributeData, out var bindingData)) {
				return bindingData.Value;
			}
		}

		return default;
	}

	public static BindingTypeData<T> GetBindingData<T> (this ISymbol symbol) where T : Enum
	{
		var boundAttributes = symbol.GetAttributes ();
		if (boundAttributes.Length == 0) {
			// no attrs in the symbol, therefore the symbol is supported in all platforms
			return default;
		}

		var targetAttrName = AttributesNames.GetBindingTypeAttributeName<T> ();
		foreach (var attributeData in boundAttributes) {
			var attrName = attributeData.AttributeClass?.ToDisplayString ();
			if (string.IsNullOrEmpty (attrName) || attrName != targetAttrName)
				continue;
			if (BindingTypeData<T>.TryParse (attributeData, out var bindingData)) {
				return bindingData.Value;
			}
		}

		return default;
	}

	/// <summary>
	/// Retrieve the data of an export attribute on a symbol.
	/// </summary>
	/// <param name="symbol">The tagged symbol.</param>
	/// <typeparam name="T">Enum type used in the attribute.</typeparam>
	/// <returns>The data of the export attribute if present or null if it was not found.</returns>
	/// <remarks>If the passed enum is unknown or not supported as an enum for the export attribute, null will be
	/// returned.</remarks>
	public static ExportData<T>? GetExportData<T> (this ISymbol symbol) where T : Enum
		=> GetAttribute<ExportData<T>> (symbol, AttributesNames.GetExportAttributeName<T>, ExportData<T>.TryParse);

	/// <summary>
	/// Retrieve the data of a field attribute on a symbol.
	/// </summary>
	/// <param name="symbol">The tagged symbol.</param>
	/// <typeparam name="T">Enum type used in the attribute.</typeparam>
	/// <returns>The data of the export attribute if present or null if it was not found.</returns>
	/// <remarks>If the passed enum is unknown or not supported as an enum for the field attribute, null will be
	/// returned.</remarks>
	public static FieldData<T>? GetFieldData<T> (this ISymbol symbol) where T : Enum
		=> GetAttribute<FieldData<T>> (symbol, AttributesNames.GetFieldAttributeName<T>, FieldData<T>.TryParse);

	public static BindFromData? GetBindFromData (this ISymbol symbol)
		=> GetAttribute<BindFromData> (symbol, AttributesNames.BindFromAttribute, BindFromData.TryParse);

	public static bool X86NeedStret (ITypeSymbol returnType)
	{
		if (!returnType.IsValueType || returnType.SpecialType == SpecialType.System_Enum ||
			returnType.TryGetBuiltInTypeSize ())
			return false;

		var fieldTypes = new List<ITypeSymbol> ();
		var size = GetValueTypeSize (returnType, fieldTypes, false);

		if (size > 8)
			return true;

		return fieldTypes.Count == 3;
	}

	public static bool X86_64NeedStret (ITypeSymbol returnType)
	{
		if (!returnType.IsValueType || returnType.SpecialType == SpecialType.System_Enum ||
			returnType.TryGetBuiltInTypeSize ())
			return false;

		var fieldTypes = new List<ITypeSymbol> ();
		return GetValueTypeSize (returnType, fieldTypes, true) > 16;
	}

	public static bool ArmNeedStret (ITypeSymbol returnType, Compilation compilation)
	{
		var currentPlatform = compilation.GetCurrentPlatform ();
		bool has32bitArm = currentPlatform != PlatformName.TvOS && currentPlatform != PlatformName.MacOSX;
		if (!has32bitArm)
			return false;

		ITypeSymbol t = returnType;

		if (!t.IsValueType || t.SpecialType == SpecialType.System_Enum || t.TryGetBuiltInTypeSize ())
			return false;

		var fieldTypes = new List<ITypeSymbol> ();
		var size = t.GetValueTypeSize (fieldTypes, false);

		bool isiOS = currentPlatform == PlatformName.iOS;

		if (isiOS && size <= 4 && fieldTypes.Count == 1) {
			
#pragma warning disable format
			return fieldTypes [0] switch {
				{ Name: "nint" } => false,
				{ Name: "nuint" } => false,
				{ SpecialType: SpecialType.System_Char } => false,
				{ SpecialType: SpecialType.System_Byte } => false,
				{ SpecialType: SpecialType.System_SByte } => false,
				{ SpecialType: SpecialType.System_UInt16 } => false,
				{ SpecialType: SpecialType.System_Int16 } => false,
				{ SpecialType: SpecialType.System_UInt32 } => false,
				{ SpecialType: SpecialType.System_Int32 } => false,
				{ SpecialType: SpecialType.System_IntPtr } => false,
				{ SpecialType: SpecialType.System_UIntPtr } => false,
				_ => true
			};
#pragma warning restore format
		}

		return true;
	}

	/// <summary>
	/// Return if a given ITypeSymbol requires to use the objc_MsgSend_stret variants.
	/// </summary>
	/// <param name="returnType">The type we are testing.</param>
	/// <param name="compilation">The current compilation, used to determine the target platform.</param>
	/// <returns>If the type represented by the symtol needs a stret call variant.</returns>
	public static bool NeedsStret (this ITypeSymbol returnType, Compilation compilation)
	{
		if (X86NeedStret (returnType))
			return true;

		if (X86_64NeedStret (returnType))
			return true;

		return ArmNeedStret (returnType, compilation);
	}

	/// <summary>
	/// A type is considered wrapped if it is an Interface or is an child of the NSObject class or the NSObject
	/// itself.
	/// </summary>
	/// <param name="symbol">The symbol to check if it is wrapped.</param>
	/// <param name="isNSObject">If the symbol is a NSObject of inherits from an NSObject.</param>
	/// <returns>True if the ymbol is considered to be wrapped.</returns>
	public static bool IsWrapped (this ITypeSymbol symbol, bool isNSObject)
		=> symbol.TypeKind == TypeKind.Interface || isNSObject;

	/// <summary>
	/// A type is considered wrapped if it is an Interface or is an child of the NSObject class or the NSObject
	/// itself.
	/// </summary>
	/// <param name="symbol">The symbol to check if it is wrapped.</param>
	/// <returns>True if the ymbol is considered to be wrapped.</returns>
	public static bool IsWrapped (this ITypeSymbol symbol)
	{
		symbol.GetInheritance (
			isNSObject: out bool isNSObject,
			isNativeObject: out bool _,
			isDictionaryContainer: out bool _,
			parents: out ImmutableArray<string> _,
			interfaces: out ImmutableArray<string> _);
		// either we are a NSObject or we are a subclass of it
		return IsWrapped (symbol, isNSObject);
	}
}
