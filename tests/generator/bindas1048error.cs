using System;
using System.Drawing;
using Foundation;
using CoreMedia;
using ObjCRuntime;

namespace BindAs1048ErrorTests {

	[BaseType (typeof (NSObject))]
	interface MyFooClass {

		[return: BindAs (typeof (string))]
		[Export ("stringMethod:")]
		NSString StringMethod (int arg1);

	}
}
