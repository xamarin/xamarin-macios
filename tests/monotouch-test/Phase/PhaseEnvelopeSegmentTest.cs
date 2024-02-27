#if __IOS__ || MONOMAC

using System;

using CoreGraphics;
using Foundation;
using ObjCRuntime;
using Phase;

using NUnit.Framework;

#if NET
using Vector2d = global::CoreGraphics.NVector2d;
#else
using OpenTK;
#endif

#nullable enable

namespace MonoTouchFixtures.Phase {

	[TestFixture]
	// we want the test to be available if we use the linker
	[Preserve (AllMembers = true)]
	public class PhaseEnvelopeSegmentTest {

		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (13, 0);
			// not supported in simulator
			TestRuntime.AssertDevice ();
		}

		[Test]
		public void ConstructorTest ()
		{
			var endPoint = new Vector2d (1, 2);
			using (var segment = new PhaseEnvelopeSegment (endPoint, PhaseCurveType.Cubed)) {
				Assert.AreEqual (endPoint, segment.EndPoint);
				var newEndPoint = new Vector2d (2, 1);
				segment.EndPoint = newEndPoint;
				Assert.AreEqual (newEndPoint, segment.EndPoint);
			}
		}

	}
}

#endif // __IOS__ || MONOMAC
