// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Extensions;
using Microsoft.Macios.Transformer;
using Microsoft.Macios.Transformer.Extensions;

namespace Microsoft.Macios.Generator.DataModel;

readonly partial struct TypeInfo {

	internal TypeInfo (ITypeSymbol symbol, Dictionary<string, List<AttributeData>> attributes) :
		this (
			symbol is IArrayTypeSymbol arrayTypeSymbol
				? arrayTypeSymbol.ElementType.ToDisplayString ()
				: symbol.ToDisplayString ().Trim ('?', '[', ']'),
			symbol.SpecialType)
	{
		IsNullable = attributes.HasNullAllowedFlag ();
		// special case, the old bindings might not have the ? operator but will have the attr 
		IsBlittable = symbol.IsBlittable () && !IsNullable;
		IsSmartEnum = symbol.IsSmartEnum ();
		IsReferenceType = symbol.IsReferenceType;
		IsStruct = symbol.TypeKind == TypeKind.Struct;
		IsInterface = symbol.TypeKind == TypeKind.Interface;
		IsNativeIntegerType = symbol.IsNativeIntegerType;
		IsNativeEnum = symbol.HasAttribute (AttributesNames.NativeAttribute);

		// data that we can get from the symbol without being INamedType
		symbol.GetInheritance (
			isNSObject: out isNSObject,
			isNativeObject: out isINativeObject,
			isDictionaryContainer: out isDictionaryContainer,
			parents: out parents,
			interfaces: out interfaces);
		IsArray = symbol is IArrayTypeSymbol;

		// try to get the named type symbol to have more educated decisions
		var namedTypeSymbol = symbol as INamedTypeSymbol;

		// store the enum special type, useful when generate code that needs to cast
		EnumUnderlyingType = namedTypeSymbol?.EnumUnderlyingType?.SpecialType;
		MetadataName = SpecialType is SpecialType.None or SpecialType.System_Void
			? null : symbol.MetadataName;
	}
}
