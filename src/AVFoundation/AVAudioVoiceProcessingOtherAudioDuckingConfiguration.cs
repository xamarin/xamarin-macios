using System;
using System.Runtime.InteropServices;
using ObjCRuntime;

#if !WATCH && !TVOS

namespace AVFoundation {
#if NET
    [SupportedOSPlatform ("ios17.0")]
    [SupportedOSPlatform ("macos14.0")]
    [UnsupportedOSPlatform ("tvos")]
    [SupportedOSPlatform ("maccatalyst17.0")]
#else
    [NoWatch, NoTV, Mac (14,0), iOS (17,0), MacCatalyst (17,0)]
#endif
    [StructLayout (LayoutKind.Sequential)]
    public struct AVAudioVoiceProcessingOtherAudioDuckingConfiguration
    {
        public bool EnableAdvancedDucking;

        public AVAudioVoiceProcessingOtherAudioDuckingLevel DuckingLevel;
    }
}
#endif