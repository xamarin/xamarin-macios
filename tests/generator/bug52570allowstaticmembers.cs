using System;
using Foundation;

namespace bug52570AllowStaticMembers {

	[Category (allowStaticMembers: true)]
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
