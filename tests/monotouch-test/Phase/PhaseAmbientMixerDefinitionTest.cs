#if __IOS__ || MONOMAC

using System;

using CoreGraphics;
using Foundation;
using ObjCRuntime;
using Phase;

using NUnit.Framework;
using AVFoundation;
using AudioToolbox;

#if NET
using System.Numerics;
#else
using OpenTK;
#endif

#nullable enable

namespace MonoTouchFixtures.Phase {

	[TestFixture]
	// we want the test to be available if we use the linker
	[Preserve (AllMembers = true)]
	public class PhaseAmbientMixerDefinitionTest {

		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (13, 0);
			// not supported in simulator
			TestRuntime.AssertDevice ();
		}

		[Test]
		public void TestConstructor ()
		{
			var orientation = new Quaternion (1, 2, 1, 1);
			var audioChannelLayout = new AudioChannelLayout {
				Tag = (int) AudioChannelLayoutTag.UseChannelDescriptions,
				Bitmap = 0,
				Channels = new AudioChannelDescription [] {
					new AudioChannelDescription {
						Flags = AudioChannelFlags.AllOff,
						Label = AudioChannelLabel.Discrete_4,
						Coords = new float [] { 0, 0, 0},
					},
					new AudioChannelDescription {
						Flags = AudioChannelFlags.AllOff,
						Label = AudioChannelLabel.Discrete_5,
						Coords = new float [] { 0, 0, 0},
					}
				}
			};
			using (var layout = new AVAudioChannelLayout (audioChannelLayout))
			using (var mixer = new PhaseAmbientMixerDefinition (layout, orientation)) {
				Assert.AreEqual (orientation, mixer.Orientation);
			}
		}
	}
}

#endif // __IOS__ || MONOMAC
