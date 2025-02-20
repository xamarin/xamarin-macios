// 
// CoreMedia.cs: Basic definitions for CoreMedia
//
// Authors: Mono Team
//          Marek Safar (marek.safar@gmail.com)
//
// Copyright 2010-2011 Novell Inc
// Copyright 2012-2014 Xamarin Inc
//

#nullable enable

using System;
using System.Runtime.InteropServices;
using CoreFoundation;
using Foundation;
using ObjCRuntime;

namespace CoreMedia {

	// CMSampleBuffer.h
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	[StructLayout (LayoutKind.Sequential)]
	public struct CMSampleTimingInfo {
		public CMTime Duration;
		public CMTime PresentationTimeStamp;
		public CMTime DecodeTimeStamp;
	}

	// CMTimeRange.h
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	[StructLayout (LayoutKind.Sequential)]
	public struct CMTimeRange {
		public CMTime Start;
		public CMTime Duration;
#if !COREBUILD
		public static readonly CMTimeRange Zero;
		public static readonly CMTimeRange InvalidRange;

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		public static readonly CMTimeRange InvalidMapping;

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		public static NSString? TimeMappingSourceKey { get; private set; }

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		public static NSString? TimeMappingTargetKey { get; private set; }

		static CMTimeRange ()
		{
			var lib = Libraries.CoreMedia.Handle;
			var retZero = Dlfcn.dlsym (lib, "kCMTimeRangeZero");
			Zero = Marshal.PtrToStructure<CMTimeRange> (retZero)!;

			var retInvalid = Dlfcn.dlsym (lib, "kCMTimeRangeInvalid");
			InvalidRange = Marshal.PtrToStructure<CMTimeRange> (retInvalid)!;

			var retMappingInvalid = Dlfcn.dlsym (lib, "kCMTimeMappingInvalid");
			if (retMappingInvalid != IntPtr.Zero)
				InvalidMapping = Marshal.PtrToStructure<CMTimeRange> (retMappingInvalid)!;

			TimeMappingSourceKey = Dlfcn.GetStringConstant (lib, "kCMTimeMappingSourceKey");
			TimeMappingTargetKey = Dlfcn.GetStringConstant (lib, "kCMTimeMappingTargetKey");
		}
#endif // !COREBUILD
	}

	// CMTimeRange.h
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	[StructLayout (LayoutKind.Sequential)]
	public struct CMTimeMapping {
		public CMTimeRange Source;
		public CMTimeRange Target;

#if !COREBUILD
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		public static CMTimeMapping Create (CMTimeRange source, CMTimeRange target)
		{
			return CMTimeMappingMake (source, target);
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		public static CMTimeMapping CreateEmpty (CMTimeRange target)
		{
			return CMTimeMappingMakeEmpty (target);
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		public static CMTimeMapping CreateFromDictionary (NSDictionary dict)
		{
			return CMTimeMappingMakeFromDictionary (dict.Handle);
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		public NSDictionary AsDictionary ()
		{
			return new NSDictionary (CMTimeMappingCopyAsDictionary (this, IntPtr.Zero), true);
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		public string? Description {
			get {
				return CFString.FromHandle (CMTimeMappingCopyDescription (IntPtr.Zero, this), true);
			}
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern CMTimeMapping CMTimeMappingMake (CMTimeRange source, CMTimeRange target);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern CMTimeMapping CMTimeMappingMakeEmpty (CMTimeRange target);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern IntPtr /* CFDictionaryRef* */ CMTimeMappingCopyAsDictionary (CMTimeMapping mapping, IntPtr allocator);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern CMTimeMapping CMTimeMappingMakeFromDictionary (/* CFDictionaryRef* */ IntPtr dict);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern IntPtr /* CFStringRef* */ CMTimeMappingCopyDescription (IntPtr allocator, CMTimeMapping mapping);
#endif // !COREBUILD
	}

	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	[StructLayout (LayoutKind.Sequential)]
	public struct CMTimeScale {
		// CMTime.h
		public static readonly CMTimeScale MaxValue = new CMTimeScale (0x7fffffff);

		// int32_t -> CMTime.h
		public int Value;

		public CMTimeScale (int value)
		{
			if (value < 0 || value > 0x7fffffff)
				ObjCRuntime.ThrowHelper.ThrowArgumentOutOfRangeException (nameof (value), "Between 0 and 0x7fffffff");

			this.Value = value;
		}
	}

	// CMVideoDimensions => int32_t width + int32_t height
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public struct CMVideoDimensions {
		public int Width;
		public int Height;

		public CMVideoDimensions (int width, int height)
		{
			Width = width;
			Height = height;
		}
	}
}
