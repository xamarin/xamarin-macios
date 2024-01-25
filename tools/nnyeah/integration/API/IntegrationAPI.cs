using System;

using Foundation;

namespace IntegrationAPI {
	public class NIntArgs {
		public NIntArgs (nint value) { Value = value; }
		public nint Value { get; }
	}

	public class NUIntArgs {
		public NUIntArgs (nuint value) { Value = value; }
		public nuint Value { get; }
	}

	public class NFloatArgs {
		public NFloatArgs (nfloat value) { Value = value; }
		public nfloat Value { get; }
	}

	public delegate void EventWithNInt (object sender, NIntArgs e);
	public delegate void EventWithNUInt (object sender, NUIntArgs e);
	public delegate void EventWithNFloat (object sender, NFloatArgs e);


	public interface INum {
		NSNumber Zero ();
	}

	public class NIntAPI {
		class MyNum : INum {
			public MyNum () { }
			public NSNumber Zero ()
			{
				nint x = 0;
				return new NSNumber (x);
			}
		}

		public NIntAPI ()
		{
		}

		public nint EchoMethod (nint x) => x;
		public nint Prop { get; set; }
		public nint Field;
#pragma warning disable CS0067 // The event 'NIntAPI.Event' is never used
		public event EventWithNInt Event;
#pragma warning disable CS0067 // The event 'NIntAPI.Event' is never used
		public nint Sum (nint a, nint b) => a + b;
		public nint Prod (nint a, nint b) => a * b;
		public long ToLong (nint a) => (long) a;
		public nuint ToNUint (nint a) => (nuint) a;
		public bool Less (nint a, nint b) => a < b;
		public bool Greater (nint a, nint b) => a > b;
		public bool Eq (nint a, nint b) => a == b;
		public nint And (nint a, nint b) => a & b;
		public nint Or (nint a, nint b) => a | b;
		public nint Xor (nint a, nint b) => a ^ b;
		public nint ToNint (sbyte a) => a;
		public nint ToNint (byte a) => a;
		public nint ToNint (short a) => a;
		public nint ToNint (char a) => a;
		public nint ToNint (int a) => a;
		public nint PlusOne (nint a) => a++;
		public nint NumberZero () => new MyNum ().Zero ().NIntValue;
	}

	public class NUIntAPI {
		public NUIntAPI ()
		{
		}

		public nuint EchoMethod (nuint x) => x;
		public nuint Prop { get; set; }
		public nuint Field;
#pragma warning disable CS0067 // The event 'NUIntAPI.Event' is never used
		public event EventWithNUInt Event;
#pragma warning disable CS0067 // The event 'NUIntAPI.Event' is never used
		public nuint Sum (nuint a, nuint b) => a + b;
		public nuint Prod (nuint a, nuint b) => a * b;
		public long ToLong (nuint a) => (long) a;
		public nint ToNInt (nuint a) => (nint) a;
		public bool Less (nuint a, nuint b) => a < b;
		public bool Greater (nuint a, nuint b) => a > b;
		public bool Eq (nuint a, nuint b) => a == b;
		public nuint And (nuint a, nuint b) => a & b;
		public nuint Or (nuint a, nuint b) => a | b;
		public nuint Xor (nuint a, nuint b) => a ^ b;
		public nuint ToNuint (byte a) => a;
		public nuint ToNuint (ushort a) => a;
		public nuint ToNuint (char a) => a;
		public nuint ToNuint (uint a) => a;
		public nuint PlusOne (nuint a) => a++;
	}

	public class NFloatAPI {
		public NFloatAPI ()
		{
		}

		public nfloat EchoMethod (nfloat x) => x;
		public nfloat Prop { get; set; }
		public nfloat Field;
#pragma warning disable CS0067 // The event 'NFloatAPI.Event' is never used
		public event EventWithNFloat Event;
#pragma warning disable CS0067 // The event 'NFloatAPI.Event' is never used
		public nfloat Sum (nfloat a, nfloat b) => a + b;
		public nfloat Prod (nfloat a, nfloat b) => a * b;
		public double ToDouble (nfloat a) => (double) a;
		public bool Less (nfloat a, nfloat b) => a < b;
		public bool Greater (nfloat a, nfloat b) => a > b;
		public bool Eq (nfloat a, nfloat b) => a == b;
		public nfloat ToNFloat (sbyte a) => a;
		public nfloat ToNFloat (byte a) => a;
		public nfloat ToNFloat (char a) => a;
		public nfloat ToNFloat (short a) => a;
		public nfloat ToNFloat (ushort a) => a;
		public nfloat ToNFloat (int a) => a;
		public nfloat ToNFloat (uint a) => a;
		public nfloat ToNFloat (long a) => a;
		public nfloat ToNFloat (ulong a) => a;
		public nfloat ToNFloat (float a) => a;
	}

	public class NSObjectDerived : NSObject {
		public NSObjectDerived (IntPtr p) : base (p) { }
		public NSObjectDerived (IntPtr p, bool b) : base (p, b) { }

	}
	public class NSObjectDerivedSubclass : NSObjectDerived {
		public NSObjectDerivedSubclass (IntPtr p) : base (p) { }
		public NSObjectDerivedSubclass (IntPtr p, bool b) : base (p, b) { }
	}
}
