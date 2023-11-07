#if __MACOS__
using NUnit.Framework;
using System;
using System.Threading;

using AppKit;
using ObjCRuntime;
using Foundation;

namespace Xamarin.Mac.Tests {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSSCreenTests {
		[Test]
		public void ScreensNotMainThread ()
		{
			var called = new AutoResetEvent (false);
			var screensCount = 0;
			var backgroundThread = new Thread (() => {
				screensCount = NSScreen.Screens.Length;
				called.Set ();
			});
			backgroundThread.Start ();
			Assert.IsTrue (called.WaitOne (1000), "called");
			Assert.IsTrue (screensCount > 0, "screens count");
		}

		[Test]
		public void MainScreenNotMainThread ()
		{
			var called = new AutoResetEvent (false);
			NSScreen main = null;
			var backgroundThread = new Thread (() => {
				main = NSScreen.MainScreen;
				called.Set ();
			});
			backgroundThread.Start ();
			Assert.IsTrue (called.WaitOne (1000), "called");
			Assert.IsNotNull (main, "main screen");
		}

		[Test]
		public void DeepScreenNotMainThread ()
		{
			var called = new AutoResetEvent (false);
			NSScreen deepScreen = null;
			var screenCount = 0;

			var backgroundThread = new Thread (() => {
				screenCount = NSScreen.Screens.Length;
				deepScreen = NSScreen.DeepestScreen;
				called.Set ();
			});
			backgroundThread.Start ();
			Assert.IsTrue (called.WaitOne (1000), "called");
			if (screenCount > 1) {
				Assert.IsNotNull (deepScreen, "deep screen");
			} else {
				Assert.Inconclusive ("Only one screen detected.");
			}
		}
	}
}
#endif // __MACOS__
