// Copyright 2020 Microsoft Corp.

#if __IOS__

using System;
using Foundation;
using ObjCRuntime;
using NearbyInteraction;
using NUnit.Framework;
using OpenTK;

namespace MonoTouchFixtures.NearbyInteraction {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NINearbyObjectTest {

		[Test]
		public void DirectionNotAvailable ()
		{
			TestRuntime.AssertXcodeVersion (12, 0);

			Vector3 vect = NINearbyObject.DirectionNotAvailable;

			unsafe {
				Vector3* v = &vect;
				byte* ptr = (byte*) v;
				byte zero = 0;
				for (var i = 0; i < sizeof (Vector3); i++)
					Assert.That (ptr [i], Is.EqualTo (zero), $"Position {i}");
			}
		}
	}
}

#endif // __IOS__
