//
// AudioUnit.cs: AudioUnit wrapper class
//
// Authors:
//   AKIHIRO Uehara (u-akihiro@reinforce-lab.com)
//   Marek Safar (marek.safar@gmail.com)
//
// Copyright 2010 Reinforce Lab.
// Copyright 2011-2013 Xamarin Inc
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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using AudioToolbox;
using ObjCRuntime;
using CoreFoundation;
using Foundation;

namespace AudioUnit
{
	public enum AudioUnitStatus { // Implictly cast to OSType
		NoError = 0,
		OK = NoError,
		FileNotFound = -43,
		ParameterError = -50,
		InvalidProperty = -10879,
		InvalidParameter = -10878,
		InvalidElement = -10877,
		NoConnection = -10876,
		FailedInitialization = -10875,
		TooManyFramesToProcess = -10874,
		InvalidFile = -10871,
		FormatNotSupported = -10868,
		Uninitialized = -10867,
		InvalidScope = -10866,
		PropertyNotWritable = -10865,
		CannotDoInCurrentContext = -10863,
		InvalidPropertyValue = -10851,
		PropertyNotInUse = -10850,
		Initialized = -10849,
		InvalidOfflineRender = -10848,
		Unauthorized = -10847,
		[iOS (11,0), Mac (10,13, onlyOn64: true), TV (11,0), NoWatch]
		MidiOutputBufferFull = -66753,
		[iOS (11,3), Mac (10,13,4, onlyOn64: true), TV (11,3), NoWatch]
		InvalidParameterValue = -66743,
		[iOS (11,0), Mac (10,13, onlyOn64: true), TV (11,0), NoWatch]
		ExtensionNotFound = -66744,
	}

	public enum AudioComponentStatus { // Implictly cast to OSType
		OK = 0,
		DuplicateDescription	= -66752,
		UnsupportedType			= -66751,
		TooManyInstances		= -66750,
		InstanceInvalidated		= -66749,
		NotPermitted			= -66748,
		InitializationTimedOut	= -66747,
		InvalidFormat			= -66746,
		[iOS (10,0), Mac (10,12, onlyOn64: true)]
		RenderTimeout			= -66745,
	}

	public enum AudioCodecManufacturer : uint  // Implictly cast to OSType in CoreAudio.framework - CoreAudioTypes.h
	{
		AppleSoftware	= 0x6170706c,	// 'appl'
		AppleHardware	= 0x61706877,	// 'aphw'
	}

	public enum InstrumentType : byte // UInt8 in AUSamplerInstrumentData
	{
		DLSPreset	= 1,
		SF2Preset	= DLSPreset,
		AUPreset	= 2,
		Audiofile	= 3,
		EXS24		= 4
	}

	public class AudioUnitException : Exception {
		static string Lookup (int k)
		{
			switch ((AudioUnitStatus)k)
			{
			case AudioUnitStatus.InvalidProperty:
				return "Invalid Property";
				
			case AudioUnitStatus.InvalidParameter :
				return "Invalid Parameter";
				
			case AudioUnitStatus.InvalidElement :
				return "Invalid Element";
				
			case AudioUnitStatus.NoConnection :
				return "No Connection";
				
			case AudioUnitStatus.FailedInitialization :
				return "Failed Initialization";
				
			case AudioUnitStatus.TooManyFramesToProcess :
				return "Too Many Frames To Process";
				
			case AudioUnitStatus.InvalidFile :
				return "Invalid File";
				
			case AudioUnitStatus.FormatNotSupported :
				return "Format Not Supported";
				
			case AudioUnitStatus.Uninitialized :
				return "Uninitialized";
				
			case AudioUnitStatus.InvalidScope :
				return "Invalid Scope";
				
			case AudioUnitStatus.PropertyNotWritable :
				return "Property Not Writable";
				
			case AudioUnitStatus.CannotDoInCurrentContext :
				return "Cannot Do In Current Context";
				
			case AudioUnitStatus.InvalidPropertyValue :
				return "Invalid Property Value";
				
			case AudioUnitStatus.PropertyNotInUse :
				return "Property Not In Use";
				
			case AudioUnitStatus.Initialized :
				return "Initialized";
				
			case AudioUnitStatus.InvalidOfflineRender :
				return "Invalid Offline Render";
				
			case AudioUnitStatus.Unauthorized :
				return "Unauthorized";
				
			}
			return String.Format ("Unknown error code: 0x{0:x}", k);
		}
		
		internal AudioUnitException (int k) : base (Lookup (k))
		{
		}
	}

#if !COREBUILD
	public delegate AudioUnitStatus RenderDelegate (AudioUnitRenderActionFlags actionFlags, AudioTimeStamp timeStamp, uint busNumber, uint numberFrames, AudioBuffers data);
	public delegate AudioUnitStatus InputDelegate (AudioUnitRenderActionFlags actionFlags, AudioTimeStamp timeStamp, uint busNumber, uint numberFrames, AudioUnit audioUnit);

	delegate AudioUnitStatus CallbackShared (IntPtr /* void* */ clientData, ref AudioUnitRenderActionFlags /* AudioUnitRenderActionFlags* */ actionFlags, ref AudioTimeStamp /* AudioTimeStamp* */ timeStamp, uint /* UInt32 */ busNumber, uint /* UInt32 */ numberFrames, IntPtr /* AudioBufferList* */ data);
#endif // !COREBUILD

	[StructLayout(LayoutKind.Sequential)]
	struct AURenderCallbackStruct
	{
		public Delegate Proc;
		public IntPtr ProcRefCon; 
	}

	[StructLayout(LayoutKind.Sequential)]
	struct AudioUnitConnection
	{
		public IntPtr SourceAudioUnit;
		public uint /* UInt32 */ SourceOutputNumber;
		public uint /* UInt32 */ DestInputNumber;
	}

	public class SamplerInstrumentData
	{
#if !COREBUILD
		public const byte DefaultPercussionBankMSB	= 0x78;
		public const byte DefaultMelodicBankMSB	= 0x79;
		public const byte DefaultBankLSB = 0x00;

		public SamplerInstrumentData (CFUrl fileUrl, InstrumentType instrumentType)
		{
			if (fileUrl == null)
				throw new ArgumentNullException ("fileUrl");

			this.FileUrl = fileUrl;
			this.InstrumentType = instrumentType;
		}

		public CFUrl FileUrl { get; private set; }
		public InstrumentType InstrumentType { get; private set; }
		public byte BankMSB { get; set; }
		public byte BankLSB { get; set; }
		public byte PresetID { get; set; }

		internal AUSamplerInstrumentData ToStruct ()
		{
			var data = new AUSamplerInstrumentData ();
			data.FileUrl = FileUrl.Handle;
			data.InstrumentType = InstrumentType;
			data.BankMSB = BankMSB;
			data.BankLSB = BankLSB;
			data.PresetID = PresetID;
			return data;
		}
#endif // !COREBUILD
	}

	[StructLayout(LayoutKind.Sequential)]
	struct AUSamplerInstrumentData
	{
		public IntPtr FileUrl;
		public InstrumentType InstrumentType;
		public byte BankMSB;
		public byte BankLSB;
		public byte PresetID;
	}

	[StructLayout(LayoutKind.Sequential)]
	unsafe struct AudioUnitParameterInfoNative // AudioUnitParameterInfo in Obj-C
	{
		fixed byte /* char[52] */ name[52]; // unused
		public IntPtr /* CFStringRef */ UnitName;
		public AudioUnitClumpID /* UInt32 */ ClumpID;
		public IntPtr /* CFStringRef */ NameString;

		public AudioUnitParameterUnit /* AudioUnitParameterUnit */ Unit;
		public float /* AudioUnitParameterValue = Float32 */ MinValue;
		public float /* AudioUnitParameterValue = Float32 */ MaxValue;
		public float /* AudioUnitParameterValue = Float32 */ DefaultValue;
		public AudioUnitParameterFlag /* UInt32 */ Flags;
	}

	public class AudioUnitParameterInfo
	{
#if !COREBUILD
		public string UnitName { get; private set; }
		public AudioUnitClumpID ClumpID { get; private set; }
		public string Name { get; private set; }
		public AudioUnitParameterUnit Unit { get; private set; }
		public float MinValue { get; private set; }
		public float MaxValue { get; private set; }
		public float DefaultValue { get; private set; }
		public AudioUnitParameterFlag Flags { get; private set; }
		public AudioUnitParameterType Type { get; private set; }

		internal static AudioUnitParameterInfo Create (AudioUnitParameterInfoNative native, AudioUnitParameterType type)
		{
			var info = new AudioUnitParameterInfo ();

			// Remove obj-c noise
			info.Flags = native.Flags & ~(AudioUnitParameterFlag.HasCFNameString | AudioUnitParameterFlag.CFNameRelease);
			info.Unit = native.Unit;
			info.MinValue = native.MinValue;
			info.MaxValue = native.MaxValue;
			info.DefaultValue = native.DefaultValue;
			info.ClumpID = native.ClumpID;
			info.Type = type;

			if ((native.Flags & AudioUnitParameterFlag.HasCFNameString) != 0) {
				info.Name = CFString.FetchString (native.NameString);

				if ((native.Flags & AudioUnitParameterFlag.CFNameRelease) != 0)
					CFObject.CFRelease (native.NameString);
			}

			if (native.Unit == AudioUnitParameterUnit.CustomUnit) {
				info.UnitName = CFString.FetchString (native.UnitName);
			}

			return info;
		}
#endif // !COREBUILD
	}

