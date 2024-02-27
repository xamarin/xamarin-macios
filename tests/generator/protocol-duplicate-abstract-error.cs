using System;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;

namespace Test {
	[Protocol]
	public interface First {
		[Abstract]
		[NullAllowed, Export ("identifier")]
		string Identifier { set; }
	}

	[Protocol]
	public interface Second {
		[Abstract]
		[NullAllowed, Export ("identifier")]
		string Identifier { get; }
	}

	[BaseType (typeof (NSObject))]
	public partial interface Derived : First, Second {
	}
}
