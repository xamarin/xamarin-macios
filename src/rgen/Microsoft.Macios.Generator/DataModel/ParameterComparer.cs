// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;

namespace Microsoft.Macios.Generator.DataModel;

class ParameterComparer : IComparer<Parameter> {
	public int Compare (Parameter x, Parameter y)
	{
		var value = x.Position.CompareTo (y.Position);
		if (value != 0)
			return value;
		var comparer = new TypeInfoComparer ();
		value = comparer.Compare (x.Type, y.Type);
		if (value != 0)
			return value;
		value = String.Compare (x.Name, y.Name, StringComparison.Ordinal);
		if (value != 0)
			return value;
		var xValues = new [] { x.IsOptional, x.IsParams, x.IsThis };
		var yValues = new [] { y.IsOptional, y.IsParams, y.IsThis };
		for (int i = 0; i < xValues.Length; ++i) {
			value = xValues [i].CompareTo (yValues [i]);
			if (value != 0)
				return value;
		}

		return x.ReferenceKind.CompareTo (y.ReferenceKind);
	}
}
