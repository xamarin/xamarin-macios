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

		[Export ("wrappedValue:")]
		[Wrap ("value + 1", IsVirtual = true)]
		int GetWrappedValue (int value);

		[Export ("wrappedValueNonVirtual:")]
		[Wrap ("value + 1")]
		int GetWrappedValueNonVirtual (int value);

		[Static]
		[Export ("wrappedValueStatic:")]
		[Wrap ("value + 1", IsVirtual = true)]
		int GetWrappedValueStatic (int value);
	}

	[Protocol]
	interface WrappedProtocol {
		[Abstract (GenerateExtensionMethod = true)]
		[Export ("wrappedName")]
		[Wrap ("this.GetType ().Name", IsVirtual = true)]
		string GetWrappedName ();

		[Export ("wrappedValue:")]
		[Wrap ("value + 1", IsVirtual = true)]
		int GetWrappedValue (int value);

		[Abstract (GenerateExtensionMethod = true)]
		[Export ("wrappedSetValue:")]
		[Wrap ("Console.WriteLine (value)", IsVirtual = true)]
		void SetWrappedValue (int value);
	}

	[BaseType (typeof (NSObject))]
	interface WrappedProtocolAdopter : WrappedProtocol {
	}
}
