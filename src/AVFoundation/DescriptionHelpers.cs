// 
// DescriptionHelpers.cs: Assorted helpers to deal with the the class/struct problem of AudioComponentDescription
//
// Authors: Miguel de Icaza (miguel@xamarin.com)
//     
// Copyright 2014 Xamarin Inc.
//

#if !WATCH

using System;

using Foundation;
using CoreFoundation;
using ObjCRuntime;
using AudioToolbox;
using AudioUnit;

namespace AVFoundation {

#if !XAMCORE_2_0
	public partial class AVAudioUnitEffect {
		public AVAudioUnitEffect (AudioComponentDescription desc) : this (new AudioComponentDescriptionNative (desc)) {}
	}

	public partial class AVAudioUnitTimeEffect {
		public AVAudioUnitTimeEffect (AudioComponentDescription desc) : this (new AudioComponentDescriptionNative (desc)) {}
	}

	public partial class AVAudioUnitTimePitch {
		public AVAudioUnitTimePitch (AudioComponentDescription desc) : this (new AudioComponentDescriptionNative (desc)) {}
	}

	public partial class AVAudioUnitVarispeed {
		public AVAudioUnitVarispeed (AudioComponentDescription desc) : this (new AudioComponentDescriptionNative (desc)) {}
	}

	public partial class AVAudioUnitGenerator {
		public AVAudioUnitGenerator (AudioComponentDescription desc) : this (new AudioComponentDescriptionNative (desc)) {}
	}

	public partial class AVAudioUnitMidiInstrument {
		public AVAudioUnitMidiInstrument (AudioComponentDescription desc) : this (new AudioComponentDescriptionNative (desc)) {}
	}

#endif
}

#endif
