using NUnit.Framework;
using System;
using System.Threading;
using CoreGraphics;

#if !XAMCORE_2_0
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
using MonoMac.Foundation;
#else
using AppKit;
using ObjCRuntime;
using Foundation;
#endif

namespace Xamarin.Mac.Tests
{
	[TestFixture]
	public class NSSCreenTests
	{
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

		[Test]
		public void FrameNoMainThread ()
		{
			if (NSScreen.MainScreen == null)
				Assert.Inconclusive ("Could not find main screen.");

			var called = new AutoResetEvent (false);
			CGRect? frame = null;

			var backgroundThread = new Thread (() => {
				frame = NSScreen.MainScreen.Frame;
				called.Set ();
			});
			backgroundThread.Start ();
			Assert.IsTrue (called.WaitOne (1000), "called");
			Assert.IsTrue (frame.HasValue, "frame");
		}

		[Test]
		public void DepthNoMainThread ()
		{ 
			if (NSScreen.MainScreen == null)
				Assert.Inconclusive ("Could not find main screen.");

			var called = new AutoResetEvent (false);
			NSWindowDepth? depth = null;

			var backgroundThread = new Thread (() => {
				depth = NSScreen.MainScreen.Depth;
				called.Set ();
			});
			backgroundThread.Start ();
			Assert.IsTrue (called.WaitOne (1000), "called");
			Assert.IsTrue (depth.HasValue, "depth");
		}

		[Test]
		public void ColorSpaceNoMainThread ()
		{
			if (NSScreen.MainScreen == null)
				Assert.Inconclusive ("Could not find main screen.");

			var called = new AutoResetEvent (false);
			NSColorSpace colorSpace = null;

			var backgroundThread = new Thread (() => {
				colorSpace = NSScreen.MainScreen.ColorSpace;
				called.Set ();
			});
			backgroundThread.Start ();
			Assert.IsTrue (called.WaitOne (1000), "called");
			Assert.IsNotNull (colorSpace, "colorSpace");
		}

		[Test]
		public void VisibleFrmeNoMainThread ()
		{ 
			if (NSScreen.MainScreen == null)
				Assert.Inconclusive ("Could not find main screen.");

			var called = new AutoResetEvent (false);
			CGRect? frame = null;

			var backgroundThread = new Thread (() => {
				frame = NSScreen.MainScreen.VisibleFrame;
				called.Set ();
			});
			backgroundThread.Start ();
			Assert.IsTrue (called.WaitOne (1000), "called");
			Assert.IsTrue (frame.HasValue, "frame");
		}

		[Test]
		public void DescriptionNoMainThread ()
		{ 
			if (NSScreen.MainScreen == null)
				Assert.Inconclusive ("Could not find main screen.");

			var called = new AutoResetEvent (false);
			string description = null;

			var backgroundThread = new Thread (() => {
				description = NSScreen.MainScreen.Description;
				called.Set ();
			});
			backgroundThread.Start ();
			Assert.IsTrue (called.WaitOne (1000), "called");
			Assert.IsNotNull (description, "description");
		}

		[Test]
		public void BackingScaleFactorNoMainThread ()
		{ 
			if (NSScreen.MainScreen == null)
				Assert.Inconclusive ("Could not find main screen.");

			var called = new AutoResetEvent (false);
			nfloat? factor = null;

			var backgroundThread = new Thread (() => {
				factor = NSScreen.MainScreen.BackingScaleFactor;
				called.Set ();
			});
			backgroundThread.Start ();
			Assert.IsTrue (called.WaitOne (1000), "called");
			Assert.IsTrue (factor.HasValue, "factor");
		}

		[Test]
		public void NameNoMainThread ()
		{ 
			if (NSScreen.MainScreen == null)
				Assert.Inconclusive ("Could not find main screen.");

			var called = new AutoResetEvent (false);
			string name = "";

			var backgroundThread = new Thread (() => {
				name = NSScreen.MainScreen.LocalizedName;
				called.Set ();
			});
			backgroundThread.Start ();
			Assert.IsTrue (called.WaitOne (1000), "called");
			Assert.IsNotNull (name, "name");
		}

		[Test]
		public void MaximumExtendedDynamicRangeColorComponentValueNoMainThread ()
		{ 
			if (NSScreen.MainScreen == null)
				Assert.Inconclusive ("Could not find main screen.");

			var called = new AutoResetEvent (false);
			nfloat? factor = null;

			var backgroundThread = new Thread (() => {
				factor = NSScreen.MainScreen.MaximumExtendedDynamicRangeColorComponentValue;
				called.Set ();
			});
			backgroundThread.Start ();
			Assert.IsTrue (called.WaitOne (1000), "called");
			Assert.IsTrue (factor.HasValue, "factor");
		}

		[Test]
		public void MaximumPotentialExtendedDynamicRangeColorComponentValueNoMainThread ()
		{ 
			if (NSScreen.MainScreen == null)
				Assert.Inconclusive ("Could not find main screen.");

			var called = new AutoResetEvent (false);
			nfloat? factor = null;

			var backgroundThread = new Thread (() => {
				factor = NSScreen.MainScreen.MaximumPotentialExtendedDynamicRangeColorComponentValue;
				called.Set ();
			});
			backgroundThread.Start ();
			Assert.IsTrue (called.WaitOne (1000), "called");
			Assert.IsTrue (factor.HasValue, "factor");
		}

		[Test]
		public void MaximumReferenceExtendedDynamicRangeColorComponentValueNoMainThread ()
		{ 
			if (NSScreen.MainScreen == null)
				Assert.Inconclusive ("Could not find main screen.");

			var called = new AutoResetEvent (false);
			nfloat? factor = null;

			var backgroundThread = new Thread (() => {
				factor = NSScreen.MainScreen.MaximumReferenceExtendedDynamicRangeColorComponentValue;
				called.Set ();
			});
			backgroundThread.Start ();
			Assert.IsTrue (called.WaitOne (1000), "called");
			Assert.IsTrue (factor.HasValue, "factor");
		}
	}
}
