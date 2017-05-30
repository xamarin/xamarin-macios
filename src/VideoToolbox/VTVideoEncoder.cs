//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//     
// Copyright 2014 Xamarin Inc.
//
using System;
using System.Runtime.InteropServices;

using XamCore.CoreFoundation;
using XamCore.ObjCRuntime;
using XamCore.Foundation;

namespace XamCore.VideoToolbox {
	[Mac (10,8), iOS (8,0), TV (10,2)]
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
				ret [i] = new VTVideoEncoder (
					dict [VTVideoEncoderList.CodecType] as NSNumber,
					dict [VTVideoEncoderList.CodecName] as NSString,
					dict [VTVideoEncoderList.DisplayName] as NSString,
					dict [VTVideoEncoderList.EncoderID] as NSString,
					dict [VTVideoEncoderList.EncoderName] as NSString);
				i++;
			}
			CFObject.CFRelease (array);
			return ret;
		}

		public int CodecType { get; private set; }
		public string CodecName { get; private set; }
		public string DisplayName { get; private set; }
		public string EncoderId { get; private set; }
		public string EncoderName { get; private set; }
				
		internal VTVideoEncoder (NSNumber codecType, NSString codecName, NSString displayName, NSString encoderId, NSString encoderName)
		{
			CodecType = codecType.Int32Value;
			CodecName = codecName;
			DisplayName = displayName;
			EncoderId = encoderId;
			EncoderName = encoderName;
		}
	}
}