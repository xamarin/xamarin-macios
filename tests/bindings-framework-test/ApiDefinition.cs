using System;

#if __UNIFIED__
using ObjCRuntime;
using Foundation;
using UIKit;
#else
using MonoTouch.ObjCRuntime;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif

namespace Bindings.Test {
#if __UNIFIED__
	[BaseType (typeof (NSObject))]
	public interface FrameworkTest
	{
		[Export ("func")]
		int Func ();
	}
#endif
}
