using System;

using AppKit;

namespace Protobuf_Test {
	static class MainClass {
		static void Main (string [] args)
		{
			NSApplication.Init ();
			NSApplication.SharedApplication.Delegate = new AppDelegate ();
			NSApplication.Main (args);
		}
	}
}
