using System;
using Foundation;
using ObjCRuntime;

namespace NS {
#if NET
	[Unavailable (PlatformName.iOS)]
#else
	[Unavailable (PlatformName.iOS, ObjCRuntime.PlatformArchitecture.All)]
#endif
	[Protocol]
	interface MyProtocol {
	}

	[BaseType (typeof (NSObject))]
	interface MyClass : MyProtocol {
	}
}
