using System;
using Foundation;

namespace DiamondProtocol {
	[Protocol]
	interface P1 {
		[Abstract]
		[Export ("p1")]
		int P1 { get; }

		[Abstract]
		[Export ("p2")]
		int P2 { set; }

		[Abstract]
		[Export ("p3")]
		int P3 { get; set; }

		[Abstract]
		[Export ("m1")]
		void M1 ();

		[Abstract]
		[Export ("m2:")]
		void M2 (int foo);
	}

	[Protocol]
	interface P2 {
		[Abstract]
		[Export ("p1")]
		int P1 { get; }

		[Abstract]
		[Export ("p2")]
		int P2 { set; }

		[Abstract]
		[Export ("p3")]
		int P3 { get; set; }

		[Abstract]
		[Export ("m1")]
		void M1 ();

		[Abstract]
		[Export ("m2:")]
		void M2 (int foo);

		[Abstract]
		[Export ("m3:")]
		void M3 (string foo);
	}


	//[BaseType (typeof (NSObject))]
	[Protocol]
	interface C : P1, P2 {
	}
}
