using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.Macios.Generator.DataModel;

class DeclarationCodeChangesEqualityComparer : IEqualityComparer<(BaseTypeDeclarationSyntax Declaration, CodeChanges
	Changes)> {
	readonly CodeChangesEqualityComparer comparer = new ();

	public bool Equals ((BaseTypeDeclarationSyntax Declaration, CodeChanges Changes) x,
		(BaseTypeDeclarationSyntax Declaration, CodeChanges Changes) y)
	{
		return comparer.Equals (x.Changes, y.Changes);
	}

	public int GetHashCode ((BaseTypeDeclarationSyntax Declaration, CodeChanges Changes) obj)
		=> comparer.GetHashCode (obj.Changes);
}

/// <summary>
/// Custom code changes comparer used for the Roslyn code generation to invalidate caching.
/// </summary>
class CodeChangesEqualityComparer : IEqualityComparer<CodeChanges> {
	/// <inheritdoc />
	public bool Equals (CodeChanges x, CodeChanges y)
	{
		// things that mean a code change is the same:
		// - the fully qualified symbol is the same
		// - the binding type is the same
		// - the syntax node type is the same
		// - the members are the same
		// - the attributes are the same

		// this could be a massive or but that makes it less readable
		if (x.FullyQualifiedSymbol != y.FullyQualifiedSymbol)
			return false;
		if (x.BindingType != y.BindingType)
			return false;
		if (x.Attributes.Length != y.Attributes.Length)
			return false;
		if (x.EnumMembers.Length != y.EnumMembers.Length)
			return false;
		if (x.Properties.Length != y.Properties.Length)
			return false;

		// compare the attrs, we need to sort them since attribute order does not matter
		var attrComparer = new AttributesEqualityComparer ();
		if (!attrComparer.Equals (x.Attributes, y.Attributes))
			return false;

		// compare the members
		var memberComparer = new EnumMembersEqualityComparer ();
		if (!memberComparer.Equals (x.EnumMembers, y.EnumMembers))
			return false;

		// compare properties
		var propertyComparer = new PropertiesEqualityComparer ();
		if (!propertyComparer.Equals (x.Properties, y.Properties))
			return false;

		// compare constructors
		var constructorComparer = new ConstructorsEqualityComparer ();
		if (!constructorComparer.Equals (x.Constructors, y.Constructors))
			return false;

		// compare events
		var eventComparer = new EventEqualityComparer ();
		if (!eventComparer.Equals (x.Events, y.Events))
			return false;

		var methodComparer = new MethodsEqualityComparer ();
		return methodComparer.Equals (x.Methods, y.Methods);
	}

	/// <inheritdoc />
	public int GetHashCode (CodeChanges obj)
	{
		return HashCode.Combine (obj.FullyQualifiedSymbol, obj.EnumMembers);
	}
}
