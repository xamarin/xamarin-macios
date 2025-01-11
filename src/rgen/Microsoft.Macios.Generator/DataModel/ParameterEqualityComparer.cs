// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Microsoft.Macios.Generator.DataModel;

class ParameterEqualityComparer<T> (Func<T, string> getName) : EqualityComparer<ImmutableArray<T>> {
	/// <inheritdoc/>
	public override bool Equals (ImmutableArray<T> x, ImmutableArray<T> y)
	{
		// we know as a fact that parameter names have to be unique, otherwise we could not 
		// compile the code. So make sure that all the parameters with the same name are the same
		var xDictionary = Enumerable.ToDictionary (x, getName);
		var yDictionary = Enumerable.ToDictionary (y, getName);

		var comparer = new DictionaryComparer<string, T> ();
		return comparer.Equals (xDictionary, yDictionary);
	}

	/// <inheritdoc/>
	public override int GetHashCode (ImmutableArray<T> obj)
	{
		var hashCode = new HashCode ();
		foreach (var parameter in obj) {
			hashCode.Add (parameter);
		}
		return hashCode.ToHashCode ();
	}
}

class MethodParameterEqualityComparer () : ParameterEqualityComparer<Parameter> (GetName) {
	static string GetName (Parameter p) => p.Name;
}

class DelegateParameterEqualityComparer () : ParameterEqualityComparer<DelegateParameter> (GetName) {
	static string GetName (DelegateParameter p) => p.Name;
}
