// 
// CoreMedia.cs: Basic definitions for CoreMedia
//
// Authors: Mono Team
//          Marek Safar (marek.safar@gmail.com)
//
// Copyright 2010-2011 Novell Inc
// Copyright 2012-2014 Xamarin Inc
//
using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;

namespace CoreMedia {

	// CMSampleBuffer.h
	[Watch (6,0)]
	[StructLayout(LayoutKind.Sequential)]
	public struct CMSampleTimingInfo
	{
		public CMTime Duration;
		public CMTime PresentationTimeStamp;
		public CMTime DecodeTimeStamp;
	}

	// CMTimeRange.h
	[Watch (6,0)]
	[StructLayout(LayoutKind.Sequential)]
	public struct CMTimeRange {
		public CMTime Start;
		public CMTime Duration;
#if !COREBUILD
		public static readonly CMTimeRange Zero;

#if !XAMCORE_3_0
#if !WATCH
		[Obsolete ("Use 'InvalidRange'.")]
		public static readonly CMTimeRange Invalid;
#endif // !WATCH
#endif // !XAMCORE_3_0
		public static readonly CMTimeRange InvalidRange;

		[iOS (9,0)][Mac (10,11)]
		public static readonly CMTimeRange InvalidMapping;

		[iOS (9,0)][Mac (10,11)]
		public static NSString TimeMappingSourceKey { get; private set; }

		[iOS (9,0)][Mac (10,11)]
		public static NSString TimeMappingTargetKey { get; private set; }

		static CMTimeRange () {
			var lib = Libraries.CoreMedia.Handle;
			var retZero = Dlfcn.dlsym (lib, "kCMTimeRangeZero");
			Zero = (CMTimeRange)Marshal.PtrToStructure (retZero, typeof(CMTimeRange));

			var retInvalid = Dlfcn.dlsym (lib, "kCMTimeRangeInvalid");
#if !XAMCORE_3_0
			Invalid = (CMTimeRange)Marshal.PtrToStructure (retInvalid, typeof(CMTimeRange));
#endif
			InvalidRange = (CMTimeRange)Marshal.PtrToStructure (retInvalid, typeof(CMTimeRange));

			var retMappingInvalid = Dlfcn.dlsym (lib, "kCMTimeMappingInvalid");
			if (retMappingInvalid  != IntPtr.Zero)
				InvalidMapping = (CMTimeRange)Marshal.PtrToStructure (retMappingInvalid, typeof(CMTimeRange));

			TimeMappingSourceKey = Dlfcn.GetStringConstant (lib, "kCMTimeMappingSourceKey");
			TimeMappingTargetKey = Dlfcn.GetStringConstant (lib, "kCMTimeMappingTargetKey");
		}
#endif // !COREBUILD
	}

	// CMTimeRange.h
	[Watch (6,0)]
	[StructLayout(LayoutKind.Sequential)]
	public struct CMTimeMapping {
		public CMTimeRange Source;
		public CMTimeRange Target;

#if !COREBUILD
		[iOS (9,0)][Mac (10,11)]
		public static CMTimeMapping Create (CMTimeRange source, CMTimeRange target)
		{
			return CMTimeMappingMake (source, target);
		}

		[iOS (9,0)][Mac (10,11)]
		public static CMTimeMapping CreateEmpty (CMTimeRange target)
		{
			return CMTimeMappingMakeEmpty (target);
		}

		[iOS (9,0)][Mac (10,11)]
		public static CMTimeMapping CreateFromDictionary (NSDictionary dict)
		{
			return CMTimeMappingMakeFromDictionary (dict.Handle);
		}

		[iOS (9,0)][Mac (10,11)]
		public NSDictionary AsDictionary ()
		{
			return new NSDictionary (CMTimeMappingCopyAsDictionary (this, IntPtr.Zero), true);
		}

		[iOS (9,0)][Mac (10,11)]
		public string Description
		{
			get
			{
				return (string) new NSString (CMTimeMappingCopyDescription(IntPtr.Zero, this), true);
			}
		}

		[iOS (9,0)][Mac (10,11)]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern CMTimeMapping CMTimeMappingMake (CMTimeRange source, CMTimeRange target);

		[iOS (9,0)][Mac (10,11)]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern CMTimeMapping CMTimeMappingMakeEmpty (CMTimeRange target);

		[iOS (9,0)][Mac (10,11)]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern IntPtr /* CFDictionaryRef* */ CMTimeMappingCopyAsDictionary (CMTimeMapping mapping, IntPtr allocator);

		[iOS (9,0)][Mac (10,11)]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern CMTimeMapping CMTimeMappingMakeFromDictionary (/* CFDictionaryRef* */ IntPtr dict);

		[iOS (9,0)][Mac (10,11)]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern IntPtr /* CFStringRef* */ CMTimeMappingCopyDescription (IntPtr allocator, CMTimeMapping mapping);
#endif // !COREBUILD
	}

	[Watch (6,0)]
	[StructLayout(LayoutKind.Sequential)]
	public struct CMTimeScale
	{
		// CMTime.h
		public static readonly CMTimeScale MaxValue = new CMTimeScale (0x7fffffff);

		// int32_t -> CMTime.h
		public int Value;

		public CMTimeScale (int value)
		{
			if (value < 0 || value > 0x7fffffff)
				throw new ArgumentOutOfRangeException ("value");

			this.Value = value;
		}
	}

	// CMVideoDimensions => int32_t width + int32_t height
	[Watch (6,0)]
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
