//
// Unit tests for SystemSound
//
// Authors:
//	Marek Safar (marek.safar@gmail.com)
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using System.IO;
#if XAMCORE_2_0
using Foundation;
using AudioToolbox;
using CoreFoundation;
using ObjCRuntime;
#else
using MonoTouch.Foundation;
using MonoTouch.MediaPlayer;
using MonoTouch.AudioToolbox;
using MonoTouch.CoreFoundation;
using MonoTouch.ObjCRuntime;
#endif
using NUnit.Framework;
using System.Threading;

namespace MonoTouchFixtures.AudioToolbox {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class SystemSoundTest
	{
#if !MONOMAC // Currently no AppDelegate in xammac_test
		[Test]
		public void FromFile ()
		{
#if MONOMAC
			var path = NSBundle.MainBundle.PathForResource ("1", "caf", "AudioToolbox");
#else
			var path = Path.GetFullPath (Path.Combine ("AudioToolbox", "1.caf"));

			if (Runtime.Arch == Arch.SIMULATOR)
				Assert.Ignore ("PlaySystemSound doesn't work in the simulator");
#endif

			using (var ss = SystemSound.FromFile (NSUrl.FromFilename (path))) {
				var completed = false;
				const int timeout = 10;

				Assert.AreEqual (AudioServicesError.None, ss.AddSystemSoundCompletion (delegate {
					completed = true;
					}));

				ss.PlaySystemSound ();
				Assert.IsTrue (MonoTouchFixtures.AppDelegate.RunAsync (DateTime.Now.AddSeconds (timeout), async () => { }, () => completed), "PlaySystemSound");
			}
		}
#endif

		[Test]
		public void Properties ()
		{
#if MONOMAC
			var path = NSBundle.MainBundle.PathForResource ("1", "caf", "AudioToolbox");
#else
			var path = Path.GetFullPath (Path.Combine ("AudioToolbox", "1.caf"));
#endif

			using (var ss = SystemSound.FromFile (NSUrl.FromFilename (path))) {
				Assert.That (ss.IsUISound, Is.True, "#1");
				Assert.That (ss.CompletePlaybackIfAppDies, Is.False, "#2");
			}
		}

#if !MONOMAC // Currently no AppDelegate in xammac_test
		[Test]
		public void TestCallbackPlaySystem ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.iOS, 9, 0, throwIfOtherPlatform: false);

			string path = Path.Combine (NSBundle.MainBundle.ResourcePath, "drum01.mp3");

			using (var ss = SystemSound.FromFile (NSUrl.FromFilename (path))) {

				var completed = false;
				const int timeout = 10;

				completed = false;
				Assert.IsTrue (MonoTouchFixtures.AppDelegate.RunAsync (DateTime.Now.AddSeconds (timeout), async () =>
					ss.PlaySystemSound (() => {	completed = true; }
				), () => completed), "TestCallbackPlaySystem");
			}
		}

		[Test]
		public void TestCallbackPlayAlert ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.iOS, 9, 0, throwIfOtherPlatform: false);

			string path = Path.Combine (NSBundle.MainBundle.ResourcePath, "drum01.mp3");

			using (var ss = SystemSound.FromFile (NSUrl.FromFilename (path))) {

				var completed = false;
				const int timeout = 10;

				completed = false;
				Assert.IsTrue (MonoTouchFixtures.AppDelegate.RunAsync (DateTime.Now.AddSeconds (timeout), async () =>
					ss.PlayAlertSound (() => { completed = true; }
				), () => completed), "TestCallbackPlayAlert");
			}
		}
#endif

		[Test]
		public void DisposeTest ()
		{
#if MONOMAC
			var path = NSBundle.MainBundle.PathForResource ("1", "caf", "AudioToolbox");
#else
			var path = Path.GetFullPath (Path.Combine ("AudioToolbox", "1.caf"));
#endif

			var ss = SystemSound.FromFile (NSUrl.FromFilename (path));
			Assert.That (ss.Handle, Is.Not.EqualTo (IntPtr.Zero), "DisposeTest");

			ss.Dispose ();
			// Handle prop checks NotDisposed and throws if it is
			Assert.Throws<ObjectDisposedException> (() => ss.Handle.ToString (), "DisposeTest");
		}
	}
}

#endif // !__WATCHOS__
