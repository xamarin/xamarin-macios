// 
// AudioSessions.cs:
//
// Authors: Mono Team
//     
// Copyright 2009 Novell, Inc
// Copyright 2011, 2012 Xamarin Inc.
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
using System.Runtime.InteropServices;
using CoreFoundation;
using ObjCRuntime;

#nullable enable

namespace AudioToolbox {

	public enum AudioSessionErrors { // Implictly cast to OSType 
		None = 0,
		NotInitialized = 0x21696e69, // '!ini',
		AlreadyInitialized = 0x696e6974, // 'init',
		InitializationError = 0x696e693f, // 'ini?',
		UnsupportedPropertyError = 0x7074793f, // 'pty?',
		BadPropertySizeError = 0x2173697a, // '!siz',
		NotActiveError = 0x21616374, // '!act',
		NoHardwareError = 0x6e6f6877, // 'nohw'
		IncompatibleCategory = 0x21636174, // '!cat'
		NoCategorySet = 0x3f636174, // '?cat'
		UnspecifiedError = 0x77686371, // 'what'
	}

	public enum AudioSessionInterruptionState { // UInt32 in AudioSessionInterruptionListener
		End = 0,
		Begin = 1,
	}

	public enum AudioSessionCategory { // UInt32 AudioSessionPropertyID
		AmbientSound = 0x616d6269, // 'ambi'
		SoloAmbientSound = 0x736f6c6f, // 'solo'
		MediaPlayback = 0x6d656469, // 'medi'
		RecordAudio = 0x72656361, // 'reca'
		PlayAndRecord = 0x706c6172, // 'plar'
		AudioProcessing = 0x70726f63  // 'proc'
	}

	public enum AudioSessionRoutingOverride { // UInt32 AudioSessionPropertyID
		None = 0,
		Speaker = 0x73706b72, // 'spkr'
	}

	public enum AudioSessionRouteChangeReason { // UInt32 AudioSessionPropertyID
		Unknown = 0,
		NewDeviceAvailable = 1,
		OldDeviceUnavailable = 2,
		CategoryChange = 3,
		Override = 4,
		WakeFromSleep = 6,
		NoSuitableRouteForCategory = 7,
		RouteConfigurationChange = 8
	}

	public enum AudioSessionInterruptionType { // UInt32 AudioSessionInterruptionType
		ShouldResume = 1769108333, // 'irsm'
		ShouldNotResume = 561148781, // '!rsm'
	}

	// Should be internal with AudioSessionPropertyListener public
	public enum AudioSessionProperty { // typedef UInt32 AudioSessionPropertyID
		PreferredHardwareSampleRate = 0x68777372,
		PreferredHardwareIOBufferDuration = 0x696f6264,
		AudioCategory = 0x61636174, // 'acat'
		[Deprecated (PlatformName.iOS, 5, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 7)]
		[Deprecated (PlatformName.TvOS, 9, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		AudioRoute = 0x726f7574,
		AudioRouteChange = 0x726f6368,
		CurrentHardwareSampleRate = 0x63687372,
		CurrentHardwareInputNumberChannels = 0x63686963,
		CurrentHardwareOutputNumberChannels = 0x63686f63,
		CurrentHardwareOutputVolume = 0x63686f76,
		CurrentHardwareInputLatency = 0x63696c74,
		CurrentHardwareOutputLatency = 0x636f6c74,
		CurrentHardwareIOBufferDuration = 0x63686264,
		OtherAudioIsPlaying = 0x6f746872,
		OverrideAudioRoute = 0x6f767264,
		AudioInputAvailable = 0x61696176,
		ServerDied = 0x64696564,
		[Deprecated (PlatformName.iOS, 7, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 9)]
		[Deprecated (PlatformName.TvOS, 9, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		OtherMixableAudioShouldDuck = 0x6475636b,
		OverrideCategoryMixWithOthers = 0x636d6978,
		OverrideCategoryDefaultToSpeaker = 0x6373706b, //'cspk'
		OverrideCategoryEnableBluetoothInput = 0x63626c75, //'cblu'
		InterruptionType = 0x74797065,      // 'type'
		Mode = 0x6d6f6465,
		InputSources = 0x73726373,      // 'srcs'
		OutputDestinations = 0x64737473,        // 'dsts'
		InputSource = 0x69737263,       // 'isrc'
		OutputDestination = 0x6f647374,     // 'odst'
		InputGainAvailable = 0x69676176,        // 'igav'
		InputGainScalar = 0x69677363,       // 'igsc'
		AudioRouteDescription = 0x63726172,     // 'crar'
	}

	public enum AudioSessionMode { // UInt32 AudioSessionPropertyID
		Default = 0x64666c74,
		VoiceChat = 0x76636374,
		VideoRecording = 0x76726364,
		Measurement = 0x6d736d74,   // 'msmt'
		GameChat = 0x676d6374,  // 'gmct'
	}

	[Deprecated (PlatformName.iOS, 6, 0)]
	[Deprecated (PlatformName.TvOS, 9, 0)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
	public enum AudioSessionActiveFlags : uint // UInt32 in AudioSessionSetActiveWithFlags
	{
		NotifyOthersOnDeactivation = (1 << 0)
	}

	public enum AudioSessionInputRouteKind { // UInt32 AudioSessionPropertyID
		None,
		LineIn,
		BuiltInMic,
		HeadsetMic,
		BluetoothHFP,
		USBAudio,
	}

	public enum AudioSessionOutputRouteKind { // UInt32           (set only) in AudioSession.h
		None,
		LineOut,
		Headphones,
		BluetoothHFP,
		BluetoothA2DP,
		BuiltInReceiver,
		BuiltInSpeaker,
		USBAudio,
		HDMI,
		AirPlay,
	}
}
