using System;

using ObjCRuntime;
using Foundation;
using UIKit;

namespace Bindings.Test {
	[BaseType (typeof (NSObject))]
	public interface FrameworkTest
	{
		[Export ("func")]
		int Func ();
	}
}
