using System;
using Foundation;

namespace nowarnTests {

	[Category]
	[BaseType (typeof (FooObject))]
	interface FooObject_Extensions {

		[Static]
		[Export ("someMethod:")]
		bool SomeMethod (NSRange range);
	}

	[BaseType (typeof (NSObject))]
	interface FooObject {

	}
}
