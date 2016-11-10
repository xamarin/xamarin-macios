using NUnit.Framework;
using System;

#if !XAMCORE_2_0
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
using MonoMac.CoreImage;
using MonoMac.CoreGraphics;
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
#else
using AppKit;
using ObjCRuntime;
using CoreImage;
using CoreGraphics;
#endif

namespace Xamarin.Mac.Tests
{
	[TestFixture]
	public class CGDisplayTests
	{
		int mainDisplayId;

		[SetUp]
		public void Setup ()
		{
			mainDisplayId = CGDisplay.MainDisplayID;
		}

		[Test]
		public void CGDisplayShouldGetDimensions ()
		{
			var bounds = CGDisplay.GetBounds (mainDisplayId);
			Assert.IsFalse (bounds.IsNull ());
			Assert.IsFalse (bounds.IsEmpty);

			var width = CGDisplay.GetWidth (mainDisplayId);
			Assert.Greater (width, 0);

			var height = CGDisplay.GetHeight (mainDisplayId);
			Assert.Greater (height, 0);
		}

		[Test]
		public void CGDisplayCursorMethods ()
		{
			int error;
			error = CGDisplay.HideCursor (mainDisplayId);
			Assert.AreEqual (0, error);

			error = CGDisplay.ShowCursor (mainDisplayId);
			Assert.AreEqual (0, error);

			error = CGDisplay.MoveCursor (mainDisplayId, new CGPoint (0, 0));
			Assert.AreEqual (0, error);
		}

		[Test]
		public void CGDisplayShouldSetAndRestoreDisplayTransfer ()
		{
			Exception exception = null;

			try {
				var error = CGDisplay.SetDisplayTransfer (mainDisplayId, 0, .5f, .5f, 0, .5f, .5f, 0, .5f, .5f);

				Assert.AreEqual (error, 0);

				CGDisplay.RestoreColorSyncSettings ();
			} catch (Exception e) {
				exception = e;
			}

			Assert.IsNull (exception);
		}

		[Test]
		public void CGDisplayShouldCaptureAndReleaseDisplays ()
		{
			int error;

			//error = CGDisplay.Capture (mainDisplayId);
			//Assert.AreEqual (0, error);

			//Assert.That (CGDisplay.IsCaptured (mainDisplayId));

			//error = CGDisplay.Release (mainDisplayId);
			//Assert.AreEqual (0, error);

			//error = CGDisplay.CaptureAllDisplays ();
			//Assert.AreEqual (0, error);

			//Assert.That (CGDisplay.IsCaptured (mainDisplayId));

			//error = CGDisplay.ReleaseAllDisplays ();
			//Assert.AreEqual (0, error);
		}
	}
}
