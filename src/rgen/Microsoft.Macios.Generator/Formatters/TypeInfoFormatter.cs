// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.DataModel;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Microsoft.Macios.Generator.Formatters;

static class TypeInfoFormatter {

	public static TypeSyntax GetIdentifierSyntax (this in TypeInfo typeInfo)
	{
		if (typeInfo.IsArray) {
			// could be a params array or simply an array
			var arrayType = ArrayType (IdentifierName (typeInfo.Name))
				.WithRankSpecifiers (SingletonList (
					ArrayRankSpecifier (
						SingletonSeparatedList<ExpressionSyntax> (OmittedArraySizeExpression ()))));
			return typeInfo.IsNullable
				? NullableType (arrayType)
				: arrayType;
		}

		// dealing with a non-array type
		return typeInfo.IsNullable
			? NullableType (IdentifierName (typeInfo.Name))
			: IdentifierName (typeInfo.Name);
	}
}
