using System;
using System.Collections.Generic;

#nullable enable

public static class CollectionsExtensions {

	public static T [] DropLast<T> (this T [] arr)
	{
		if (arr.Length == 0)
			return Array.Empty<T> ();

		T [] res = new T [arr.Length - 1];
		Array.Copy (arr, res, res.Length);
		return res;
	}

	public static IEnumerable<T> Yield<T> (this T item)
	{
		yield return item;
	}
}
