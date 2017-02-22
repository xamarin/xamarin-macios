using System;
using UIKit;
using ObjCRuntime;
using Foundation;

namespace Bug35176 {
	
	[Introduced (PlatformName.iOS, 8,0)]
	[Introduced (PlatformName.MacOSX, 10,10)]
	[Protocol]
	interface FooInterface {

		[Abstract]
		[Export ("fooView")]
		UIView FooView { get; set; }

		[Export ("BarView")]
		UIView BarView { [Introduced (PlatformName.iOS, 9,0)] get; }

		[Export ("barMember:")]
		UIView GetBarMember (int x);
	}

	[BaseType (typeof (NSObject))]
	interface BarObject : FooInterface
	{

	}
}