// 
// CMSync.cs: Implements the managed CMSync infrastructure
//
// Authors: Marek Safar (marek.safar@gmail.com)
//     
// Copyright 2012-2014 Xamarin Inc
//

using System;
using System.Runtime.InteropServices;

using Foundation;
using CoreFoundation;
using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

#nullable enable

namespace CoreMedia {

	// CMSync.h
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#else
	[Watch (6, 0)]
#endif
	public class CMClock : CMClockOrTimebase {
#if !NET
		public CMClock (NativeHandle handle) : base (handle)
		{
		}
#endif

		[Preserve (Conditional = true)]
		internal CMClock (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}
#if !COREBUILD

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* CMClockRef */ IntPtr CMClockGetHostTimeClock ();

		public static CMClock HostTimeClock {
			get {
				return new CMClock (CMClockGetHostTimeClock (), false);
			}
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static CMTime CMClockGetTime (/* CMClockRef */ IntPtr clock);

		public CMTime CurrentTime {
			get {
				return CMClockGetTime (Handle);
			}
		}

#if !MONOMAC
		// CMAudioClock.h
#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMClockError CMAudioClockCreate (/* CFAllocatorRef */ IntPtr allocator, /* CMClockRef* */ out IntPtr clockOut);

		public static CMClock? CreateAudioClock (out CMClockError clockError)
		{
			IntPtr ptr;
			clockError = CMAudioClockCreate (IntPtr.Zero, out ptr);
			return clockError == CMClockError.None ? new CMClock (ptr, true) : null;
		}
#endif

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMClockError CMClockGetAnchorTime (/* CMClockRef */ IntPtr clock, out CMTime outClockTime, out CMTime outReferenceClockTime);

		public CMClockError GetAnchorTime (out CMTime clockTime, out CMTime referenceClockTime)
		{
			return CMClockGetAnchorTime (Handle, out clockTime, out referenceClockTime);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static /* Boolean */ bool CMClockMightDrift (/* CMClockRef */ IntPtr clock, /* CMClockRef */ IntPtr otherClock);

		public bool MightDrift (CMClock otherClock)
		{
			if (otherClock is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (otherClock));

			return CMClockMightDrift (Handle, otherClock.Handle);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static void CMClockInvalidate (/* CMClockRef */ IntPtr clock);

		public void Invalidate ()
		{
			CMClockInvalidate (Handle);
		}

		[DllImport (Constants.CoreMediaLibrary, EntryPoint = "CMClockConvertHostTimeToSystemUnits")]
		public extern static /* uint64_t */ ulong ConvertHostTimeToSystemUnits (CMTime hostTime);

		[DllImport (Constants.CoreMediaLibrary, EntryPoint = "CMClockMakeHostTimeFromSystemUnits")]
		public extern static CMTime CreateHostTimeFromSystemUnits (/* uint64_t */ ulong hostTime);
#endif // !COREBUILD
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#else
	[Watch (6, 0)]
#endif
	public class CMTimebase : CMClockOrTimebase {
#if !NET
		public CMTimebase (NativeHandle handle)
			: base (handle)
		{
		}
#endif

		[Preserve (Conditional = true)]
		private CMTimebase (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}
#if !COREBUILD

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("maccatalyst13.0")]
		[ObsoletedOSPlatform ("tvos9.0")]
		[ObsoletedOSPlatform ("macos10.10")]
		[ObsoletedOSPlatform ("ios9.0")]
#else
		[Deprecated (PlatformName.iOS, 8, 0)]
		[Deprecated (PlatformName.TvOS, 9, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 10)]
		[Deprecated (PlatformName.WatchOS, 6, 0)]
#endif
		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMTimebaseError CMTimebaseCreateWithMasterClock (/* CFAllocatorRef */ IntPtr allocator, /* CMClockRef */ IntPtr masterClock, /* CMTimebaseRef* */ out IntPtr timebaseOut);

		static IntPtr Create (CMClock masterClock)
		{
			if (masterClock is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (masterClock));

			var error = CMTimebaseCreateWithMasterClock (IntPtr.Zero, masterClock.Handle, out var handle);
			if (error != CMTimebaseError.None)
				ObjCRuntime.ThrowHelper.ThrowArgumentException (error.ToString ());
			return handle;
		}

		public CMTimebase (CMClock masterClock)
			: base (Create (masterClock), true)
		{
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("tvos9.0")]
		[ObsoletedOSPlatform ("macos10.10")]
		[ObsoletedOSPlatform ("ios8.0")]
		[ObsoletedOSPlatform ("maccatalyst13.0")]
#else
		[Deprecated (PlatformName.iOS, 8, 0)]
		[Deprecated (PlatformName.TvOS, 9, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 10)]
		[Deprecated (PlatformName.WatchOS, 6, 0)]
#endif
		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMTimebaseError CMTimebaseCreateWithMasterTimebase (/* CFAllocatorRef */ IntPtr allocator, /* CMTimebaseRef */ IntPtr masterTimebase, /* CMTimebaseRef* */ out IntPtr timebaseOut);

		static IntPtr Create (CMTimebase masterTimebase)
		{
			if (masterTimebase is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (masterTimebase));

			var error = CMTimebaseCreateWithMasterTimebase (IntPtr.Zero, masterTimebase.Handle, out var handle);
			if (error != CMTimebaseError.None)
				ObjCRuntime.ThrowHelper.ThrowArgumentException (error.ToString ());
			return handle;
		}

		public CMTimebase (CMTimebase masterTimebase)
			: base (Create (masterTimebase), true)
		{
		}

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		[DllImport (Constants.CoreMediaLibrary)]
		static extern CMTimebaseError CMTimebaseCreateWithSourceClock (/* [NullAllowed] CFAllocatorRef */ IntPtr allocator, /* CMClock */ IntPtr sourceClock, /* CMTimebase */ out IntPtr timebaseOut);

		static IntPtr Create (CFAllocator? allocator, CMClock sourceClock)
		{
			if (sourceClock is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (sourceClock));

			var error = CMTimebaseCreateWithSourceClock (allocator.GetHandle (), sourceClock.Handle, out var handle);
			if (error != CMTimebaseError.None)
				ObjCRuntime.ThrowHelper.ThrowArgumentException (error.ToString ());
			return handle;
		}

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		public CMTimebase (CFAllocator? allocator, CMClock sourceClock)
			: base (Create (allocator, sourceClock), true)
		{
		}

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		[DllImport (Constants.CoreMediaLibrary)]
		static extern CMTimebaseError CMTimebaseCreateWithSourceTimebase (/* [NullAllowed] CFAllocatorRef */ IntPtr allocator, /* CMTimebase */ IntPtr sourceTimebase, /* CMTimebase */ out IntPtr timebaseOut);

		static IntPtr Create (CFAllocator? allocator, CMTimebase sourceTimebase)
		{
			if (sourceTimebase is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (sourceTimebase));

			var error = CMTimebaseCreateWithSourceTimebase (allocator.GetHandle (), sourceTimebase.Handle, out var handle);
			if (error != CMTimebaseError.None)
				ObjCRuntime.ThrowHelper.ThrowArgumentException (error.ToString ());
			return handle;
		}

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		public CMTimebase (CFAllocator? allocator, CMTimebase sourceTimebase)
			: base (Create (allocator, sourceTimebase), true)
		{
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* Float64 */ double CMTimebaseGetEffectiveRate (/* CMTimebaseRef */ IntPtr timebase);

		public double EffectiveRate {
			get {
				return CMTimebaseGetEffectiveRate (Handle);
			}
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* Float64 */ double CMTimebaseGetRate (/* CMTimebaseRef */ IntPtr timebase);

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMTimebaseError CMTimebaseSetRate (/* CMTimebaseRef */ IntPtr timebase, /* Float64 */ double rate);

		public double Rate {
			get {
				return CMTimebaseGetRate (Handle);
			}
			set {
				var error = CMTimebaseSetRate (Handle, value);
				if (error != CMTimebaseError.None)
					ObjCRuntime.ThrowHelper.ThrowArgumentException (error.ToString ());
			}
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static CMTime CMTimebaseGetTime (/* CMTimebaseRef */ IntPtr timebase);

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMTimebaseError CMTimebaseSetTime (/* CMTimebaseRef */ IntPtr timebase, CMTime time);

		public new CMTime Time {
			get {
				return CMTimebaseGetTime (Handle);
			}
			set {
				var error = CMTimebaseSetTime (Handle, value);
				if (error != CMTimebaseError.None)
					ObjCRuntime.ThrowHelper.ThrowArgumentException (error.ToString ());
			}
		}

#if !WATCH
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("maccatalyst13.0")]
		[ObsoletedOSPlatform ("tvos9.0")]
		[ObsoletedOSPlatform ("macos10.10")]
		[ObsoletedOSPlatform ("ios9.0")]
#else
		[Deprecated (PlatformName.iOS, 9, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 11)]
		[Deprecated (PlatformName.TvOS, 9, 0)]
#endif
		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* CMTimebaseRef */ IntPtr CMTimebaseGetMasterTimebase (/* CMTimebaseRef */ IntPtr timebase);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.11", "Use 'CopyMasterTimebase' instead.")]
		[ObsoletedOSPlatform ("ios9.0", "Use 'CopyMasterTimebase' instead.")]
		[ObsoletedOSPlatform ("tvos9.0", "Use 'CopyMasterTimebase' instead.")]
		[ObsoletedOSPlatform ("maccatalyst13.0", "Use 'CopyMasterTimebase' instead.")]
#else
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'CopyMasterTimebase' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'CopyMasterTimebase' instead.")]
#endif
		public CMTimebase? GetMasterTimebase ()
		{
			var ptr = CMTimebaseGetMasterTimebase (Handle);
			if (ptr == IntPtr.Zero)
				return null;

			return new CMTimebase (ptr, false);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("maccatalyst13.0")]
		[ObsoletedOSPlatform ("tvos9.0")]
		[ObsoletedOSPlatform ("macos10.10")]
		[ObsoletedOSPlatform ("ios9.0")]
#else
		[Deprecated (PlatformName.iOS, 9, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 11)]
		[Deprecated (PlatformName.TvOS, 9, 0)]
#endif
		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* CMClockRef */ IntPtr CMTimebaseGetMasterClock (/* CMTimebaseRef */ IntPtr timebase);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.11", "Use 'CopyMasterClock' instead.")]
		[ObsoletedOSPlatform ("ios9.0", "Use 'CopyMasterClock' instead.")]
		[ObsoletedOSPlatform ("tvos9.0", "Use 'CopyMasterClock' instead.")]
		[ObsoletedOSPlatform ("maccatalyst13.0", "Use 'CopyMasterClock' instead.")]
#else
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'CopyMasterClock' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'CopyMasterClock' instead.")]
#endif
		public CMClock? GetMasterClock ()
		{
			var ptr = CMTimebaseGetMasterClock (Handle);
			if (ptr == IntPtr.Zero)
				return null;

			return new CMClock (ptr, false);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("maccatalyst13.0")]
		[ObsoletedOSPlatform ("tvos9.0")]
		[ObsoletedOSPlatform ("macos10.11")]
		[ObsoletedOSPlatform ("ios9.0")]
#else
		[Deprecated (PlatformName.iOS, 9, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 11)]
		[Deprecated (PlatformName.TvOS, 9, 0)]
#endif
		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* CMClockOrTimebaseRef */ IntPtr CMTimebaseGetMaster (/* CMTimebaseRef */ IntPtr timebase);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.11", "Use 'CopyMaster' instead.")]
		[ObsoletedOSPlatform ("ios9.0", "Use 'CopyMaster' instead.")]
		[ObsoletedOSPlatform ("tvos9.0", "Use 'CopyMaster' instead.")]
		[ObsoletedOSPlatform ("maccatalyst13.0", "Use 'CopyMaster' instead.")]
#else
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'CopyMaster' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'CopyMaster' instead.")]
#endif
		public CMClockOrTimebase? GetMaster ()
		{
			var ptr = CMTimebaseGetMaster (Handle);
			if (ptr == IntPtr.Zero)
				return null;

			return new CMClockOrTimebase (ptr, false);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("maccatalyst13.0")]
		[ObsoletedOSPlatform ("tvos9.0")]
		[ObsoletedOSPlatform ("macos10.11")]
		[ObsoletedOSPlatform ("ios9.0")]
#else
		[Deprecated (PlatformName.iOS, 9, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 11)]
		[Deprecated (PlatformName.TvOS, 9, 0)]
#endif
		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* CMClockRef */ IntPtr CMTimebaseGetUltimateMasterClock (/* CMTimebaseRef */ IntPtr timebase);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.11", "Use 'CopyUltimateMasterClock' instead.")]
		[ObsoletedOSPlatform ("ios9.0", "Use 'CopyUltimateMasterClock' instead.")]
		[ObsoletedOSPlatform ("tvos9.0", "Use 'CopyUltimateMasterClock' instead.")]
		[ObsoletedOSPlatform ("maccatalyst13.0", "Use 'CopyUltimateMasterClock' instead.")]
#else
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'CopyUltimateMasterClock' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'CopyUltimateMasterClock' instead.")]
#endif
		public CMClock? GetUltimateMasterClock ()
		{
			var ptr = CMTimebaseGetUltimateMasterClock (Handle);
			if (ptr == IntPtr.Zero)
				return null;

			return new CMClock (ptr, false);
		}
#endif

		[DllImport (Constants.CoreMediaLibrary)]
		extern static CMTime CMTimebaseGetTimeWithTimeScale (/* CMTimebaseRef */ IntPtr timebase, CMTimeScale timescale, CMTimeRoundingMethod method);

		public CMTime GetTime (CMTimeScale timeScale, CMTimeRoundingMethod roundingMethod)
		{
			return CMTimebaseGetTimeWithTimeScale (Handle, timeScale, roundingMethod);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMTimebaseError CMTimebaseSetAnchorTime (/* CMTimebaseRef */ IntPtr timebase, CMTime timebaseTime, CMTime immediateMasterTime);

		public CMTimebaseError SetAnchorTime (CMTime timebaseTime, CMTime immediateMasterTime)
		{
			return CMTimebaseSetAnchorTime (Handle, timebaseTime, immediateMasterTime);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMTimebaseError CMTimebaseGetTimeAndRate (/* CMTimebaseRef */ IntPtr timebase, out CMTime time, /* Float64* */ out double rate);

		public CMTimebaseError GetTimeAndRate (out CMTime time, out double rate)
		{
			return CMTimebaseGetTimeAndRate (Handle, out time, out rate);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* OSStatus */  CMTimebaseError CMTimebaseSetRateAndAnchorTime (/* CMTimebaseRef */ IntPtr timebase, /* Float64 */ double rate, CMTime timebaseTime, CMTime immediateMasterTime);

		public CMTimebaseError SetRateAndAnchorTime (double rate, CMTime timebaseTime, CMTime immediateMasterTime)
		{
			return CMTimebaseSetRateAndAnchorTime (Handle, rate, timebaseTime, immediateMasterTime);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMTimebaseError CMTimebaseNotificationBarrier (/* CMTimebaseRef */ IntPtr timebase);

		public CMTimebaseError NotificationBarrier ()
		{
			return CMTimebaseNotificationBarrier (Handle);
		}

		public const double VeryLongTimeInterval = 256.0 * 365.0 * 24.0 * 60.0 * 60.0;

#if !COREBUILD
		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMTimebaseError CMTimebaseAddTimer (/* CMTimebaseRef */ IntPtr timebase, /* CFRunLoopTimerRef */ IntPtr timer, /* CFRunLoopRef */ IntPtr runloop);

		public CMTimebaseError AddTimer (NSTimer timer, NSRunLoop runloop)
		{
			if (timer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (timer));
			if (runloop is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (runloop));

			// NSRunloop and CFRunloop[Ref] are NOT toll free bridged types
			using (var cf = runloop.GetCFRunLoop ())
				return CMTimebaseAddTimer (Handle, timer.Handle, cf.Handle);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMTimebaseError CMTimebaseRemoveTimer (/* CMTimebaseRef */ IntPtr timebase, /* CFRunLoopTimerRef */ IntPtr timer);

		public CMTimebaseError RemoveTimer (NSTimer timer)
		{
			if (timer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (timer));

			return CMTimebaseRemoveTimer (Handle, timer.Handle);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMTimebaseError CMTimebaseSetTimerNextFireTime (/* CMTimebaseRef */ IntPtr timebase, /* CFRunLoopTimerRef */ IntPtr timer, CMTime fireTime, /* uint32_t */ uint flags);

		public CMTimebaseError SetTimerNextFireTime (NSTimer timer, CMTime fireTime)
		{
			if (timer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (timer));

			return CMTimebaseSetTimerNextFireTime (Handle, timer.Handle, fireTime, 0);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMTimebaseError CMTimebaseSetTimerToFireImmediately (/* CMTimebaseRef */ IntPtr timebase, /* CFRunLoopTimerRef */ IntPtr timer);

		public CMTimebaseError SetTimerToFireImmediately (NSTimer timer)
		{
			if (timer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (timer));

			return CMTimebaseSetTimerToFireImmediately (Handle, timer.Handle);
		}

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("tvos9.0")]
		[ObsoletedOSPlatform ("macos10.10")]
		[ObsoletedOSPlatform ("ios8.0")]
#else
		[Deprecated (PlatformName.iOS, 8, 0)]
		[Deprecated (PlatformName.TvOS, 9, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 10)]
		[Deprecated (PlatformName.WatchOS, 6, 0)]
#endif
		[DllImport (Constants.CoreMediaLibrary)]
		extern static CMTimebaseError CMTimebaseSetMasterTimebase (/* CMTimebaseRef* */ IntPtr timebase, /* CMTimebaseRef* */ IntPtr newMasterTimebase);

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("maccatalyst13.0")]
		[ObsoletedOSPlatform ("tvos9.0")]
		[ObsoletedOSPlatform ("macos10.10")]
		[ObsoletedOSPlatform ("ios8.0")]
#else
		[Deprecated (PlatformName.iOS, 8, 0)]
		[Deprecated (PlatformName.TvOS, 9, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 10)]
		[Deprecated (PlatformName.WatchOS, 6, 0)]
#endif
		public CMTimebaseError SetMasterTimebase (CMTimebase newMasterTimebase)
		{
			if (newMasterTimebase is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (newMasterTimebase));

			return CMTimebaseSetMasterTimebase (Handle, newMasterTimebase.Handle);
		}

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("maccatalyst13.0")]
		[ObsoletedOSPlatform ("tvos9.0")]
		[ObsoletedOSPlatform ("macos10.10")]
		[ObsoletedOSPlatform ("ios8.0")]
#else
		[Deprecated (PlatformName.iOS, 8, 0)]
		[Deprecated (PlatformName.TvOS, 9, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 10)]
		[Deprecated (PlatformName.WatchOS, 6, 0)]
#endif
		[DllImport (Constants.CoreMediaLibrary)]
		extern static CMTimebaseError CMTimebaseSetMasterClock (/* CMTimebaseRef* */ IntPtr timebase, /* CMClockRef* */ IntPtr newMasterClock);

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("maccatalyst13.0")]
		[ObsoletedOSPlatform ("tvos9.0")]
		[ObsoletedOSPlatform ("macos10.10")]
		[ObsoletedOSPlatform ("ios8.0")]
#else
		[Deprecated (PlatformName.iOS, 8, 0)]
		[Deprecated (PlatformName.TvOS, 9, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 10)]
		[Deprecated (PlatformName.WatchOS, 6, 0)]
#endif
		public CMTimebaseError SetMasterClock (CMClock newMasterClock)
		{
			if (newMasterClock is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (newMasterClock));

			return CMTimebaseSetMasterClock (Handle, newMasterClock.Handle);
		}
#endif

#if !COREBUILD
		bool IsDeprecated ()
		{
#if __MACCATALYST__
			return true;
#elif IOS
			return SystemVersion.CheckiOS (9, 0);
#elif MONOMAC
			return SystemVersion.CheckmacOS (10, 11);
#elif TVOS || WATCH
			return true;
#endif
		}

		public CMTimebase? CopyMasterTimebase ()
		{
			IntPtr ptr = IntPtr.Zero;
			bool deprecated = IsDeprecated ();
			if (deprecated)
				ptr = CMTimebaseCopyMasterTimebase (Handle);
#if !WATCH
			else
				ptr = CMTimebaseGetMasterTimebase (Handle);
#endif

			if (ptr == IntPtr.Zero)
				return null;

			return new CMTimebase (ptr, deprecated);
		}

		public CMClock? CopyMasterClock ()
		{
			IntPtr ptr = IntPtr.Zero;
			bool deprecated = IsDeprecated ();
			if (deprecated)
				ptr = CMTimebaseCopyMasterClock (Handle);
#if !WATCH
			else
				ptr = CMTimebaseGetMasterClock (Handle);
#endif

			if (ptr == IntPtr.Zero)
				return null;

			return new CMClock (ptr, deprecated);
		}

		public CMClockOrTimebase? CopyMaster ()
		{
			IntPtr ptr = IntPtr.Zero;
			bool deprecated = IsDeprecated ();
			if (deprecated)
				ptr = CMTimebaseCopyMaster (Handle);
#if !WATCH
			else
				ptr = CMTimebaseGetMaster (Handle);
#endif

			if (ptr == IntPtr.Zero)
				return null;

			return new CMClockOrTimebase (ptr, deprecated);
		}

		public CMClock? CopyUltimateMasterClock ()
		{
			IntPtr ptr = IntPtr.Zero;
			bool deprecated = IsDeprecated ();
			if (deprecated)
				ptr = CMTimebaseCopyUltimateMasterClock (Handle);
#if !WATCH
			else
				ptr = CMTimebaseGetUltimateMasterClock (Handle);
#endif

			if (ptr == IntPtr.Zero)
				return null;

			return new CMClock (ptr, deprecated);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("tvos9.0", "Use 'CMTimebaseGetMasterTimebase' instead.")]
		[ObsoletedOSPlatform ("macos10.11", "Use 'CMTimebaseGetMasterTimebase' instead.")]
		[ObsoletedOSPlatform ("ios9.0", "Use 'CMTimebaseGetMasterTimebase' instead.")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[NoMacCatalyst]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'CMTimebaseGetMasterTimebase' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'CMTimebaseGetMasterTimebase' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'CMTimebaseGetMasterTimebase' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'CMTimebaseGetMasterTimebase' instead.")]
#endif
		[DllImport (Constants.CoreMediaLibrary)]
		static extern unsafe /* CMTimebaseRef */ IntPtr CMTimebaseCopyMasterTimebase (/* CMTimebaseRef */ IntPtr timebase);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("maccatalyst13.0", "Use 'CMTimebaseGetMasterClock' instead.")]
		[ObsoletedOSPlatform ("tvos9.0", "Use 'CMTimebaseGetMasterClock' instead.")]
		[ObsoletedOSPlatform ("macos10.11", "Use 'CMTimebaseGetMasterClock' instead.")]
		[ObsoletedOSPlatform ("ios9.0", "Use 'CMTimebaseGetMasterClock' instead.")]
#else
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'CMTimebaseGetMasterClock' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'CMTimebaseGetMasterClock' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'CMTimebaseGetMasterClock' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'CMTimebaseGetMasterClock' instead.")]
#endif
		[DllImport (Constants.CoreMediaLibrary)]
		static extern unsafe /* CMClockRef */ IntPtr CMTimebaseCopyMasterClock (/* CMTimebaseRef */ IntPtr timebase);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("tvos9.0", "Use 'CMTimebaseGetMaster' instead.")]
		[ObsoletedOSPlatform ("macos10.11", "Use 'CMTimebaseGetMaster' instead.")]
		[ObsoletedOSPlatform ("ios9.0", "Use 'CMTimebaseGetMaster' instead.")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[NoMacCatalyst]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'CMTimebaseGetMaster' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'CMTimebaseGetMaster' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'CMTimebaseGetMaster' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'CMTimebaseGetMaster' instead.")]
#endif
		[DllImport (Constants.CoreMediaLibrary)]
		static extern unsafe IntPtr /* void* */ CMTimebaseCopyMaster (/* CMTimebaseRef */ IntPtr timebase);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("tvos9.0", "Use 'CMTimebaseGetUltimateMasterClock' instead.")]
		[ObsoletedOSPlatform ("macos10.11", "Use 'CMTimebaseGetUltimateMasterClock' instead.")]
		[ObsoletedOSPlatform ("ios9.0", "Use 'CMTimebaseGetUltimateMasterClock' instead.")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[NoMacCatalyst]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'CMTimebaseGetUltimateMasterClock' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'CMTimebaseGetUltimateMasterClock' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'CMTimebaseGetUltimateMasterClock' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'CMTimebaseGetUltimateMasterClock' instead.")]
#endif
		[DllImport (Constants.CoreMediaLibrary)]
		static extern unsafe /* CMClockRef */ IntPtr CMTimebaseCopyUltimateMasterClock (/* CMTimebaseRef */ IntPtr timebase);
#endif
		//
		// Dispatch timers not supported
		//
		// CMTimebaseAddTimerDispatchSource
		// CMTimebaseRemoveTimerDispatchSource
		// CMTimebaseSetTimerDispatchSourceNextFireTime
		// CMTimebaseSetTimerDispatchSourceToFireImmediately
#endif // !COREBUILD
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#else
	[Watch (6, 0)]
#endif
	public class CMClockOrTimebase : NativeObject {
#if !NET
		public CMClockOrTimebase (NativeHandle handle)
			: base (handle, false)
		{
		}
#endif

		[Preserve (Conditional = true)]
		internal CMClockOrTimebase (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

#if !COREBUILD
		[DllImport (Constants.CoreMediaLibrary)]
		extern static CMTime CMSyncGetTime (/* CMClockOrTimebaseRef */ IntPtr clockOrTimebase);

		public CMTime Time {
			get {
				return CMSyncGetTime (Handle);
			}
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* Float64 */ double CMSyncGetRelativeRate (/* CMClockOrTimebaseRef */ IntPtr ofClockOrTimebase, /* CMClockOrTimebaseRef */ IntPtr relativeToClockOrTimebase);

		public static double GetRelativeRate (CMClockOrTimebase clockOrTimebaseA, CMClockOrTimebase clockOrTimebaseB)
		{
			if (clockOrTimebaseA is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (clockOrTimebaseA));

			if (clockOrTimebaseB is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (clockOrTimebaseB));

			return CMSyncGetRelativeRate (clockOrTimebaseA.Handle, clockOrTimebaseB.Handle);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMSyncError CMSyncGetRelativeRateAndAnchorTime (
			/* CMClockOrTimebaseRef */ IntPtr ofClockOrTimebase,
			/* CMClockOrTimebaseRef */ IntPtr relativeToClockOrTimebase,
			/* Float64* */ out double outRelativeRate,
			out CMTime outOfClockOrTimebaseAnchorTime,
			out CMTime outRelativeToClockOrTimebaseAnchorTime);

		public static CMSyncError GetRelativeRateAndAnchorTime (CMClockOrTimebase clockOrTimebaseA, CMClockOrTimebase clockOrTimebaseB, out double relativeRate, out CMTime timeA, out CMTime timeB)
		{
			if (clockOrTimebaseA is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (clockOrTimebaseA));

			if (clockOrTimebaseB is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (clockOrTimebaseB));

			return CMSyncGetRelativeRateAndAnchorTime (clockOrTimebaseA.Handle, clockOrTimebaseB.Handle, out relativeRate, out timeA, out timeB);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static CMTime CMSyncConvertTime (CMTime time, /* CMClockOrTimebaseRef */ IntPtr fromClockOrTimebase, /* CMClockOrTimebaseRef */ IntPtr toClockOrTimebase);

		public static CMTime ConvertTime (CMTime time, CMClockOrTimebase from, CMClockOrTimebase to)
		{
			if (from is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (from));
			if (to is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (to));

			return CMSyncConvertTime (time, from.Handle, to.Handle);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static /* Boolean */ bool CMSyncMightDrift (/* CMClockOrTimebaseRef */ IntPtr clockOrTimebase1, /* CMClockOrTimebaseRef */ IntPtr clockOrTimebase2);

		public static bool MightDrift (CMClockOrTimebase clockOrTimebaseA, CMClockOrTimebase clockOrTimebaseB)
		{
			if (clockOrTimebaseA is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (clockOrTimebaseA));

			if (clockOrTimebaseB is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (clockOrTimebaseB));

			return CMSyncMightDrift (clockOrTimebaseA.Handle, clockOrTimebaseB.Handle);
		}

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		[DllImport (Constants.CoreMediaLibrary)]
		static extern /* CMTimebase */ IntPtr CMTimebaseCopySourceTimebase (/* CMTimebase */ IntPtr timebase);

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		[DllImport (Constants.CoreMediaLibrary)]
		static extern int CMTimebaseSetSourceTimebase (/* CMTimebase */ IntPtr timebase, /* CMTimebase */ IntPtr newSourceTimebase);

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		public CMTimebase? SourceTimebase {
			get {
				var source = CMTimebaseCopySourceTimebase (Handle);
				return Runtime.GetINativeObject<CMTimebase> (source, true);
			}
			set {
				CMTimebaseSetSourceTimebase (Handle, value.GetHandle ());
			}
		}

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		[DllImport (Constants.CoreMediaLibrary)]
		static extern /* CMClock */ IntPtr CMTimebaseCopySourceClock (/* CMTimebase */ IntPtr timebase);

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		[DllImport (Constants.CoreMediaLibrary)]
		static extern int CMTimebaseSetSourceClock (/* CMTimebase */ IntPtr timebase, /* CMClock */ IntPtr newSourceClock);

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		public CMClock? SourceClock {
			get {
				var clock = CMTimebaseCopySourceClock (Handle);
				return Runtime.GetINativeObject<CMClock> (clock, true);
			}
			set {
				CMTimebaseSetSourceClock (Handle, value.GetHandle ());
			}
		}

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		[DllImport (Constants.CoreMediaLibrary)]
		static extern /* CMClock */ IntPtr CMTimebaseCopyUltimateSourceClock (/* CMTimebase */ IntPtr timebase);

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		public CMClock? UltimateSourceClock {
			get {
				var clock = CMTimebaseCopyUltimateSourceClock (Handle);
				return Runtime.GetINativeObject<CMClock> (clock, true);
			}
		}

#endif // !COREBUILD
	}
}
