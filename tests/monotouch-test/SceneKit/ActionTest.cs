//
// Unit tests for SCNAction
//

using System;
using CoreAnimation;
using Foundation;
using SceneKit;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.SceneKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ActionTest {

		float timeFunctionValue;

		[Test]
		public void TimingFunction_5058 ()
		{
			var a = new SCNAction ();
			Assert.Null (a.TimingFunction2, "TimingFunction2");
#if !XAMCORE_4_0
			Assert.Null (a.TimingFunction, "TimingFunction");
#endif
			a.TimingFunction2 = (float f) => {
				timeFunctionValue = f;
				return timeFunctionValue;
			};
			Assert.That (a.TimingFunction2 (Single.NaN), Is.NaN, "value returned");
#if !XAMCORE_4_0
			a.TimingFunction (Single.NaN);
			Assert.That (timeFunctionValue, Is.NaN, "TimingFunction assigned from TimingFunction2");
#endif
			a.TimingFunction2 = null;
			// https://github.com/xamarin/xamarin-macios/issues/5072
			// Assert.Null (a.TimingFunction2, "TimingFunction2-end");
#if !XAMCORE_4_0
			// Assert.Null (a.TimingFunction, "TimingFunction-end");
#endif
		}
	}
}
