// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.Macios.Generator.DataModel;

/// <summary>
/// Structure that represents a change that was made by the user on a members attribute list that has to be
/// reflected in the generated code.
/// </summary>
readonly struct AttributeCodeChange : IEquatable<AttributeCodeChange> {
	/// <summary>
	/// Get the name of the attribute that was added.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// Get the arguments used to create the attribute.
	/// </summary>
	public ImmutableArray<string> Arguments { get; }

	/// <summary>
	/// Create a new change that happened on an attribute.
	/// </summary>
	/// <param name="name">The name of the attribute that was added.</param>
	/// <param name="arguments">The arguments used to create the attribute.</param>
	public AttributeCodeChange (string name, ImmutableArray<string> arguments)
	{
		Name = name;
		Arguments = arguments;
	}

	/// <summary>
	/// Create a new change that happened on an attribute with no parameters.
	/// </summary>
	/// <param name="name">The name of the attribute that was added.</param>
	public AttributeCodeChange (string name) : this (name, []) { }

	/// <inheritdoc />
	public bool Equals (AttributeCodeChange other)
	{
		if (Name != other.Name || Arguments.Length != other.Arguments.Length)
			return false;
		// arguments CANNOT be sorted, since the order of the arguments is important
		for (var index = 0; index < Arguments.Length; index++) {
			if (Arguments [index] != other.Arguments [index])
				return false;
		}

		return true;
	}

	/// <inheritdoc />
	public override bool Equals (object? obj)
	{
		return obj is AttributeCodeChange other && Equals (other);
	}

	/// <inheritdoc />
	public override int GetHashCode ()
	{
		return HashCode.Combine (Name, Arguments);
	}

	/// <summary>
	/// Compare two <see cref="AttributeCodeChange"/> instances for equality.
	/// </summary>
	/// <param name="x">Code change to compare.</param>
	/// <param name="y">Code change to compare.</param>
	/// <returns>True if the code changes are equal.</returns>
	public static bool operator == (AttributeCodeChange x, AttributeCodeChange y)
	{
		return x.Equals (y);
	}

	/// <summary>
	/// Compare two <see cref="AttributeCodeChange"/> instances for inequality.
	/// </summary>
	/// <param name="x">Code change to compare.</param>
	/// <param name="y">Code change to compare.</param>
	/// <returns>True if the objects are not equal.</returns>
	public static bool operator != (AttributeCodeChange x, AttributeCodeChange y)
	{
		return !(x == y);
	}

	public static ImmutableArray<AttributeCodeChange> From (SyntaxList<AttributeListSyntax> attributes,
		SemanticModel semanticModel)
	{
		var bucket = ImmutableArray.CreateBuilder<AttributeCodeChange> ();
		foreach (AttributeListSyntax attributeListSyntax in attributes) {
			foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes) {
				var x = semanticModel.GetSymbolInfo (attributeSyntax);
				if (semanticModel.GetSymbolInfo (attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
					continue; // if we can't get the symbol, ignore it
				var name = attributeSymbol.ContainingType.ToDisplayString ();
				var arguments = ImmutableArray.CreateBuilder<string> ();
				var argumentList = attributeSyntax.ArgumentList?.Arguments;
				if (argumentList is not null) {
					foreach (var argSyntax in argumentList) {
						// there are two types of argument nodes, those that are literal and those that
						// are a literal expression
						if (argSyntax.Expression is LiteralExpressionSyntax literalExpressionSyntax) {
							arguments.Add (literalExpressionSyntax.ToFullString ().Trim ()
								.Replace ("\"", string.Empty));
						}
						if (argSyntax.Expression is MemberAccessExpressionSyntax memberAccessExpressionSyntax) {
							var eumExpr = memberAccessExpressionSyntax.ToFullString ().Trim ();
							if (semanticModel.GetSymbolInfo (memberAccessExpressionSyntax).Symbol is IFieldSymbol
								enumSymbol) {
								arguments.Add (enumSymbol.ToDisplayString ().Trim ());
							} else {
								// could not get the symbol, add the full expre
								arguments.Add (eumExpr);
							}
						}
						if (argSyntax.Expression is TypeOfExpressionSyntax typeOfExpressionSyntax) {
							if (semanticModel.GetSymbolInfo (typeOfExpressionSyntax.Type).Symbol is INamedTypeSymbol typeSymbol) {
								arguments.Add (typeSymbol.ToDisplayString ().Trim ());
							}
						}
					}
				}

				bucket.Add (new (name, arguments.ToImmutable ()));
			}
		}

		return bucket.ToImmutable ();
	}

	/// <inheritdoc />
	public override string ToString ()
	{
		return $"{{ Name: {Name}, Arguments: [{string.Join (", ", Arguments)}] }}";
	}
}
