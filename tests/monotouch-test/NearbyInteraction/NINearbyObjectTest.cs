// Copyright 2020 Microsoft Corp.

#if __IOS__

using System;
using Foundation;
using ObjCRuntime;
using NearbyInteraction;
using NUnit.Framework;
using Xamarin.Utils;

#if NET
using System.Numerics;
#else
using OpenTK;
#endif

namespace MonoTouchFixtures.NearbyInteraction {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NINearbyObjectTest {
		[SetUp]
		public void Setup ()
		{
			// The API here was introduced to Mac Catalyst later than for the other frameworks, so we have this additional check
			TestRuntime.AssertSystemVersion (ApplePlatform.MacCatalyst, 14, 0, throwIfOtherPlatform: false);
		}

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
