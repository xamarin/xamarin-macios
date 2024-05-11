using System;
using System.Diagnostics.CodeAnalysis;

using Foundation;

namespace Experimental {
	[Experimental ("BGEN0001")]
	[BaseType (typeof (NSObject))]
	interface T1 : P1 {
		[Experimental ("BGEN0002")]
		[Export ("initWithString:")]
		IntPtr Constructor (string p);

		[Experimental ("BGEN0003")]
		[Export ("method")]
		int Method ();

		[Experimental ("BGEN0004")]
		[Export ("property")]
		D Property {
			[Experimental ("BGEN0005")]
			get;
			[Experimental ("BGEN0006")]
			set;
		}
	}

	[Experimental ("BGEN0007")]
	delegate void D ();


	[Experimental ("BGEN0008")]
	[Protocol]
	interface P1 {
		[Experimental ("BGEN0009")]
		[Export ("method")]
		int PMethod ();

		[Experimental ("BGEN0010")]
		[Export ("property")]
		int PProperty { get; set; }

		[Experimental ("BGEN0011")]
		[Abstract]
		[Export ("methodRequired")]
		int PAMethod ();

		[Experimental ("BGEN0012")]
		[Abstract]
		[Export ("propertyRequired")]
		int PAProperty { get; set; }
	}

	[Experimental ("BGEN0013")]
	[BaseType (typeof (NSObject))]
	interface TG1<T, U>
		where T : NSObject
		where U : NSObject {

		[Experimental ("BGEN0014")]
		[Export ("method")]
		int TGMethod ();

		[Experimental ("BGEN0015")]
		[Export ("property")]
		int TGProperty { get; set; }

		[Experimental ("BGEN0016")]
		[Export ("method2:")]
		void TGMethod2 (TG1<T, U> value);
	}

	[Experimental ("BGEN0017")]
	public enum E1 {
		[Experimental ("BGEN0018")]
		Value1,
	}

	[Experimental ("BGEN0019")]
	[BaseType (typeof (NSObject))]
	interface Notification1 {
		[Experimental ("BGEN0020")]
		[Notification]
		[Field ("NSANotification", LibraryName = "__Internal")]
		NSString ANotification { get; }
	}

	[Experimental ("BGEN0021")]
	enum E2 {
		[Experimental ("BGEN0022")]
		[Field ("E2A", LibraryName = "__Internal")]
		A,
	}

	[Experimental ("BGEN0023")]
	[ErrorDomain ("E3Domain", LibraryName = "__Internal")]
	enum E3 {
		[Experimental ("BGEN0024")]
		ErrorA,
	}

	[Experimental ("BGEN0025")]
	[Flags]
	enum E4 {
		[Experimental ("BGEN0026")]
		Bit1 = 1,
		[Experimental ("BGEN0027")]
		Bit3 = 4,
	}

	[Experimental ("BGEN0028")]
	[Protocol, Model]
	interface PM1 {
		[Experimental ("BGEN0029")]
		[Export ("method")]
		int PMethod ();

		[Experimental ("BGEN0030")]
		[Export ("property")]
		int PProperty { get; set; }

		[Experimental ("BGEN0031")]
		[Abstract]
		[Export ("methodRequired")]
		int PAMethod ();

		[Experimental ("BGEN0032")]
		[Abstract]
		[Export ("propertyRequired")]
		int PAProperty { get; set; }
	}

}
