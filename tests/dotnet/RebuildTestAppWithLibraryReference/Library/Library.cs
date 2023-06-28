using System;
using System.Runtime.InteropServices;

namespace Library {
	public class Class {
		public static string SomeFunction ()
		{
#if FIRSTBUILD
			return "FIRSTBUILD";
#elif SECONDBUILD
			return "SECONDBUILD";
#endif
		}
	}
}
