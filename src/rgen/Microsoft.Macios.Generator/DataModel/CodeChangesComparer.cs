using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Macios.Generator.DataModel;

/// <summary>
/// Custom code changes comparer used for the Roslyn code generation to invalidate caching.
/// </summary>
class CodeChangesComparer : IEqualityComparer<CodeChanges> {

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
		if (x.SymbolDeclaration.GetType () != y.SymbolDeclaration.GetType ())
			return false;
		if (x.Attributes.Length != y.Attributes.Length)
			return false;
		if (x.Members.Length != y.Members.Length)
			return false;

		// compare the attrs, we need to sort them since attribute order does not matter
		var xAttrs = x.Attributes.OrderBy (a => a.Name).ToArray ();
		var yAttrs = y.Attributes.OrderBy (a => a.Name).ToArray ();
		for (var index = 0; index < xAttrs.Length; index++) {
			if (xAttrs [index] != yAttrs [index])
				return false;
		}

		// compare the members, we need to sort them since member order does not matter
		var xMembers = x.Members.OrderBy (m => m.Name).ToArray ();
		var yMembers = y.Members.OrderBy (m => m.Name).ToArray ();
		for (var index = 0; index < xMembers.Length; index++) {
			if (xMembers [index] != yMembers [index])
				return false;
		}

		return true;
	}

	/// <inheritdoc />
	public int GetHashCode (CodeChanges obj)
	{
		return HashCode.Combine (obj.FullyQualifiedSymbol, obj.Members);
	}
}
