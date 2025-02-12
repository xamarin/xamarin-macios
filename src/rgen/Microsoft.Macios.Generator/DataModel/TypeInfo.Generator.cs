// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Extensions;

namespace Microsoft.Macios.Generator.DataModel;

readonly partial struct TypeInfo {

	/// <summary>
	/// Return if the type represents a wrapped object from the objc world.
	/// </summary>
	public bool IsWrapped { get; init; }

	/// <summary>
	/// True if the type needs to use a stret call.
	/// </summary>
	public bool NeedsStret { get; init; }

	/// <summary>
	/// True if the type represents a delegate.
	/// </summary>
	public bool IsDelegate { get; init; }

	/// <summary>
	/// Returns, if the type is an array, if its elements are a wrapped object from the objc world.
	/// </summary>
	public bool ArrayElementTypeIsWrapped { get; init; }

	/// <summary>
	/// Get the name of the variable for the type when it is used as a return value.
	/// </summary>
	public string ReturnVariableName => "ret"; // nothing fancy for now
	
	internal TypeInfo (ITypeSymbol symbol, Compilation compilation) :
		this (
			symbol is IArrayTypeSymbol arrayTypeSymbol
				? arrayTypeSymbol.ElementType.ToDisplayString ()
				: symbol.ToDisplayString ().Trim ('?', '[', ']'),
			symbol.SpecialType)
	{
		IsNullable = symbol.NullableAnnotation == NullableAnnotation.Annotated;
		IsBlittable = symbol.IsBlittable ();
		IsSmartEnum = symbol.IsSmartEnum ();
		IsReferenceType = symbol.IsReferenceType;
		IsStruct = symbol.TypeKind == TypeKind.Struct;
		IsInterface = symbol.TypeKind == TypeKind.Interface;
		IsDelegate = symbol.TypeKind == TypeKind.Delegate;
		IsNativeIntegerType = symbol.IsNativeIntegerType;
		IsNativeEnum = symbol.HasAttribute (AttributesNames.NativeEnumAttribute);
		NeedsStret = symbol.NeedsStret (compilation);

		// data that we can get from the symbol without being INamedType
		symbol.GetInheritance (
			isNSObject: out isNSObject,
			isNativeObject: out isINativeObject,
			isDictionaryContainer: out isDictionaryContainer,
			parents: out parents,
			interfaces: out interfaces);

		IsWrapped = symbol.IsWrapped (isNSObject);
		if (symbol is IArrayTypeSymbol arraySymbol) {
			IsArray = true;
			ArrayElementTypeIsWrapped = arraySymbol.ElementType.IsWrapped ();
		}

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
