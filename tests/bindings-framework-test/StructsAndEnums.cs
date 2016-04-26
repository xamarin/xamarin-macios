using System;
using System.Runtime.InteropServices;

namespace Bindings.Test
{
	public static class CFunctions {
		[DllImport ("__Internal")]
		public static extern int theUltimateAnswer ();

		[DllImport ("__Internal")]
		public static extern int object_theUltimateAnswer ();

		[DllImport ("__Internal")]
		public static extern int ar_theUltimateAnswer ();
	}
}

