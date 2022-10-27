using AppKit;

namespace ImmutableCollection_Test {
	static class MainClass {
		static void Main (string [] args)
		{
			NSApplication.Init ();
			NSApplication.SharedApplication.Delegate = new AppDelegate ();
			NSApplication.Main (args);
		}
	}
}
