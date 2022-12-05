using System;
using Foundation;

namespace DiamondProtocol {
	namespace A {
		[Protocol]
		interface P1 {
			[Abstract]
			[Export ("p1")]
			int P1 { get; }
		}

		[Protocol]
		interface P2 {
			[Abstract]
			[Export ("p1")]
			int P1 { set; }
		}

		[Protocol]
		interface C : P1, P2 {
		}
	}

	namespace B {
		[Protocol]
		interface P1 {
			[Abstract]
			[Export ("p1")]
			int P1 { get; }
		}

		[Protocol]
		interface P2 {
			[Abstract]
			[Export ("p1")]
			int P1 { get; set; }
		}

		[Protocol]
		interface C : P1, P2 {
		}
	}

	namespace C {
		[Protocol]
		interface P1 {
			[Abstract]
			[Export ("p1")]
			int P1 { get; set; }
		}

		[Protocol]
		interface P2 {
			[Abstract]
			[Export ("p1")]
			long P1 { get; set; }
		}

		[Protocol]
		interface C : P1, P2 {
		}
	}

	namespace D {
		[Protocol]
		interface P1 {
			[Abstract]
			[Export ("pA")]
			int P1 { get; set; }
		}

		[Protocol]
		interface P2 {
			[Abstract]
			[Export ("pB")]
			int P1 { get; set; }
		}

		[Protocol]
		interface C : P1, P2 {
		}
	}
	namespace Z {

		[Protocol]
		interface P1 {
			[Abstract]
			[Export ("m1")]
			void M1 ();
		}

		[Protocol]
		interface P2 {
			[Abstract]
			[Export ("m1")]
			int M1 ();
		}


		//[BaseType (typeof (NSObject))]
		[Protocol]
		interface C : P1, P2 {
		}
	}

	namespace Y {

		[Protocol]
		interface P1 {
			[Abstract]
			[Export ("m1:")]
			void M1 (int why);
		}

		[Protocol]
		interface P2 {
			[Abstract]
			[Export ("m1:")]
			int M1 (bool because);
		}


		//[BaseType (typeof (NSObject))]
		[Protocol]
		interface C : P1, P2 {
		}
	}
}
