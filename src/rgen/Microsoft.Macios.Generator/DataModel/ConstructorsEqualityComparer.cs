using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Microsoft.Macios.Generator.DataModel;

class ConstructorsEqualityComparer : EqualityComparer<ImmutableArray<Constructor>> {
	/// <inheritdoc/>
	public override bool Equals (ImmutableArray<Constructor> x, ImmutableArray<Constructor> y)
	{
		// group the constructors based on the number of parameters. We create two dictionaries, that will have 
		// the number of params as the key, and a list of constructors as the value
		var xConstructors = x.GroupBy (c => c.Parameters.Length).ToDictionary (g => g.Key, g => g.ToList ());
		var yConstructors = y.GroupBy (c => c.Parameters.Length).ToDictionary (g => g.Key, g => g.ToList ());

		// create a dictionary compare that will take a special comparer for the list of constructors
		var dictionaryComparer =
			new DictionaryComparer<int, List<Constructor>> (new CollectionComparer<Constructor> (new ConstructorComparer ()));
		return dictionaryComparer.Equals (xConstructors, yConstructors);
	}

	/// <inheritdoc/>
	public override int GetHashCode (ImmutableArray<Constructor> obj)
	{
		var hashCode = new HashCode ();
		foreach (var ctr in obj) {
			hashCode.Add (ctr);
		}
		return hashCode.ToHashCode ();
	}
}
