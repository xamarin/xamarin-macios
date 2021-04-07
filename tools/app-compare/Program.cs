using System;
using Tools;

namespace AppCompare {
	class MainClass {
		public static void Main (string[] args)
		{
			var dc = new AppComparer (args [0], args [1]);
			dc.Compare ();
		}
	}
}
