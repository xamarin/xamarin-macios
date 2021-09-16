// 
// CMSync.cs: Implements the managed CMSync infrastructure
//
// Authors: Marek Safar (marek.safar@gmail.com)
//     
// Copyright 2012-2014 Xamarin Inc
//

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using Foundation;
using CoreFoundation;
using ObjCRuntime;

#nullable enable

namespace CoreMedia {

	// CMSync.h
#if !NET
	[Watch (6,0)]
#endif
	public class CMClock : CMClockOrTimebase
	{
		public CMClock (IntPtr handle) : base (handle)
		{
		}

		internal CMClock (IntPtr handle, bool owns) 
			: base (handle, owns)
		{
		}
#if !COREBUILD

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

#if !MONOMAC
		// CMAudioClock.h
#if !NET
		[Mac (10,10)]
#endif
		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMClockError CMAudioClockCreate (/* CFAllocatorRef */ IntPtr allocator, /* CMClockRef* */ out IntPtr clockOut);

		public static CMClock? CreateAudioClock (out CMClockError clockError)
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
		[return: MarshalAs (UnmanagedType.I1)]
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
#endif // !COREBUILD
	}

#if !NET
	[Watch (6,0)]
#endif
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
#if !COREBUILD

#if !NET
		[Deprecated (PlatformName.iOS, 8, 0)]
		[Deprecated (PlatformName.TvOS, 9, 0)] 
		[Deprecated (PlatformName.MacOSX, 10, 10)]
		[Deprecated (PlatformName.WatchOS, 6, 0)]
#else
		[UnsupportedOSPlatform ("ios8.0")]
		[UnsupportedOSPlatform ("tvos9.0")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("macos10.10")]
#endif
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

#if !NET
		[Deprecated (PlatformName.iOS, 8, 0)]
		[Deprecated (PlatformName.TvOS, 9, 0)] 
		[Deprecated (PlatformName.MacOSX, 10, 10)]
		[Deprecated (PlatformName.WatchOS, 6, 0)]
#else
		[UnsupportedOSPlatform ("ios8.0")]
		[UnsupportedOSPlatform ("tvos9.0")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("macos10.10")]
#endif
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
		
		[Watch (6,0), TV (9,0), Mac (10,8), iOS (6,0)]
		[DllImport(Constants.CoreMediaLibrary)]
		static extern CMTimebaseError CMTimebaseCreateWithSourceClock (/* [NullAllowed] CFAllocatorRef */ IntPtr allocator, /* CMClock */ IntPtr sourceClock, /* CMTimebase */ out IntPtr  timebaseOut);

		[Watch (6,0), TV (9,0), Mac (10,8), iOS (6,0)]
		public CMTimebase (CFAllocator? allocator, CMClock sourceClock)
		{
			if (sourceClock == null)
				throw new ArgumentNullException (nameof(sourceClock));

			var error = CMTimebaseCreateWithSourceClock (allocator.GetHandle (), sourceClock.Handle, out handle);
			if (error != CMTimebaseError.None)
				throw new ArgumentException (error.ToString ());

			CFObject.CFRetain (Handle);
		}

		[Watch (6,0), TV (9,0), Mac (10,8), iOS (6,0)]
		[DllImport(Constants.CoreMediaLibrary)]
		static extern CMTimebaseError CMTimebaseCreateWithSourceTimebase (/* [NullAllowed] CFAllocatorRef */ IntPtr allocator, /* CMTimebase */ IntPtr sourceTimebase, /* CMTimebase */ out IntPtr timebaseOut);

