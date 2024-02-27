using System;
using Foundation;

namespace NS {
	[BaseType (typeof (NSObject))]
	interface OutNSObject {
		[Export ("func:")]
		void Func (out ErrorObject error);
	}

	[BaseType (typeof (NSObject))]
	interface ErrorObject {

	}
}
