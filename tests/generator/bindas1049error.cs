using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.CoreMedia;

namespace BindAs1049ErrorTests {

	[BaseType (typeof (NSObject))]
	interface MyFooClass {

		[return: BindAs (typeof (string))]
		[Export ("stringMethod:")]
		NSNumber StringMethod (int arg1);
		
	}
}