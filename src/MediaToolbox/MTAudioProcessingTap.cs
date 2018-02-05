#if IOS || MONOMAC
#if !XAMCORE_2_0
// NOTICE: 32-bit only implementation 
//
// There were a bunch of assumptions in nested callbacks that used the wrong
// public type definition, which makes it possible, but annoying to supprot with
// the Unified API.  We would pay a price that we do not need to pay with too
// many casts.
//
// For now, i am applying a band aid to this, and fixing the result in
// XAMCORE_2_0 in a separate file.
//
// MTAudioProcessingTap.cs: Type wrapper for MTAudioProcessingTap
//
// Authors:
//   Marek Safar (marek.safar@gmail.com)
//
// Copyright 2012-2014, Xamarin Inc.
//
// The class can be either constructed from a string (from user code)
// or from a handle (from iphone-sharp.dll internal calls).  This
// delays the creation of the actual managed string until actually
// required
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
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Foundation;
using CoreFoundation;
using ObjCRuntime;
using AudioToolbox;
using CoreMedia;

namespace MediaToolbox
{
	[iOS (6,0)][Mac (10, 9)]
	public class MTAudioProcessingTap : INativeObject
#if !COREBUILD
		, IDisposable
#endif
		{
#if !COREBUILD
		// MTAudioProcessingTapCallbacks
		[StructLayout (LayoutKind.Sequential, Pack = 1)]
		unsafe struct Callbacks
		{
#pragma warning disable 169
			/* int */ int version; // kMTAudioProcessingTapCallbacksVersion_0 == 0
			public /* void* */ IntPtr clientInfo;
			public /* MTAudioProcessingTapInitCallback */ MTAudioProcessingTapInitCallbackProxy init;
			public /* MTAudioProcessingTapFinalizeCallback */ Action<IntPtr> finalize;
			public /* MTAudioProcessingTapPrepareCallback */ MTAudioProcessingTapPrepareCallbackProxy prepare;
			public /* MTAudioProcessingTapUnprepareCallback */ Action<IntPtr> unprepare;
			public /* MTAudioProcessingTapProcessCallback */ MTAudioProcessingTapProcessCallbackProxy process;
#pragma warning restore 169
		}

		// MTAudioProcessingTapInitCallback
		unsafe delegate void MTAudioProcessingTapInitCallbackProxy (/* MTAudioProcessingTapRef */ IntPtr tap, /* void* */ IntPtr clientInfo, /* void** */ out void* tapStorageOut);

		delegate void MTAudioProcessingTapProcessCallbackProxy (/* MTAudioProcessingTapRef */ IntPtr tap, 
			int numberFrames, MTAudioProcessingTapFlags flags, /* AudioBufferList* */ IntPtr bufferListInOut, 
			out int numberFramesOut, out MTAudioProcessingTapFlags flagsOut);

		delegate void MTAudioProcessingTapPrepareCallbackProxy (/* MTAudioProcessingTapRef */ IntPtr tap, int maxFrames, ref AudioStreamBasicDescription processingFormat);

		static readonly Dictionary<IntPtr, MTAudioProcessingTap> handles = new Dictionary<IntPtr, MTAudioProcessingTap> (Runtime.IntPtrEqualityComparer);

		IntPtr handle;
		MTAudioProcessingTapCallbacks callbacks;
		GCHandle gch;
		
		internal static MTAudioProcessingTap FromHandle (IntPtr handle)
		{
			lock (handles){
				MTAudioProcessingTap ret;
				if (handles.TryGetValue (handle, out ret))
					return ret;
				return null;
			}
		}

		[DllImport (Constants.MediaToolboxLibrary)]
		extern static /* OSStatus */ MTAudioProcessingTapError MTAudioProcessingTapCreate (
			/* CFAllocatorRef*/ IntPtr allocator, 
			/* const MTAudioProcessingTapCallbacks* */ ref Callbacks callbacks, 
			MTAudioProcessingTapCreationFlags flags,
			/* MTAudioProcessingTapRef* */ out IntPtr tapOut);

		public MTAudioProcessingTap (MTAudioProcessingTapCallbacks callbacks, MTAudioProcessingTapCreationFlags flags)
		{
			if (callbacks == null)
				throw new ArgumentNullException ("callbacks");

			const MTAudioProcessingTapCreationFlags all_flags = MTAudioProcessingTapCreationFlags.PreEffects | MTAudioProcessingTapCreationFlags.PostEffects;
			if ((flags & all_flags) == all_flags)
				throw new ArgumentException ("Only one effect type can be set");

			this.callbacks = callbacks;

			var c = new Callbacks ();
			unsafe {
				if (callbacks.Initialize != null)
					c.init = InitializeProxy;
				if (callbacks.Finalize != null)
					c.finalize = FinalizeProxy;
				if (callbacks.Prepare != null)
					c.prepare = PrepareProxy;
				if (callbacks.Unprepare != null)
					c.unprepare = UnprepareProxy;

				c.process = ProcessProxy;
			}

			// a GCHandle is needed because we do not have an handle before calling MTAudioProcessingTapCreate
			// and that will call the InitializeProxy. So using this (short-lived) GCHandle allow us to find back the
			// original managed instance
			gch = GCHandle.Alloc (this);
			c.clientInfo = (IntPtr)gch;

			var res = MTAudioProcessingTapCreate (IntPtr.Zero, ref c, flags, out handle);

			// we won't need the GCHandle after the Create call
			gch.Free ();

			if (res != 0)
				throw new ArgumentException (res.ToString ());

			lock (handles)
				handles [handle] = this;
		}

		~MTAudioProcessingTap ()
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
			if (handle != IntPtr.Zero) {
				lock (handles)
					handles.Remove (handle);
				
				CFObject.CFRelease (handle);
				handle = IntPtr.Zero;
			}
		}

