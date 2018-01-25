// 
// ScheduledAudioFileRegion.cs: A wrapper class around ScheduledAudioFileRegionProxy struct
//
// Authors:
//    Rustam Zaitov (rustam.zaitov@xamarin.com)
//    Alex Soto (alex.soto@xamarin.com)
// 
// Copyright 2015 Xamarin Inc.
//

using System;
using System.Runtime.InteropServices;

using AudioToolbox;
using ObjCRuntime;

namespace AudioUnit {

	public delegate void AUScheduledAudioFileRegionCompletionHandler (AUScheduledAudioFileRegion audioFileRegion, AudioUnitStatus status);

	public class AUScheduledAudioFileRegion : IDisposable {

		[StructLayout (LayoutKind.Sequential)]
		internal struct ScheduledAudioFileRegion
		{
			public AudioTimeStamp TimeStamp;
			public ScheduledAudioFileRegionCompletionHandler CompletionHandler;
			public /* void * */ IntPtr CompletionHandlerUserData;
			public IntPtr AudioFile;
			public uint LoopCount;
			public long StartFrame;
			public uint FramesToPlay;
		}

		GCHandle handle;
		AUScheduledAudioFileRegionCompletionHandler completionHandler;
		bool alreadyUsed = false;

		public AudioTimeStamp TimeStamp { get; set; }
		public AudioFile AudioFile { get; private set; }
		public uint LoopCount { get; set; }
		public long StartFrame { get; set; }
		public uint FramesToPlay { get; set; }

		public AUScheduledAudioFileRegion (AudioFile audioFile, AUScheduledAudioFileRegionCompletionHandler completionHandler = null)
		{
			if (audioFile == null)
				throw new ArgumentNullException (nameof (audioFile));

			AudioFile = audioFile;
			this.completionHandler = completionHandler;
		}

		internal delegate void ScheduledAudioFileRegionCompletionHandler (
			/* void * */IntPtr userData, 
			/* ScheduledAudioFileRegion * */ref ScheduledAudioFileRegion fileRegion, 
			/* OSStatus */ AudioUnitStatus result);

		static readonly ScheduledAudioFileRegionCompletionHandler static_ScheduledAudioFileRegionCompletionHandler = new ScheduledAudioFileRegionCompletionHandler (ScheduledAudioFileRegionCallback);

#if !MONOMAC
		[MonoPInvokeCallback (typeof (ScheduledAudioFileRegionCompletionHandler))]
#endif
		static void ScheduledAudioFileRegionCallback (IntPtr userData, ref ScheduledAudioFileRegion fileRegion, AudioUnitStatus status)
		{
			if (userData == IntPtr.Zero)
				return;
			
			var handle = GCHandle.FromIntPtr (userData);
			var inst = (AUScheduledAudioFileRegion) handle.Target;
			inst?.completionHandler (inst, status);
		}

		internal ScheduledAudioFileRegion GetAudioFileRegion ()
		{
			if (alreadyUsed)
				throw new InvalidOperationException ("You should not call SetScheduledFileRegion with a previously set region instance");

			IntPtr ptr = IntPtr.Zero;
			if (completionHandler != null) {
				handle = GCHandle.Alloc (this);
				ptr = GCHandle.ToIntPtr (handle);
			}

			var ret = new ScheduledAudioFileRegion {
				TimeStamp = TimeStamp,
				CompletionHandlerUserData = ptr,
				CompletionHandler = ptr != IntPtr.Zero ? static_ScheduledAudioFileRegionCompletionHandler : null,
				AudioFile = AudioFile.Handle,
				LoopCount = LoopCount,
				StartFrame = StartFrame,
				FramesToPlay = FramesToPlay
			};

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

