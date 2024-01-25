using System;
using System.Runtime.InteropServices;

using Foundation;

using ObjCRuntime;

namespace Test {
	[Protocol]
	public interface First {
		[Abstract]
		[Export ("doit:withmore")]
		void DoIt (int a, out int b);
	}

	[Protocol]
	public interface Second {
		[Abstract]
		[Export ("doit:withmore")]
		void DoIt (int a, int b);
	}

	[BaseType (typeof (NSObject))]
	public partial interface Derived : First, Second {
	}
}
