using System;

#if XAMCORE_2_0
using Foundation;
#else
using MonoMac.Foundation;
#endif

namespace warnaserrorTests {

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