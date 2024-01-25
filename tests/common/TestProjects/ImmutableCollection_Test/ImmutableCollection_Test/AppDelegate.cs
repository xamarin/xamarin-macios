using AppKit;

using Foundation;

using System;

namespace ImmutableCollection_Test {
	[Register ("AppDelegate")]
	public class AppDelegate : NSApplicationDelegate {
		public AppDelegate ()
		{
		}

		public override void DidFinishLaunching (NSNotification notification)
		{
			var v = System.Collections.Immutable.ImmutableList.Create (new int [] { 1, 2, 3 });
			Environment.Exit (0);
			// Insert code here to initialize your application
		}

		public override void WillTerminate (NSNotification notification)
		{
			// Insert code here to tear down your application
		}
	}
}
