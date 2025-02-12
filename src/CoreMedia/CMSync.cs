// 
// CMSync.cs: Implements the managed CMSync infrastructure
//
// Authors: Marek Safar (marek.safar@gmail.com)
//     
// Copyright 2012-2014 Xamarin Inc
//

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Foundation;
using CoreFoundation;
using ObjCRuntime;

#nullable enable

namespace CoreMedia {

	// CMSync.h
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CMClock : CMClockOrTimebase {
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
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static /* OSStatus */ CMClockError CMAudioClockCreate (/* CFAllocatorRef */ IntPtr allocator, /* CMClockRef* */ IntPtr* clockOut);

		public static CMClock? CreateAudioClock (out CMClockError clockError)
		{
			IntPtr ptr;
			unsafe {
				clockError = CMAudioClockCreate (IntPtr.Zero, &ptr);
			}
			return clockError == CMClockError.None ? new CMClock (ptr, true) : null;
		}
#endif

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static /* OSStatus */ CMClockError CMClockGetAnchorTime (/* CMClockRef */ IntPtr clock, CMTime* outClockTime, CMTime* outReferenceClockTime);

		public CMClockError GetAnchorTime (out CMTime clockTime, out CMTime referenceClockTime)
		{
			clockTime = default;
			referenceClockTime = default;
			unsafe {
				return CMClockGetAnchorTime (Handle, (CMTime*) Unsafe.AsPointer<CMTime> (ref clockTime), (CMTime*) Unsafe.AsPointer<CMTime> (ref referenceClockTime));
			}
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* Boolean */ byte CMClockMightDrift (/* CMClockRef */ IntPtr clock, /* CMClockRef */ IntPtr otherClock);

		public bool MightDrift (CMClock otherClock)
		{
			if (otherClock is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (otherClock));

			return CMClockMightDrift (Handle, otherClock.Handle) != 0;
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

	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CMTimebase : CMClockOrTimebase {
		[Preserve (Conditional = true)]
		private CMTimebase (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}
#if !COREBUILD

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("maccatalyst13.0")]
		[ObsoletedOSPlatform ("tvos9.0")]
		[ObsoletedOSPlatform ("macos10.10")]
		[ObsoletedOSPlatform ("ios9.0")]
		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static /* OSStatus */ CMTimebaseError CMTimebaseCreateWithMasterClock (/* CFAllocatorRef */ IntPtr allocator, /* CMClockRef */ IntPtr masterClock, /* CMTimebaseRef* */ IntPtr* timebaseOut);

		static IntPtr Create (CMClock masterClock)
		{
			if (masterClock is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (masterClock));

			CMTimebaseError error;
			IntPtr handle;
			unsafe {
				error = CMTimebaseCreateWithMasterClock (IntPtr.Zero, masterClock.Handle, &handle);
			}
			if (error != CMTimebaseError.None)
				ObjCRuntime.ThrowHelper.ThrowArgumentException (error.ToString ());
			return handle;
		}

		public CMTimebase (CMClock masterClock)
			: base (Create (masterClock), true)
		{
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("tvos9.0")]
		[ObsoletedOSPlatform ("macos10.10")]
		[ObsoletedOSPlatform ("ios8.0")]
		[ObsoletedOSPlatform ("maccatalyst13.0")]
		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static /* OSStatus */ CMTimebaseError CMTimebaseCreateWithMasterTimebase (/* CFAllocatorRef */ IntPtr allocator, /* CMTimebaseRef */ IntPtr masterTimebase, /* CMTimebaseRef* */ IntPtr* timebaseOut);

		static IntPtr Create (CMTimebase masterTimebase)
		{
			if (masterTimebase is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (masterTimebase));

			CMTimebaseError error;
			IntPtr handle;
			unsafe {
				error = CMTimebaseCreateWithMasterTimebase (IntPtr.Zero, masterTimebase.Handle, &handle);
			}
			if (error != CMTimebaseError.None)
				ObjCRuntime.ThrowHelper.ThrowArgumentException (error.ToString ());
			return handle;
		}

		public CMTimebase (CMTimebase masterTimebase)
			: base (Create (masterTimebase), true)
		{
		}

		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.CoreMediaLibrary)]
		unsafe static extern CMTimebaseError CMTimebaseCreateWithSourceClock (/* [NullAllowed] CFAllocatorRef */ IntPtr allocator, /* CMClock */ IntPtr sourceClock, /* CMTimebase */ IntPtr* timebaseOut);

		static IntPtr Create (CFAllocator? allocator, CMClock sourceClock)
		{
			if (sourceClock is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (sourceClock));

			CMTimebaseError error;
			IntPtr handle;
			unsafe {
				error = CMTimebaseCreateWithSourceClock (allocator.GetHandle (), sourceClock.Handle, &handle);
			}
			if (error != CMTimebaseError.None)
				ObjCRuntime.ThrowHelper.ThrowArgumentException (error.ToString ());
			return handle;
		}

		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
		public CMTimebase (CFAllocator? allocator, CMClock sourceClock)
			: base (Create (allocator, sourceClock), true)
		{
		}

		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.CoreMediaLibrary)]
		unsafe static extern CMTimebaseError CMTimebaseCreateWithSourceTimebase (/* [NullAllowed] CFAllocatorRef */ IntPtr allocator, /* CMTimebase */ IntPtr sourceTimebase, /* CMTimebase */ IntPtr* timebaseOut);

		static IntPtr Create (CFAllocator? allocator, CMTimebase sourceTimebase)
		{
			if (sourceTimebase is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (sourceTimebase));

			CMTimebaseError error;
			IntPtr handle;
			unsafe {
				error = CMTimebaseCreateWithSourceTimebase (allocator.GetHandle (), sourceTimebase.Handle, &handle);
			}
			if (error != CMTimebaseError.None)
				ObjCRuntime.ThrowHelper.ThrowArgumentException (error.ToString ());
			return handle;
		}

		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
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

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("maccatalyst13.0")]
		[ObsoletedOSPlatform ("tvos9.0")]
		[ObsoletedOSPlatform ("macos10.10")]
		[ObsoletedOSPlatform ("ios9.0")]
		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* CMTimebaseRef */ IntPtr CMTimebaseGetMasterTimebase (/* CMTimebaseRef */ IntPtr timebase);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.11", "Use 'CopyMasterTimebase' instead.")]
		[ObsoletedOSPlatform ("ios9.0", "Use 'CopyMasterTimebase' instead.")]
		[ObsoletedOSPlatform ("tvos9.0", "Use 'CopyMasterTimebase' instead.")]
		[ObsoletedOSPlatform ("maccatalyst13.0", "Use 'CopyMasterTimebase' instead.")]
		public CMTimebase? GetMasterTimebase ()
		{
			var ptr = CMTimebaseGetMasterTimebase (Handle);
			if (ptr == IntPtr.Zero)
				return null;

			return new CMTimebase (ptr, false);
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("maccatalyst13.0")]
		[ObsoletedOSPlatform ("tvos9.0")]
		[ObsoletedOSPlatform ("macos10.10")]
		[ObsoletedOSPlatform ("ios9.0")]
		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* CMClockRef */ IntPtr CMTimebaseGetMasterClock (/* CMTimebaseRef */ IntPtr timebase);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.11", "Use 'CopyMasterClock' instead.")]
		[ObsoletedOSPlatform ("ios9.0", "Use 'CopyMasterClock' instead.")]
		[ObsoletedOSPlatform ("tvos9.0", "Use 'CopyMasterClock' instead.")]
		[ObsoletedOSPlatform ("maccatalyst13.0", "Use 'CopyMasterClock' instead.")]
		public CMClock? GetMasterClock ()
		{
			var ptr = CMTimebaseGetMasterClock (Handle);
			if (ptr == IntPtr.Zero)
				return null;

			return new CMClock (ptr, false);
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("maccatalyst13.0")]
		[ObsoletedOSPlatform ("tvos9.0")]
		[ObsoletedOSPlatform ("macos10.11")]
		[ObsoletedOSPlatform ("ios9.0")]
		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* CMClockOrTimebaseRef */ IntPtr CMTimebaseGetMaster (/* CMTimebaseRef */ IntPtr timebase);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.11", "Use 'CopyMaster' instead.")]
		[ObsoletedOSPlatform ("ios9.0", "Use 'CopyMaster' instead.")]
		[ObsoletedOSPlatform ("tvos9.0", "Use 'CopyMaster' instead.")]
		[ObsoletedOSPlatform ("maccatalyst13.0", "Use 'CopyMaster' instead.")]
		public CMClockOrTimebase? GetMaster ()
		{
			var ptr = CMTimebaseGetMaster (Handle);
			if (ptr == IntPtr.Zero)
				return null;

			return new CMClockOrTimebase (ptr, false);
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("maccatalyst13.0")]
		[ObsoletedOSPlatform ("tvos9.0")]
		[ObsoletedOSPlatform ("macos10.11")]
		[ObsoletedOSPlatform ("ios9.0")]
		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* CMClockRef */ IntPtr CMTimebaseGetUltimateMasterClock (/* CMTimebaseRef */ IntPtr timebase);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.11", "Use 'CopyUltimateMasterClock' instead.")]
		[ObsoletedOSPlatform ("ios9.0", "Use 'CopyUltimateMasterClock' instead.")]
		[ObsoletedOSPlatform ("tvos9.0", "Use 'CopyUltimateMasterClock' instead.")]
		[ObsoletedOSPlatform ("maccatalyst13.0", "Use 'CopyUltimateMasterClock' instead.")]
		public CMClock? GetUltimateMasterClock ()
		{
			var ptr = CMTimebaseGetUltimateMasterClock (Handle);
			if (ptr == IntPtr.Zero)
				return null;

			return new CMClock (ptr, false);
		}

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
		unsafe extern static /* OSStatus */ CMTimebaseError CMTimebaseGetTimeAndRate (/* CMTimebaseRef */ IntPtr timebase, CMTime* time, /* Float64* */ double* rate);

		public CMTimebaseError GetTimeAndRate (out CMTime time, out double rate)
		{
			time = default;
			rate = default;
			unsafe {
				return CMTimebaseGetTimeAndRate (Handle, (CMTime*) Unsafe.AsPointer<CMTime> (ref time), (double*) Unsafe.AsPointer<double> (ref rate));
			}
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

		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("tvos9.0")]
		[ObsoletedOSPlatform ("macos10.10")]
		[ObsoletedOSPlatform ("ios8.0")]
		[DllImport (Constants.CoreMediaLibrary)]
		extern static CMTimebaseError CMTimebaseSetMasterTimebase (/* CMTimebaseRef* */ IntPtr timebase, /* CMTimebaseRef* */ IntPtr newMasterTimebase);

		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("maccatalyst13.0")]
		[ObsoletedOSPlatform ("tvos9.0")]
		[ObsoletedOSPlatform ("macos10.10")]
		[ObsoletedOSPlatform ("ios8.0")]
		public CMTimebaseError SetMasterTimebase (CMTimebase newMasterTimebase)
		{
			if (newMasterTimebase is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (newMasterTimebase));

			return CMTimebaseSetMasterTimebase (Handle, newMasterTimebase.Handle);
		}

		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("maccatalyst13.0")]
		[ObsoletedOSPlatform ("tvos9.0")]
		[ObsoletedOSPlatform ("macos10.10")]
		[ObsoletedOSPlatform ("ios8.0")]
		[DllImport (Constants.CoreMediaLibrary)]
		extern static CMTimebaseError CMTimebaseSetMasterClock (/* CMTimebaseRef* */ IntPtr timebase, /* CMClockRef* */ IntPtr newMasterClock);

		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("maccatalyst13.0")]
		[ObsoletedOSPlatform ("tvos9.0")]
		[ObsoletedOSPlatform ("macos10.10")]
		[ObsoletedOSPlatform ("ios8.0")]
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
#elif TVOS
			return true;
