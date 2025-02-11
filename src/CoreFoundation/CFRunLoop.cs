//
// CFRunLoop.cs: Main Loop
//
// Authors:
//    Miguel de Icaza (miguel@novell.com)
//    Martin Baulig (martin.baulig@gmail.com)
//
// Copyright 2011-2014 Xamarin Inc
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
//

#nullable enable

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;
using System.Runtime.Versioning;

using CFIndex = System.IntPtr;

#nullable enable

namespace CoreFoundation {

	// anonymous and typeless native enum - System/Library/Frameworks/CoreFoundation.framework/Headers/CFRunLoop.h
	public enum CFRunLoopExitReason : int {
		Finished = 1,
		Stopped = 2,
		TimedOut = 3,
		HandledSource = 4
	}

	// CFRunLoop.h
	[StructLayout (LayoutKind.Sequential)]
	internal unsafe struct CFRunLoopSourceContext {
		public CFIndex Version;
		public IntPtr Info;
		public IntPtr Retain;
		public IntPtr Release;
		public IntPtr CopyDescription;
		public IntPtr Equal;
		public IntPtr Hash;
		public delegate* unmanaged<IntPtr, IntPtr, IntPtr, void> Schedule;
		public delegate* unmanaged<IntPtr, IntPtr, IntPtr, void> Cancel;
		public delegate* unmanaged<IntPtr, void> Perform;
	}

	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CFRunLoopSource : NativeObject {
		[Preserve (Conditional = true)]
		internal CFRunLoopSource (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFIndex */ nint CFRunLoopSourceGetOrder (/* CFRunLoopSourceRef */ IntPtr source);

		public nint Order {
			get {
				return CFRunLoopSourceGetOrder (Handle);
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFRunLoopSourceInvalidate (/* CFRunLoopSourceRef */ IntPtr source);

		public void Invalidate ()
		{
			CFRunLoopSourceInvalidate (Handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* Boolean */ byte CFRunLoopSourceIsValid (/* CFRunLoopSourceRef */ IntPtr source);

		public bool IsValid {
			get {
				return CFRunLoopSourceIsValid (Handle) != 0;
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFRunLoopSourceSignal (/* CFRunLoopSourceRef */ IntPtr source);

		public void Signal ()
		{
			CFRunLoopSourceSignal (Handle);
		}
	}

#if !COREBUILD
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public abstract class CFRunLoopSourceCustom : CFRunLoopSource {
		GCHandle gch;

		[DllImport (Constants.CoreFoundationLibrary)]
		unsafe extern static /* CFRunLoopSourceRef */ IntPtr CFRunLoopSourceCreate (/* CFAllocatorRef */ IntPtr allocator, /* CFIndex */ nint order, /* CFRunLoopSourceContext* */ CFRunLoopSourceContext* context);

		protected CFRunLoopSourceCustom ()
			: base (IntPtr.Zero, true)
		{
			gch = GCHandle.Alloc (this);
			var ctx = new CFRunLoopSourceContext ();
			ctx.Info = GCHandle.ToIntPtr (gch);
			unsafe {
				ctx.Schedule = &Schedule;
				ctx.Cancel = &Cancel;
				ctx.Perform = &Perform;
			}

			IntPtr handle;
			unsafe {
				handle = CFRunLoopSourceCreate (IntPtr.Zero, 0, &ctx);
			}
			InitializeHandle (handle);
		}

		[UnmanagedCallersOnly]
		static void Schedule (IntPtr info, IntPtr runLoop, IntPtr mode)
		{
			var source = GCHandle.FromIntPtr (info).Target as CFRunLoopSourceCustom;
			if (source is null)
				return;

			using (var loop = new CFRunLoop (runLoop, false))
			using (var mstring = new NSString (mode)) {
				source.OnSchedule (loop, mstring);
			}
		}

		protected abstract void OnSchedule (CFRunLoop loop, NSString mode);

		[UnmanagedCallersOnly]
		static void Cancel (IntPtr info, IntPtr runLoop, IntPtr mode)
		{
			var source = GCHandle.FromIntPtr (info).Target as CFRunLoopSourceCustom;
			if (source is null)
				return;

			using (var loop = new CFRunLoop (runLoop, false))
			using (var mstring = new NSString (mode)) {
				source.OnCancel (loop, mstring);
			}
		}

		protected abstract void OnCancel (CFRunLoop loop, NSString mode);

		[UnmanagedCallersOnly]
		static void Perform (IntPtr info)
		{
			var source = GCHandle.FromIntPtr (info).Target as CFRunLoopSourceCustom;
			if (source is null)
				return;

			source.OnPerform ();
		}

		protected abstract void OnPerform ();

		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				if (gch.IsAllocated)
					gch.Free ();
			}
			base.Dispose (disposing);
		}
	}
#endif

	public partial class CFRunLoop : NativeObject {
#if !COREBUILD
		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFRunLoopRef */ IntPtr CFRunLoopGetCurrent ();

		static public CFRunLoop Current {
			get {
				return new CFRunLoop (CFRunLoopGetCurrent (), false);
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFRunLoopRef */ IntPtr CFRunLoopGetMain ();

		static public CFRunLoop Main {
			get {
				return new CFRunLoop (CFRunLoopGetMain (), false);
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFRunLoopRun ();

		public void Run ()
		{
			CFRunLoopRun ();
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFRunLoopStop (/* CFRunLoopRef */ IntPtr rl);

		public void Stop ()
		{
			CFRunLoopStop (Handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFRunLoopWakeUp (/* CFRunLoopRef */ IntPtr rl);

		public void WakeUp ()
		{
			CFRunLoopWakeUp (Handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* Boolean */ byte CFRunLoopIsWaiting (/* CFRunLoopRef */ IntPtr rl);

		public bool IsWaiting {
			get {
				return CFRunLoopIsWaiting (Handle) != 0;
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static CFRunLoopExitReason /* SInt32 */ CFRunLoopRunInMode (/* CFStringRef */ IntPtr mode,
			/* CFTimeInterval */ double seconds,
			/* Boolean */ byte returnAfterSourceHandled);

		public CFRunLoopExitReason RunInMode (NSString mode, double seconds, bool returnAfterSourceHandled)
		{
			if (mode is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (mode));

			return CFRunLoopRunInMode (mode.Handle, seconds, returnAfterSourceHandled ? (byte) 1 : (byte) 0);
		}

		public CFRunLoopExitReason RunInMode (string mode, double seconds, bool returnAfterSourceHandled)
		{
			return RunInMode ((NSString) mode, seconds, returnAfterSourceHandled);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFRunLoopAddSource (/* CFRunLoopRef */ IntPtr rl, /* CFRunLoopSourceRef */ IntPtr source, /* CFStringRef */ IntPtr mode);

		public void AddSource (CFRunLoopSource source, NSString mode)
		{
			if (source is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (source));
			if (mode is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (mode));

			CFRunLoopAddSource (Handle, source.Handle, mode.Handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* Boolean */ byte CFRunLoopContainsSource (/* CFRunLoopRef */ IntPtr rl, /* CFRunLoopSourceRef */ IntPtr source, /* CFStringRef */ IntPtr mode);

		public bool ContainsSource (CFRunLoopSource source, NSString mode)
		{
			if (source is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (source));
			if (mode is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (mode));

			return CFRunLoopContainsSource (Handle, source.Handle, mode.Handle) != 0;
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFRunLoopRemoveSource (/* CFRunLoopRef */ IntPtr rl, /* CFRunLoopSourceRef */ IntPtr source, /* CFStringRef */ IntPtr mode);

		public void RemoveSource (CFRunLoopSource source, NSString mode)
		{
			if (source is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (source));
			if (mode is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (mode));

			CFRunLoopRemoveSource (Handle, source.Handle, mode.Handle);
		}

		[Preserve (Conditional = true)]
		internal CFRunLoop (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static NativeHandle /* CFArrayRef */ CFRunLoopCopyAllModes (NativeHandle /* CFRunLoopRef */ rl);

		public string? []? AllModes {
			get {
				return CFArray.StringArrayFromHandle (CFRunLoopCopyAllModes (GetCheckedHandle ()), releaseHandle: true);
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static IntPtr /* CFRunLoopMode */ CFRunLoopCopyCurrentMode (NativeHandle /* CFRunLoopRef */ rl);

		public string? CurrentMode {
			get {
				return CFString.FromHandle (CFRunLoopCopyCurrentMode (GetCheckedHandle ()), releaseHandle: true);
			}
		}
#endif // !COREBUILD
	}
}