	public enum AudioUnitParameterUnit // UInt32 AudioUnitParameterUnit
	{
		Generic				= 0,
		Indexed				= 1,
		Boolean				= 2,
		Percent				= 3,
		Seconds				= 4,
		SampleFrames		= 5,
		Phase				= 6,
		Rate				= 7,
		Hertz				= 8,
		Cents				= 9,
		RelativeSemiTones	= 10,
		MIDINoteNumber		= 11,
		MIDIController		= 12,
		Decibels			= 13,
		LinearGain			= 14,
		Degrees				= 15,
		EqualPowerCrossfade = 16,
		MixerFaderCurve1	= 17,
		Pan					= 18,
		Meters				= 19,
		AbsoluteCents		= 20,
		Octaves				= 21,
		BPM					= 22,
		Beats               = 23,
		Milliseconds		= 24,
		Ratio				= 25,
		CustomUnit			= 26
	}

	[Flags]
	public enum AudioUnitParameterFlag : uint // UInt32 in AudioUnitParameterInfo
	{
		CFNameRelease		= (1 << 4),

		[iOS (8,0)]
		OmitFromPresets		= (1 << 13),
		PlotHistory			= (1 << 14),
		MeterReadOnly		= (1 << 15),
	
		// bit positions 18,17,16 are set aside for display scales. bit 19 is reserved.
		DisplayMask			= (7 << 16) | (1 << 22),
		DisplaySquareRoot	= (1 << 16),
		DisplaySquared		= (2 << 16),
		DisplayCubed		= (3 << 16),
		DisplayCubeRoot		= (4 << 16),
		DisplayExponential	= (5 << 16),

		HasClump	 		= (1 << 20),
		ValuesHaveStrings	= (1 << 21),
	
		DisplayLogarithmic 	= (1 << 22),
	
		IsHighResolution 	= (1 << 23),
		NonRealTime 		= (1 << 24),
		CanRamp 			= (1 << 25),
		ExpertMode 			= (1 << 26),
		HasCFNameString 	= (1 << 27),
		IsGlobalMeta 		= (1 << 28),
		IsElementMeta		= (1 << 29),
		IsReadable			= (1 << 30),
		IsWritable			= ((uint)1 << 31)
	}

	public enum AudioUnitClumpID // UInt32 in AudioUnitParameterInfo
	{
		System = 0
	}

	public enum AUParameterEventType : uint
	{
		Immediate = 1,
		Ramped = 2,
	}

	[StructLayout (LayoutKind.Sequential)]
	public struct AudioUnitParameterEvent
	{
		public uint Scope;
		public uint Element;
		public uint Parameter;
		public AUParameterEventType EventType;

		[StructLayout (LayoutKind.Explicit)]
		public struct EventValuesStruct
		{
			[StructLayout (LayoutKind.Sequential)]
			public struct RampStruct
			{
				public int StartBufferOffset;
				public uint DurationInFrames;
				public float StartValue;
				public float EndValue;
			}


			[FieldOffset (0)]
			public RampStruct Ramp;

			[StructLayout (LayoutKind.Sequential)]
			public struct ImmediateStruct
			{
				public uint BufferOffset;
				public float Value;
			}

			[FieldOffset (0)]
			public ImmediateStruct Immediate;
		}

		public EventValuesStruct EventValues;
	}

	public class AudioUnit : IDisposable, ObjCRuntime.INativeObject
	{
#pragma warning disable 649 // Field 'AudioUnit.handle' is never assigned to, and will always have its default value
		internal IntPtr handle;
#pragma warning restore 649
		public IntPtr Handle {
			get {
				return handle;
			}
		}

#if COREBUILD
		public void Dispose () { /* FAKE DURING COREBUILD */ }
#else
		static readonly CallbackShared CreateRenderCallback = RenderCallbackImpl;
		static readonly CallbackShared CreateInputCallback = InputCallbackImpl;

		GCHandle gcHandle;
		bool _isPlaying;

		Dictionary<uint, RenderDelegate> renderer;
		Dictionary<uint, InputDelegate> inputs;

		internal AudioUnit (IntPtr ptr)
		{
			handle = ptr;
			gcHandle = GCHandle.Alloc(this);
		}
		
