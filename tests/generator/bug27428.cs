using System;
using System.Drawing;

using ObjCRuntime;
using Foundation;
using UIKit;

namespace Foo.BindingBugs {
	[BaseType (typeof (NSObject))]
	interface Widget {
		[Export ("doSomething:atIndex:")]
		void DoSomething (NSObject @object, int index);
	}
}
