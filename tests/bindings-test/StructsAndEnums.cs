using System;
using System.Runtime.InteropServices;

#if !__UNIFIED__
using nint=System.Int32;
#endif

namespace Bindings.Test
{
	public static class CFunctions {
		[DllImport ("__Internal")]
		public static extern int theUltimateAnswer ();
	}
}

