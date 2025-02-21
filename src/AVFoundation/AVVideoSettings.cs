// 
// AVVideoSettings.cs: Implements strongly typed access for AV video settings
//
// Authors: Marek Safar (marek.safar@gmail.com)
//     
// Copyright 2012, 2014 Xamarin Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;

using Foundation;
using CoreFoundation;
using ObjCRuntime;
using CoreVideo;

#nullable enable

namespace AVFoundation {

	// Convenience enum for native strings - AVVideoSettings.h
	public enum AVVideoCodec : int {
		H264 = 1,
		JPEG = 2
	}

	// Convenience enum for native strings - AVVideoSettings.h
	public enum AVVideoScalingMode : int {
		/// <summary>Crop to remove edge processing region.</summary>
		///         <remarks>Preserves aspect ratio of cropped source by reducing specified width or height if necessary. This mode does not scale a small source up to larger dimensions.</remarks>
		Fit,
		/// <summary>Crop to remove edge processing region and scales remaining area to fit destination area.</summary>
		///         <remarks>This mode does not preserve the aspect ratio.</remarks>
		Resize,
		/// <summary>Preserves aspect ratio of the source and fills remaining areas with black to fit destination dimensions.</summary>
		ResizeAspect,
		/// <summary>Preserves aspect ratio of the source and crops picture to fit destination dimensions.</summary>
		ResizeAspectFill
	}

