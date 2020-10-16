//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//     
// Copyright 2014 Xamarin Inc.
//
using System;
using System.Runtime.InteropServices;

using CoreFoundation;
using ObjCRuntime;
using Foundation;
using CoreMedia;

namespace VideoToolbox {
	[iOS (8,0), TV (10,2)]
	public class VTVideoEncoder {

		[DllImport (Constants.VideoToolboxLibrary)]
		static extern /* OSStatus */ VTStatus VTCopyVideoEncoderList (
			/* CFDictionaryRef */ IntPtr options,	// documented to accept NULL (no other thing)
			/* CFArrayRef* */ out IntPtr listOfVideoEncodersOut);

		static public VTVideoEncoder [] GetEncoderList ()
		{
			IntPtr array;
			if (VTCopyVideoEncoderList (IntPtr.Zero, out array) != VTStatus.Ok)
				return null;

			var dicts = NSArray.ArrayFromHandle<NSDictionary> (array);
			var ret = new VTVideoEncoder [dicts.Length];
			int i = 0;
			foreach (var dict in dicts){
				var e = new VTVideoEncoder ();
				e.CodecType = (dict [VTVideoEncoderList.CodecType] as NSNumber).Int32Value;
				e.CodecName = dict [VTVideoEncoderList.CodecName] as NSString;
				e.DisplayName = dict [VTVideoEncoderList.DisplayName] as NSString;
				e.EncoderId = dict [VTVideoEncoderList.EncoderID] as NSString;
				e.EncoderName = dict [VTVideoEncoderList.EncoderName] as NSString;
				// here should be missing constants - https://github.com/xamarin/xamarin-macios/issues/9904
				var sfr = dict [VTVideoEncoderList.SupportsFrameReordering] as NSNumber;
				e.SupportsFrameReordering = sfr == null ? true : sfr.BoolValue; // optional, default true
				ret [i++] = e;
			}
			CFObject.CFRelease (array);
			return ret;
		}

		public int CodecType { get; private set; }
		public string CodecName { get; private set; }
		public string DisplayName { get; private set; }
		public string EncoderId { get; private set; }
		public string EncoderName { get; private set; }

		[iOS (14,2)][TV (14,2)][Mac (11,0)]
		public bool SupportsFrameReordering { get; private set; }
				
		internal VTVideoEncoder ()
		{
		}

		[Mac (10,13), iOS (11,0), TV (11,0)]
		[DllImport (Constants.VideoToolboxLibrary)]
		static extern /* OSStatus */ VTStatus VTCopySupportedPropertyDictionaryForEncoder (
			/* int32_t */ int width,
			/* int32_t */ int height,
			/* CMVideoCodecType */ CMVideoCodecType codecType,
			/* CFDictionaryRef */ IntPtr encoderSpecification,
			/* CFStringRef */ out IntPtr outEncoderId,
			/* CFDictionaryRef */ out IntPtr outSupportedProperties
		);

		[Mac (10,13), iOS (11,0), TV (11,0)]
		public static VTSupportedEncoderProperties GetSupportedEncoderProperties (int width, int height, CMVideoCodecType codecType, NSDictionary encoderSpecification = null)
		{
			IntPtr encoderIdPtr = IntPtr.Zero;
			IntPtr supportedPropertiesPtr = IntPtr.Zero;
			var result = VTCopySupportedPropertyDictionaryForEncoder (width, height, codecType, encoderSpecification == null ? IntPtr.Zero : encoderSpecification.Handle, out encoderIdPtr, out supportedPropertiesPtr);

			if (result != VTStatus.Ok) {
				if (encoderIdPtr != IntPtr.Zero)
					CFObject.CFRelease (encoderIdPtr);
				if (supportedPropertiesPtr != IntPtr.Zero)
					CFObject.CFRelease (supportedPropertiesPtr);

				return null;
			}

			// The caller must CFRelease the returned supported properties and encoder ID.
			var ret = new VTSupportedEncoderProperties {
				EncoderId = CFString.FetchString (encoderIdPtr, releaseHandle:true),
				SupportedProperties = Runtime.GetNSObject<NSDictionary> (supportedPropertiesPtr, owns: true)
			};
			return ret;
		}
	}

	[Mac (10,13), iOS (11,0), TV (11,0)]
	public class VTSupportedEncoderProperties {
		public string EncoderId { get; set; }
		public NSDictionary SupportedProperties { get; set; }
	}
}
