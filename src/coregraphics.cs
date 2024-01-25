//
// coregraphics.cs: Definitions for CoreGraphics
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

using System;

using Foundation;

using ObjCRuntime;

namespace CoreGraphics {

	[Partial]
	interface CGPDFPageInfo {

		[Internal]
		[Field ("kCGPDFContextMediaBox")]
		IntPtr kCGPDFContextMediaBox { get; }

		[Internal]
		[Field ("kCGPDFContextCropBox")]
		IntPtr kCGPDFContextCropBox { get; }

		[Internal]
		[Field ("kCGPDFContextBleedBox")]
		IntPtr kCGPDFContextBleedBox { get; }

		[Internal]
		[Field ("kCGPDFContextTrimBox")]
		IntPtr kCGPDFContextTrimBox { get; }

		[Internal]
		[Field ("kCGPDFContextArtBox")]
		IntPtr kCGPDFContextArtBox { get; }
	}

	[Partial]
	interface CGPDFInfo {

		[Internal]
		[Field ("kCGPDFContextTitle")]
		IntPtr kCGPDFContextTitle { get; }

		[Internal]
		[Field ("kCGPDFContextAuthor")]
		IntPtr kCGPDFContextAuthor { get; }

		[Internal]
		[Field ("kCGPDFContextSubject")]
		IntPtr kCGPDFContextSubject { get; }

		[Internal]
		[Field ("kCGPDFContextKeywords")]
		IntPtr kCGPDFContextKeywords { get; }

		[Internal]
		[Field ("kCGPDFContextCreator")]
		IntPtr kCGPDFContextCreator { get; }

		[Internal]
		[Field ("kCGPDFContextOwnerPassword")]
		IntPtr kCGPDFContextOwnerPassword { get; }

		[Internal]
		[Field ("kCGPDFContextUserPassword")]
		IntPtr kCGPDFContextUserPassword { get; }

		[Internal]
		[Field ("kCGPDFContextEncryptionKeyLength")]
		IntPtr kCGPDFContextEncryptionKeyLength { get; }

		[Internal]
		[Field ("kCGPDFContextAllowsPrinting")]
		IntPtr kCGPDFContextAllowsPrinting { get; }

		[Internal]
		[Field ("kCGPDFContextAllowsCopying")]
		IntPtr kCGPDFContextAllowsCopying { get; }

#if false
		kCGPDFContextOutputIntent;
		kCGPDFXOutputIntentSubtype;
		kCGPDFXOutputConditionIdentifier;
		kCGPDFXOutputCondition;
		kCGPDFXRegistryName;
		kCGPDFXInfo;
		kCGPDFXDestinationOutputProfile;
		kCGPDFContextOutputIntents;
#endif

		[MacCatalyst (13, 1)]
		[Internal]
		[Field ("kCGPDFContextAccessPermissions")]
		IntPtr kCGPDFContextAccessPermissions { get; }

		[Mac (11, 0)]
		[iOS (14, 0)]
		[TV (14, 0)]
		[Watch (7, 0)]
		[MacCatalyst (14, 0)]
		[Internal]
		[Field ("kCGPDFContextCreateLinearizedPDF")]
		IntPtr kCGPDFContextCreateLinearizedPDF { get; }

		[Mac (11, 0)]
		[iOS (14, 0)]
		[TV (14, 0)]
		[Watch (7, 0)]
		[MacCatalyst (14, 0)]
		[Internal]
		[Field ("kCGPDFContextCreatePDFA")]
		IntPtr kCGPDFContextCreatePDFA { get; }
	}

	[Static]
	[MacCatalyst (13, 1)]
	interface CGColorSpaceNames {
		[Field ("kCGColorSpaceGenericGray")]
		NSString GenericGray { get; }

		[Field ("kCGColorSpaceGenericRGB")]
		NSString GenericRgb { get; }

