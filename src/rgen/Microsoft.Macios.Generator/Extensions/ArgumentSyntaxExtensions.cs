// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Microsoft.Macios.Generator.Extensions;

static class ArgumentSyntaxExtensions {

	public static SyntaxNodeOrToken [] ToSyntaxNodeOrTokenArray (this ImmutableArray<ArgumentSyntax> arguments)
	{
		// the size of the array is simple to calculate, we need space for all parameters
		// and for a comma for each parameter except for the last one
		// length = parameters.Length + parameters.Length - 1
		// length = (2 * parameters.Length) - 1
		var nodes = new SyntaxNodeOrToken [(2 * arguments.Length) - 1];
		var argsIndex = 0;
		var parametersIndex = 0;
		while (argsIndex < nodes.Length) {
			var currentarg = arguments [parametersIndex++];
			nodes [argsIndex++] = currentarg;
			if (argsIndex < nodes.Length - 1) {
				nodes [argsIndex++] = Token (SyntaxKind.CommaToken).WithTrailingTrivia (Space);
			}
		}

		return nodes;
	}

}
