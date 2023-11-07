// 
// CGImageProperties.cs: Accessors to various kCGImageProperty values
//
// Authors: Marek Safar (marek.safar@gmail.com)
//     
// Copyright 2012-2014, Xamarin Inc.
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

#nullable enable

using System;
using System.Runtime.Versioning;

using Foundation;
using CoreFoundation;
using ObjCRuntime;
#if !WATCH
using CoreImage;
#endif
#if !COREBUILD
using Keys = ImageIO.CGImageProperties;
#endif

namespace CoreGraphics {

	// convenience enum mapped to kCGImagePropertyColorModelXXX fields (see imageio.cs)
	public enum CGImageColorModel {
		RGB,
		Gray,
		CMYK,
		Lab
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CGImageProperties : DictionaryContainer {
#if !COREBUILD

		public CGImageProperties ()
			: base (new NSMutableDictionary ())
		{
		}

		public CGImageProperties (NSDictionary? dictionary)
			: base (dictionary)
		{
		}

		public bool? Alpha {
			get {
				return GetBoolValue (Keys.HasAlpha);
			}
			set {
				SetBooleanValue (Keys.HasAlpha, value);
			}
		}

		public CGImageColorModel? ColorModel {
			get {
				var v = GetNSStringValue (Keys.ColorModel);
				if (v == Keys.ColorModelRGB)
					return CGImageColorModel.RGB;
				if (v == Keys.ColorModelGray)
					return CGImageColorModel.Gray;
				if (v == Keys.ColorModelCMYK)
					return CGImageColorModel.CMYK;
				if (v == Keys.ColorModelLab)
					return CGImageColorModel.Lab;
				return null;
			}
			set {
				NSString key;
				switch (value) {
				case CGImageColorModel.RGB:
					key = Keys.ColorModelRGB;
					break;
				case CGImageColorModel.Gray:
					key = Keys.ColorModelGray;
					break;
				case CGImageColorModel.CMYK:
					key = Keys.ColorModelCMYK;
					break;
				case CGImageColorModel.Lab:
					key = Keys.ColorModelLab;
					break;
				default:
					throw new ArgumentOutOfRangeException ("value");
				}

				SetNativeValue (Keys.ColorModel, key);
			}
		}

		public int? Depth {
			get {
				return GetInt32Value (Keys.Depth);
			}
			set {
				SetNumberValue (Keys.Depth, value);
			}
		}

		public float? DPIHeightF {
			get {
				return GetFloatValue (Keys.DPIHeight);
			}
			set {
				SetNumberValue (Keys.DPIHeight, value);
			}
		}

		public float? DPIWidthF {
			get {
				return GetFloatValue (Keys.DPIWidth);
			}
			set {
				SetNumberValue (Keys.DPIWidth, value);
			}
		}

		public int? FileSize {
			get {
				return GetInt32Value (Keys.FileSize);
			}
			set {
				SetNumberValue (Keys.FileSize, value);
			}
		}

		public bool? IsFloat {
			get {
				return GetBoolValue (Keys.IsFloat);
			}
			set {
				SetBooleanValue (Keys.IsFloat, value);
			}
		}

		public bool? IsIndexed {
			get {
				return GetBoolValue (Keys.IsIndexed);
			}
			set {
				SetBooleanValue (Keys.IsIndexed, value);
			}
		}

#if !WATCH
		public CIImageOrientation? Orientation {
			get {
				return (CIImageOrientation?) GetInt32Value (Keys.Orientation);
			}
			set {
				SetNumberValue (Keys.Orientation, (int?) value);
			}
		}
#endif

		public int? PixelHeight {
			get {
				return GetInt32Value (Keys.PixelHeight);
			}
			set {
				SetNumberValue (Keys.PixelHeight, value);
			}
		}

		public int? PixelWidth {
			get {
				return GetInt32Value (Keys.PixelWidth);
			}
			set {
				SetNumberValue (Keys.PixelWidth, value);
			}
		}

		public string? ProfileName {
			get {
				return GetStringValue (Keys.ProfileName);
			}
			set {
				SetStringValue (Keys.ProfileName, value);
			}
		}

		public CGImagePropertiesExif? Exif {
			get {
				var dict = GetNSDictionary (Keys.ExifDictionary);
				return dict is null ? null : new CGImagePropertiesExif (dict);
			}
		}

		public CGImagePropertiesGps? Gps {
			get {
				var dict = GetNSDictionary (Keys.GPSDictionary);
				return dict is null ? null : new CGImagePropertiesGps (dict);
			}
		}

		public CGImagePropertiesIptc? Iptc {
			get {
				var dict = GetNSDictionary (Keys.IPTCDictionary);
				return dict is null ? null : new CGImagePropertiesIptc (dict);
			}
		}

		public CGImagePropertiesPng? Png {
			get {
				var dict = GetNSDictionary (Keys.PNGDictionary);
				return dict is null ? null : new CGImagePropertiesPng (dict);
			}
		}

		public CGImagePropertiesJfif? Jfif {
			get {
				var dict = GetNSDictionary (Keys.JFIFDictionary);
				return dict is null ? null : new CGImagePropertiesJfif (dict);
			}
		}

		public CGImagePropertiesTiff? Tiff {
			get {
				var dict = GetNSDictionary (Keys.TIFFDictionary);
				return dict is null ? null : new CGImagePropertiesTiff (dict);
			}
		}

#endif
	}

#if !COREBUILD
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CGImagePropertiesExif : DictionaryContainer {
		public CGImagePropertiesExif ()
			: base (new NSMutableDictionary ())
		{
		}

		public CGImagePropertiesExif (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public float? Aperture {
			get {
				return GetFloatValue (Keys.ExifApertureValue);
			}
			set {
				SetNumberValue (Keys.ExifApertureValue, value);
			}
		}

		public float? Brightness {
			get {
				return GetFloatValue (Keys.ExifBrightnessValue);
			}
			set {
				SetNumberValue (Keys.ExifBrightnessValue, value);
			}
		}

		public float? CompressedBitsPerPixel {
			get {
				return GetFloatValue (Keys.ExifCompressedBitsPerPixel);
			}
			set {
				SetNumberValue (Keys.ExifCompressedBitsPerPixel, value);
			}
		}

		public float? DigitalZoomRatio {
			get {
				return GetFloatValue (Keys.ExifDigitalZoomRatio);
			}
			set {
				SetNumberValue (Keys.ExifDigitalZoomRatio, value);
			}
		}

		public float? ExposureBias {
			get {
				return GetFloatValue (Keys.ExifExposureBiasValue);
			}
			set {
				SetNumberValue (Keys.ExifExposureBiasValue, value);
			}
		}

		public float? ExposureIndex {
			get {
				return GetFloatValue (Keys.ExifExposureIndex);
			}
			set {
				SetNumberValue (Keys.ExifExposureIndex, value);
			}
		}

		public float? ExposureTime {
			get {
				return GetFloatValue (Keys.ExifExposureTime);
			}
			set {
				SetNumberValue (Keys.ExifExposureTime, value);
			}
		}

		public int? ExposureProgram {
			get {
				return GetInt32Value (Keys.ExifExposureProgram);
			}
			set {
				SetNumberValue (Keys.ExifExposureProgram, value);
			}
		}

		public bool? Flash {
			get {
				return GetBoolValue (Keys.ExifFlash);
			}
			set {
				SetBooleanValue (Keys.ExifFlash, value);
			}
		}

		public float? FlashEnergy {
			get {
				return GetFloatValue (Keys.ExifFlashEnergy);
			}
			set {
				SetNumberValue (Keys.ExifFlashEnergy, value);
			}
		}

		public float? FocalPlaneXResolution {
			get {
				return GetFloatValue (Keys.ExifFocalPlaneXResolution);
			}
			set {
				SetNumberValue (Keys.ExifFocalPlaneXResolution, value);
			}
		}

		public float? FocalPlaneYResolution {
			get {
				return GetFloatValue (Keys.ExifFocalPlaneYResolution);
			}
			set {
				SetNumberValue (Keys.ExifFocalPlaneYResolution, value);
			}
		}

		public float? GainControl {
			get {
				return GetFloatValue (Keys.ExifGainControl);
			}
			set {
				SetNumberValue (Keys.ExifGainControl, value);
			}
		}

		public int []? ISOSpeedRatings {
			get {
				return GetArray (Keys.ExifISOSpeedRatings, l => new NSNumber (l).Int32Value);
			}
		}

		public float? MaximumLensAperture {
			get {
				return GetFloatValue (Keys.ExifMaxApertureValue);
			}
			set {
				SetNumberValue (Keys.ExifMaxApertureValue, value);
			}
		}

		public int? PixelXDimension {
			get {
				return GetInt32Value (Keys.ExifPixelXDimension);
			}
			set {
				SetNumberValue (Keys.ExifPixelXDimension, value);
			}
		}

		public int? PixelYDimension {
			get {
				return GetInt32Value (Keys.ExifPixelYDimension);
			}
			set {
				SetNumberValue (Keys.ExifPixelYDimension, value);
			}
		}

		public float? SubjectDistance {
			get {
				return GetFloatValue (Keys.ExifSubjectDistance);
			}
			set {
				SetNumberValue (Keys.ExifSubjectDistance, value);
			}
		}

		public float? ShutterSpeed {
			get {
				return GetFloatValue (Keys.ExifShutterSpeedValue);
			}
			set {
				SetNumberValue (Keys.ExifShutterSpeedValue, value);
			}
		}

		// TODO: Many more available but underlying types need to be investigated
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CGImagePropertiesTiff : DictionaryContainer {
		public CGImagePropertiesTiff ()
			: base (new NSMutableDictionary ())
		{
		}

		public CGImagePropertiesTiff (NSDictionary dictionary)
			: base (dictionary)
		{
		}

#if !WATCH
		public CIImageOrientation? Orientation {
			get {
				return (CIImageOrientation?) GetInt32Value (Keys.TIFFOrientation);
			}
			set {
				SetNumberValue (Keys.TIFFOrientation, (int?) value);
			}
		}
#endif

		public int? XResolution {
			get {
				return GetInt32Value (Keys.TIFFXResolution);
			}
			set {
				SetNumberValue (Keys.TIFFXResolution, value);
			}
		}

		public int? YResolution {
			get {
				return GetInt32Value (Keys.TIFFYResolution);
			}
			set {
				SetNumberValue (Keys.TIFFYResolution, value);
			}
		}

		public string? Software {
			get {
				return GetStringValue (Keys.TIFFSoftware);
			}
			set {
				SetStringValue (Keys.TIFFSoftware, value);
			}
		}

		// TODO: Many more available but underlying types need to be investigated
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CGImagePropertiesJfif : DictionaryContainer {
		public CGImagePropertiesJfif ()
			: base (new NSMutableDictionary ())
		{
		}

		public CGImagePropertiesJfif (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public int? XDensity {
			get {
				return GetInt32Value (Keys.JFIFXDensity);
			}
			set {
				SetNumberValue (Keys.JFIFXDensity, value);
			}
		}

		public int? YDensity {
			get {
				return GetInt32Value (Keys.JFIFYDensity);
			}
			set {
				SetNumberValue (Keys.JFIFYDensity, value);
			}
		}

		// TODO: Many more available but underlying types need to be investigated
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CGImagePropertiesPng : DictionaryContainer {
		public CGImagePropertiesPng ()
			: base (new NSMutableDictionary ())
		{
		}

		public CGImagePropertiesPng (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public string? Author {
			get {
				return GetStringValue (Keys.PNGAuthor);
			}
			set {
				SetStringValue (Keys.PNGAuthor, value);
			}
		}

		public string? Description {
			get {
				return GetStringValue (Keys.PNGDescription);
			}
			set {
				SetStringValue (Keys.PNGDescription, value);
			}
		}

		public float? Gamma {
			get {
				return GetFloatValue (Keys.PNGGamma);
			}
			set {
				SetNumberValue (Keys.PNGGamma, value);
			}
		}

		public string? Software {
			get {
				return GetStringValue (Keys.PNGSoftware);
			}
			set {
				SetStringValue (Keys.PNGSoftware, value);
			}
		}

		public int? XPixelsPerMeter {
			get {
				return GetInt32Value (Keys.PNGXPixelsPerMeter);
			}
			set {
				SetNumberValue (Keys.PNGXPixelsPerMeter, value);
			}
		}

		public int? YPixelsPerMeter {
			get {
				return GetInt32Value (Keys.PNGYPixelsPerMeter);
			}
			set {
				SetNumberValue (Keys.PNGYPixelsPerMeter, value);
			}
		}

		public string? Title {
			get {
				return GetStringValue (Keys.PNGTitle);
			}
			set {
				SetStringValue (Keys.PNGTitle, value);
			}
		}

		// TODO: Many more available but underlying types need to be investigated
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CGImagePropertiesGps : DictionaryContainer {
		public CGImagePropertiesGps ()
			: base (new NSMutableDictionary ())
		{
		}

		public CGImagePropertiesGps (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public int? Altitude {
			get {
				return GetInt32Value (Keys.GPSAltitude);
			}
			set {
				SetNumberValue (Keys.GPSAltitude, value);
			}
		}

		public float? Latitude {
			get {
				return GetFloatValue (Keys.GPSLatitude);
			}
			set {
				SetNumberValue (Keys.GPSLatitude, value);
			}
		}

		public string? LatitudeRef {
			get {
				return GetStringValue (Keys.GPSLatitudeRef);
			}
			set {
				SetStringValue (Keys.GPSLatitudeRef, value);
			}
		}

		public float? Longitude {
			get {
				return GetFloatValue (Keys.GPSLongitude);
			}
			set {
				SetNumberValue (Keys.GPSLongitude, value);
			}
		}

		public string? LongitudeRef {
			get {
				return GetStringValue (Keys.GPSLongitudeRef);
			}
			set {
				SetStringValue (Keys.GPSLongitudeRef, value);
			}
		}

		// TODO: Many more available but underlying types need to be investigated
	}


#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CGImagePropertiesIptc : DictionaryContainer {
		public CGImagePropertiesIptc ()
			: base (new NSMutableDictionary ())
		{
		}

		public CGImagePropertiesIptc (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public string? Byline {
			get {
				return GetStringValue (Keys.IPTCByline);
			}
			set {
				SetStringValue (Keys.IPTCByline, value);
			}
		}

		public string? BylineTitle {
			get {
				return GetStringValue (Keys.IPTCBylineTitle);
			}
			set {
				SetStringValue (Keys.IPTCBylineTitle, value);
			}
		}

		public string? CaptionAbstract {
			get {
				return GetStringValue (Keys.IPTCCaptionAbstract);
			}
			set {
				SetStringValue (Keys.IPTCCaptionAbstract, value);
			}
		}

		public string? City {
			get {
				return GetStringValue (Keys.IPTCCity);
			}
			set {
				SetStringValue (Keys.IPTCCity, value);
			}
		}

		public string? ContentLocationName {
			get {
				return GetStringValue (Keys.IPTCContentLocationName);
			}
			set {
				SetStringValue (Keys.IPTCContentLocationName, value);
			}
		}

		public string? CountryPrimaryLocationName {
			get {
				return GetStringValue (Keys.IPTCCountryPrimaryLocationName);
			}
			set {
				SetStringValue (Keys.IPTCCountryPrimaryLocationName, value);
			}
		}

		public string? CopyrightNotice {
			get {
				return GetStringValue (Keys.IPTCCopyrightNotice);
			}
			set {
				SetStringValue (Keys.IPTCCopyrightNotice, value);
			}
		}

		public string? Credit {
			get {
				return GetStringValue (Keys.IPTCCredit);
			}
			set {
				SetStringValue (Keys.IPTCCredit, value);
			}
		}

		public string? Source {
			get {
				return GetStringValue (Keys.IPTCSource);
			}
			set {
				SetStringValue (Keys.IPTCSource, value);
			}
		}

		public string? WriterEditor {
			get {
				return GetStringValue (Keys.IPTCWriterEditor);
			}
			set {
				SetStringValue (Keys.IPTCWriterEditor, value);
			}
		}

		// TODO: Many more available but underlying types need to be investigated
	}


#endif
}
