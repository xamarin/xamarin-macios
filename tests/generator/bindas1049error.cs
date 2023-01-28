using System;
using System.Drawing;
using Foundation;
using CoreMedia;
using ObjCRuntime;

namespace BindAs1049ErrorTests {

	[BaseType (typeof (NSObject))]
	interface MyFooClass {

		[return: BindAs (typeof (string))]
		[Export ("stringMethod:")]
		NSNumber StringMethod (int arg1);

	}
}
