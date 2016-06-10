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
		string Identifier { get; set; }

		[Abstract]
		[Export ("doit")]
		void DoIt ();

		[Abstract]
		[Export ("doit:with:more")]
		void DoIt (int a, int b);
	}

	[Protocol]
	public interface Second {
		[Abstract]
		[NullAllowed, Export ("identifier")]
		string Identifier { get; }

		[Abstract]
		[Export ("doit")]
		void DoIt ();

		[Abstract]
		[Export ("doit:with:more")]
		void DoIt (int a, int b);
	}

	[BaseType (typeof (NSObject))]
	public partial interface Derived : First, Second {
	}
}

