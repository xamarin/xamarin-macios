//
// ImageIO.cs : Constants
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012-2014, Xamarin, Inc.
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

using XamCore.ObjCRuntime;
using XamCore.Foundation;
using XamCore.CoreFoundation;
using XamCore.CoreGraphics;
using System;

namespace XamCore.ImageIO {

	[Since (4,0)]
	[Static]
	// Bad name should end with Keys
	interface CGImageProperties {
		// Format-Specific Dictionaries
		[Field ("kCGImagePropertyTIFFDictionary")]
		NSString TIFFDictionary { get; }
		[Field ("kCGImagePropertyGIFDictionary")]
		NSString GIFDictionary { get; }
		[Field ("kCGImagePropertyJFIFDictionary")]
		NSString JFIFDictionary { get; }
		[Field ("kCGImagePropertyExifDictionary")]
		NSString ExifDictionary { get; }
		[Field ("kCGImagePropertyPNGDictionary")]
		NSString PNGDictionary { get; }
		[Field ("kCGImagePropertyIPTCDictionary")]
		NSString IPTCDictionary { get; }
		[Field ("kCGImagePropertyGPSDictionary")]
		NSString GPSDictionary { get; }
		[Field ("kCGImagePropertyRawDictionary")]
		NSString RawDictionary { get; }
		[Field ("kCGImagePropertyCIFFDictionary")]
		NSString CIFFDictionary { get; }
		[Field ("kCGImageProperty8BIMDictionary")]
		NSString EightBIMDictionary { get; }
		[Field ("kCGImagePropertyDNGDictionary")]
		NSString DNGDictionary { get; }
		[Field ("kCGImagePropertyExifAuxDictionary")]
		NSString ExifAuxDictionary { get; }

		// Camera-Maker Dictionaries
		[Field ("kCGImagePropertyMakerCanonDictionary")]
		NSString MakerCanonDictionary { get; }
		[Field ("kCGImagePropertyMakerNikonDictionary")]
		NSString MakerNikonDictionary { get; }
		[Field ("kCGImagePropertyMakerMinoltaDictionary")]
		NSString MakerMinoltaDictionary { get; }
		[Field ("kCGImagePropertyMakerFujiDictionary")]
		NSString MakerFujiDictionary { get; }
		[Field ("kCGImagePropertyMakerOlympusDictionary")]
		NSString MakerOlympusDictionary { get; }
		[Field ("kCGImagePropertyMakerPentaxDictionary")]
		NSString MakerPentaxDictionary { get; }

		// Image Source Container Properties
		[Field ("kCGImagePropertyFileSize")]
		NSString FileSize { get; }

		// Individual Image Properties
		[Field ("kCGImagePropertyDPIHeight")]
		NSString DPIHeight { get; }
		[Field ("kCGImagePropertyDPIWidth")]
		NSString DPIWidth { get; }
		[Field ("kCGImagePropertyPixelWidth")]
		NSString PixelWidth { get; }
		[Field ("kCGImagePropertyPixelHeight")]
		NSString PixelHeight { get; }
		[Field ("kCGImagePropertyDepth")]
		NSString Depth { get; }
		[Field ("kCGImagePropertyOrientation")]
		NSString Orientation { get; }
		[Field ("kCGImagePropertyIsFloat")]
		NSString IsFloat { get; }
		[Field ("kCGImagePropertyIsIndexed")]
		NSString IsIndexed { get; }
		[Field ("kCGImagePropertyHasAlpha")]
		NSString HasAlpha { get; }
		[Field ("kCGImagePropertyColorModel")]
		NSString ColorModel { get; }
		[Field ("kCGImagePropertyProfileName")]
		NSString ProfileName { get; }

		// Color Model Values

		[Field ("kCGImagePropertyColorModelRGB")]
		NSString ColorModelRGB { get; }
		[Field ("kCGImagePropertyColorModelGray")]
		NSString ColorModelGray { get; }
		[Field ("kCGImagePropertyColorModelCMYK")]
		NSString ColorModelCMYK { get; }
		[Field ("kCGImagePropertyColorModelLab")]
		NSString ColorModelLab { get; }

		// EXIF Dictionary Keys

		[Field ("kCGImagePropertyExifExposureTime")]
		NSString ExifExposureTime { get; }
		[Field ("kCGImagePropertyExifFNumber")]
		NSString ExifFNumber { get; }
		[Field ("kCGImagePropertyExifExposureProgram")]
		NSString ExifExposureProgram { get; }
		[Field ("kCGImagePropertyExifSpectralSensitivity")]
		NSString ExifSpectralSensitivity { get; }
		[Field ("kCGImagePropertyExifISOSpeedRatings")]
		NSString ExifISOSpeedRatings { get; }
		[Field ("kCGImagePropertyExifOECF")]
		NSString ExifOECF { get; }
		[Field ("kCGImagePropertyExifVersion")]
		NSString ExifVersion { get; }
		[Field ("kCGImagePropertyExifDateTimeOriginal")]
		NSString ExifDateTimeOriginal { get; }
		[Field ("kCGImagePropertyExifDateTimeDigitized")]
		NSString ExifDateTimeDigitized { get; }
		[Field ("kCGImagePropertyExifComponentsConfiguration")]
		NSString ExifComponentsConfiguration { get; }
		[Field ("kCGImagePropertyExifCompressedBitsPerPixel")]
		NSString ExifCompressedBitsPerPixel { get; }
		[Field ("kCGImagePropertyExifShutterSpeedValue")]
		NSString ExifShutterSpeedValue { get; }
		[Field ("kCGImagePropertyExifApertureValue")]
		NSString ExifApertureValue { get; }
		[Field ("kCGImagePropertyExifBrightnessValue")]
		NSString ExifBrightnessValue { get; }
		[Field ("kCGImagePropertyExifExposureBiasValue")]
		NSString ExifExposureBiasValue { get; }
		[Field ("kCGImagePropertyExifMaxApertureValue")]
		NSString ExifMaxApertureValue { get; }
		[Field ("kCGImagePropertyExifSubjectDistance")]
		NSString ExifSubjectDistance { get; }
		[Field ("kCGImagePropertyExifMeteringMode")]
		NSString ExifMeteringMode { get; }
		[Field ("kCGImagePropertyExifLightSource")]
		NSString ExifLightSource { get; }
		[Field ("kCGImagePropertyExifFlash")]
		NSString ExifFlash { get; }
		[Field ("kCGImagePropertyExifFocalLength")]
		NSString ExifFocalLength { get; }
		[Field ("kCGImagePropertyExifSubjectArea")]
		NSString ExifSubjectArea { get; }
		[Field ("kCGImagePropertyExifMakerNote")]
		NSString ExifMakerNote { get; }
		[Field ("kCGImagePropertyExifUserComment")]
		NSString ExifUserComment { get; }
		[Field ("kCGImagePropertyExifSubsecTime")]
		NSString ExifSubsecTime { get; }
		[Field ("kCGImagePropertyExifSubsecTimeOrginal")]
		NSString ExifSubsecTimeOrginal { get; }
		[iOS (10,0)][Mac (10,11)]
		[Field ("kCGImagePropertyExifSubsecTimeOriginal")]
		NSString ExifSubsecTimeOriginal { get; }
		[Field ("kCGImagePropertyExifSubsecTimeDigitized")]
		NSString ExifSubsecTimeDigitized { get; }
		[Field ("kCGImagePropertyExifFlashPixVersion")]
		NSString ExifFlashPixVersion { get; }
		[Field ("kCGImagePropertyExifColorSpace")]
		NSString ExifColorSpace { get; }
		[Field ("kCGImagePropertyExifPixelXDimension")]
		NSString ExifPixelXDimension { get; }
		[Field ("kCGImagePropertyExifPixelYDimension")]
		NSString ExifPixelYDimension { get; }
		[Field ("kCGImagePropertyExifRelatedSoundFile")]
		NSString ExifRelatedSoundFile { get; }
		[Field ("kCGImagePropertyExifFlashEnergy")]
		NSString ExifFlashEnergy { get; }
		[Field ("kCGImagePropertyExifSpatialFrequencyResponse")]
		NSString ExifSpatialFrequencyResponse { get; }
		[Field ("kCGImagePropertyExifFocalPlaneXResolution")]
		NSString ExifFocalPlaneXResolution { get; }
		[Field ("kCGImagePropertyExifFocalPlaneYResolution")]
		NSString ExifFocalPlaneYResolution { get; }
		[Field ("kCGImagePropertyExifFocalPlaneResolutionUnit")]
		NSString ExifFocalPlaneResolutionUnit { get; }
		[Field ("kCGImagePropertyExifSubjectLocation")]
		NSString ExifSubjectLocation { get; }
		[Field ("kCGImagePropertyExifExposureIndex")]
		NSString ExifExposureIndex { get; }
		[Field ("kCGImagePropertyExifSensingMethod")]
		NSString ExifSensingMethod { get; }
		[Field ("kCGImagePropertyExifFileSource")]
		NSString ExifFileSource { get; }
		[Field ("kCGImagePropertyExifSceneType")]
		NSString ExifSceneType { get; }
		[Field ("kCGImagePropertyExifCFAPattern")]
		NSString ExifCFAPattern { get; }
		[Field ("kCGImagePropertyExifCustomRendered")]
		NSString ExifCustomRendered { get; }
		[Field ("kCGImagePropertyExifExposureMode")]
		NSString ExifExposureMode { get; }
		[Field ("kCGImagePropertyExifWhiteBalance")]
		NSString ExifWhiteBalance { get; }
		[Field ("kCGImagePropertyExifDigitalZoomRatio")]
		NSString ExifDigitalZoomRatio { get; }
		[Field ("kCGImagePropertyExifFocalLenIn35mmFilm")]
		NSString ExifFocalLenIn35mmFilm { get; }
		[Field ("kCGImagePropertyExifSceneCaptureType")]
		NSString ExifSceneCaptureType { get; }
		[Field ("kCGImagePropertyExifGainControl")]
		NSString ExifGainControl { get; }
		[Field ("kCGImagePropertyExifContrast")]
		NSString ExifContrast { get; }
		[Field ("kCGImagePropertyExifSaturation")]
		NSString ExifSaturation { get; }
		[Field ("kCGImagePropertyExifSharpness")]
		NSString ExifSharpness { get; }
		[Field ("kCGImagePropertyExifDeviceSettingDescription")]
		NSString ExifDeviceSettingDescription { get; }
		[Field ("kCGImagePropertyExifSubjectDistRange")]
		NSString ExifSubjectDistRange { get; }
		[Field ("kCGImagePropertyExifImageUniqueID")]
		NSString ExifImageUniqueID { get; }
		[Field ("kCGImagePropertyExifGamma")]
		NSString ExifGamma { get; }

