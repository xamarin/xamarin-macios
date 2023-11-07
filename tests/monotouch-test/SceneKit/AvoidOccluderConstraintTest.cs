using System;
using Foundation;
using SceneKit;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.SceneKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AvoidOccluderConstraintTest {

		[Test]
		public void Delegate_Nullability ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);

			using (var aoc = SCNAvoidOccluderConstraint.FromTarget (null)) {
				// header says non-null but it's null by default
				Assert.That (aoc.Delegate, Is.Null, "get");
				aoc.Delegate = new SCNAvoidOccluderConstraintDelegate ();
				Assert.That (aoc.Delegate, Is.Not.Null, "get/not-null-but-not-same");
				aoc.Delegate = null;
				Assert.That (aoc.Delegate, Is.Null, "get-reset-to-null");
			}
		}
	}
}