	// Convenience enum for native strings - AVVideoSettings.h
	public enum AVVideoProfileLevelH264 : int {
		/// <summary>Specifies a baseline level 3.0 profile.</summary>
		Baseline30 = 1,
		/// <summary>Specifies a baseline level 3.1 profile.</summary>
		Baseline31,
		/// <summary>Specifies a baseline level 4.1 profile.</summary>
		Baseline41,
		/// <summary>Specifies a main level 3.0 profile.</summary>
		Main30,
		/// <summary>Specifies a main level 3.1 profile.</summary>
		Main31,
		/// <summary>Specifies a main level 3.2 profile.</summary>
		Main32,
		/// <summary>Specifies a main level 4.1 profile.</summary>
		Main41,
		/// <summary>Specifies a high level 4.0 profile.</summary>
		High40,
		/// <summary>Specifies a high level 4.1 profile.</summary>
		High41,
		/// <summary>To be added.</summary>
		BaselineAutoLevel,
		/// <summary>To be added.</summary>
		MainAutoLevel,
		/// <summary>To be added.</summary>
		HighAutoLevel,
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class AVVideoSettingsUncompressed : CVPixelBufferAttributes {
#if !COREBUILD
		public AVVideoSettingsUncompressed ()
		{
		}

		public AVVideoSettingsUncompressed (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		internal static AVVideoScalingMode? ScalingModeFromNSString (NSString? k)
		{
			if (k is null)
				return null;
			if (k == AVVideoScalingModeKey.Fit)
				return AVVideoScalingMode.Fit;
			if (k == AVVideoScalingModeKey.Resize)
				return AVVideoScalingMode.Resize;
			if (k == AVVideoScalingModeKey.ResizeAspect)
				return AVVideoScalingMode.ResizeAspect;
			if (k == AVVideoScalingModeKey.ResizeAspectFill)
				return AVVideoScalingMode.ResizeAspectFill;
			return null;
		}

		/// <summary>Represents the video scaling mode.</summary>
		///         <value>
		///         </value>
		///         <remarks>The property uses constant AVVideoScalingModeKey value to access the underlying dictionary.</remarks>
		public AVVideoScalingMode? ScalingMode {
			get {
				return ScalingModeFromNSString (GetNSStringValue (AVVideo.ScalingModeKey));
			}

			set {
				NSString? v;
				switch (value) {
				case AVVideoScalingMode.Fit:
					v = AVVideoScalingModeKey.Fit;
					break;
				case AVVideoScalingMode.Resize:
					v = AVVideoScalingModeKey.Resize;
					break;
				case AVVideoScalingMode.ResizeAspect:
					v = AVVideoScalingModeKey.ResizeAspect;
					break;
				case AVVideoScalingMode.ResizeAspectFill:
					v = AVVideoScalingModeKey.ResizeAspectFill;
					break;
				case null:
					v = null;
					break;
				default:
					throw new ArgumentException ("value");
				}

				if (v is null)
					RemoveValue (AVVideo.ScalingModeKey);
				else
					SetNativeValue (AVVideo.ScalingModeKey, v);
			}
		}
#endif
	}

#if !MONOMAC
	// Convenience enum for native strings - AVVideoSettings.h
	public enum AVVideoH264EntropyMode {
		AdaptiveVariableLength,
		AdaptiveBinaryArithmetic
	}
#endif

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class AVVideoSettingsCompressed : DictionaryContainer {
#if !COREBUILD
		public AVVideoSettingsCompressed ()
			: base (new NSMutableDictionary ())
		{
		}

		public AVVideoSettingsCompressed (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>Represents codec used to encode the video.</summary>
		///         <value>
		///         </value>
		///         <remarks>The property uses constant AVVideoCodecKey value to access the underlying dictionary.</remarks>
		public AVVideoCodec? Codec {
			get {
				var k = GetNSStringValue (AVVideo.CodecKey);
				if (k == AVVideo.CodecH264)
					return AVVideoCodec.H264;
				if (k == AVVideo.CodecJPEG)
					return AVVideoCodec.JPEG;
				return null;
			}

			set {
				NSString? v;
				switch (value) {
				case AVVideoCodec.H264:
					v = AVVideo.CodecH264;
					break;
				case AVVideoCodec.JPEG:
					v = AVVideo.CodecJPEG;
					break;
				case null:
					v = null;
					break;
				default:
					throw new ArgumentException ("value");
				}

				if (v is null)
					RemoveValue (AVVideo.CodecKey);
				else
					SetNativeValue (AVVideo.CodecKey, v);
			}
		}

		// documentation only says 'NSNumber', leaving as int because I doubt we'll need more than 32bits for it ever.
		/// <summary>Represents width of the video in pixels.</summary>
		///         <value>
		///         </value>
		///         <remarks>The property uses constant AVVideoWidthKey value to access the underlying dictionary.</remarks>
		public int? Width {
			set {
				SetNumberValue (AVVideo.WidthKey, value);
			}
			get {
				return GetInt32Value (AVVideo.WidthKey);
			}
		}

		// documentation only says 'NSNumber', leaving as int because I doubt we'll need more than 32bits for it ever.
		/// <summary>Represents height of the video in pixels.</summary>
		///         <value>
		///         </value>
		///         <remarks>The property uses constant AVVideoHeightKey value to access the underlying dictionary.</remarks>
		public int? Height {
			set {
				SetNumberValue (AVVideo.HeightKey, value);
			}
			get {
				return GetInt32Value (AVVideo.HeightKey);
			}
		}

#if NET
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		public double? MaxKeyFrameIntervalDuration {
			get {
				return GetDoubleValue (AVVideo.MaxKeyFrameIntervalDurationKey);
			}
			set {
				SetNumberValue (AVVideo.MaxKeyFrameIntervalDurationKey, value);
			}
		}

#if !MONOMAC
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
#endif
		public bool? AllowFrameReordering {
			get {
				return GetBoolValue (AVVideo.AllowFrameReorderingKey);
			}
			set {
				SetBooleanValue (AVVideo.AllowFrameReorderingKey, value);
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
#endif
		public AVVideoH264EntropyMode? EntropyEncoding {
			get {
				var k = GetNSStringValue (AVVideo.H264EntropyModeKey);
				if (k is null)
					return null;
				if (k == AVVideo.H264EntropyModeCABAC)
					return AVVideoH264EntropyMode.AdaptiveBinaryArithmetic;
				if (k == AVVideo.H264EntropyModeCAVLC)
					return AVVideoH264EntropyMode.AdaptiveVariableLength;
				return null;
			}
			set {
				NSString? v;
				switch (value) {
				case AVVideoH264EntropyMode.AdaptiveBinaryArithmetic:
					v = AVVideo.H264EntropyModeCABAC;
					break;
				case AVVideoH264EntropyMode.AdaptiveVariableLength:
					v = AVVideo.H264EntropyModeCAVLC;
					break;
				case null:
					v = null;
					break;
				default:
					throw new ArgumentException ("value");
				}

				if (v is null)
					RemoveValue (AVVideo.H264EntropyModeKey);
				else
					SetNativeValue (AVVideo.H264EntropyModeKey, v);
			}
		}

		// frame rate can be floating point (29.97 is common for instance)
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
#endif
		public float? ExpectedSourceFrameRate {
			get {
				return GetFloatValue (AVVideo.ExpectedSourceFrameRateKey);
			}
			set {
				SetNumberValue (AVVideo.ExpectedSourceFrameRateKey, value);
			}
		}

		// frame rate can be floating point (29.97 is common for instance)
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
#endif
		public float? AverageNonDroppableFrameRate {
			get {
				return GetFloatValue (AVVideo.AverageNonDroppableFrameRateKey);
			}
			set {
				SetNumberValue (AVVideo.AverageNonDroppableFrameRateKey, value);
			}
		}
#endif

		/// <summary>Represents the video scaling mode.</summary>
		///         <value>
		///         </value>
		///         <remarks>The property uses constant AVVideoScalingModeKey value to access the underlying dictionary.</remarks>
		public AVVideoScalingMode? ScalingMode {
			get {
				return AVVideoSettingsUncompressed.ScalingModeFromNSString (GetNSStringValue (AVVideo.ScalingModeKey));
			}

			set {
				NSString? v;
				switch (value) {
				case AVVideoScalingMode.Fit:
					v = AVVideoScalingModeKey.Fit;
					break;
				case AVVideoScalingMode.Resize:
					v = AVVideoScalingModeKey.Resize;
					break;
				case AVVideoScalingMode.ResizeAspect:
					v = AVVideoScalingModeKey.ResizeAspect;
					break;
				case AVVideoScalingMode.ResizeAspectFill:
					v = AVVideoScalingModeKey.ResizeAspectFill;
					break;
				case null:
					v = null;
					break;
				default:
					throw new ArgumentException ("value");
				}

				if (v is null)
					RemoveValue (AVVideo.ScalingModeKey);
				else
					SetNativeValue (AVVideo.ScalingModeKey, v);
			}
		}

		/// <summary>Specifies access the compression properties.</summary>
		///         <value>
		///         </value>
		///         <remarks>The property uses constant AVVideoCompressionPropertiesKey value to access the underlying dictionary.</remarks>
		public AVVideoCodecSettings? CodecSettings {
			get {
				var dict = GetNSDictionary (AVVideo.CompressionPropertiesKey);
				if (dict is null)
					return null;
				return new AVVideoCodecSettings (dict);
			}

			set {
				SetNativeValue (AVVideo.CompressionPropertiesKey, value?.Dictionary);
			}
		}
#endif
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class AVVideoCodecSettings : DictionaryContainer {
#if !COREBUILD
		public AVVideoCodecSettings ()
			: base (new NSMutableDictionary ())
		{
		}

		public AVVideoCodecSettings (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>Represents average bit rate (as bits per second) used in encoding.</summary>
		///         <value>
		///         </value>
		///         <remarks>The property uses constant AVVideoAverageBitRateKey value to access the underlying dictionary.</remarks>
		public int? AverageBitRate {
			set {
				SetNumberValue (AVVideo.AverageBitRateKey, value);
			}
			get {
				return GetInt32Value (AVVideo.AverageBitRateKey);
			}
		}

		/// <summary>Represents JPEG coded quality.</summary>
		///         <value>Value is in range 0 to 1.0</value>
		///         <remarks>The property uses constant AVVideoQualityKey value to access the underlying dictionary.</remarks>
		public float? JPEGQuality {
			set {
				SetNumberValue (AVVideo.QualityKey, value);
			}
			get {
				return GetFloatValue (AVVideo.QualityKey);
			}
		}

		/// <summary>Specifies a key to access the maximum interval between key frames.</summary>
		///         <value>1 means key frames only.</value>
		///         <remarks>The property uses constant AVVideoMaxKeyFrameIntervalKey value to access the underlying dictionary.</remarks>
		public int? MaxKeyFrameInterval {
			set {
				SetNumberValue (AVVideo.MaxKeyFrameIntervalKey, value);
			}
			get {
				return GetInt32Value (AVVideo.MaxKeyFrameIntervalKey);
			}
		}

		/// <summary>Represents the video profile.</summary>
		///         <value>
		///         </value>
		///         <remarks>The property uses constant AVVideoProfileLevelKey value to access the underlying dictionary.</remarks>
		public AVVideoProfileLevelH264? ProfileLevelH264 {
			get {
				var level = GetNSStringValue (AVVideo.ProfileLevelKey);
				if (level == AVVideo.ProfileLevelH264Baseline30)
					return AVVideoProfileLevelH264.Baseline30;

				if (level == AVVideo.ProfileLevelH264Baseline31)
					return AVVideoProfileLevelH264.Baseline31;

				if (level == AVVideo.ProfileLevelH264Baseline41)
					return AVVideoProfileLevelH264.Baseline41;

				if (level == AVVideo.ProfileLevelH264Main30)
					return AVVideoProfileLevelH264.Main30;

				if (level == AVVideo.ProfileLevelH264Main31)
					return AVVideoProfileLevelH264.Main31;

				if (level == AVVideo.ProfileLevelH264Main32)
					return AVVideoProfileLevelH264.Main32;

				if (level == AVVideo.ProfileLevelH264Main41)
					return AVVideoProfileLevelH264.Main41;

				if (level == AVVideo.ProfileLevelH264High40)
					return AVVideoProfileLevelH264.High40;

				if (level == AVVideo.ProfileLevelH264High41)
					return AVVideoProfileLevelH264.High41;

				if (level == AVVideo.ProfileLevelH264BaselineAutoLevel)
					return AVVideoProfileLevelH264.BaselineAutoLevel;

				if (level == AVVideo.ProfileLevelH264MainAutoLevel)
					return AVVideoProfileLevelH264.MainAutoLevel;

				if (level == AVVideo.ProfileLevelH264HighAutoLevel)
					return AVVideoProfileLevelH264.HighAutoLevel;

				return null;
			}

			set {
				NSString? v;
				switch (value) {
				case AVVideoProfileLevelH264.Baseline30:
					v = AVVideo.ProfileLevelH264Baseline30;
					break;
				case AVVideoProfileLevelH264.Baseline31:
					v = AVVideo.ProfileLevelH264Baseline31;
					break;
				case AVVideoProfileLevelH264.Baseline41:
					v = AVVideo.ProfileLevelH264Baseline41;
					break;
				case AVVideoProfileLevelH264.Main30:
					v = AVVideo.ProfileLevelH264Main30;
					break;
				case AVVideoProfileLevelH264.Main31:
					v = AVVideo.ProfileLevelH264Main31;
					break;
				case AVVideoProfileLevelH264.Main32:
					v = AVVideo.ProfileLevelH264Main32;
					break;
				case AVVideoProfileLevelH264.Main41:
					v = AVVideo.ProfileLevelH264Main41;
					break;

				case AVVideoProfileLevelH264.High40:
					v = AVVideo.ProfileLevelH264High40;
					break;
				case AVVideoProfileLevelH264.High41:
					v = AVVideo.ProfileLevelH264High41;
					break;

				case AVVideoProfileLevelH264.BaselineAutoLevel:
					v = AVVideo.ProfileLevelH264BaselineAutoLevel;
					break;
				case AVVideoProfileLevelH264.MainAutoLevel:
					v = AVVideo.ProfileLevelH264MainAutoLevel;
					break;
				case AVVideoProfileLevelH264.HighAutoLevel:
					v = AVVideo.ProfileLevelH264HighAutoLevel;
					break;
				case null:
					v = null;
					break;
				default:
					throw new ArgumentException ("value");
				}

				if (v is null)
					RemoveValue (AVVideo.ProfileLevelKey);
				else
					SetNativeValue (AVVideo.ProfileLevelKey, v);
			}
		}

		/// <summary>Represents pixel aspect ratio.</summary>
		///         <value>
		///         </value>
		///         <remarks>The property uses constant AVVideoPixelAspectRatioKey value to access the underlying dictionary.</remarks>
		public AVVideoPixelAspectRatioSettings? PixelAspectRatio {
			get {
				var dict = GetNSDictionary (AVVideo.PixelAspectRatioKey);
				if (dict is null)
					return null;
				return new AVVideoPixelAspectRatioSettings (dict);
			}

			set {
				SetNativeValue (AVVideo.PixelAspectRatioKey, value?.Dictionary);
			}
		}

		/// <summary>Represents the clean aperture settings.</summary>
		///         <value>
		///         </value>
		///         <remarks>The property uses constant AVVideoCleanApertureKey value to access the underlying dictionary.</remarks>
		public AVVideoCleanApertureSettings? VideoCleanAperture {
			get {
				var dict = GetNSDictionary (AVVideo.CleanApertureKey);
				if (dict is null)
					return null;
				return new AVVideoCleanApertureSettings (dict);
			}

			set {
				SetNativeValue (AVVideo.CleanApertureKey, value?.Dictionary);
			}
		}
#endif
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class AVVideoPixelAspectRatioSettings : DictionaryContainer {
#if !COREBUILD
		public AVVideoPixelAspectRatioSettings ()
			: base (new NSMutableDictionary ())
		{
		}

		public AVVideoPixelAspectRatioSettings (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>Represents pixel aspect ratio horizontal spacing.</summary>
		///         <value>
		///         </value>
		///         <remarks>The property uses constant AVVideoPixelAspectRatioHorizontalSpacingKey value to access the underlying dictionary.</remarks>
		public int? HorizontalSpacing {
			set {
				SetNumberValue (AVVideo.PixelAspectRatioHorizontalSpacingKey, value);
			}
			get {
				return GetInt32Value (AVVideo.PixelAspectRatioHorizontalSpacingKey);
			}
		}

		/// <summary>Represents pixel aspect ratio vertical spacing.</summary>
		///         <value>
		///         </value>
		///         <remarks>The property uses constant AVVideoPixelAspectRatioVerticalSpacingKey value to access the underlying dictionary.</remarks>
		public int? VerticalSpacing {
			set {
				SetNumberValue (AVVideo.PixelAspectRatioVerticalSpacingKey, value);
			}
			get {
				return GetInt32Value (AVVideo.PixelAspectRatioVerticalSpacingKey);
			}
		}
#endif
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class AVVideoCleanApertureSettings : DictionaryContainer {
#if !COREBUILD
		public AVVideoCleanApertureSettings ()
			: base (new NSMutableDictionary ())
		{
		}

		public AVVideoCleanApertureSettings (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>Represents the clean aperture width.</summary>
		///         <value>
		///         </value>
		///         <remarks>The property uses constant AVVideoCleanApertureWidthKey value to access the underlying dictionary.</remarks>
		public int? Width {
			set {
				SetNumberValue (AVVideo.CleanApertureWidthKey, value);
			}
			get {
				return GetInt32Value (AVVideo.CleanApertureWidthKey);
			}
		}

		/// <summary>Represents the clean aperture height.</summary>
		///         <value>
		///         </value>
		///         <remarks>The property uses constant AVVideoCleanApertureHeightKey value to access the underlying dictionary.</remarks>
		public int? Height {
			set {
				SetNumberValue (AVVideo.CleanApertureHeightKey, value);
			}
			get {
				return GetInt32Value (AVVideo.CleanApertureHeightKey);
			}
		}

		/// <summary>Represents the clean aperture horizontal offset.</summary>
		///         <value>
		///         </value>
		///         <remarks>The property uses constant AVVideoCleanApertureHorizontalOffsetKey value to access the underlying dictionary.</remarks>
		public int? HorizontalOffset {
			set {
				SetNumberValue (AVVideo.CleanApertureHorizontalOffsetKey, value);
			}
			get {
				return GetInt32Value (AVVideo.CleanApertureHorizontalOffsetKey);
			}
		}

		/// <summary>Represents the clean aperture vertical offset.</summary>
		///         <value>
		///         </value>
		///         <remarks>The property uses constant AVVideoCleanApertureVerticalOffsetKey value to access the underlying dictionary.</remarks>
		public int? VerticalOffset {
			set {
				SetNumberValue (AVVideo.CleanApertureVerticalOffsetKey, value);
			}
			get {
				return GetInt32Value (AVVideo.CleanApertureVerticalOffsetKey);
			}
		}
#endif
	}
}