		// misdocumented (first 4.3, then 5.0) but the constants were not present until 6.x

		[iOS (6,0)][Field ("kCGImagePropertyExifCameraOwnerName")]
		[Mac (10,9)] // Really introduced in 10.8 (according to header), but as a private symbol in the framework (nm shows it as 's'), so we can't access it.
		NSString ExifCameraOwnerName { get; }

		[iOS (6,0)][Field ("kCGImagePropertyExifBodySerialNumber")]
		[Mac (10,9)] // Really introduced in 10.8 (according to header), but as a private symbol in the framework (nm shows it as 's'), so we can't access it.
		NSString ExifBodySerialNumber { get; }

		[iOS (6,0)][Field ("kCGImagePropertyExifLensSpecification")]
		[Mac (10,9)] // Really introduced in 10.8 (according to header), but as a private symbol in the framework (nm shows it as 's'), so we can't access it.
		NSString ExifLensSpecification { get; }

		[iOS (6,0)][Field ("kCGImagePropertyExifLensMake")]
		[Mac (10,9)] // Really introduced in 10.8 (according to header), but as a private symbol in the framework (nm shows it as 's'), so we can't access it.
		NSString ExifLensMake { get; }

		[iOS (6,0)][Field ("kCGImagePropertyExifLensModel")]
		[Mac (10,9)] // Really introduced in 10.8 (according to header), but as a private symbol in the framework (nm shows it as 's'), so we can't access it.
		NSString ExifLensModel { get; }

		[iOS (6,0)][Field ("kCGImagePropertyExifLensSerialNumber")]
		[Mac (10,9)] // Really introduced in 10.8 (according to header), but as a private symbol in the framework (nm shows it as 's'), so we can't access it.
		NSString ExifLensSerialNumber { get; }

		// EXIF Auxiliary Dictionary Keys

		[Field ("kCGImagePropertyExifAuxLensInfo")]
		NSString ExifAuxLensInfo { get; }
		[Field ("kCGImagePropertyExifAuxLensModel")]
		NSString ExifAuxLensModel { get; }
		[Field ("kCGImagePropertyExifAuxSerialNumber")]
		NSString ExifAuxSerialNumber { get; }
		[Field ("kCGImagePropertyExifAuxLensID")]
		NSString ExifAuxLensID { get; }
		[Field ("kCGImagePropertyExifAuxLensSerialNumber")]
		NSString ExifAuxLensSerialNumber { get; }
		[Field ("kCGImagePropertyExifAuxImageNumber")]
		NSString ExifAuxImageNumber { get; }
		[Field ("kCGImagePropertyExifAuxFlashCompensation")]
		NSString ExifAuxFlashCompensation { get; }
		[Field ("kCGImagePropertyExifAuxOwnerName")]
		NSString ExifAuxOwnerName { get; }
		[Field ("kCGImagePropertyExifAuxFirmware")]
		NSString ExifAuxFirmware { get; }

		// GIF Dictionary Keys

		[Field ("kCGImagePropertyGIFLoopCount")]
		NSString GIFLoopCount { get; }
		[Field ("kCGImagePropertyGIFDelayTime")]
		NSString GIFDelayTime { get; }
		[Field ("kCGImagePropertyGIFImageColorMap")]
		NSString GIFImageColorMap { get; }
		[Field ("kCGImagePropertyGIFHasGlobalColorMap")]
		NSString GIFHasGlobalColorMap { get; }
		[Field ("kCGImagePropertyGIFUnclampedDelayTime")]
		NSString GIFUnclampedDelayTime { get; }

		// GPS Dictionary Keys

		[Field ("kCGImagePropertyGPSVersion")]
		NSString GPSVersion { get; }
		[Field ("kCGImagePropertyGPSLatitudeRef")]
		NSString GPSLatitudeRef { get; }
		[Field ("kCGImagePropertyGPSLatitude")]
		NSString GPSLatitude { get; }
		[Field ("kCGImagePropertyGPSLongitudeRef")]
		NSString GPSLongitudeRef { get; }
		[Field ("kCGImagePropertyGPSLongitude")]
		NSString GPSLongitude { get; }
		[Field ("kCGImagePropertyGPSAltitudeRef")]
		NSString GPSAltitudeRef { get; }
		[Field ("kCGImagePropertyGPSAltitude")]
		NSString GPSAltitude { get; }
		[Field ("kCGImagePropertyGPSTimeStamp")]
		NSString GPSTimeStamp { get; }
		[Field ("kCGImagePropertyGPSSatellites")]
		NSString GPSSatellites { get; }
		[Field ("kCGImagePropertyGPSStatus")]
		NSString GPSStatus { get; }
		[Field ("kCGImagePropertyGPSMeasureMode")]
		NSString GPSMeasureMode { get; }
		[Field ("kCGImagePropertyGPSDOP")]
		NSString GPSDOP { get; }
		[Field ("kCGImagePropertyGPSSpeedRef")]
		NSString GPSSpeedRef { get; }
		[Field ("kCGImagePropertyGPSSpeed")]
		NSString GPSSpeed { get; }
		[Field ("kCGImagePropertyGPSTrackRef")]
		NSString GPSTrackRef { get; }
		[Field ("kCGImagePropertyGPSTrack")]
		NSString GPSTrack { get; }
		[Field ("kCGImagePropertyGPSImgDirectionRef")]
		NSString GPSImgDirectionRef { get; }
		[Field ("kCGImagePropertyGPSImgDirection")]
		NSString GPSImgDirection { get; }
		[Field ("kCGImagePropertyGPSMapDatum")]
		NSString GPSMapDatum { get; }
		[Field ("kCGImagePropertyGPSDestLatitudeRef")]
		NSString GPSDestLatitudeRef { get; }
		[Field ("kCGImagePropertyGPSDestLatitude")]
		NSString GPSDestLatitude { get; }
		[Field ("kCGImagePropertyGPSDestLongitudeRef")]
		NSString GPSDestLongitudeRef { get; }
		[Field ("kCGImagePropertyGPSDestLongitude")]
		NSString GPSDestLongitude { get; }
		[Field ("kCGImagePropertyGPSDestBearingRef")]
		NSString GPSDestBearingRef { get; }
		[Field ("kCGImagePropertyGPSDestBearing")]
		NSString GPSDestBearing { get; }
		[Field ("kCGImagePropertyGPSDestDistanceRef")]
		NSString GPSDestDistanceRef { get; }
		[Field ("kCGImagePropertyGPSDestDistance")]
		NSString GPSDestDistance { get; }
		[Field ("kCGImagePropertyGPSAreaInformation")]
		NSString GPSAreaInformation { get; }
		[Field ("kCGImagePropertyGPSDateStamp")]
		NSString GPSDateStamp { get; }
		[Field ("kCGImagePropertyGPSDifferental")]
		NSString GPSDifferental { get; }

		[iOS (8,0), Mac (10,10)]
		[Field ("kCGImagePropertyGPSHPositioningError")]
		NSString GPSHPositioningError { get; }

		// IPTC Dictionary Keys

