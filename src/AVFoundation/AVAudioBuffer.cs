//
// Copyright 2014 Xamarin Inc
//
// Authors:
//   Miguel de Icaza
//

using Foundation;
using System;
using AudioToolbox;

#nullable enable

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