		public AudioUnit (AudioComponent component)
		{
			if (component == null)
				throw new ArgumentNullException ("component");
			if (component.Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("component");
			
			int err = AudioComponentInstanceNew (component.handle, out handle);
			if (err != 0)
				throw new AudioUnitException (err);
			
			gcHandle = GCHandle.Alloc(this);

#if !XAMCORE_2_0
#pragma warning disable 612
			BrokenSetRender ();
#pragma warning restore 612
#endif
		}

#if !XAMCORE_2_0
		[Obsolete] 		// Broken only few AudioComponent types support this property
		void BrokenSetRender ()
		{
			var callbackStruct = new AURenderCallbackStrct();
			callbackStruct.inputProc = renderCallback; // setting callback function            
			callbackStruct.inputProcRefCon = GCHandle.ToIntPtr(gcHandle); // a pointer that passed to the renderCallback (IntPtr inRefCon) 
			AudioUnitSetProperty(handle,
						   AudioUnitPropertyIDType.SetRenderCallback,
						   AudioUnitScopeType.Input,
						   0, // 0 == speaker                
						   callbackStruct,
						   (uint)Marshal.SizeOf(callbackStruct));
		}
#endif

		public AudioComponent Component {
			get {
				return new AudioComponent (AudioComponentInstanceGetComponent (handle));
			}
		}

#if !XAMCORE_2_0
		[Obsolete ("Use SetRenderCallback")]
#pragma warning disable 612 // AudioUnitEventArgs is obsolete
		public event EventHandler<AudioUnitEventArgs> RenderCallback;
#pragma warning restore 612
#endif

		public bool IsPlaying { get { return _isPlaying; } }
		
#if !XAMCORE_2_0
		[Obsolete]
		// callback funtion should be static method and be attatched a MonoPInvokeCallback attribute.        
		[MonoPInvokeCallback (typeof (AURenderCallback))]
		static int renderCallback(IntPtr inRefCon, ref AudioUnitRenderActionFlags _ioActionFlags,
					  ref AudioTimeStamp _inTimeStamp,
					  int _inBusNumber,
					  int _inNumberFrames,
					  AudioBufferList _ioData)
		{
			// getting audiounit instance
			var handler = GCHandle.FromIntPtr(inRefCon);
			var inst = (AudioUnit)handler.Target;
			
			// evoke event handler with an argument
			if (inst.RenderCallback != null)  { 
				var args = new AudioUnitEventArgs(
					_ioActionFlags,
					_inTimeStamp,
					_inBusNumber,
					_inNumberFrames,
					_ioData);
				inst.RenderCallback(inst, args);
			}
			
			return 0; // noerror
		}
#endif

#if !XAMCORE_3_0
		[Obsolete ("Use 'SetFormat' instead as it has the ability of returning a status code.")]
		public void SetAudioFormat(AudioToolbox.AudioStreamBasicDescription audioFormat, AudioUnitScopeType scope, uint audioUnitElement = 0)
		{
			var err = AudioUnitSetProperty(handle,
						       AudioUnitPropertyIDType.StreamFormat,
						       scope,
						       audioUnitElement, 
						       ref audioFormat,
						       (uint)Marshal.SizeOf(audioFormat));
			if (err != 0)
				throw new AudioUnitException (err);
		}
#endif

		public AudioUnitStatus SetFormat(AudioToolbox.AudioStreamBasicDescription audioFormat, AudioUnitScopeType scope, uint audioUnitElement = 0)
		{
			return (AudioUnitStatus) AudioUnitSetProperty(handle,
						       AudioUnitPropertyIDType.StreamFormat,
						       scope,
						       audioUnitElement, 
						       ref audioFormat,
						       (uint)Marshal.SizeOf(audioFormat));
		}

		public uint GetCurrentDevice (AudioUnitScopeType scope, uint audioUnitElement = 0)
		{
			uint device = 0;
			int size = Marshal.SizeOf (typeof (uint));
			var err = AudioUnitGetProperty(handle,
						AudioUnitPropertyIDType.CurrentDevice,
						scope,
						audioUnitElement,
						ref device,
						ref size);
			if (err != 0)
				throw new AudioUnitException ((int) err);
			return device;
		}

#if !XAMCORE_3_0 || MONOMAC
#if !MONOMAC
		[Obsolete ("This API is not available on iOS.")]
#endif
		public static uint GetCurrentInputDevice ()
		{
#if MONOMAC
			// We need to replace AudioHardwareGetProperty since it has been deprecated since OS X 10.6 and iOS 2.0
			// Replacing with the following implementation recommended in the following doc
			// See Listing 4  New - Getting the default input device.
			// https://developer.apple.com/library/mac/technotes/tn2223/_index.html

			uint inputDevice;
			uint size = (uint) Marshal.SizeOf (typeof (uint));
			var theAddress = new AudioObjectPropertyAddress (
				AudioObjectPropertySelector.DefaultInputDevice,
				AudioObjectPropertyScope.Global,
				AudioObjectPropertyElement.Master);
			uint inQualifierDataSize = 0;
			IntPtr inQualifierData = IntPtr.Zero;

			var err = AudioObjectGetPropertyData (1, ref theAddress, ref inQualifierDataSize, ref inQualifierData, ref size, out inputDevice);

			if (err != 0)
				throw new AudioUnitException ((int) err);
			return inputDevice;
#elif !XAMCORE_3_0
			return 0;
#endif
		}
#endif
		public AudioUnitStatus SetCurrentDevice (uint inputDevice, AudioUnitScopeType scope, uint audioUnitElement = 0)
		{
			return AudioUnitSetProperty(handle,
						AudioUnitPropertyIDType.CurrentDevice,
						scope,
						audioUnitElement,
						ref inputDevice,
						(uint)Marshal.SizeOf(inputDevice));
		}

		public AudioStreamBasicDescription GetAudioFormat(AudioUnitScopeType scope, uint audioUnitElement = 0)
		{
			var audioFormat = new AudioStreamBasicDescription();
			uint size = (uint) Marshal.SizeOf (audioFormat);

			var err = AudioUnitGetProperty(handle,
						       AudioUnitPropertyIDType.StreamFormat,
						       scope,
						       audioUnitElement,
						       ref audioFormat,
						       ref size);
			if (err != 0)
				throw new AudioUnitException ((int) err);
			
			return audioFormat;
		}

		public ClassInfoDictionary GetClassInfo (AudioUnitScopeType scope = AudioUnitScopeType.Global, uint audioUnitElement = 0)
		{
			IntPtr ptr = new IntPtr ();
			int size = Marshal.SizeOf (typeof (IntPtr));
			var res = AudioUnitGetProperty (handle, AudioUnitPropertyIDType.ClassInfo, scope, audioUnitElement,
				ref ptr, ref size);

			if (res != 0)
				return null;

			return new ClassInfoDictionary (new NSDictionary (ptr, true));
		}

		public AudioUnitStatus SetClassInfo (ClassInfoDictionary preset, AudioUnitScopeType scope = AudioUnitScopeType.Global, uint audioUnitElement = 0)
		{
			var ptr = preset.Dictionary.Handle;
			return AudioUnitSetProperty (handle, AudioUnitPropertyIDType.ClassInfo, scope, audioUnitElement,
				ref ptr, Marshal.SizeOf (typeof (IntPtr)));
		}

		public unsafe AudioUnitParameterInfo[] GetParameterList (AudioUnitScopeType scope = AudioUnitScopeType.Global, uint audioUnitElement = 0)
		{
			uint size;
			bool writable;
			if (AudioUnitGetPropertyInfo (handle, AudioUnitPropertyIDType.ParameterList, scope, audioUnitElement, out size, out writable) != 0)
				return null;

			// Array of AudioUnitParameterID = UInt32
			var data = new uint [size / sizeof (uint)];
			fixed (uint* ptr = data) {
				if (AudioUnitGetProperty (handle, AudioUnitPropertyIDType.ParameterList, scope, audioUnitElement, ptr, ref size) != 0)
					return null;
			}

			var info = new AudioUnitParameterInfo [data.Length];
			size = (uint) Marshal.SizeOf (typeof (AudioUnitParameterInfoNative));

			for (int i = 0; i < data.Length; ++i) {
				var native = new AudioUnitParameterInfoNative ();
				if (AudioUnitGetProperty (handle, AudioUnitPropertyIDType.ParameterInfo, scope, data [i], ref native, ref size) != 0)
					return null;

				info [i] = AudioUnitParameterInfo.Create (native, (AudioUnitParameterType) data [i]);
			}

			return info;
		}

		public AudioUnitStatus LoadInstrument (SamplerInstrumentData instrumentData, AudioUnitScopeType scope = AudioUnitScopeType.Global, uint audioUnitElement = 0)
		{
			if (instrumentData == null)
				throw new ArgumentNullException ("instrumentData");

			var data = instrumentData.ToStruct ();
			return AudioUnitSetProperty (handle, AudioUnitPropertyIDType.LoadInstrument, scope, audioUnitElement, 
				ref data, Marshal.SizeOf (typeof (AUSamplerInstrumentData)));
		}

		public AudioUnitStatus MakeConnection (AudioUnit sourceAudioUnit, uint sourceOutputNumber, uint destInputNumber)
		{
			var auc = new AudioUnitConnection {
				SourceAudioUnit = sourceAudioUnit == null ? IntPtr.Zero : sourceAudioUnit.handle,
				SourceOutputNumber = sourceOutputNumber,
				DestInputNumber = destInputNumber
			};

			return AudioUnitSetProperty (handle, AudioUnitPropertyIDType.MakeConnection, AudioUnitScopeType.Input, 0, ref auc, Marshal.SizeOf (typeof (AudioUnitConnection)));
		}

		public AudioUnitStatus SetEnableIO (bool enableIO, AudioUnitScopeType scope, uint audioUnitElement = 0)
		{                         
			// EnableIO: UInt32          
			uint flag = enableIO ? (uint)1 : 0;
			return AudioUnitSetProperty (handle, AudioUnitPropertyIDType.EnableIO, scope, audioUnitElement, ref flag, sizeof (uint));
		}

		public AudioUnitStatus SetMaximumFramesPerSlice (uint value, AudioUnitScopeType scope, uint audioUnitElement = 0)
		{
			// MaximumFramesPerSlice: UInt32
			return AudioUnitSetProperty (handle, AudioUnitPropertyIDType.MaximumFramesPerSlice, scope, audioUnitElement, ref value, sizeof (uint));
		}

		public uint GetMaximumFramesPerSlice (AudioUnitScopeType scope = AudioUnitScopeType.Global, uint audioUnitElement = 0)
		{
			// MaximumFramesPerSlice: UInt32
			uint value = 0;
			uint size = sizeof (uint);
			var res = AudioUnitGetProperty (handle, AudioUnitPropertyIDType.MaximumFramesPerSlice, scope,
				audioUnitElement, ref value, ref size);

			if (res != 0)
				throw new AudioUnitException ((int) res);

			return value;
		}

		public AudioUnitStatus SetElementCount (AudioUnitScopeType scope, uint count)
		{
			// ElementCount: UInt32
			return AudioUnitSetProperty (handle, AudioUnitPropertyIDType.ElementCount, scope, 0, ref count, sizeof (uint));
		}

		public uint GetElementCount (AudioUnitScopeType scope)
		{
			// ElementCount: UInt32
			uint value = 0;
			uint size = sizeof (uint);
			var res = AudioUnitGetProperty (handle, AudioUnitPropertyIDType.ElementCount, scope,
				0, ref value, ref size);

			if (res != 0)
				throw new AudioUnitException ((int) res);

			return value;
		}

		public AudioUnitStatus SetSampleRate (double sampleRate, AudioUnitScopeType scope = AudioUnitScopeType.Output, uint audioUnitElement = 0)
		{
			// ElementCount: Float64
			return AudioUnitSetProperty (handle, AudioUnitPropertyIDType.SampleRate, scope, 0, ref sampleRate, sizeof (double));
		}

		public AudioUnitStatus MusicDeviceMIDIEvent (uint status, uint data1, uint data2, uint offsetSampleFrame = 0)
		{
			return MusicDeviceMIDIEvent (handle, status, data1, data2, offsetSampleFrame);
		}

		public AudioUnitStatus SetLatency (double latency)
		{
			// ElementCount: Float64, AudioUnitScopeType.Global is the only valid scope for Latency.
			return AudioUnitSetProperty (handle, AudioUnitPropertyIDType.Latency, AudioUnitScopeType.Global, 0, ref latency, sizeof (double));
		}

		[DllImport (Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioUnitGetProperty (IntPtr inUnit, AudioUnitPropertyIDType inID, AudioUnitScopeType inScope, uint inElement, ref double outData, ref uint ioDataSize);

		public double GetLatency ()
		{
			uint size = sizeof (double);
			double latency = 0;
			var err = AudioUnitGetProperty (handle, AudioUnitPropertyIDType.Latency, AudioUnitScopeType.Global, 0, ref latency, ref size);
			if (err != 0)
				throw new AudioUnitException ((int) err);
			return latency;
		}

		#region SetRenderCallback

		public AudioUnitStatus SetRenderCallback (RenderDelegate renderDelegate, AudioUnitScopeType scope = AudioUnitScopeType.Global, uint audioUnitElement = 0)
		{
			if (renderer == null)
				Interlocked.CompareExchange (ref renderer, new Dictionary<uint, RenderDelegate> (), null);

			renderer [audioUnitElement] = renderDelegate;

			var cb = new AURenderCallbackStruct ();
			cb.Proc = CreateRenderCallback;
			cb.ProcRefCon = GCHandle.ToIntPtr (gcHandle);
			return AudioUnitSetProperty (handle, AudioUnitPropertyIDType.SetRenderCallback, scope, audioUnitElement, ref cb, Marshal.SizeOf (cb));
		}

		[MonoPInvokeCallback (typeof (CallbackShared))]
		static AudioUnitStatus RenderCallbackImpl (IntPtr clientData, ref AudioUnitRenderActionFlags actionFlags, ref AudioTimeStamp timeStamp, uint busNumber, uint numberFrames, IntPtr data)
		{
			GCHandle gch = GCHandle.FromIntPtr (clientData);
			var au = (AudioUnit) gch.Target;
			var renderer = au.renderer;

			if (renderer == null)
				return AudioUnitStatus.Uninitialized;

			RenderDelegate render;
			if (!renderer.TryGetValue (busNumber, out render))
				return AudioUnitStatus.Uninitialized;

			using (var buffers = new AudioBuffers (data)) {
				return render (actionFlags, timeStamp, busNumber, numberFrames, buffers);
			}
		}

		#endregion

		#region SetInputCallback

		public AudioUnitStatus SetInputCallback (InputDelegate inputDelegate, AudioUnitScopeType scope = AudioUnitScopeType.Global, uint audioUnitElement = 0)
		{
			if (inputs == null)
				Interlocked.CompareExchange (ref inputs, new Dictionary<uint, InputDelegate> (), null);

			inputs [audioUnitElement] = inputDelegate;

			var cb = new AURenderCallbackStruct ();
			cb.Proc = CreateInputCallback;
			cb.ProcRefCon = GCHandle.ToIntPtr (gcHandle);
			return AudioUnitSetProperty (handle, AudioUnitPropertyIDType.SetInputCallback, scope, audioUnitElement, ref cb, Marshal.SizeOf (cb));
		}

		[MonoPInvokeCallback (typeof (CallbackShared))]
		static AudioUnitStatus InputCallbackImpl (IntPtr clientData, ref AudioUnitRenderActionFlags actionFlags, ref AudioTimeStamp timeStamp, uint busNumber, uint numberFrames, IntPtr data)
		{
			GCHandle gch = GCHandle.FromIntPtr (clientData);
			var au = (AudioUnit) gch.Target;
			var inputs = au.inputs;

			if (inputs == null)
				return AudioUnitStatus.Uninitialized;

			InputDelegate input;
			if (!inputs.TryGetValue (busNumber, out input))
				return AudioUnitStatus.Uninitialized;

			return input (actionFlags, timeStamp, busNumber, numberFrames, au);
		}

		#endregion

#if !MONOMAC
		[iOS (7,0)]
		[DllImport (Constants.AudioUnitLibrary)]
#if XAMCORE_2_0
		static extern AudioComponentStatus AudioOutputUnitPublish (AudioComponentDescription inDesc, IntPtr /* CFStringRef */ inName, uint /* UInt32 */ inVersion, IntPtr /* AudioUnit */ inOutputUnit);
#else
		static extern AudioComponentStatus AudioOutputUnitPublish (ref AudioComponentDescription inDesc, IntPtr inName, uint inVersion, IntPtr inOutputUnit);
#endif

		[iOS (7,0)]
		public AudioComponentStatus AudioOutputUnitPublish (AudioComponentDescription description, string name, uint version = 1)
		{
#if !XAMCORE_2_0
			if (description == null)
				throw new ArgumentNullException ("description");
#endif

			if (name == null)
				throw new ArgumentNullException ("name");
				
			using (CFString n = name) {
#if XAMCORE_2_0
				return AudioOutputUnitPublish (description, n.Handle, version, handle);
#else
				return AudioOutputUnitPublish (ref description, n.Handle, version, handle);
#endif
			}
		}

		[iOS (7,0)]
		[DllImport (Constants.AudioUnitLibrary)]
		static extern IntPtr AudioOutputUnitGetHostIcon (IntPtr /* AudioUnit */ au, float /* float */ desiredPointSize);

		[iOS (7,0)]
		public UIKit.UIImage GetHostIcon (float desiredPointSize)
		{
			return new UIKit.UIImage (AudioOutputUnitGetHostIcon (handle, desiredPointSize));
		}
#endif

		// TODO: return AudioUnitStatus
		public int Initialize ()
		{
			return AudioUnitInitialize(handle);
		}

		// TODO: return AudioUnitStatus
		public int Uninitialize ()
		{
			return AudioUnitUninitialize (handle);
		}

		public void Start()
		{
			if (! _isPlaying) {
				AudioOutputUnitStart(handle);
				_isPlaying = true;
			}
		}
		
		public void Stop()
		{
			if (_isPlaying) {
				AudioOutputUnitStop(handle);
				_isPlaying = false;
			}
		}

		#region Render

		public AudioUnitStatus Render (ref AudioUnitRenderActionFlags actionFlags, AudioTimeStamp timeStamp, uint busNumber, uint numberFrames, AudioBuffers data)
		{
			if ((IntPtr)data == IntPtr.Zero)
				throw new ArgumentNullException ("data");
			return AudioUnitRender (handle, ref actionFlags, ref timeStamp, busNumber, numberFrames, (IntPtr) data);
		}

#if !XAMCORE_2_0
		[Obsolete]
		public void Render(AudioUnitRenderActionFlags flags,
				   AudioTimeStamp timeStamp,
				   int outputBusnumber,
				   int numberFrames, AudioBufferList data)
		{
			if (data == null)
				throw new ArgumentNullException ("data");
			int err = AudioUnitRender(handle,
						  ref flags,
						  ref timeStamp,
						  outputBusnumber,
						  numberFrames,
						  data);
			if (err != 0)
				throw new AudioUnitException (err);
		}

		[Obsolete]
		public AudioUnitStatus TryRender(AudioUnitRenderActionFlags flags,
						AudioTimeStamp timeStamp,
						int outputBusnumber,
						int numberFrames, AudioBufferList data)
		{
			return (AudioUnitStatus) AudioUnitRender(handle,
								ref flags,
								ref timeStamp,
								outputBusnumber,
								numberFrames,
								data);
		}
#endif

		#endregion

		public AudioUnitStatus SetParameter (AudioUnitParameterType type, float value, AudioUnitScopeType scope, uint audioUnitElement = 0)
		{
			return AudioUnitSetParameter (handle, type, scope, audioUnitElement, value, 0);
		}
		
		public AudioUnitStatus ScheduleParameter (AudioUnitParameterEvent inParameterEvent, uint inNumParamEvents)
		{
			return AudioUnitScheduleParameters (handle, inParameterEvent, inNumParamEvents);
		}

		public void Dispose()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}
		
		[DllImport(Constants.AudioUnitLibrary)]
		static extern int AudioComponentInstanceDispose(IntPtr inInstance);

#if XAMCORE_2_0
		protected virtual void Dispose (bool disposing)
#else
		public void Dispose (bool disposing)
#endif
		{
			if (handle != IntPtr.Zero){
				Stop ();
				AudioUnitUninitialize (handle);
				AudioComponentInstanceDispose (handle);
				gcHandle.Free();
				handle = IntPtr.Zero;
			}
		}

#if !XAMCORE_2_0
		[Obsolete]
		internal delegate int AURenderCallback(IntPtr inRefCon,
					      ref AudioUnitRenderActionFlags ioActionFlags,
					      ref AudioTimeStamp inTimeStamp,
					      int inBusNumber,
					      int inNumberFrames,
					      AudioBufferList ioData);
		
		[Obsolete]
		[StructLayout(LayoutKind.Sequential)]
		class AURenderCallbackStrct {
			public AURenderCallback inputProc;
			public IntPtr inputProcRefCon;
			
			public AURenderCallbackStrct() { }
		}
#endif

		[DllImport(Constants.AudioUnitLibrary, EntryPoint = "AudioComponentInstanceNew")]
		static extern int AudioComponentInstanceNew(IntPtr inComponent, out IntPtr inDesc);

		[DllImport(Constants.AudioUnitLibrary)]
		static extern IntPtr AudioComponentInstanceGetComponent (IntPtr inComponent);
		
		[DllImport(Constants.AudioUnitLibrary)]
		static extern int AudioUnitInitialize(IntPtr inUnit);
		
		[DllImport(Constants.AudioUnitLibrary)]
		static extern int AudioUnitUninitialize(IntPtr inUnit);

		[DllImport(Constants.AudioUnitLibrary)]
		static extern int AudioOutputUnitStart(IntPtr ci);

		[DllImport(Constants.AudioUnitLibrary)]
		static extern int AudioOutputUnitStop(IntPtr ci);

#if !XAMCORE_2_0
		[Obsolete]
		[DllImport(Constants.AudioUnitLibrary)]
		static extern int AudioUnitRender(IntPtr inUnit,
						  ref AudioUnitRenderActionFlags ioActionFlags,
						  ref AudioTimeStamp inTimeStamp,
						  int inOutputBusNumber,
						  int inNumberFrames,
						  AudioBufferList ioData);
#endif

		[DllImport(Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioUnitRender(IntPtr inUnit, ref AudioUnitRenderActionFlags ioActionFlags, ref AudioTimeStamp inTimeStamp,
						  uint inOutputBusNumber, uint inNumberFrames, IntPtr ioData);

#if !XAMCORE_2_0
		[Obsolete]
		[DllImport(Constants.AudioUnitLibrary)]
		static extern int AudioUnitSetProperty(IntPtr inUnit,
						       [MarshalAs(UnmanagedType.U4)] AudioUnitPropertyIDType inID,
						       [MarshalAs(UnmanagedType.U4)] AudioUnitScopeType inScope,
						       [MarshalAs(UnmanagedType.U4)] uint inElement,
						       AURenderCallbackStrct inData,
						       uint inDataSize);
#endif

		[DllImport(Constants.AudioUnitLibrary)]
		static extern int AudioUnitSetProperty (IntPtr inUnit, AudioUnitPropertyIDType inID, AudioUnitScopeType inScope, uint inElement,
						       ref AudioStreamBasicDescription inData,
						       uint inDataSize);
        
		[DllImport(Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioUnitSetProperty (IntPtr inUnit, AudioUnitPropertyIDType inID, AudioUnitScopeType inScope, uint inElement,
						       ref uint inData, uint inDataSize);

		[DllImport(Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioUnitSetProperty (IntPtr inUnit, AudioUnitPropertyIDType inID, AudioUnitScopeType inScope, uint inElement,
						       ref double inData, uint inDataSize);

		[DllImport(Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioUnitSetProperty (IntPtr inUnit, AudioUnitPropertyIDType inID, AudioUnitScopeType inScope, uint inElement,
						       ref IntPtr inData, int inDataSize);

		[DllImport(Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioUnitSetProperty (IntPtr inUnit, AudioUnitPropertyIDType inID, AudioUnitScopeType inScope, uint inElement,
						       ref AURenderCallbackStruct inData, int inDataSize);

		[DllImport(Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioUnitSetProperty (IntPtr inUnit, AudioUnitPropertyIDType inID, AudioUnitScopeType inScope, uint inElement,
						       ref AudioUnitConnection inData, int inDataSize);

		[DllImport(Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioUnitSetProperty (IntPtr inUnit, AudioUnitPropertyIDType inID, AudioUnitScopeType inScope, uint inElement,
						       ref AUSamplerInstrumentData inData, int inDataSize);

		[DllImport(Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioUnitGetProperty (IntPtr inUnit, AudioUnitPropertyIDType inID, AudioUnitScopeType inScope, uint inElement,
						       ref AudioStreamBasicDescription outData,
						       ref uint ioDataSize);

		[DllImport(Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioUnitGetProperty (IntPtr inUnit, AudioUnitPropertyIDType inID, AudioUnitScopeType inScope, uint inElement,
							   ref IntPtr outData,
						       ref int ioDataSize);

		[DllImport(Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioUnitGetProperty (IntPtr inUnit, AudioUnitPropertyIDType inID, AudioUnitScopeType inScope, uint inElement,
						ref uint outData,
						ref int ioDataSize);

		[DllImport(Constants.AudioUnitLibrary)]
		static extern unsafe AudioUnitStatus AudioUnitGetProperty (IntPtr inUnit, AudioUnitPropertyIDType inID, AudioUnitScopeType inScope, uint inElement,
						       uint* outData,
						       ref uint ioDataSize);

		[DllImport(Constants.AudioUnitLibrary)]
		static extern unsafe AudioUnitStatus AudioUnitGetProperty (IntPtr inUnit, AudioUnitPropertyIDType inID, AudioUnitScopeType inScope, uint inElement,
						       ref AudioUnitParameterInfoNative outData,
						       ref uint ioDataSize);

		[DllImport(Constants.AudioUnitLibrary)]
		static extern int AudioUnitGetProperty(IntPtr inUnit,
						       [MarshalAs(UnmanagedType.U4)] AudioUnitPropertyIDType inID,
						       [MarshalAs(UnmanagedType.U4)] AudioUnitScopeType inScope,
						       [MarshalAs(UnmanagedType.U4)] uint inElement,
						       ref uint flag,
						       ref uint ioDataSize
			);


		[DllImport (Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioUnitGetPropertyInfo (IntPtr inUnit, AudioUnitPropertyIDType inID, AudioUnitScopeType inScope,  uint inElement,
								out uint outDataSize, [MarshalAs (UnmanagedType.I1)] out bool outWritable);

		[DllImport (Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioUnitSetParameter (IntPtr inUnit, AudioUnitParameterType inID, AudioUnitScopeType inScope,
			uint inElement, float inValue, uint inBufferOffsetInFrames);

		[DllImport (Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioUnitScheduleParameters (IntPtr inUnit, AudioUnitParameterEvent inParameterEvent, uint inNumParamEvents);

#if MONOMAC
		[DllImport (Constants.AudioUnitLibrary)]
		static extern int AudioObjectGetPropertyData (
			uint inObjectID,
			ref AudioObjectPropertyAddress inAddress,
			ref uint inQualifierDataSize,
			ref IntPtr inQualifierData,
			ref uint ioDataSize,
			out uint outData
		);
#endif // MONOMAC
		[DllImport (Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus MusicDeviceMIDIEvent (IntPtr /* MusicDeviceComponent = void* */ inUnit, uint /* UInt32 */ inStatus, uint /* UInt32 */ inData1, uint /* UInt32 */ inData2, uint /* UInt32 */ inOffsetSampleFrame);

		[DllImport (Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioUnitSetProperty (IntPtr inUnit, AudioUnitPropertyIDType inID, AudioUnitScopeType inScope, uint inElement,
			ref AUScheduledAudioFileRegion.ScheduledAudioFileRegion inData, int inDataSize);

		public AudioUnitStatus SetScheduledFileRegion (AUScheduledAudioFileRegion region)
		{
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("AudioUnit");

			if (region == null)
				throw new ArgumentNullException (nameof (region));

			var safr = region.GetAudioFileRegion ();
			return AudioUnitSetProperty (handle, AudioUnitPropertyIDType.ScheduledFileRegion, AudioUnitScopeType.Global, 0, ref safr, Marshal.SizeOf (safr));
		}

		[DllImport (Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioUnitSetProperty (IntPtr inUnit, AudioUnitPropertyIDType inID, AudioUnitScopeType inScope, uint inElement,
			ref AudioTimeStamp inData, int inDataSize);

		public AudioUnitStatus SetScheduleStartTimeStamp (AudioTimeStamp timeStamp)
		{
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("AudioUnit");

			return AudioUnitSetProperty (handle, AudioUnitPropertyIDType.ScheduleStartTimeStamp , AudioUnitScopeType.Global, 0, ref timeStamp, Marshal.SizeOf (timeStamp));
		}

		public AudioUnitStatus SetScheduledFiles (AudioFile audioFile)
		{
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("AudioUnit");

			if (audioFile == null)
				throw new ArgumentNullException (nameof (audioFile));

			var audioFilehandle = audioFile.Handle;
			return AudioUnitSetProperty (handle, AudioUnitPropertyIDType.ScheduledFileIDs, AudioUnitScopeType.Global, 0, ref audioFilehandle,  Marshal.SizeOf (handle));
		}

		[DllImport (Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioUnitSetProperty (IntPtr inUnit, AudioUnitPropertyIDType inID, AudioUnitScopeType inScope, uint inElement,
			IntPtr inData, int inDataSize);

		public unsafe AudioUnitStatus SetScheduledFiles (AudioFile[] audioFiles)
		{
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("AudioUnit");

			if (audioFiles == null)
				throw new ArgumentNullException (nameof (audioFiles));

			int count = audioFiles.Length;
			IntPtr[] handles = new IntPtr[count];
			for (int i = 0; i < count; i++)
				handles [i] = audioFiles [i].Handle;

			fixed (IntPtr* ptr = handles)
				return AudioUnitSetProperty (Handle, AudioUnitPropertyIDType.ScheduledFileIDs, AudioUnitScopeType.Global, 0, (IntPtr) ptr,  IntPtr.Size * count);
		}

#endif // !COREBUILD
	}

#if !XAMCORE_3_0 || MONOMAC
	[StructLayout(LayoutKind.Sequential)]
	struct AudioObjectPropertyAddress
	{
		public uint /* UInt32 */ Selector;
		public uint /* UInt32 */ Scope;
		public uint /* UInt32 */ Element;

		public AudioObjectPropertyAddress (uint selector, uint scope, uint element)
		{
			Selector = selector;
			Scope = scope;
			Element = element;
		}

		public AudioObjectPropertyAddress (AudioObjectPropertySelector selector, AudioObjectPropertyScope scope, AudioObjectPropertyElement element)
		{
			Selector = (uint) selector;
			Scope = (uint) scope;
			Element = (uint) element;
		}
	}

	public enum AudioObjectPropertySelector : uint
	{
		PropertyDevices = 1684370979, // 'dev#'
		Devices = 1684370979, // 'dev#'
		DefaultInputDevice = 1682533920, // 'dIn '
		DefaultOutputDevice = 1682929012, // 'dOut'
		DefaultSystemOutputDevice = 1934587252, // 'sOut'
		TranslateUIDToDevice = 1969841252, // 'uidd'
		MixStereoToMono = 1937010031, // 'stmo'
		PlugInList = 1886152483, // 'plg#'
		TranslateBundleIDToPlugIn = 1651074160, // 'bidp'
		TransportManagerList = 1953326883, // 'tmg#'
		TranslateBundleIDToTransportManager = 1953325673, // 'tmbi'
		BoxList = 1651472419, // 'box#'
		TranslateUIDToBox = 1969841250, // 'uidb'
		ProcessIsMaster = 1835103092, // 'mast'
		IsInitingOrExiting = 1768845172, // 'inot'
		UserIDChanged = 1702193508, // 'euid'
		ProcessIsAudible = 1886221684, // 'pmut'
		SleepingIsAllowed = 1936483696, // 'slep'
		UnloadingIsAllowed = 1970170980, // 'unld'
		HogModeIsAllowed = 1752131442, // 'hogr'
		UserSessionIsActiveOrHeadless = 1970496882, // 'user'
		ServiceRestarted = 1936880500, // 'srst'
		PowerHint = 1886353256 // 'powh'
	}

	public enum AudioObjectPropertyScope : uint
	{
		Global = 1735159650, // 'glob'
		Input = 1768845428, // 'inpt'
		Output = 1869968496, // 'outp'
		PlayThrough = 1886679669, // 'ptru'
	}

	public enum AudioObjectPropertyElement : uint
	{
		Master = 0, // 0
	}
#endif // !XAMCORE_3_0 || MONOMAC

#if XAMCORE_3_0
	internal
#elif XAMCORE_2_0
	// TODO: Uncomment once bug https://bugzilla.xamarin.com/show_bug.cgi?id=27924 is fixed
	//[Obsolete ("Please use the strongly typed properties instead.")]
	public
#else
	public
#endif
	enum AudioUnitPropertyIDType { // UInt32 AudioUnitPropertyID
		// Audio Unit Properties
		ClassInfo = 0,
		MakeConnection = 1,
		SampleRate = 2,
		ParameterList = 3,
		ParameterInfo = 4,
		CPULoad = 6,
		StreamFormat = 8,
		ElementCount = 11,
		Latency = 12,
		SupportedNumChannels = 13,
		MaximumFramesPerSlice = 14,
		ParameterValueStrings = 16,
		AudioChannelLayout = 19,
		TailTime = 20,
		BypassEffect = 21,
		LastRenderError = 22,
		SetRenderCallback = 23,
		FactoryPresets = 24,
		RenderQuality = 26,
		HostCallbacks = 27,
		InPlaceProcessing = 29,
		ElementName = 30,
		SupportedChannelLayoutTags = 32,
		PresentPreset = 36,
		DependentParameters = 45,
		InputSampleInOutput = 49,
		ShouldAllocateBuffer = 51,
		FrequencyResponse = 52,
		ParameterHistoryInfo = 53,
		Nickname = 54,
		OfflineRender = 37,
		[iOS (8, 0)]
		ParameterIDName = 34,
		[iOS (8, 0)]
		ParameterStringFromValue = 33,
		ParameterClumpName = 35,
		[iOS (8, 0)]
		ParameterValueFromString = 38,
		ContextName = 25,
		PresentationLatency = 40,
		ClassInfoFromDocument = 50,
		RequestViewController = 56,
		ParametersForOverview = 57,
		[iOS (10,0), Mac (10,12, onlyOn64: true)]
		SupportsMpe = 58,

#if MONOMAC
		FastDispatch = 5,
		SetExternalBuffer = 15,
		GetUIComponentList = 18,
		CocoaUI = 31,
		IconLocation = 39,
		AUHostIdentifier = 46,
		MIDIOutputCallbackInfo = 47,
		MIDIOutputCallback = 48,
#else
		RemoteControlEventListener = 100,
		IsInterAppConnected = 101,
		PeerURL = 102,
#endif // MONOMAC

		// Output Unit
		IsRunning = 2001,

		// OS X Availability
#if MONOMAC

		// Music Effects and Instruments
		AllParameterMIDIMappings = 41,
		AddParameterMIDIMapping = 42,
		RemoveParameterMIDIMapping = 43,
		HotMapParameterMIDIMapping = 44,

		// Music Device
		MIDIXMLNames = 1006,
		PartGroup = 1010,
		DualSchedulingMode = 1013,
		SupportsStartStopNote = 1014,

		// Offline Unit
		InputSize = 3020,
		OutputSize = 3021,
		StartOffset = 3022,
		PreflightRequirements = 3023,
		PreflightName = 3024,

		// Translation Service
		FromPlugin = 4000,
		OldAutomation = 4001,

#endif // MONOMAC

		// Apple Specific Properties
		// AUConverter
		SampleRateConverterComplexity = 3014,

		// AUHAL and device units
		CurrentDevice = 2000,
		ChannelMap = 2002, // this will also work with AUConverter
		EnableIO = 2003,
		StartTime = 2004,
		SetInputCallback = 2005,
		HasIO = 2006,
		StartTimestampsAtZero = 2007, // this will also work with AUConverter

#if !MONOMAC
		MIDICallbacks = 2010,
		HostReceivesRemoteControlEvents = 2011,
		RemoteControlToHost = 2012,
		HostTransportState = 2013,
		NodeComponentDescription = 2014,
#endif // !MONOMAC

		// AUVoiceProcessing unit
		BypassVoiceProcessing = 2100,
		VoiceProcessingEnableAGC = 2101,
		MuteOutput = 2104,

		// AUNBandEQ unit
		NumberOfBands = 2200,
		MaxNumberOfBands = 2201,
		BiquadCoefficients = 2203,

		// Mixers
		// General mixers
		MeteringMode = 3007,

		// Matrix Mixer
		MatrixLevels = 3006,
		MatrixDimensions = 3009,
		MeterClipping = 3011,
		[iOS (10,0), Mac (10,12, onlyOn64: true)]
		InputAnchorTimeStamp = 3016,

		// SpatialMixer
		ReverbRoomType = 10,
		UsesInternalReverb = 1005,
		SpatializationAlgorithm = 3000,
		[Deprecated (PlatformName.iOS, 9, 0)]
		DistanceParams = 3010,
		[Deprecated (PlatformName.iOS, 9, 0)]
		AttenuationCurve = 3013,
		[Deprecated (PlatformName.iOS, 9, 0)]
		RenderingFlags = 3003,

		// AUScheduledSoundPlayer
		ScheduleAudioSlice = 3300,
		ScheduleStartTimeStamp = 3301,
		CurrentPlayTime = 3302,

		// AUAudioFilePlayer
		ScheduledFileIDs = 3310,
		ScheduledFileRegion = 3311,
		ScheduledFilePrime = 3312,
		ScheduledFileBufferSizeFrames = 3313,
		ScheduledFileNumberBuffers = 3314,

#if MONOMAC
		// OS X-specific Music Device Properties
		SoundBankData = 1008,
		StreamFromDisk = 1011,
		SoundBankFSRef = 1012,

#endif // !MONOMAC

		// Music Device Properties
		InstrumentName = 1001,
		InstrumentNumber  = 1004,

		// Music Device Properties used by DLSMusicDevice and AUMIDISynth
		InstrumentCount = 1000,
		BankName = 1007,
		SoundBankURL = 1100,

		// AUMIDISynth
		MidiSynthEnablePreload = 4119,

		// AUSampler
		LoadInstrument = 4102,
		LoadAudioFiles = 4101,

		// AUDeferredRenderer
		DeferredRendererPullSize = 3320,
		DeferredRendererExtraLatency = 3321,
		DeferredRendererWaitFrames = 3322,

#if MONOMAC
		// AUNetReceive
		Hostname = 3511,
		NetReceivePassword = 3512,

		// AUNetSend
		PortNum = 3513,
		TransmissionFormat = 3514,
		TransmissionFormatIndex = 3515,
		ServiceName = 3516,
		Disconnect = 3517,
		NetSendPassword = 3518,
#endif // MONOMAC
	}

	public enum AudioUnitParameterType // UInt32 in AudioUnitParameterInfo
	{
		// AUMixer3D unit
		Mixer3DAzimuth						= 0,
		Mixer3DElevation					= 1,
		Mixer3DDistance						= 2,
		Mixer3DGain							= 3,
		Mixer3DPlaybackRate					= 4,
#if MONOMAC
		Mixer3DReverbBlend					= 5,
		Mixer3DGlobalReverbGain				= 6,
		Mixer3DOcclusionAttenuation			= 7,
		Mixer3DObstructionAttenuation		= 8,
		Mixer3DMinGain						= 9,
		Mixer3DMaxGain						= 10,
		Mixer3DPreAveragePower				= 1000,
		Mixer3DPrePeakHoldLevel				= 2000,
		Mixer3DPostAveragePower				= 3000,
		Mixer3DPostPeakHoldLevel			= 4000,
#else
		Mixer3DEnable						= 5,
		Mixer3DMinGain						= 6,
		Mixer3DMaxGain						= 7,
		Mixer3DReverbBlend					= 8,
		Mixer3DGlobalReverbGain				= 9,
		Mixer3DOcclusionAttenuation			= 10,
		Mixer3DObstructionAttenuation		= 11,
#endif

		// AUSpatialMixer unit
		SpatialAzimuth						= 0,
		SpatialElevation					= 1,
		SpatialDistance						= 2,
		SpatialGain							= 3,
		SpatialPlaybackRate					= 4,
		SpatialEnable						= 5,
		SpatialMinGain						= 6,
		SpatialMaxGain						= 7,
		SpatialReverbBlend					= 8,
		SpatialGlobalReverbGain				= 9,
		SpatialOcclusionAttenuation			= 10,
		SpatialObstructionAttenuation		= 11,

		// Reverb applicable to the 3DMixer or AUSpatialMixer
		ReverbFilterFrequency				= 14,
		ReverbFilterBandwidth				= 15,
		ReverbFilterGain					= 16,
		[iOS (8, 0)]
		ReverbFilterType					= 17,
		[iOS (8, 0)]
		ReverbFilterEnable					= 18,

		// AUMultiChannelMixer
		MultiChannelMixerVolume				= 0,
		MultiChannelMixerEnable				= 1,
		MultiChannelMixerPan				= 2,

		// AUMatrixMixer unit
		MatrixMixerVolume					= 0,
		MatrixMixerEnable					= 1,
	
		// AudioDeviceOutput, DefaultOutputUnit, and SystemOutputUnit units
		HALOutputVolume 					= 14, 

		// AUTimePitch, AUTimePitch (offline), AUPitch units
		TimePitchRate						= 0,
#if MONOMAC
		TimePitchPitch						= 1,
		TimePitchEffectBlend				= 2,
#endif

		// AUNewTimePitch
		NewTimePitchRate					= 0,
		NewTimePitchPitch					= 1,
		NewTimePitchOverlap					= 4,
		NewTimePitchEnablePeakLocking		= 6,

		// AUSampler unit
		AUSamplerGain						= 900,
		AUSamplerCoarseTuning				= 901,
		AUSamplerFineTuning					= 902,
		AUSamplerPan						= 903,

		// AUBandpass
		BandpassCenterFrequency 			= 0,
		BandpassBandwidth	 				= 1,

		// AUHipass
		HipassCutoffFrequency 				= 0,
		HipassResonance						= 1,

		// AULowpass
		LowPassCutoffFrequency 				= 0,
		LowPassResonance 					= 1,

		// AUHighShelfFilter
		HighShelfCutOffFrequency 			= 0,
		HighShelfGain 						= 1,

		// AULowShelfFilter
		AULowShelfCutoffFrequency			= 0,
		AULowShelfGain						= 1,

		[Obsoleted (PlatformName.iOS, 7, 0)]
		AUDCFilterDecayTime					= 0,

		// AUParametricEQ
		ParametricEQCenterFreq				= 0,
		ParametricEQQ						= 1,
		ParametricEQGain					= 2,

		// AUPeakLimiter
		LimiterAttackTime		 			= 0,
		LimiterDecayTime 					= 1,
		LimiterPreGain 						= 2,

		// AUDynamicsProcessor
		DynamicsProcessorThreshold 			= 0,
		DynamicsProcessorHeadRoom	 		= 1,
		DynamicsProcessorExpansionRatio		= 2,
		DynamicsProcessorExpansionThreshold	= 3,
		DynamicsProcessorAttackTime			= 4,
		DynamicsProcessorReleaseTime 		= 5,
		DynamicsProcessorMasterGain			= 6,
		DynamicsProcessorCompressionAmount 	= 1000,
		DynamicsProcessorInputAmplitude		= 2000,
		DynamicsProcessorOutputAmplitude 	= 3000,

		// AUVarispeed
		VarispeedPlaybackRate				= 0,
		VarispeedPlaybackCents				= 1,

		// Distortion unit 
		DistortionDelay						= 0,
		DistortionDecay						= 1,
		DistortionDelayMix					= 2,
		DistortionDecimation				= 3,
		DistortionRounding					= 4,
		DistortionDecimationMix				= 5,
		DistortionLinearTerm				= 6,  
		DistortionSquaredTerm				= 7,	
		DistortionCubicTerm					= 8,  
		DistortionPolynomialMix				= 9,
		DistortionRingModFreq1				= 10,
		DistortionRingModFreq2				= 11,
		DistortionRingModBalance			= 12,
		DistortionRingModMix				= 13,
		DistortionSoftClipGain				= 14,
		DistortionFinalMix					= 15,

		// AUDelay
		DelayWetDryMix 						= 0,
		DelayTime							= 1,
		DelayFeedback 						= 2,
		DelayLopassCutoff	 				= 3,

		// AUNBandEQ
		AUNBandEQGlobalGain					= 0,
		AUNBandEQBypassBand					= 1000,
		AUNBandEQFilterType					= 2000,
		AUNBandEQFrequency					= 3000,
		AUNBandEQGain						= 4000,
		AUNBandEQBandwidth					= 5000,

		// AURandomUnit
		RandomBoundA 						= 0,
		RandomBoundB						= 1,
		RandomCurve							= 2,

#if !MONOMAC
		// iOS reverb
		Reverb2DryWetMix					= 0,
		Reverb2Gain							= 1,
		Reverb2MinDelayTime					= 2,
		Reverb2MaxDelayTime					= 3,
		Reverb2DecayTimeAt0Hz				= 4,
		Reverb2DecayTimeAtNyquist			= 5,
		Reverb2RandomizeReflections			= 6,
#endif

		// RoundTripAAC
		RoundTripAacFormat = 0,
		RoundTripAacEncodingStrategy = 1,
		RoundTripAacRateOrQuality = 2,

		// Spacial Mixer
		SpacialMixerAzimuth = 0,
		Elevation = 1, 
		Distance = 2, 
		Gain = 3, 
		PlaybackRate = 4,
		Enable = 5,
		MinGain = 6, 
		MaxGain = 7,
		ReverbBlend = 8,
		GlobalReverbGain = 9,
		OcclussionAttenuation = 10,
		ObstructionAttenuation = 11
	}

	[iOS (8, 0)]
	public enum SpatialMixerAttenuation {
		Power = 0,
		Exponential = 1,
		Inverse = 2,
		Linear = 3
	}

	[Flags]
	[iOS (8, 0)]
	public enum SpatialMixerRenderingFlags {
		InterAuralDelay = (1 << 0),
		[Deprecated (PlatformName.iOS, 9, 0)]
		DistanceAttenuation = (1 << 2),
	}

	[Flags]
	public enum ScheduledAudioSliceFlag {
		Complete = 0x01,
		BeganToRender = 0x02,
		BeganToRenderLate = 0x04,

		[iOS (8, 0)]
		[Mac (10, 10)]
		Loop                   = 0x08,
		[iOS (8, 0)]
		[Mac (10, 10)]
		Interrupt              = 0x10,
		[iOS (8, 0)]
		[Mac (10, 10)]
		InterruptAtLoop        = 0x20
	}

	public enum AudioUnitScopeType { // UInt32 AudioUnitScope
		Global		= 0,
		Input		= 1,
		Output		= 2,
		Group		= 3,
		Part		= 4,
		Note		= 5,
		Layer		= 6,
		LayerItem	= 7
	}

	[Flags]
	public enum AudioUnitRenderActionFlags { // UInt32 AudioUnitRenderActionFlags
		PreRender = (1 << 2),
		PostRender = (1 << 3),
		OutputIsSilence = (1 << 4),
		OfflinePreflight = (1 << 5),
		OfflineRender = (1 << 6),
		OfflineComplete = (1 << 7),
		PostRenderError = (1 << 8),
		DoNotCheckRenderArgs = (1 << 9)
	}

	public enum AudioUnitRemoteControlEvent // Unused?
	{
		TogglePlayPause		= 1,
		ToggleRecord		= 2,
		Rewind				= 3
	}

	[Native]
	public enum AudioUnitBusType : long
	{
		Input = 1,
		Output = 2
	}

	[Native]
	public enum AUHostTransportStateFlags : ulong
	{
		Changed = 1,
		Moving = 2,
		Recording = 4,
		Cycling = 8
	}

	public enum AUEventSampleTime : long
	{
		Immediate = unchecked ((long) 0xffffffff00000000)
	}

	public unsafe class AURenderEventEnumerator : IEnumerator<AURenderEvent>, INativeObject
	{
		AURenderEvent *current;

		public IntPtr Handle { get; private set; }
		public bool IsEmpty { get { return Handle == IntPtr.Zero; } }
		public bool IsAtEnd { get { return current == null; }}

		public AURenderEventEnumerator (IntPtr ptr)
		{
			Handle = ptr;
			current = (AURenderEvent *) ptr;
		}

		public void Dispose ()
		{
			Handle = IntPtr.Zero;
			current = null;
		}

		public AURenderEvent * UnsafeFirst {
			get {
				return (AURenderEvent*) Handle;
			}
		}

		public AURenderEvent First {
			get {
				if (IsEmpty)
					throw new InvalidOperationException ("Can not get First on AURenderEventEnumerator when empty");
				return *(AURenderEvent *) Handle;
			}
		}

		public AURenderEvent Current {
			get {
				if (IsAtEnd)
					throw new InvalidOperationException ("Can not get Current on AURenderEventEnumerator when at end");
				return *current;
			}
		}

		object IEnumerator.Current
		{
			get { return Current; }
		}

		bool IsAt (nint now)
		{
			return current != null && (current->Head.EventSampleTime == now);
		}

		public IEnumerable <AURenderEvent> EnumeratorCurrentEvents (nint now)
		{
			if (IsAtEnd)
				throw new InvalidOperationException ("Can not enumerate events on AURenderEventEnumerator when at end");

			do {
				yield return Current;
				MoveNext ();
			} while (IsAt (now));
		}

		public bool /*IEnumerator<AURenderEvent>.*/MoveNext ()
		{
			if (current != null)
				current = ((AURenderEvent *)current)->Head.UnsafeNext;
			return current != null;
		}

		public void /*IEnumerator<AURenderEvent>.*/Reset ()
		{
			current = (AURenderEvent *) Handle;
		}
	}

	[StructLayout (LayoutKind.Sequential)]
	public unsafe struct AURenderEventHeader
	{
		public AURenderEvent * UnsafeNext;

		public AURenderEvent? Next {
			get {
				if (UnsafeNext != null)
					return (AURenderEvent?) Marshal.PtrToStructure ((IntPtr)UnsafeNext, typeof (AURenderEvent));
				return null;
			}
		}

		public long EventSampleTime;

		public AURenderEventType EventType;

		public byte Reserved;
	}

	[StructLayout (LayoutKind.Sequential)]
	public struct AUParameterObserverToken
	{
		public IntPtr ObserverToken;
		public AUParameterObserverToken (IntPtr observerToken)
		{
			ObserverToken = observerToken;
		}
	}

	[StructLayout (LayoutKind.Sequential)]
	public unsafe struct AUParameterEvent
	{
		public AURenderEvent * UnsafeNext;

		public AURenderEvent? Next {
			get {
				if (UnsafeNext != null)
					return (AURenderEvent?) Marshal.PtrToStructure ((IntPtr)UnsafeNext, typeof (AURenderEvent));
				return null;
			}
		}

		public long EventSampleTime;

		public AURenderEventType EventType;

		internal byte reserved1, reserved2, reserved3;

		public uint RampDurationSampleFrames;

		public ulong ParameterAddress;

		public float Value;
	}

// 	AUAudioTODO - We need testing for these bindings
// 	[StructLayout (LayoutKind.Sequential)]
// 	public unsafe struct AUMidiEvent
// 	{
//		public AURenderEvent * UnsafeNext;
//
//		public AURenderEvent? Next {
//			get {
//				if (UnsafeNext != null)
//					return (AURenderEvent?) Marshal.PtrToStructure ((IntPtr)UnsafeNext, typeof (AURenderEvent));
//				return null;
//			}
//		}
//
// 		public long EventSampleTime;
//
// 		public AURenderEventType EventType;
//
// 		public byte Reserved;
//
// 		public ushort Length;
//
// 		public byte Cable;
//
// 		public byte Data_1, Data_2, Data_3;
// 	}

	[StructLayout (LayoutKind.Explicit)]
	public struct AURenderEvent
	{
		[FieldOffset (0)]
		public AURenderEventHeader Head;

		[FieldOffset (0)]
		public AUParameterEvent Parameter;

// 		[FieldOffset (0)]
// 		public AUMidiEvent Midi;
	}

 	[StructLayout (LayoutKind.Sequential)]
 	public struct AURecordedParameterEvent
 	{
 		public ulong HostTime;

 		public ulong Address;

 		public float Value;
 	}

	public enum AURenderEventType : byte
	{
		Parameter = 1,
		ParameterRamp = 2,
		Midi = 8,
		MidiSysEx = 9
	}

	[iOS (9,0), Mac (10,11)]
	public enum AudioComponentInstantiationOptions : uint {
		OutOfProcess = 1,
		InProcess = 2
	}

	[Native]
	public enum AUAudioUnitBusType : long
	{
		Input = 1,
		Output = 2
	}

	public enum AudioUnitParameterOptions : uint
	{
		CFNameRelease = (1 << 4),
		OmitFromPresets = (1 << 13),
		PlotHistory = (1 << 14),
		MeterReadOnly = (1 << 15),
		DisplayMask = (7 << 16) | (1 << 22),
		DisplaySquareRoot = (1 << 16),
		DisplaySquared = (2 << 16),
		DisplayCubed = (3 << 16),
		DisplayCubeRoot = (4 << 16),
		DisplayExponential = (5 << 16),
		HasClump = (1 << 20),
		ValuesHaveStrings = (1 << 21),
		DisplayLogarithmic = (1 << 22),
		IsHighResolution = (1 << 23),
		NonRealTime = (1 << 24),
		CanRamp = (1 << 25),
		ExpertMode = (1 << 26),
		HasCFNameString = (1 << 27),
		IsGlobalMeta = (1 << 28),
		IsElementMeta = (1 << 29),
		IsReadable = (1 << 30),
		IsWritable = unchecked((uint)1 << 31)
	}

	public enum AudioComponentValidationResult : uint
	{
		Unknown = 0,
		Passed,
		Failed,
		TimedOut,
		UnauthorizedErrorOpen,
		UnauthorizedErrorInit
	}

	public enum AUSpatialMixerAttenuationCurve : uint
	{
		Power = 0,
		Exponential = 1,
		Inverse = 2,
		Linear = 3
	}

	public enum AU3DMixerRenderingFlags : uint
	{
		InterAuralDelay = (1 << 0),
		DopplerShift = (1 << 1),
		DistanceAttenuation = (1 << 2),
		DistanceFilter = (1 << 3),
		DistanceDiffusion = (1 << 4),
		LinearDistanceAttenuation = (1 << 5),
		ConstantReverbBlend = (1 << 6)
	}

	public enum AUReverbRoomType : uint
	{
		SmallRoom = 0,
		MediumRoom = 1,
		LargeRoom = 2,
		MediumHall = 3,
		LargeHall = 4,
		Plate = 5,
		MediumChamber = 6,
		LargeChamber = 7,
		Cathedral = 8,
		LargeRoom2 = 9,
		MediumHall2 = 10,
		MediumHall3 = 11,
		LargeHall2 = 12
	}

	public enum AUScheduledAudioSliceFlags : uint
	{
		Complete = 1,
		BeganToRender = 2,
		BeganToRenderLate = 4,
		Loop = 8,
		Interrupt = 16,
		InterruptAtLoop = 32
	}

	public enum AUSpatializationAlgorithm : uint
	{
		EqualPowerPanning = 0,
		SphericalHead = 1,
		Hrtf = 2,
		SoundField = 3,
		VectorBasedPanning = 4,
		StereoPassThrough = 5,
		HrtfHQ = 6,
	}

	public enum AU3DMixerAttenuationCurve : uint
	{
		Power = 0,
		Exponential = 1,
		Inverse = 2,
		Linear = 3
	}

	public enum AUSpatialMixerRenderingFlags : uint
	{
		InterAuralDelay = (1 << 0),
		DistanceAttenuation = (1 << 2)
	}

	[iOS (10,0), Mac (10,12, onlyOn64: true)]
	[StructLayout (LayoutKind.Sequential)]
	public struct AUParameterAutomationEvent {
		public ulong HostTime;
		public ulong Address;
		public float Value;
		public AUParameterAutomationEventType EventType;
		ulong Reserved;
	}

	[iOS (10,0), Mac (10,12, onlyOn64: true)]
	public enum AUParameterAutomationEventType : uint {
		Value = 0,
		Touch = 1,
		Release = 2
	}

#if !COREBUILD
//	Configuration Info Keys
	public static class AudioUnitConfigurationInfo
	{
//		#define kAudioUnitConfigurationInfo_HasCustomView	"HasCustomView"
		public static NSString HasCustomView = new NSString ("HasCustomView");

//		#define kAudioUnitConfigurationInfo_ChannelConfigurations	"ChannelConfigurations"
		public static NSString ChannelConfigurations = new NSString ("ChannelConfigurations");

//		#define kAudioUnitConfigurationInfo_InitialInputs	"InitialInputs"
		public static NSString InitialInputs = new NSString ("InitialInputs");

//		#define kAudioUnitConfigurationInfo_IconURL			"IconURL"
		public static NSString IconUrl = new NSString ("IconURL");

//		#define kAudioUnitConfigurationInfo_BusCountWritable	"BusCountWritable"
		public static NSString BusCountWritable = new NSString ("BusCountWritable");

//		#define kAudioUnitConfigurationInfo_SupportedChannelLayoutTags	"SupportedChannelLayoutTags"
		public static NSString SupportedChannelLayoutTags = new NSString ("SupportedChannelLayoutTags");
	}
#endif

	public enum AudioUnitSubType : uint
	{
		AUConverter 			= 0x636F6E76, // 'conv'
		Varispeed 				= 0x76617269, // 'vari'
		DeferredRenderer 		= 0x64656672, // 'defr'
		Splitter 				= 0x73706C74, // 'splt'
		MultiSplitter 			= 0x6D73706C, // 'mspl'
		Merger 					= 0x6D657267, // 'merg'
		NewTimePitch 			= 0x6E757470, // 'nutp'
		AUiPodTimeOther 		= 0x6970746F, // 'ipto'
		RoundTripAac 			= 0x72616163, // 'raac'
		GenericOutput 			= 0x67656E72, // 'genr'
		VoiceProcessingIO 		= 0x7670696F, // 'vpio'
		Sampler 				= 0x73616D70, // 'samp'
		MidiSynth 				= 0x6D73796E, // 'msyn'
		PeakLimiter 			= 0x6C6D7472, // 'lmtr'
		DynamicsProcessor 		= 0x64636D70, // 'dcmp'
		LowPassFilter 			= 0x6C706173, // 'lpas'
		HighPassFilter 			= 0x68706173, // 'hpas'
		BandPassFilter 			= 0x62706173, // 'bpas'
		HighShelfFilter 		= 0x68736866, // 'hshf'
		LowShelfFilter 			= 0x6C736866, // 'lshf'
		ParametricEQ 			= 0x706D6571, // 'pmeq'
		Distortion 				= 0x64697374, // 'dist'
		Delay 					= 0x64656C79, // 'dely'
		SampleDelay 			= 0x73646C79, // 'sdly'
		NBandEQ 				= 0x6E626571, // 'nbeq'
		MultiChannelMixer 		= 0x6D636D78, // 'mcmx'
		MatrixMixer 			= 0x6D786D78, // 'mxmx'
		SpatialMixer 			= 0x3364656D, // '3dem'
		ScheduledSoundPlayer 	= 0x7373706C, // 'sspl'
		AudioFilePlayer 		= 0x6166706C, // 'afpl'
			
#if MONOMAC
		HALOutput 				= 0x6168616C, // 'ahal'
		DefaultOutput 			= 0x64656620, // 'def '
		SystemOutput 			= 0x73797320, // 'sys '
		DLSSynth 				= 0x646C7320, // 'dls '
		TimePitch 				= 0x746D7074, // 'tmpt'
		GraphicEQ 				= 0x67726571, // 'greq'
		MultiBandCompressor 	= 0x6D636D70, // 'mcmp'
		MatrixReverb 			= 0x6D726576, // 'mrev'
		Pitch 					= 0x746D7074, // 'tmpt'
		AUFilter 				= 0x66696C74, // 'filt
		NetSend 				= 0x6E736E64, // 'nsnd'
		RogerBeep 				= 0x726F6772, // 'rogr'
		StereoMixer 			= 0x736D7872, // 'smxr'
		SphericalHeadPanner 	= 0x73706872, // 'sphr'
		VectorPanner 			= 0x76626173, // 'vbas'
		SoundFieldPanner 		= 0x616D6269, // 'ambi'
		HRTFPanner 				= 0x68727466, // 'hrtf'
		NetReceive 				= 0x6E726376, // 'nrcv'
#endif
	}
#if !XAMCORE_4_0 && !COREBUILD
#if XAMCORE_2_0 || !MONOMAC
	[Obsolete ("Use 'AUImplementorStringFromValueCallback' instead.")]
	public delegate NSString _AUImplementorStringFromValueCallback (AUParameter param, IntPtr value);
#endif
#endif
}
