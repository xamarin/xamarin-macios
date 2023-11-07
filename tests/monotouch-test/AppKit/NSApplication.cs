#if __MACOS__
using NUnit.Framework;
using System;

using AppKit;
using ObjCRuntime;
using Foundation;

namespace Xamarin.Mac.Tests {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSApplicationTests {
		[Test]
		public void NSApplication_SendActionNullTest ()
		{
			NSApplication.SharedApplication.SendAction (new Selector ("undo:"), null, new NSObject ());
		}
	}
}
#endif // __MACOS__
