#if __MACOS__
//
// Unit tests for the assembly itself
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

using System;
using System.Reflection;
using Foundation;
using NUnit.Framework;

namespace MonoMacFixtures {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AssemblyTest {

		static byte [] pkt = { 0x84, 0xe0, 0x4f, 0xf9, 0xcf, 0xb7, 0x90, 0x65 };

		[Test]
		public void PublicKeyToken ()
		{
			Assert.AreEqual (pkt, typeof (NSObject).Assembly.GetName ().GetPublicKeyToken (), "GetPublicKeyToken");
		}
	}
}
#endif // __MACOS__
