#if IOS || TVOS || MONOMAC
#if XAMCORE_2_0
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
	[iOS (6,0)][Mac (10,9)]
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
			IntPtr numberFrames, MTAudioProcessingTapFlags flags, /* AudioBufferList* */ IntPtr bufferListInOut, 
			out IntPtr numberFramesOut, out MTAudioProcessingTapFlags flagsOut);

		delegate void MTAudioProcessingTapPrepareCallbackProxy (/* MTAudioProcessingTapRef */ IntPtr tap, IntPtr maxFrames, ref AudioStreamBasicDescription processingFormat);

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

		[DllImport (Constants.MediaToolboxLibrary)]
		extern static /* OSStatus */ MTAudioProcessingTapError MTAudioProcessingTapGetSourceAudio (
			/* MTAudioProcessingTapRef */ IntPtr tap, IntPtr numberFrames, 
			/* AudioBufferList* */ IntPtr bufferListInOut,
			out MTAudioProcessingTapFlags flagsOut, out CMTimeRange timeRangeOut, out IntPtr numberFramesOut);

		public MTAudioProcessingTapError GetSourceAudio (nint frames, AudioBuffers bufferList, out MTAudioProcessingTapFlags flags, out CMTimeRange timeRange, out nint framesProvided)
		{
			if (bufferList == null)
				throw new ArgumentNullException ("bufferList");

			IntPtr result;
			var r = MTAudioProcessingTapGetSourceAudio (handle, (IntPtr) frames, (IntPtr) bufferList, out flags, out timeRange, out result);
			framesProvided = (nint) result;
			return r;
		}
			

		//
		// Proxy callbacks
		//
		[MonoPInvokeCallback (typeof (MTAudioProcessingTapInitCallbackProxy))]
		unsafe static void InitializeProxy (IntPtr tap, IntPtr /*void**/ clientInfo, out void* tapStorage)
		{
			// at this stage the handle is not yet known (or part of the `handles` dictionary
			// so we track back the managed MTAudioProcessingTap instance from the GCHandle
			var apt = (MTAudioProcessingTap) GCHandle.FromIntPtr (clientInfo).Target;
			apt.callbacks.Initialize (apt, out tapStorage);
		}	

		[MonoPInvokeCallback (typeof (MTAudioProcessingTapProcessCallbackProxy))]
		static void ProcessProxy (IntPtr tap, IntPtr numberFrames, MTAudioProcessingTapFlags flags,
			IntPtr bufferList, out IntPtr numberFramesOut, out MTAudioProcessingTapFlags flagsOut)
		{
			// from here we do not have access to `clientInfo` so it's not possible to use the GCHandle to get the
			// MTAudioProcessingTap managed instance. Instead we get it from a static Dictionary
			nint numberOut;
			MTAudioProcessingTap apt;
			lock (handles)
				apt = handles [tap];
			apt.callbacks.Processing (apt, (nint) numberFrames, flags, new AudioBuffers (bufferList), out numberOut, out flagsOut);
			numberFramesOut = (IntPtr) numberOut;
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
		static void PrepareProxy (IntPtr tap, IntPtr maxFrames, ref AudioStreamBasicDescription processingFormat)
		{
			MTAudioProcessingTap apt;
			lock (handles)
				apt = handles [tap];
			apt.callbacks.Prepare (apt, (nint) maxFrames, ref processingFormat);
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
		public MTAudioProcessingTapProcessDelegate Processing { get; private set; }
	}

	public unsafe delegate void MTAudioProcessingTapInitCallback (MTAudioProcessingTap tap, out void* tapStorage);
	public delegate void MTAudioProcessingTapPrepareCallback (MTAudioProcessingTap tap, nint maxFrames, ref AudioStreamBasicDescription processingFormat);

	public delegate void MTAudioProcessingTapProcessDelegate (MTAudioProcessingTap tap, nint numberFrames, MTAudioProcessingTapFlags flags,
								  AudioBuffers bufferList, out nint numberFramesOut, out MTAudioProcessingTapFlags flagsOut);

#if COREBUILD
	public class AudioBufferList {}
#endif
}
#endif // XAMCORE_2_0
#endif // IOS || TVOS
