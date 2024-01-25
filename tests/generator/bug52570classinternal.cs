using System;

using Foundation;

namespace Bug52570ClassInternal {

	[Category]
	[Internal]
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
