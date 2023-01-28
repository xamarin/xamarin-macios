using System;
using Foundation;

namespace bug52570MethodInternal {

	[Category]
	[BaseType (typeof (FooObject))]
	interface FooObject_Extensions {

		[Static]
		[Internal]
		[Export ("someMethod:")]
		bool SomeMethod (NSRange range);
	}

	[BaseType (typeof (NSObject))]
	interface FooObject {

	}
}
