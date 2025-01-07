using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Microsoft.Macios.Generator.DataModel;

class EventEqualityComparer : EqualityComparer<ImmutableArray<Event>> {

	/// <inheritdoc/>
	public override bool Equals (ImmutableArray<Event> x, ImmutableArray<Event> y)
	{
		// properties are unique by their name, that means that we can build two dicts and comparethem
		var xDictionary = Enumerable.ToDictionary (x, property => property.Name);
		var yDictionary = Enumerable.ToDictionary (y, property => property.Name);

		var comparer = new DictionaryComparer<string, Event> ();
		return comparer.Equals (xDictionary, yDictionary);
	}

	/// <inheritdoc/>
	public override int GetHashCode (ImmutableArray<Event> obj)
	{
		var hash = new HashCode ();
		foreach (var property in obj) {
			hash.Add (property);
		}
		return hash.ToHashCode ();
	}
}
