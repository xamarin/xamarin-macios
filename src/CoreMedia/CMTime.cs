// 
// CMTime.cs: API for creating and manipulating CMTime structs
//
// Authors: Mono Team
//
// Copyright 2010-2011 Novell Inc
// Copyright 2012-2014 Xamarin Inc. All rights reserved.
//

#nullable enable

using System;
using System.Runtime.InteropServices;
using CoreFoundation;
using Foundation;
using ObjCRuntime;

namespace CoreMedia {

	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	[StructLayout (LayoutKind.Sequential)]
	public partial struct CMTime {
		// CMTimeFlags -> uint32_t -> CMTime.h
		[Flags]
		public enum Flags : uint {
			Valid = 1,
			HasBeenRounded = 2,
			PositiveInfinity = 4,
			NegativeInfinity = 8,
			Indefinite = 16,
			ImpliedValueFlagsMask = PositiveInfinity | NegativeInfinity | Indefinite
		}
#if !COREBUILD

		public readonly static CMTime Invalid = new CMTime (0);

		const Flags kIndefinite = Flags.Valid | Flags.Indefinite;
		public readonly static CMTime Indefinite = new CMTime (kIndefinite);

		const Flags kPositive = Flags.Valid | Flags.PositiveInfinity;
		public readonly static CMTime PositiveInfinity = new CMTime (kPositive);

		const Flags kNegative = Flags.Valid | Flags.NegativeInfinity;
		public readonly static CMTime NegativeInfinity = new CMTime (kNegative);

		public readonly static CMTime Zero = new CMTime (Flags.Valid, 1);

		public const int MaxTimeScale = 0x7fffffff;

#endif // !COREBUILD

		// CMTimeValue -> int64_t -> CMTime.h
		public long Value;

		// CMTimeScale -> int32_t -> CMTime.h
		public int TimeScale;

		// CMTimeFlags -> uint32_t -> CMTime.h
		public Flags TimeFlags;

		// CMTimeEpoch -> int64_t -> CMTime.h
		public long TimeEpoch;

#if !COREBUILD
		CMTime (Flags f)
		{
			Value = 0;
			TimeScale = 0;
			TimeEpoch = 0;
			TimeFlags = f;
		}

		CMTime (Flags f, int timescale)
		{
			Value = 0;
			TimeScale = timescale;
			TimeEpoch = 0;
			TimeFlags = f;
		}

		public CMTime (long value, int timescale)
		{
			Value = value;
			TimeScale = timescale;
			TimeFlags = Flags.Valid;
			TimeEpoch = 0;
		}

		public CMTime (long value, int timescale, long epoch)
		{
			Value = value;
			TimeScale = timescale;
			TimeFlags = Flags.Valid;
			TimeEpoch = epoch;
		}

		public bool IsInvalid {
			get {
				return (TimeFlags & Flags.Valid) == 0;
			}
		}

		public bool IsNumeric {
			get {
				return ((TimeFlags & (Flags.Valid | Flags.ImpliedValueFlagsMask)) == Flags.Valid);
			}

		}

		public bool HasBeenRounded {
			get {
				return IsNumeric && ((TimeFlags & Flags.HasBeenRounded) != 0);
			}
		}

		public bool IsIndefinite {
			get {
				return (TimeFlags & kIndefinite) == kIndefinite;
			}
		}

		public bool IsPositiveInfinity {
			get {
				return (TimeFlags & kPositive) == kPositive;
			}
		}

