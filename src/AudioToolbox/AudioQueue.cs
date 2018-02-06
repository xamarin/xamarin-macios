// 
// AudioQueue.cs:
//
// Authors:
//    Miguel de Icaza (miguel@xamarin.com)
//    Marek Safar (marek.safar@gmail.com)
//     
// Copyright 2009 Novell, Inc
// Copyright 2011-2013 Xamarin Inc.
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
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CoreFoundation;
using Foundation;
using ObjCRuntime;

using OSStatus = System.Int32;
using AudioQueueParameterValue = System.Single;
using AudioQueueRef = System.IntPtr;
using AudioQueueTimelineRef = System.IntPtr;

namespace AudioToolbox {

	public enum AudioQueueStatus { // Implictly cast to OSType 
		Ok =  0,
		InvalidBuffer        = -66687,
		BufferEmpty          = -66686,
		DisposalPending      = -66685,
		InvalidProperty      = -66684,
		InvalidPropertySize  = -66683,
		InvalidParameter     = -66682,
		CannotStart          = -66681,
		InvalidDevice        = -66680,
		BufferInQueue        = -66679,
		InvalidRunState      = -66678,
		InvalidQueueType     = -66677,
		Permissions          = -66676,
		InvalidPropertyValue = -66675,
		PrimeTimedOut        = -66674,
		CodecNotFound        = -66673,
		InvalidCodecAccess   = -66672,
		QueueInvalidated     = -66671,
		TooManyTaps          = -66670,
		InvalidTapContext    = -66669,
		RecordUnderrun       = -66668,
		InvalidTapType       = -66667,		
		EnqueueDuringReset   = -66632,
		InvalidOfflineMode   = -66626,
		BufferEnqueuedTwice  = -66666,
		[iOS (10,0), Mac (10,12, onlyOn64: true)]
		CannotStartYet       = -66665,
		
		// There is countless of not well documented error codes returned
		QueueStopped         = 0x73746f70,	// 'stop'
		DataFormatError      = 0x666d743f,	// 'fmt?'
		UnsupportedProperty  = 0x70726F70,  // 'prop'

		// From kAudio_
		GeneralParamError    = -50
	}

	public class AudioQueueException : Exception {
		static string Lookup (int k)
		{
			var status = (AudioQueueStatus)k;
			switch (status) {
			case AudioQueueStatus.InvalidBuffer:
				return "The specified audio queue buffer does not belong to this audio queue";
				
			case AudioQueueStatus.BufferEmpty:
				return "The audio buffer is empty (AudioDataByteSize is zero)";
				
			case AudioQueueStatus.DisposalPending:
				return "This audio queue is being asynchronously disposed";
				
			case AudioQueueStatus.InvalidProperty:
				return "Invalid property specified";
				
			case AudioQueueStatus.InvalidPropertySize:
				return "Invalid property size";
				
			case AudioQueueStatus.InvalidParameter:
				return "The specified parameter is invalid";
				
			case AudioQueueStatus.CannotStart:
				return "The queue has encountered a problem and can not start";
				
			case AudioQueueStatus.InvalidDevice:
				return "The specified hardware device can not be located";
				
			case AudioQueueStatus.BufferInQueue:
				return "The specified buffer can not be disposed while it is on an active queue";
				
			case AudioQueueStatus.InvalidRunState:
				return "The operation can not be performed in the current queue state (running or stopped)";
				
			case AudioQueueStatus.InvalidQueueType:
				return "Invalid queue type (input operation attempted on output or output operation on input";
			
			case AudioQueueStatus.Permissions:
				return "No permissions to access that function";
				
			case AudioQueueStatus.InvalidPropertyValue:
				return "The property value is invalid";
				
			case AudioQueueStatus.PrimeTimedOut:
				return "Timeout during Prime operation";
				
			case AudioQueueStatus.CodecNotFound:
				return "The requested codec was not found";
				
			case AudioQueueStatus.InvalidCodecAccess:
				return "The codec could not be accessed";
				
			case AudioQueueStatus.QueueInvalidated:
				return "The audio server has terminated, the queue has been invalidated";

			case AudioQueueStatus.RecordUnderrun:
				return "Recording lost data because enqueued buffer was not available";
				
			case AudioQueueStatus.EnqueueDuringReset:
				return "You tried to enqueue a buffer during a Reset, Stop or Dispose methods";
			
			case AudioQueueStatus.InvalidOfflineMode:
				return "Offline mode is either required or not required for the operation";

			case AudioQueueStatus.GeneralParamError:
				return "Error in user parameter list";

			default:
				return String.Format ("Error code: {0}", status.ToString ());
			}
		}

		internal AudioQueueException (AudioQueueStatus k) : base (Lookup ((int)k))
		{
			ErrorCode = k;
		}
		
		internal AudioQueueException (int k) : base (Lookup (k))
		{
			ErrorCode = (AudioQueueStatus) k;
		}

		public AudioQueueStatus ErrorCode { get; private set; }
	}
	
	public enum AudioQueueProperty : uint // UInt32 AudioQueuePropertyID
	{ 
		IsRunning = 0x6171726e,
		DeviceSampleRate = 0x61717372,			// 'aqsr'
		DeviceNumberChannels = 0x61716463,
		CurrentDevice = 0x61716364,
		MagicCookie = 0x61716d63,
		MaximumOutputPacketSize = 0x786f7073,	// 'xops'
		StreamDescription = 0x61716674,			// 'aqft'
		ChannelLayout = 0x6171636c,				// 'aqcl'
		EnableLevelMetering = 0x61716d65,
		CurrentLevelMeter = 0x61716d76,
		CurrentLevelMeterDB = 0x61716d64,
		DecodeBufferSizeFrames = 0x64636266, 
		ConverterError = 0x71637665,			// 'qcve'
		EnableTimePitch         = 0x715f7470,	// 'q_tp'
		TimePitchAlgorithm      = 0x71747061,	// 'qtpa'
		TimePitchBypass         = 0x71747062,	// 'qtpb'
#if !MONOMAC
		HardwareCodecPolicy		= 0x61716370,	// 'aqcp'
		ChannelAssignments		= 0x61716361,	// 'aqca'
#endif
	}

	public enum AudioQueueTimePitchAlgorithm : uint {
		Spectral = 0x73706563,					// spec
		TimeDomain = 0x7469646f,				// tido
#if !MONOMAC
		LowQualityZeroLatency = 0x6c717a6c,		// lqzl
#endif
		Varispeed = 0x76737064					// vspd
	}

	public enum AudioQueueHardwareCodecPolicy { // A AudioQueuePropertyID (UInt32)
		Default = 0,
		UseSoftwareOnly = 1,
		UseHardwareOnly = 2,
		PreferSoftware = 3,
		PreferHardware = 4
	}

