//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//     
// Copyright 2014 Xamarin Inc.
//
using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

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
			foreach (var dict in dicts)
				ret [i++] = new VTVideoEncoder (dict);
			CFObject.CFRelease (array);
			return ret;
		}

		public int CodecType { get; private set; }
		public string CodecName { get; private set; }
		public string DisplayName { get; private set; }
		public string EncoderId { get; private set; }
		public string EncoderName { get; private set; }

		[Mac (10,14,6), iOS (13,0), TV (13,0)]
		public ulong? GpuRegistryId { get; private set; } // optional, same type as `[MTLDevice registryID]`

		[Mac (10,14,6), iOS (13,0), TV (13,0)]
		public NSDictionary SupportedSelectionProperties {get; private set; }

		[Mac (10,14,6), iOS (13,0), TV (13,0)]
		public NSNumber PerformanceRating { get; private set; }

		[Mac (10,14,6), iOS (13,0), TV (13,0)]
		public NSNumber QualityRating { get; private set; }

		[Mac (10,14,6), iOS (13,0), TV (13,0)]
		public bool? InstanceLimit { get; private set; }

		[Mac (10,14,6), iOS (13,0), TV (13,0)]
		public bool? IsHardwareAccelerated { get; private set; }

		[iOS (14,2)][TV (14,2)][Mac (11,0)]
		[MacCatalyst (14,2)]
		public bool SupportsFrameReordering { get; private set; }

#if !NET
		[NoiOS, NoTV, NoMacCatalyst, NoMac, NoWatch]
		public bool SupportsMultiPass { get; private set; }
#endif // !NET

		[iOS (15,0), TV (15,0), MacCatalyst (15,0), Mac (12,0), Watch (8,0)]
		public bool IncludeStandardDefinitionDVEncoders { get; private set; }

		internal VTVideoEncoder (NSDictionary dict)
		{
			CodecType = (dict [VTVideoEncoderList.CodecType] as NSNumber).Int32Value;
			CodecName = dict [VTVideoEncoderList.CodecName] as NSString;
			DisplayName = dict [VTVideoEncoderList.DisplayName] as NSString;
			EncoderId = dict [VTVideoEncoderList.EncoderID] as NSString;
			EncoderName = dict [VTVideoEncoderList.EncoderName] as NSString;

			// added in Xcode 11 so the constants won't exists in earlier SDK, making all values optional

			var constant = VTVideoEncoderList.GpuRegistryId;
			if (constant != null) {
				var gri = dict [constant] as NSNumber;
				GpuRegistryId = gri?.UInt64Value; // optional
			}

			constant = VTVideoEncoderList.SupportedSelectionProperties;
			if (constant != null) {
				if (dict.TryGetValue (constant, out NSDictionary d)) // optional
					SupportedSelectionProperties = d;
			}

			constant = VTVideoEncoderList.PerformanceRating;
			if (constant != null) {
				PerformanceRating = dict [constant] as NSNumber; // optional
			}

			constant = VTVideoEncoderList.QualityRating;
			if (constant != null) {
				QualityRating = dict [constant] as NSNumber; // optional
			}

			constant = VTVideoEncoderList.InstanceLimit;
			if (constant != null) {
				var il = dict [constant] as NSNumber;
				InstanceLimit = il?.BoolValue; // optional
			}

			constant = VTVideoEncoderList.IsHardwareAccelerated;
			if (constant != null) {
				var ha = dict [constant] as NSNumber;
				IsHardwareAccelerated = ha?.BoolValue; // optional
			}

			// added in xcode 12.2 so the constant won't exists in earlier SDK

			constant = VTVideoEncoderList.SupportsFrameReordering;
			if (constant != null) {
				var sfr = dict [constant] as NSNumber;
				SupportsFrameReordering = sfr == null ? true : sfr.BoolValue; // optional, default true
			}

			// added in xcode 13
			constant = VTVideoEncoderList.IncludeStandardDefinitionDVEncoders;
			if (constant != null) {
				var includeDef = dict [constant] as NSNumber;
				IncludeStandardDefinitionDVEncoders = includeDef == null ? false : includeDef.BoolValue; // optional, default false 
			}
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
				EncoderId = CFString.FromHandle (encoderIdPtr, releaseHandle:true),
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
