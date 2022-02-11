//
// Copyright 2014 Xamarin Inc
//
// Authors:
//   Miguel de Icaza
//

#if !WATCH

using Foundation;
using System;
using AudioToolbox;
using System.Runtime.Versioning;

namespace AVFoundation {
#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("macos10.10")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos")]
#endif
	public partial class AVAudioBuffer {
		public AudioBuffers AudioBufferList {
			get {
				return new AudioBuffers (audioBufferList);
			}
		}
		
		public AudioBuffers MutableAudioBufferList {
			get {
				return new AudioBuffers (mutableAudioBufferList);
			}
		}
	}
}

#endif
