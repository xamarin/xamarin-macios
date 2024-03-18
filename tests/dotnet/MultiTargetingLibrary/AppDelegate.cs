using System;
using System.Runtime.InteropServices;

namespace MySimpleApp {
	public class Program {
		public static int SomeFunction ()
		{
			return 42;
		}

		public static Type GetNSObjectType ()
		{
			return typeof (Foundation.NSObject);
		}
	}
}
