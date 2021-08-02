#if __IOS__ || MONOMAC

using System;

using CoreGraphics;
using Foundation;
using ObjCRuntime;
using Phase;

using OpenTK;
using NUnit.Framework;
using AVFoundation;

#nullable enable

namespace MonoTouchFixtures.Phase
{

	[TestFixture]
	// we want the test to be available if we use the linker
	[Preserve (AllMembers = true)]
	public class PhaseAmbientMixerDefinitionTest {

		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);
			// not supported in simulator
			TestRuntime.AssertDevice ();
		}

		[Test]
		public void TestConstructor ()
		{
			var orientation = new Quaternion (1, 2, 1, 1);
			using (var layout = new AVAudioChannelLayout ())
			using (var mixer = new PhaseAmbientMixerDefinition (layout, orientation)) {
				Assert.AreEqual (orientation, mixer.Orientation);
			}
		}
	}
}

#endif // __IOS__ || MONOMAC