		[Field ("kCGImagePropertyIPTCObjectTypeReference")]
		NSString IPTCObjectTypeReference { get; }
		[Field ("kCGImagePropertyIPTCObjectAttributeReference")]
		NSString IPTCObjectAttributeReference { get; }
		[Field ("kCGImagePropertyIPTCObjectName")]
		NSString IPTCObjectName { get; }
		[Field ("kCGImagePropertyIPTCEditStatus")]
		NSString IPTCEditStatus { get; }
		[Field ("kCGImagePropertyIPTCEditorialUpdate")]
		NSString IPTCEditorialUpdate { get; }
		[Field ("kCGImagePropertyIPTCUrgency")]
		NSString IPTCUrgency { get; }
		[Field ("kCGImagePropertyIPTCSubjectReference")]
		NSString IPTCSubjectReference { get; }
		[Field ("kCGImagePropertyIPTCCategory")]
		NSString IPTCCategory { get; }
		[Field ("kCGImagePropertyIPTCSupplementalCategory")]
		NSString IPTCSupplementalCategory { get; }
		[Field ("kCGImagePropertyIPTCFixtureIdentifier")]
		NSString IPTCFixtureIdentifier { get; }
		[Field ("kCGImagePropertyIPTCKeywords")]
		NSString IPTCKeywords { get; }
		[Field ("kCGImagePropertyIPTCContentLocationCode")]
		NSString IPTCContentLocationCode { get; }
		[Field ("kCGImagePropertyIPTCContentLocationName")]
		NSString IPTCContentLocationName { get; }
		[Field ("kCGImagePropertyIPTCReleaseDate")]
		NSString IPTCReleaseDate { get; }
		[Field ("kCGImagePropertyIPTCReleaseTime")]
		NSString IPTCReleaseTime { get; }
		[Field ("kCGImagePropertyIPTCExpirationDate")]
		NSString IPTCExpirationDate { get; }
		[Field ("kCGImagePropertyIPTCExpirationTime")]
		NSString IPTCExpirationTime { get; }
		[Field ("kCGImagePropertyIPTCSpecialInstructions")]
		NSString IPTCSpecialInstructions { get; }
		[Field ("kCGImagePropertyIPTCActionAdvised")]
		NSString IPTCActionAdvised { get; }
		[Field ("kCGImagePropertyIPTCReferenceService")]
		NSString IPTCReferenceService { get; }
		[Field ("kCGImagePropertyIPTCReferenceDate")]
		NSString IPTCReferenceDate { get; }
		[Field ("kCGImagePropertyIPTCReferenceNumber")]
		NSString IPTCReferenceNumber { get; }
		[Field ("kCGImagePropertyIPTCDateCreated")]
		NSString IPTCDateCreated { get; }
		[Field ("kCGImagePropertyIPTCTimeCreated")]
		NSString IPTCTimeCreated { get; }
		[Field ("kCGImagePropertyIPTCDigitalCreationDate")]
		NSString IPTCDigitalCreationDate { get; }
		[Field ("kCGImagePropertyIPTCDigitalCreationTime")]
		NSString IPTCDigitalCreationTime { get; }
		[Field ("kCGImagePropertyIPTCOriginatingProgram")]
		NSString IPTCOriginatingProgram { get; }
		[Field ("kCGImagePropertyIPTCProgramVersion")]
		NSString IPTCProgramVersion { get; }
		[Field ("kCGImagePropertyIPTCObjectCycle")]
		NSString IPTCObjectCycle { get; }
		[Field ("kCGImagePropertyIPTCByline")]
		NSString IPTCByline { get; }
		[Field ("kCGImagePropertyIPTCBylineTitle")]
		NSString IPTCBylineTitle { get; }
		[Field ("kCGImagePropertyIPTCCity")]
		NSString IPTCCity { get; }
		[Field ("kCGImagePropertyIPTCSubLocation")]
		NSString IPTCSubLocation { get; }
		[Field ("kCGImagePropertyIPTCProvinceState")]
		NSString IPTCProvinceState { get; }
		[Field ("kCGImagePropertyIPTCCountryPrimaryLocationCode")]
		NSString IPTCCountryPrimaryLocationCode { get; }
		[Field ("kCGImagePropertyIPTCCountryPrimaryLocationName")]
		NSString IPTCCountryPrimaryLocationName { get; }
		[Field ("kCGImagePropertyIPTCOriginalTransmissionReference")]
		NSString IPTCOriginalTransmissionReference { get; }
		[Field ("kCGImagePropertyIPTCHeadline")]
		NSString IPTCHeadline { get; }
		[Field ("kCGImagePropertyIPTCCredit")]
		NSString IPTCCredit { get; }
		[Field ("kCGImagePropertyIPTCSource")]
		NSString IPTCSource { get; }
		[Field ("kCGImagePropertyIPTCCopyrightNotice")]
		NSString IPTCCopyrightNotice { get; }
		[Field ("kCGImagePropertyIPTCContact")]
		NSString IPTCContact { get; }
		[Field ("kCGImagePropertyIPTCCaptionAbstract")]
		NSString IPTCCaptionAbstract { get; }
		[Field ("kCGImagePropertyIPTCWriterEditor")]
		NSString IPTCWriterEditor { get; }
		[Field ("kCGImagePropertyIPTCImageType")]
		NSString IPTCImageType { get; }
		[Field ("kCGImagePropertyIPTCImageOrientation")]
		NSString IPTCImageOrientation { get; }
		[Field ("kCGImagePropertyIPTCLanguageIdentifier")]
		NSString IPTCLanguageIdentifier { get; }
		[Field ("kCGImagePropertyIPTCStarRating")]
		NSString IPTCStarRating { get; }
		[Field ("kCGImagePropertyIPTCCreatorContactInfo")]
		NSString IPTCCreatorContactInfo { get; }
		[Field ("kCGImagePropertyIPTCRightsUsageTerms")]
		NSString IPTCRightsUsageTerms { get; }
		[Field ("kCGImagePropertyIPTCScene")]
		NSString IPTCScene { get; }

		// IPTC Creator Contact Info Dictionary Keys

		[Field ("kCGImagePropertyIPTCContactInfoCity")]
		NSString IPTCContactInfoCity { get; }
		[Field ("kCGImagePropertyIPTCContactInfoCountry")]
		NSString IPTCContactInfoCountry { get; }
		[Field ("kCGImagePropertyIPTCContactInfoAddress")]
		NSString IPTCContactInfoAddress { get; }
		[Field ("kCGImagePropertyIPTCContactInfoPostalCode")]
		NSString IPTCContactInfoPostalCode { get; }
		[Field ("kCGImagePropertyIPTCContactInfoStateProvince")]
		NSString IPTCContactInfoStateProvince { get; }
		[Field ("kCGImagePropertyIPTCContactInfoEmails")]
		NSString IPTCContactInfoEmails { get; }
		[Field ("kCGImagePropertyIPTCContactInfoPhones")]
		NSString IPTCContactInfoPhones { get; }
		[Field ("kCGImagePropertyIPTCContactInfoWebURLs")]
		NSString IPTCContactInfoWebURLs { get; }

		// JFIF Dictionary Keys

		[Field ("kCGImagePropertyJFIFVersion")]
		NSString JFIFVersion { get; }
		[Field ("kCGImagePropertyJFIFXDensity")]
		NSString JFIFXDensity { get; }
		[Field ("kCGImagePropertyJFIFYDensity")]
		NSString JFIFYDensity { get; }
		[Field ("kCGImagePropertyJFIFDensityUnit")]
		NSString JFIFDensityUnit { get; }
		[Field ("kCGImagePropertyJFIFIsProgressive")]
		NSString JFIFIsProgressive { get; }

		// PNG Dictionary Keys

		[Field ("kCGImagePropertyPNGGamma")]
		NSString PNGGamma { get; }
		[Field ("kCGImagePropertyPNGInterlaceType")]
		NSString PNGInterlaceType { get; }
		[Field ("kCGImagePropertyPNGXPixelsPerMeter")]
		NSString PNGXPixelsPerMeter { get; }
		[Field ("kCGImagePropertyPNGYPixelsPerMeter")]
		NSString PNGYPixelsPerMeter { get; }
		[Field ("kCGImagePropertyPNGsRGBIntent")]
		NSString PNGsRGBIntent { get; }
		[Field ("kCGImagePropertyPNGChromaticities")]
		NSString PNGChromaticities { get; }
		[Since (5,0)][Field ("kCGImagePropertyPNGAuthor")]
		NSString PNGAuthor { get; }
		[Since (5,0)][Field ("kCGImagePropertyPNGCopyright")]
		NSString PNGCopyright { get; }
		[Since (5,0)][Field ("kCGImagePropertyPNGCreationTime")]
		NSString PNGCreationTime { get; }
		[Since (5,0)][Field ("kCGImagePropertyPNGDescription")]
		NSString PNGDescription { get; }
		[Since (5,0)][Field ("kCGImagePropertyPNGModificationTime")]
		NSString PNGModificationTime { get; }
		[Since (5,0)][Field ("kCGImagePropertyPNGSoftware")]
		NSString PNGSoftware { get; }
		[Since (5,0)][Field ("kCGImagePropertyPNGTitle")]
		NSString PNGTitle { get; }

		[iOS (9,0)][Mac (10,11)]
		[Field ("kCGImagePropertyPNGCompressionFilter")]
		NSString PNGCompressionFilter { get; }

		[iOS (8,0)][Mac (10,10)]
		[Field ("kCGImagePropertyAPNGLoopCount")]
		NSString PNGLoopCount { get; }

		[iOS (8,0)][Mac (10,10)]
		[Field ("kCGImagePropertyAPNGDelayTime")]
		NSString PNGDelayTime { get; }

		[iOS (8,0)][Mac (10,10)]
		[Field ("kCGImagePropertyAPNGUnclampedDelayTime")]
		NSString PNGUnclampedDelayTime { get; }

