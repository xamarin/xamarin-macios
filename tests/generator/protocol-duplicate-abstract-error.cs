using System;
using System.Runtime.InteropServices;

#if !XAMCORE_2_0
#if MONOMAC
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
#else
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
#endif
#else
using Foundation;
using ObjCRuntime;
#endif

namespace Test 
{
	[Protocol]
	public interface First
	{
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

