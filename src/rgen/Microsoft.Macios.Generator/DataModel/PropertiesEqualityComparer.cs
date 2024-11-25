using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Microsoft.Macios.Generator.DataModel;

class PropertiesEqualityComparer : IEqualityComparer<ImmutableArray<Property>> {
	public bool Equals (ImmutableArray<Property> x, ImmutableArray<Property> y)
	{
		// properties are unique by their name, that means that we can build two dicts and comparethem
		var xDictionary = Enumerable.ToDictionary (x, property => property.Name);
		var yDictionary = Enumerable.ToDictionary (y, property => property.Name);

		var comparer = new DictionaryComparer<string, Property> ();
		return comparer.Equals (xDictionary, yDictionary);
	}

	public int GetHashCode (ImmutableArray<Property> obj)
	{
		var hash = new HashCode ();
		foreach (var property in obj) {
			hash.Add (property);
		}
		return hash.ToHashCode ();
	}
}
