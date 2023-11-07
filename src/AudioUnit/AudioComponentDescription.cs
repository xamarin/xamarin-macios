//
// AudioComponentDescription.cs: AudioComponentDescription wrapper class
//
// Author:
//   AKIHIRO Uehara (u-akihiro@reinforce-lab.com)
//   Miguel de Icaza (miguel@gnome.org)
//
// Copyright 2010 Reinforce Lab.
// Copyright 2010 Novell, Inc
// Copyright 2011, 2012 Xamarin Inc
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

#nullable enable

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ObjCRuntime;
using AudioToolbox;
using Foundation;

namespace AudioUnit {
	public enum AudioComponentType : uint { // OSType in AudioComponentDescription
		Output = 0x61756f75, //'auou',
		MusicDevice = 0x61756d75, // 'aumu'
		MusicEffect = 0x61756d66, // 'aumf'
		FormatConverter = 0x61756663, // 'aufc'
		Effect = 0x61756678, // 'aufx'
		Mixer = 0x61756d78, // 'aumx'
		Panner = 0x6175706e, // 'aupn'
		OfflineEffect = 0x61756f6c, // 'auol'
		Generator = 0x6175676e, // 'augn'
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		MIDIProcessor = 0x61756d69, // 'aumi'
#if NET
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("tvos16.0")]
#else
		[iOS (16, 0), Mac (13, 0), TV (16, 0), MacCatalyst (16, 0)]
#endif
		SpeechSynthesize = 0x61757370, // ausp

#if !MONOMAC
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		RemoteEffect = 0x61757278, // 'aurx',
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		RemoteGenerator = 0x61757267, // 'aurg',
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		RemoteInstrument = 0x61757269, // 'auri',
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		RemoteMusicEffect = 0x6174726d, // 'aurm'
#endif
	}

	public enum AudioTypeOutput { // OSType in AudioComponentDescription
		Generic = 0x67656e72, // 'genr'
#if MONOMAC
		HAL=0x6168616c, // 'ahal'
		Default=0x64656620, // 'def'
		System=0x73797320, // 'sys'
#endif
#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		Remote = 0x72696f63, // 'rioc'
		VoiceProcessingIO = 0x7670696f // 'vpio'
	}

	public enum AudioTypeMusicDevice { // OSType in AudioComponentDescription
#if MONOMAC
		DlsSynth	= 0x646c7320, // 'dls '
#endif
		Sampler = 0x73616d70, // 'samp'

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		MidiSynth = 0x6d73796e, // 'msyn'
	}

	public enum AudioTypeConverter { // OSType in AudioComponentDescription
		AU = 0x636f6e76, // 'conv'
		Varispeed = 0x76617269, // 'vari'
		DeferredRenderer = 0x64656672, // 'defr'
		Splitter = 0x73706c74, // 'splt'
		Merger = 0x6d657267, // 'merg'
		NewTimePitch = 0x6e757470, // 'nutp'
		AUiPodTimeOther = 0x6970746f, // 'ipto
		RoundTripAAC = 0x72616163, // 'raac'
		MultiSplitter = 0x6d73706c, // 'mspl'
#if MONOMAC
		TimePitch=0x746d7074, // 'tmpt'
#else
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("tvos13.0", "Use 'AudioTypeConverter.NewTimePitch' instead.")]
		[ObsoletedOSPlatform ("ios13.0", "Use 'AudioTypeConverter.NewTimePitch' instead.")]
		[ObsoletedOSPlatform ("maccatalyst13.1", "Use 'AudioTypeConverter.NewTimePitch' instead.")]
#else
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'AudioTypeConverter.NewTimePitch' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'AudioTypeConverter.NewTimePitch' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'AudioTypeConverter.NewTimePitch' instead.")]
#endif
		AUiPodTime = 0x6970746d, // 'iptm'
#endif
	}

	public enum AudioTypeEffect { // OSType in AudioComponentDescription
		PeakLimiter = 0x6c6d7472, // 'lmtr'
		DynamicsProcessor = 0x64636d70, // 'dcmp'
		LowPassFilter = 0x6c706173, // 'lpas'
		HighPassFilter = 0x68706173, // 'hpas'
		HighShelfFilter = 0x68736866, // 'hshf'
		LowShelfFilter = 0x6c736866, // 'lshf'
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("ios7.0")]
		[ObsoletedOSPlatform ("maccatalyst13.1")]
		[ObsoletedOSPlatform ("tvos9.0")]
		[ObsoletedOSPlatform ("macos10.9")]
#else
		[Obsoleted (PlatformName.iOS, 7, 0)]
#endif
		DCFilter = 0x6463666c, // 'dcfl'
		ParametricEQ = 0x706d6571, // 'pmeq'
		Delay = 0x64656c79, // 'dely'

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		SampleDelay = 0x73646c79, // 'sdly'
		Distortion = 0x64697374, // 'dist'
		BandPassFilter = 0x62706173, // 'bpas'
#if MONOMAC
		GraphicEQ=0x67726571, // 'greq'
		MultiBandCompressor=0x6d636d70, // 'mcmp'
		MatrixReverb=0x6d726576, // 'mrev'
		Pitch=0x70697463, // 'pitc'
		AUFilter=0x66696c74, // 'filt'
		NetSend=0x6e736e64, // 'nsnd'
		RogerBeep=0x726f6772, // 'rogr'
#else
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("tvos13.0", "Use 'AudioTypeEffect.GraphicEQ' instead.")]
		[ObsoletedOSPlatform ("ios13.0", "Use 'AudioTypeEffect.GraphicEQ' instead.")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Unavailable (PlatformName.MacCatalyst)]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'AudioTypeEffect.GraphicEQ' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'AudioTypeEffect.GraphicEQ' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'AudioTypeEffect.GraphicEQ' instead.")]
