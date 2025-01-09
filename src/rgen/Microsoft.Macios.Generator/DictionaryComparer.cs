// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Macios.Generator;

public class DictionaryComparer<TKey, TValue> (IEqualityComparer<TValue>? valueComparer = null)
	: EqualityComparer<IDictionary<TKey, TValue>>
	where TKey : notnull {
	readonly IEqualityComparer<TValue> valueComparer = valueComparer ?? EqualityComparer<TValue>.Default;

	/// <inheritdoc/>
	public override bool Equals (IDictionary<TKey, TValue>? x, IDictionary<TKey, TValue>? y)
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

	/// <inheritdoc/>
	public override int GetHashCode (IDictionary<TKey, TValue> obj)
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
