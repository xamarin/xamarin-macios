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

namespace CoreMedia {

	// untyped enum (used as OSStatus) -> CMSync.h
	public enum CMClockError : int
	{
		None = 0,
		MissingRequiredParameter	= -12745,
		InvalidParameter			= -12746,
		AllocationFailed			= -12747,
		UnsupportedOperation		= -12756,
	}

	// CMSync.h
	[iOS (6,0)][Mac (10,8)]
	public class CMClock : CMClockOrTimebase
	{
		public CMClock (IntPtr handle) : base (handle)
		{
		}

		internal CMClock (IntPtr handle, bool owns) 
			: base (handle, owns)
		{
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* CMClockRef */ IntPtr CMClockGetHostTimeClock ();

		public static CMClock HostTimeClock {
			get {
				return new CMClock (CMClockGetHostTimeClock (), false);
			}
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static CMTime CMClockGetTime (/* CMClockRef */ IntPtr clock);

		public CMTime CurrentTime {
			get {
				return CMClockGetTime (Handle);
			}
		}

#if MONOMAC && !XAMCORE_2_0
		// avoid breaking change on MonoMac.dll
		[Obsolete ("The 'CMAudioClockCreate' API is only available on iOS.")]
		public static CMClock CreateAudioClock (out CMClockError clockError)
		{
			throw new NotSupportedException ();
		}
#elif !MONOMAC
		// CMAudioClock.h
		[Mac (10,10)]
		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMClockError CMAudioClockCreate (/* CFAllocatorRef */ IntPtr allocator, /* CMClockRef* */ out IntPtr clockOut);

		public static CMClock CreateAudioClock (out CMClockError clockError)
		{
			IntPtr ptr;
			clockError = CMAudioClockCreate (IntPtr.Zero, out ptr);
			return clockError == CMClockError.None ? new CMClock (ptr) : null;
		}
#endif

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMClockError CMClockGetAnchorTime (/* CMClockRef */ IntPtr clock, out CMTime outClockTime, out CMTime outReferenceClockTime);

		public CMClockError GetAnchorTime (out CMTime clockTime, out CMTime referenceClockTime)
		{
			return CMClockGetAnchorTime (Handle, out clockTime, out referenceClockTime);
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* Boolean */ bool CMClockMightDrift (/* CMClockRef */ IntPtr clock, /* CMClockRef */ IntPtr otherClock);

		public bool MightDrift (CMClock otherClock)
		{
			if (otherClock == null)
				throw new ArgumentNullException ("otherClock");

			return CMClockMightDrift (Handle, otherClock.Handle);
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static void CMClockInvalidate (/* CMClockRef */ IntPtr clock);

		public void Invalidate ()
		{
			CMClockInvalidate (Handle);
		}

		[DllImport(Constants.CoreMediaLibrary, EntryPoint="CMClockConvertHostTimeToSystemUnits")]
		public extern static /* uint64_t */ ulong ConvertHostTimeToSystemUnits (CMTime hostTime);

		[DllImport(Constants.CoreMediaLibrary, EntryPoint="CMClockMakeHostTimeFromSystemUnits")]
		public extern static CMTime CreateHostTimeFromSystemUnits (/* uint64_t */ ulong hostTime);
	}

	// untyped enum (used as OSStatus) -> CMSync.h
	public enum CMTimebaseError : int
	{
		None = 0,
		MissingRequiredParameter	= -12748,
		InvalidParameter			= -12749,
		AllocationFailed			= -12750,
		TimerIntervalTooShort		= -12751,
		ReadOnly					= -12757,
	}

	[iOS (6,0)][Mac (10,8)]
	public class CMTimebase : CMClockOrTimebase
	{
		public CMTimebase (IntPtr handle)
			: base (handle)
		{
		}

		private CMTimebase (IntPtr handle, bool owns) 
			: base (handle, owns)
		{
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMTimebaseError CMTimebaseCreateWithMasterClock (/* CFAllocatorRef */ IntPtr allocator, /* CMClockRef */ IntPtr masterClock, /* CMTimebaseRef* */ out IntPtr timebaseOut);

		public CMTimebase (CMClock masterClock)
		{
			if (masterClock == null)
				throw new ArgumentNullException ("masterClock");

			var error = CMTimebaseCreateWithMasterClock (IntPtr.Zero, masterClock.Handle, out handle);
			if (error != CMTimebaseError.None)
				throw new ArgumentException (error.ToString ());

			CFObject.CFRetain (Handle);
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMTimebaseError CMTimebaseCreateWithMasterTimebase (/* CFAllocatorRef */ IntPtr allocator, /* CMTimebaseRef */ IntPtr masterTimebase, /* CMTimebaseRef* */ out IntPtr timebaseOut);

		public CMTimebase (CMTimebase masterTimebase)
		{
			if (masterTimebase == null)
				throw new ArgumentNullException ("masterTimebase");

			var error = CMTimebaseCreateWithMasterTimebase (IntPtr.Zero, masterTimebase.Handle, out handle);
			if (error != CMTimebaseError.None)
				throw new ArgumentException (error.ToString ());

			CFObject.CFRetain (Handle);
		}


		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* Float64 */ double CMTimebaseGetEffectiveRate (/* CMTimebaseRef */ IntPtr timebase);

		public double EffectiveRate {
			get {
				return CMTimebaseGetEffectiveRate (Handle);
			}
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* Float64 */ double CMTimebaseGetRate (/* CMTimebaseRef */ IntPtr timebase);

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMTimebaseError CMTimebaseSetRate (/* CMTimebaseRef */ IntPtr timebase, /* Float64 */ double rate);

		public double Rate {
			get {
				return CMTimebaseGetRate (Handle);
			}
			set {
				var error = CMTimebaseSetRate (Handle, value);
				if (error != CMTimebaseError.None)
					throw new ArgumentException (error.ToString ());				
			}
		} 

		[DllImport(Constants.CoreMediaLibrary)]
		extern static CMTime CMTimebaseGetTime (/* CMTimebaseRef */ IntPtr timebase);

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMTimebaseError CMTimebaseSetTime (/* CMTimebaseRef */ IntPtr timebase, CMTime time);

		public new CMTime Time {
			get {
				return CMTimebaseGetTime (Handle);
			}
			set {
				var error = CMTimebaseSetTime (Handle, value);
				if (error != CMTimebaseError.None)
					throw new ArgumentException (error.ToString ());
			}
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* CMTimebaseRef */ IntPtr CMTimebaseGetMasterTimebase (/* CMTimebaseRef */ IntPtr timebase);

		[iOS (6, 0)]
		[Deprecated (PlatformName.iOS, 9, 0, message : "Use 'CopyMasterTimebase' instead.")]
		[Mac (10, 8)]
		[Deprecated (PlatformName.MacOSX, 10, 11, message : "Use 'CopyMasterTimebase' instead.")]
		public CMTimebase GetMasterTimebase ()
		{
			var ptr = CMTimebaseGetMasterTimebase (Handle);
			if (ptr == IntPtr.Zero)
				return null;

			return new CMTimebase (ptr, false);			
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* CMClockRef */ IntPtr CMTimebaseGetMasterClock (/* CMTimebaseRef */ IntPtr timebase);

		[iOS (6, 0)]
		[Deprecated (PlatformName.iOS, 9, 0, message : "Use 'CopyMasterClock' instead.")]
		[Mac (10, 8)]
		[Deprecated (PlatformName.MacOSX, 10, 11, message : "Use 'CopyMasterClock' instead.")]
		public CMClock GetMasterClock ()
		{
			var ptr = CMTimebaseGetMasterClock (Handle);
			if (ptr == IntPtr.Zero)
				return null;

			return new CMClock (ptr, false);
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* CMClockOrTimebaseRef */ IntPtr CMTimebaseGetMaster (/* CMTimebaseRef */ IntPtr timebase);

		[iOS (6, 0)]
		[Deprecated (PlatformName.iOS, 9, 0, message : "Use 'CopyMaster' instead.")]
		[Mac (10, 8)]
		[Deprecated (PlatformName.MacOSX, 10, 11, message : "Use 'CopyMaster' instead.")]
		public CMClockOrTimebase GetMaster ()
		{
			var ptr = CMTimebaseGetMaster (Handle);
			if (ptr == IntPtr.Zero)
				return null;

			return new CMClockOrTimebase (ptr, false);
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* CMClockRef */ IntPtr CMTimebaseGetUltimateMasterClock (/* CMTimebaseRef */ IntPtr timebase);

		[iOS (6, 0)]
		[Deprecated (PlatformName.iOS, 9, 0, message : "Use 'CopyUltimateMasterClock' instead.")]
		[Mac (10, 8)]
		[Deprecated (PlatformName.MacOSX, 10, 11, message : "Use 'CopyUltimateMasterClock' instead.")]
		public CMClock GetUltimateMasterClock ()
		{
			var ptr  = CMTimebaseGetUltimateMasterClock (Handle);
			if (ptr == IntPtr.Zero)
				return null;

			return new CMClock (ptr, false);
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static CMTime CMTimebaseGetTimeWithTimeScale (/* CMTimebaseRef */ IntPtr timebase, CMTimeScale timescale, CMTimeRoundingMethod method);

		public CMTime GetTime (CMTimeScale timeScale, CMTimeRoundingMethod roundingMethod)
		{
			return CMTimebaseGetTimeWithTimeScale (Handle, timeScale, roundingMethod);
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMTimebaseError CMTimebaseSetAnchorTime (/* CMTimebaseRef */ IntPtr timebase, CMTime timebaseTime, CMTime immediateMasterTime);

		public CMTimebaseError SetAnchorTime (CMTime timebaseTime, CMTime immediateMasterTime)
		{
			return CMTimebaseSetAnchorTime (Handle, timebaseTime, immediateMasterTime);
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMTimebaseError CMTimebaseGetTimeAndRate (/* CMTimebaseRef */ IntPtr timebase, out CMTime time, /* Float64* */ out double rate);

		public CMTimebaseError GetTimeAndRate (out CMTime time, out double rate)
		{
			return CMTimebaseGetTimeAndRate (Handle, out time, out rate);
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* OSStatus */  CMTimebaseError CMTimebaseSetRateAndAnchorTime (/* CMTimebaseRef */ IntPtr timebase, /* Float64 */ double rate, CMTime timebaseTime, CMTime immediateMasterTime);

		public CMTimebaseError SetRateAndAnchorTime (double rate, CMTime timebaseTime, CMTime immediateMasterTime)
		{
			return CMTimebaseSetRateAndAnchorTime (Handle, rate, timebaseTime, immediateMasterTime);
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMTimebaseError CMTimebaseNotificationBarrier (/* CMTimebaseRef */ IntPtr timebase);

		public CMTimebaseError NotificationBarrier ()
		{
			return CMTimebaseNotificationBarrier (handle);
		}

		public const double VeryLongTimeInterval = 256.0 * 365.0 * 24.0 * 60.0 * 60.0;

 #if !COREBUILD
		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMTimebaseError CMTimebaseAddTimer (/* CMTimebaseRef */ IntPtr timebase, /* CFRunLoopTimerRef */ IntPtr timer, /* CFRunLoopRef */ IntPtr runloop);

		public CMTimebaseError AddTimer (NSTimer timer, NSRunLoop runloop)
		{
			if (timer == null)
				throw new ArgumentNullException ("timer");
			if (runloop == null)
				throw new ArgumentNullException ("runloop");

			// NSRunloop and CFRunloop[Ref] are NOT toll free bridged types
			using (var cf = runloop.GetCFRunLoop ())
				return CMTimebaseAddTimer (Handle, timer.Handle, cf.Handle);
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMTimebaseError CMTimebaseRemoveTimer (/* CMTimebaseRef */ IntPtr timebase, /* CFRunLoopTimerRef */ IntPtr timer);

		public CMTimebaseError RemoveTimer (NSTimer timer)
		{
			if (timer == null)
				throw new ArgumentNullException ("timer");

			return CMTimebaseRemoveTimer (Handle, timer.Handle);
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMTimebaseError CMTimebaseSetTimerNextFireTime (/* CMTimebaseRef */ IntPtr timebase, /* CFRunLoopTimerRef */ IntPtr timer, CMTime fireTime, /* uint32_t */ uint flags);

		public CMTimebaseError SetTimerNextFireTime (NSTimer timer, CMTime fireTime)
		{
			if (timer == null)
				throw new ArgumentNullException ("timer");

			return CMTimebaseSetTimerNextFireTime (Handle, timer.Handle, fireTime, 0);
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMTimebaseError CMTimebaseSetTimerToFireImmediately (/* CMTimebaseRef */ IntPtr timebase, /* CFRunLoopTimerRef */ IntPtr timer);

		public CMTimebaseError SetTimerToFireImmediately (NSTimer timer)
		{
			if (timer == null)
				throw new ArgumentNullException ("timer");

			return CMTimebaseSetTimerToFireImmediately (Handle, timer.Handle);
		}
#endif

#if !COREBUILD
#if MONOMAC // CheckVersion
		const int GetMethodDeprecatedMajor = 10;
		const int GetMethodDeprecatedMinor = 11;
#elif IOS
		const int GetMethodDeprecatedMajor = 9;
		const int GetMethodDeprecatedMinor = 0;
#endif

		bool IsDeprecated ()
		{
#if IOS || MONOMAC
			return PlatformHelper.CheckSystemVersion (GetMethodDeprecatedMajor, GetMethodDeprecatedMinor);
#elif TVOS
			return true;
#endif
		}

		public CMTimebase CopyMasterTimebase ()
		{
			IntPtr ptr;
			bool deprecated = IsDeprecated ();
			if (deprecated)
				ptr = CMTimebaseCopyMasterTimebase (Handle);
			else
				ptr = CMTimebaseGetMasterTimebase (Handle);

			if (ptr == IntPtr.Zero)
				return null;

			return new CMTimebase (ptr, deprecated);
		}

		public CMClock CopyMasterClock ()
		{
			IntPtr ptr;
			bool deprecated = IsDeprecated ();
			if (deprecated)
				ptr = CMTimebaseCopyMasterClock (Handle);
			else
				ptr = CMTimebaseGetMasterClock (Handle);

			if (ptr == IntPtr.Zero)
				return null;

			return new CMClock (ptr, deprecated);
		}

		public CMClockOrTimebase CopyMaster ()
		{
			IntPtr ptr;
			bool deprecated = IsDeprecated ();
			if (deprecated)
				ptr = CMTimebaseCopyMaster (Handle);
			else
				ptr = CMTimebaseGetMaster (Handle);

			if (ptr == IntPtr.Zero)
				return null;

			return new CMClockOrTimebase (ptr, deprecated);
		}

		public CMClock CopyUltimateMasterClock ()
		{
			IntPtr ptr;
			bool deprecated = IsDeprecated ();
			if (deprecated)
				ptr = CMTimebaseCopyUltimateMasterClock (Handle);
			else
				ptr = CMTimebaseGetUltimateMasterClock (Handle);

			if (ptr == IntPtr.Zero)
				return null;

			return new CMClock (ptr, deprecated);
		}

		[iOS (9,0)][Mac (10,11)]
		[DllImport(Constants.CoreMediaLibrary)]
		static extern unsafe /* CMTimebaseRef */ IntPtr  CMTimebaseCopyMasterTimebase (/* CMTimebaseRef */ IntPtr timebase);

		[iOS (9,0)][Mac (10,11)]
		[DllImport(Constants.CoreMediaLibrary)]
		static extern unsafe /* CMClockRef */ IntPtr  CMTimebaseCopyMasterClock (/* CMTimebaseRef */ IntPtr timebase);

		[iOS (9,0)][Mac (10,11)]
		[DllImport(Constants.CoreMediaLibrary)]
		static extern unsafe IntPtr /* void* */ CMTimebaseCopyMaster (/* CMTimebaseRef */ IntPtr timebase);

		[iOS (9,0)][Mac (10,11)]
		[DllImport(Constants.CoreMediaLibrary)]
		static extern unsafe /* CMClockRef */ IntPtr CMTimebaseCopyUltimateMasterClock (/* CMTimebaseRef */ IntPtr timebase);
#endif
		//
		// Dispatch timers not supported
		//
		// CMTimebaseAddTimerDispatchSource
		// CMTimebaseRemoveTimerDispatchSource
		// CMTimebaseSetTimerDispatchSourceNextFireTime
		// CMTimebaseSetTimerDispatchSourceToFireImmediately
	}

	// untyped enum (used as OSStatus) -> CMSync.h
	public enum CMSyncError : int {
		None = 0,
		MissingRequiredParameter	= -12752,
		InvalidParameter			= -12753,
		AllocationFailed			= -12754,
		RateMustBeNonZero			= -12755,
	}

	[iOS (6,0)][Mac (10,8)]
	public class CMClockOrTimebase : IDisposable, INativeObject
	{
		internal IntPtr handle;

		internal CMClockOrTimebase ()
		{
		}

		public CMClockOrTimebase (IntPtr handle) : this (handle, false)
		{
		}

		internal CMClockOrTimebase (IntPtr handle, bool owns)
		{
			this.handle = handle;
			if (!owns)
				CFObject.CFRetain (handle);
		}

		~CMClockOrTimebase ()
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
			if (Handle != IntPtr.Zero){
				CFObject.CFRelease (Handle);
				handle = IntPtr.Zero;
			}
		}

		public IntPtr Handle { 
			get {
				return handle;
			}
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static CMTime CMSyncGetTime (/* CMClockOrTimebaseRef */ IntPtr clockOrTimebase);

		public CMTime Time { 
			get {
				return CMSyncGetTime (handle);
			}
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* Float64 */ double CMSyncGetRelativeRate (/* CMClockOrTimebaseRef */ IntPtr ofClockOrTimebase, /* CMClockOrTimebaseRef */ IntPtr relativeToClockOrTimebase);

		public static double GetRelativeRate (CMClockOrTimebase clockOrTimebaseA, CMClockOrTimebase clockOrTimebaseB)
		{
			if (clockOrTimebaseA == null)
				throw new ArgumentNullException ("clockOrTimebaseA");

			if (clockOrTimebaseB == null)
				throw new ArgumentNullException ("clockOrTimebaseB");

			return CMSyncGetRelativeRate (clockOrTimebaseA.Handle, clockOrTimebaseB.Handle);
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMSyncError CMSyncGetRelativeRateAndAnchorTime (
			/* CMClockOrTimebaseRef */ IntPtr ofClockOrTimebase, 
			/* CMClockOrTimebaseRef */ IntPtr relativeToClockOrTimebase,
			/* Float64* */ out double outRelativeRate, 
			out CMTime outOfClockOrTimebaseAnchorTime, 
			out CMTime outRelativeToClockOrTimebaseAnchorTime);

		public static CMSyncError GetRelativeRateAndAnchorTime (CMClockOrTimebase clockOrTimebaseA, CMClockOrTimebase clockOrTimebaseB,  out double relativeRate, out CMTime timeA, out CMTime timeB)
		{
			if (clockOrTimebaseA == null)
				throw new ArgumentNullException ("clockOrTimebaseA");

			if (clockOrTimebaseB == null)
				throw new ArgumentNullException ("clockOrTimebaseB");

			return CMSyncGetRelativeRateAndAnchorTime (clockOrTimebaseA.Handle, clockOrTimebaseB.handle, out relativeRate, out timeA, out timeB);
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static CMTime CMSyncConvertTime (CMTime time, /* CMClockOrTimebaseRef */ IntPtr fromClockOrTimebase, /* CMClockOrTimebaseRef */ IntPtr toClockOrTimebase);

		public static CMTime ConvertTime (CMTime time, CMClockOrTimebase from, CMClockOrTimebase to)
		{
			if (from == null)
				throw new ArgumentNullException ("from");
			if (to == null)
				throw new ArgumentNullException ("to");

			return CMSyncConvertTime (time, from.Handle, to.Handle);
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* Boolean */ bool CMSyncMightDrift (/* CMClockOrTimebaseRef */ IntPtr clockOrTimebase1, /* CMClockOrTimebaseRef */ IntPtr clockOrTimebase2);

		public static bool MightDrift (CMClockOrTimebase clockOrTimebaseA, CMClockOrTimebase clockOrTimebaseB)
		{
			if (clockOrTimebaseA == null)
				throw new ArgumentNullException ("clockOrTimebaseA");

			if (clockOrTimebaseB == null)
				throw new ArgumentNullException ("clockOrTimebaseB");

			return CMSyncMightDrift (clockOrTimebaseA.Handle, clockOrTimebaseB.Handle);
		}
	}
}
