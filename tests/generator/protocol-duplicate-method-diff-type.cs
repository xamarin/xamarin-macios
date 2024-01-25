using System;
using System.Runtime.InteropServices;

using Foundation;

using ObjCRuntime;

namespace Test {
	[Protocol]
	public interface First {
		[Abstract]
		[Export ("doit:with:more:")]
		void DoIt (int a, int b, int c);
	}

	[Protocol]
	public interface Second {
		[Abstract]
		[Export ("doit:with:more:")]
		void DoIt (int a, int b, NSObject c);
	}

	[BaseType (typeof (NSObject))]
	public partial interface Derived : First, Second {
	}
}
