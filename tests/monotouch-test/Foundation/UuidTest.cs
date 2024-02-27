//
// Unit tests for NSUuid
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012-2013 Xamarin Inc. All rights reserved.
//

using System;
using System.IO;
using Foundation;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class UuidTest {

		[Test]
		public void Constructors ()
		{
			TestRuntime.AssertXcodeVersion (4, 5);

			var uuid = new NSUuid (new byte [] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 });
			Assert.That (uuid, Is.Not.EqualTo (null), "constructed");

			var bytes = uuid.GetBytes ();
			Assert.That (bytes.Length, Is.EqualTo (16), "lenght");

			for (int i = 0; i < 16; i++)
				Assert.That (bytes [i], Is.EqualTo (i), "value " + i);
		}

		[Test]
		public void ConstructorFailures ()
		{
			TestRuntime.AssertXcodeVersion (4, 5);

			try {
				var uuid = new NSUuid ((byte []) null);
				Assert.Fail ("Should have t;hrown an exception");
			} catch (ArgumentNullException) {
				// good
			} catch (Exception e) {
				Assert.Fail ("Unexpected exception {0}", e);
			}

			try {
				var uuid = new NSUuid (new byte [] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 });
				Assert.Fail ("Should have thrown an ArgumentException");
			} catch (ArgumentException) {
				// ok
			} catch (Exception e) {
				Assert.Fail ("Expected an ArgumentException {0}", e);
			}
		}
	}
}
