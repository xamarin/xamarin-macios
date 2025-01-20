// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Extensions;

namespace Microsoft.Macios.Generator.DataModel;

readonly partial struct TypeInfo {

	internal TypeInfo (ITypeSymbol symbol) :
		this (
			symbol is IArrayTypeSymbol arrayTypeSymbol
				? arrayTypeSymbol.ElementType.ToDisplayString ()
				: symbol.ToDisplayString ().Trim ('?', '[', ']'),
			symbol.SpecialType)
	{
		IsNullable = symbol.NullableAnnotation == NullableAnnotation.Annotated;
		IsBlittable = symbol.IsBlittable ();
		IsSmartEnum = symbol.IsSmartEnum ();
		IsArray = symbol is IArrayTypeSymbol;
		IsReferenceType = symbol.IsReferenceType;
		IsInterface = symbol.TypeKind == TypeKind.Interface;
		IsNativeIntegerType = symbol.IsNativeIntegerType;
		IsNativeEnum = symbol.HasAttribute (AttributesNames.NativeEnumAttribute);

		// data that we can get from the symbol without being INamedType
		symbol.GetInheritance (
			isNSObject: out isNSObject,
			isNativeObject: out isINativeObject,
			parents: out parents,
			interfaces: out interfaces);

		// try to get the named type symbol to have more educated decisions
		var namedTypeSymbol = symbol as INamedTypeSymbol;

		// store the enum special type, useful when generate code that needs to cast
		EnumUnderlyingType = namedTypeSymbol?.EnumUnderlyingType?.SpecialType;

		if (!IsReferenceType && IsNullable && namedTypeSymbol is not null) {
			// get the type argument for nullable, which we know is the data that was boxed and use it to 
			// overwrite the SpecialType 
			var typeArgument = namedTypeSymbol.TypeArguments [0];
			SpecialType = typeArgument.SpecialType;
			MetadataName = SpecialType is SpecialType.None or SpecialType.System_Void
				? null : typeArgument.MetadataName;
		} else {
			MetadataName = SpecialType is SpecialType.None or SpecialType.System_Void
				? null : symbol.MetadataName;
		}

	}

}
