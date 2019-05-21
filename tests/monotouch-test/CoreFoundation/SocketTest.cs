//
// Unit tests for CFSocket
//
// Authors:
//	Marius Ungureanu <maungu@microsoft.com>
//
// Copyright 2019 Microsoft Inc. All rights reserved.
//

using System;
#if XAMCORE_2_0
using Foundation;
using CoreFoundation;
using ObjCRuntime;
#else
using MonoTouch.CoreFoundation;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.CoreFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CFSocketTest
	{
		[Test]
		public void RetainCount ()
		{
			// All constructors end up using the shared private constructor.
			using (var socket = new CFSocket ()) {
				Assert.That (TestRuntime.CFGetRetainCount (socket.Handle), Is.EqualTo (1), "RetainCount");
			}
		}
	}
}
