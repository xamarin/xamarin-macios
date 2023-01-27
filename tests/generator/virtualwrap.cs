using System;
using UIKit;
using ObjCRuntime;
using Foundation;

namespace WrapTest {

	[BaseType (typeof (NSObject))]
	interface MyFooClass {

		[Export ("fooString")]
		string FooString { get; }

		[Wrap ("(NSString) FooString", isVirtual: true)]
		NSString FooNSString { get; }

		[Wrap ("(NSString) FooString")]
		NSString FooNSStringN { get; }

		[Export ("fooWithContentsOfURL:")]
		void FromUrl (NSUrl url);

		[Wrap ("FromUrl (NSUrl.FromString (url))", isVirtual: true)]
		void FromUrl (string url);

		[Wrap ("FromUrl (NSUrl.FromString (url))")]
		void FromUrlN (string url);
	}
}
