using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Microsoft.Macios.Generator.DataModel;

class PropertyComparer : IEqualityComparer<ImmutableArray<PropertyCodeChange>> {
	public bool Equals (ImmutableArray<PropertyCodeChange> x, ImmutableArray<PropertyCodeChange> y)
	{
		// properties are unique by their name, that means that we can build two dicts and comparethem
		var xDictionary = Enumerable.ToDictionary (x, property => property.Name);
		var yDictionary = Enumerable.ToDictionary (y, property => property.Name);

		var comparer = new DictionaryComparer<string, PropertyCodeChange> ();
		return comparer.Equals (xDictionary, yDictionary);
	}

	public int GetHashCode (ImmutableArray<PropertyCodeChange> obj)
	{
		var hash = new HashCode ();
		foreach (var property in obj) {
			hash.Add (property);	
		}
		return hash.ToHashCode ();
	}
}
