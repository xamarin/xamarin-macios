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

#if !XAMCORE_4_0
		[Test]
		public void TimingFunction_5058 ()
		{
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