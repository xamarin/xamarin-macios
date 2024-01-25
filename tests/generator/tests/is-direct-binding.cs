using System;

using Foundation;

namespace NS {
	[BaseType (typeof (NSObject))]
	interface C {
	}

	[Sealed]
	[BaseType (typeof (NSObject))]
	interface S {
	}

	[Model]
	[Protocol]
	[BaseType (typeof (NSObject))]
	interface P {
	}
}
