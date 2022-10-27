using System;
using Foundation;
using SceneKit;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.SceneKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MorpherTest {

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);
		}

		[Test]
		public void Targets_Nullability ()
		{
			using (var m = new SCNMorpher ()) {
				// default is empty (not null)
				Assert.That (m.Targets, Is.Empty, "Targets");
				// even if set to null it would read back as empty
			}
		}
	}
}
