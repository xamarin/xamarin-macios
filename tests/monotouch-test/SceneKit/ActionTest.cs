//
// Unit tests for SCNAction
//

using System;
using Foundation;
using SceneKit;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.SceneKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ActionTest {

		float timeFunctionValue;

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (6, 0);
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 10, throwIfOtherPlatform: false);
		}

#if !XAMCORE_4_0
		[Test]
		public void TimingFunction_5058 ()
		{
			// API was introduced in iOS 8, but it's broken (it copies the block pointer instead of copying the block itself, which means that a block stored on the stack will not behave as expected when trying to invoke it later)
			TestRuntime.AssertXcodeVersion (7, 0);
			var a = new SCNAction ();
			Assert.Null (a.TimingFunction2, "TimingFunction2");
			Assert.Null (a.TimingFunction, "TimingFunction");
			a.TimingFunction2 = (float f) => {
				timeFunctionValue = f;
				return timeFunctionValue;
			};
			Assert.That (a.TimingFunction2 (Single.NaN), Is.NaN, "value returned");
			a.TimingFunction (Single.NaN);
			Assert.That (timeFunctionValue, Is.NaN, "TimingFunction assigned from TimingFunction2");
		}
#endif

		[Test]
		public void TimingFunction_5072 ()
		{
			// API was introduced in iOS 8, but it's broken (it copies the block pointer instead of copying the block itself, which means that a block stored on the stack will not behave as expected when trying to invoke it later)
			TestRuntime.AssertXcodeVersion (7, 0);
			// https://github.com/xamarin/xamarin-macios/issues/5072
			var a = new SCNAction ();
#if !XAMCORE_4_0
			a.TimingFunction2 = (float f) => {
				timeFunctionValue = f;
				return timeFunctionValue;
			};
			// Assert.Null (a.TimingFunction2, "TimingFunction2-end");
#else
			a.TimingFunction = (float f) => {
				timeFunctionValue = f;
				return timeFunctionValue;
			};
#endif
			// Assert.Null (a.TimingFunction, "TimingFunction-end");
		}
	}
}