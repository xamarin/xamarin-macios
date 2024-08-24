using System;

using Foundation;
using ObjCRuntime;

namespace NS {
	[return: NullAllowed]
	delegate NSObject MyCallback ([NullAllowed] NSObject obj);

	[BaseType (typeof (NSObject))]
	interface Widget {
		[Export ("foo")]
		[NullAllowed]
		MyCallback Foo { get; set; }
	}
}
