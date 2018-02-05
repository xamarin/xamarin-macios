#if !__WATCHOS__

using System;
using Foundation;

#if XAMCORE_2_0
using Metal;
using MetalPerformanceShaders;
#else
using MonoTouch.Metal;
using MonoTouch.MetalPerformanceShaders;
#endif

using NUnit.Framework;

namespace MonoTouchFixtures.MetalPerformanceShaders {

	[TestFixture]
	public class ImageScaleTest {

		IMTLDevice device;

		[TestFixtureSetUp]
		public void Metal ()
		{
			TestRuntime.AssertXcodeVersion (9,0);

			device = MTLDevice.SystemDefault;
			// some older hardware won't have a default
			if (device == null)
				Assert.Inconclusive ("Metal is not supported");
		}

		[Test]
		public void ScaleTransform ()
		{
			var st = new MPSScaleTransform () {
				ScaleX = 1,
				ScaleY = 2,
				TranslateX = 3,
				TranslateY = 4,
			};
			using (var scale = new MPSImageScale (device)) {
				scale.ScaleTransform = st;
				// roundtrip with our (non generated) code
				var rt = scale.ScaleTransform.Value;
				Assert.That (rt.ScaleX, Is.EqualTo (st.ScaleX), "ScaleX");
				Assert.That (rt.ScaleY, Is.EqualTo (st.ScaleY), "ScaleY");
				Assert.That (rt.TranslateX, Is.EqualTo (st.TranslateX), "TranslateX");
				Assert.That (rt.TranslateY, Is.EqualTo (st.TranslateY), "TranslateY");
			}
		}
	}
}

#endif
