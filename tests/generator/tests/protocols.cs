using System;
using Foundation;
using ObjCRuntime;

namespace Protocols {
	[Protocol]
	interface ProtocolWithConstructors {
		/// <summary>init</summary>
		[Abstract]
		[Export ("init")]
		NativeHandle Constructor ();

		/// <summary>initWithValue:</summary>
		[Export ("initWithValue:")]
		NativeHandle Constructor (string p0);

		/// <summary>initWithError:</summary>
		[Export ("initWithError:")]
		NativeHandle Constructor (out NSError error);

		/// <summary>initWithCustomName:</summary>
		[Bind ("Create")]
		[Export ("initWithCustomName:")]
		NativeHandle Constructor (NSDate error);
	}

	[Protocol]
	interface ProtocolWithStaticMembers {
		/// <summary>member</summary>
		[Static]
		[Abstract]
		[Export ("member")]
		NativeHandle Method ();

		/// <summary>memberWithValue</summary>
		[Static]
		[Export ("memberWithValue:")]
		int Method (string p0);

		/// <summary>methodWithError</summary>
		[Static]
		[Export ("methodWithError:")]
		string Method (out NSError error);

		/// <summary>methodWitHDate</summary>
		[Static]
		[Export ("methodWithDate:")]
		NSString Method (NSDate error);

		/// <summary>property</summary>
		[Static]
		[Export ("property")]
		bool Property { get; set; }

		/// <summary>stringProperty</summary>
		[Static]
		[Export ("stringProperty")]
		string StringProperty { get; set; }

		/// <summary>dateProperty</summary>
		[Static]
		[Export ("dateProperty")]
		NSDate DateProperty { get; set; }
	}
}
