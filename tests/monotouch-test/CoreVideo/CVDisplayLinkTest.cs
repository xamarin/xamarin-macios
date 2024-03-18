#if __MACOS__
using System;
using CoreGraphics;
using Foundation;
using NUnit.Framework;
using ObjCRuntime;
using CoreVideo;
using Xamarin.Utils;

namespace MonoTouchFixtures.CoreVideo {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CVDisplayLinkTest {

		[Test]
		public void CreateFromDisplayIdValidIdTest ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 12, 0);
			Assert.DoesNotThrow (() => {
				using var displayLink = CVDisplayLink.CreateFromDisplayId ((uint) CGDisplay.MainDisplayID);
				Assert.NotNull (displayLink, "Not null");
				Assert.AreEqual (CGDisplay.MainDisplayID, displayLink.GetCurrentDisplay (), "DisplayId");
			}, "Throws");
		}

		[Test]
		public void CreateFromDisplayWrongIdTest ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 12, 0);
			Assert.DoesNotThrow (() => {
				using var displayLink = CVDisplayLink.CreateFromDisplayId (UInt32.MaxValue);
				Assert.Null (displayLink, "null");
			}, "Throws");
		}

		[Test]
		public void CreateFromDisplayIdsTest ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 12, 0);
			// we might not have more than one display, therefore we will use an array 
			// with a single one, there is nothing in the docs that say that we cannot do that
			Assert.DoesNotThrow (() => {
				using var displayLink = CVDisplayLink.CreateFromDisplayIds (new [] { (uint) CGDisplay.MainDisplayID });
				Assert.NotNull (displayLink, "Not null");
			}, "Throws");
		}

		[Test]
		public void CreateFromOpenGLMaskTest ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 12, 0);
			var openGLMask = CGDisplay.GetOpenGLDisplayMask (CGDisplay.MainDisplayID);
			Assert.DoesNotThrow (() => {
				using var displayLink = CVDisplayLink.CreateFromOpenGLMask ((uint) openGLMask);
				Assert.NotNull (displayLink, "Not null");
			}, "Throws");
		}

		[Test]
		public void DefaultConstructorTest ()
		{
			TestRuntime.AssertNotVSTS ();
			Assert.DoesNotThrow (() => {
				using var displayLink = new CVDisplayLink ();
			});
		}

		[Test]
		public void SetCurrentDisplayOpenGLTest ()
		{
			TestRuntime.AssertNotVSTS ();
			Assert.DoesNotThrow (() => {
				using var displayLink = new CVDisplayLink ();
				displayLink.SetCurrentDisplay (CGDisplay.MainDisplayID);
			});
		}

		[Test]
		public void GetCurrentDisplayTest ()
		{
			TestRuntime.AssertNotVSTS ();
			Assert.DoesNotThrow (() => {
				using var displayLink = new CVDisplayLink ();
				Assert.AreEqual (CGDisplay.MainDisplayID, displayLink.GetCurrentDisplay ());
			});
		}

		[Test]
		public void GetTypeIdTest ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 12, 0);
			Assert.DoesNotThrow (() => {
				CVDisplayLink.GetTypeId ();
			}, "Throws");
		}

		[Test]
		public void TryTranslateTimeValidTest ()
		{
			TestRuntime.AssertNotVSTS ();
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 12, 0);
			var outTime = new CVTimeStamp {
				Version = 0,
				Flags = (1L << 0) | (1L << 1), // kCVTimeStampVideoTimeValid | kCVTimeStampHostTimeValid
			};
			using var displayLink = new CVDisplayLink ();
			// it has to be running else you will get a crash
			if (displayLink.Start () == 0) {
				displayLink.GetCurrentTime (out var timeStamp);
				Assert.True (displayLink.TryTranslateTime (timeStamp, ref outTime));
				displayLink.Stop ();
			}
		}
	}
}
#endif
