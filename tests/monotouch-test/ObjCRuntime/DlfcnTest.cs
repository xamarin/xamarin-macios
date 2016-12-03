//
// Unit tests for Dlfcn
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using System;
#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
#else
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.ObjCRuntime
{

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class DlfcnTest
	{

		[Test]
		public void OpenClose_libSystem ()
		{
			IntPtr handle = Dlfcn.dlopen ("/usr/lib/libSystem.dylib", 0);
			Assert.That (handle, Is.Not.EqualTo (IntPtr.Zero), "dlopen");
			var err = Dlfcn.dlclose (handle);
#if !MONOMAC
			if ((Runtime.Arch == Arch.DEVICE) && TestRuntime.CheckXcodeVersion (7, 0)) {
				// Apple is doing some funky stuff with dlopen... this condition is to track if this change during betas
				Assert.That (err, Is.EqualTo (-1), "dlclose");
			} else {
#endif
			Assert.That (err, Is.EqualTo (0), "dlclose");
#if !MONOMAC
			}
#endif
		}
	}
}
