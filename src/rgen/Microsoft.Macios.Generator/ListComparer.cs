using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Macios.Generator;

public class ListComparer<T> : EqualityComparer<IList<T>> {
	readonly IComparer<T>? comparer;
	readonly IEqualityComparer<T> valueComparer;

	public ListComparer (IComparer<T>? sortComparer = null, IEqualityComparer<T>? equalityComparer = null)
	{
		comparer = sortComparer;
		valueComparer = equalityComparer ?? EqualityComparer<T>.Default;
	}

	/// <inheritdoc/>
	public override bool Equals (IList<T>? x, IList<T>? y)
	{
		// bases cases for null or diff size
		if (x is null && y is null)
			return true;
		if (x is null || y is null)
			return false;
		if (x.Count != y.Count)
			return false;

		// make copies of the lists and sort them
		var xSorted = x.ToArray ();
		if (comparer is not null)
			Array.Sort (xSorted, comparer);

		var ySorted = y.ToArray ();
		if (comparer is not null)
			Array.Sort (ySorted, comparer);

		for (var i = 0; i < xSorted.Length; i++) {
			if (!valueComparer.Equals (xSorted [i], ySorted [i]))
				return false;
		}
		return true;
	}

	/// <inheritdoc/>
	public override int GetHashCode (IList<T> obj)
	{
		var hash = new HashCode ();
		foreach (var element in obj) {
			hash.Add (element);
		}
		return hash.ToHashCode ();
	}
}
