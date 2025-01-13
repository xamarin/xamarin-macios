using System;
using System.Runtime.InteropServices;
using ObjCRuntime;

#if !TVOS

namespace AVFoundation {
	[SupportedOSPlatform ("ios17.0")]
	[SupportedOSPlatform ("macos14.0")]
	[UnsupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("maccatalyst17.0")]
	public struct AVAudioVoiceProcessingOtherAudioDuckingConfiguration {
		byte enableAdvancedDucking;
#pragma warning disable CS0169 // The field 'AVAudioVoiceProcessingOtherAudioDuckingConfiguration.duckingLevel' is never used
		nint duckingLevel;
#pragma warning restore CS0169

		public bool EnableAdvancedDucking {
			get => enableAdvancedDucking != 0;
			set => enableAdvancedDucking = value.AsByte ();
		}

#if !COREBUILD
		public AVAudioVoiceProcessingOtherAudioDuckingLevel DuckingLevel {
			get => (AVAudioVoiceProcessingOtherAudioDuckingLevel) (long) duckingLevel;
			set => duckingLevel = (nint) (long) value;
		}
#endif
	}
}
#endif
