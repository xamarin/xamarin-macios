// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using TypeInfo = Microsoft.Macios.Generator.DataModel.TypeInfo;

namespace Microsoft.Macios.Generator.Formatters;

static class TypeInfoFormatter {

	public static TypeSyntax GetIdentifierSyntax (this in TypeInfo typeInfo)
	{
		// If we are dealing with a IntPtr or UIntPtr, we want to use the metadata name, otherwise we will
		// get a nint in the signature. The compiler won't care, this is just done for completeness and to
		// reduce the surprise factor for a customer debugging the generated code.
		var name = typeInfo.SpecialType is SpecialType.System_IntPtr or SpecialType.System_UIntPtr
			? typeInfo.MetadataName! // we know is not null
			: typeInfo.FullyQualifiedName;

		if (typeInfo.IsArray) {
			// could be a params array or simply an array
			var arrayType = ArrayType (IdentifierName (name))
				.WithRankSpecifiers (SingletonList (
					ArrayRankSpecifier (
						SingletonSeparatedList<ExpressionSyntax> (OmittedArraySizeExpression ()))));
			return typeInfo.IsNullable
				? NullableType (arrayType)
				: arrayType;
		}

		// dealing with a non-array type
		return typeInfo.IsNullable
			? NullableType (IdentifierName (name))
			: IdentifierName (name);
	}
}
