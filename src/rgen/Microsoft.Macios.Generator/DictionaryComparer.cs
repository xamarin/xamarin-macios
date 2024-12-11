using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Macios.Generator;

public class DictionaryComparer<TKey, TValue> (IEqualityComparer<TValue>? valueComparer = null)
	: IEqualityComparer<Dictionary<TKey, TValue>>
	where TKey : notnull {
	readonly IEqualityComparer<TValue> valueComparer = valueComparer ?? EqualityComparer<TValue>.Default;

	public bool Equals (Dictionary<TKey, TValue>? x, Dictionary<TKey, TValue>? y)
	{
		if (x is null && y is null)
			return true;
		if (x is null || y is null)
			return false;
		if (x.Count != y.Count)
			return false;
		if (x.Keys.Except (y.Keys).Any ())
			return false;
		if (y.Keys.Except (x.Keys).Any ())
			return false;
		return x.All (pair => valueComparer.Equals (pair.Value, y [pair.Key]));
	}

	public int GetHashCode (Dictionary<TKey, TValue> obj)
	{
		var hash = new HashCode ();
		foreach (var (key, value) in obj) {
			hash.Add (key.GetHashCode ());
			if (value is not null)
				hash.Add (value.GetHashCode ());
		}
		return hash.ToHashCode ();
	}
}
