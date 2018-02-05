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

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ObjCRuntime;
using AudioToolbox;

namespace AudioUnit
{
    public enum AudioComponentType : uint { // OSType in AudioComponentDescription
		Output = 0x61756f75, //'auou',
		MusicDevice=0x61756d75, // 'aumu'
		MusicEffect=0x61756d66, // 'aumf'
		FormatConverter=0x61756663, // 'aufc'
		Effect=0x61756678, // 'aufx'
		Mixer=0x61756d78, // 'aumx'
		Panner=0x6175706e, // 'aupn'
		OfflineEffect=0x61756f6c, // 'auol'
		Generator=0x6175676e, // 'augn'
		[iOS (7,0)]
		MIDIProcessor		= 0x61756d69, // 'aumi'

#if !MONOMAC
		[iOS (7,0)]
		RemoteEffect		= 0x61757278, // 'aurx',
		[iOS (7,0)]
		RemoteGenerator		= 0x61757267, // 'aurg',
		[iOS (7,0)]
		RemoteInstrument	= 0x61757269, // 'auri',
		[iOS (7,0)]
		RemoteMusicEffect	= 0x6174726d, // 'aurm'
#endif
	}

	public enum AudioTypeOutput { // OSType in AudioComponentDescription
		Generic = 0x67656e72, // 'genr'
#if MONOMAC
		HAL=0x6168616c, // 'ahal'
		Default=0x64656620, // 'def'
		System=0x73797320, // 'sys'
#else
		Remote=0x72696f63, // 'rioc'
#endif
		VoiceProcessingIO = 0x7670696f // 'vpio'
	}

	public enum AudioTypeMusicDevice { // OSType in AudioComponentDescription
#if !XAMCORE_2_0
		[Obsolete]
		None,
#endif
#if MONOMAC
		DlsSynth	= 0x646c7320, // 'dls '
#endif
		Sampler		= 0x73616d70, // 'samp'

		[iOS (8,0)]
		MidiSynth	= 0x6d73796e, // 'msyn'
	}

	public enum AudioTypeConverter { // OSType in AudioComponentDescription
		AU=0x636f6e76, // 'conv'
		Varispeed=0x76617269, // 'vari'
		DeferredRenderer=0x64656672, // 'defr'
		Splitter=0x73706c74, // 'splt'
		Merger=0x6d657267, // 'merg'
		NewTimePitch=0x6e757470, // 'nutp'
		AUiPodTimeOther=0x6970746f, // 'ipto
		RoundTripAAC=0x72616163, // 'raac'
		MultiSplitter=0x6d73706c, // 'mspl'
#if MONOMAC
		TimePitch=0x746d7074, // 'tmpt'
#else
		AUiPodTime=0x6970746d, // 'iptm'
#endif
	}

	public enum AudioTypeEffect { // OSType in AudioComponentDescription
		PeakLimiter=0x6c6d7472, // 'lmtr'
		DynamicsProcessor=0x64636d70, // 'dcmp'
		LowPassFilter=0x6c706173, // 'lpas'
		HighPassFilter=0x68706173, // 'hpas'
		HighShelfFilter=0x68736866, // 'hshf'
		LowShelfFilter=0x6c736866, // 'lshf'
		[Obsoleted (PlatformName.iOS, 7, 0)]
		DCFilter=0x6463666c, // 'dcfl'
		ParametricEQ=0x706d6571, // 'pmeq'
		Delay=0x64656c79, // 'dely'

		[iOS (8, 0)]
		SampleDelay=0x73646c79, // 'sdly'
		Distortion=0x64697374, // 'dist'
		BandPassFilter=0x62706173, // 'bpas'
#if MONOMAC
		GraphicEQ=0x67726571, // 'greq'
		MultiBandCompressor=0x6d636d70, // 'mcmp'
		MatrixReverb=0x6d726576, // 'mrev'
		Pitch=0x70697463, // 'pitc'
		AUFilter=0x66696c74, // 'filt'
		NetSend=0x6e736e64, // 'nsnd'
		RogerBeep=0x726f6772, // 'rogr'
#else
		AUiPodEQ=0x69706571, // 'ipeq'
		Reverb2=0x72766232, // 'rvb2'
#endif
		NBandEq=0x6e626571, // 'nbeq'
	}