#endif
		AUiPodEQ = 0x69706571, // 'ipeq'
#endif
#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Unavailable (PlatformName.MacCatalyst)]
#endif
		Reverb2 = 0x72766232, // 'rvb2'
		NBandEq = 0x6e626571, // 'nbeq'
	}

	public enum AudioTypeMixer { // OSType in AudioComponentDescription
		MultiChannel = 0x6d636d78, // 'mcmx'
		Matrix = 0x6d786d78, // 'mxmx'
		Spacial = 0x3364656d, // Same as Embedded3D
#if MONOMAC
		Stereo=0x736d7872, // 'smxr'
#if NET
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos10.10", "Use 'Spacial' instead.")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'Spacial' instead.")]
#endif
		ThreeD=0x33646d78, // '3dmx'
#else
#if NET
		[SupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("ios8.0", "Use 'Spacial' instead.")]
#endif
		Embedded3D = 0x3364656d, // '3dem'
#endif
	}

	public enum AudioTypePanner { // OSType in AudioComponentDescription
#if MONOMAC
		SphericalHead=0x73706872, // 'sphr'
		Vector=0x76626173, // 'vbas'
		SoundField=0x616d6269, // 'ambi'
		rHRTF=0x68727466, // 'hrtf'
#endif
	}

	public enum AudioTypeGenerator { // OSType in AudioComponentDescription
#if MONOMAC
		NetReceive=0x6e726376, // 'nrcv'
#endif
		ScheduledSoundPlayer = 0x7373706c, // 'sspl'
		AudioFilePlayer = 0x6166706c, // 'afpl'
	}

	public enum AudioComponentManufacturerType : uint // OSType in AudioComponentDescription
	{
		Apple = 0x6170706c // little endian 0x6c707061 //'appl'
	}

	[Flags]
	public enum AudioComponentFlag // UInt32 in AudioComponentDescription
	{
		Unsearchable = 1,
		SandboxSafe = 2,
		IsV3AudioUnit = 4,
		RequiresAsyncInstantiation = 8,
		CanLoadInProcess = 0x10
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct AudioComponentDescription {
		[MarshalAs (UnmanagedType.U4)]
		public AudioComponentType ComponentType;

		[MarshalAs (UnmanagedType.U4)]
#if NET && !COREBUILD
		public AudioUnitSubType ComponentSubType;
#else
		public int ComponentSubType;
#endif

		[MarshalAs (UnmanagedType.U4)]
		public AudioComponentManufacturerType ComponentManufacturer;

		public AudioComponentFlag ComponentFlags;
		public int ComponentFlagsMask;

		internal AudioComponentDescription (AudioComponentType type, int subType)
		{
			ComponentType = type;
#if NET && !COREBUILD
			ComponentSubType = (AudioUnitSubType) subType;
#else
			ComponentSubType = subType;
#endif
			ComponentManufacturer = AudioComponentManufacturerType.Apple;
			ComponentFlags = (AudioComponentFlag) 0;
			ComponentFlagsMask = 0;
		}

		public static AudioComponentDescription CreateGeneric (AudioComponentType type, int subType)
		{
			return new AudioComponentDescription (type, subType);
		}

		public static AudioComponentDescription CreateOutput (AudioTypeOutput outputType)
		{
			return new AudioComponentDescription (AudioComponentType.Output, (int) outputType);
		}

		public static AudioComponentDescription CreateMusicDevice (AudioTypeMusicDevice musicDevice)
		{
			return new AudioComponentDescription (AudioComponentType.MusicDevice, (int) musicDevice);
		}

		public static AudioComponentDescription CreateConverter (AudioTypeConverter converter)
		{
			return new AudioComponentDescription (AudioComponentType.FormatConverter, (int) converter);
		}

		public static AudioComponentDescription CreateEffect (AudioTypeEffect effect)
		{
			return new AudioComponentDescription (AudioComponentType.Effect, (int) effect);
		}

		public static AudioComponentDescription CreateMixer (AudioTypeMixer mixer)
		{
			return new AudioComponentDescription (AudioComponentType.Mixer, (int) mixer);
		}

		public static AudioComponentDescription CreatePanner (AudioTypePanner panner)
		{
			return new AudioComponentDescription (AudioComponentType.Panner, (int) panner);
		}

		public static AudioComponentDescription CreateGenerator (AudioTypeGenerator generator)
		{
			return new AudioComponentDescription (AudioComponentType.Generator, (int) generator);
		}

		public override string ToString ()
		{
			const string fmt = "[componentType={0}, subType={1}]";

			switch (ComponentType) {
			case AudioComponentType.Output:
				return String.Format (fmt, ComponentType, (AudioTypeOutput) ComponentSubType);
			case AudioComponentType.MusicDevice:
				return String.Format (fmt, ComponentType, (AudioTypeMusicDevice) ComponentSubType);
			case AudioComponentType.FormatConverter:
				return String.Format (fmt, ComponentType, (AudioTypeConverter) ComponentSubType);
			case AudioComponentType.Effect:
				return String.Format (fmt, ComponentType, (AudioTypeEffect) ComponentSubType);
			case AudioComponentType.Mixer:
				return String.Format (fmt, ComponentType, (AudioTypeMixer) ComponentSubType);
			case AudioComponentType.Panner:
				return String.Format (fmt, ComponentType, (AudioTypePanner) ComponentSubType);
			case AudioComponentType.Generator:
				return String.Format (fmt, ComponentType, (AudioTypeGenerator) ComponentSubType);
			default:
				return String.Format (fmt, ComponentType, ComponentSubType);
			}
		}
	}
}
