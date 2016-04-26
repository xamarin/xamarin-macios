using System;
using System.Runtime.InteropServices;

#if !__UNIFIED__
using nint=System.Int32;
#endif

namespace Bindings.Test
{
	public struct Sd { public double d1; }
	public struct Sdd { public double d1; public double d2; }
	public struct Sddd { public double d1; public double d2; public double d3; }
	public struct Sdddd { public double d1; public double d2; public double d3; public double d4; }
	public struct Si { public int i1; }
	public struct Sii { public int i1; public int i2; }
	public struct Siii { public int i1; public int i2; public int i3; }
	public struct Siiii { public int i1; public int i2; public int i3; public int i4; }
	public struct Siiiii { public int i1; public int i2; public int i3; public int i4; public int i5; }
	public struct Sid { public int i1; public double d2; }
	public struct Sdi { public double d1; public int i2; }
	public struct Sidi { public int i1; public double d2; public int i3; }
	public struct Siid { public int i1; public int i2; public double d3; }
	public struct Sddi { public double d1; public double d2; public int i3; }
	public struct Sl { public nint l1; }
	public struct Sll { public nint l1; public nint l2; }
	public struct Slll { public nint l1; public nint l2; public nint l3; }
	public struct Scccc { public char c1; public char c2; public char c3; public char c4; }
	public struct Sffff { public float f1; public float f2; public float f3; public float f4; }
	public struct Sif { public int i1; public float f2; }
	public struct Sf { public float f1; }
	public struct Sff { public float f1; public float f2; }
	public struct Siff { public int i1; public float f2; public float f3; }
	public struct Siiff { public int i1; public int i2; public float f3; public float f4; }
	public struct Sfi { public float f1; public int i2; }

	public static class CFunctions {
		[DllImport ("__Internal")]
		public static extern int theUltimateAnswer ();
	}
}

