using System;
using System.Collections.Generic;
using System.Reflection;
using NUnitLite;
using AppKit;
using Foundation;

namespace Mono.Native.Tests {
	[Register ("AppDelegate")]
	public class MacAppDelegate : NSApplicationDelegate {
		public override void DidFinishLaunching (NSNotification notification)
		{
		}

		public override void WillTerminate (NSNotification notification)
		{
			// Insert code here to tear down your application
		}
	}
}
