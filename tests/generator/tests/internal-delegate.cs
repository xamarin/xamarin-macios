using System;
using Foundation;

namespace NS {
	[Internal]
	delegate void Whatever (SomeType value);

	[Internal]
	[BaseType (typeof (NSObject))]
	interface SomeType {
		[Export ("whatever")]
		Whatever Nope { get; set; }
	}
}