		[Field ("kCGColorSpaceGenericCMYK")]
		NSString GenericCmyk { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCGColorSpaceDisplayP3")]
		NSString DisplayP3 { get; }

		[Field ("kCGColorSpaceGenericRGBLinear")]
		NSString GenericRgbLinear { get; }

		[Field ("kCGColorSpaceAdobeRGB1998")]
		NSString AdobeRgb1998 { get; }

		[Field ("kCGColorSpaceSRGB")]
		NSString Srgb { get; }

		[Field ("kCGColorSpaceGenericGrayGamma2_2")]
		NSString GenericGrayGamma2_2 { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCGColorSpaceGenericXYZ")]
		NSString GenericXyz { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCGColorSpaceACESCGLinear")]
		NSString AcesCGLinear { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCGColorSpaceITUR_709")]
		NSString ItuR_709 { get; }

		[Mac (12, 1), iOS (15, 2), TV (15, 2), Watch (8, 3)]
		[MacCatalyst (15, 2)]
		[Field ("kCGColorSpaceITUR_709_PQ")]
		NSString ItuR_709_PQ { get; }

		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0), Watch (9, 0)]
		[Field ("kCGColorSpaceITUR_709_HLG")]
		NSString ItuR_709_Hlg { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCGColorSpaceITUR_2020")]
		NSString ItuR_2020 { get; }

		[Mac (12, 1), iOS (15, 2), TV (15, 2), Watch (8, 3)]
		[MacCatalyst (15, 2)]
		[Field ("kCGColorSpaceITUR_2020_sRGBGamma")]
		NSString ItuR_2020_sRgbGamma { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCGColorSpaceROMMRGB")]
		NSString RommRgb { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCGColorSpaceDCIP3")]
		NSString Dcip3 { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCGColorSpaceExtendedSRGB")]
		NSString ExtendedSrgb { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCGColorSpaceLinearSRGB")]
		NSString LinearSrgb { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCGColorSpaceExtendedLinearSRGB")]
		NSString ExtendedLinearSrgb { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCGColorSpaceExtendedGray")]
		NSString ExtendedGray { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCGColorSpaceLinearGray")]
		NSString LinearGray { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCGColorSpaceExtendedLinearGray")]
		NSString ExtendedLinearGray { get; }

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Obsolete ("Now accessible as GenericCmyk.")]
		[Field ("kCGColorSpaceGenericCMYK")]
		NSString GenericCMYK { get; }

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Obsolete ("Now accessible as AdobeRgb1998.")]
		[Field ("kCGColorSpaceAdobeRGB1998")]
		NSString AdobeRGB1998 { get; }

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Obsolete ("Now accessible as Srgb.")]
		[Field ("kCGColorSpaceSRGB")]
		NSString SRGB { get; }

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Obsolete ("Now accessible as GenericRgb.")]
		[Field ("kCGColorSpaceGenericRGB")]
		NSString GenericRGB { get; }

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Obsolete ("Now accessible as GenericRgb.")]
		[Field ("kCGColorSpaceGenericRGBLinear")]
		NSString GenericRGBLinear { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCGColorSpaceGenericLab")]
		NSString GenericLab { get; }

		[iOS (12, 3)]
		[TV (12, 3)]
		[Watch (5, 3)]
		[MacCatalyst (13, 1)]
		[Field ("kCGColorSpaceExtendedLinearITUR_2020")]
		NSString ExtendedLinearItur_2020 { get; }

		[iOS (14, 1), Mac (11, 0), TV (14, 2), Watch (7, 1)]
		[MacCatalyst (14, 1)]
		[Field ("kCGColorSpaceExtendedITUR_2020")]
		NSString ExtendedItur_2020 { get; }

		[iOS (12, 3)]
		[TV (12, 3)]
		[Watch (5, 3)]
		[MacCatalyst (13, 1)]
		[Field ("kCGColorSpaceExtendedLinearDisplayP3")]
		NSString ExtendedLinearDisplayP3 { get; }

		[iOS (14, 1), Mac (11, 0), TV (14, 2), Watch (7, 1)]
		[MacCatalyst (14, 1)]
		[Field ("kCGColorSpaceExtendedDisplayP3")]
		NSString ExtendedDisplayP3 { get; }

		[iOS (12, 0)]
		[TV (12, 0)]
		[Watch (5, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 15, 4, message: "Use 'Itur_2100_PQ' instead.")]
		[Deprecated (PlatformName.iOS, 13, 4, message: "Use 'Itur_2100_PQ' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 4, message: "Use 'Itur_2100_PQ' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 2, message: "Use 'Itur_2100_PQ' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Itur_2100_PQ' instead.")]
		[Field ("kCGColorSpaceITUR_2020_PQ_EOTF")]
		NSString Itur_2020_PQ_Eotf { get; }

		[iOS (13, 4), TV (13, 4), Watch (6, 2)]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'Itur_2100_PQ' instead.")]
		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'Itur_2100_PQ' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'Itur_2100_PQ' instead.")]
		[Deprecated (PlatformName.WatchOS, 7, 0, message: "Use 'Itur_2100_PQ' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'Itur_2100_PQ' instead.")]
		[Field ("kCGColorSpaceITUR_2020_PQ")]
		NSString Itur_2020_PQ { get; }

		[iOS (13, 0)]
		[TV (13, 0)]
		[Watch (6, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 15, 4)]
		[Deprecated (PlatformName.iOS, 13, 4)]
		[Deprecated (PlatformName.TvOS, 13, 4)]
		[Deprecated (PlatformName.WatchOS, 6, 2)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Field ("kCGColorSpaceDisplayP3_PQ_EOTF")]
		NSString DisplayP3_PQ_Eotf { get; }

		[iOS (13, 4), TV (13, 4), Watch (6, 2)]
		[MacCatalyst (13, 1)]
		[Field ("kCGColorSpaceDisplayP3_PQ")]
		NSString DisplayP3_PQ { get; }

		[iOS (13, 0)]
		[TV (13, 0)]
		[Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Field ("kCGColorSpaceDisplayP3_HLG")]
		NSString DisplayP3_Hlg { get; }

		[iOS (13, 0)]
		[TV (13, 0)]
		[Watch (6, 0)]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'Itur_2100_PQ' instead.")]
		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'Itur_2100_PQ' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'Itur_2100_PQ' instead.")]
		[Deprecated (PlatformName.WatchOS, 7, 0, message: "Use 'Itur_2100_PQ' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'Itur_2100_PQ' instead.")]
		[Field ("kCGColorSpaceITUR_2020_HLG")]
		NSString Itur_2020_Hlg { get; }

		[iOS (14, 0)]
		[TV (14, 0)]
		[Watch (7, 0)]
		[Mac (11, 0)]
		[MacCatalyst (14, 0)]
		[Field ("kCGColorSpaceITUR_2100_HLG")]
		NSString Itur_2100_Hlg { get; }

		[iOS (14, 0)]
		[TV (14, 0)]
		[Watch (7, 0)]
		[Mac (11, 0)]
		[MacCatalyst (14, 0)]
		[Field ("kCGColorSpaceITUR_2100_PQ")]
		NSString Itur_2100_PQ { get; }

		[Mac (12, 0), iOS (15, 0), TV (15, 0), Watch (8, 0), MacCatalyst (15, 0)]
		[Field ("kCGColorSpaceExtendedRange")]
		NSString ExtendedRange { get; }

		[Mac (12, 0), iOS (15, 0), TV (15, 0), Watch (8, 0), MacCatalyst (15, 0)]
		[Field ("kCGColorSpaceLinearDisplayP3")]
		NSString LinearDisplayP3 { get; }

		[Mac (12, 0), iOS (15, 0), TV (15, 0), Watch (8, 0), MacCatalyst (15, 0)]
		[Field ("kCGColorSpaceLinearITUR_2020")]
		NSString LinearItur_2020 { get; }
	}

	[Partial]
	partial interface CGColorConversionInfo {

		[Internal]
		[Field ("kCGColorConversionBlackPointCompensation")]
		NSString BlackPointCompensationKey { get; }

		[Internal]
		[Field ("kCGColorConversionTRCSize")]
		[MacCatalyst (13, 1)]
		NSString TrcSizeKey { get; }
	}

	[MacCatalyst (13, 1)]
	[StrongDictionary ("CGColorConversionInfo")]
	interface CGColorConversionOptions {
		bool BlackPointCompensation { get; set; }
		CGSize TrcSize { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Static]
	[Internal]
	public interface CGPDFOutlineKeys {
		[Internal]
		[Field ("kCGPDFOutlineTitle")]
		NSString OutlineTitleKey { get; }

		[Internal]
		[Field ("kCGPDFOutlineChildren")]
		NSString OutlineChildrenKey { get; }

		[Internal]
		[Field ("kCGPDFOutlineDestination")]
		NSString OutlineDestinationKey { get; }

		[Internal]
		[Field ("kCGPDFOutlineDestinationRect")]
		NSString DestinationRectKey { get; }

		[Internal]
		[Field ("kCGPDFContextAccessPermissions")]
		NSString AccessPermissionsKey { get; }
	}

	[MacCatalyst (13, 1)]
	[StrongDictionary ("CGPDFOutlineKeys")]
	interface CGPDFOutlineOptions {
		string OutlineTitle { get; set; }
		NSDictionary [] OutlineChildren { get; set; }
		NSObject OutlineDestination { get; set; }
		CGRect DestinationRect { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[Static]
	[Internal]
	interface CGPdfTagPropertyKeys {
		[Field ("kCGPDFTagPropertyActualText")]
		NSString ActualTextKey { get; }

		[Field ("kCGPDFTagPropertyAlternativeText")]
		NSString AlternativeTextKey { get; }

		[Field ("kCGPDFTagPropertyTitleText")]
		NSString TitleTextKey { get; }

		[Field ("kCGPDFTagPropertyLanguageText")]
		NSString LanguageTextKey { get; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[StrongDictionary ("CGPdfTagPropertyKeys")]
	interface CGPdfTagProperties {
		// <quote>The following CGPDFTagProperty keys are to be paired with CFStringRef values</quote>
		string ActualText { get; set; }
		string AlternativeText { get; set; }
		string TitleText { get; set; }
		string LanguageText { get; set; }
	}

	// macOS 10.5
	[iOS (14, 0)]
	[TV (14, 0)]
	[Watch (7, 0)]
	[MacCatalyst (14, 0)]
	enum CGConstantColor {
		[Field ("kCGColorWhite")]
		White,
		[Field ("kCGColorBlack")]
		Black,
		[Field ("kCGColorClear")]
		Clear,
	}

	// Adding suffix *Keys to avoid possible name clash
	[NoiOS, NoTV, NoWatch, MacCatalyst (13, 1)]
	[Static]
	interface CGDisplayStreamKeys {

		[Field ("kCGDisplayStreamColorSpace")]
		NSString ColorSpace { get; }

		[Field ("kCGDisplayStreamDestinationRect")]
		NSString DestinationRect { get; }

		[Field ("kCGDisplayStreamMinimumFrameTime")]
		NSString MinimumFrameTime { get; }

		[Field ("kCGDisplayStreamPreserveAspectRatio")]
		NSString PreserveAspectRatio { get; }

		[Field ("kCGDisplayStreamQueueDepth")]
		NSString QueueDepth { get; }

		[Field ("kCGDisplayStreamShowCursor")]
		NSString ShowCursor { get; }

		[Field ("kCGDisplayStreamSourceRect")]
		NSString SourceRect { get; }

		[Field ("kCGDisplayStreamYCbCrMatrix")]
		NSString YCbCrMatrix { get; }
	}

	[NoiOS, NoTV, NoWatch, MacCatalyst (13, 1)]
	[Static]
	interface CGDisplayStreamYCbCrMatrixOptionKeys {

		[Field ("kCGDisplayStreamYCbCrMatrix_ITU_R_601_4")]
		NSString Itu_R_601_4 { get; }

		[Field ("kCGDisplayStreamYCbCrMatrix_ITU_R_709_2")]
		NSString Itu_R_709_2 { get; }

		[Field ("kCGDisplayStreamYCbCrMatrix_SMPTE_240M_1995")]
		NSString Smpte_240M_1995 { get; }
	}
}
