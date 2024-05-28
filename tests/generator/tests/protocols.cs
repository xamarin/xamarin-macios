using System;
using Foundation;
using ObjCRuntime;

namespace Protocols {
	[Protocol]
	interface ProtocolWithConstructors {
		[Abstract]
		[Export ("init")]
		NativeHandle Constructor ();

		[Export ("initWithValue:")]
		NativeHandle Constructor (string p0);

		[Export ("initWithError:")]
		NativeHandle Constructor (out NSError error);

		[Bind ("Create")]
		[Export ("initWithCustomName:")]
		NativeHandle Constructor (NSDate error);
	}
}
