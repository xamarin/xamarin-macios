using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.CoreAnimation;

namespace BindAs1050ProtocolErrorTests {

	[Protocol]
	interface MyFooClass {

		[return: BindAs (typeof (CAScroll? []))]
		[Export ("getScrollArrayEnum2:")]
		NSNumber [] GetScrollArrayFooEnumArray2 ([BindAs (typeof (CAScroll []))] NSNumber [] arg1);
		
	}
}