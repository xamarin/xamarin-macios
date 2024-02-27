using System;
using Foundation;

namespace Bug42855Tests {

	[Protocol]
	[Model]
	interface MyFooClass {

		[Abstract]
		[Export ("stringMethod:")]
		NSString StringMethod (int arg1);

		[Export ("stringMethod2:")]
		NSString StringMethod2 (int arg1);
	}
}
