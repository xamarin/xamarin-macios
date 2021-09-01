#if __MACOS__
using System;
using CoreGraphics;
using Foundation;
using NUnit.Framework;
using Foundation;
using ObjCRuntime;
using CoreVideo;

namespace MonoTouchFixtures.CoreVideo {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CVDisplayLinkTest {
		int? mainDisplayId = null;

		[SetUp]
		public void SetUp ()
		{
			// we need to have access to the media display to be able to perform
			// the displaylink tests
			mainDisplayId = CGDisplay.MainDisplayID;
		}

		[TearDown]
		public void TearDown ()
		{
			mainDisplayId = null;
		}

		[Test]
		public void CreateFromDisplayIdValidIdTest ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 12, 0);
			Assert.DoesNotThrow (() => {
				using var displayLink = CVDisplayLink.CreateFromDisplayId ((uint) CGDisplay.MainDisplayID);
				Assert.NotNull (displayLink, "Not null");
				Assert.AreEqual (CGDisplay.MainDisplayID,  displayLink.GetCurrentDisplay (), "DisplayId");
			}, "Throws");
		}

		[Test]
		public void CreateFromDisplayWrongIdTest ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 12, 0);
			Assert.DoesNotThrow (() => {
				using var displayLink = CVDisplayLink.CreateFromDisplayId (UInt32.MaxValue);
				Assert.Null (displayLink, "null");
			}, "Throws");
		}

		[Test]
		public void CreateFromDisplayIdsTest ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 12, 0);
			// we might not have more than one display, therefore we will use an array 
			// with a single one, there is nothing in the docs that say that we cannot do that
			Assert.DoesNotThrow (() => {
				using var displayLink = CVDisplayLink.CreateFromDisplayIds (new []{ (uint) CGDisplay.MainDisplayID});
				Assert.Null (displayLink, "null");
			}, "Throws");
		}

		[Test]
		public void CreateFromOpenGLMaskTest ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 12, 0);
			var openGLMask = CGDisplay.GetOpenGLDisplayMask (CGDisplay.MainDisplayID);
			Assert.DoesNotThrow (() => {
				using var displayLink = CVDisplayLink.CreateFromOpenGLMask ((uint) openGLMask);
				Assert.NotNull (displayLink, "Not null");
			}, "Throws");
		}

		[Test]
		public void DefaultconstructorTest () => Assert.DoesNotThrow (() => {
			using var displayLink = new CVDisplayLink ();
		});

		[Test]
		public void SetCurrentDisplayOpenGLTest () => Assert.DoesNotThrow (() => {
			using var displayLink = new CVDisplayLink ();
			displayLink.SetCurrentDisplay (CGDisplay.MainDisplayID);
		});

		[Test]
		public void GetCurrentDisplayTest () => Assert.DoesNotThrow (() => {
			using var displayLink = new CVDisplayLink ();
			Assert.AreEqual (CGDisplay.MainDisplayID,  displayLink.GetCurrentDisplay ());
		});

		[Test]
		public void GetTypeIdTest ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 12, 0);
			Assert.DoesNotThrow (() => {
				CVDisplayLink.GetTypeId ();
			}, "Throws");
		}

		[Test]
		public void TryTranslateTimeValidTest ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 12, 0);
			using var displayLink = new CVDisplayLink ();
			displayLink.GetCurrentTime (out var timeStamp);
			Assert.True (displayLink.TryTranslateTime (timeStamp, out var _));
		}
	}
}
#endif