		public IntPtr Handle {
			get { return handle; }
		}

		[DllImport (Constants.MediaToolboxLibrary)]
		unsafe extern static void* MTAudioProcessingTapGetStorage (/* MTAudioProcessingTapRef */ IntPtr tap);

		public unsafe void* GetStorage ()
		{
			return MTAudioProcessingTapGetStorage (handle);
		}
			
		[Obsolete]
		[DllImport (Constants.MediaToolboxLibrary)]
		extern static /* OSStatus */ MTAudioProcessingTapError MTAudioProcessingTapGetSourceAudio (
			/* MTAudioProcessingTapRef */ IntPtr tap, int numberFrames, 
			/* AudioBufferList* */ ref AudioBufferList bufferListInOut,
			out MTAudioProcessingTapFlags flagsOut, out CMTimeRange timeRangeOut, out int numberFramesOut);

		[Obsolete ("Use overload with 'AudioBuffers'.")]
		public MTAudioProcessingTapError GetSourceAudio (long frames, ref AudioBufferList bufferList, out MTAudioProcessingTapFlags flags,
			out CMTimeRange timeRange, long framesProvided)
		{
			int outFp;
			return MTAudioProcessingTapGetSourceAudio (handle, (int) frames, ref bufferList, out flags, out timeRange, out outFp);
		}

		[DllImport (Constants.MediaToolboxLibrary)]
		extern static /* OSStatus */ MTAudioProcessingTapError MTAudioProcessingTapGetSourceAudio (
			/* MTAudioProcessingTapRef */ IntPtr tap, int numberFrames, 
			/* AudioBufferList* */ IntPtr bufferListInOut,
			out MTAudioProcessingTapFlags flagsOut, out CMTimeRange timeRangeOut, out int numberFramesOut);
			
		[Obsolete ("Use 'GetSourceAudio(int,AudioBuffers,out MTAudioProcessingTapFlags, out CMTimeRange, out int)' instead.")]
		public MTAudioProcessingTapError GetSourceAudio (long frames, AudioBuffers bufferList, out MTAudioProcessingTapFlags flags,
			out CMTimeRange timeRange, long framesProvided)
		{
			if (bufferList == null)
				throw new ArgumentNullException ("bufferList");

			int result;
			var r = MTAudioProcessingTapGetSourceAudio (handle, (int) frames, (IntPtr) bufferList, out flags, out timeRange, out result);
			return r;
		}

		public MTAudioProcessingTapError GetSourceAudio (int frames, AudioBuffers bufferList, out MTAudioProcessingTapFlags flags, out CMTimeRange timeRange, out int framesProvided)
		{
			if (bufferList == null)
				throw new ArgumentNullException ("bufferList");

			return MTAudioProcessingTapGetSourceAudio (handle, frames, (IntPtr) bufferList, out flags, out timeRange, out framesProvided);
		}
			

		//
		// Proxy callbacks
		//
		[MonoPInvokeCallback (typeof (MTAudioProcessingTapInitCallbackProxy))]
		unsafe static void InitializeProxy (IntPtr tap, IntPtr /*void**/ clientInfo, out void* tapStorage)
		{
			var apt = (MTAudioProcessingTap) GCHandle.FromIntPtr (clientInfo).Target;
			apt.callbacks.Initialize (apt, out tapStorage);
		}	

