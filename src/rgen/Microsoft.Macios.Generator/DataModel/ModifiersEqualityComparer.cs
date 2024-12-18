using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Generator.DataModel;

public class ModifiersEqualityComparer : EqualityComparer<ImmutableArray<SyntaxToken>> {
	/// <inheritdoc/>
	public override bool Equals (ImmutableArray<SyntaxToken> x, ImmutableArray<SyntaxToken> y)
	{
		if (x.Length != y.Length)
			return false;
		// order does not matter, the language does have rules for that, but we can ignore them
		var xOrdered = x.Select (t => t.Text).Order ().ToArray ();
		var yOrdered = y.Select (t => t.Text).Order ().ToArray ();
		for (var i = 0; i < x.Length; i++) {
			if (xOrdered [i] != yOrdered [i])
				return false;
		}
		return true;
	}

	/// <inheritdoc/>
	public override int GetHashCode (ImmutableArray<SyntaxToken> obj)
	{
		var hash = new HashCode ();
		foreach (var token in obj) {
			hash.Add (token);
		}
		return hash.ToHashCode ();
	}
}
