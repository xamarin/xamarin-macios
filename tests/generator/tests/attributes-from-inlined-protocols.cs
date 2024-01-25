using System;

using Foundation;

using ObjCRuntime;

namespace NS {
	[NoTV]
	[BaseType (typeof (NSObject))]
	interface TypeA : ProtocolA {
	}

	[Introduced (PlatformName.iOS, 11, 0)]
	[Introduced (PlatformName.MacCatalyst, 13, 1)]
	[Introduced (PlatformName.TvOS, 11, 0)]
	[Protocol]
	interface ProtocolA {
		[Introduced (PlatformName.iOS, 12, 0)]
		[Introduced (PlatformName.MacCatalyst, 13, 1)]
		[Introduced (PlatformName.TvOS, 12, 0)]
		[Export ("someMethod1:")]
		void SomeMethod1 (nint row);

		[Introduced (PlatformName.iOS, 12, 0)]
		[Introduced (PlatformName.MacCatalyst, 13, 1)]
		[Export ("someMethod2:")]
		void SomeMethod2 (nint row);

		[Introduced (PlatformName.TvOS, 12, 0)]
		[Export ("someMethod3:")]
		void SomeMethod3 (nint row);

		[Export ("someMethod4:")]
		void SomeMethod4 (nint row);
	}
}
