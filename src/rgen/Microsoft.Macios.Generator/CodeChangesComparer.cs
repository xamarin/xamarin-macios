using System;
using System.Collections.Generic;

namespace Microsoft.Macios.Generator;

/// <summary>
/// Custom code changes comparer used for the Roslyn code generation to invalidate caching.
/// </summary>
class CodeChangesComparer : IEqualityComparer<CodeChanges> {

	public bool Equals(CodeChanges x, CodeChanges y)
	{
		// we want to ignore the syntax declaration, is not something we want to compare since it can change,
		// for example, with new attributes or comments.
		if (x.FullyQualifiedSymbol != y.FullyQualifiedSymbol || x.Members.Length != y.Members.Length)
			return false;

		// compare arrays of members
		var xMembers = x.Members.Sort();
		var yMembers = y.Members.Sort();
		for (int i = 0; i < xMembers.Length; i++) {
			if (xMembers[i] != yMembers[i]) {
				return false;
			}
		}

		return true;
	}

	public int GetHashCode(CodeChanges obj)
	{
		return HashCode.Combine(obj.FullyQualifiedSymbol, obj.Members);
	}
}
