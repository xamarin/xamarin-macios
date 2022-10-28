//
// Unit tests for NSFileHandle
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
	public class FileHandleTest {

		[Test]
		public void Descriptor ()
		{
			using (var s = new NSFileHandle (0)) {
				// initWithFileDescriptor: does not respond (see dontlink.app) but it works
				Assert.That (s.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
			}
		}

		[Test]
		public void DescriptorClose ()
		{
			using (var s = new NSFileHandle (0, false)) {
				// initWithFileDescriptor:closeOnDealloc: does not respond (see dontlink.app) but it works
				Assert.That (s.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
			}
		}
	}
}