		public bool IsNegativeInfinity {
			get {
				return (TimeFlags & kNegative) == kNegative;
			}
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static CMTime CMTimeAbsoluteValue (CMTime time);

		public CMTime AbsoluteValue {
			get {
				return CMTimeAbsoluteValue (this);
			}
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* int32_t */ int CMTimeCompare (CMTime time1, CMTime time2);

		public static int Compare (CMTime time1, CMTime time2)
		{
			return CMTimeCompare (time1, time2);
		}

		public static bool operator == (CMTime time1, CMTime time2)
		{
			return CMTimeCompare (time1, time2) == 0;
		}

		public static bool operator != (CMTime time1, CMTime time2)
		{
			return CMTimeCompare (time1, time2) != 0;
		}

		public static bool operator < (CMTime time1, CMTime time2)
		{
			return CMTimeCompare (time1, time2) == -1;
		}

		public static bool operator <= (CMTime time1, CMTime time2)
		{
			var comp = CMTimeCompare (time1, time2);
			return comp <= 0;
		}

		public static bool operator > (CMTime time1, CMTime time2)
		{
			return CMTimeCompare (time1, time2) == 1;
		}

		public static bool operator >= (CMTime time1, CMTime time2)
		{
			var comp = CMTimeCompare (time1, time2);
			return comp >= 0;
		}

		public override bool Equals (object? obj)
		{
			if (obj is CMTime time)
				return CMTimeCompare (this, time) == 0;
			return false;
		}

		public override int GetHashCode ()
		{
			return HashCode.Combine (Value, TimeScale, TimeFlags, TimeEpoch);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static CMTime CMTimeAdd (CMTime addend1, CMTime addend2);

		public static CMTime Add (CMTime time1, CMTime time2)
		{
			return CMTimeAdd (time1, time2);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static CMTime CMTimeSubtract (CMTime minuend, CMTime subtrahend);

		public static CMTime Subtract (CMTime minuend, CMTime subtraend)
		{
			return CMTimeSubtract (minuend, subtraend);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static CMTime CMTimeMultiply (CMTime time, /* int32_t */ int multiplier);

		public static CMTime Multiply (CMTime time, int multiplier)
		{
			return CMTimeMultiply (time, multiplier);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static CMTime CMTimeMultiplyByFloat64 (CMTime time, /* Float64 */ double multiplier);

		public static CMTime Multiply (CMTime time, double multiplier)
		{
			return CMTimeMultiplyByFloat64 (time, multiplier);
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[DllImport (Constants.CoreMediaLibrary)]
		extern static CMTime CMTimeMultiplyByRatio (CMTime time, /* int32_t */ int multiplier, /* int32_t */ int divisor);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		public static CMTime Multiply (CMTime time, int multiplier, int divisor)
		{
			return CMTimeMultiplyByRatio (time, multiplier, divisor);
		}

		public static CMTime operator + (CMTime time1, CMTime time2)
		{
			return Add (time1, time2);
		}

		public static CMTime operator - (CMTime minuend, CMTime subtraend)
		{
			return Subtract (minuend, subtraend);
		}

		public static CMTime operator * (CMTime time, int multiplier)
		{
			return Multiply (time, multiplier);
		}

		public static CMTime operator * (CMTime time, double multiplier)
		{
			return Multiply (time, multiplier);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static CMTime CMTimeConvertScale (CMTime time, /* int32_t */ int newScale, CMTimeRoundingMethod method);

		public CMTime ConvertScale (int newScale, CMTimeRoundingMethod method)
		{
			return CMTimeConvertScale (this, newScale, method);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* Float64 */ double CMTimeGetSeconds (CMTime time);

		public double Seconds {
			get {
				return CMTimeGetSeconds (this);
			}
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static CMTime CMTimeMakeWithSeconds (/* Float64 */ double seconds, /* int32_t */ int preferredTimeScale);

		public static CMTime FromSeconds (double seconds, int preferredTimeScale)
		{
			return CMTimeMakeWithSeconds (seconds, preferredTimeScale);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static CMTime CMTimeMaximum (CMTime time1, CMTime time2);

		public static CMTime GetMaximum (CMTime time1, CMTime time2)
		{
			return CMTimeMaximum (time1, time2);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static CMTime CMTimeMinimum (CMTime time1, CMTime time2);

		public static CMTime GetMinimum (CMTime time1, CMTime time2)
		{
			return CMTimeMinimum (time1, time2);
		}

		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.CoreMediaLibrary)]
		extern static CMTime CMTimeFoldIntoRange (CMTime time, CMTimeRange foldRange);

		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		public static CMTime Fold (CMTime time, CMTimeRange foldRange)
		{
			return CMTimeFoldIntoRange (time, foldRange);
		}

		// FIXME: generated will need some changes to emit [Field] in partial struct (not class)
		public readonly static NSString? ValueKey;
		public readonly static NSString? ScaleKey;
		public readonly static NSString? EpochKey;
		public readonly static NSString? FlagsKey;

		static CMTime ()
		{
			var lib = Libraries.CoreMedia.Handle;
			ValueKey = Dlfcn.GetStringConstant (lib, "kCMTimeValueKey");
			ScaleKey = Dlfcn.GetStringConstant (lib, "kCMTimeScaleKey");
			EpochKey = Dlfcn.GetStringConstant (lib, "kCMTimeEpochKey");
			FlagsKey = Dlfcn.GetStringConstant (lib, "kCMTimeFlagsKey");
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* CFDictionaryRef */ IntPtr CMTimeCopyAsDictionary (CMTime time, /* CFAllocatorRef */ IntPtr allocator);

		public NSDictionary ToDictionary ()
		{
			return new NSDictionary (CMTimeCopyAsDictionary (this, IntPtr.Zero), true);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* CFStringRef */ IntPtr CMTimeCopyDescription (/* CFAllocatorRef */ IntPtr allocator, CMTime time);

		public string? Description {
			get {
				return CFString.FromHandle (CMTimeCopyDescription (IntPtr.Zero, this));
			}
		}

		public override string? ToString ()
		{
			return Description;
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static CMTime CMTimeMakeFromDictionary (/* CFDictionaryRef */ IntPtr dict);

		public static CMTime FromDictionary (NSDictionary dict)
		{
			if (dict is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (dict));
			return CMTimeMakeFromDictionary (dict.Handle);
		}
#endif // !COREBUILD
	}
}
