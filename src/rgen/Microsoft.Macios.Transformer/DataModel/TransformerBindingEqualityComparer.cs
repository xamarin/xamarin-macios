// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Macios.Generator;
using Microsoft.Macios.Generator.DataModel;

namespace Microsoft.Macios.Transformer.DataModel;

class TransformerBindingEqualityComparer : BindingEqualityComparer {

	public override bool Equals (Binding x, Binding y)
	{
		// call the base, if they are equals, compare the new properties
		if (!base.Equals (x, y))
			return false;

		// comparer for when order does not matter.
		var ignoreOrderComparer = new CollectionComparer<string> (StringComparer.InvariantCulture);
		if (!ignoreOrderComparer.Equals (x.Protocols, y.Protocols))
			return false;
		return true;
	}
}
