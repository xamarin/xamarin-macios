using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.Extensions;

namespace Microsoft.Macios.Generator.DataModel;

readonly struct Constructor : IEquatable<Constructor> {
	/// <summary>
	/// Type name that owns the constructor.
	/// </summary>
	public string Type { get; }

	/// <summary>
	/// The platform availability of the constructor.
	/// </summary>
	public SymbolAvailability SymbolAvailability { get; }

	/// <summary>
	/// Get the attributes added to the constructor.
	/// </summary>
	public ImmutableArray<AttributeCodeChange> Attributes { get; } = [];

	/// <summary>
	/// Modifiers list.
	/// </summary>
	public ImmutableArray<SyntaxToken> Modifiers { get; } = [];

	/// <summary>
	/// Parameters list.
	/// </summary>
	public ImmutableArray<Parameter> Parameters { get; } = [];

	public Constructor (string type,
		SymbolAvailability symbolAvailability,
		ImmutableArray<AttributeCodeChange> attributes,
		ImmutableArray<SyntaxToken> modifiers,
		ImmutableArray<Parameter> parameters)
	{
		Type = type;
		SymbolAvailability = symbolAvailability;
		Attributes = attributes;
		Modifiers = modifiers;
		Parameters = parameters;
	}

	public static bool TryCreate (ConstructorDeclarationSyntax declaration, SemanticModel semanticModel,
		[NotNullWhen (true)] out Constructor? change)
	{
		if (semanticModel.GetDeclaredSymbol (declaration) is not IMethodSymbol constructor) {
			change = null;
			return false;
		}

		var attributes = declaration.GetAttributeCodeChanges (semanticModel);
		var parametersBucket = ImmutableArray.CreateBuilder<Parameter> ();
		// loop over the parameters of the construct since changes on those implies a change in the generated code
		foreach (var parameter in constructor.Parameters) {
			var parameterDeclaration = declaration.ParameterList.Parameters [parameter.Ordinal];
			if (!Parameter.TryCreate (parameter, parameterDeclaration, semanticModel, out var parameterChange))
				continue;
			parametersBucket.Add (parameterChange.Value);
		}

		change = new (
			type: constructor.ContainingSymbol.ToDisplayString ().Trim (), // we want the full name
			symbolAvailability: constructor.GetSupportedPlatforms (),
			attributes: attributes,
			modifiers: [.. declaration.Modifiers],
			parameters: parametersBucket.ToImmutable ());
		return true;
	}

	/// <inheritdoc/>
	public bool Equals (Constructor other)
	{
		if (Type != other.Type)
			return false;
		if (SymbolAvailability != other.SymbolAvailability)
			return false;

		var attrsComparer = new AttributesEqualityComparer ();
		if (!attrsComparer.Equals (Attributes, other.Attributes))
			return false;
		var modifiersComparer = new ModifiersEqualityComparer ();
		if (!modifiersComparer.Equals (Modifiers, other.Modifiers))
			return false;

		var paramComparer = new ParameterEqualityComparer ();
		return paramComparer.Equals (Parameters, other.Parameters);
	}

	/// <inheritdoc/>
	public override bool Equals (object? obj)
	{
		return obj is Constructor other && Equals (other);
	}

	/// <inheritdoc/>
	public override int GetHashCode ()
	{
		var hashCode = new HashCode ();
		hashCode.Add (Type);
		hashCode.Add (SymbolAvailability);
		foreach (var modifier in Modifiers) {
			hashCode.Add (modifier);
		}

		foreach (var attr in Attributes) {
			hashCode.Add (attr);
		}

		foreach (var parameter in Parameters) {
			hashCode.Add (parameter);
		}

		return hashCode.ToHashCode ();
	}

	public static bool operator == (Constructor left, Constructor right)
	{
		return left.Equals (right);
	}

	public static bool operator != (Constructor left, Constructor right)
	{
		return !left.Equals (right);
	}

	/// <inheritdoc/>
	public override string ToString ()
	{
		var sb = new StringBuilder ($"{{ Ctr: Type: {Type}, ");
		sb.Append ("Attributes: [");
		sb.AppendJoin (", ", Attributes);
		sb.Append ("], Modifiers: [");
		sb.AppendJoin (", ", Modifiers.Select (x => x.Text));
		sb.Append ("], Parameters: [");
		sb.AppendJoin (", ", Parameters);
		sb.Append ("] }}");
		return sb.ToString ();
	}
}
