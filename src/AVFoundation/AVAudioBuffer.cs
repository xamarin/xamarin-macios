//
// Copyright 2014 Xamarin Inc
//
// Authors:
//   Miguel de Icaza
//

#if !WATCH

using XamCore.Foundation;
using System;
using XamCore.AudioToolbox;

namespace XamCore.AVFoundation {
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
