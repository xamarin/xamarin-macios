using System;
using System.Drawing;

using MonoTouch.ObjCRuntime;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Foo.BindingBugs
{
	[BaseType (typeof (NSObject))]
	interface Widget {
		[Export ("doSomething:atIndex:")]
		void DoSomething (NSObject @object, int index);
	}
}