		// TIFF Dictionary Keys

		[Field ("kCGImagePropertyTIFFCompression")]
		NSString TIFFCompression { get; }
		[Field ("kCGImagePropertyTIFFPhotometricInterpretation")]
		NSString TIFFPhotometricInterpretation { get; }
		[Field ("kCGImagePropertyTIFFDocumentName")]
		NSString TIFFDocumentName { get; }
		[Field ("kCGImagePropertyTIFFImageDescription")]
		NSString TIFFImageDescription { get; }
		[Field ("kCGImagePropertyTIFFMake")]
		NSString TIFFMake { get; }
		[Field ("kCGImagePropertyTIFFModel")]
		NSString TIFFModel { get; }
		[Field ("kCGImagePropertyTIFFOrientation")]
		NSString TIFFOrientation { get; }
		[Field ("kCGImagePropertyTIFFXResolution")]
		NSString TIFFXResolution { get; }
		[Field ("kCGImagePropertyTIFFYResolution")]
		NSString TIFFYResolution { get; }
		[Field ("kCGImagePropertyTIFFResolutionUnit")]
		NSString TIFFResolutionUnit { get; }
		[Field ("kCGImagePropertyTIFFSoftware")]
		NSString TIFFSoftware { get; }
		[Field ("kCGImagePropertyTIFFTransferFunction")]
		NSString TIFFTransferFunction { get; }
		[Field ("kCGImagePropertyTIFFDateTime")]
		NSString TIFFDateTime { get; }
		[Field ("kCGImagePropertyTIFFArtist")]
		NSString TIFFArtist { get; }
		[Field ("kCGImagePropertyTIFFHostComputer")]
		NSString TIFFHostComputer { get; }
		[Field ("kCGImagePropertyTIFFWhitePoint")]
		NSString TIFFWhitePoint { get; }
		[Field ("kCGImagePropertyTIFFPrimaryChromaticities")]
		NSString TIFFPrimaryChromaticities { get; }

		[iOS (9,0)][Mac (10,11)]
		[Field ("kCGImagePropertyTIFFTileLength")]
		NSString TIFFTileLength { get; }
		[iOS (9,0)][Mac (10,11)]
		[Field ("kCGImagePropertyTIFFTileWidth")]
		NSString TIFFTileWidth { get; }

		// DNG Dictionary Keys

		[Field ("kCGImagePropertyDNGVersion")]
		NSString DNGVersion { get; }
		[Field ("kCGImagePropertyDNGBackwardVersion")]
		NSString DNGBackwardVersion { get; }
		[Field ("kCGImagePropertyDNGUniqueCameraModel")]
		NSString DNGUniqueCameraModel { get; }
		[Field ("kCGImagePropertyDNGLocalizedCameraModel")]
		NSString DNGLocalizedCameraModel { get; }
		[Field ("kCGImagePropertyDNGCameraSerialNumber")]
		NSString DNGCameraSerialNumber { get; }
		[Field ("kCGImagePropertyDNGLensInfo")]
		NSString DNGLensInfo { get; }

		[iOS (10,0)][Mac (12,0)]
		[Watch (3,0)]
		[TV (10,0)]
		[Field ("kCGImagePropertyDNGBlackLevel")]
		NSString DNGBlackLevel { get; }

		[iOS (10,0)][Mac (12,0)]
		[Watch (3,0)]
		[TV (10,0)]
		[Field ("kCGImagePropertyDNGWhiteLevel")]
		NSString DNGWhiteLevel { get; }

		[iOS (10,0)][Mac (12,0)]
		[Watch (3,0)]
		[TV (10,0)]
		[Field ("kCGImagePropertyDNGCalibrationIlluminant1")]
		NSString DNGCalibrationIlluminant1 { get; }

		[iOS (10,0)][Mac (12,0)]
		[Watch (3,0)]
		[TV (10,0)]
		[Field ("kCGImagePropertyDNGCalibrationIlluminant2")]
		NSString DNGCalibrationIlluminant2 { get; }

		[iOS (10,0)][Mac (12,0)]
		[Watch (3,0)]
		[TV (10,0)]
		[Field ("kCGImagePropertyDNGColorMatrix1")]
		NSString DNGColorMatrix1 { get; }

		[iOS (10,0)][Mac (12,0)]
		[Watch (3,0)]
		[TV (10,0)]
		[Field ("kCGImagePropertyDNGColorMatrix2")]
		NSString DNGColorMatrix2 { get; }

		[iOS (10,0)][Mac (12,0)]
		[Watch (3,0)]
		[TV (10,0)]
		[Field ("kCGImagePropertyDNGCameraCalibration1")]
		NSString DNGCameraCalibration1 { get; }

		[iOS (10,0)][Mac (12,0)]
		[Watch (3,0)]
		[TV (10,0)]
		[Field ("kCGImagePropertyDNGCameraCalibration2")]
		NSString DNGCameraCalibration2 { get; }

		[iOS (10,0)][Mac (12,0)]
		[Watch (3,0)]
		[TV (10,0)]
		[Field ("kCGImagePropertyDNGAsShotNeutral")]
		NSString DNGAsShotNeutral { get; }

		[iOS (10,0)][Mac (12,0)]
		[Watch (3,0)]
		[TV (10,0)]
		[Field ("kCGImagePropertyDNGAsShotWhiteXY")]
		NSString DNGAsShotWhiteXY { get; }

		[iOS (10,0)][Mac (12,0)]
		[Watch (3,0)]
		[TV (10,0)]
		[Field ("kCGImagePropertyDNGBaselineExposure")]
		NSString DNGBaselineExposure { get; }

		[iOS (10,0)][Mac (12,0)]
		[Watch (3,0)]
		[TV (10,0)]
		[Field ("kCGImagePropertyDNGBaselineNoise")]
		NSString DNGBaselineNoise { get; }

		[iOS (10,0)][Mac (12,0)]
		[Watch (3,0)]
		[TV (10,0)]
		[Field ("kCGImagePropertyDNGBaselineSharpness")]
		NSString DNGBaselineSharpness { get; }

		[iOS (10,0)][Mac (12,0)]
		[Watch (3,0)]
		[TV (10,0)]
		[Field ("kCGImagePropertyDNGPrivateData")]
		NSString DNGPrivateData { get; }

		[iOS (10,0)][Mac (12,0)]
		[Watch (3,0)]
		[TV (10,0)]
		[Field ("kCGImagePropertyDNGCameraCalibrationSignature")]
		NSString DNGCameraCalibrationSignature { get; }

		[iOS (10,0)][Mac (12,0)]
		[Watch (3,0)]
		[TV (10,0)]
		[Field ("kCGImagePropertyDNGProfileCalibrationSignature")]
		NSString DNGProfileCalibrationSignature { get; }

		[iOS (10,0)][Mac (12,0)]
		[Watch (3,0)]
		[TV (10,0)]
		[Field ("kCGImagePropertyDNGNoiseProfile")]
		NSString DNGNoiseProfile { get; }

		[iOS (10,0)][Mac (12,0)]
		[Watch (3,0)]
		[TV (10,0)]
		[Field ("kCGImagePropertyDNGWarpRectilinear")]
		NSString DNGWarpRectilinear { get; }

		[iOS (10,0)][Mac (12,0)]
		[Watch (3,0)]
		[TV (10,0)]
		[Field ("kCGImagePropertyDNGWarpFisheye")]
		NSString DNGWarpFisheye { get; }

		[iOS (10,0)][Mac (12,0)]
		[Watch (3,0)]
		[TV (10,0)]
		[Field ("kCGImagePropertyDNGFixVignetteRadial")]
		NSString DNGFixVignetteRadial { get; }

		// 8BIM Dictionary Keys

		[Field ("kCGImageProperty8BIMLayerNames")]
		NSString EightBIMLayerNames { get; }

		// CIFF Dictionary Keys

		[Field ("kCGImagePropertyCIFFDescription")]
		NSString CIFFDescription { get; }
		[Field ("kCGImagePropertyCIFFFirmware")]
		NSString CIFFFirmware { get; }
		[Field ("kCGImagePropertyCIFFOwnerName")]
		NSString CIFFOwnerName { get; }
		[Field ("kCGImagePropertyCIFFImageName")]
		NSString CIFFImageName { get; }
		[Field ("kCGImagePropertyCIFFImageFileName")]
		NSString CIFFImageFileName { get; }
		[Field ("kCGImagePropertyCIFFReleaseMethod")]
		NSString CIFFReleaseMethod { get; }
		[Field ("kCGImagePropertyCIFFReleaseTiming")]
		NSString CIFFReleaseTiming { get; }
		[Field ("kCGImagePropertyCIFFRecordID")]
		NSString CIFFRecordID { get; }
		[Field ("kCGImagePropertyCIFFSelfTimingTime")]
		NSString CIFFSelfTimingTime { get; }
		[Field ("kCGImagePropertyCIFFCameraSerialNumber")]
		NSString CIFFCameraSerialNumber { get; }
		[Field ("kCGImagePropertyCIFFImageSerialNumber")]
		NSString CIFFImageSerialNumber { get; }
		[Field ("kCGImagePropertyCIFFContinuousDrive")]
		NSString CIFFContinuousDrive { get; }
		[Field ("kCGImagePropertyCIFFFocusMode")]
		NSString CIFFFocusMode { get; }
		[Field ("kCGImagePropertyCIFFMeteringMode")]
		NSString CIFFMeteringMode { get; }
		[Field ("kCGImagePropertyCIFFShootingMode")]
		NSString CIFFShootingMode { get; }
		[Field ("kCGImagePropertyCIFFLensMaxMM")]
		NSString CIFFLensMaxMM { get; }
		[Field ("kCGImagePropertyCIFFLensMinMM")]
		NSString CIFFLensMinMM { get; }
		[Field ("kCGImagePropertyCIFFLensModel")]
		NSString CIFFLensModel { get; }
		[Field ("kCGImagePropertyCIFFWhiteBalanceIndex")]
		NSString CIFFWhiteBalanceIndex { get; }
		[Field ("kCGImagePropertyCIFFFlashExposureComp")]
		NSString CIFFFlashExposureComp { get; }
		[Field ("kCGImagePropertyCIFFMeasuredEV")]
		NSString CIFFMeasuredEV { get; }

