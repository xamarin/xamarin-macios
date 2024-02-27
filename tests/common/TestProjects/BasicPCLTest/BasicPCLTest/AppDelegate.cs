using System;

using Foundation;
using AppKit;
using MyLibrary;

namespace BasicPCLTest {
	public partial class AppDelegate : NSApplicationDelegate {
		public AppDelegate ()
		{
		}

		public override void DidFinishLaunching (NSNotification notification)
		{
			MyClass.DoIt ();
		}
	}
}
