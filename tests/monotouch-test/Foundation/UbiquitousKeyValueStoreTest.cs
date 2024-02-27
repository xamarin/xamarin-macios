//
// Unit tests for NSUbiquitousKeyValueStore
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

using Foundation;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSUbiquitousKeyValueStoreTest {
#if !__WATCHOS__
		// Looks like NSUbiquitousKeyValueStore doesn't work on watchOS:
		// http://stackoverflow.com/questions/37412775/nsubiquitouskeyvaluestore-is-unavailable-watchos-2
		// https://forums.developer.apple.com/thread/47564
		[Test]
		public void Indexer ()
		{
			using (var store = new NSUbiquitousKeyValueStore ()) {
				using (var key = new NSString ("key")) {
					using (var value = new NSString ("value")) {
						store [key] = value;
#if __TVOS__
						// broken on appletv devices running tvOS 14, test will fail when fixed
						if ((Runtime.Arch == Arch.DEVICE) && TestRuntime.CheckXcodeVersion (12, 0))
							Assert.Null (store [key], "key 1");
						else
#elif __MACCATALYST__ || __MACOS__
						if (TestRuntime.CheckXcodeVersion (13, 0))
							Assert.Null (store [key], "key 1");
						else
#endif
						Assert.AreEqual (value, store [key], "key 1");

						store [(string) key] = value;
#if __TVOS__
						// broken on appletv devices running tvOS 14, test will fail when fixed
						if ((Runtime.Arch == Arch.DEVICE) && TestRuntime.CheckXcodeVersion (12, 0))
							Assert.Null (store [(string) key], "key 2");
						else
#elif __MACCATALYST__ || __MACOS__
						if (TestRuntime.CheckXcodeVersion (13, 0))
							Assert.Null (store [(string) key], "key 2");
						else
#endif
						Assert.AreEqual (value, store [(string) key], "key 2");
					}

				}
			}
		}
#endif
	}
}
