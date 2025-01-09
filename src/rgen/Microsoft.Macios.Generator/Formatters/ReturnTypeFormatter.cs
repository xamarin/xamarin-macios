// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.DataModel;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Microsoft.Macios.Generator.Formatters;

static class ReturnTypeFormatter {

	public static TypeSyntax GetIdentifierSyntax (this in ReturnType returnType)
	{
		if (returnType.IsArray) {
			// could be a params array or simply an array
			var arrayType = ArrayType (IdentifierName (returnType.Type))
				.WithRankSpecifiers (SingletonList (
					ArrayRankSpecifier (
						SingletonSeparatedList<ExpressionSyntax> (OmittedArraySizeExpression ()))));
			return returnType.IsNullable
				? NullableType (arrayType)
				: arrayType;
		}

		// dealing with a non-array type
		return returnType.IsNullable
			? NullableType (IdentifierName (returnType.Type))
			: IdentifierName (returnType.Type);
	}
}
