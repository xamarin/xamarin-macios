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

using CFIndex = System.IntPtr;

#nullable enable

namespace CoreFoundation {

	// anonymous and typeless native enum - System/Library/Frameworks/CoreFoundation.framework/Headers/CFRunLoop.h
	/// <summary>The reason for a <see cref="CoreFoundation.CFRunLoop" /> to stop running.</summary>
	///     <remarks>
	///       <para>
	///       </para>
	///     </remarks>
	[NativeName ("CFRunLoopRunResult")]
	public enum CFRunLoopExitReason : int {
		/// <summary>The run loop terminated.</summary>
		Finished = 1,
		/// <summary>The run loop was stopped by a call to the <see cref="CoreFoundation.CFRunLoop.Stop" /> method.</summary>
		Stopped = 2,
		/// <summary>The number of seconds specified in the call to <see cref="CoreFoundation.CFRunLoop.RunInMode(Foundation.NSString,System.Double,System.Boolean)" /> elapsed.</summary>
		TimedOut = 3,
		/// <summary>An event from a source was handled, and the developer specified that a single source should be processed on the call to <see cref="CoreFoundation.CFRunLoop.RunInMode(Foundation.NSString,System.Double,System.Boolean)" /></summary>
		HandledSource = 4,
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

	/// <summary>An input source that generates asynchronous events and is intended to be used with a <see cref="CoreFoundation.CFRunLoop" />.</summary>
	///     <remarks>To be added.</remarks>
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

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public nint Order {
			get {
				return CFRunLoopSourceGetOrder (Handle);
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFRunLoopSourceInvalidate (/* CFRunLoopSourceRef */ IntPtr source);

		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public void Invalidate ()
		{
			CFRunLoopSourceInvalidate (Handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* Boolean */ byte CFRunLoopSourceIsValid (/* CFRunLoopSourceRef */ IntPtr source);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool IsValid {
			get {
				return CFRunLoopSourceIsValid (Handle) != 0;
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFRunLoopSourceSignal (/* CFRunLoopSourceRef */ IntPtr source);

		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public void Signal ()
		{
			CFRunLoopSourceSignal (Handle);
		}
	}

#if !COREBUILD
	/// <summary>An abstract <see cref="CoreFoundation.CFRunLoop" /> that, when extended, gives the application developer fine-grained control over lifecycle events. </summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public abstract class CFRunLoopSourceCustom : CFRunLoopSource {
		GCHandle gch;

		[DllImport (Constants.CoreFoundationLibrary)]
		unsafe extern static /* CFRunLoopSourceRef */ IntPtr CFRunLoopSourceCreate (/* CFAllocatorRef */ IntPtr allocator, /* CFIndex */ nint order, /* CFRunLoopSourceContext* */ CFRunLoopSourceContext* context);

		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
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

		/// <param name="loop">To be added.</param>
		///         <param name="mode">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
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

		/// <param name="loop">To be added.</param>
		///         <param name="mode">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		protected abstract void OnCancel (CFRunLoop loop, NSString mode);

		[UnmanagedCallersOnly]
		static void Perform (IntPtr info)
		{
			var source = GCHandle.FromIntPtr (info).Target as CFRunLoopSourceCustom;
			if (source is null)
				return;

			source.OnPerform ();
		}

		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		protected abstract void OnPerform ();

		/// <inheritdoc />
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

		/// <summary>Active runloop for the current thread.</summary>
		///         <value>
		///           <para>
		///           </para>
		///         </value>
		///         <remarks>
		///           <para>
		///           </para>
		///         </remarks>
		static public CFRunLoop Current {
			get {
				return new CFRunLoop (CFRunLoopGetCurrent (), false);
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFRunLoopRef */ IntPtr CFRunLoopGetMain ();

		/// <summary>Main run loop object for the application.</summary>
		///         <value>The main runloop object for the application.</value>
		///         <remarks>
		///           <para>
		///           </para>
		///         </remarks>
		static public CFRunLoop Main {
			get {
				return new CFRunLoop (CFRunLoopGetMain (), false);
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFRunLoopRun ();

		/// <summary>Starts the <see cref="CoreFoundation.CFRunLoop" /> for the current thread.</summary>
		///         <remarks>
		///           <para>Run the runloop in the default mode.</para>
		///           <para>
		///           </para>
		///           <para>The run loop can be stopped by calling <see cref="CoreFoundation.CFRunLoop.Stop" />. </para>
		///           <para>
		///           </para>
		///           <para>The run loop can be determinated if all the sources and timers are removed from it.</para>
		///         </remarks>
		public void Run ()
		{
			CFRunLoopRun ();
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFRunLoopStop (/* CFRunLoopRef */ IntPtr rl);

		/// <summary>Stops execution of this runloop.</summary>
		///         <remarks>To be added.</remarks>
		public void Stop ()
		{
			CFRunLoopStop (Handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFRunLoopWakeUp (/* CFRunLoopRef */ IntPtr rl);

		/// <summary>Wakes up a sleeping runloop.</summary>
		///         <remarks>To be added.</remarks>
		public void WakeUp ()
		{
			CFRunLoopWakeUp (Handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* Boolean */ byte CFRunLoopIsWaiting (/* CFRunLoopRef */ IntPtr rl);

		/// <summary>Indicates that the run loop is currently waiting for an event.</summary>
		///         <value>
		///           <see langword="true" /> if the run loop is waiting for an event, <see langword="false" /> otherwise.</value>
		///         <remarks>
		///           <para>
		///           </para>
		///         </remarks>
		public bool IsWaiting {
			get {
				return CFRunLoopIsWaiting (Handle) != 0;
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static CFRunLoopExitReason /* SInt32 */ CFRunLoopRunInMode (/* CFStringRef */ IntPtr mode,
			/* CFTimeInterval */ double seconds,
			/* Boolean */ byte returnAfterSourceHandled);

		/// <param name="mode">
		///
		///
		///
		///
		///
		/// Mode to execute the runloop on.   This can be any arbitrary string.
		///
		///
		///
		///
		///   	   </param>
		///         <param name="seconds">
		///
		///
		///
		///
		/// Number of seconds to execute the run loop for.   If seconds is zero, the run loop performs a single pass.
		///
		///
		///
		///
		///   	   </param>
		///         <param name="returnAfterSourceHandled">
		///
		///
		///
		///
		/// If <see langword="true" />, processing will return after a single source has been processed.   If <see langword="false" />, then execution continues until the number of <see langword="seconds" /> has elapsed.
		///
		///
		///
		///
		///   	   </param>
		///         <summary>Initiates the runloop for a a particular duration of time.</summary>
		///         <returns>
		///
		///
		///
		///
		///
		/// Status indicating the reason for the run loop to complete executing.
		///
		///
		///
		///
		///      </returns>
		///         <remarks>To be added.</remarks>
		public CFRunLoopExitReason RunInMode (NSString mode, double seconds, bool returnAfterSourceHandled)
		{
			if (mode is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (mode));

			CFRunLoopExitReason result = CFRunLoopRunInMode (mode.Handle, seconds, returnAfterSourceHandled ? (byte) 1 : (byte) 0);
			GC.KeepAlive (mode);
			return result;
		}

		public CFRunLoopExitReason RunInMode (string mode, double seconds, bool returnAfterSourceHandled)
		{
			return RunInMode ((NSString) mode, seconds, returnAfterSourceHandled);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFRunLoopAddSource (/* CFRunLoopRef */ IntPtr rl, /* CFRunLoopSourceRef */ IntPtr source, /* CFStringRef */ IntPtr mode);

		/// <summary>Adds a new source to the run loop on the specified mode.</summary>
		/// <param name="source">Source to be added.</param>
		/// <param name="mode">The mode to add the source to. If you use <see cref="NSRunLoopMode.Common" /> the source is added to all common modes.</param>
		public void AddSource (CFRunLoopSource source, NSString mode)
		{
			if (source is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (source));
			if (mode is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (mode));

			CFRunLoopAddSource (Handle, source.Handle, mode.Handle);
			GC.KeepAlive (source);
			GC.KeepAlive (mode);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* Boolean */ byte CFRunLoopContainsSource (/* CFRunLoopRef */ IntPtr rl, /* CFRunLoopSourceRef */ IntPtr source, /* CFStringRef */ IntPtr mode);

		/// <param name="source">The source to probe.</param>
		///         <param name="mode">The mode to probe into.</param>
		///         <summary>Determines whether the run loop contains the specified <see cref="CoreFoundation.CFRunLoopSource" /> on a specific mode.</summary>
		///         <returns>
		///           <see langword="true" /> if the runloop contains the specified source in the specified mode.</returns>
		///         <remarks>
		///           <para />
		///         </remarks>
		public bool ContainsSource (CFRunLoopSource source, NSString mode)
		{
			if (source is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (source));
			if (mode is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (mode));

			bool result = CFRunLoopContainsSource (Handle, source.Handle, mode.Handle) != 0;
			GC.KeepAlive (source);
			GC.KeepAlive (mode);
			return result;
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFRunLoopRemoveSource (/* CFRunLoopRef */ IntPtr rl, /* CFRunLoopSourceRef */ IntPtr source, /* CFStringRef */ IntPtr mode);

		/// <summary>Removes a source from the runloop.</summary>
		/// <param name="source">Run loop source to remove.</param>
		/// <param name="mode">The mode to remove it from. If you use <see cref="NSRunLoopMode.Common" /> the source is removed from all common modes.</param>
		public void RemoveSource (CFRunLoopSource source, NSString mode)
		{
			if (source is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (source));
			if (mode is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (mode));

			CFRunLoopRemoveSource (Handle, source.Handle, mode.Handle);
			GC.KeepAlive (source);
			GC.KeepAlive (mode);
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
