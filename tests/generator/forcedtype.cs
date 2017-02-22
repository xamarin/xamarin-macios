using System;
using Foundation;
using CoreFoundation;
using UIKit;

namespace Forced {
	delegate void SomethingDelegate ([ForcedType] UIButton button);

	[BaseType (typeof (NSObject))]
	interface Foo {

		[Export ("barButton")]
		[ForcedType]
		UIButton BarButton { get; set; }

		[Static]
		[return: ForcedType]
		[Export ("fooType:")]
		UIButton FromFoo (string fooType);

		[Export ("getBar:")]
		[return: ForcedType]
		UIButton GetBar (string bar);

		[Export ("getSomething:")]
		UIButton GetSomething (SomethingDelegate handler);

		[Export ("getSomethingElse:label:view:")]
		UIButton GetSomethingElse ([ForcedType] out UIButton button, [ForcedType] ref UILabel label, ref UIView view);

		[Export ("getAll:label:view:")]
		[return: ForcedType (true)]
		UIButton GetAll ([ForcedType (true)] out UIButton button, [ForcedType] ref UILabel label, ref UIView view);
	}
}
