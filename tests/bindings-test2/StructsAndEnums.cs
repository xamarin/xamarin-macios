using System;
using System.Runtime.InteropServices;

namespace Bindings.Test2 {
	public static class CFunctions {
		[DllImport ("__Internal")]
		public static extern int getIntOfChocolate ();
	}
}
