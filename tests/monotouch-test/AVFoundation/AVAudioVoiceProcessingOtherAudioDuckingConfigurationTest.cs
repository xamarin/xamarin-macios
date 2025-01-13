//
// Unit tests for AVAudioVoiceProcessingOtherAudioDuckingConfiguration
//

#if !__TVOS__

using System;

using AVFoundation;
using Foundation;

using NUnit.Framework;

#nullable enable

namespace MonoTouchFixtures.AVFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AVAudioVoiceProcessingOtherAudioDuckingConfigurationTest {

		[Test]
		public void Properties ()
		{
			var s = new AVAudioVoiceProcessingOtherAudioDuckingConfiguration ();
			Assert.IsFalse (s.EnableAdvancedDucking, "EnableAdvancedDucking");
			Assert.AreEqual ((AVAudioVoiceProcessingOtherAudioDuckingLevel) 0, s.DuckingLevel, "DuckingLevel");

			s.EnableAdvancedDucking = true;
			Assert.IsTrue (s.EnableAdvancedDucking, "EnableAdvancedDucking 2");
			Assert.AreEqual ((AVAudioVoiceProcessingOtherAudioDuckingLevel) 0, s.DuckingLevel, "DuckingLevel 2");

			s.EnableAdvancedDucking = false;
			Assert.IsFalse (s.EnableAdvancedDucking, "EnableAdvancedDucking 3");
			Assert.AreEqual ((AVAudioVoiceProcessingOtherAudioDuckingLevel) 0, s.DuckingLevel, "DuckingLevel 3");

			s.DuckingLevel = AVAudioVoiceProcessingOtherAudioDuckingLevel.Min;
			Assert.IsFalse (s.EnableAdvancedDucking, "EnableAdvancedDucking 4");
			Assert.AreEqual (AVAudioVoiceProcessingOtherAudioDuckingLevel.Min, s.DuckingLevel, "DuckingLevel 4");

			s.DuckingLevel = (AVAudioVoiceProcessingOtherAudioDuckingLevel) 314;
			Assert.IsFalse (s.EnableAdvancedDucking, "EnableAdvancedDucking 5");
			Assert.AreEqual ((AVAudioVoiceProcessingOtherAudioDuckingLevel) 314, s.DuckingLevel, "DuckingLevel 5");

			s.DuckingLevel = AVAudioVoiceProcessingOtherAudioDuckingLevel.Default;
			Assert.IsFalse (s.EnableAdvancedDucking, "EnableAdvancedDucking 6");
			Assert.AreEqual (AVAudioVoiceProcessingOtherAudioDuckingLevel.Default, s.DuckingLevel, "DuckingLevel 6");
		}
	}
}

#endif // !__TVOS__
