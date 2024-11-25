using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Extensions;

namespace Microsoft.Macios.Generator.DataModel;

/// <summary>
/// Readonly struct that represent the changes that a user has made in a property.
/// </summary>
readonly struct PropertyCodeChange : IEquatable<PropertyCodeChange> {
	/// <summary>
	/// Name of the property.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// String representation of the property type.
	/// </summary>
	public string Type { get; }

	/// <summary>
	/// Get the attributes added to the member.
	/// </summary>
	public ImmutableArray<AttributeCodeChange> Attributes { get; }

	/// <summary>
	/// Get the modifiers of the property.
	/// </summary>
	public ImmutableArray<SyntaxToken> Modifiers { get; }

	/// <summary>
	/// Get the list of accessor changes of the property.
	/// </summary>
	public ImmutableArray<PropertyAccessorCodeChange> Accessors { get; } = [];

	internal PropertyCodeChange (string name, string type, ImmutableArray<AttributeCodeChange> attributes,
		ImmutableArray<SyntaxToken> modifiers, ImmutableArray<PropertyAccessorCodeChange> accessors)
	{
		Name = name;
		Type = type;
		Attributes = attributes;
		Modifiers = modifiers;
		Accessors = accessors;
	}

	/// <inheritdoc />
	public bool Equals (PropertyCodeChange other)
	{
		// this could be a large && but ifs are more readable
		if (Name != other.Name)
			return false;
		if (Type != other.Type)
			return false;
		var attrsComparer = new AttributesComparer ();
		if (!attrsComparer.Equals (Attributes, other.Attributes))
			return false;

		var modifiersComparer = new ModifiersComparer ();
		if (!modifiersComparer.Equals (Modifiers, other.Modifiers))
			return false;

		var accessorComparer = new PropertyAccessorsComparer ();
		return accessorComparer.Equals (Accessors, other.Accessors);
	}

	/// <inheritdoc />
	public override bool Equals (object? obj)
	{
		return obj is PropertyCodeChange other && Equals (other);
	}

	/// <inheritdoc />
	public override int GetHashCode ()
	{
		return HashCode.Combine (Name, Type, Attributes, Modifiers, Accessors);
	}

	public static bool operator == (PropertyCodeChange left, PropertyCodeChange right)
	{
		return left.Equals (right);
	}

	public static bool operator != (PropertyCodeChange left, PropertyCodeChange right)
	{
		return !left.Equals (right);
	}

	public static bool TryCreate (PropertyDeclarationSyntax declaration, SemanticModel semanticModel,
		[NotNullWhen (true)] out PropertyCodeChange? change)
	{
		var memberName = declaration.Identifier.ToFullString ().Trim ();
		// get the symbol from the property declaration
		if (semanticModel.GetDeclaredSymbol (declaration) is not IPropertySymbol propertySymbol) {
			change = null;
			return false;
		}

		var type = propertySymbol.Type.ToDisplayString ().Trim ();
		var attributes = declaration.GetAttributeCodeChanges (semanticModel);
		ImmutableArray<PropertyAccessorCodeChange> accessorCodeChanges = [];
		if (declaration.AccessorList is not null && declaration.AccessorList.Accessors.Count > 0) {
			// calculate any possible changes in the accessors of the property
			var accessorsBucket = ImmutableArray.CreateBuilder<PropertyAccessorCodeChange> ();
			foreach (var accessor in declaration.AccessorList.Accessors) {
				var kind = accessor.Kind ().ToAccessorKind ();
				var accessorAttributeChanges = accessor.GetAttributeCodeChanges (semanticModel);
				accessorsBucket.Add (new (kind, accessorAttributeChanges, [.. accessor.Modifiers]));
			}

			accessorCodeChanges = accessorsBucket.ToImmutable ();
		}

		if (declaration.ExpressionBody is not null) {
			// an expression body == a getter with no attrs or modifiers
			accessorCodeChanges = [
				new (AccessorKind.Getter, [], [])
			];
		}

		change = new (memberName, type, attributes, [.. declaration.Modifiers], accessorCodeChanges);
		return true;
	}

	/// <inheritdoc />
	public override string ToString ()
	{
		var sb = new StringBuilder ($"Name: {Name}, Type: {Type}, Attributes: [");
		sb.AppendJoin (",", Attributes);
		sb.Append ("], Modifiers: [");
		sb.AppendJoin (",", Modifiers.Select (x => x.Text));
		sb.Append ("], Accessors: [");
		sb.AppendJoin (",", Accessors);
		sb.Append (']');
		return sb.ToString ();
	}
}
