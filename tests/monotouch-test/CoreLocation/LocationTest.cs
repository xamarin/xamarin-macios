//
// Unit tests for CLLocation
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012-2013 Xamarin Inc. All rights reserved.
//

using System;
#if XAMCORE_2_0
using Foundation;
using CoreLocation;
using ObjCRuntime;
using UIKit;
#else
using MonoTouch;
using MonoTouch.CoreLocation;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.CoreLocation {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class LocationTest {
		
		[Test]
		public void Fields ()
		{
			// documented as new in iOS 5.0 - https://developer.apple.com/library/ios/DOCUMENTATION/CoreLocation/Reference/CoreLocationConstantsRef/CoreLocationConstantsRef.pdf
			// in PDF only, not in the HTML documentation - but it's also inside CLError.h
			// this test will fail if Apple decide to include them in the future
			IntPtr lib = Dlfcn.dlopen (Constants.CoreLocationLibrary, 0);
			try {
				string field = "kCLErrorUserInfoAlternateRegionKey";
				var p = Dlfcn.dlsym (lib, field);
				if (TestRuntime.CheckiOSSystemVersion (7,1))
					Assert.That (p, Is.Not.EqualTo (IntPtr.Zero), field);
				else
					Assert.That (p, Is.EqualTo (IntPtr.Zero), field);
			}
			finally {
				Dlfcn.dlclose (lib);
			}
		}
	}
}
