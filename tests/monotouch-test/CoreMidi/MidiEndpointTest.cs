//
// Unit tests for MidiEndpointTest
//
// Authors:
//	Alex Soto <alexsoto@microsoft.com>
//	
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !__WATCHOS__
using System;
using Foundation;
using CoreMidi;
using NUnit.Framework;

namespace MonoTouchFixtures.CoreMidi {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MidiEndpointTest {
		[Test]
		public void CorrectDisposeTest ()
		{
			// Test for bug 43582 - https://bugzilla.xamarin.com/show_bug.cgi?id=43582
			// This will throw if bug dispose code isn't fixed
			// System.InvalidOperationException: Handle is not initialized
			Assert.DoesNotThrow (() => {
				for (int i = 0; i < Midi.SourceCount; i++) {
					using (var endpoint = MidiEndpoint.GetSource (i)) {
						if (endpoint.Handle == 0)
							continue;
					}
				}
			});
		}
	}
}
#endif