	public enum AudioQueueParameter : uint // UInt32 AudioQueueParameterID
	{
		Volume = 1,
		PlayRate = 2,
		Pitch = 3,
		VolumeRampTime = 4,
		Pan = 13,
	}
	
	public enum AudioQueueDeviceProperty { // UInt32 AudioQueueParameterID
		SampleRate = 0x61717372,
		NumberChannels = 0x61716463
	}

	[Flags]
	public enum AudioQueueProcessingTapFlags : uint // UInt32 in AudioQueueProcessingTapNew
	{
		PreEffects         = (1 << 0),
		PostEffects        = (1 << 1),
		Siphon             = (1 << 2),

		StartOfStream      = (1 << 8),
		EndOfStream        = (1 << 9),
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public struct AudioQueueBuffer {
		public uint AudioDataBytesCapacity;
		public IntPtr AudioData;
		public uint AudioDataByteSize;
		public IntPtr UserData;

		public uint PacketDescriptionCapacity;
		public IntPtr IntPtrPacketDescriptions;
		public int PacketDescriptionCount;

		public AudioStreamPacketDescription [] PacketDescriptions {
			get {
				return AudioFile.PacketDescriptionFrom (PacketDescriptionCount, IntPtrPacketDescriptions);
			}
		}

		public unsafe void CopyToAudioData (IntPtr source, int size)
		{
			byte *t = (byte *) AudioData;
			byte *s = (byte *) source;
			Runtime.memcpy (t, s, size);
		}
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct AudioQueueParameterEvent {
		[FieldOffset(0)]
		[Advice ("Use Parameter.")]
		public uint ID;

		[FieldOffset(0)] 
		public AudioQueueParameter Parameter;

		[FieldOffset(4)] 
		public float Value;

		public AudioQueueParameterEvent (AudioQueueParameter parameter, float value)
		{
			this.ID = (uint) parameter;
			this.Parameter = parameter;
			this.Value = value;
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct AudioQueueLevelMeterState {
		public float AveragePower;
		public float PeakPower;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct AudioQueueChannelAssignment
	{
		IntPtr deviceUID; // CFString
		uint channelNumber;

		public AudioQueueChannelAssignment (CFString deviceUID, uint channelNumber)
		{
			this.deviceUID = deviceUID.Handle;
			this.channelNumber = channelNumber;
		}
	}

	delegate void AudioQueueOutputCallback (IntPtr userData, IntPtr AQ, IntPtr audioQueueBuffer);
	unsafe delegate void AudioQueueInputCallback (IntPtr userData, IntPtr AQ, IntPtr audioQueueBuffer,
						      AudioTimeStamp *startTime, int descriptors, IntPtr AudioStreamPacketDescription_inPacketDesc);
	delegate void AudioQueuePropertyListener (IntPtr userData, IntPtr AQ,  AudioQueueProperty id);

#if XAMCORE_2_0
	public class BufferCompletedEventArgs : EventArgs {
		public BufferCompletedEventArgs (IntPtr audioQueueBuffer)
		{
			IntPtrBuffer = audioQueueBuffer;
		}

		public unsafe BufferCompletedEventArgs (AudioQueueBuffer *audioQueueBuffer)
		{
			IntPtrBuffer = (IntPtr) audioQueueBuffer;
		}
#else
	public class OutputCompletedEventArgs : EventArgs {
		public OutputCompletedEventArgs (IntPtr audioQueueBuffer)
		{
			IntPtrBuffer = audioQueueBuffer;
		}

		public unsafe OutputCompletedEventArgs (AudioQueueBuffer *audioQueueBuffer)
		{
			IntPtrBuffer = (IntPtr) audioQueueBuffer;
		}
#endif

		public IntPtr IntPtrBuffer { get; private set; }
		public unsafe AudioQueueBuffer *UnsafeBuffer {
			get { return (AudioQueueBuffer *) IntPtrBuffer; }
			set { IntPtrBuffer = (IntPtr) value; }
		}
	}

	public class InputCompletedEventArgs : EventArgs {
		public unsafe InputCompletedEventArgs (IntPtr audioQueueBuffer, AudioTimeStamp timeStamp, AudioStreamPacketDescription [] pdec)
		{
			IntPtrBuffer = audioQueueBuffer;
			TimeStamp = timeStamp;
			PacketDescriptions = pdec;
		}

		public IntPtr IntPtrBuffer { get; private set; }
		public unsafe AudioQueueBuffer *UnsafeBuffer {
			get { return (AudioQueueBuffer *) IntPtrBuffer; }
			set { IntPtrBuffer = (IntPtr) value; }
		}
		public unsafe AudioQueueBuffer Buffer {
			get { return *(AudioQueueBuffer *) IntPtrBuffer; }
		}
		public AudioTimeStamp TimeStamp { get; private set; }
		public AudioStreamPacketDescription [] PacketDescriptions { get; private set; }
	}

	public abstract class AudioQueue : IDisposable {
		internal protected IntPtr handle;
		internal protected GCHandle gch;

		public IntPtr Handle { get { return handle; } }

		internal AudioQueue ()
		{
		}

		~AudioQueue ()
		{
			Dispose (false, true);
		}
		
		public void Dispose ()
		{
			Dispose (true, true);
			GC.SuppressFinalize (this);
		}

		public void QueueDispose ()
		{
			Dispose (true, false);
		}
		
		[DllImport (Constants.AudioToolboxLibrary)]
		extern static OSStatus AudioQueueDispose (IntPtr AQ, bool immediate);

#if XAMCORE_2_0
		protected virtual void Dispose (bool disposing)
		{
			Dispose (disposing, true);
		}

		void Dispose (bool disposing, bool immediate)
#else
		public virtual void Dispose (bool disposing, bool immediate)
#endif
		{
			if (handle != IntPtr.Zero){
				if (disposing){
					if (listeners != null){
						foreach (AudioQueueProperty prop in listeners.Keys){
							AudioQueueRemovePropertyListener (handle, prop, property_changed, GCHandle.ToIntPtr (gch));
						}
					}
				}
			
				AudioQueueDispose (handle, immediate);
				handle = IntPtr.Zero;
			}
			
			if (gch.IsAllocated)
				gch.Free ();
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static AudioQueueStatus AudioQueueStart (IntPtr AQ, ref AudioTimeStamp startTime);

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static AudioQueueStatus AudioQueueStart (IntPtr AQ, IntPtr startTime);
		
		public AudioQueueStatus Start (AudioTimeStamp startTime)
		{
			return AudioQueueStart (handle, ref startTime);
		}

		public AudioQueueStatus Start ()
		{
			return AudioQueueStart (handle, IntPtr.Zero);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static AudioQueueStatus AudioQueuePrime (IntPtr AQ, int toPrepare, out int prepared);
		public AudioQueueStatus Prime (int toPrepare, out int prepared)
		{
			return AudioQueuePrime (handle, toPrepare, out prepared);
		}
		
		[DllImport (Constants.AudioToolboxLibrary)]
		extern static AudioQueueStatus AudioQueueFlush (IntPtr aq);
		public AudioQueueStatus Flush ()
		{
			return AudioQueueFlush (handle);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static AudioQueueStatus AudioQueueStop (IntPtr aq, bool immediate);
		public AudioQueueStatus Stop (bool immediate)
		{
			return AudioQueueStop (handle, immediate);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static AudioQueueStatus AudioQueuePause (IntPtr aq);
		public AudioQueueStatus Pause ()
		{
			return AudioQueuePause (handle);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static AudioQueueStatus AudioQueueReset (IntPtr aq);
		public AudioQueueStatus Reset ()
		{
			return AudioQueueReset (handle);
		}
		
		[DllImport (Constants.AudioToolboxLibrary)]
		extern static AudioQueueStatus AudioQueueAllocateBuffer (AudioQueueRef AQ, int bufferSize, out IntPtr audioQueueBuffer);
		public AudioQueueStatus AllocateBuffer (int bufferSize, out IntPtr audioQueueBuffer)
		{
			return AudioQueueAllocateBuffer (handle, bufferSize, out audioQueueBuffer);
		}

		public unsafe AudioQueueStatus AllocateBuffer (int bufferSize, out AudioQueueBuffer* audioQueueBuffer)
		{
			IntPtr buf;
			AudioQueueStatus result;
			result = AudioQueueAllocateBuffer (handle, bufferSize, out buf);
			audioQueueBuffer = (AudioQueueBuffer *) buf;
			return result;
		}
		
		[DllImport (Constants.AudioToolboxLibrary)]
		extern static AudioQueueStatus AudioQueueAllocateBufferWithPacketDescriptions(IntPtr AQ, int bufferSize, int nPackets, out IntPtr audioQueueBuffer);
		public AudioQueueStatus AllocateBufferWithPacketDescriptors (int bufferSize, int nPackets, out IntPtr audioQueueBuffer)
		{
			return AudioQueueAllocateBufferWithPacketDescriptions (handle, bufferSize, nPackets, out audioQueueBuffer);
		}
		
		[DllImport (Constants.AudioToolboxLibrary)]
		extern static AudioQueueStatus AudioQueueFreeBuffer (IntPtr AQ, IntPtr audioQueueBuffer);
		public void FreeBuffer (IntPtr audioQueueBuffer)
		{
			if (audioQueueBuffer == IntPtr.Zero)
				throw new ArgumentNullException ("audioQueueBuffer");
			AudioQueueFreeBuffer (handle, audioQueueBuffer);
		}
		
		public static void FillAudioData (IntPtr audioQueueBuffer, int offset, IntPtr source, int sourceOffset, nint size)
		{
			IntPtr target = Marshal.ReadIntPtr (audioQueueBuffer, IntPtr.Size);
			unsafe {
				byte *targetp = (byte *) target;
				byte *sourcep = (byte *) source;
				Runtime.memcpy (targetp + offset, sourcep + sourceOffset, size);
			}
		}
		
		[DllImport (Constants.AudioToolboxLibrary)]
		internal extern unsafe static AudioQueueStatus AudioQueueEnqueueBuffer (IntPtr AQ, AudioQueueBuffer* audioQueueBuffer, int nPackets, AudioStreamPacketDescription [] desc);

		public AudioQueueStatus EnqueueBuffer (IntPtr audioQueueBuffer, int bytes, AudioStreamPacketDescription [] desc)
		{
			if (audioQueueBuffer == IntPtr.Zero)
				throw new ArgumentNullException ("audioQueueBuffer");

			unsafe {
				AudioQueueBuffer *buffer = (AudioQueueBuffer *) audioQueueBuffer;
				buffer->AudioDataByteSize = (uint) bytes;
				return EnqueueBuffer (buffer, desc);
			}
		}

		public unsafe AudioQueueStatus EnqueueBuffer (AudioQueueBuffer* audioQueueBuffer, AudioStreamPacketDescription [] desc)
		{
			if (audioQueueBuffer == null)
				throw new ArgumentNullException ("audioQueueBuffer");

			return AudioQueueEnqueueBuffer (handle, audioQueueBuffer, desc == null ? 0 : desc.Length, desc);
		}

		public unsafe AudioQueueStatus EnqueueBuffer (IntPtr audioQueueBuffer, AudioStreamPacketDescription [] desc)
		{
			if (audioQueueBuffer == IntPtr.Zero)
				throw new ArgumentNullException ("audioQueueBuffer");

			return AudioQueueEnqueueBuffer (handle, (AudioQueueBuffer *) audioQueueBuffer, desc == null ? 0 : desc.Length, desc);
		}
		
		[DllImport (Constants.AudioToolboxLibrary)]
		extern unsafe static AudioQueueStatus AudioQueueEnqueueBufferWithParameters (
			IntPtr AQ,
			AudioQueueBuffer *audioQueueBuffer,
			int nPackets,
			AudioStreamPacketDescription [] desc,
			int trimFramesAtStart,
			int trimFramesAtEnd,
			int nParam,
			AudioQueueParameterEvent      [] parameterEvents,
			ref AudioTimeStamp  startTime,
			out AudioTimeStamp actualStartTime);

		[DllImport (Constants.AudioToolboxLibrary)]
		extern unsafe static AudioQueueStatus AudioQueueEnqueueBufferWithParameters (
			IntPtr AQ,
			AudioQueueBuffer *audioQueueBuffer,
			int nPackets,
			AudioStreamPacketDescription [] desc,
			int trimFramesAtStart,
			int trimFramesAtEnd,
			int nParam,
			AudioQueueParameterEvent      [] parameterEvents,
			AudioTimeStamp *startTime,
			out AudioTimeStamp actualStartTime);

		public AudioQueueStatus EnqueueBuffer (IntPtr audioQueueBuffer, int bytes, AudioStreamPacketDescription [] desc,
						       int trimFramesAtStart, int trimFramesAtEnd, AudioQueueParameterEvent [] parameterEvents,
						       ref AudioTimeStamp startTime, out AudioTimeStamp actualStartTime)
		{
			if (audioQueueBuffer == IntPtr.Zero)
				throw new ArgumentNullException ("audioQueueBuffer");

			unsafe {
				AudioQueueBuffer *buffer = (AudioQueueBuffer *) audioQueueBuffer;
				buffer->AudioDataByteSize = (uint) bytes;

				return AudioQueueEnqueueBufferWithParameters (
					handle, buffer, desc == null ? 0 : desc.Length, desc,
					trimFramesAtStart, trimFramesAtEnd, parameterEvents == null ? 0 : parameterEvents.Length,
					parameterEvents,
					ref startTime,
					out actualStartTime);
			}
		}
		public AudioQueueStatus EnqueueBuffer (IntPtr audioQueueBuffer, int bytes, AudioStreamPacketDescription [] desc,
						       int trimFramesAtStart, int trimFramesAtEnd, AudioQueueParameterEvent [] parameterEvents,
						       out AudioTimeStamp actualStartTime)
		{
			if (audioQueueBuffer == IntPtr.Zero)
				throw new ArgumentNullException ("audioQueueBuffer");

			unsafe {
				AudioQueueBuffer *buffer = (AudioQueueBuffer *) audioQueueBuffer;
				buffer->AudioDataByteSize = (uint) bytes;

				return AudioQueueEnqueueBufferWithParameters (
					handle, buffer, desc == null ? 0 : desc.Length, desc,
					trimFramesAtStart, trimFramesAtEnd, parameterEvents == null ? 0 : parameterEvents.Length,
					parameterEvents,
					null,
					out actualStartTime);
			}
		}
		
		public unsafe AudioQueueStatus EnqueueBuffer (AudioQueueBuffer *audioQueueBuffer, int bytes, AudioStreamPacketDescription [] desc,
						       int trimFramesAtStart, int trimFramesAtEnd, AudioQueueParameterEvent [] parameterEvents,
						       ref AudioTimeStamp startTime, out AudioTimeStamp actualStartTime)
		{
			if (audioQueueBuffer == null)
				throw new ArgumentNullException ("audioQueueBuffer");

			return AudioQueueEnqueueBufferWithParameters (
				handle, audioQueueBuffer, desc == null ? 0 : desc.Length, desc,
				trimFramesAtStart, trimFramesAtEnd, parameterEvents == null ? 0 : parameterEvents.Length,
				parameterEvents,
				ref startTime,
				out actualStartTime);
		}
		
		public unsafe AudioQueueStatus EnqueueBuffer (AudioQueueBuffer *audioQueueBuffer, int bytes, AudioStreamPacketDescription [] desc,
						       int trimFramesAtStart, int trimFramesAtEnd, AudioQueueParameterEvent [] parameterEvents,
						       out AudioTimeStamp actualStartTime)
		{
			if (audioQueueBuffer == null)
				throw new ArgumentNullException ("audioQueueBuffer");

			return AudioQueueEnqueueBufferWithParameters (
				handle, audioQueueBuffer, desc == null ? 0 : desc.Length, desc,
				trimFramesAtStart, trimFramesAtEnd, parameterEvents == null ? 0 : parameterEvents.Length,
				parameterEvents,
				null,
				out actualStartTime);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static AudioQueueStatus AudioQueueCreateTimeline (IntPtr AQ, out IntPtr timeline);

		public AudioQueueTimeline CreateTimeline ()
		{
			IntPtr thandle;
			
			if (AudioQueueCreateTimeline (handle, out thandle) == AudioQueueStatus.Ok)
				return new AudioQueueTimeline (handle, thandle);
			return null;
		}
		
		[DllImport (Constants.AudioToolboxLibrary)]
		extern static AudioQueueStatus AudioQueueGetCurrentTime (IntPtr AQ, IntPtr timelineHandle, ref AudioTimeStamp time, ref bool discontinuty);

		public AudioQueueStatus GetCurrentTime (AudioQueueTimeline timeline, ref AudioTimeStamp time, ref bool timelineDiscontinuty)
		{
			IntPtr arg;
			if (timeline == null)
				arg = IntPtr.Zero;
			else {
				arg = timeline.timelineHandle;
				if (arg == IntPtr.Zero)
					throw new ObjectDisposedException ("timeline");
			}

			return AudioQueueGetCurrentTime (handle, arg, ref time, ref timelineDiscontinuty);
		}


		[DllImport (Constants.AudioToolboxLibrary)]
		extern static AudioQueueStatus AudioQueueDeviceGetCurrentTime (IntPtr AQ, ref AudioTimeStamp time);

		public AudioTimeStamp CurrentTime {
			get {
				AudioTimeStamp stamp = new AudioTimeStamp ();
				
				if (AudioQueueDeviceGetCurrentTime (handle, ref stamp) != AudioQueueStatus.Ok){
					// Set no values as valid
					stamp.Flags = 0;
				}

				return stamp;
			}
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static AudioQueueStatus AudioQueueDeviceGetNearestStartTime (IntPtr AQ, ref AudioTimeStamp data, int flags);

		public AudioTimeStamp GetNearestStartTime (AudioTimeStamp requestedStartTime)
		{
			var k = AudioQueueDeviceGetNearestStartTime (handle, ref requestedStartTime, 0);
			if (k != 0)
				throw new AudioQueueException (k);

			return requestedStartTime;
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static AudioQueueStatus AudioQueueDeviceTranslateTime (IntPtr AQ, ref AudioTimeStamp inTime, out AudioTimeStamp translatedTime);

		public AudioTimeStamp TranslateTime (AudioTimeStamp timeToTranslate)
		{
			AudioTimeStamp ret;
			
			AudioQueueDeviceTranslateTime (handle, ref timeToTranslate, out ret);
			return ret;
		}
		
		[DllImport (Constants.AudioToolboxLibrary)]
		extern static OSStatus AudioQueueGetParameter (IntPtr AQ, AudioQueueParameter parameterId, out float result);
			
		[DllImport (Constants.AudioToolboxLibrary)]
		extern static OSStatus AudioQueueSetParameter (IntPtr AQ, AudioQueueParameter parameterId, float value);

		public float Volume {
			get {
				float r;
				var res = AudioQueueGetParameter (handle, AudioQueueParameter.Volume, out r);
				if (res != 0)
					throw new AudioQueueException (res);
				
				return r;
			}

			set {
				var res = AudioQueueSetParameter (handle, AudioQueueParameter.Volume, value);
				if (res != 0)
					throw new AudioQueueException (res);
			}
		}

		public float VolumeRampTime {
			get {
				float r;
				var res = AudioQueueGetParameter (handle, AudioQueueParameter.VolumeRampTime, out r);
				if (res != 0)
					throw new AudioQueueException (res);
				
				return r;
			}

			set {
				var res = AudioQueueSetParameter (handle, AudioQueueParameter.VolumeRampTime, value);
				if (res != 0)
					throw new AudioQueueException (res);
			}
		}

		public float Pan {
			get {
				float r;
				var res = AudioQueueGetParameter (handle, AudioQueueParameter.Pan, out r);
				if (res != 0)
					throw new AudioQueueException (res);
				
				return r;
			}

			set {
				var res = AudioQueueSetParameter (handle, AudioQueueParameter.Pan, value);
				if (res != 0)
					throw new AudioQueueException (res);
			}
		}

		delegate void AudioQueuePropertyListenerProc (IntPtr userData, IntPtr AQ, AudioQueueProperty id);

		Hashtable listeners;
		
		[MonoPInvokeCallback(typeof(AudioQueuePropertyListenerProc))]
		static void property_changed (IntPtr userData, IntPtr AQ, AudioQueueProperty id)
		{
			GCHandle gch = GCHandle.FromIntPtr (userData);
			var aq = gch.Target as AudioQueue;
			lock (aq.listeners){
				ArrayList a = (ArrayList)aq.listeners [id];
				if (a == null)
					return;
				foreach (AudioQueuePropertyChanged cback in a){
					cback (id);
				}
			}
		}

		public delegate void AudioQueuePropertyChanged (AudioQueueProperty property);
		
		public AudioQueueStatus AddListener (AudioQueueProperty property, AudioQueuePropertyChanged callback)
		{
			if (callback == null)
				throw new ArgumentNullException ("callback");
			if (listeners == null)
				listeners = new Hashtable ();
			
			AudioQueueStatus res = AudioQueueStatus.Ok;
			lock (listeners){
				var a = (ArrayList) listeners [property];
				if (a == null){
					res = AudioQueueAddPropertyListener (handle, property, property_changed, GCHandle.ToIntPtr (gch));
					if (res != AudioQueueStatus.Ok)
						return res;

					listeners [property] = a = new ArrayList ();
				}
				a.Add (callback);
			}

			return res;
		}

		public void RemoveListener (AudioQueueProperty property, AudioQueuePropertyChanged callback)
		{
			if (callback == null)
				throw new ArgumentNullException ("callback");
			if (listeners == null)
				return;
			lock (listeners){
				var a = (ArrayList) listeners [property];
				if (a == null)
					return;
				a.Remove (callback);
				if (a.Count == 0){
					AudioQueueRemovePropertyListener (handle, property, property_changed, GCHandle.ToIntPtr (gch));
				}
			}
		}
		
		[DllImport (Constants.AudioToolboxLibrary)]
		extern static AudioQueueStatus AudioQueueAddPropertyListener (IntPtr AQ, AudioQueueProperty id, AudioQueuePropertyListenerProc proc, IntPtr data);

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static OSStatus AudioQueueRemovePropertyListener (IntPtr AQ, AudioQueueProperty id, AudioQueuePropertyListenerProc proc, IntPtr data);
		
		[DllImport (Constants.AudioToolboxLibrary)]
		extern static AudioQueueStatus AudioQueueGetProperty (IntPtr AQ, uint id, IntPtr outdata, ref int dataSize);

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static AudioQueueStatus AudioQueueGetPropertySize (IntPtr AQ, uint id, out int size);

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static AudioQueueStatus AudioQueueSetProperty (
			IntPtr AQ, AudioQueueProperty id, IntPtr data, int size);

		// Should be private
		public bool GetProperty (AudioQueueProperty property, ref int dataSize, IntPtr outdata)
		{
			if (outdata == IntPtr.Zero)
				throw new ArgumentNullException ("outdata");
			return AudioQueueGetProperty (handle, (uint) property, outdata, ref dataSize) == 0;
		}

		// Should be private
		public bool SetProperty (AudioQueueProperty property, int dataSize, IntPtr propertyData)
		{
			if (propertyData == IntPtr.Zero)
				throw new ArgumentNullException ("propertyData");
			return AudioQueueSetProperty (handle, property, propertyData, dataSize) == 0;
		}

		// Should be private
		public IntPtr GetProperty (AudioQueueProperty property, out int size)
		{
			var r = AudioQueueGetPropertySize (handle, (uint) property, out size);
			if (r != 0)
				throw new AudioQueueException (r);

			var buffer = Marshal.AllocHGlobal (size);
			if (buffer == IntPtr.Zero)
				return IntPtr.Zero;

			r = AudioQueueGetProperty (handle, (uint) property, buffer, ref size);
			if (r == 0)
				return buffer;
			Marshal.FreeHGlobal (buffer);
			return IntPtr.Zero;
		}

		// Should be private
		public unsafe T GetProperty<T> (AudioQueueProperty property) where T:struct
		{
			int size;

			var r = AudioQueueGetPropertySize (handle, (uint) property, out size);
			if (r != 0)
				throw new AudioQueueException (r);

			var buffer = Marshal.AllocHGlobal (size);
			if (buffer == IntPtr.Zero)
				return default(T);
			try {
				r = AudioQueueGetProperty (handle, (uint) property, buffer, ref size);
				if (r == 0){
					T result = (T) Marshal.PtrToStructure (buffer, typeof (T));
					return result;
				}

				throw new AudioQueueException (r);
			} finally {
				Marshal.FreeHGlobal (buffer);
			}
		}

		unsafe T GetProperty<T> (AudioConverterPropertyID property) where T : struct
		{
			int size;

			var r = AudioQueueGetPropertySize (handle, (uint) property, out size);
			if (r != 0)
				throw new AudioQueueException (r);

			var buffer = Marshal.AllocHGlobal (size);
			if (buffer == IntPtr.Zero)
				return default (T);
			try {
				r = AudioQueueGetProperty (handle, (uint) property, buffer, ref size);
				if (r == 0){
					T result = (T) Marshal.PtrToStructure (buffer, typeof (T));
					return result;
				}

				throw new AudioQueueException (r);
			} finally {
				Marshal.FreeHGlobal (buffer);
			}
		}

		int GetInt (AudioQueueProperty property)
		{
			unsafe {
				int val = 0;
				int size = 4;
				var k = AudioQueueGetProperty (handle, (uint) property, (IntPtr) (&val), ref size);
				if (k == 0)
					return val;
				throw new AudioQueueException (k);
			}
		}

		int SetInt (AudioQueueProperty property, int val)
		{
			unsafe {
				int size = 4;
				var k = AudioQueueSetProperty (handle, property, (IntPtr) (&val), size);
				if (k == 0)
					return val;
				throw new AudioQueueException (k);
			}
		}

		double GetDouble (AudioQueueProperty property)
		{
			unsafe {
				double val = 0;
				int size = 8;
				var k = AudioQueueGetProperty (handle, (uint) property, (IntPtr) (&val), ref size);
				if (k == 0)
					return val;
				throw new AudioQueueException (k);

			}
		}

		public bool IsRunning {
			get {
				return GetInt (AudioQueueProperty.IsRunning) != 0;
			}
		}

		public double SampleRate {
			get {
				return GetDouble (AudioQueueProperty.DeviceSampleRate);
			}
		}

		public int DeviceChannels {
			get {
				return GetInt (AudioQueueProperty.DeviceNumberChannels);
			}
		}

		public string CurrentDevice {
			get {
				return CFString.FetchString ((IntPtr) GetInt (AudioQueueProperty.CurrentDevice));
			}

			set {
				// TODO
				throw new NotImplementedException ();
			}
		}

#pragma warning disable 612
		
		public byte [] MagicCookie {
			get {
				int size;
				var h = GetProperty (AudioQueueProperty.MagicCookie, out size);
				if (h == IntPtr.Zero)
					return new byte [0];
				
				byte [] cookie = new byte [size];
				for (int i = 0; i < cookie.Length; i++)
					cookie [i] = Marshal.ReadByte (h, i);
				Marshal.FreeHGlobal (h);

				return cookie;
			}

			set {
				if (value == null)
					throw new ArgumentNullException ("value");

				if (value.Length == 0)
					return;
				
				unsafe {
					fixed (byte *bp = &value [0]){
						SetProperty (AudioQueueProperty.MagicCookie, value.Length, (IntPtr) bp);
					}
				}
			}
		}

		public AudioChannelLayout ChannelLayout {
			get {
				int size;
				var h = GetProperty (AudioQueueProperty.ChannelLayout, out size);
				if (h == IntPtr.Zero)
					return null;
				
				var layout = AudioChannelLayout.FromHandle (h);
				Marshal.FreeHGlobal (h);

				return layout;
			}

			set {
				if (value == null)
					throw new ArgumentNullException ("value"); // TODO: enable ?

				int size;
				var h = value.ToBlock (out size);
				SetProperty (AudioQueueProperty.ChannelLayout, size, h);
				Marshal.FreeHGlobal (h);
			}
		}

		public bool EnableLevelMetering {
			get {
				return GetInt (AudioQueueProperty.EnableLevelMetering) != 0;
			}
			set {
				SetInt (AudioQueueProperty.EnableLevelMetering, value ? 1 : 0);
			}
		}

		public int MaximumOutputPacketSize {
			get {
				return GetInt (AudioQueueProperty.MaximumOutputPacketSize);
			}
		}

		public int DecodeBufferSizeFrames {
			get {
				return GetInt (AudioQueueProperty.DecodeBufferSizeFrames);
			}
		}

#if !XAMCORE_3_0
		[Obsolete ("Use 'AudioStreamDescription' instead.")]
		public AudioStreamBasicDescription AudioStreamPacketDescription {
			get {
				return AudioStreamDescription;
			}
		}
#endif

		public AudioStreamBasicDescription AudioStreamDescription {
			get {
#if !MONOMAC
				return GetProperty <AudioStreamBasicDescription> (AudioQueueProperty.StreamDescription);
#else
				return GetProperty <AudioStreamBasicDescription> (AudioConverterPropertyID.CurrentInputStreamDescription);
#endif
			}
		}

		public AudioQueueLevelMeterState [] CurrentLevelMeter {
			get {
				unsafe {
					int size = DeviceChannels * sizeof (AudioQueueLevelMeterState);
					int n;
					var buffer = GetProperty (AudioQueueProperty.CurrentLevelMeter, out n);
					if (buffer == IntPtr.Zero)
						return new AudioQueueLevelMeterState [0];
					var ret = new AudioQueueLevelMeterState [n];
					AudioQueueLevelMeterState *ptr = (AudioQueueLevelMeterState *) buffer;
					for (int i = 0; i < n; i++)
						ret [i] = ptr [i];
					return ret;
				}
			}
		}

		public AudioQueueLevelMeterState [] CurrentLevelMeterDB {
			get {
				unsafe {
					int size = DeviceChannels * sizeof (AudioQueueLevelMeterState);
					int n;
					var buffer = GetProperty (AudioQueueProperty.CurrentLevelMeterDB, out n);
					if (buffer == IntPtr.Zero)
						return new AudioQueueLevelMeterState [0];
					var ret = new AudioQueueLevelMeterState [n];
					AudioQueueLevelMeterState *ptr = (AudioQueueLevelMeterState *) buffer;
					for (int i = 0; i < n; i++)
						ret [i] = ptr [i];
					return ret;
				}
			}
		}

#pragma warning restore 612

		public uint ConverterError {
			get {
				return (uint) GetInt (AudioQueueProperty.ConverterError);
			}
		}

#if !MONOMAC
		public AudioQueueHardwareCodecPolicy HardwareCodecPolicy {
			get {
				return (AudioQueueHardwareCodecPolicy) GetInt (AudioQueueProperty.HardwareCodecPolicy);
			}
			set {
				SetInt (AudioQueueProperty.HardwareCodecPolicy, (int)value);
			}
		}

		[iOS (6,0)]
		public AudioQueueStatus SetChannelAssignments (params AudioQueueChannelAssignment[] channelAssignments)
		{
			if (channelAssignments == null)
				throw new ArgumentNullException ("channelAssignments");

			int length;
			var ptr = MarshalArray (ref channelAssignments, out length);
			try {
				return AudioQueueSetProperty (handle, AudioQueueProperty.ChannelAssignments, ptr, length);
			} finally {
				Marshal.FreeHGlobal (ptr);
			}
		}

		unsafe static IntPtr MarshalArray (ref AudioQueueChannelAssignment[] array, out int totalSize)
		{
			int elementSize = sizeof (AudioQueueChannelAssignment);
			totalSize = elementSize * array.Length;
			var array_ptr = (AudioQueueChannelAssignment*) Marshal.AllocHGlobal (totalSize);
			
			for (int i = 0; i < array.Length; i++)
				array_ptr [i] = array [i];
			
			return (IntPtr) array_ptr;
		}	
#endif

		[iOS (6,0)]
		[DllImport (Constants.AudioToolboxLibrary)]
		extern static AudioQueueStatus AudioQueueProcessingTapNew (IntPtr inAQ, AudioQueueProcessingTapCallbackShared inCallback,
			IntPtr inClientData, AudioQueueProcessingTapFlags inFlags, out uint outMaxFrames,
			out AudioStreamBasicDescription outProcessingFormat, out IntPtr outAQTap);

#if !XAMCORE_2_0
		[Obsolete ("Use 'CreateProcessingTap (AudioQueueProcessingTapDelegate, AudioQueueProcessingTapFlags, out AudioQueueStatus)' instead.", true)]
		[iOS (6,0)]
		public AudioQueueProcessingTap CreateProcessingTap (AudioQueueProcessingTapCallback processingCallback, AudioQueueProcessingTapFlags flags,
		                                                    out AudioQueueStatus status)
		{
			throw new NotSupportedException ();
		}
#endif

		[iOS (6,0)]
		public AudioQueueProcessingTap CreateProcessingTap (AudioQueueProcessingTapDelegate processingCallback, AudioQueueProcessingTapFlags flags,
		                                                    out AudioQueueStatus status)
		{		
			var aqpt = new AudioQueueProcessingTap (processingCallback);
			uint maxFrames;
			AudioStreamBasicDescription processingFormat;
			IntPtr tapHandle;

			status = AudioQueueProcessingTapNew (handle, AudioQueueProcessingTap.CreateTapCallback, GCHandle.ToIntPtr (aqpt.Handle), flags, out maxFrames,
			                                     out processingFormat, out tapHandle);

			if (status != AudioQueueStatus.Ok) {
				aqpt.Dispose ();
				return null;
			}

			aqpt.TapHandle = tapHandle;
			aqpt.MaxFrames = maxFrames;
			aqpt.ProcessingFormat = processingFormat;
			return aqpt;
		}
	}

	delegate void AudioQueueProcessingTapCallbackShared (IntPtr clientData, IntPtr tap, uint numberOfFrames,
	                                                     ref AudioTimeStamp timeStamp, ref AudioQueueProcessingTapFlags flags,
	                                                     out uint outNumberFrames, IntPtr data);

#if !XAMCORE_2_0
	[Obsolete ("Use 'AudioQueueProcessingTapDelegate'.")]
	public delegate uint AudioQueueProcessingTapCallback (AudioQueueProcessingTap audioQueueTap, uint numberOfFrames,
	                                                      ref AudioTimeStamp timeStamp, ref AudioQueueProcessingTapFlags flags,
	                                                      AudioBufferList data);
#endif

	public delegate uint AudioQueueProcessingTapDelegate (AudioQueueProcessingTap audioQueueTap, uint numberOfFrames,
	                                                      ref AudioTimeStamp timeStamp, ref AudioQueueProcessingTapFlags flags,
	                                                      AudioBuffers data);

	[iOS (6,0)]
	public class AudioQueueProcessingTap : IDisposable
	{
		internal static readonly AudioQueueProcessingTapCallbackShared CreateTapCallback = TapCallback;

		AudioQueueProcessingTapDelegate callback;
		readonly GCHandle gc_handle;

		internal AudioQueueProcessingTap (AudioQueueProcessingTapDelegate callback)
		{
			this.callback = callback;
			gc_handle = GCHandle.Alloc (this);
		}

		~AudioQueueProcessingTap ()
		{
			Dispose (false);
		}

		internal GCHandle Handle { 
			get {
				return gc_handle;
			}
		}

		internal IntPtr TapHandle { get; set; }
		public uint MaxFrames { get; internal set; }
		public AudioStreamBasicDescription ProcessingFormat { get; internal set; }

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (disposing) {
				callback = null;
			}

			if (gc_handle.IsAllocated) {
				gc_handle.Free ();
				AudioQueueProcessingTapDispose (TapHandle);
			}
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static OSStatus AudioQueueProcessingTapDispose (IntPtr inAQTap);

#if !XAMCORE_2_0
		[Obsolete]
		[DllImport (Constants.AudioToolboxLibrary)]
		extern static AudioQueueStatus AudioQueueProcessingTapGetSourceAudio (IntPtr inAQTap, uint inNumberFrames, ref AudioTimeStamp ioTimeStamp,
		                                                               out AudioQueueProcessingTapFlags outFlags, out uint outNumberFrames,
		                                                               AudioBufferList ioData);

		[Obsolete ("Use overload with 'AudioBuffers'.")]
		public AudioQueueStatus GetSourceAudio (uint numberOfFrames, ref AudioTimeStamp timeStamp,
		                                        out AudioQueueProcessingTapFlags flags, out uint parentNumberOfFrames, AudioBufferList data)
		{
			if (data == null)
				throw new ArgumentNullException ("data");

			return AudioQueueProcessingTapGetSourceAudio (TapHandle, numberOfFrames, ref timeStamp,
		                                                  out flags, out parentNumberOfFrames, data);
		}
#endif

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static AudioQueueStatus AudioQueueProcessingTapGetSourceAudio (IntPtr inAQTap, uint inNumberFrames, ref AudioTimeStamp ioTimeStamp,
		                                                               out AudioQueueProcessingTapFlags outFlags, out uint outNumberFrames,
		                                                               IntPtr ioData);

		public AudioQueueStatus GetSourceAudio (uint numberOfFrames, ref AudioTimeStamp timeStamp,
		                                        out AudioQueueProcessingTapFlags flags, out uint parentNumberOfFrames, AudioBuffers data)
		{
			if (data == null)
				throw new ArgumentNullException ("data");

			return AudioQueueProcessingTapGetSourceAudio (TapHandle, numberOfFrames, ref timeStamp,
		                                                  out flags, out parentNumberOfFrames, (IntPtr) data);
		}

		[Mac (10,8)]
		[DllImport (Constants.AudioToolboxLibrary)]
		extern static AudioQueueStatus AudioQueueProcessingTapGetQueueTime (IntPtr inAQTap, out double outQueueSampleTime, out uint outQueueFrameCount);

		[Mac (10,8)]
		public AudioQueueStatus GetQueueTime (out double sampleTime, out uint frameCount)
		{
			return AudioQueueProcessingTapGetQueueTime (TapHandle, out sampleTime, out frameCount);
		}

		[MonoPInvokeCallback (typeof (AudioQueueProcessingTapCallbackShared))]
		static void TapCallback (IntPtr clientData, IntPtr tap, uint numberFrames, ref AudioTimeStamp timeStamp, ref AudioQueueProcessingTapFlags flags,
		                         out uint outNumberFrames, IntPtr data)
		{
			GCHandle gch = GCHandle.FromIntPtr (clientData);
			var aqpt = (AudioQueueProcessingTap) gch.Target;

			using (var buffers = new AudioBuffers (data)) {
				outNumberFrames = aqpt.callback (aqpt, numberFrames, ref timeStamp, ref flags, buffers);
			}
		}
	}

	public class OutputAudioQueue : AudioQueue {
		static readonly AudioQueueOutputCallback dOutputCallback = output_callback;
						
		[DllImport (Constants.AudioToolboxLibrary)]
		extern static OSStatus AudioQueueNewOutput (ref AudioStreamBasicDescription format, AudioQueueOutputCallback callback,
							    IntPtr userData, IntPtr cfrunLoop_callbackRunloop, IntPtr cfstr_runMode,
							    uint flags, out IntPtr audioQueue);

		[MonoPInvokeCallback(typeof(AudioQueueOutputCallback))]
		static void output_callback (IntPtr userData, IntPtr AQ, IntPtr audioQueueBuffer)
		{
			GCHandle gch = GCHandle.FromIntPtr (userData);
			var aq = gch.Target as OutputAudioQueue;
#if XAMCORE_2_0
			aq.OnBufferCompleted (audioQueueBuffer);
#else
			aq.OnOutputCompleted (audioQueueBuffer);
#endif
		}

#if XAMCORE_2_0
		public event EventHandler<BufferCompletedEventArgs> BufferCompleted;

		protected virtual void OnBufferCompleted (IntPtr audioQueueBuffer)
		{
			var h = BufferCompleted;
			if (h != null)
				h (this, new BufferCompletedEventArgs (audioQueueBuffer));
		}
#else
		public event EventHandler<OutputCompletedEventArgs> OutputCompleted;
		
		protected virtual void OnOutputCompleted (IntPtr audioQueueBuffer)
		{
			var h = OutputCompleted;
			if (h != null)
				h (this, new OutputCompletedEventArgs (audioQueueBuffer));
		}
#endif

		public OutputAudioQueue (AudioStreamBasicDescription desc) : this (desc, null, (CFString) null)
		{
		}

		public OutputAudioQueue (AudioStreamBasicDescription desc, CFRunLoop runLoop, string runMode)
			: this (desc, runLoop, runMode == null ? null : new CFString (runMode))
		{
		}

		public OutputAudioQueue (AudioStreamBasicDescription desc, CFRunLoop runLoop, CFString runMode)
		{
			IntPtr h;
			GCHandle gch = GCHandle.Alloc (this);

			var code = AudioQueueNewOutput (ref desc, dOutputCallback, GCHandle.ToIntPtr (gch),
							runLoop == null ? IntPtr.Zero : runLoop.Handle,
							runMode == null ? IntPtr.Zero : runMode.Handle, 0, out h);

			if (code != 0) {
				gch.Free ();
				throw new AudioQueueException (code);
			}

			this.gch = gch;
			handle = h;
		}

		[DllImport (Constants.AudioToolboxLibrary, EntryPoint="AudioQueueSetOfflineRenderFormat")]
		extern static AudioQueueStatus AudioQueueSetOfflineRenderFormat2 (IntPtr aq, IntPtr format, IntPtr layout);

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static AudioQueueStatus AudioQueueSetOfflineRenderFormat (IntPtr aq, ref AudioStreamBasicDescription format, IntPtr layout);

		public AudioQueueStatus SetOfflineRenderFormat (AudioStreamBasicDescription desc, AudioChannelLayout layout)
		{
			int size;
			var h = layout == null ? IntPtr.Zero : layout.ToBlock (out size);
			try {
				return AudioQueueSetOfflineRenderFormat (handle, ref desc, h);
			} finally {
				Marshal.FreeHGlobal (h);
			}
		}

		public AudioQueueStatus DisableOfflineRender ()
		{
			return AudioQueueSetOfflineRenderFormat2 (handle, IntPtr.Zero, IntPtr.Zero);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern unsafe static AudioQueueStatus AudioQueueOfflineRender (IntPtr aq, ref AudioTimeStamp stamp, AudioQueueBuffer* buffer, int frames);

		public unsafe AudioQueueStatus RenderOffline (double timeStamp, AudioQueueBuffer* audioQueueBuffer, int frameCount)
		{
			if (audioQueueBuffer == null)
				throw new ArgumentNullException ("audioQueueBuffer");

			var stamp = new AudioTimeStamp () {
				SampleTime = timeStamp,
				Flags = AudioTimeStamp.AtsFlags.SampleTimeValid
			};
			return AudioQueueOfflineRender (handle, ref stamp, audioQueueBuffer, frameCount);
		}
	}

	public class InputAudioQueue : AudioQueue {
		static unsafe readonly AudioQueueInputCallback dInputCallback = input_callback;

		[MonoPInvokeCallback(typeof(AudioQueueInputCallback))]
		unsafe static void input_callback (IntPtr userData, IntPtr AQ, IntPtr audioQueueBuffer,
					    AudioTimeStamp *startTime, int descriptors, IntPtr inPacketDesc)
		{
			GCHandle gch = GCHandle.FromIntPtr (userData);
			var aq = gch.Target as InputAudioQueue;

			aq.OnInputCompleted (audioQueueBuffer, *startTime, AudioFile.PacketDescriptionFrom (descriptors, inPacketDesc));
		}

		public event EventHandler<InputCompletedEventArgs> InputCompleted;
		protected virtual void OnInputCompleted (IntPtr audioQueueBuffer, AudioTimeStamp timeStamp, AudioStreamPacketDescription [] packetDescriptions)
		{
			var h = InputCompleted;
			if (h != null)
				h (this, new InputCompletedEventArgs (audioQueueBuffer, timeStamp, packetDescriptions));
		}
		
		[DllImport (Constants.AudioToolboxLibrary)]
		extern static OSStatus AudioQueueNewInput (
			ref AudioStreamBasicDescription format,
			AudioQueueInputCallback callback,
			IntPtr inUserData,
			IntPtr cfrunLoop_inCallbackRunLoop,
			IntPtr cfstringref_inCallbackRunLoopMode,
			UInt32 inFlags,
			out IntPtr audioQueue);

		public InputAudioQueue (AudioStreamBasicDescription desc)
		 : this (desc, null, null)
		{
		}

		public InputAudioQueue (AudioStreamBasicDescription desc, CFRunLoop runLoop, string runMode)
		{
			IntPtr h;
			GCHandle mygch = GCHandle.Alloc (this);
			CFString s = runMode == null ? null : new CFString (runMode);
			
			var code = AudioQueueNewInput (ref desc, dInputCallback, GCHandle.ToIntPtr (mygch),
						       runLoop == null ? IntPtr.Zero : runLoop.Handle,
						       s == null ? IntPtr.Zero : s.Handle, 0, out h);
			if (s != null)
				s.Dispose ();
			
			if (code == 0){
				handle = h;
				gch = mygch;
				return;
			}
			mygch.Free ();
			throw new AudioQueueException (code);
		}

		public unsafe AudioQueueStatus EnqueueBuffer (AudioQueueBuffer* buffer)
		{
			return AudioQueueEnqueueBuffer (handle, buffer, 0, null);
		}
	}

	public class AudioQueueTimeline : IDisposable {
		internal protected IntPtr timelineHandle, queueHandle;

		internal AudioQueueTimeline (IntPtr queueHandle, IntPtr timelineHandle)
		{
			this.queueHandle = queueHandle;
			this.timelineHandle = timelineHandle;
		}
		
		[DllImport (Constants.AudioToolboxLibrary)]
		extern static AudioQueueStatus AudioQueueDisposeTimeline (IntPtr AQ, IntPtr timeline);

		public void Dispose ()
		{
			Dispose (true);
		}
	
#if XAMCORE_2_0
		protected virtual void Dispose (bool disposing)
#else
		public virtual void Dispose (bool disposing)
#endif
		{
			if (timelineHandle != IntPtr.Zero){
				AudioQueueDisposeTimeline (queueHandle, timelineHandle);
				timelineHandle = IntPtr.Zero;
			}
			GC.SuppressFinalize (this);
		}

		~AudioQueueTimeline ()
		{
			Dispose (false);
		}
	}
}
