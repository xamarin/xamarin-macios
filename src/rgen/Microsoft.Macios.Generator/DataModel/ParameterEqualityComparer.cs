using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Microsoft.Macios.Generator.DataModel;

class ParameterEqualityComparer : EqualityComparer<ImmutableArray<Parameter>> {

	/// <inheritdoc/>
	public override bool Equals (ImmutableArray<Parameter> x, ImmutableArray<Parameter> y)
	{
		// we know as a fact that parameter names have to be unique, otherwise we could not 
		// compile the code. So make sure that all the parameters with the same name are the same
		var xDictionary = Enumerable.ToDictionary (x, parameter => parameter.Name);
		var yDictionary = Enumerable.ToDictionary (y, parameter => parameter.Name);

		var comparer = new DictionaryComparer<string, Parameter> ();
		return comparer.Equals (xDictionary, yDictionary);
	}

	/// <inheritdoc/>
	public override int GetHashCode (ImmutableArray<Parameter> obj)
	{
		var hashCode = new HashCode ();
		foreach (var constructor in obj) {
			hashCode.Add (constructor);
		}
		return hashCode.ToHashCode ();
	}
}
