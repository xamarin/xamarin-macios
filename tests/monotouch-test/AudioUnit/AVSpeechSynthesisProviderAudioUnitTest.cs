//
// Unit tests for AVSpeechSynthesisProviderAudioUnit
//

using System;
using System.Runtime.InteropServices;

using NUnit.Framework;

using AudioUnit;
using AVFoundation;
using Foundation;
using ObjCRuntime;

namespace MonoTouchFixtures.AudioUnit {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AVSpeechSynthesisProviderAudioUnitTest {
		[Test]
		public void Create ()
		{

			var cd = new AudioComponentDescription () {
				ComponentType = AudioComponentType.Output,
				ComponentSubType = AudioUnitSubType.Sampler,
				ComponentManufacturer = AudioComponentManufacturerType.Apple,
			};
			using var unit = AVSpeechSynthesisProviderAudioUnit.Create (cd, (AudioComponentInstantiationOptions) 0, out var error);
			Assert.IsNotNull (unit, "Unit");
			Assert.IsNull (error, "Error");
		}
	}
}
