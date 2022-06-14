//
// Unit tests for Runtime.EnsureWeakPropertyStorage
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2022 Microsoft Corp. All rights reserved.
//

using System;
using System.Threading;

using Foundation;
using ObjCRuntime;

using NUnit.Framework;

namespace MonoTouchFixtures.ObjCRuntime {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class EnsureWeakPropertyStorageTest {
		[Test]
		public void EventsNotAllowed ()
		{
			try {
				Runtime.DisableWeakPropertyStorage = true;
				using var cache = new NSCache ();
				Assert.Throws<InvalidOperationException> (() => cache.WillEvictObject += delegate { }, "Can't add event handlers.");
			} finally {
				Runtime.DisableWeakPropertyStorage = false;
			}
		}

		[Test]
		public void WeakObjectsCollected ()
		{
			try {
				Runtime.DisableWeakPropertyStorage = true;

				var counter = 100;
				int disposedCount = 0;
				int finalizedCount = 0;
				var disposedCallback = new Action (() => disposedCount++);
				var finalizedCallback = new Action (() => finalizedCount++);
				var objects = new IDisposable [counter];
				for (var i = 0; i < objects.Length; i++)
					objects [i] = new NSCache ();

				var thread = new Thread (() => {
					for (var i = 0; i < objects.Length; i++) {
						var del = new WeakNSCacheDelegate () {
							FinalizedCallback = finalizedCallback,
							DisposedCallback = disposedCallback,
						};
						((NSCache) objects [i]).WeakDelegate = del;
					}
					GC.Collect ();
				}) {
					IsBackground = true,
					Name = "EnsureWeakPropertyStorageTest.WeakObjectsCollected",
				};
				thread.Start ();
				Assert.IsTrue (thread.Join (TimeSpan.FromMinutes (1)), "Background thread completed");

				// Iterate a few times
				for (var i = 0; i < 10; i++) {
					// Console.WriteLine ($"{i} Disposed: {disposedCount}");
					// Console.WriteLine ($"{i} Finalized: {finalizedCount}");
					if (disposedCount >= counter / 2 && finalizedCount >= counter / 2)
						break;
					Thread.Sleep (20);
					GC.Collect ();
				}
				Assert.That (disposedCount, Is.GreaterThan (counter / 2), "Disposed count");
				Assert.That (finalizedCount, Is.GreaterThan (counter / 2), "Finalized count");

				// Console.WriteLine ($"Disposed: {disposedCount}");
				// Console.WriteLine ($"Finalized: {finalizedCount}");

				for (var i = 0; i < objects.Length; i++)
					objects [i].Dispose ();

				GC.Collect ();
			} finally {
				Runtime.DisableWeakPropertyStorage = false;
			}
		}

		class WeakNSCacheDelegate : NSCacheDelegate {
			public Action FinalizedCallback;
			public Action DisposedCallback;

			~WeakNSCacheDelegate ()
			{
				FinalizedCallback ();
			}

			protected override void Dispose (bool value)
			{
				DisposedCallback ();
				base.Dispose (value);
			}
		}
	}
}
