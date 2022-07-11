//
// Framework tests
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
using System.Runtime.InteropServices;

using MonoTouch;
using Foundation;
using ObjCRuntime;

using NUnit.Framework;

using Bindings.Test;

namespace MonoTouchFixtures {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class FrameworkTests {
		[Test]
		public void CFunction ()
		{
			Assert.AreEqual (42, CFunctions.theUltimateAnswer (), "a");
		}

		[Test]
		public void ObjCClass ()
		{
			using (var obj = new FrameworkTest ()) {
				Assert.AreEqual (42, obj.Func (), "a");
			}
		}
	}
}
