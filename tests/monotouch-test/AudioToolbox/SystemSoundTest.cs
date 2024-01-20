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
using System.Threading.Tasks;

using Foundation;
using AudioToolbox;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.AudioToolbox {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class SystemSoundTest {
		[Test]
		public void FromFile ()
		{
			TestRuntime.AssertNotSimulator ();

			var path = NSBundle.MainBundle.PathForResource ("1", "caf", "AudioToolbox");

			using (var ss = SystemSound.FromFile (NSUrl.FromFilename (path))) {
				var completed = new TaskCompletionSource<bool> ();
				const int timeout = 10;

				Assert.AreEqual (AudioServicesError.None, ss.AddSystemSoundCompletion (delegate
				{
					completed.SetResult (true);
				}));

				ss.PlaySystemSound ();
				Assert.IsTrue (TestRuntime.RunAsync (TimeSpan.FromSeconds (timeout), completed.Task), "PlaySystemSound");
			}
		}

		[Test]
		public void Properties ()
		{
			var path = NSBundle.MainBundle.PathForResource ("1", "caf", "AudioToolbox");

			using (var ss = SystemSound.FromFile (NSUrl.FromFilename (path))) {
				Assert.That (ss.IsUISound, Is.True, "#1");
				Assert.That (ss.CompletePlaybackIfAppDies, Is.False, "#2");

				ss.CompletePlaybackIfAppDies = true;
				ss.IsUISound = false;
				Assert.That (ss.IsUISound, Is.False, "#1 B");
				Assert.That (ss.CompletePlaybackIfAppDies, Is.True, "#2 B");

				ss.CompletePlaybackIfAppDies = false;
				ss.IsUISound = true;
				Assert.That (ss.IsUISound, Is.True, "#1 C");
				Assert.That (ss.CompletePlaybackIfAppDies, Is.False, "#2 C");
			}
		}

		[Test]
		public void TestCallbackPlaySystem ()
		{
			TestRuntime.AssertNotSimulator ();
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 9, 0, throwIfOtherPlatform: false);

			string path = Path.Combine (NSBundle.MainBundle.ResourcePath, "drum01.mp3");

			using (var ss = SystemSound.FromFile (NSUrl.FromFilename (path))) {

				var completed = new TaskCompletionSource<bool> ();
				const int timeout = 10;

				ss.PlaySystemSound (() => { completed.SetResult (true); });
				Assert.IsTrue (TestRuntime.RunAsync (TimeSpan.FromSeconds (timeout), completed.Task), "TestCallbackPlaySystem");
			}
		}

		[Test]
		public void TestCallbackPlayAlert ()
		{
			TestRuntime.AssertNotSimulator ();
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 9, 0, throwIfOtherPlatform: false);

			string path = Path.Combine (NSBundle.MainBundle.ResourcePath, "drum01.mp3");

			using (var ss = SystemSound.FromFile (NSUrl.FromFilename (path))) {

				var completed = new TaskCompletionSource<bool> ();
				const int timeout = 10;

				ss.PlayAlertSound (() => { completed.SetResult (true); });
				Assert.IsTrue (TestRuntime.RunAsync (TimeSpan.FromSeconds (timeout), completed.Task), "TestCallbackPlayAlert");
			}
		}

		[Test]
		public void DisposeTest ()
		{
			var path = NSBundle.MainBundle.PathForResource ("1", "caf", "AudioToolbox");

			var ss = SystemSound.FromFile (NSUrl.FromFilename (path));
#if NET
			Assert.That (ss.SoundId, Is.Not.EqualTo ((uint) 0), "DisposeTest");
#else
			Assert.That (ss.Handle, Is.Not.EqualTo (IntPtr.Zero), "DisposeTest");
#endif

			ss.Dispose ();
			// Handle prop checks NotDisposed and throws if it is
#if NET
			Assert.Throws<ObjectDisposedException> (() => ss.SoundId.ToString (), "DisposeTest");
#else
			Assert.Throws<ObjectDisposedException> (() => ss.Handle.ToString (), "DisposeTest");
#endif
		}
	}
}

#endif // !__WATCHOS__
