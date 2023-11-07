using System;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;

namespace Test {
	[Protocol]
	public interface First {
		[Abstract]
		[Export ("doit:itwith:more:")]
		void DoIt (int a, int b, int c);
	}

	[Protocol]
	public interface Second {
		[Abstract]
		[Export ("doit:itwith:more:")]
		void DoIt (int a, int b, int c, int d);
	}

	[BaseType (typeof (NSObject))]
	public partial interface Derived : First, Second {
	}
}
