// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Microsoft.Macios.Generator.DataModel;

class MethodsEqualityComparer : EqualityComparer<ImmutableArray<Method>> {
	/// <inheritdoc/>
	public override bool Equals (ImmutableArray<Method> x, ImmutableArray<Method> y)
	{
		// to compare two lists, we need to do the following:
		// 1. Group all the methods by return type + name + param count, this way we
		//    can quickly know if they are any diff.
		// 2. Compare the methods in each of the lists 
		var xMethods = x.GroupBy (m => (ReturnType: m.ReturnType, Name: m.Name, ParameterCount: m.Parameters.Length))
			.ToDictionary (g => g.Key, g => g.ToList ());
		var yMethods = y.GroupBy (m => (ReturnType: m.ReturnType, Name: m.Name, ParameterCount: m.Parameters.Length))
			.ToDictionary (g => g.Key, g => g.ToList ());

		// create the dictionary comparer that will do the based comparison and relay on a list comparer for the
		// diff methods
		var dictionaryComparer =
			new DictionaryComparer<(TypeInfo ReturnType, string Name, int ParameterCount), List<Method>> (
				new CollectionComparer<Method> (new MethodComparer ()));
		return dictionaryComparer.Equals (xMethods, yMethods);
	}

	/// <inheritdoc/>
	public override int GetHashCode (ImmutableArray<Method> obj)
	{
		var hashCode = new HashCode ();
		foreach (var ctr in obj) {
			hashCode.Add (ctr);
		}

		return hashCode.ToHashCode ();
	}
}
