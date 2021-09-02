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
using System.Runtime.Versioning;
using System.Threading;
using AudioToolbox;
using ObjCRuntime;
using CoreFoundation;
using Foundation;

namespace AudioUnit
{
#if !COREBUILD
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

	[StructLayout(LayoutKind.Sequential)]
	unsafe struct AudioUnitParameterInfoNative // AudioUnitParameterInfo in Obj-C
	{
		fixed byte /* char[52] */ name[52]; // unused
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
		}

		public AudioComponent Component {
			get {
				return new AudioComponent (AudioComponentInstanceGetComponent (handle));
			}
		}

		public bool IsPlaying { get { return _isPlaying; } }
		

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
#if NET
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[MacCatalyst (15,0)]
#endif
		public static uint GetCurrentInputDevice ()
		{
#if MONOMAC || __MACCATALYST__
			// We need to replace AudioHardwareGetProperty since it has been deprecated since OS X 10.6 and iOS 2.0
			// Replacing with the following implementation recommended in the following doc
			// See Listing 4  New - Getting the default input device.
			// https://developer.apple.com/library/mac/technotes/tn2223/_index.html

			uint inputDevice;
			uint size = (uint) Marshal.SizeOf (typeof (uint));
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
			size = (uint)sizeof (AudioUnitParameterInfoNative);

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

#if !XAMCORE_4_0
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
#if !NET
		[iOS (7,0)]
		[Deprecated (PlatformName.iOS, 13,0)]
		[MacCatalyst (14,0)]
#else
		[UnsupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#endif
		[DllImport (Constants.AudioUnitLibrary)]
		static extern AudioComponentStatus AudioOutputUnitPublish (AudioComponentDescription inDesc, IntPtr /* CFStringRef */ inName, uint /* UInt32 */ inVersion, IntPtr /* AudioUnit */ inOutputUnit);

#if !NET
		[iOS (7,0)]
		[Deprecated (PlatformName.iOS, 13,0, message: "Use 'AudioUnit' instead.")]
		[MacCatalyst (14,0)][Deprecated (PlatformName.MacCatalyst, 14,0, message: "Use 'AudioUnit' instead.")]
#else
		[UnsupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#if IOS
		[Obsolete ("Starting with ios13.0 use 'AudioUnit' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif __MACCATALYST__
		[Obsolete ("Starting with maccatalyst14.0 use 'AudioUnit' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
		public AudioComponentStatus AudioOutputUnitPublish (AudioComponentDescription description, string name, uint version = 1)
		{

			if (name == null)
				throw new ArgumentNullException ("name");
				
			using (CFString n = name) {
				return AudioOutputUnitPublish (description, n.Handle, version, handle);
			}
		}

#if !NET
		[iOS (7,0)]
		[MacCatalyst (14,0)]
		[Deprecated (PlatformName.iOS, 13,0)]
#else
		[SupportedOSPlatform ("maccatalyst14.0")]
		[UnsupportedOSPlatform ("ios13.0")]
#endif
		[DllImport (Constants.AudioUnitLibrary)]
		static extern IntPtr AudioOutputUnitGetHostIcon (IntPtr /* AudioUnit */ au, float /* float */ desiredPointSize);

#if !NET
		[iOS (7,0)]
		[Deprecated (PlatformName.iOS, 13,0, message: "Use 'AudioUnit' instead.")]
		[MacCatalyst (14,0)][Deprecated (PlatformName.MacCatalyst, 14,0, message: "Use 'AudioUnit' instead.")]
#else
		[SupportedOSPlatform ("maccatalyst14.0")]
		[UnsupportedOSPlatform ("ios13.0")]
#if IOS
		[Obsolete ("Starting with ios13.0 use 'AudioUnit' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif __MACCATALYST__
		[Obsolete ("Starting with maccatalyst14.0 use 'AudioUnit' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
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

		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero){
				Stop ();
				AudioUnitUninitialize (handle);
				AudioComponentInstanceDispose (handle);
				gcHandle.Free();
				handle = IntPtr.Zero;
			}
		}

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

		[DllImport(Constants.AudioUnitLibrary)]
		static extern AudioUnitStatus AudioUnitRender(IntPtr inUnit, ref AudioUnitRenderActionFlags ioActionFlags, ref AudioTimeStamp inTimeStamp,
						  uint inOutputBusNumber, uint inNumberFrames, IntPtr ioData);

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

#if MONOMAC || __MACCATALYST__
#if !NET
		[MacCatalyst (15,0)]
#else
		[SupportedOSPlatform ("maccatalyst15.0")]
#endif
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

		// TODO: https://github.com/xamarin/xamarin-macios/issues/12489
		// [TV (15,0), Mac (12,0), iOS (15,0), MacCatalyst (15,0)]
		// [DllImport (Constants.AudioUnitLibrary)]
		// static extern MusicDeviceMIDIEvent[] MusicDeviceMIDIEventList (IntPtr /* MusicDeviceComponent = void* */ inUnit, uint /* UInt32 */ inOffsetSampleFrame, MIDIEventList eventList);

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
#endif // !XAMCORE_3_0 || MONOMAC

	public unsafe class AURenderEventEnumerator : INativeObject
#if COREBUILD
	{}
#else
	, IEnumerator<AURenderEvent>
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
#endif // !COREBUILD

	public enum AURenderEventType : byte
	{
		Parameter = 1,
		ParameterRamp = 2,
		Midi = 8,
		MidiSysEx = 9,
#if !NET
		[iOS (15,0), TV (15,0), Mac (12,0), MacCatalyst (15,0)]
#else
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
		[SupportedOSPlatform ("macos12.0")]
#endif
		MidiEventList  = 10,
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

#if !NET
	[iOS (10,0), Mac (10,12)]
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

#if !XAMCORE_4_0 && !COREBUILD
#if !MONOMAC
	[Obsolete ("Use 'AUImplementorStringFromValueCallback' instead.")]
	public delegate NSString _AUImplementorStringFromValueCallback (AUParameter param, IntPtr value);
#endif
#endif
}
