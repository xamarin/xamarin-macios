//
// AudioUnitEventArgs.cs: AudioUnit callback argument
//
// Author:
//   AKIHIRO Uehara (u-akihiro@reinforce-lab.com)
//
// Copyright 2010 Reinforce Lab.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using ObjCRuntime;
using AudioToolbox;
using Foundation;

namespace AudioUnit
{
    public static class AudioUnitUtils
    {
        public const int SampleFractionBits = 24;

#if !XAMCORE_2_0
        [Advice ("Use 'AudioStreamBasicDescription::CreateLinearPCM' instead.")]
        public static AudioStreamBasicDescription AUCanonicalASBD(double sampleRate, int channel)
        {
            // setting AudioStreamBasicDescription
            int AudioUnitSampleTypeSize = 		
#if !MONOMAC
            (ObjCRuntime.Runtime.Arch == ObjCRuntime.Arch.SIMULATOR) ? sizeof(float) : sizeof(int);
#else
		sizeof (float);
#endif
            AudioStreamBasicDescription audioFormat = new AudioStreamBasicDescription()
            {
                SampleRate = sampleRate,
                Format = AudioFormatType.LinearPCM,
                //kAudioFormatFlagsAudioUnitCanonical = kAudioFormatFlagIsSignedInteger | kAudioFormatFlagsNativeEndian | kAudioFormatFlagIsPacked | kAudioFormatFlagIsNonInterleaved | (SampleFractionBits << kLinearPCMFormatFlagsSampleFractionShift),
                FormatFlags      = (AudioFormatFlags)((int)AudioFormatFlags.IsSignedInteger | (int)AudioFormatFlags.IsPacked | (int)AudioFormatFlags.IsNonInterleaved | (int)(SampleFractionBits << (int)AudioFormatFlags.LinearPCMSampleFractionShift)),
                ChannelsPerFrame = channel,
                BytesPerPacket   = AudioUnitSampleTypeSize,
                BytesPerFrame    = AudioUnitSampleTypeSize,
                FramesPerPacket  = 1,
                BitsPerChannel   = 8 * AudioUnitSampleTypeSize,
                Reserved = 0
            };
            return audioFormat;
        }

        [Advice ("Use 'AudioSession::OverrideCategoryDefaultToSpeaker' instead.")]
        public static void SetOverrideCategoryDefaultToSpeaker(bool isSpeaker)
        {
		int val = isSpeaker ? 1 : 0;
		int err = AudioSessionSetProperty(
			0x6373706b, //'cspk'
			(UInt32)sizeof(UInt32),
			ref val);
		if (err != 0)
			throw new ArgumentException();            
        }

	    [DllImport(Constants.AudioToolboxLibrary, EntryPoint = "AudioSessionSetProperty")]
	    static extern int AudioSessionSetProperty(
		    UInt32 inID,
		    UInt32 inDataSize,
		    ref int inData);
#endif
    }
}
