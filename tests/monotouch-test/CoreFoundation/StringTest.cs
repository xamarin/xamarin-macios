//
// Unit tests for CFString
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012-2014 Xamarin Inc. All rights reserved.
//

using System;
#if XAMCORE_2_0
using Foundation;
using CoreFoundation;
#else
using MonoTouch.CoreFoundation;
using MonoTouch.Foundation;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.CoreFoundation {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class StringTest {
		
		[Test]
		public void ToString_ ()
		{
			using (CFString str = new CFString ("string")) {
				Assert.That (str.ToString (), Is.EqualTo ("string"), "ctor(string)");
			}
		}

		[Test]
		public void Null ()
		{
			Assert.Throws<ArgumentNullException> (delegate { new CFString (null); }, "null");
		}
	}
}
