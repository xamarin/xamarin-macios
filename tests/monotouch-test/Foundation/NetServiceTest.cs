//
// Unit tests for NSNetService
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using Foundation;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NetServiceTest {

		[Test]
		public void DefaultCtor ()
		{
			using (var ns = new NSNetService ("d",
											  "_test._tcp",
#if MONOMAC
											  "DeviceName",
#else
											  UIDevice.CurrentDevice.Name,
#endif
											  1234)) {
				Assert.That (ns.Domain, Is.EqualTo ("d"), "Domain");
				Assert.That (ns.Type, Is.EqualTo ("_test._tcp"), "Type");
				Assert.That (ns.Port, Is.EqualTo ((nint) 1234), "Port");
				NSInputStream input;
				NSOutputStream output;
				Assert.True (ns.GetStreams (out input, out output), "GetStreams");
				Assert.NotNull (input, "input");
				Assert.NotNull (output, "output");
			}
		}
	}
}

#endif // !__WATCHOS__
