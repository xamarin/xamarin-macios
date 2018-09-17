//
// JSValue Unit Tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc.
//

#if !__WATCHOS__

using System;
#if XAMCORE_2_0
using Foundation;
using JavaScriptCore;
#else
using MonoTouch.Foundation;
using MonoTouch.JavaScriptCore;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.JavascriptCore {

	[TestFixture]
	// we want the test to be availble if we use the linker
	[Preserve (AllMembers = true)]
	[TestFixture]
	public class ValueTest {

		[Test]
		public void From ()
		{
			TestRuntime.AssertXcodeVersion (5, 0, 1);

			using (var c = new JSContext ()) {
				using (var d = JSValue.From (1.0, c)) {
					Assert.That (d.ToDouble (), Is.EqualTo (1.0d), "double");
					Assert.AreSame (d.Context, c, "double.Context");
					Assert.True (d.IsNumber, "double.IsNumber");
				}
			}
		}

		[Test]
		public void Invoke ()
		{
			TestRuntime.AssertXcodeVersion (5, 0, 1);

			using (var c = new JSContext ()) {
				using (var d = JSValue.From (1.0, c)) {
					Assert.That (d.Invoke ("toString").ToString (), Is.EqualTo ("1"), "toString");
				}

				using (var s1 = JSValue.From ("Hello Xamarin!", c))
				using (var s2 = JSValue.From ("Hello", c))
				using (var s3 = JSValue.From ("Bonjour", c)) {
					Assert.That (s1.Invoke ("replace", s2, s3).ToString (), Is.EqualTo ("Bonjour Xamarin!"), "replace");

					Assert.That (s1.Invoke ("replace", s2, JSValue.Null (c)).ToString (), Is.EqualTo ("null Xamarin!"), "replace-2");
				}
			}
		}

		[Test]
		public void IsEqual ()
		{
			TestRuntime.AssertXcodeVersion (5, 0, 1);

			using (var c = new JSContext ())
			using (var d = JSValue.From (1.0d, c))
			using (var f = JSValue.From (1.0f, c)) {
				Assert.True (d.IsEqualTo (d), "=== self");
				Assert.True (d.IsEqualTo (f), "=== double/float"); // it's a number now
				Assert.True (d.IsEqualTo ((NSNumber) 1.0d), "=== NSNumber");
				Assert.False (d.IsEqualTo ((NSNumber) 2.0d), "=== NSNumber-2");

				Assert.True (d.IsEqualWithTypeCoercionTo (d), "== self");
				Assert.True (d.IsEqualWithTypeCoercionTo (f), "== double/float");
				Assert.True (d.IsEqualWithTypeCoercionTo ((NSNumber) 1.0d), "== NSNumber");
				Assert.False (d.IsEqualWithTypeCoercionTo ((NSNumber) 2.0d), "== NSNumber-2");
			}
		}
	}
}

#endif // !__WATCHOS__
