#if __IOS__ || MONOMAC

using System;

using CoreGraphics;
using Foundation;
using ObjCRuntime;
using Phase;

using NUnit.Framework;

#nullable enable

namespace MonoTouchFixtures.Phase {

	[TestFixture]
	// we want the test to be available if we use the linker
	[Preserve (AllMembers = true)]
	public class PhaseObjectTest {
		PhaseEngine? engine;
		PhaseObject? phaseObject;

		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (13, 0);
			// not supported in simulator
			TestRuntime.AssertDevice ();
			engine = new PhaseEngine (PhaseUpdateMode.Automatic);
			phaseObject = new PhaseObject (engine);
		}

		[TearDown]
		public void TearDown ()
		{
			phaseObject?.Dispose ();
			engine?.Dispose ();
		}

		[Test]
		public void RightTest ()
		{
			var right = PhaseObject.Right;
			Assert.NotNull (right, "not null");
#if NET
			Assert.AreEqual (1, right.Length (), "length");
#else
			Assert.AreEqual (1, right.Length, "length");
#endif
		}

		[Test]
		public void UpTest ()
		{
			var up = PhaseObject.Up;
			Assert.NotNull (up, "not null");
#if NET
			Assert.AreEqual (1, up.Length (), "length");
#else
			Assert.AreEqual (1, up.Length, "length");
#endif
		}

		[Test]
		public void ForwardTest ()
		{
			var fwd = PhaseObject.Forward;
			Assert.NotNull (fwd, "not null");
#if NET
			Assert.AreEqual (1, fwd.Length (), "length");
#else
			Assert.AreEqual (1, fwd.Length, "length");
#endif
		}

		[Test]
		public void TransformTest ()
		{
			var matrix = phaseObject.Transform;
			Assert.NotNull (matrix, "not null");
			Assert.AreEqual (1, matrix.M11, "11");
		}

		[Test]
		public void WorldTransform ()
		{
			var matrix = phaseObject.WorldTransform;
			Assert.NotNull (matrix, "not null");
			Assert.AreEqual (1, matrix.M11, "11");
		}

	}
}

#endif // __IOS__ || MONOMAC
