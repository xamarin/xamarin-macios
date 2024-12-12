using System;
using System.Collections.Generic;

namespace Microsoft.Macios.Generator;

public class ListComparer<T> : IEqualityComparer<List<T>> {
	readonly IComparer<T> comparer;
	readonly IEqualityComparer<T> valueComparer;

	public ListComparer (IComparer<T> sortComparer, IEqualityComparer<T>? equalityComparer = null)
	{
		comparer = sortComparer ?? throw new ArgumentNullException (nameof (sortComparer));
		valueComparer = equalityComparer ?? EqualityComparer<T>.Default;
	}

	public bool Equals (List<T>? x, List<T>? y)
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
		Array.Sort (xSorted, comparer);

		var ySorted = y.ToArray ();
		Array.Sort (ySorted, comparer);

		for (var i = 0; i < xSorted.Length; i++) {
			if (!valueComparer.Equals (xSorted [i], ySorted [i]))
				return false;
		}
		return true;
	}

	public int GetHashCode (List<T> obj)
	{
		var hash = new HashCode ();
		foreach (var element in obj) {
			hash.Add (element);
		}
		return hash.ToHashCode ();
	}
}
