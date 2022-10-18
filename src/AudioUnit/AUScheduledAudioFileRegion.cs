// 
// ScheduledAudioFileRegion.cs: A wrapper class around ScheduledAudioFileRegionProxy struct
//
// Authors:
//    Rustam Zaitov (rustam.zaitov@xamarin.com)
//    Alex Soto (alex.soto@xamarin.com)
// 
// Copyright 2015 Xamarin Inc.
//

#nullable enable

using System;
using System.Runtime.InteropServices;

using AudioToolbox;
using ObjCRuntime;
using System.Runtime.Versioning;

namespace AudioUnit {

	public delegate void AUScheduledAudioFileRegionCompletionHandler (AUScheduledAudioFileRegion audioFileRegion, AudioUnitStatus status);

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class AUScheduledAudioFileRegion : IDisposable {

		[StructLayout (LayoutKind.Sequential)]
		internal struct ScheduledAudioFileRegion {
			public AudioTimeStamp TimeStamp;
#if NET
			public unsafe delegate* unmanaged<IntPtr, IntPtr, AudioUnitStatus, void> CompletionHandler;
#else
			public ScheduledAudioFileRegionCompletionHandler CompletionHandler;
#endif
			public /* void * */ IntPtr CompletionHandlerUserData;
			public IntPtr AudioFile;
			public uint LoopCount;
			public long StartFrame;
			public uint FramesToPlay;
		}

		GCHandle handle;
		AUScheduledAudioFileRegionCompletionHandler? completionHandler;
		bool alreadyUsed = false;

		public AudioTimeStamp TimeStamp { get; set; }
		public AudioFile AudioFile { get; private set; }
		public uint LoopCount { get; set; }
		public long StartFrame { get; set; }
		public uint FramesToPlay { get; set; }

		public AUScheduledAudioFileRegion (AudioFile audioFile, AUScheduledAudioFileRegionCompletionHandler? completionHandler = null)
		{
			if (audioFile is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (audioFile));

			AudioFile = audioFile;
			this.completionHandler = completionHandler;
		}

#if !NET
		internal delegate void ScheduledAudioFileRegionCompletionHandler (
			/* void * */IntPtr userData,
			/* ScheduledAudioFileRegion * */ IntPtr fileRegion,
			/* OSStatus */ AudioUnitStatus result);

		static readonly ScheduledAudioFileRegionCompletionHandler static_ScheduledAudioFileRegionCompletionHandler = new ScheduledAudioFileRegionCompletionHandler (ScheduledAudioFileRegionCallback);

#if !MONOMAC
		[MonoPInvokeCallback (typeof (ScheduledAudioFileRegionCompletionHandler))]
#endif
#else
		[UnmanagedCallersOnly]
#endif
		static void ScheduledAudioFileRegionCallback (IntPtr userData, IntPtr fileRegion, AudioUnitStatus status)
		{
			if (userData == IntPtr.Zero)
				return;

			var handle = GCHandle.FromIntPtr (userData);
			var inst = (AUScheduledAudioFileRegion?) handle.Target;
			if (inst?.completionHandler is not null)
				inst.completionHandler (inst, status);
		}

		internal ScheduledAudioFileRegion GetAudioFileRegion ()
		{
			if (alreadyUsed)
				throw new InvalidOperationException ("You should not call SetScheduledFileRegion with a previously set region instance");

			IntPtr ptr = IntPtr.Zero;
			if (completionHandler is not null) {
				handle = GCHandle.Alloc (this);
				ptr = GCHandle.ToIntPtr (handle);
			}

			var ret = new ScheduledAudioFileRegion {
				TimeStamp = TimeStamp,
				CompletionHandlerUserData = ptr,
				AudioFile = AudioFile.Handle,
				LoopCount = LoopCount,
				StartFrame = StartFrame,
				FramesToPlay = FramesToPlay
			};

			if (ptr != IntPtr.Zero) {
				unsafe {
#if NET
					ret.CompletionHandler = &ScheduledAudioFileRegionCallback;
#else
					ret.CompletionHandler = static_ScheduledAudioFileRegionCompletionHandler;
#endif
				}
			}

			alreadyUsed = true;
			return ret;
		}

		~AUScheduledAudioFileRegion ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (disposing)
				completionHandler = null;

			if (handle.IsAllocated)
				handle.Free ();
		}
	}
}
