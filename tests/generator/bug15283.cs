using System;
using System.Drawing;
using MonoTouch.ObjCRuntime;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Runtime.InteropServices;

namespace BindingTests
{
	delegate NSObject MyBlockToBind (NSObject[] keys);

	[BaseType (typeof (NSObject))]
	interface TestInterface {
		[Export ("testMethod:")]
		void TestMethod (MyBlockToBind myBlockToBind);
	}
}