		[MonoPInvokeCallback (typeof (MTAudioProcessingTapProcessCallbackProxy))]
		static void ProcessProxy (IntPtr tap, int numberFrames, MTAudioProcessingTapFlags flags,
			IntPtr bufferList, out int numberFramesOut, out MTAudioProcessingTapFlags flagsOut)
		{
			long numberOut;
			MTAudioProcessingTap apt;
			lock (handles)
				apt = handles [tap];

			apt.callbacks.Processing (apt, (long) numberFrames, flags, new AudioBuffers (bufferList), out numberOut, out flagsOut);
			numberFramesOut = (int) numberOut;
		}

		[MonoPInvokeCallback (typeof (Action<IntPtr>))]
		static void FinalizeProxy (IntPtr tap)
		{
			MTAudioProcessingTap apt;
			lock (handles)
				apt = handles [tap];
			apt.callbacks.Finalize (apt);
		}

		[MonoPInvokeCallback (typeof (MTAudioProcessingTapPrepareCallbackProxy))]
		static void PrepareProxy (IntPtr tap, int maxFrames, ref AudioStreamBasicDescription processingFormat)
		{
			MTAudioProcessingTap apt;
			lock (handles)
				apt = handles [tap];
			apt.callbacks.Prepare (apt, (long) maxFrames, ref processingFormat);
		}

		[MonoPInvokeCallback (typeof (Action<IntPtr>))]
		static void UnprepareProxy (IntPtr tap)
		{
			MTAudioProcessingTap apt;
			lock (handles)
				apt = handles [tap];
			apt.callbacks.Unprepare (apt);
		}
#endif // !COREBUILD
	}

	// uint32_t -> MTAudioProcessingTap.h
	[Flags]
	public enum MTAudioProcessingTapCreationFlags : uint
	{
		PreEffects	= (1 << 0),
		PostEffects	= (1 << 1),
	}

	// uint32_t -> MTAudioProcessingTap.h
	[Flags]
	public enum MTAudioProcessingTapFlags : uint
	{
		StartOfStream	= (1 << 8),
		EndOfStream		= (1 << 9),
	}

	// used as OSStatus (4 bytes)
	// Not documented error codes
	public enum MTAudioProcessingTapError
	{
		None				= 0,
		InvalidArgument		= -12780
	}

	public class MTAudioProcessingTapCallbacks
	{
		[Obsolete ("Use constructor with MTAudioProcessingTapProcessDelegate", true)]
		public MTAudioProcessingTapCallbacks (MTAudioProcessingTapProcessCallback process)
		{
			throw new NotSupportedException ();
		}

		public MTAudioProcessingTapCallbacks (MTAudioProcessingTapProcessDelegate process)
		{
			if (process == null)
				throw new ArgumentNullException ("process");

			Processing = process;
		}

		public MTAudioProcessingTapInitCallback Initialize { get; set; }
		public Action<MTAudioProcessingTap> Finalize { get; set; }
		public MTAudioProcessingTapPrepareCallback Prepare { get; set; }
		public Action<MTAudioProcessingTap> Unprepare { get; set; }

		[Obsolete ("Use 'Processing' property instead.")]
		public MTAudioProcessingTapProcessCallback Process { get; private set; }

		public MTAudioProcessingTapProcessDelegate Processing { get; private set; }
	}

	public unsafe delegate void MTAudioProcessingTapInitCallback (MTAudioProcessingTap tap, out void* tapStorage);
	public delegate void MTAudioProcessingTapPrepareCallback (MTAudioProcessingTap tap, long maxFrames, ref AudioStreamBasicDescription processingFormat);

	[Obsolete ("Use 'MTAudioProcessingTapProcessDelegate' instead.")]
	public delegate void MTAudioProcessingTapProcessCallback (MTAudioProcessingTap tap, long numberFrames, MTAudioProcessingTapFlags flags,
			ref AudioBufferList bufferList, out long numberFramesOut, out MTAudioProcessingTapFlags flagsOut);

	public delegate void MTAudioProcessingTapProcessDelegate (MTAudioProcessingTap tap, long numberFrames, MTAudioProcessingTapFlags flags,
			AudioBuffers bufferList, out long numberFramesOut, out MTAudioProcessingTapFlags flagsOut);

#if COREBUILD
	public class AudioBufferList {}
#endif
}
#endif // !XAMCORE_2_0
#endif // IOS
