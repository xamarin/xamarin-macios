//
// JSContext Unit Tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013, 2015 Xamarin Inc.
//

#if !__WATCHOS__

using System;
using Foundation;
using JavaScriptCore;
using NUnit.Framework;

namespace MonoTouchFixtures.JavascriptCore {

	[TestFixture]
	// we want the test to be availble if we use the linker
	[Preserve (AllMembers = true)]
	[TestFixture]
	public class ContextTest {

		[Test]
		public void EvaluateScript ()
		{
			TestRuntime.AssertXcodeVersion (5, 0, 1);

			using (var c = new JSContext ())
			using (JSValue r = c.EvaluateScript ("function FourthyTwo () { return 42; }; FourthyTwo ()")) {
				Assert.That (r.ToInt32 (), Is.EqualTo (42), "42");
			}
		}

		[Test]
		public void EvaluateScript_Param ()
		{
			TestRuntime.AssertXcodeVersion (5, 0, 1);

			using (var context = new JSContext ())
			using (JSValue script = context.EvaluateScript ("var square = function (x) { return x * x; }"))
			using (JSValue function = context [(NSString) "square"])
			using (JSValue input = JSValue.From (2, context))
			using (JSValue result = function.Call (input)) {
				Assert.That (result.ToInt32 (), Is.EqualTo (4), "4");
			}
		}

		[Test]
		public void EvaluateScript_Context ()
		{
			TestRuntime.AssertXcodeVersion (5, 0, 1);

			using (var context = new JSContext ())
			using (JSValue value = context.EvaluateScript ("a = 3"))
			using (JSValue script = context.EvaluateScript ("var square = function (x) { return x * x; }"))
			using (JSValue function = context [(NSString) "square"])
			using (JSValue result = function.Call (context [(NSString) "a"])) {
				Assert.That (result.ToInt32 (), Is.EqualTo (9), "9");
			}
		}
	}
}

#endif // !__WATCHOS__