		[Watch (6,0), TV (9,0), Mac (10,8), iOS (6,0)]
		public CMTimebase (CFAllocator? allocator, CMTimebase sourceTimebase)
		{
			if (sourceTimebase == null)
				throw new ArgumentNullException (nameof(sourceTimebase));

			var error = CMTimebaseCreateWithSourceTimebase (allocator.GetHandle (), sourceTimebase.Handle, out handle);
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

#if !WATCH
#if !NET
		[Deprecated (PlatformName.iOS, 9,0)]
		[Deprecated (PlatformName.MacOSX, 10,11)]
#else
		[UnsupportedOSPlatform ("ios9.0")]
		[UnsupportedOSPlatform ("macos10.11")]
#endif
		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* CMTimebaseRef */ IntPtr CMTimebaseGetMasterTimebase (/* CMTimebaseRef */ IntPtr timebase);

#if !NET
		[Deprecated (PlatformName.iOS, 9, 0, message : "Use 'CopyMasterTimebase' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message : "Use 'CopyMasterTimebase' instead.")]
#else
		[UnsupportedOSPlatform ("ios9.0")]
		[UnsupportedOSPlatform ("macos10.11")]
#if IOS
		[Obsolete ("Starting with ios9.0 use 'CopyMasterTimebase' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif MONOMAC
		[Obsolete ("Starting with macos10.11 use 'CopyMasterTimebase' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
		public CMTimebase? GetMasterTimebase ()
		{
			var ptr = CMTimebaseGetMasterTimebase (Handle);
			if (ptr == IntPtr.Zero)
				return null;

			return new CMTimebase (ptr, false);			
		}

#if !NET
		[Deprecated (PlatformName.iOS, 9,0)]
		[Deprecated (PlatformName.MacOSX, 10,11)]
#else
		[UnsupportedOSPlatform ("ios9.0")]
		[UnsupportedOSPlatform ("macos10.11")]
#endif
		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* CMClockRef */ IntPtr CMTimebaseGetMasterClock (/* CMTimebaseRef */ IntPtr timebase);

#if !NET
		[Deprecated (PlatformName.iOS, 9, 0, message : "Use 'CopyMasterClock' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message : "Use 'CopyMasterClock' instead.")]
#else
		[UnsupportedOSPlatform ("ios9.0")]
		[UnsupportedOSPlatform ("macos10.11")]
#if IOS
		[Obsolete ("Starting with ios9.0 use 'CopyMasterClock' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif MONOMAC
		[Obsolete ("Starting with macos10.11 use 'CopyMasterClock' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
		public CMClock? GetMasterClock ()
		{
			var ptr = CMTimebaseGetMasterClock (Handle);
			if (ptr == IntPtr.Zero)
				return null;

			return new CMClock (ptr, false);
		}

#if !NET
		[Deprecated (PlatformName.iOS, 9,0)]
		[Deprecated (PlatformName.MacOSX, 10,11)]
#else
		[UnsupportedOSPlatform ("ios9.0")]
		[UnsupportedOSPlatform ("macos10.11")]
#endif
		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* CMClockOrTimebaseRef */ IntPtr CMTimebaseGetMaster (/* CMTimebaseRef */ IntPtr timebase);

#if !NET
		[Deprecated (PlatformName.iOS, 9, 0, message : "Use 'CopyMaster' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message : "Use 'CopyMaster' instead.")]
#else
		[UnsupportedOSPlatform ("ios9.0")]
		[UnsupportedOSPlatform ("macos10.11")]
#if IOS
		[Obsolete ("Starting with ios9.0 use 'CopyMaster' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif MONOMAC
		[Obsolete ("Starting with macos10.11 use 'CopyMaster' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
		public CMClockOrTimebase? GetMaster ()
		{
			var ptr = CMTimebaseGetMaster (Handle);
			if (ptr == IntPtr.Zero)
				return null;

			return new CMClockOrTimebase (ptr, false);
		}

#if !NET
		[Deprecated (PlatformName.iOS, 9,0)]
		[Deprecated (PlatformName.MacOSX, 10,11)]
#else
		[UnsupportedOSPlatform ("ios9.0")]
		[UnsupportedOSPlatform ("macos10.11")]
#endif
		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* CMClockRef */ IntPtr CMTimebaseGetUltimateMasterClock (/* CMTimebaseRef */ IntPtr timebase);

#if !NET
		[Deprecated (PlatformName.iOS, 9, 0, message : "Use 'CopyUltimateMasterClock' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message : "Use 'CopyUltimateMasterClock' instead.")]
#else
		[UnsupportedOSPlatform ("ios9.0")]
		[UnsupportedOSPlatform ("macos10.11")]
#if IOS
		[Obsolete ("Starting with ios9.0 use 'CopyUltimateMasterClock' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif MONOMAC
		[Obsolete ("Starting with macos10.11 use 'CopyUltimateMasterClock' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
		public CMClock? GetUltimateMasterClock ()
		{
			var ptr  = CMTimebaseGetUltimateMasterClock (Handle);
			if (ptr == IntPtr.Zero)
				return null;

			return new CMClock (ptr, false);
		}
#endif

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

#if !NET
		[TV (13,0), Mac (10,15), iOS (13,0)]
#else
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
#endif
		[DllImport(Constants.CoreMediaLibrary)]
		extern static CMTimebaseError CMTimebaseSetMasterTimebase (/* CMTimebaseRef* */ IntPtr timebase, /* CMTimebaseRef* */ IntPtr newMasterTimebase);

#if !NET
		[TV (13,0), Mac (10,15), iOS (13,0)]
#else
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
#endif
		public CMTimebaseError SetMasterTimebase (CMTimebase newMasterTimebase)
		{
			if (newMasterTimebase == null)
				throw new ArgumentNullException (nameof (newMasterTimebase));

			return CMTimebaseSetMasterTimebase (Handle, newMasterTimebase.Handle);
		}

#if !NET
		[TV (13,0), Mac (10,15), iOS (13,0)]
#else
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
#endif
		[DllImport(Constants.CoreMediaLibrary)]
		extern static CMTimebaseError CMTimebaseSetMasterClock (/* CMTimebaseRef* */ IntPtr timebase, /* CMClockRef* */ IntPtr newMasterClock);

#if !NET
		[TV (13,0), Mac (10,15), iOS (13,0)]
#else
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
#endif
		public CMTimebaseError SetMasterClock (CMClock newMasterClock)
		{
			if (newMasterClock == null)
				throw new ArgumentNullException (nameof (newMasterClock));

			return CMTimebaseSetMasterClock (Handle, newMasterClock.Handle);
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
#if __MACCATALYST__
			return true;
#elif IOS || MONOMAC
			return PlatformHelper.CheckSystemVersion (GetMethodDeprecatedMajor, GetMethodDeprecatedMinor);
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

#if !NET
		[iOS (9,0)][Mac (10,11), NoMacCatalyst]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'CMTimebaseGetMasterTimebase' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'CMTimebaseGetMasterTimebase' instead.")] 
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'CMTimebaseGetMasterTimebase' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message:  "Use 'CMTimebaseGetMasterTimebase' instead.")]
#else
		[UnsupportedOSPlatform ("ios15.0")]
		[UnsupportedOSPlatform ("tvos15.0")]
		[UnsupportedOSPlatform ("maccatalyst15.0")]
		[UnsupportedOSPlatform ("macos12.0")]
#endif
		[DllImport(Constants.CoreMediaLibrary)]
		static extern unsafe /* CMTimebaseRef */ IntPtr  CMTimebaseCopyMasterTimebase (/* CMTimebaseRef */ IntPtr timebase);

#if !NET
		[iOS (9,0)][Mac (10,11)]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'CMTimebaseGetMasterClock' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'CMTimebaseGetMasterClock' instead.")] 
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'CMTimebaseGetMasterClock' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message:  "Use 'CMTimebaseGetMasterClock' instead.")]
#else
		[UnsupportedOSPlatform ("ios9.0")]
		[UnsupportedOSPlatform ("tvos9.0")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("macos10.11")]
#endif
		[DllImport(Constants.CoreMediaLibrary)]
		static extern unsafe /* CMClockRef */ IntPtr  CMTimebaseCopyMasterClock (/* CMTimebaseRef */ IntPtr timebase);

#if !NET
		[iOS (9,0)][Mac (10,11), NoMacCatalyst]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'CMTimebaseGetMaster' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'CMTimebaseGetMaster' instead.")] 
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'CMTimebaseGetMaster' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message:  "Use 'CMTimebaseGetMaster' instead.")]
#else
		[UnsupportedOSPlatform ("ios9.0")]
		[UnsupportedOSPlatform ("tvos9.0")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("macos10.11")]
#endif
		[DllImport(Constants.CoreMediaLibrary)]
		static extern unsafe IntPtr /* void* */ CMTimebaseCopyMaster (/* CMTimebaseRef */ IntPtr timebase);

#if !NET
		[iOS (9,0)][Mac (10,11), NoMacCatalyst]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'CMTimebaseGetUltimateMasterClock' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'CMTimebaseGetUltimateMasterClock' instead.")] 
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'CMTimebaseGetUltimateMasterClock' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message:  "Use 'CMTimebaseGetUltimateMasterClock' instead.")]
#else
		[UnsupportedOSPlatform ("ios15.0")]
		[UnsupportedOSPlatform ("tvos15.0")]
		[UnsupportedOSPlatform ("maccatalyst15.0")]
		[UnsupportedOSPlatform ("macos12.0")]
#endif
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
#endif // !COREBUILD
	}

#if !NET
	[Watch (6,0)]
#endif
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

#if !COREBUILD
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
		[return: MarshalAs (UnmanagedType.I1)]
		extern static /* Boolean */ bool CMSyncMightDrift (/* CMClockOrTimebaseRef */ IntPtr clockOrTimebase1, /* CMClockOrTimebaseRef */ IntPtr clockOrTimebase2);

		public static bool MightDrift (CMClockOrTimebase clockOrTimebaseA, CMClockOrTimebase clockOrTimebaseB)
		{
			if (clockOrTimebaseA == null)
				throw new ArgumentNullException ("clockOrTimebaseA");

			if (clockOrTimebaseB == null)
				throw new ArgumentNullException ("clockOrTimebaseB");

			return CMSyncMightDrift (clockOrTimebaseA.Handle, clockOrTimebaseB.Handle);
		}

		[Watch (6,0), TV (9,0), Mac (10,11), iOS (9,0)]
		[DllImport(Constants.CoreMediaLibrary)]
		static extern /* CMTimebase */ IntPtr CMTimebaseCopySourceTimebase (/* CMTimebase */ IntPtr timebase);

		[Watch (6,0), TV (9,0), Mac (10,8), iOS (6,0)]
		[DllImport(Constants.CoreMediaLibrary)]
		static extern int CMTimebaseSetSourceTimebase (/* CMTimebase */ IntPtr timebase, /* CMTimebase */ IntPtr newSourceTimebase);

		[Watch (6,0), TV (9,0), Mac (10,11), iOS (9,0)]
		public CMTimebase? SourceTimebase  {
			get {
				var source = CMTimebaseCopySourceTimebase  (Handle);
				return source == IntPtr.Zero ? null : Runtime.GetINativeObject<CMTimebase> (source, true); 
			}
			set {
				CMTimebaseSetSourceTimebase (Handle, value?.GetHandle () ?? IntPtr.Zero);
			}
		}

		[Watch (6,0), TV (9,0), Mac (10,11), iOS (9,0)]
		[DllImport(Constants.CoreMediaLibrary)]
		static extern /* CMClock */ IntPtr CMTimebaseCopySourceClock (/* CMTimebase */ IntPtr timebase);

		[Watch (6,0), TV (9,0), Mac (10,8), iOS (6,0)]
		[DllImport(Constants.CoreMediaLibrary)]
		static extern int CMTimebaseSetSourceClock (/* CMTimebase */ IntPtr timebase, /* CMClock */ IntPtr newSourceClock);

		[Watch (6,0), TV (9,0), Mac (10,11), iOS (9,0)]
		public CMClock? SourceClock {
			get {
				var clock = CMTimebaseCopySourceClock (Handle);
				return clock == IntPtr.Zero ? null : Runtime.GetINativeObject<CMClock> (clock, true);
			}
			set {
				CMTimebaseSetSourceClock (Handle, value?.GetHandle() ?? IntPtr.Zero);
			}
		}

		[Watch (6,0), TV (9,0), Mac (10,11), iOS (9,0)]
		[DllImport(Constants.CoreMediaLibrary)]
		static extern /* CMClock */ IntPtr CMTimebaseCopyUltimateSourceClock (/* CMTimebase */ IntPtr timebase);

		[Watch (6,0), TV (9,0), Mac (10,11), iOS (9,0)]
		public CMClock? UltimateSourceClock  {
			get {
				var clock = CMTimebaseCopyUltimateSourceClock (Handle);
				return clock == IntPtr.Zero ? null : Runtime.GetINativeObject<CMClock> (clock, true);
			}
		}

#endif // !COREBUILD
	}
}
