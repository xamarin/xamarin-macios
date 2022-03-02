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

namespace AVFoundation {
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
