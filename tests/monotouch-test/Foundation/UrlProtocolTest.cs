//
// Unit tests for NSUrlProtocol
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using System;
using System.IO;
#if XAMCORE_2_0
using Foundation;
using UIKit;
using ObjCRuntime;
#else
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class UrlProtocolTest {
		[Test]
		public void Registration ()
		{
			Class c = new Class (typeof (CustomProtocol));
			bool res;
			
			res = NSUrlProtocol.RegisterClass (c);
			
			Assert.That (res, "#1");
			
			NSUrlProtocol.UnregisterClass (c);
		}

		class CustomProtocol : NSUrlProtocol
		{
		}

		// API disabled - see comments in src/foundation.cs
#if false
		[Test]
		public void CanInitWithTask ()
		{
			// NSInvalidArgumentException Reason: *** -canInitWithRequest: cannot be sent to an abstract object of class NSURLProtocol: Create a concrete instance!
			using (var t = new NSUrlSessionTask ()) {
				Assert.False (NSUrlProtocol.CanInitWithTask (t), "CanInitWithTask");
			}
		}

		[Test]
		public void Task ()
		{
			// NSInvalidArgumentException -[MonoTouchFixtures_Foundation_UrlProtocolTest_CustomProtocol task]: unrecognized selector sent to instance 0x7ff4c910
			using (var p = new CustomProtocol ()) {
				Assert.Null (p.Task, "Task");
			}
		}
#endif
	}
}
