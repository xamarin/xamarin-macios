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

using System;
using System.Runtime.InteropServices;

using XamCore.ObjCRuntime;
using XamCore.Foundation;

#if XAMCORE_2_0
using CFIndex = System.nint;
#endif

namespace XamCore.CoreFoundation {

	// anonymous and typeless native enum - System/Library/Frameworks/CoreFoundation.framework/Headers/CFRunLoop.h
	public enum CFRunLoopExitReason : int {
		Finished = 1,
		Stopped = 2,
		TimedOut = 3,
		HandledSource = 4
	}

	// CFRunLoop.h
	[StructLayout (LayoutKind.Sequential)]
	internal struct CFRunLoopSourceContext {
		public CFIndex Version;
		public IntPtr Info;
		public IntPtr Retain;
		public IntPtr Release;
		public IntPtr CopyDescription;
		public IntPtr Equal;
		public IntPtr Hash;
		public IntPtr Schedule;
		public IntPtr Cancel;
		public IntPtr Perform;
	}

	public class CFRunLoopSource : INativeObject, IDisposable {
		internal IntPtr handle;

		public CFRunLoopSource (IntPtr handle)
			: this (handle, false)
		{
		}

		public CFRunLoopSource (IntPtr handle, bool ownsHandle)
		{
			if (!ownsHandle)
				CFObject.CFRetain (handle);
			this.handle = handle;
		}

		~CFRunLoopSource ()
		{
			Dispose (false);
		}

