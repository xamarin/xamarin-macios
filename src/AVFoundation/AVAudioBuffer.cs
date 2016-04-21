//
// Copyright 2014 Xamarin Inc
//
// Authors:
//   Miguel de Icaza
//
using XamCore.Foundation;
using XamCore.ObjCRuntime;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