	public enum AudioTypeMixer { // OSType in AudioComponentDescription
		MultiChannel=0x6d636d78, // 'mcmx'
		Matrix=0x6d786d78, // 'mxmx'
		Spacial=0x3364656d, // Same as Embedded3D
#if MONOMAC
		Stereo=0x736d7872, // 'smxr'
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'Spacial' instead.")]
		ThreeD=0x33646d78, // '3dmx'
#else
		[Deprecated (PlatformName.iOS, 8, 0, message : "Use 'Spacial' instead.")]
		Embedded3D=0x3364656d, // '3dem'
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
		ScheduledSoundPlayer=0x7373706c, // 'sspl'
		AudioFilePlayer=0x6166706c, // 'afpl'
    }
        
    public enum AudioComponentManufacturerType : uint // OSType in AudioComponentDescription
    {
		Apple = 0x6170706c // little endian 0x6c707061 //'appl'
    }

	[Flags]
	public enum AudioComponentFlag // UInt32 in AudioComponentDescription
	{
		Unsearchable				= 1,
		[iOS (6,0)]
		SandboxSafe					= 2,
		IsV3AudioUnit				= 4,
		RequiresAsyncInstantiation	= 8,
		CanLoadInProcess			= 0x10
	}


#if !XAMCORE_2_0
	[StructLayout(LayoutKind.Sequential, Pack=4)]
	public struct AudioComponentDescriptionNative // AudioComponentDescription in AudioComponent.h
	{
		public AudioComponentType ComponentType;
		public int ComponentSubType;
		public AudioComponentManufacturerType ComponentManufacturer;
		public AudioComponentFlag ComponentFlags;
		public int ComponentFlagsMask;

		public AudioComponentDescriptionNative (AudioComponentDescription other)
		{
			this.ComponentType = other.ComponentType;
			this.ComponentSubType = other.ComponentSubType;
			this.ComponentManufacturer = other.ComponentManufacturer;
			this.ComponentFlags = other.ComponentFlags;
			this.ComponentFlagsMask = other.ComponentFlagsMask;
		}

		public void CopyTo (AudioComponentDescription cd)
		{
			cd.ComponentType = ComponentType;
			cd.ComponentSubType = ComponentSubType;
			cd.ComponentManufacturer = ComponentManufacturer;
			cd.ComponentFlags = ComponentFlags;
			cd.ComponentFlagsMask = ComponentFlagsMask;
		}
	}
#endif

	[StructLayout(LayoutKind.Sequential)]
#if XAMCORE_2_0
	public struct AudioComponentDescription
#else
	public class AudioComponentDescription
#endif
	{
		[MarshalAs(UnmanagedType.U4)] 
		public AudioComponentType ComponentType;
		
		[MarshalAs(UnmanagedType.U4)]
		public int ComponentSubType;
        
		[MarshalAs(UnmanagedType.U4)] 
		public AudioComponentManufacturerType ComponentManufacturer;

		public AudioComponentFlag ComponentFlags;
		public int ComponentFlagsMask;

#if !XAMCORE_2_0
		public AudioComponentDescription () {}
#endif

		internal AudioComponentDescription (AudioComponentType type, int subType)
		{
			ComponentType = type;
			ComponentSubType = subType;
			ComponentManufacturer = AudioComponentManufacturerType.Apple;
			ComponentFlags = (AudioComponentFlag) 0;
			ComponentFlagsMask = 0;
		}

#if !XAMCORE_2_0
		// Because someone made AudioComponentDescription a class
		internal AudioComponentDescription (AudioComponentDescriptionNative native)
		{
			this.ComponentType = native.ComponentType;
			this.ComponentSubType = native.ComponentSubType;
			this.ComponentManufacturer = native.ComponentManufacturer;
			this.ComponentFlags = native.ComponentFlags;
			this.ComponentFlagsMask = native.ComponentFlagsMask;
		}
#endif

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

			switch (ComponentType){
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
