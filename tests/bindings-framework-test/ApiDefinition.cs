using System;

using ObjCRuntime;
using Foundation;

namespace Bindings.Test {
	[BaseType (typeof (NSObject))]
	public interface FrameworkTest {
		[Export ("func")]
		int Func ();
	}
}