		// Nikon Camera Dictionary Keys

		[Field ("kCGImagePropertyMakerNikonISOSetting")]
		NSString MakerNikonISOSetting { get; }
		[Field ("kCGImagePropertyMakerNikonColorMode")]
		NSString MakerNikonColorMode { get; }
		[Field ("kCGImagePropertyMakerNikonQuality")]
		NSString MakerNikonQuality { get; }
		[Field ("kCGImagePropertyMakerNikonWhiteBalanceMode")]
		NSString MakerNikonWhiteBalanceMode { get; }
		[Field ("kCGImagePropertyMakerNikonSharpenMode")]
		NSString MakerNikonSharpenMode { get; }
		[Field ("kCGImagePropertyMakerNikonFocusMode")]
		NSString MakerNikonFocusMode { get; }
		[Field ("kCGImagePropertyMakerNikonFlashSetting")]
		NSString MakerNikonFlashSetting { get; }
		[Field ("kCGImagePropertyMakerNikonISOSelection")]
		NSString MakerNikonISOSelection { get; }
		[Field ("kCGImagePropertyMakerNikonFlashExposureComp")]
		NSString MakerNikonFlashExposureComp { get; }
		[Field ("kCGImagePropertyMakerNikonImageAdjustment")]
		NSString MakerNikonImageAdjustment { get; }
		[Field ("kCGImagePropertyMakerNikonLensAdapter")]
		NSString MakerNikonLensAdapter { get; }
		[Field ("kCGImagePropertyMakerNikonLensType")]
		NSString MakerNikonLensType { get; }
		[Field ("kCGImagePropertyMakerNikonLensInfo")]
		NSString MakerNikonLensInfo { get; }
		[Field ("kCGImagePropertyMakerNikonFocusDistance")]
		NSString MakerNikonFocusDistance { get; }
		[Field ("kCGImagePropertyMakerNikonDigitalZoom")]
		NSString MakerNikonDigitalZoom { get; }
		[Field ("kCGImagePropertyMakerNikonShootingMode")]
		NSString MakerNikonShootingMode { get; }
		[Field ("kCGImagePropertyMakerNikonShutterCount")]
		NSString MakerNikonShutterCount { get; }
		[Field ("kCGImagePropertyMakerNikonCameraSerialNumber")]
		NSString MakerNikonCameraSerialNumber { get; }

		// Canon Camera Dictionary Keys

		[Field ("kCGImagePropertyMakerCanonOwnerName")]
		NSString MakerCanonOwnerName { get; }
		[Field ("kCGImagePropertyMakerCanonCameraSerialNumber")]
		NSString MakerCanonCameraSerialNumber { get; }
		[Field ("kCGImagePropertyMakerCanonImageSerialNumber")]
		NSString MakerCanonImageSerialNumber { get; }
		[Field ("kCGImagePropertyMakerCanonFlashExposureComp")]
		NSString MakerCanonFlashExposureComp { get; }
		[Field ("kCGImagePropertyMakerCanonContinuousDrive")]
		NSString MakerCanonContinuousDrive { get; }
		[Field ("kCGImagePropertyMakerCanonLensModel")]
		NSString MakerCanonLensModel { get; }
		[Field ("kCGImagePropertyMakerCanonFirmware")]
		NSString MakerCanonFirmware { get; }
		[Field ("kCGImagePropertyMakerCanonAspectRatioInfo")]
		NSString MakerCanonAspectRatioInfo { get; }

		[Since(7,0), Mavericks]
		[Field ("kCGImagePropertyExifISOSpeed")]
		NSString ExifISOSpeed { get; }
		[Since(7,0), Mavericks]
		[Field ("kCGImagePropertyExifISOSpeedLatitudeyyy")]
		NSString ExifISOSpeedLatitudeYyy { get; }
		[Since(7,0), Mavericks]
		[Field ("kCGImagePropertyExifISOSpeedLatitudezzz")]
		NSString ExifISOSpeedLatitudeZzz { get; }
		[Since(7,0), Mavericks]
		[Field ("kCGImagePropertyExifRecommendedExposureIndex")]
		NSString ExifRecommendedExposureIndex { get; }
		[Since(7,0), Mavericks]
		[Field ("kCGImagePropertyExifSensitivityType")]
		NSString ExifSensitivityType { get; }
		[Since(7,0), Mavericks]
		[Field ("kCGImagePropertyExifStandardOutputSensitivity")]
		NSString ExifStandardOutputSensitivity { get; }

#if !MONOMAC
		[Since(7,0)]
		[Field ("kCGImagePropertyMakerAppleDictionary")]
		NSString MakerAppleDictionary { get; }
#endif

		[Mac (10, 13), iOS (11,0), TV (11,0), Watch (4,0)]
		[Field ("kCGImagePropertyImageCount")]
		NSString ImageCount { get; }

		[Mac (10, 13), iOS (11,0), TV (11,0), Watch (4,0)]
		[Field ("kCGImagePropertyWidth")]
		NSString Width { get; }

		[Mac (10, 13), iOS (11,0), TV (11,0), Watch (4,0)]
		[Field ("kCGImagePropertyHeight")]
		NSString Height { get; }

		[Mac (10, 13), iOS (11,0), TV (11,0), Watch (4,0)]
		[Field ("kCGImagePropertyBytesPerRow")]
		NSString BytesPerRow { get; }

		[Mac (10, 13), iOS (11,0), TV (11,0), Watch (4,0)]
		[Field ("kCGImagePropertyNamedColorSpace")]
		NSString NamedColorSpace { get; }

		[Mac (10, 13), iOS (11,0), TV (11,0), Watch (4,0)]
		[Field ("kCGImagePropertyPixelFormat")]
		NSString PixelFormat { get; }

		[Mac (10, 13), iOS (11,0), TV (11,0), Watch (4,0)]
		[Field ("kCGImagePropertyImages")]
		NSString Images { get; }

		[Mac (10, 13), iOS (11,0), TV (11,0), Watch (4,0)]
		[Field ("kCGImagePropertyThumbnailImages")]
		NSString ThumbnailImages { get; }

		[Mac (10, 13), iOS (11,0), TV (11,0), Watch (4,0)]
		[Field ("kCGImagePropertyAuxiliaryData")]
		NSString AuxiliaryData { get; }

		[Mac (10, 13), iOS (11,0), TV (11,0), Watch (4,0)]
		[Field ("kCGImagePropertyAuxiliaryDataType")]
		NSString AuxiliaryDataType { get; }

		[Mac (10, 13), iOS (11,0), TV (11,0), Watch (4,0)]
		[Field ("kCGImagePropertyFileContentsDictionary")]
		NSString FileContentsDictionary { get; }
		