		public IntPtr Handle {
			get {
				return handle;
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFIndex */ nint CFRunLoopSourceGetOrder (/* CFRunLoopSourceRef */ IntPtr source);

		public nint Order {
			get {
				return CFRunLoopSourceGetOrder (handle);
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFRunLoopSourceInvalidate (/* CFRunLoopSourceRef */ IntPtr source);

		public void Invalidate ()
		{
			CFRunLoopSourceInvalidate (handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static /* Boolean */ bool CFRunLoopSourceIsValid (/* CFRunLoopSourceRef */ IntPtr source);

		public bool IsValid {
			get {
				return CFRunLoopSourceIsValid (handle);
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFRunLoopSourceSignal (/* CFRunLoopSourceRef */ IntPtr source);

		public void Signal ()
		{
			CFRunLoopSourceSignal (handle);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

#if XAMCORE_2_0
		protected virtual void Dispose (bool disposing)
#else
		public virtual void Dispose (bool disposing)
#endif
		{
			if (handle != IntPtr.Zero) {
				CFObject.CFRelease (handle);
				handle = IntPtr.Zero;
			}
		}
	}

#if !COREBUILD
	public abstract class CFRunLoopSourceCustom : CFRunLoopSource {
		GCHandle gch;

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFRunLoopSourceRef */ IntPtr CFRunLoopSourceCreate (/* CFAllocatorRef */ IntPtr allocator, /* CFIndex */ nint order, /* CFRunLoopSourceContext* */ IntPtr context);

		static ScheduleCallback ScheduleDelegate = (ScheduleCallback) Schedule;
		static CancelCallback CancelDelegate = (CancelCallback) Cancel;
		static PerformCallback PerformDelegate = (PerformCallback) Perform;
		
		protected CFRunLoopSourceCustom ()
			: base (IntPtr.Zero, true)
		{
			gch = GCHandle.Alloc (this);
			var ctx = new CFRunLoopSourceContext ();
			ctx.Info = GCHandle.ToIntPtr (gch);
			ctx.Schedule = Marshal.GetFunctionPointerForDelegate (ScheduleDelegate);
			ctx.Cancel = Marshal.GetFunctionPointerForDelegate (CancelDelegate);
			ctx.Perform = Marshal.GetFunctionPointerForDelegate (PerformDelegate);

			var ptr = Marshal.AllocHGlobal (Marshal.SizeOf (typeof(CFRunLoopSourceContext)));
			try {
				Marshal.StructureToPtr (ctx, ptr, false);
				handle = CFRunLoopSourceCreate (IntPtr.Zero, 0, ptr);
			} finally {
				Marshal.FreeHGlobal (ptr);
			}

			if (handle == IntPtr.Zero)
				throw new NotSupportedException ();
		}

		delegate void ScheduleCallback (IntPtr info, IntPtr runLoop, IntPtr mode);

		[MonoPInvokeCallback (typeof(ScheduleCallback))]
		static void Schedule (IntPtr info, IntPtr runLoop, IntPtr mode)
		{
			var source = GCHandle.FromIntPtr (info).Target as CFRunLoopSourceCustom;

			using (var loop = new CFRunLoop (runLoop))
			using (var mstring = new NSString (mode)) {
#if XAMCORE_2_0
				source.OnSchedule (loop, mstring);
#else
				source.OnSchedule (loop, (string)mstring);
#endif
			}
		}

#if XAMCORE_2_0
		protected abstract void OnSchedule (CFRunLoop loop, NSString mode);
#else
		protected abstract void OnSchedule (CFRunLoop loop, string mode);
#endif

		delegate void CancelCallback (IntPtr info, IntPtr runLoop, IntPtr mode);

		[MonoPInvokeCallback (typeof(CancelCallback))]
		static void Cancel (IntPtr info, IntPtr runLoop, IntPtr mode)
		{
			var source = GCHandle.FromIntPtr (info).Target as CFRunLoopSourceCustom;

			using (var loop = new CFRunLoop (runLoop))
			using (var mstring = new NSString (mode)) {
#if XAMCORE_2_0
				source.OnCancel (loop, mstring);
#else
				source.OnCancel (loop, (string)mstring);
#endif
			}
		}

#if XAMCORE_2_0
		protected abstract void OnCancel (CFRunLoop loop, NSString mode);
#else
		protected abstract void OnCancel (CFRunLoop loop, string mode);
#endif

		delegate void PerformCallback (IntPtr info);

		[MonoPInvokeCallback (typeof(PerformCallback))]
		static void Perform (IntPtr info)
		{
			var source = GCHandle.FromIntPtr (info).Target as CFRunLoopSourceCustom;
			source.OnPerform ();
		}

		protected abstract void OnPerform ();

#if XAMCORE_2_0
		protected override void Dispose (bool disposing)
#else
		public override void Dispose (bool disposing)
#endif
		{
			if (disposing) {
				if (gch.IsAllocated)
					gch.Free ();
			}
			base.Dispose (disposing);
		}
	}
#endif

	public partial class CFRunLoop
#if !COREBUILD
		: INativeObject, IDisposable
#endif
	{
#if !COREBUILD
		internal IntPtr handle;

#if !XAMCORE_2_0
		public static NSString CFDefaultRunLoopMode {
			get { return ModeDefault; }
		}

		public static NSString CFRunLoopCommonModes {
			get { return ModeCommon; }
		}
#endif
		
		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFRunLoopRef */ IntPtr CFRunLoopGetCurrent ();

		static public CFRunLoop Current {
			get {
				return new CFRunLoop (CFRunLoopGetCurrent ());
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFRunLoopRef */ IntPtr CFRunLoopGetMain ();
		
		static public CFRunLoop Main {
			get {
				return new CFRunLoop (CFRunLoopGetMain ());
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
			CFRunLoopStop (handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFRunLoopWakeUp (/* CFRunLoopRef */ IntPtr rl);

		public void WakeUp ()
		{
			CFRunLoopWakeUp (handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static /* Boolean */ bool CFRunLoopIsWaiting (/* CFRunLoopRef */ IntPtr rl);

		public bool IsWaiting {
			get {
				return CFRunLoopIsWaiting (handle);
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static int /* SInt32 */ CFRunLoopRunInMode (/* CFStringRef */ IntPtr mode, 
			/* CFTimeInterval */ double seconds, 
			/* Boolean */ [MarshalAs (UnmanagedType.I1)] bool returnAfterSourceHandled);

		public CFRunLoopExitReason RunInMode (NSString mode, double seconds, bool returnAfterSourceHandled)
		{
			if (mode == null)
				throw new ArgumentNullException ("mode");

			return (CFRunLoopExitReason) CFRunLoopRunInMode (mode.Handle, seconds, returnAfterSourceHandled);
		}

#if !XAMCORE_2_0
		[Obsolete ("Use the NSString version of CFRunLoop.RunInMode() instead.")]
		public CFRunLoopExitReason RunInMode (string mode, double seconds, bool returnAfterSourceHandled)
		{
			// .ctor will check for ANE
			using (CFString s = new CFString (mode)) {
				return (CFRunLoopExitReason) CFRunLoopRunInMode (s.Handle, seconds, returnAfterSourceHandled);
			}
		}
#endif

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFRunLoopAddSource (/* CFRunLoopRef */ IntPtr rl, /* CFRunLoopSourceRef */ IntPtr source, /* CFStringRef */ IntPtr mode);

		public void AddSource (CFRunLoopSource source, NSString mode)
		{
			if (source == null)
				throw new ArgumentNullException ("source");
			if (mode == null)
				throw new ArgumentNullException ("mode");

			CFRunLoopAddSource (handle, source.Handle, mode.Handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static /* Boolean */ bool CFRunLoopContainsSource (/* CFRunLoopRef */ IntPtr rl, /* CFRunLoopSourceRef */ IntPtr source, /* CFStringRef */ IntPtr mode);

		public bool ContainsSource (CFRunLoopSource source, NSString mode)
		{
			if (source == null)
				throw new ArgumentNullException ("source");
			if (mode == null)
				throw new ArgumentNullException ("mode");

			return CFRunLoopContainsSource (handle, source.Handle, mode.Handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFRunLoopRemoveSource (/* CFRunLoopRef */ IntPtr rl, /* CFRunLoopSourceRef */ IntPtr source, /* CFStringRef */ IntPtr mode);

#if XAMCORE_2_0
		public void RemoveSource (CFRunLoopSource source, NSString mode)
#else
		public bool RemoveSource (CFRunLoopSource source, NSString mode)
#endif
		{
			if (source == null)
				throw new ArgumentNullException ("source");
			if (mode == null)
				throw new ArgumentNullException ("mode");

			CFRunLoopRemoveSource (handle, source.Handle, mode.Handle);
#if !XAMCORE_2_0
			return true;
#endif
		}

		internal CFRunLoop (IntPtr handle)
			: this (handle, false)
		{
		}

		[Preserve (Conditional = true)]
		internal CFRunLoop (IntPtr handle, bool owns)
		{
			if (!owns)
				CFObject.CFRetain (handle);
			this.handle = handle;
		}

		~CFRunLoop ()
		{
			Dispose (false);
		}

		public IntPtr Handle {
			get {
				return handle;
			}
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

#if XAMCORE_2_0
		protected virtual void Dispose (bool disposing)
#else
		public virtual void Dispose (bool disposing)
#endif
		{
			if (handle != IntPtr.Zero){
				CFObject.CFRelease (handle);
				handle = IntPtr.Zero;
			}
		}

		public static bool operator == (CFRunLoop a, CFRunLoop b)
		{
			return Object.Equals (a, b);
		}

		public static bool operator != (CFRunLoop a, CFRunLoop b)
		{
			return !Object.Equals (a, b);
		}

		public override int GetHashCode ()
		{
			return handle.GetHashCode ();
		}

		public override bool Equals (object other)
		{
			CFRunLoop cfother = other as CFRunLoop;
			if (cfother == null)
				return false;

			return cfother.Handle == handle;
		}
#endif // !COREBUILD
	}
}