#endif
		}

		public CMTimebase? CopyMasterTimebase ()
		{
			IntPtr ptr = IntPtr.Zero;
			bool deprecated = IsDeprecated ();
			if (deprecated)
				ptr = CMTimebaseCopyMasterTimebase (Handle);
			else
				ptr = CMTimebaseGetMasterTimebase (Handle);

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
			else
				ptr = CMTimebaseGetMasterClock (Handle);

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
			else
				ptr = CMTimebaseGetMaster (Handle);

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
			else
				ptr = CMTimebaseGetUltimateMasterClock (Handle);

			if (ptr == IntPtr.Zero)
				return null;

			return new CMClock (ptr, deprecated);
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("tvos9.0", "Use 'CMTimebaseGetMasterTimebase' instead.")]
		[ObsoletedOSPlatform ("macos10.11", "Use 'CMTimebaseGetMasterTimebase' instead.")]
		[ObsoletedOSPlatform ("ios9.0", "Use 'CMTimebaseGetMasterTimebase' instead.")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern unsafe /* CMTimebaseRef */ IntPtr CMTimebaseCopyMasterTimebase (/* CMTimebaseRef */ IntPtr timebase);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("maccatalyst13.0", "Use 'CMTimebaseGetMasterClock' instead.")]
		[ObsoletedOSPlatform ("tvos9.0", "Use 'CMTimebaseGetMasterClock' instead.")]
		[ObsoletedOSPlatform ("macos10.11", "Use 'CMTimebaseGetMasterClock' instead.")]
		[ObsoletedOSPlatform ("ios9.0", "Use 'CMTimebaseGetMasterClock' instead.")]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern unsafe /* CMClockRef */ IntPtr CMTimebaseCopyMasterClock (/* CMTimebaseRef */ IntPtr timebase);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("tvos9.0", "Use 'CMTimebaseGetMaster' instead.")]
		[ObsoletedOSPlatform ("macos10.11", "Use 'CMTimebaseGetMaster' instead.")]
		[ObsoletedOSPlatform ("ios9.0", "Use 'CMTimebaseGetMaster' instead.")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern unsafe IntPtr /* void* */ CMTimebaseCopyMaster (/* CMTimebaseRef */ IntPtr timebase);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("tvos9.0", "Use 'CMTimebaseGetUltimateMasterClock' instead.")]
		[ObsoletedOSPlatform ("macos10.11", "Use 'CMTimebaseGetUltimateMasterClock' instead.")]
		[ObsoletedOSPlatform ("ios9.0", "Use 'CMTimebaseGetUltimateMasterClock' instead.")]
		[UnsupportedOSPlatform ("maccatalyst")]
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

	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CMClockOrTimebase : NativeObject {
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
		unsafe extern static /* OSStatus */ CMSyncError CMSyncGetRelativeRateAndAnchorTime (
			/* CMClockOrTimebaseRef */ IntPtr ofClockOrTimebase,
			/* CMClockOrTimebaseRef */ IntPtr relativeToClockOrTimebase,
			/* Float64* */ double* outRelativeRate,
			CMTime* outOfClockOrTimebaseAnchorTime,
			CMTime* outRelativeToClockOrTimebaseAnchorTime);

		public static CMSyncError GetRelativeRateAndAnchorTime (CMClockOrTimebase clockOrTimebaseA, CMClockOrTimebase clockOrTimebaseB, out double relativeRate, out CMTime timeA, out CMTime timeB)
		{
			if (clockOrTimebaseA is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (clockOrTimebaseA));

			if (clockOrTimebaseB is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (clockOrTimebaseB));

			relativeRate = default;
			timeA = default;
			timeB = default;
			unsafe {
				return CMSyncGetRelativeRateAndAnchorTime (
					clockOrTimebaseA.Handle,
					clockOrTimebaseB.Handle,
					(double*) Unsafe.AsPointer<double> (ref relativeRate),
					(CMTime*) Unsafe.AsPointer<CMTime> (ref timeA),
					(CMTime*) Unsafe.AsPointer<CMTime> (ref timeB));
			}
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
		extern static /* Boolean */ byte CMSyncMightDrift (/* CMClockOrTimebaseRef */ IntPtr clockOrTimebase1, /* CMClockOrTimebaseRef */ IntPtr clockOrTimebase2);

		public static bool MightDrift (CMClockOrTimebase clockOrTimebaseA, CMClockOrTimebase clockOrTimebaseB)
		{
			if (clockOrTimebaseA is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (clockOrTimebaseA));

			if (clockOrTimebaseB is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (clockOrTimebaseB));

			return CMSyncMightDrift (clockOrTimebaseA.Handle, clockOrTimebaseB.Handle) != 0;
		}

		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern /* CMTimebase */ IntPtr CMTimebaseCopySourceTimebase (/* CMTimebase */ IntPtr timebase);

		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern int CMTimebaseSetSourceTimebase (/* CMTimebase */ IntPtr timebase, /* CMTimebase */ IntPtr newSourceTimebase);

		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
		public CMTimebase? SourceTimebase {
			get {
				var source = CMTimebaseCopySourceTimebase (Handle);
				return Runtime.GetINativeObject<CMTimebase> (source, true);
			}
			set {
				CMTimebaseSetSourceTimebase (Handle, value.GetHandle ());
			}
		}

		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern /* CMClock */ IntPtr CMTimebaseCopySourceClock (/* CMTimebase */ IntPtr timebase);

		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern int CMTimebaseSetSourceClock (/* CMTimebase */ IntPtr timebase, /* CMClock */ IntPtr newSourceClock);

		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
		public CMClock? SourceClock {
			get {
				var clock = CMTimebaseCopySourceClock (Handle);
				return Runtime.GetINativeObject<CMClock> (clock, true);
			}
			set {
				CMTimebaseSetSourceClock (Handle, value.GetHandle ());
			}
		}

		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern /* CMClock */ IntPtr CMTimebaseCopyUltimateSourceClock (/* CMTimebase */ IntPtr timebase);

		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
		public CMClock? UltimateSourceClock {
			get {
				var clock = CMTimebaseCopyUltimateSourceClock (Handle);
				return Runtime.GetINativeObject<CMClock> (clock, true);
			}
		}

#endif // !COREBUILD
	}
}
