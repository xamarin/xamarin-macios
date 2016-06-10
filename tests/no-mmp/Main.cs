using System;
using MonoMac.AppKit;

class NoMMP {
	static int Main ()
	{
		NSApplication.Init ();
		Console.WriteLine ("Success");
		return 0;
	}
}