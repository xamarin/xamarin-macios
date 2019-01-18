//
// Unit tests for DispatchBlock
//
// Authors:
//	Rolf Bjarne Kvinge (rolf@xamarin.com)
//
// Copyright 2019 Microsoft Corp.
//

using System;
using System.Net;
using System.Threading;

#if XAMCORE_2_0
using Foundation;
using CoreFoundation;
using ObjCRuntime;
#else
using MonoTouch.CoreFoundation;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.CoreFoundation
{

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class DispatchBlockTest
	{
		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 10, throwIfOtherPlatform: false);
		}

		[Test]
		public void Invoke ()
		{
			var called = false;
			var callback = new Action (() => called = true);
			using (var db = new DispatchBlock (callback)) {
				db.Invoke ();
				Assert.IsTrue (called, "Called");
			}
		}

		[Test]
		public void ExplicitActionConversionInvoke ()
		{
			Assert.IsNull ((Action) ((DispatchBlock) null), "Null conversion");

			var called = false;
			var callback = new Action (() => called = true);
			using (var db = new DispatchBlock (callback)) {
				((Action) db) ();
				Assert.IsTrue (called, "Called");
			}
		}

		[Test]
		public void NotifyAction ()
		{
			var called = false;
			var notified = false;
			var callback = new Action (() => called = true);
			var notification = new Action (() => notified = true);
			using (var db = new DispatchBlock (callback)) {
				Assert.Throws<ArgumentNullException> (() => db.Notify (null, notification), "Null 1");
				Assert.Throws<ArgumentNullException> (() => db.Notify (DispatchQueue.MainQueue, (Action) null), "Null 2");
				db.Notify (DispatchQueue.MainQueue, notification);
				DispatchQueue.MainQueue.DispatchAsync (db);
				TestRuntime.RunAsync (DateTime.Now.AddSeconds (5), () => { }, () => notified);
				Assert.IsTrue (called, "Called");
			}
		}

		[Test]
		public void NotifyDispatchBlock ()
		{
			var called = false;
			var notified = false;
			var callback = new Action (() => called = true);
			var notification = new Action (() => notified = true);
			using (var db = new DispatchBlock (callback)) {
				using (var notification_block = new DispatchBlock (notification)) {
					Assert.Throws<ArgumentNullException> (() => db.Notify (null, notification), "Null 1");
					Assert.Throws<ArgumentNullException> (() => db.Notify (DispatchQueue.MainQueue, (DispatchBlock) null), "Null 2");
					db.Notify (DispatchQueue.MainQueue, notification_block);
					DispatchQueue.MainQueue.DispatchAsync (db);
					TestRuntime.RunAsync (DateTime.Now.AddSeconds (5), () => { }, () => notified);
					Assert.IsTrue (called, "Called");
				}
			}
		}

		[Test]
		public void Wait_DispatchTime ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);

			var called = false;
			var callback = new Action (() => called = true);
			using (var db = new DispatchBlock (callback)) {
				using (var queue = new DispatchQueue ("Background")) {
					queue.Activate ();
					var rv = db.Wait (new DispatchTime (DispatchTime.Now, TimeSpan.FromSeconds (0.1)));
					Assert.AreNotEqual (0, rv, "Timed Out");

					queue.DispatchAsync (db);
					rv = db.Wait (new DispatchTime (DispatchTime.Now, TimeSpan.FromSeconds (5)));
					Assert.AreEqual (0, rv, "Timed Out 2");
					Assert.IsTrue (called, "Called");
				}
			}
		}

		[Test]
		public void Wait_TimeSpan ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);

			var called = false;
			var callback = new Action (() => called = true);
			using (var db = new DispatchBlock (callback)) {
				using (var queue = new DispatchQueue ("Background")) {
					queue.Activate ();
					var rv = db.Wait (TimeSpan.FromSeconds (0.1));
					Assert.AreNotEqual (0, rv, "Timed Out");

					queue.DispatchAsync (db);
					rv = db.Wait (TimeSpan.FromSeconds (5));
					Assert.AreEqual (0, rv, "Timed Out 2");
					Assert.IsTrue (called, "Called");
				}
			}
		}

		[Test]
		public void Cancellation ()
		{
			var called = false;
			var callback = new Action (() => called = true);
			using (var db = new DispatchBlock (callback)) {
				Assert.AreEqual (0, db.TestCancel (), "TestCancel 1");
				Assert.IsFalse (db.Cancelled, "Cancelled 1");
				db.Cancel ();
				Assert.AreNotEqual (0, db.TestCancel (), "TestCancel 2");
				Assert.IsTrue (db.Cancelled, "Cancelled 2");
			}
		}

		[Test]
		public void Constructors ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);

			var called = false;
			var callback = new Action (() => called = true);
			DispatchBlockFlags flags;

			Assert.Throws<ArgumentNullException> (() => new DispatchBlock (null), "ANE 1");
			Assert.Throws<ArgumentNullException> (() => new DispatchBlock (null, DispatchBlockFlags.AssignCurrent), "ANE 2");
			Assert.Throws<ArgumentNullException> (() => new DispatchBlock ((Action) null, DispatchBlockFlags.AssignCurrent, DispatchQualityOfService.Background, 2), "ANE 3");
			Assert.Throws<ArgumentNullException> (() => new DispatchBlock ((DispatchBlock) null, DispatchBlockFlags.AssignCurrent, DispatchQualityOfService.Background, 2), "ANE 4");
			// Invalid input results in NULL and an exception
			Assert.Throws<Exception> (() => new DispatchBlock (callback, (DispatchBlockFlags) 12345678), "E 1");
			Assert.Throws<Exception> (() => new DispatchBlock (callback, (DispatchBlockFlags) 12345678, DispatchQualityOfService.UserInteractive, 0), "E 2");
			Assert.Throws<Exception> (() => new DispatchBlock (callback, DispatchBlockFlags.None, (DispatchQualityOfService) 12345678, 0), "E 3");
			Assert.Throws<Exception> (() => new DispatchBlock (callback, DispatchBlockFlags.None, DispatchQualityOfService.Default, 12345678), "E 4");

			called = false;
			using (var db = new DispatchBlock (callback)) {
				using (var queue = new DispatchQueue ("Background")) {
					queue.Activate ();
					queue.DispatchAsync (db);
					var rv = db.Wait (new DispatchTime (DispatchTime.Now, TimeSpan.FromSeconds (5)));
					Assert.AreEqual (0, rv, "Timed Out A");
					Assert.IsTrue (called, "Called A");
				}
			}

			called = false;
			flags = DispatchBlockFlags.None;
			using (var db = new DispatchBlock (callback, flags)) {
				using (var queue = new DispatchQueue ("Background")) {
					queue.Activate ();
					queue.DispatchAsync (db);
					var rv = db.Wait (new DispatchTime (DispatchTime.Now, TimeSpan.FromSeconds (5)));
					Assert.AreEqual (0, rv, "Timed Out " + flags);
					Assert.IsTrue (called, "Called " + flags);
				}
			}

			called = false;
			flags = DispatchBlockFlags.AssignCurrent;
			using (var db = new DispatchBlock (callback, flags)) {
				using (var queue = new DispatchQueue ("Background")) {
					queue.Activate ();
					queue.DispatchAsync (db);
					var rv = db.Wait (new DispatchTime (DispatchTime.Now, TimeSpan.FromSeconds (5)));
					Assert.AreEqual (0, rv, "Timed Out " + flags);
					Assert.IsTrue (called, "Called " + flags);
				}
			}


			called = false;
			flags = DispatchBlockFlags.Detached;
			using (var db = new DispatchBlock (callback, flags)) {
				using (var queue = new DispatchQueue ("Background")) {
					queue.Activate ();
					queue.DispatchAsync (db);
					var rv = db.Wait (new DispatchTime (DispatchTime.Now, TimeSpan.FromSeconds (5)));
					Assert.AreEqual (0, rv, "Timed Out " + flags);
					Assert.IsTrue (called, "Called " + flags);
				}
			}

			called = false;
			flags = DispatchBlockFlags.Detached;
			using (var db = new DispatchBlock (callback, flags, DispatchQualityOfService.Background, -8)) {
				using (var queue = new DispatchQueue ("Background")) {
					queue.Activate ();
					queue.DispatchAsync (db);
					var rv = db.Wait (new DispatchTime (DispatchTime.Now, TimeSpan.FromSeconds (5)));
					Assert.AreEqual (0, rv, "Timed Out Background 8" + flags);
					Assert.IsTrue (called, "Called Background 8" + flags);
				}
			}
		}

		[Test]
		public void Create ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);

			var called = false;
			var callback = new Action (() => called = true);
			DispatchBlockFlags flags;

			Assert.Throws<ArgumentNullException> (() => DispatchBlock.Create (null), "ANE 1");
			Assert.Throws<ArgumentNullException> (() => DispatchBlock.Create (null, DispatchBlockFlags.AssignCurrent), "ANE 2");
			Assert.Throws<ArgumentNullException> (() => DispatchBlock.Create ((Action) null, DispatchBlockFlags.AssignCurrent, DispatchQualityOfService.Background, 2), "ANE 3");
			Assert.Throws<ArgumentNullException> (() => DispatchBlock.Create ((DispatchBlock) null, DispatchBlockFlags.AssignCurrent, DispatchQualityOfService.Background, 2), "ANE 4");
			// Invalid input results in NULL and an exception
			Assert.Throws<Exception> (() => DispatchBlock.Create (callback, (DispatchBlockFlags) 12345678), "E 1");
			Assert.Throws<Exception> (() => DispatchBlock.Create (callback, (DispatchBlockFlags) 12345678, DispatchQualityOfService.UserInteractive, 0), "E 2");
			Assert.Throws<Exception> (() => DispatchBlock.Create (callback, DispatchBlockFlags.None, (DispatchQualityOfService) 12345678, 0), "E 3");
			Assert.Throws<Exception> (() => DispatchBlock.Create (callback, DispatchBlockFlags.None, DispatchQualityOfService.Default, 12345678), "E 4");

			called = false;
			using (var db = DispatchBlock.Create (callback)) {
				using (var queue = new DispatchQueue ("Background")) {
					queue.Activate ();
					queue.DispatchAsync (db);
					var rv = db.Wait (new DispatchTime (DispatchTime.Now, TimeSpan.FromSeconds (5)));
					Assert.AreEqual (0, rv, "Timed Out A");
					Assert.IsTrue (called, "Called A");
				}
			}

			called = false;
			flags = DispatchBlockFlags.None;
			using (var db = DispatchBlock.Create (callback, flags)) {
				using (var queue = new DispatchQueue ("Background")) {
					queue.Activate ();
					queue.DispatchAsync (db);
					var rv = db.Wait (new DispatchTime (DispatchTime.Now, TimeSpan.FromSeconds (5)));
					Assert.AreEqual (0, rv, "Timed Out " + flags);
					Assert.IsTrue (called, "Called " + flags);
				}
			}

			called = false;
			flags = DispatchBlockFlags.AssignCurrent;
			using (var db = DispatchBlock.Create (callback, flags)) {
				using (var queue = new DispatchQueue ("Background")) {
					queue.Activate ();
					queue.DispatchAsync (db);
					var rv = db.Wait (new DispatchTime (DispatchTime.Now, TimeSpan.FromSeconds (5)));
					Assert.AreEqual (0, rv, "Timed Out " + flags);
					Assert.IsTrue (called, "Called " + flags);
				}
			}


			called = false;
			flags = DispatchBlockFlags.Detached;
			using (var db = DispatchBlock.Create (callback, flags)) {
				using (var queue = new DispatchQueue ("Background")) {
					queue.Activate ();
					queue.DispatchAsync (db);
					var rv = db.Wait (new DispatchTime (DispatchTime.Now, TimeSpan.FromSeconds (5)));
					Assert.AreEqual (0, rv, "Timed Out " + flags);
					Assert.IsTrue (called, "Called " + flags);
				}
			}

			called = false;
			flags = DispatchBlockFlags.Detached;
			using (var db = DispatchBlock.Create (callback, flags, DispatchQualityOfService.Background, -8)) {
				using (var queue = new DispatchQueue ("Background")) {
					queue.Activate ();
					queue.DispatchAsync (db);
					var rv = db.Wait (new DispatchTime (DispatchTime.Now, TimeSpan.FromSeconds (5)));
					Assert.AreEqual (0, rv, "Timed Out Background 8" + flags);
					Assert.IsTrue (called, "Called Background 8" + flags);
				}
			}

			called = false;
			flags = DispatchBlockFlags.Detached;
			using (var db = DispatchBlock.Create (callback, flags, DispatchQualityOfService.Background, -8)) {
				using (var db2 = db.Create (DispatchBlockFlags.EnforceQosClass, DispatchQualityOfService.Unspecified, -7)) {
					using (var queue = new DispatchQueue ("Background")) {
						queue.Activate ();
						queue.DispatchAsync (db2);
						var rv = db2.Wait (new DispatchTime (DispatchTime.Now, TimeSpan.FromSeconds (5)));
						Assert.AreEqual (0, rv, "Timed Out Background DB" + flags);
						Assert.IsTrue (called, "Called Background DB" + flags);
					}
				}
			}
		}
	}
}
