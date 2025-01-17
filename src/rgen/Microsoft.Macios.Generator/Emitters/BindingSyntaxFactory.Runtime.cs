// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.Macios.Generator.Emitters;

static partial class BindingSyntaxFactory {
	readonly static string Runtime = "Runtime";

	public static CompilationUnitSyntax GetNSObject (string nsObjectType, ArgumentListSyntax argumentList,
		bool suppressNullableWarning = false)
		=> StaticInvocationGenericExpression (Runtime, "GetNSObject", nsObjectType, argumentList,
			suppressNullableWarning);
}
