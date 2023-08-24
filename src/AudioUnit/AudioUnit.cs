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

#nullable enable

// Adding this warning disable since AudioUnitPropertyIDType is removed from public API but used internally
#if !XAMCORE_3_0
#pragma warning disable CS0618
#endif

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

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace AudioUnit {
#if !COREBUILD
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class AudioUnitException : Exception {
		static string Lookup (int k)
		{
			switch ((AudioUnitStatus) k) {
			case AudioUnitStatus.InvalidProperty:
				return "Invalid Property";

			case AudioUnitStatus.InvalidParameter:
				return "Invalid Parameter";

			case AudioUnitStatus.InvalidElement:
				return "Invalid Element";

			case AudioUnitStatus.NoConnection:
				return "No Connection";

			case AudioUnitStatus.FailedInitialization:
				return "Failed Initialization";

			case AudioUnitStatus.TooManyFramesToProcess:
				return "Too Many Frames To Process";

			case AudioUnitStatus.InvalidFile:
				return "Invalid File";

			case AudioUnitStatus.FormatNotSupported:
				return "Format Not Supported";

			case AudioUnitStatus.Uninitialized:
				return "Uninitialized";

			case AudioUnitStatus.InvalidScope:
				return "Invalid Scope";

			case AudioUnitStatus.PropertyNotWritable:
				return "Property Not Writable";

			case AudioUnitStatus.CannotDoInCurrentContext:
				return "Cannot Do In Current Context";

			case AudioUnitStatus.InvalidPropertyValue:
				return "Invalid Property Value";

			case AudioUnitStatus.PropertyNotInUse:
				return "Property Not In Use";

			case AudioUnitStatus.Initialized:
				return "Initialized";

			case AudioUnitStatus.InvalidOfflineRender:
				return "Invalid Offline Render";

			case AudioUnitStatus.Unauthorized:
				return "Unauthorized";

			}
			return String.Format ("Unknown error code: 0x{0:x}", k);
		}

		internal AudioUnitException (AudioUnitStatus status)
			: this ((int) status)
		{
		}

		internal AudioUnitException (int k) : base (Lookup (k))
		{
		}
	}

	public delegate AudioUnitStatus RenderDelegate (AudioUnitRenderActionFlags actionFlags, AudioTimeStamp timeStamp, uint busNumber, uint numberFrames, AudioBuffers data);
	public delegate AudioUnitStatus InputDelegate (AudioUnitRenderActionFlags actionFlags, AudioTimeStamp timeStamp, uint busNumber, uint numberFrames, AudioUnit audioUnit);

	delegate AudioUnitStatus CallbackShared (IntPtr /* void* */ clientData, ref AudioUnitRenderActionFlags /* AudioUnitRenderActionFlags* */ actionFlags, ref AudioTimeStamp /* AudioTimeStamp* */ timeStamp, uint /* UInt32 */ busNumber, uint /* UInt32 */ numberFrames, IntPtr /* AudioBufferList* */ data);
#endif // !COREBUILD

#if NET
	[StructLayout (LayoutKind.Sequential)]
	unsafe struct AURenderCallbackStruct
	{
#if COREBUILD
		public delegate* unmanaged<IntPtr, int*, AudioTimeStamp*, uint, uint, IntPtr, int> Proc;
#else
		public delegate* unmanaged<IntPtr, AudioUnitRenderActionFlags*, AudioTimeStamp*, uint, uint, IntPtr, AudioUnitStatus> Proc;
#endif
		public IntPtr ProcRefCon; 
	}
#else
	[StructLayout (LayoutKind.Sequential)]
	struct AURenderCallbackStruct {
		public Delegate Proc;
		public IntPtr ProcRefCon;
	}
#endif

	[StructLayout (LayoutKind.Sequential)]
	struct AudioUnitConnection {
		public IntPtr SourceAudioUnit;
		public uint /* UInt32 */ SourceOutputNumber;
		public uint /* UInt32 */ DestInputNumber;
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class SamplerInstrumentData {
#if !COREBUILD
		public const byte DefaultPercussionBankMSB = 0x78;
		public const byte DefaultMelodicBankMSB = 0x79;
		public const byte DefaultBankLSB = 0x00;

		public SamplerInstrumentData (CFUrl fileUrl, InstrumentType instrumentType)
		{
			if (fileUrl is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (fileUrl));

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

	[StructLayout (LayoutKind.Sequential)]
	struct AUSamplerInstrumentData {
		public IntPtr FileUrl;
#if COREBUILD
		// keep structure size identical across builds
		public byte InstrumentType;
#else
		public InstrumentType InstrumentType;
#endif // !COREBUILD
		public byte BankMSB;
		public byte BankLSB;
		public byte PresetID;
	}

	[StructLayout (LayoutKind.Sequential)]
	unsafe struct AudioUnitParameterInfoNative // AudioUnitParameterInfo in Obj-C
	{
		fixed byte /* char[52] */ name [52]; // unused
		public IntPtr /* CFStringRef */ UnitName;
#if COREBUILD
		// keep structure size identical across builds
		public uint ClumpID;
#else
		public AudioUnitClumpID /* UInt32 */ ClumpID;
#endif // !COREBUILD
		public IntPtr /* CFStringRef */ NameString;

#if COREBUILD
		// keep structure size identical across builds
		public uint Unit;
#else
		public AudioUnitParameterUnit /* AudioUnitParameterUnit */ Unit;
#endif // !COREBUILD
		public float /* AudioUnitParameterValue = Float32 */ MinValue;
		public float /* AudioUnitParameterValue = Float32 */ MaxValue;
		public float /* AudioUnitParameterValue = Float32 */ DefaultValue;
#if COREBUILD
		// keep structure size identical across builds
		public uint Flags;
#else
		public AudioUnitParameterFlag /* UInt32 */ Flags;
#endif // !COREBUILD
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class AudioUnitParameterInfo {
#if !COREBUILD
		public string? UnitName { get; private set; }
		public AudioUnitClumpID ClumpID { get; private set; }
		public string? Name { get; private set; }
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
				info.Name = CFString.FromHandle (native.NameString);

				if ((native.Flags & AudioUnitParameterFlag.CFNameRelease) != 0)
					CFObject.CFRelease (native.NameString);
			}

			if (native.Unit == AudioUnitParameterUnit.CustomUnit) {
				info.UnitName = CFString.FromHandle (native.UnitName);
			}

			return info;
		}
#endif // !COREBUILD
	}

	public enum AUParameterEventType : uint {
		Immediate = 1,
		Ramped = 2,
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct AudioUnitParameterEvent {
		public uint Scope;
		public uint Element;
		public uint Parameter;
		public AUParameterEventType EventType;

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		[StructLayout (LayoutKind.Explicit)]
		public struct EventValuesStruct {
			[StructLayout (LayoutKind.Sequential)]
			public struct RampStruct {
				public int StartBufferOffset;
				public uint DurationInFrames;
				public float StartValue;
				public float EndValue;
			}


			[FieldOffset (0)]
			public RampStruct Ramp;

			[StructLayout (LayoutKind.Sequential)]
			public struct ImmediateStruct {
				public uint BufferOffset;
				public float Value;
			}

			[FieldOffset (0)]
			public ImmediateStruct Immediate;
		}

		public EventValuesStruct EventValues;
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class AudioUnit : DisposableObject {
#if !COREBUILD
#if !NET
		static readonly CallbackShared CreateRenderCallback = RenderCallbackImpl;
		static readonly CallbackShared CreateInputCallback = InputCallbackImpl;
#endif

		GCHandle gcHandle;
		bool _isPlaying;

		Dictionary<uint, RenderDelegate>? renderer;
		Dictionary<uint, InputDelegate>? inputs;

		[Preserve (Conditional = true)]
		internal AudioUnit (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		static IntPtr Create (AudioComponent component)
		{
			if (component is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (component));

			var err = AudioComponentInstanceNew (component.GetCheckedHandle (), out var handle);
			if (err != 0)
				throw new AudioUnitException (err);

			return handle;
		}

		public AudioUnit (AudioComponent component)
			: this (Create (component), true)
		{
		}

		public AudioComponent Component {
			get {
				return new AudioComponent (AudioComponentInstanceGetComponent (Handle), false);
			}
		}

		public bool IsPlaying { get { return _isPlaying; } }


#if !XAMCORE_3_0
		[Obsolete ("Use 'SetFormat' instead as it has the ability of returning a status code.")]
		public void SetAudioFormat (AudioToolbox.AudioStreamBasicDescription audioFormat, AudioUnitScopeType scope, uint audioUnitElement = 0)
		{
			var err = AudioUnitSetProperty (Handle,
							   AudioUnitPropertyIDType.StreamFormat,
							   scope,
							   audioUnitElement,
							   ref audioFormat,
							   (uint) Marshal.SizeOf<AudioToolbox.AudioStreamBasicDescription> ());
			if (err != 0)
				throw new AudioUnitException (err);
		}
#endif

		public AudioUnitStatus SetFormat (AudioToolbox.AudioStreamBasicDescription audioFormat, AudioUnitScopeType scope, uint audioUnitElement = 0)
		{
			return (AudioUnitStatus) AudioUnitSetProperty (Handle,
							   AudioUnitPropertyIDType.StreamFormat,
							   scope,
							   audioUnitElement,
							   ref audioFormat,
							   (uint) Marshal.SizeOf<AudioToolbox.AudioStreamBasicDescription> ());
		}

		public uint GetCurrentDevice (AudioUnitScopeType scope, uint audioUnitElement = 0)
		{
			uint device = 0;
			int size = sizeof (uint);
			var err = AudioUnitGetProperty (Handle,
						AudioUnitPropertyIDType.CurrentDevice,
						scope,
						audioUnitElement,
						ref device,
						ref size);
			if (err != 0)
				throw new AudioUnitException ((int) err);
			return device;
		}

#if !XAMCORE_3_0 || MONOMAC || __MACCATALYST__
#if !MONOMAC && !__MACCATALYST__
		[Obsolete ("This API is not available on iOS.")]
#endif
#if NET
		[SupportedOSPlatform ("maccatalyst15.0")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
#else
		[MacCatalyst (15, 0)]
#endif
		public static uint GetCurrentInputDevice ()
		{
#if MONOMAC || __MACCATALYST__
			// We need to replace AudioHardwareGetProperty since it has been deprecated since OS X 10.6 and iOS 2.0
			// Replacing with the following implementation recommended in the following doc
			// See Listing 4  New - Getting the default input device.
			// https://developer.apple.com/library/mac/technotes/tn2223/_index.html

			uint inputDevice;
			uint size = (uint) sizeof (uint);
			var theAddress = new AudioObjectPropertyAddress (
				AudioObjectPropertySelector.DefaultInputDevice,
				AudioObjectPropertyScope.Global,
				AudioObjectPropertyElement.Main);
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
			return AudioUnitSetProperty (Handle,
						AudioUnitPropertyIDType.CurrentDevice,
						scope,
						audioUnitElement,
						ref inputDevice,
						(uint) sizeof (uint));
		}

		public AudioStreamBasicDescription GetAudioFormat (AudioUnitScopeType scope, uint audioUnitElement = 0)
		{
			var audioFormat = new AudioStreamBasicDescription ();
			uint size = (uint) Marshal.SizeOf<AudioStreamBasicDescription> ();

			var err = AudioUnitGetProperty (Handle,
							   AudioUnitPropertyIDType.StreamFormat,
							   scope,
							   audioUnitElement,
							   ref audioFormat,
							   ref size);
			if (err != 0)
				throw new AudioUnitException ((int) err);

			return audioFormat;
		}

		public ClassInfoDictionary? GetClassInfo (AudioUnitScopeType scope = AudioUnitScopeType.Global, uint audioUnitElement = 0)
		{
			IntPtr ptr = new IntPtr ();
			int size = IntPtr.Size;
			var res = AudioUnitGetProperty (Handle, AudioUnitPropertyIDType.ClassInfo, scope, audioUnitElement,
				ref ptr, ref size);

			if (res != 0)
				return null;

			return new ClassInfoDictionary (Runtime.GetNSObject<NSDictionary> (ptr, true));
		}

		public AudioUnitStatus SetClassInfo (ClassInfoDictionary preset, AudioUnitScopeType scope = AudioUnitScopeType.Global, uint audioUnitElement = 0)
		{
			var ptr = preset.Dictionary.Handle;
			return AudioUnitSetProperty (Handle, AudioUnitPropertyIDType.ClassInfo, scope, audioUnitElement,
				ref ptr, IntPtr.Size);
		}

		public unsafe AudioUnitParameterInfo []? GetParameterList (AudioUnitScopeType scope = AudioUnitScopeType.Global, uint audioUnitElement = 0)
		{
			uint size;
			bool writable;
			if (AudioUnitGetPropertyInfo (Handle, AudioUnitPropertyIDType.ParameterList, scope, audioUnitElement, out size, out writable) != 0)
				return null;

			// Array of AudioUnitParameterID = UInt32
			var data = new uint [size / sizeof (uint)];
			fixed (uint* ptr = data) {
				if (AudioUnitGetProperty (Handle, AudioUnitPropertyIDType.ParameterList, scope, audioUnitElement, ptr, ref size) != 0)
					return null;
			}

			var info = new AudioUnitParameterInfo [data.Length];
			size = (uint) sizeof (AudioUnitParameterInfoNative);

			for (int i = 0; i < data.Length; ++i) {
				var native = new AudioUnitParameterInfoNative ();
				if (AudioUnitGetProperty (Handle, AudioUnitPropertyIDType.ParameterInfo, scope, data [i], ref native, ref size) != 0)
					return null;

				info [i] = AudioUnitParameterInfo.Create (native, (AudioUnitParameterType) data [i]);
			}

			return info;
		}

		public AudioUnitStatus LoadInstrument (SamplerInstrumentData instrumentData, AudioUnitScopeType scope = AudioUnitScopeType.Global, uint audioUnitElement = 0)
		{
			if (instrumentData is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (instrumentData));

			var data = instrumentData.ToStruct ();
			return AudioUnitSetProperty (Handle, AudioUnitPropertyIDType.LoadInstrument, scope, audioUnitElement,
				ref data, Marshal.SizeOf<AUSamplerInstrumentData> ());
		}

		public AudioUnitStatus MakeConnection (AudioUnit sourceAudioUnit, uint sourceOutputNumber, uint destInputNumber)
		{
			var auc = new AudioUnitConnection {
				SourceAudioUnit = sourceAudioUnit.GetHandle (),
				SourceOutputNumber = sourceOutputNumber,
				DestInputNumber = destInputNumber
			};

			return AudioUnitSetProperty (Handle, AudioUnitPropertyIDType.MakeConnection, AudioUnitScopeType.Input, 0, ref auc, Marshal.SizeOf<AudioUnitConnection> ());
		}

		public AudioUnitStatus SetEnableIO (bool enableIO, AudioUnitScopeType scope, uint audioUnitElement = 0)
		{
			// EnableIO: UInt32          
			uint flag = enableIO ? (uint) 1 : 0;
			return AudioUnitSetProperty (Handle, AudioUnitPropertyIDType.EnableIO, scope, audioUnitElement, ref flag, sizeof (uint));
		}

		public AudioUnitStatus SetMaximumFramesPerSlice (uint value, AudioUnitScopeType scope, uint audioUnitElement = 0)
		{
			// MaximumFramesPerSlice: UInt32
			return AudioUnitSetProperty (Handle, AudioUnitPropertyIDType.MaximumFramesPerSlice, scope, audioUnitElement, ref value, sizeof (uint));
		}

		public uint GetMaximumFramesPerSlice (AudioUnitScopeType scope = AudioUnitScopeType.Global, uint audioUnitElement = 0)
		{
			// MaximumFramesPerSlice: UInt32
			uint value = 0;
			uint size = sizeof (uint);
			var res = AudioUnitGetProperty (Handle, AudioUnitPropertyIDType.MaximumFramesPerSlice, scope,
				audioUnitElement, ref value, ref size);

			if (res != 0)
				throw new AudioUnitException ((int) res);

			return value;
		}

		public AudioUnitStatus SetElementCount (AudioUnitScopeType scope, uint count)
		{
			// ElementCount: UInt32
			return AudioUnitSetProperty (Handle, AudioUnitPropertyIDType.ElementCount, scope, 0, ref count, sizeof (uint));
		}

		public uint GetElementCount (AudioUnitScopeType scope)
		{
			// ElementCount: UInt32
			uint value = 0;
			uint size = sizeof (uint);
			var res = AudioUnitGetProperty (Handle, AudioUnitPropertyIDType.ElementCount, scope,
				0, ref value, ref size);

			if (res != 0)
				throw new AudioUnitException ((int) res);

			return value;
		}

		public AudioUnitStatus SetSampleRate (double sampleRate, AudioUnitScopeType scope = AudioUnitScopeType.Output, uint audioUnitElement = 0)
		{
			// ElementCount: Float64
			return AudioUnitSetProperty (Handle, AudioUnitPropertyIDType.SampleRate, scope, 0, ref sampleRate, sizeof (double));
		}

		public AudioUnitStatus MusicDeviceMIDIEvent (uint status, uint data1, uint data2, uint offsetSampleFrame = 0)
		{
			return MusicDeviceMIDIEvent (Handle, status, data1, data2, offsetSampleFrame);
		}

#if !NET
		[Obsolete ("This API has been removed.")]
		public AudioUnitStatus SetLatency (double latency)
		{
			return AudioUnitStatus.OK;
		}
#endif

		[DllImport (Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioUnitGetProperty (IntPtr inUnit, AudioUnitPropertyIDType inID, AudioUnitScopeType inScope, uint inElement, ref double outData, ref uint ioDataSize);

		public double GetLatency ()
		{
			uint size = sizeof (double);
			double latency = 0;
			var err = AudioUnitGetProperty (Handle, AudioUnitPropertyIDType.Latency, AudioUnitScopeType.Global, 0, ref latency, ref size);
			if (err != 0)
				throw new AudioUnitException ((int) err);
			return latency;
		}

		#region SetRenderCallback

		public AudioUnitStatus SetRenderCallback (RenderDelegate renderDelegate, AudioUnitScopeType scope = AudioUnitScopeType.Global, uint audioUnitElement = 0)
		{
			if (renderer is null)
				Interlocked.CompareExchange (ref renderer, new Dictionary<uint, RenderDelegate> (), null);

			renderer [audioUnitElement] = renderDelegate;

			if (!gcHandle.IsAllocated)
				gcHandle = GCHandle.Alloc (this);

			var cb = new AURenderCallbackStruct ();
#if NET
			unsafe {
				cb.Proc = &RenderCallbackImpl;
			}
#else
			cb.Proc = CreateRenderCallback;
#endif
			cb.ProcRefCon = GCHandle.ToIntPtr (gcHandle);
			return AudioUnitSetProperty (Handle, AudioUnitPropertyIDType.SetRenderCallback, scope, audioUnitElement, ref cb, Marshal.SizeOf<AURenderCallbackStruct> ());
		}

#if NET
		[UnmanagedCallersOnly]
		static unsafe AudioUnitStatus RenderCallbackImpl (IntPtr clientData,  AudioUnitRenderActionFlags* actionFlags, AudioTimeStamp* timeStamp, uint busNumber, uint numberFrames, IntPtr data)
#else
		[MonoPInvokeCallback (typeof (CallbackShared))]
		static AudioUnitStatus RenderCallbackImpl (IntPtr clientData, ref AudioUnitRenderActionFlags actionFlags, ref AudioTimeStamp timeStamp, uint busNumber, uint numberFrames, IntPtr data)
#endif
		{
			GCHandle gch = GCHandle.FromIntPtr (clientData);
			var au = (AudioUnit?) gch.Target;
			var renderer = au?.renderer;
			if (renderer is null)
				return AudioUnitStatus.Uninitialized;

			if (!renderer.TryGetValue (busNumber, out var render))
				return AudioUnitStatus.Uninitialized;

			using (var buffers = new AudioBuffers (data)) {
#if NET
				unsafe {
					return render (*actionFlags, *timeStamp, busNumber, numberFrames, buffers);
				}
#else
				return render (actionFlags, timeStamp, busNumber, numberFrames, buffers);
#endif
			}
		}

		#endregion

		#region SetInputCallback

		public AudioUnitStatus SetInputCallback (InputDelegate inputDelegate, AudioUnitScopeType scope = AudioUnitScopeType.Global, uint audioUnitElement = 0)
		{
			if (inputs is null)
				Interlocked.CompareExchange (ref inputs, new Dictionary<uint, InputDelegate> (), null);

			inputs [audioUnitElement] = inputDelegate;

			if (!gcHandle.IsAllocated)
				gcHandle = GCHandle.Alloc (this);

			var cb = new AURenderCallbackStruct ();
#if NET
			unsafe {
				cb.Proc = &InputCallbackImpl;
			}
#else
			cb.Proc = CreateInputCallback;
#endif
			cb.ProcRefCon = GCHandle.ToIntPtr (gcHandle);
			return AudioUnitSetProperty (Handle, AudioUnitPropertyIDType.SetInputCallback, scope, audioUnitElement, ref cb, Marshal.SizeOf<AURenderCallbackStruct> ());
		}
#if NET
		[UnmanagedCallersOnly]
		static unsafe AudioUnitStatus InputCallbackImpl (IntPtr clientData, AudioUnitRenderActionFlags* actionFlags, AudioTimeStamp* timeStamp, uint busNumber, uint numberFrames, IntPtr data)
#else
		[MonoPInvokeCallback (typeof (CallbackShared))]
		static AudioUnitStatus InputCallbackImpl (IntPtr clientData, ref AudioUnitRenderActionFlags actionFlags, ref AudioTimeStamp timeStamp, uint busNumber, uint numberFrames, IntPtr data)
#endif
		{
			GCHandle gch = GCHandle.FromIntPtr (clientData);
			var au = gch.Target as AudioUnit;
			if (au is null)
				return AudioUnitStatus.Uninitialized;

			var inputs = au.inputs;
			if (inputs is null)
				return AudioUnitStatus.Uninitialized;

			if (!inputs.TryGetValue (busNumber, out var input))
				return AudioUnitStatus.Uninitialized;
#if NET
			unsafe {
				return input (*actionFlags, *timeStamp, busNumber, numberFrames, au);
			}
#else
			return input (actionFlags, timeStamp, busNumber, numberFrames, au);
#endif
		}

		#endregion

#if !MONOMAC
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst14.0")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("tvos13.0")]
		[ObsoletedOSPlatform ("maccatalyst14.0")]
		[ObsoletedOSPlatform ("ios13.0")]
#else
		[Deprecated (PlatformName.iOS, 13, 0)]
		[Deprecated (PlatformName.TvOS, 13, 0)]
		[MacCatalyst (14, 0)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0)]
#endif
		[DllImport (Constants.AudioUnitLibrary)]
		static extern AudioComponentStatus AudioOutputUnitPublish (AudioComponentDescription inDesc, IntPtr /* CFStringRef */ inName, uint /* UInt32 */ inVersion, IntPtr /* AudioUnit */ inOutputUnit);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst14.0")]
		[SupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("tvos13.0", "Use 'AudioUnit' instead.")]
		[ObsoletedOSPlatform ("maccatalyst14.0", "Use 'AudioUnit' instead.")]
		[ObsoletedOSPlatform ("ios13.0", "Use 'AudioUnit' instead.")]
#else
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'AudioUnit' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'AudioUnit' instead.")]
		[MacCatalyst (14, 0)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'AudioUnit' instead.")]
#endif
		public AudioComponentStatus AudioOutputUnitPublish (AudioComponentDescription description, string name, uint version = 1)
		{

			if (name is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (name));

			var nameHandle = CFString.CreateNative (name);
			try {
				return AudioOutputUnitPublish (description, nameHandle, version, Handle);
			} finally {
				CFString.ReleaseNative (nameHandle);
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst14.0")]
		[SupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("tvos13.0")]
		[ObsoletedOSPlatform ("maccatalyst14.0")]
		[ObsoletedOSPlatform ("ios13.0")]
#else
		[MacCatalyst (14, 0)]
		[Deprecated (PlatformName.iOS, 13, 0)]
		[Deprecated (PlatformName.TvOS, 13, 0)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0)]
#endif
		[DllImport (Constants.AudioUnitLibrary)]
		static extern IntPtr AudioOutputUnitGetHostIcon (IntPtr /* AudioUnit */ au, float /* float */ desiredPointSize);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst14.0")]
		[SupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("tvos13.0", "Use 'AudioUnit' instead.")]
		[ObsoletedOSPlatform ("maccatalyst14.0", "Use 'AudioUnit' instead.")]
		[ObsoletedOSPlatform ("ios13.0", "Use 'AudioUnit' instead.")]
#else
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'AudioUnit' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'AudioUnit' instead.")]
		[MacCatalyst (14, 0)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'AudioUnit' instead.")]
#endif
		public UIKit.UIImage? GetHostIcon (float desiredPointSize)
		{
			return Runtime.GetNSObject<UIKit.UIImage> (AudioOutputUnitGetHostIcon (Handle, desiredPointSize));
		}
#endif

#if NET
		public AudioUnitStatus Initialize ()
		{
			return AudioUnitInitialize (Handle);
		}
#else
		public int Initialize ()
		{
			return (int) AudioUnitInitialize (Handle);
		}
#endif

#if NET
		public AudioUnitStatus Uninitialize ()
		{
			return AudioUnitUninitialize (Handle);
		}
#else
		public int Uninitialize ()
		{
			return (int) AudioUnitUninitialize (Handle);
		}
#endif

#if NET
		public AudioUnitStatus Start ()
#else
		public void Start ()
#endif
		{
			AudioUnitStatus rv = 0;
			if (!_isPlaying) {
				rv = AudioOutputUnitStart (Handle);
				_isPlaying = true;
			}
#if NET
			return rv;
#endif
		}

#if NET
		public AudioUnitStatus Stop ()
#else
		public void Stop ()
#endif
		{
			AudioUnitStatus rv = 0;
			if (_isPlaying) {
				rv = AudioOutputUnitStop (Handle);
				_isPlaying = false;
			}
#if NET
			return rv;
#endif
		}

		#region Render

		public AudioUnitStatus Render (ref AudioUnitRenderActionFlags actionFlags, AudioTimeStamp timeStamp, uint busNumber, uint numberFrames, AudioBuffers data)
		{
			if ((IntPtr) data == IntPtr.Zero)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (data));
			return AudioUnitRender (Handle, ref actionFlags, ref timeStamp, busNumber, numberFrames, (IntPtr) data);
		}

		#endregion

		public AudioUnitStatus SetParameter (AudioUnitParameterType type, float value, AudioUnitScopeType scope, uint audioUnitElement = 0)
		{
			return AudioUnitSetParameter (Handle, type, scope, audioUnitElement, value, 0);
		}

		public AudioUnitStatus ScheduleParameter (AudioUnitParameterEvent inParameterEvent, uint inNumParamEvents)
		{
			return AudioUnitScheduleParameters (Handle, inParameterEvent, inNumParamEvents);
		}

		[DllImport (Constants.AudioUnitLibrary)]
		static extern int AudioComponentInstanceDispose (IntPtr inInstance);

		protected override void Dispose (bool disposing)
		{
			if (Handle != IntPtr.Zero && Owns) {
				Stop ();
				AudioUnitUninitialize (Handle);
				AudioComponentInstanceDispose (Handle);
			}
			if (gcHandle.IsAllocated)
				gcHandle.Free ();
			base.Dispose (disposing);
		}

		[DllImport (Constants.AudioUnitLibrary, EntryPoint = "AudioComponentInstanceNew")]
		static extern AudioUnitStatus AudioComponentInstanceNew (IntPtr inComponent, out IntPtr inDesc);

		[DllImport (Constants.AudioUnitLibrary)]
		static extern IntPtr AudioComponentInstanceGetComponent (IntPtr inComponent);

		[DllImport (Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioUnitInitialize (IntPtr inUnit);

		[DllImport (Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioUnitUninitialize (IntPtr inUnit);

		[DllImport (Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioOutputUnitStart (IntPtr ci);

		[DllImport (Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioOutputUnitStop (IntPtr ci);

		[DllImport (Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioUnitRender (IntPtr inUnit, ref AudioUnitRenderActionFlags ioActionFlags, ref AudioTimeStamp inTimeStamp,
						  uint inOutputBusNumber, uint inNumberFrames, IntPtr ioData);

		[DllImport (Constants.AudioUnitLibrary)]
		static extern int AudioUnitSetProperty (IntPtr inUnit, AudioUnitPropertyIDType inID, AudioUnitScopeType inScope, uint inElement,
							   ref AudioStreamBasicDescription inData,
							   uint inDataSize);

		[DllImport (Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioUnitSetProperty (IntPtr inUnit, AudioUnitPropertyIDType inID, AudioUnitScopeType inScope, uint inElement,
							   ref uint inData, uint inDataSize);

		[DllImport (Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioUnitSetProperty (IntPtr inUnit, AudioUnitPropertyIDType inID, AudioUnitScopeType inScope, uint inElement,
							   ref double inData, uint inDataSize);

		[DllImport (Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioUnitSetProperty (IntPtr inUnit, AudioUnitPropertyIDType inID, AudioUnitScopeType inScope, uint inElement,
							   ref IntPtr inData, int inDataSize);

#if NET
		[DllImport (Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioUnitSetProperty (IntPtr inUnit, AudioUnitPropertyIDType inID, AudioUnitScopeType inScope, uint inElement,
							   ref NativeHandle inData, int inDataSize);
#endif

		[DllImport (Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioUnitSetProperty (IntPtr inUnit, AudioUnitPropertyIDType inID, AudioUnitScopeType inScope, uint inElement,
							   ref AURenderCallbackStruct inData, int inDataSize);

		[DllImport (Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioUnitSetProperty (IntPtr inUnit, AudioUnitPropertyIDType inID, AudioUnitScopeType inScope, uint inElement,
							   ref AudioUnitConnection inData, int inDataSize);

		[DllImport (Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioUnitSetProperty (IntPtr inUnit, AudioUnitPropertyIDType inID, AudioUnitScopeType inScope, uint inElement,
							   ref AUSamplerInstrumentData inData, int inDataSize);

		[DllImport (Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioUnitGetProperty (IntPtr inUnit, AudioUnitPropertyIDType inID, AudioUnitScopeType inScope, uint inElement,
							   ref AudioStreamBasicDescription outData,
							   ref uint ioDataSize);

		[DllImport (Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioUnitGetProperty (IntPtr inUnit, AudioUnitPropertyIDType inID, AudioUnitScopeType inScope, uint inElement,
							   ref IntPtr outData,
							   ref int ioDataSize);

		[DllImport (Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioUnitGetProperty (IntPtr inUnit, AudioUnitPropertyIDType inID, AudioUnitScopeType inScope, uint inElement,
						ref uint outData,
						ref int ioDataSize);

		[DllImport (Constants.AudioUnitLibrary)]
		static extern unsafe AudioUnitStatus AudioUnitGetProperty (IntPtr inUnit, AudioUnitPropertyIDType inID, AudioUnitScopeType inScope, uint inElement,
							   uint* outData,
							   ref uint ioDataSize);

		[DllImport (Constants.AudioUnitLibrary)]
		static extern unsafe AudioUnitStatus AudioUnitGetProperty (IntPtr inUnit, AudioUnitPropertyIDType inID, AudioUnitScopeType inScope, uint inElement,
							   ref AudioUnitParameterInfoNative outData,
							   ref uint ioDataSize);

		[DllImport (Constants.AudioUnitLibrary)]
		static extern int AudioUnitGetProperty (IntPtr inUnit,
							   [MarshalAs (UnmanagedType.U4)] AudioUnitPropertyIDType inID,
							   [MarshalAs (UnmanagedType.U4)] AudioUnitScopeType inScope,
							   [MarshalAs (UnmanagedType.U4)] uint inElement,
							   ref uint flag,
							   ref uint ioDataSize
			);


		[DllImport (Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioUnitGetPropertyInfo (IntPtr inUnit, AudioUnitPropertyIDType inID, AudioUnitScopeType inScope, uint inElement,
								out uint outDataSize, [MarshalAs (UnmanagedType.I1)] out bool outWritable);

		[DllImport (Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioUnitSetParameter (IntPtr inUnit, AudioUnitParameterType inID, AudioUnitScopeType inScope,
			uint inElement, float inValue, uint inBufferOffsetInFrames);

		[DllImport (Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioUnitScheduleParameters (IntPtr inUnit, AudioUnitParameterEvent inParameterEvent, uint inNumParamEvents);

#if MONOMAC || __MACCATALYST__
#if NET
		[SupportedOSPlatform ("maccatalyst15.0")]
		[SupportedOSPlatform ("macos")]
#else
		[MacCatalyst (15,0)]
#endif
		[DllImport (Constants.CoreAudioLibrary)]
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

		// TODO: https://github.com/xamarin/xamarin-macios/issues/12489
		// [TV (15,0), Mac (12,0), iOS (15,0), MacCatalyst (15,0)]
		// [DllImport (Constants.AudioUnitLibrary)]
		// static extern MusicDeviceMIDIEvent[] MusicDeviceMIDIEventList (IntPtr /* MusicDeviceComponent = void* */ inUnit, uint /* UInt32 */ inOffsetSampleFrame, MIDIEventList eventList);

		[DllImport (Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioUnitSetProperty (IntPtr inUnit, AudioUnitPropertyIDType inID, AudioUnitScopeType inScope, uint inElement,
			ref AUScheduledAudioFileRegion.ScheduledAudioFileRegion inData, int inDataSize);

		public AudioUnitStatus SetScheduledFileRegion (AUScheduledAudioFileRegion region)
		{
			if (region is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (region));

			var safr = region.GetAudioFileRegion ();
			return AudioUnitSetProperty (GetCheckedHandle (), AudioUnitPropertyIDType.ScheduledFileRegion, AudioUnitScopeType.Global, 0, ref safr, Marshal.SizeOf<AUScheduledAudioFileRegion.ScheduledAudioFileRegion> ());
		}

		[DllImport (Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioUnitSetProperty (IntPtr inUnit, AudioUnitPropertyIDType inID, AudioUnitScopeType inScope, uint inElement,
			ref AudioTimeStamp inData, int inDataSize);

		public AudioUnitStatus SetScheduleStartTimeStamp (AudioTimeStamp timeStamp)
		{
			return AudioUnitSetProperty (GetCheckedHandle (), AudioUnitPropertyIDType.ScheduleStartTimeStamp, AudioUnitScopeType.Global, 0, ref timeStamp, Marshal.SizeOf<AudioTimeStamp> ());
		}

		public AudioUnitStatus SetScheduledFiles (AudioFile audioFile)
		{
			if (audioFile is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (audioFile));

			var audioFilehandle = audioFile.Handle;
			return AudioUnitSetProperty (GetCheckedHandle (), AudioUnitPropertyIDType.ScheduledFileIDs, AudioUnitScopeType.Global, 0, ref audioFilehandle, Marshal.SizeOf<NativeHandle> ());
		}

		[DllImport (Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioUnitSetProperty (IntPtr inUnit, AudioUnitPropertyIDType inID, AudioUnitScopeType inScope, uint inElement,
			IntPtr inData, int inDataSize);

		public unsafe AudioUnitStatus SetScheduledFiles (AudioFile [] audioFiles)
		{
			if (audioFiles is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (audioFiles));

			int count = audioFiles.Length;
			IntPtr [] handles = new IntPtr [count];
			for (int i = 0; i < count; i++)
				handles [i] = audioFiles [i].Handle;

			fixed (IntPtr* ptr = handles)
				return AudioUnitSetProperty (GetCheckedHandle (), AudioUnitPropertyIDType.ScheduledFileIDs, AudioUnitScopeType.Global, 0, (IntPtr) ptr, IntPtr.Size * count);
		}

#endif // !COREBUILD
	}

#if !XAMCORE_3_0 || MONOMAC || __MACCATALYST__
	[StructLayout (LayoutKind.Sequential)]
	struct AudioObjectPropertyAddress {
#if !COREBUILD
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
#endif // !COREBUILD
	}
#endif // !XAMCORE_3_0 || MONOMAC || __MACCATALYST__

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public unsafe class AURenderEventEnumerator : INativeObject
#if COREBUILD
	{}
#else
	, IEnumerator<AURenderEvent> {
		AURenderEvent* current;

		public NativeHandle Handle { get; private set; }
		public bool IsEmpty { get { return Handle == IntPtr.Zero; } }
		public bool IsAtEnd { get { return current is null; } }

		public AURenderEventEnumerator (NativeHandle ptr)
			: this (ptr, false)
		{
		}

		[Preserve (Conditional = true)]
		internal AURenderEventEnumerator (NativeHandle handle, bool owns)
		{
			Handle = handle;
			current = (AURenderEvent*) (IntPtr) handle;
		}

		public void Dispose ()
		{
			Handle = NativeHandle.Zero;
			current = null;
		}

		public AURenderEvent* UnsafeFirst {
			get {
				return (AURenderEvent*) (IntPtr) Handle;
			}
		}

		public AURenderEvent First {
			get {
				if (IsEmpty)
					throw new InvalidOperationException ("Can not get First on AURenderEventEnumerator when empty");
				return *(AURenderEvent*) (IntPtr) Handle;
			}
		}

		public AURenderEvent Current {
			get {
				if (IsAtEnd)
					throw new InvalidOperationException ("Can not get Current on AURenderEventEnumerator when at end");
				return *current;
			}
		}

		object IEnumerator.Current {
			get { return Current; }
		}

		bool IsAt (nint now)
		{
			return current is not null && (current->Head.EventSampleTime == now);
		}

		public IEnumerable<AURenderEvent> EnumeratorCurrentEvents (nint now)
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
			if (current is not null)
				current = ((AURenderEvent*) current)->Head.UnsafeNext;
			return current is not null;
		}

		public void /*IEnumerator<AURenderEvent>.*/Reset ()
		{
			current = (AURenderEvent*) (IntPtr) Handle;
		}
	}
#endif // !COREBUILD

	public enum AURenderEventType : byte {
		Parameter = 1,
		ParameterRamp = 2,
		Midi = 8,
		MidiSysEx = 9,
#if NET
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos12.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[iOS (15, 0)]
		[TV (15, 0)]
		[Mac (12, 0)]
		[MacCatalyst (15, 0)]
#endif
		MidiEventList = 10,
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public unsafe struct AURenderEventHeader {
		public AURenderEvent* UnsafeNext;

		public AURenderEvent? Next {
			get {
				if (UnsafeNext is not null)
					return Marshal.PtrToStructure<AURenderEvent> ((IntPtr) UnsafeNext);
				return null;
			}
		}

		public long EventSampleTime;

		public AURenderEventType EventType;

		public byte Reserved;
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct AUParameterObserverToken {
		public IntPtr ObserverToken;
		public AUParameterObserverToken (IntPtr observerToken)
		{
			ObserverToken = observerToken;
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public unsafe struct AUParameterEvent {
		public AURenderEvent* UnsafeNext;

		public AURenderEvent? Next {
			get {
				if (UnsafeNext is not null)
					return Marshal.PtrToStructure<AURenderEvent> ((IntPtr) UnsafeNext);
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
	//				if (UnsafeNext is not null)
	//					return Marshal.PtrToStructure<AURenderEvent> ((IntPtr)UnsafeNext);
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

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Explicit)]
	public struct AURenderEvent {
		[FieldOffset (0)]
		public AURenderEventHeader Head;

		[FieldOffset (0)]
		public AUParameterEvent Parameter;

		// 		[FieldOffset (0)]
		// 		public AUMidiEvent Midi;
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct AURecordedParameterEvent {
		public ulong HostTime;

		public ulong Address;

		public float Value;
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct AUParameterAutomationEvent {
		public ulong HostTime;
		public ulong Address;
		public float Value;
#if COREBUILD
		// keep structure size identical across builds
		public uint EventType;
#else
		public AUParameterAutomationEventType EventType;
#endif
		ulong Reserved;
	}

#if !COREBUILD
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	//	Configuration Info Keys
	public static class AudioUnitConfigurationInfo {
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

#if !NET && !COREBUILD
#if !MONOMAC
	[Obsolete ("Use 'AUImplementorStringFromValueCallback' instead.")]
	public delegate NSString _AUImplementorStringFromValueCallback (AUParameter param, IntPtr value);
#endif
#endif
}
