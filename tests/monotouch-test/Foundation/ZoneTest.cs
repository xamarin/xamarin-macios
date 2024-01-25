//
// Unit tests for NSZone
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

using System;

using Foundation;

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

			// crash on iOS 10 beta 2
			if (TestRuntime.CheckXcodeVersion (8, 0))
				return;
			z.Name = "default";
			Assert.That (z.Name, Is.EqualTo ("default"), "Name-modified");

			// in case the test is executed more than once
			z.Name = "DefaultMallocZone";
		}
	}
}
