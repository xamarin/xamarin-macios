//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//     
// Copyright 2014 Xamarin Inc.
//

#nullable enable

using CoreFoundation;
using CoreMedia;

namespace VideoToolbox {
	/// <summary>Class to fetch available encoders</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	public class VTVideoEncoder {

		[DllImport (Constants.VideoToolboxLibrary)]
		unsafe static extern /* OSStatus */ VTStatus VTCopyVideoEncoderList (
			/* CFDictionaryRef */ IntPtr options,   // documented to accept NULL (no other thing)
			/* CFArrayRef* */ IntPtr* listOfVideoEncodersOut);

		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		static public VTVideoEncoder []? GetEncoderList ()
		{
			IntPtr array;
			unsafe {
				if (VTCopyVideoEncoderList (IntPtr.Zero, &array) != VTStatus.Ok)
					return null;
			}

			return NSArray.DictionaryArrayFromHandleDropNullElements<VTVideoEncoder> (array, dict => new VTVideoEncoder (dict), releaseHandle: true);
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int CodecType { get; private set; }
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? CodecName { get; private set; }
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? DisplayName { get; private set; }
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? EncoderId { get; private set; }
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? EncoderName { get; private set; }

		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		public ulong? GpuRegistryId { get; private set; } // optional, same type as `[MTLDevice registryID]`

		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		public NSDictionary? SupportedSelectionProperties { get; private set; }

		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		public NSNumber? PerformanceRating { get; private set; }

		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		public NSNumber? QualityRating { get; private set; }

		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		public bool? InstanceLimit { get; private set; }

		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		public bool? IsHardwareAccelerated { get; private set; }

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		public bool SupportsFrameReordering { get; private set; }

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		public bool IncludeStandardDefinitionDVEncoders { get; private set; }

		internal VTVideoEncoder (NSDictionary dict)
		{
			if (dict [VTVideoEncoderList.CodecType] is NSNumber codecTypeNum)
				CodecType = codecTypeNum.Int32Value;
			else
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException ("VTVideoEncoder 'dict [VTVideoEncoderList.CodecType]' could not be casted to NSNumber.");

			CodecName = dict [VTVideoEncoderList.CodecName] as NSString;
			DisplayName = dict [VTVideoEncoderList.DisplayName] as NSString;
			EncoderId = dict [VTVideoEncoderList.EncoderID] as NSString;
			EncoderName = dict [VTVideoEncoderList.EncoderName] as NSString;

			var constant = VTVideoEncoderList.GpuRegistryId;
			if (constant is not null) {
				var gri = dict [constant] as NSNumber;
				GpuRegistryId = gri?.UInt64Value; // optional
			}

			constant = VTVideoEncoderList.SupportedSelectionProperties;
			if (constant is not null) {
				if (dict.TryGetValue<NSDictionary> (constant, out var d)) // optional
					SupportedSelectionProperties = d;
			}

			constant = VTVideoEncoderList.PerformanceRating;
			if (constant is not null) {
				PerformanceRating = dict [constant] as NSNumber; // optional
			}

			constant = VTVideoEncoderList.QualityRating;
			if (constant is not null) {
				QualityRating = dict [constant] as NSNumber; // optional
			}

			constant = VTVideoEncoderList.InstanceLimit;
			if (constant is not null) {
				var il = dict [constant] as NSNumber;
				InstanceLimit = il?.BoolValue; // optional
			}

			constant = VTVideoEncoderList.IsHardwareAccelerated;
			if (constant is not null) {
				var ha = dict [constant] as NSNumber;
				IsHardwareAccelerated = ha?.BoolValue; // optional
			}

			constant = VTVideoEncoderList.SupportsFrameReordering;
			if (constant is not null) {
				var sfr = dict [constant] as NSNumber;
				SupportsFrameReordering = sfr?.BoolValue ?? true; // optional, default true
			}

			constant = VTVideoEncoderList.IncludeStandardDefinitionDVEncoders;
			if (constant is not null) {
				var includeDef = dict [constant] as NSNumber;
				IncludeStandardDefinitionDVEncoders = includeDef?.BoolValue ?? false; // optional, default false 
			}
		}

		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.VideoToolboxLibrary)]
		unsafe static extern /* OSStatus */ VTStatus VTCopySupportedPropertyDictionaryForEncoder (
			/* int32_t */ int width,
			/* int32_t */ int height,
			/* CMVideoCodecType */ CMVideoCodecType codecType,
			/* CFDictionaryRef */ IntPtr encoderSpecification,
			/* CFStringRef */ IntPtr* outEncoderId,
			/* CFDictionaryRef */ IntPtr* outSupportedProperties
		);

		/// <param name="width">To be added.</param>
		///         <param name="height">To be added.</param>
		///         <param name="codecType">To be added.</param>
		///         <param name="encoderSpecification">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		public static VTSupportedEncoderProperties? GetSupportedEncoderProperties (int width, int height, CMVideoCodecType codecType, NSDictionary? encoderSpecification = null)
		{
			IntPtr encoderIdPtr = IntPtr.Zero;
			IntPtr supportedPropertiesPtr = IntPtr.Zero;
			VTStatus result;
			unsafe {
				result = VTCopySupportedPropertyDictionaryForEncoder (width, height, codecType, encoderSpecification.GetHandle (), &encoderIdPtr, &supportedPropertiesPtr);
				GC.KeepAlive (encoderSpecification);
			}

			if (result != VTStatus.Ok) {
				if (encoderIdPtr != IntPtr.Zero)
					CFObject.CFRelease (encoderIdPtr);
				if (supportedPropertiesPtr != IntPtr.Zero)
					CFObject.CFRelease (supportedPropertiesPtr);

				return null;
			}

			// The caller must CFRelease the returned supported properties and encoder ID.
			var ret = new VTSupportedEncoderProperties {
				EncoderId = CFString.FromHandle (encoderIdPtr, releaseHandle: true),
				SupportedProperties = Runtime.GetNSObject<NSDictionary> (supportedPropertiesPtr, owns: true),
			};
			return ret;
		}
	}

	/// <summary>To be added.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("maccatalyst")]
	public class VTSupportedEncoderProperties {
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? EncoderId { get; set; }
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public NSDictionary? SupportedProperties { get; set; }
	}
}
