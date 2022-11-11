//
// Unit tests for CFString
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012-2014 Xamarin Inc. All rights reserved.
//

using System;
using Foundation;
using CoreFoundation;
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

		[Test]
		public void Index ()
		{
			var str = "Ab🤔日ㅁ名";
			using var nativeStr = new CFString (str);
			var array = str.ToCharArray ();
			for (int i = 0; i < array.Length; i++) {
				Assert.AreEqual (str [i], nativeStr [i], $"{str [i]} != {nativeStr [i]}");
			}
		}
	}
}
