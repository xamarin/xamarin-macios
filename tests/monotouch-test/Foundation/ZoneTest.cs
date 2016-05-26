//
// Unit tests for NSZone
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

using System;

#if XAMCORE_2_0
using Foundation;
#else
using MonoTouch.Foundation;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ZoneTest {

		[Test]
		public void Default ()
		{
			var z = NSZone.Default;
			Assert.That (z.Handle, Is.Not.EqualTo (IntPtr.Zero), "Default");
			Assert.That (z.Name, Is.EqualTo ("DefaultMallocZone"), "Name");

			z.Name = "default";
			Assert.That (z.Name, Is.EqualTo ("default"), "Name-modified");

			// in case the test is executed more than once
			z.Name = "DefaultMallocZone";
		}
	}
}
