using System;

using AppKit;
using Foundation;

namespace MySceneKitGame {
	[Register ("AppDelegate")]
	public class AppDelegate : NSApplicationDelegate {
		public AppDelegate ()
		{
		}

		public override void DidFinishLaunching (NSNotification notification)
		{
			Console.WriteLine (typeof (MyBindingLibrary.MyBindingClass));
			// Insert code here to initialize your application
		}

		public override void WillTerminate (NSNotification notification)
		{
			// Insert code here to tear down your application
		}
	}
}
