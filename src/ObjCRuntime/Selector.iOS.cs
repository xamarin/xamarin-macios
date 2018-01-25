#if !MONOMAC

using System;
using System.Runtime.InteropServices;
using Foundation;

namespace ObjCRuntime {
	public partial class Selector {
#if !XAMCORE_2_0
		public static IntPtr Init = Selector.GetHandle ("init");
		public static IntPtr InitWithCoder = Selector.GetHandle ("initWithCoder:");
#endif
	}
}

#endif // !MONOMAC
