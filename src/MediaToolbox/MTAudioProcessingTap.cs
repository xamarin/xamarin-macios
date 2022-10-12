#if HAS_MEDIATOOLBOX
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

#nullable enable

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
	public class MTAudioProcessingTap : NativeObject
	{
#if !COREBUILD
		delegate void Action_IntPtr (IntPtr arg);

		// MTAudioProcessingTapCallbacks
		[StructLayout (LayoutKind.Sequential, Pack = 1)]
		unsafe struct Callbacks
		{
#pragma warning disable 169
			/* int */ int version; // kMTAudioProcessingTapCallbacksVersion_0 == 0
			public /* void* */ IntPtr clientInfo;
#if NET
			public /* MTAudioProcessingTapInitCallback */ delegate* unmanaged<IntPtr, IntPtr, void**, void> init;
			public /* MTAudioProcessingTapFinalizeCallback */ delegate* unmanaged<IntPtr, void> finalize;
			public /* MTAudioProcessingTapPrepareCallback */ delegate* unmanaged<IntPtr, IntPtr, AudioStreamBasicDescription*, void> prepare;
			public /* MTAudioProcessingTapUnprepareCallback */ delegate* unmanaged<IntPtr, void> unprepare;
			public /* MTAudioProcessingTapProcessCallback */ delegate* unmanaged<IntPtr, IntPtr, MTAudioProcessingTapFlags, IntPtr, IntPtr*, MTAudioProcessingTapFlags*, void> process;
#else
			public /* MTAudioProcessingTapInitCallback */ MTAudioProcessingTapInitCallbackProxy init;
			public /* MTAudioProcessingTapFinalizeCallback */ Action_IntPtr finalize;
			public /* MTAudioProcessingTapPrepareCallback */ MTAudioProcessingTapPrepareCallbackProxy prepare;
			public /* MTAudioProcessingTapUnprepareCallback */ Action_IntPtr unprepare;
			public /* MTAudioProcessingTapProcessCallback */ MTAudioProcessingTapProcessCallbackProxy process;
#endif
#pragma warning restore 169
		}

		// MTAudioProcessingTapInitCallback
		unsafe delegate void MTAudioProcessingTapInitCallbackProxy (/* MTAudioProcessingTapRef */ IntPtr tap, /* void* */ IntPtr clientInfo, /* void** */ out void* tapStorageOut);

		delegate void MTAudioProcessingTapProcessCallbackProxy (/* MTAudioProcessingTapRef */ IntPtr tap, 
			IntPtr numberFrames, MTAudioProcessingTapFlags flags, /* AudioBufferList* */ IntPtr bufferListInOut, 
			out IntPtr numberFramesOut, out MTAudioProcessingTapFlags flagsOut);

		delegate void MTAudioProcessingTapPrepareCallbackProxy (/* MTAudioProcessingTapRef */ IntPtr tap, IntPtr maxFrames, ref AudioStreamBasicDescription processingFormat);

		static readonly Dictionary<IntPtr, MTAudioProcessingTap> handles = new Dictionary<IntPtr, MTAudioProcessingTap> (Runtime.IntPtrEqualityComparer);

		MTAudioProcessingTapCallbacks callbacks;
		
		internal static MTAudioProcessingTap? FromHandle (IntPtr handle)
		{
			lock (handles){
				if (handles.TryGetValue (handle, out var ret))
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
			if (callbacks is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (callbacks));

			const MTAudioProcessingTapCreationFlags all_flags = MTAudioProcessingTapCreationFlags.PreEffects | MTAudioProcessingTapCreationFlags.PostEffects;
			if ((flags & all_flags) == all_flags)
				throw new ArgumentException ("Only one effect type can be set");

			this.callbacks = callbacks;

			var c = new Callbacks ();
			unsafe {
#if NET
				if (callbacks.Initialize is not null)
					c.init = &InitializeProxy;
				if (callbacks.Finalize is not null)
					c.finalize = &FinalizeProxy;
				if (callbacks.Prepare is not null)
					c.prepare = &PrepareProxy;
				if (callbacks.Unprepare is not null)
					c.unprepare = &UnprepareProxy;
				c.process = &ProcessProxy;
#else
				if (callbacks.Initialize is not null)
					c.init = InitializeProxy;
				if (callbacks.Finalize is not null)
					c.finalize = FinalizeProxy;
				if (callbacks.Prepare is not null)
					c.prepare = PrepareProxy;
				if (callbacks.Unprepare is not null)
					c.unprepare = UnprepareProxy;
				c.process = ProcessProxy;
#endif

			}

			// a GCHandle is needed because we do not have an handle before calling MTAudioProcessingTapCreate
			// and that will call the InitializeProxy. So using this (short-lived) GCHandle allow us to find back the
			// original managed instance
			var gch = GCHandle.Alloc (this);
			c.clientInfo = (IntPtr)gch;

			var res = MTAudioProcessingTapCreate (IntPtr.Zero, ref c, flags, out var handle);

			// we won't need the GCHandle after the Create call
			gch.Free ();

			if (res != 0)
				throw new ArgumentException (res.ToString ());

			InitializeHandle (handle);

			lock (handles)
				handles [handle] = this;
		}

		protected override void Dispose (bool disposing)
		{
			if (Handle != IntPtr.Zero) {
				lock (handles)
					handles.Remove (Handle);
			}
			base.Dispose (disposing);
		}

		[DllImport (Constants.MediaToolboxLibrary)]
		unsafe extern static void* MTAudioProcessingTapGetStorage (/* MTAudioProcessingTapRef */ IntPtr tap);

		public unsafe void* GetStorage ()
		{
			return MTAudioProcessingTapGetStorage (Handle);
		}

		[DllImport (Constants.MediaToolboxLibrary)]
		extern static /* OSStatus */ MTAudioProcessingTapError MTAudioProcessingTapGetSourceAudio (
			/* MTAudioProcessingTapRef */ IntPtr tap, IntPtr numberFrames, 
			/* AudioBufferList* */ IntPtr bufferListInOut,
			out MTAudioProcessingTapFlags flagsOut, out CMTimeRange timeRangeOut, out IntPtr numberFramesOut);

		public MTAudioProcessingTapError GetSourceAudio (nint frames, AudioBuffers bufferList, out MTAudioProcessingTapFlags flags, out CMTimeRange timeRange, out nint framesProvided)
		{
			if (bufferList is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (bufferList));

			IntPtr result;
			var r = MTAudioProcessingTapGetSourceAudio (Handle, (IntPtr) frames, (IntPtr) bufferList, out flags, out timeRange, out result);
			framesProvided = (nint) result;
			return r;
		}
			

		//
		// Proxy callbacks
		//
#if NET
		[UnmanagedCallersOnly]
		unsafe static void InitializeProxy (IntPtr tap, IntPtr /*void**/ clientInfo, void** tapStorage)
#else
		[MonoPInvokeCallback (typeof (MTAudioProcessingTapInitCallbackProxy))]
		unsafe static void InitializeProxy (IntPtr tap, IntPtr /*void**/ clientInfo, out void* tapStorage)
#endif
		{
#if NET
			void *tempTapStorage = null;
			*tapStorage = tempTapStorage;
#else
			tapStorage = null;
#endif
			// at this stage the handle is not yet known (or part of the `handles` dictionary
			// so we track back the managed MTAudioProcessingTap instance from the GCHandle
			var apt = (MTAudioProcessingTap?) GCHandle.FromIntPtr (clientInfo).Target;
			if (apt?.callbacks.Initialize is not null) {
#if NET
				apt?.callbacks.Initialize (apt, out tempTapStorage);
				*tapStorage = tempTapStorage;
#else
				apt?.callbacks.Initialize (apt, out tapStorage);
#endif
			}
		}	

#if NET
		[UnmanagedCallersOnly]
		static unsafe void ProcessProxy (IntPtr tap, IntPtr numberFrames, MTAudioProcessingTapFlags flags,
			IntPtr bufferList, IntPtr* numberFramesOut, MTAudioProcessingTapFlags* flagsOut)
#else
		[MonoPInvokeCallback (typeof (MTAudioProcessingTapProcessCallbackProxy))]
		static void ProcessProxy (IntPtr tap, IntPtr numberFrames, MTAudioProcessingTapFlags flags,
			IntPtr bufferList, out IntPtr numberFramesOut, out MTAudioProcessingTapFlags flagsOut)
#endif
		{
			// from here we do not have access to `clientInfo` so it's not possible to use the GCHandle to get the
			// MTAudioProcessingTap managed instance. Instead we get it from a static Dictionary
			nint numberOut;
			MTAudioProcessingTap apt;
			lock (handles)
				apt = handles [tap];
#if NET
			*flagsOut = default (MTAudioProcessingTapFlags);
			*numberFramesOut = IntPtr.Zero;
#else
			flagsOut = default (MTAudioProcessingTapFlags);
			numberFramesOut = IntPtr.Zero;
#endif
			if (apt.callbacks.Processing is not null) {
#if NET
				apt.callbacks.Processing (apt, (nint) numberFrames, flags, new AudioBuffers (bufferList), out numberOut, out System.Runtime.CompilerServices.Unsafe.AsRef<MTAudioProcessingTapFlags> (flagsOut));
				*numberFramesOut = (IntPtr) numberOut;
#else
				apt.callbacks.Processing (apt, (nint) numberFrames, flags, new AudioBuffers (bufferList), out numberOut, out flagsOut);
				numberFramesOut = (IntPtr) numberOut;
#endif
			}
		}

#if NET
		[UnmanagedCallersOnly]
#else
		[MonoPInvokeCallback (typeof (Action_IntPtr))]
#endif
		static void FinalizeProxy (IntPtr tap)
		{
			MTAudioProcessingTap apt;
			lock (handles)
				apt = handles [tap];
			if (apt.callbacks.Finalize is not null)
				apt.callbacks.Finalize (apt);
		}
#if NET
		[UnmanagedCallersOnly]
		static unsafe void PrepareProxy (IntPtr tap, IntPtr maxFrames, AudioStreamBasicDescription* processingFormat)
#else
		[MonoPInvokeCallback (typeof (MTAudioProcessingTapPrepareCallbackProxy))]
		static void PrepareProxy (IntPtr tap, IntPtr maxFrames, ref AudioStreamBasicDescription processingFormat)
#endif
		{
			MTAudioProcessingTap apt;
			lock (handles)
				apt = handles [tap];
			if (apt.callbacks.Prepare is not null)
#if NET
				apt.callbacks.Prepare (apt, (nint) maxFrames, ref System.Runtime.CompilerServices.Unsafe.AsRef<AudioStreamBasicDescription> (processingFormat));
#else
				apt.callbacks.Prepare (apt, (nint) maxFrames, ref processingFormat);
#endif
		}

#if NET
		[UnmanagedCallersOnly]
#else
		[MonoPInvokeCallback (typeof (Action_IntPtr))]
#endif
		static void UnprepareProxy (IntPtr tap)
		{
			MTAudioProcessingTap apt;
			lock (handles)
				apt = handles [tap];
			if (apt.callbacks.Unprepare is not null)
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
			if (process is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (process));

			Processing = process;
		}

		public MTAudioProcessingTapInitCallback? Initialize { get; set; }
		public Action<MTAudioProcessingTap>? Finalize { get; set; }
		public MTAudioProcessingTapPrepareCallback? Prepare { get; set; }
		public Action<MTAudioProcessingTap>? Unprepare { get; set; }
		public MTAudioProcessingTapProcessDelegate? Processing { get; private set; }
	}

	public unsafe delegate void MTAudioProcessingTapInitCallback (MTAudioProcessingTap tap, out void* tapStorage);
	public delegate void MTAudioProcessingTapPrepareCallback (MTAudioProcessingTap tap, nint maxFrames, ref AudioStreamBasicDescription processingFormat);

	public delegate void MTAudioProcessingTapProcessDelegate (MTAudioProcessingTap tap, nint numberFrames, MTAudioProcessingTapFlags flags,
								  AudioBuffers bufferList, out nint numberFramesOut, out MTAudioProcessingTapFlags flagsOut);

#if COREBUILD
	public class AudioBufferList {}
#endif
}
#endif // HAS_MEDIATOOLBOX
