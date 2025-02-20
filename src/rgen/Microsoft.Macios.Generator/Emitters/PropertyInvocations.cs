// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.Macios.Generator.Emitters;

/// <summary>
/// Represents all the invocations of a property.
/// </summary>
readonly record struct PropertyInvocations {

	/// <summary>
	/// Invocations for the getter.
	/// </summary>
	public (StatementSyntax Send, StatementSyntax SendSuper) Getter { get; init; }

	/// <summary>
	/// Invocations for the setter.
	/// </summary>
	public (StatementSyntax Send, StatementSyntax SendSuper) Setter { get; init; }
}
