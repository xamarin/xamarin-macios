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

/// <summary>
/// Readonly struct that represent the changes that a user has made in a property.
/// </summary>
readonly struct Property : IEquatable<Property> {
	/// <summary>
	/// Name of the property.
	/// </summary>
	public string Name { get; } = string.Empty;

	/// <summary>
	/// String representation of the property type.
	/// </summary>
	public string Type { get; } = string.Empty;

	/// <summary>
	/// The platform availability of the enum value.
	/// </summary>
	public SymbolAvailability SymbolAvailability { get; }

	/// <summary>
	/// Get the attributes added to the member.
	/// </summary>
	public ImmutableArray<AttributeCodeChange> Attributes { get; } = [];

	/// <summary>
	/// Get the modifiers of the property.
	/// </summary>
	public ImmutableArray<SyntaxToken> Modifiers { get; } = [];

	/// <summary>
	/// Get the list of accessor changes of the property.
	/// </summary>
	public ImmutableArray<Accessor> Accessors { get; } = [];

	internal Property (string name, string type,
		SymbolAvailability symbolAvailability,
		ImmutableArray<AttributeCodeChange> attributes,
		ImmutableArray<SyntaxToken> modifiers, ImmutableArray<Accessor> accessors)
	{
		Name = name;
		Type = type;
		SymbolAvailability = symbolAvailability;
		Attributes = attributes;
		Modifiers = modifiers;
		Accessors = accessors;
	}

	/// <inheritdoc />
	public bool Equals (Property other)
	{
		// this could be a large && but ifs are more readable
		if (Name != other.Name)
			return false;
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

		var accessorComparer = new AccessorsEqualityComparer ();
		return accessorComparer.Equals (Accessors, other.Accessors);
	}

	/// <inheritdoc />
	public override bool Equals (object? obj)
	{
		return obj is Property other && Equals (other);
	}

	/// <inheritdoc />
	public override int GetHashCode ()
	{
		return HashCode.Combine (Name, Type, Attributes, Modifiers, Accessors);
	}

	public static bool operator == (Property left, Property right)
	{
		return left.Equals (right);
	}

	public static bool operator != (Property left, Property right)
	{
		return !left.Equals (right);
	}

	public static bool TryCreate (PropertyDeclarationSyntax declaration, SemanticModel semanticModel,
		[NotNullWhen (true)] out Property? change)
	{
		var memberName = declaration.Identifier.ToFullString ().Trim ();
		// get the symbol from the property declaration
		if (semanticModel.GetDeclaredSymbol (declaration) is not IPropertySymbol propertySymbol) {
			change = null;
			return false;
		}

		var propertySupportedPlatforms = propertySymbol.GetSupportedPlatforms ();

		var type = propertySymbol.Type.ToDisplayString ().Trim ();
		var attributes = declaration.GetAttributeCodeChanges (semanticModel);
		ImmutableArray<Accessor> accessorCodeChanges = [];
		if (declaration.AccessorList is not null && declaration.AccessorList.Accessors.Count > 0) {
			// calculate any possible changes in the accessors of the property
			var accessorsBucket = ImmutableArray.CreateBuilder<Accessor> ();
			foreach (var accessorDeclaration in declaration.AccessorList.Accessors) {
				if (semanticModel.GetDeclaredSymbol (accessorDeclaration) is not ISymbol accessorSymbol)
					continue;
				var kind = accessorDeclaration.Kind ().ToAccessorKind ();
				var accessorAttributeChanges = accessorDeclaration.GetAttributeCodeChanges (semanticModel);
				accessorsBucket.Add (new(kind, accessorSymbol.GetSupportedPlatforms (), accessorAttributeChanges,
					[.. accessorDeclaration.Modifiers]));
			}

			accessorCodeChanges = accessorsBucket.ToImmutable ();
		}

		if (declaration.ExpressionBody is not null) {
			// an expression body == a getter with no attrs or modifiers; that means that the accessor does not have
			// extra availability, but the ones form the property
			accessorCodeChanges = [
				new(AccessorKind.Getter, propertySupportedPlatforms, [], [])
			];
		}

		change = new(
			name: memberName,
			type: type,
			symbolAvailability: propertySupportedPlatforms,
			attributes: attributes,
			modifiers: [.. declaration.Modifiers],
			accessors: accessorCodeChanges);
		return true;
	}

	/// <inheritdoc />
	public override string ToString ()
	{
		var sb = new StringBuilder ($"Name: {Name}, Type: {Type}, Supported Platforms: {SymbolAvailability}, Attributes: [");
		sb.AppendJoin (",", Attributes);
		sb.Append ("], Modifiers: [");
		sb.AppendJoin (",", Modifiers.Select (x => x.Text));
		sb.Append ("], Accessors: [");
		sb.AppendJoin (",", Accessors);
		sb.Append (']');
		return sb.ToString ();
	}
}
