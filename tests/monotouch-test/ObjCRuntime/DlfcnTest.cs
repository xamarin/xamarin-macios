//
// Unit tests for Dlfcn
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using System;
using Foundation;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.ObjCRuntime {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class DlfcnTest {

		[Test]
		public void OpenClose_libSystem ()
		{
			IntPtr handle = Dlfcn.dlopen ("/usr/lib/libSystem.dylib", 0);
			Assert.That (handle, Is.Not.EqualTo (IntPtr.Zero), "dlopen");
			var err = Dlfcn.dlclose (handle);
			var expected = 0;
#if !MONOMAC && !__MACCATALYST__
			if (Runtime.Arch == Arch.DEVICE && TestRuntime.CheckXcodeVersion (7, 0) && !TestRuntime.CheckXcodeVersion (10, 0)) {
				// Apple is doing some funky stuff with dlopen... this condition is to track if this change during betas
				expected = -1;
			}
#endif
			Assert.That (err, Is.EqualTo (expected), "dlclose");
		}
	}
}