		[Mac (10, 9)][iOS (11,0)]
		[Field ("kCGImagePropertyOpenEXRDictionary")]
		NSString OpenExrDictionary { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtAboutCvTerm")]
		NSString IPTCExtAboutCvTerm { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtAboutCvTermCvId")]
		NSString IPTCExtAboutCvTermCvId { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtAboutCvTermId")]
		NSString IPTCExtAboutCvTermId { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtAboutCvTermName")]
		NSString IPTCExtAboutCvTermName { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtAboutCvTermRefinedAbout")]
		NSString IPTCExtAboutCvTermRefinedAbout { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtAddlModelInfo")]
		NSString IPTCExtAddlModelInfo { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtArtworkOrObject")]
		NSString IPTCExtArtworkOrObject { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtArtworkCircaDateCreated")]
		NSString IPTCExtArtworkCircaDateCreated { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtArtworkContentDescription")]
		NSString IPTCExtArtworkContentDescription { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtArtworkContributionDescription")]
		NSString IPTCExtArtworkContributionDescription { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtArtworkCopyrightNotice")]
		NSString IPTCExtArtworkCopyrightNotice { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtArtworkCreator")]
		NSString IPTCExtArtworkCreator { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtArtworkCreatorID")]
		NSString IPTCExtArtworkCreatorId { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtArtworkCopyrightOwnerID")]
		NSString IPTCExtArtworkCopyrightOwnerId { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtArtworkCopyrightOwnerName")]
		NSString IPTCExtArtworkCopyrightOwnerName { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtArtworkLicensorID")]
		NSString IPTCExtArtworkLicensorId { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtArtworkLicensorName")]
		NSString IPTCExtArtworkLicensorName { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtArtworkDateCreated")]
		NSString IPTCExtArtworkDateCreated { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtArtworkPhysicalDescription")]
		NSString IPTCExtArtworkPhysicalDescription { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtArtworkSource")]
		NSString IPTCExtArtworkSource { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtArtworkSourceInventoryNo")]
		NSString IPTCExtArtworkSourceInventoryNo { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtArtworkSourceInvURL")]
		NSString IPTCExtArtworkSourceInvURL { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtArtworkStylePeriod")]
		NSString IPTCExtArtworkStylePeriod { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtArtworkTitle")]
		NSString IPTCExtArtworkTitle { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtAudioBitrate")]
		NSString IPTCExtAudioBitrate { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtAudioBitrateMode")]
		NSString IPTCExtAudioBitrateMode { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtAudioChannelCount")]
		NSString IPTCExtAudioChannelCount { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtCircaDateCreated")]
		NSString IPTCExtCircaDateCreated { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtContainerFormat")]
		NSString IPTCExtContainerFormat { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtContainerFormatIdentifier")]
		NSString IPTCExtContainerFormatIdentifier { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtContainerFormatName")]
		NSString IPTCExtContainerFormatName { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtContributor")]
		NSString IPTCExtContributor { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtContributorIdentifier")]
		NSString IPTCExtContributorIdentifier { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtContributorName")]
		NSString IPTCExtContributorName { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtContributorRole")]
		NSString IPTCExtContributorRole { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtCopyrightYear")]
		NSString IPTCExtCopyrightYear { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtCreator")]
		NSString IPTCExtCreator { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtCreatorIdentifier")]
		NSString IPTCExtCreatorIdentifier { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtCreatorName")]
		NSString IPTCExtCreatorName { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtCreatorRole")]
		NSString IPTCExtCreatorRole { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtControlledVocabularyTerm")]
		NSString IPTCExtControlledVocabularyTerm { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtDataOnScreen")]
		NSString IPTCExtDataOnScreen { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtDataOnScreenRegion")]
		NSString IPTCExtDataOnScreenRegion { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtDataOnScreenRegionD")]
		NSString IPTCExtDataOnScreenRegionD { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtDataOnScreenRegionH")]
		NSString IPTCExtDataOnScreenRegionH { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtDataOnScreenRegionText")]
		NSString IPTCExtDataOnScreenRegionText { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtDataOnScreenRegionUnit")]
		NSString IPTCExtDataOnScreenRegionUnit { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtDataOnScreenRegionW")]
		NSString IPTCExtDataOnScreenRegionW { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtDataOnScreenRegionX")]
		NSString IPTCExtDataOnScreenRegionX { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtDataOnScreenRegionY")]
		NSString IPTCExtDataOnScreenRegionY { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtDigitalImageGUID")]
		NSString IPTCExtDigitalImageGuid { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtDigitalSourceFileType")]
		NSString IPTCExtDigitalSourceFileType { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtDigitalSourceType")]
		NSString IPTCExtDigitalSourceType { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtDopesheet")]
		NSString IPTCExtDopesheet { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtDopesheetLink")]
		NSString IPTCExtDopesheetLink { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtDopesheetLinkLink")]
		NSString IPTCExtDopesheetLinkLink { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtDopesheetLinkLinkQualifier")]
		NSString IPTCExtDopesheetLinkLinkQualifier { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtEmbdEncRightsExpr")]
		NSString IPTCExtEmbdEncRightsExpr { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtEmbeddedEncodedRightsExpr")]
		NSString IPTCExtEmbeddedEncodedRightsExpr { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtEmbeddedEncodedRightsExprType")]
		NSString IPTCExtEmbeddedEncodedRightsExprType { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtEmbeddedEncodedRightsExprLangID")]
		NSString IPTCExtEmbeddedEncodedRightsExprLangId { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtEpisode")]
		NSString IPTCExtEpisode { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtEpisodeIdentifier")]
		NSString IPTCExtEpisodeIdentifier { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtEpisodeName")]
		NSString IPTCExtEpisodeName { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtEpisodeNumber")]
		NSString IPTCExtEpisodeNumber { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtEvent")]
		NSString IPTCExtEvent { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtShownEvent")]
		NSString IPTCExtShownEvent { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtShownEventIdentifier")]
		NSString IPTCExtShownEventIdentifier { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtShownEventName")]
		NSString IPTCExtShownEventName { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtExternalMetadataLink")]
		NSString IPTCExtExternalMetadataLink { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtFeedIdentifier")]
		NSString IPTCExtFeedIdentifier { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtGenre")]
		NSString IPTCExtGenre { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtGenreCvId")]
		NSString IPTCExtGenreCvId { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtGenreCvTermId")]
		NSString IPTCExtGenreCvTermId { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtGenreCvTermName")]
		NSString IPTCExtGenreCvTermName { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtGenreCvTermRefinedAbout")]
		NSString IPTCExtGenreCvTermRefinedAbout { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtHeadline")]
		NSString IPTCExtHeadline { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtIPTCLastEdited")]
		NSString IPTCExtIPTCLastEdited { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtLinkedEncRightsExpr")]
		NSString IPTCExtLinkedEncRightsExpr { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtLinkedEncodedRightsExpr")]
		NSString IPTCExtLinkedEncodedRightsExpr { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtLinkedEncodedRightsExprType")]
		NSString IPTCExtLinkedEncodedRightsExprType { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtLinkedEncodedRightsExprLangID")]
		NSString IPTCExtLinkedEncodedRightsExprLangId { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtLocationCreated")]
		NSString IPTCExtLocationCreated { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtLocationCity")]
		NSString IPTCExtLocationCity { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtLocationCountryCode")]
		NSString IPTCExtLocationCountryCode { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtLocationCountryName")]
		NSString IPTCExtLocationCountryName { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtLocationGPSAltitude")]
		NSString IPTCExtLocationGpsAltitude { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtLocationGPSLatitude")]
		NSString IPTCExtLocationGPSLatitude { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtLocationGPSLongitude")]
		NSString IPTCExtLocationGPSLongitude { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtLocationIdentifier")]
		NSString IPTCExtLocationIdentifier { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtLocationLocationId")]
		NSString IPTCExtLocationLocationId { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtLocationLocationName")]
		NSString IPTCExtLocationLocationName { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtLocationProvinceState")]
		NSString IPTCExtLocationProvinceState { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtLocationSublocation")]
		NSString IPTCExtLocationSublocation { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtLocationWorldRegion")]
		NSString IPTCExtLocationWorldRegion { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtLocationShown")]
		NSString IPTCExtLocationShown { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtMaxAvailHeight")]
		NSString IPTCExtMaxAvailHeight { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtMaxAvailWidth")]
		NSString IPTCExtMaxAvailWidth { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtModelAge")]
		NSString IPTCExtModelAge { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtOrganisationInImageCode")]
		NSString IPTCExtOrganisationInImageCode { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtOrganisationInImageName")]
		NSString IPTCExtOrganisationInImageName { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtPersonHeard")]
		NSString IPTCExtPersonHeard { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtPersonHeardIdentifier")]
		NSString IPTCExtPersonHeardIdentifier { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtPersonHeardName")]
		NSString IPTCExtPersonHeardName { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtPersonInImage")]
		NSString IPTCExtPersonInImage { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtPersonInImageWDetails")]
		NSString IPTCExtPersonInImageWDetails { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtPersonInImageCharacteristic")]
		NSString IPTCExtPersonInImageCharacteristic { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtPersonInImageCvTermCvId")]
		NSString IPTCExtPersonInImageCvTermCvId { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtPersonInImageCvTermId")]
		NSString IPTCExtPersonInImageCvTermId { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtPersonInImageCvTermName")]
		NSString IPTCExtPersonInImageCvTermName { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtPersonInImageCvTermRefinedAbout")]
		NSString IPTCExtPersonInImageCvTermRefinedAbout { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtPersonInImageDescription")]
		NSString IPTCExtPersonInImageDescription { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtPersonInImageId")]
		NSString IPTCExtPersonInImageId { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtPersonInImageName")]
		NSString IPTCExtPersonInImageName { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtProductInImage")]
		NSString IPTCExtProductInImage { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtProductInImageDescription")]
		NSString IPTCExtProductInImageDescription { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtProductInImageGTIN")]
		NSString IPTCExtProductInImageGTIN { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtProductInImageName")]
		NSString IPTCExtProductInImageName { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtPublicationEvent")]
		NSString IPTCExtPublicationEvent { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtPublicationEventDate")]
		NSString IPTCExtPublicationEventDate { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtPublicationEventIdentifier")]
		NSString IPTCExtPublicationEventIdentifier { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtPublicationEventName")]
		NSString IPTCExtPublicationEventName { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtRating")]
		NSString IPTCExtRating { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtRatingRatingRegion")]
		NSString IPTCExtRatingRatingRegion { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtRatingRegionCity")]
		NSString IPTCExtRatingRegionCity { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtRatingRegionCountryCode")]
		NSString IPTCExtRatingRegionCountryCode { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtRatingRegionCountryName")]
		NSString IPTCExtRatingRegionCountryName { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtRatingRegionGPSAltitude")]
		NSString IPTCExtRatingRegionGPSAltitude { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtRatingRegionGPSLatitude")]
		NSString IPTCExtRatingRegionGPSLatitude { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtRatingRegionGPSLongitude")]
		NSString IPTCExtRatingRegionGPSLongitude { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtRatingRegionIdentifier")]
		NSString IPTCExtRatingRegionIdentifier { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtRatingRegionLocationId")]
		NSString IPTCExtRatingRegionLocationId { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtRatingRegionLocationName")]
		NSString IPTCExtRatingRegionLocationName { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtRatingRegionProvinceState")]
		NSString IPTCExtRatingRegionProvinceState { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtRatingRegionSublocation")]
		NSString IPTCExtRatingRegionSublocation { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtRatingRegionWorldRegion")]
		NSString IPTCExtRatingRegionWorldRegion { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtRatingScaleMaxValue")]
		NSString IPTCExtRatingScaleMaxValue { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtRatingScaleMinValue")]
		NSString IPTCExtRatingScaleMinValue { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtRatingSourceLink")]
		NSString IPTCExtRatingSourceLink { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtRatingValue")]
		NSString IPTCExtRatingValue { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtRatingValueLogoLink")]
		NSString IPTCExtRatingValueLogoLink { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtRegistryID")]
		NSString IPTCExtRegistryID { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtRegistryEntryRole")]
		NSString IPTCExtRegistryEntryRole { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtRegistryItemID")]
		NSString IPTCExtRegistryItemID { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtRegistryOrganisationID")]
		NSString IPTCExtRegistryOrganisationID { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtReleaseReady")]
		NSString IPTCExtReleaseReady { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtSeason")]
		NSString IPTCExtSeason { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtSeasonIdentifier")]
		NSString IPTCExtSeasonIdentifier { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtSeasonName")]
		NSString IPTCExtSeasonName { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtSeasonNumber")]
		NSString IPTCExtSeasonNumber { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtSeries")]
		NSString IPTCExtSeries { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtSeriesIdentifier")]
		NSString IPTCExtSeriesIdentifier { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtSeriesName")]
		NSString IPTCExtSeriesName { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtStorylineIdentifier")]
		NSString IPTCExtStorylineIdentifier { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtStreamReady")]
		NSString IPTCExtStreamReady { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtStylePeriod")]
		NSString IPTCExtStylePeriod { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtSupplyChainSource")]
		NSString IPTCExtSupplyChainSource { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtSupplyChainSourceIdentifier")]
		NSString IPTCExtSupplyChainSourceIdentifier { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtSupplyChainSourceName")]
		NSString IPTCExtSupplyChainSourceName { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtTemporalCoverage")]
		NSString IPTCExtTemporalCoverage { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtTemporalCoverageFrom")]
		NSString IPTCExtTemporalCoverageFrom { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtTemporalCoverageTo")]
		NSString IPTCExtTemporalCoverageTo { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtTranscript")]
		NSString IPTCExtTranscript { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtTranscriptLink")]
		NSString IPTCExtTranscriptLink { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtTranscriptLinkLink")]
		NSString IPTCExtTranscriptLinkLink { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtTranscriptLinkLinkQualifier")]
		NSString IPTCExtTranscriptLinkLinkQualifier { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtVideoBitrate")]
		NSString IPTCExtVideoBitrate { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtVideoBitrateMode")]
		NSString IPTCExtVideoBitrateMode { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtVideoDisplayAspectRatio")]
		NSString IPTCExtVideoDisplayAspectRatio { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtVideoEncodingProfile")]
		NSString IPTCExtVideoEncodingProfile { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtVideoShotType")]
		NSString IPTCExtVideoShotType { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtVideoShotTypeIdentifier")]
		NSString IPTCExtVideoShotTypeIdentifier { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtVideoShotTypeName")]
		NSString IPTCExtVideoShotTypeName { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtVideoStreamsCount")]
		NSString IPTCExtVideoStreamsCount { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtVisualColor")]
		NSString IPTCExtVisualColor { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtWorkflowTag")]
		NSString IPTCExtWorkflowTag { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtWorkflowTagCvId")]
		NSString IPTCExtWorkflowTagCvId { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtWorkflowTagCvTermId")]
		NSString IPTCExtWorkflowTagCvTermId { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtWorkflowTagCvTermName")]
		NSString IPTCExtWorkflowTagCvTermName { get; }

		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImagePropertyIPTCExtWorkflowTagCvTermRefinedAbout")]
		NSString IPTCExtWorkflowTagCvTermRefinedAbout { get; }

		[Mac (10, 9)][iOS (11, 3)]
		[Field ("kCGImagePropertyOpenEXRAspectRatio")]
		NSString OpenExrAspectRatio { get; }
	}

	[Since (7,0), MountainLion]
	[Static]
	interface CGImageMetadataTagNamespaces {
		[Field ("kCGImageMetadataNamespaceExif")]
		NSString Exif { get; }
		[Field ("kCGImageMetadataNamespaceExifAux")]
		NSString ExifAux { get; }
		[Mavericks, Field ("kCGImageMetadataNamespaceExifEX")]
		NSString ExifEx { get; }
		[Field ("kCGImageMetadataNamespaceDublinCore")]
		NSString DublinCore { get; }
		[Field ("kCGImageMetadataNamespaceIPTCCore")]
		NSString IPTCCore { get; }
		[Field ("kCGImageMetadataNamespacePhotoshop")]
		NSString Photoshop { get; }
		[Field ("kCGImageMetadataNamespaceTIFF")]
		NSString TIFF { get; }
		[Field ("kCGImageMetadataNamespaceXMPBasic")]
		NSString XMPBasic { get; }
		[Field ("kCGImageMetadataNamespaceXMPRights")]
		NSString XMPRights { get; }
		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImageMetadataNamespaceIPTCExtension")]
		NSString IPTCExtension { get; }
	}

	[Since (7,0), MountainLion]
	[Static]
	interface CGImageMetadataTagPrefixes {
		[Field ("kCGImageMetadataPrefixExif")]
		NSString Exif { get; }
		[Field ("kCGImageMetadataPrefixExifAux")]
		NSString ExifAux { get; }
		[Mavericks, Field ("kCGImageMetadataPrefixExifEX")]
		NSString ExifEx { get; }
		[Field ("kCGImageMetadataPrefixDublinCore")]
		NSString DublinCore { get; }
		[Field ("kCGImageMetadataPrefixIPTCCore")]
		NSString IPTCCore { get; }
		[Field ("kCGImageMetadataPrefixPhotoshop")]
		NSString Photoshop { get; }
		[Field ("kCGImageMetadataPrefixTIFF")]
		NSString TIFF { get; }
		[Field ("kCGImageMetadataPrefixXMPBasic")]
		NSString XMPBasic { get; }
		[Field ("kCGImageMetadataPrefixXMPRights")]
		NSString XMPRights { get; }
		[Mac (10, 13, 4)][iOS (11, 3)]
		[Field ("kCGImageMetadataPrefixIPTCExtension")]
		NSString IPTCExtension { get; }
	}

	[Since (7,0), MountainLion]
	interface CGImageMetadata {
		[Field ("kCFErrorDomainCGImageMetadata")]
		NSString ErrorDomain { get; }
	}

	[Partial]
	interface CGImageOptions {
		[Internal][Field ("kCGImageSourceTypeIdentifierHint")]
		IntPtr kTypeIdentifierHint { get; }

		[Internal][Field ("kCGImageSourceShouldCache")]
		IntPtr kShouldCache { get; }

		[iOS (7,0)]
		[Mac (10,9)]
		[Internal][Field ("kCGImageSourceShouldCacheImmediately")]
		IntPtr kShouldCacheImmediately { get; }

		[Internal][Field ("kCGImageSourceShouldAllowFloat")]
		IntPtr kShouldAllowFloat { get; }
	}

	[Partial]
	interface CGImageThumbnailOptions {
		[Internal][Field ("kCGImageSourceCreateThumbnailFromImageIfAbsent")]
		IntPtr kCreateThumbnailFromImageIfAbsent { get; }

		[Internal][Field ("kCGImageSourceCreateThumbnailFromImageAlways")]
		IntPtr kCreateThumbnailFromImageAlways { get; }

		[Internal][Field ("kCGImageSourceThumbnailMaxPixelSize")]
		IntPtr kThumbnailMaxPixelSize { get; }

		[Internal][Field ("kCGImageSourceCreateThumbnailWithTransform")]
		IntPtr kCreateThumbnailWithTransform { get; }

		[iOS (9,0)][Mac (10,11)]
		[Internal][Field ("kCGImageSourceSubsampleFactor")]
		IntPtr kCGImageSourceSubsampleFactor { get; }
	}

	[Partial]
	interface CGImageMetadataEnumerateOptions {
		[Internal][Field ("kCGImageMetadataEnumerateRecursively")]
		IntPtr kCGImageMetadataEnumerateRecursively { get; }
	}

#if !XAMCORE_2_0
	[Partial]
	interface CGImageDestinationOptions {
		[Internal][Field ("kCGImageDestinationLossyCompressionQuality")]
		IntPtr kLossyCompressionQuality { get; }

		[Internal][Field ("kCGImageDestinationBackgroundColor")]
		IntPtr kBackgroundColor { get; }

		[Since (7,0), MountainLion]
		[Internal][Field ("kCGImageDestinationDateTime")]
		IntPtr kDateTime { get; }

		[Since (7,0), MountainLion]
		[Internal][Field ("kCGImageDestinationMergeMetadata")]
		IntPtr kMergeMetadata { get; }

		[Since (7,0), MountainLion]
		[Internal][Field ("kCGImageDestinationMetadata")]
		IntPtr kMetadata { get; }

		[Since (7,0), MountainLion]
		[Internal][Field ("kCGImageDestinationOrientation")]
		IntPtr kOrientation { get; }

		[Since (7,0), MountainLion]
		[Internal][Field ("kCGImageMetadataShouldExcludeXMP")]
		IntPtr kShouldExcludeXMP { get; }

		[iOS (8,0)][Mac (10,10)]
		[Internal][Field ("kCGImageDestinationImageMaxPixelSize")]
		IntPtr kImageMaxPixelSize { get; }

		[iOS (8,0)][Mac (10,10)]
		[Internal][Field ("kCGImageDestinationEmbedThumbnail")]
		IntPtr kEmbedThumbnail { get; }

		[iOS (8,0)][Mac (10,10)]
		[Internal][Field ("kCGImageMetadataShouldExcludeGPS")]
		IntPtr kShouldExcludeGPS { get; }

		[iOS (9,3)][Mac (10,12)]
		[TV (9,2)]
		[Internal][Field ("kCGImageDestinationOptimizeColorForSharing")]
		IntPtr kOptimizeColorForSharing { get; }
	}
#else

	// Defined in CGImageProperties.cs in CoreGraphics
	interface CGImagePropertiesTiff { }
	interface CGImagePropertiesExif { }
	interface CGImagePropertiesJfif { }
	interface CGImagePropertiesPng { }
	interface CGImagePropertiesGps { }
	interface CGImagePropertiesIptc { }

	[StrongDictionary ("CGImageDestinationOptionsKeys")]
	interface CGImageDestinationOptions {

		[Export ("LossyCompressionQuality")]
		float LossyCompressionQuality { get; set; }

		[iOS (8,0)][Mac (10,10)]
		[Export ("ImageMaxPixelSize")]
		int ImageMaxPixelSize { get; set; }

		[iOS (8,0)][Mac (10,10)]
		[Export ("EmbedThumbnail")]
		bool EmbedThumbnail { get; set; }

		[iOS (9,3)][Mac (10,12)]
		[Export ("OptimizeColorForSharing")]
		bool OptimizeColorForSharing { get; set; }

		[StrongDictionary]
		[Export ("TIFFDictionary")]
		CGImagePropertiesTiff TiffDictionary { get; set; }

		[Export ("GIFDictionary")]
		NSDictionary GifDictionary { get; set; }

		[StrongDictionary]
		[Export ("JFIFDictionary")]
		CGImagePropertiesJfif JfifDictionary { get; set; }

		[StrongDictionary]
		[Export ("ExifDictionary")]
		CGImagePropertiesExif ExifDictionary { get; set; }

		[StrongDictionary]
		[Export ("PNGDictionary")]
		CGImagePropertiesPng PngDictionary { get; set; }

		[StrongDictionary]
		[Export ("IPTCDictionary")]
		CGImagePropertiesIptc IptcDictionary { get; set; }

		[StrongDictionary]
		[Export ("GPSDictionary")]
		CGImagePropertiesGps GpsDictionary { get; set; }

		[Export ("RawDictionary")]
		NSDictionary RawDictionary { get; set; }

		[Export ("CIFFDictionary")]
		NSDictionary CiffDictionary { get; set; }

		[Export ("EightBIMDictionary")]
		NSDictionary EightBimDictionary { get; set; }

		[Export ("DNGDictionary")]
		NSDictionary DngDictionary { get; set; }

		[Export ("ExifAuxDictionary")]
		NSDictionary ExifAuxDictionary { get; set; }
	}

	[Static]
	interface CGImageDestinationOptionsKeys {

		[Field ("kCGImageDestinationLossyCompressionQuality")]
		NSString LossyCompressionQuality { get; }

		[Field ("kCGImageDestinationBackgroundColor")]
		NSString BackgroundColor { get; }

		[iOS (8,0)][Mac (10,10)]
		[Field ("kCGImageDestinationImageMaxPixelSize")]
		NSString ImageMaxPixelSize { get; }

		[iOS (8,0)][Mac (10,10)]
		[Field ("kCGImageDestinationEmbedThumbnail")]
		NSString EmbedThumbnail { get; }

		[iOS (9,3)][Mac (10,12)]
		[TV (9,2)]
		[Field ("kCGImageDestinationOptimizeColorForSharing")]
		NSString OptimizeColorForSharing { get; }

		// [Field ("kCGImagePropertyTIFFDictionary")]
		[Static][Wrap ("CGImageProperties.TIFFDictionary")]
		NSString TIFFDictionary { get; }

		// [Field ("kCGImagePropertyGIFDictionary")]
		[Static][Wrap ("CGImageProperties.GIFDictionary")]
		NSString GIFDictionary { get; }

		// [Field ("kCGImagePropertyJFIFDictionary")]
		[Static][Wrap ("CGImageProperties.JFIFDictionary")]
		NSString JFIFDictionary { get; }

		// [Field ("kCGImagePropertyExifDictionary")]
		[Static][Wrap ("CGImageProperties.ExifDictionary")]
		NSString ExifDictionary { get; }

		// [Field ("kCGImagePropertyPNGDictionary")]
		[Static][Wrap ("CGImageProperties.PNGDictionary")]
		NSString PNGDictionary { get; }

		// [Field ("kCGImagePropertyIPTCDictionary")]
		[Static][Wrap ("CGImageProperties.IPTCDictionary")]
		NSString IPTCDictionary { get; }

		// [Field ("kCGImagePropertyGPSDictionary")]
		[Static][Wrap ("CGImageProperties.GPSDictionary")]
		NSString GPSDictionary { get; }

		// [Field ("kCGImagePropertyRawDictionary")]
		[Static][Wrap ("CGImageProperties.RawDictionary")]
		NSString RawDictionary { get; }

		// [Field ("kCGImagePropertyCIFFDictionary")]
		[Static][Wrap ("CGImageProperties.CIFFDictionary")]
		NSString CIFFDictionary { get; }

		// [Field ("kCGImageProperty8BIMDictionary")]
		[Static][Wrap ("CGImageProperties.EightBIMDictionary")]
		NSString EightBIMDictionary { get; }

		// [Field ("kCGImagePropertyDNGDictionary")]
		[Static][Wrap ("CGImageProperties.DNGDictionary")]
		NSString DNGDictionary { get; }

		// [Field ("kCGImagePropertyExifAuxDictionary")]
		[Static][Wrap ("CGImageProperties.ExifAuxDictionary")]
		NSString ExifAuxDictionary { get; }
	}

	[Partial]
	interface CGCopyImageSourceOptions {

		[Since (7,0), MountainLion]
		[Internal][Field ("kCGImageDestinationMetadata")]
		IntPtr kMetadata { get; }

		[Since (7,0), MountainLion]
		[Internal][Field ("kCGImageDestinationMergeMetadata")]
		IntPtr kMergeMetadata { get; }

		[Since (7,0), MountainLion]
		[Internal][Field ("kCGImageMetadataShouldExcludeXMP")]
		IntPtr kShouldExcludeXMP { get; }

		[iOS (8,0)][Mac (10,10)]
		[Internal][Field ("kCGImageMetadataShouldExcludeGPS")]
		IntPtr kShouldExcludeGPS { get; }

		[Since (7,0), MountainLion]
		[Internal][Field ("kCGImageDestinationDateTime")]
		IntPtr kDateTime { get; }

		[Since (7,0), MountainLion]
		[Internal][Field ("kCGImageDestinationOrientation")]
		IntPtr kOrientation { get; }
	}
#endif

	[Mac (10, 13), iOS (11,0), TV (11,0), Watch (4,0)]
	enum CGImageAuxiliaryDataType {
		[Field ("kCGImageAuxiliaryDataTypeDepth")]
		Depth,

		[Field ("kCGImageAuxiliaryDataTypeDisparity")]
		Disparity,
	}

	[Mac (10,13), iOS (11,0), TV (11,0), Watch (4,0)]
	[Static]
	[Internal]
	interface CGImageAuxiliaryDataInfoKeys {
		[Field ("kCGImageAuxiliaryDataInfoData")]
		NSString DataKey { get; }

		[Field ("kCGImageAuxiliaryDataInfoDataDescription")]
		NSString DataDescriptionKey { get; }

		[Field ("kCGImageAuxiliaryDataInfoMetadata")]
		NSString MetadataKey { get; }
	}

	[Mac (10,13), iOS (11,0), TV (11,0), Watch (4,0)]
	[StrongDictionary ("CGImageAuxiliaryDataInfoKeys")]
	interface CGImageAuxiliaryDataInfo {

		NSData Data { get; set; }
		NSDictionary DataDescription { get; set; }
	}
}
