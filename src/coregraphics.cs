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

		[Internal][Field ("kCGPDFContextMediaBox")]
		IntPtr kCGPDFContextMediaBox { get; }

		[Internal][Field ("kCGPDFContextCropBox")]
		IntPtr kCGPDFContextCropBox { get; }

		[Internal][Field ("kCGPDFContextBleedBox")]
		IntPtr kCGPDFContextBleedBox { get; }

		[Internal][Field ("kCGPDFContextTrimBox")]
		IntPtr kCGPDFContextTrimBox { get; }

		[Internal][Field ("kCGPDFContextArtBox")]
		IntPtr kCGPDFContextArtBox { get; }
	}

	[Partial]
	interface CGPDFInfo {

		[Internal][Field ("kCGPDFContextTitle")]
		IntPtr kCGPDFContextTitle { get; }

		[Internal][Field ("kCGPDFContextAuthor")]
		IntPtr kCGPDFContextAuthor { get; }

		[Internal][Field ("kCGPDFContextSubject")]
		IntPtr kCGPDFContextSubject { get; }

		[Internal][Field ("kCGPDFContextKeywords")]
		IntPtr kCGPDFContextKeywords { get; }

		[Internal][Field ("kCGPDFContextCreator")]
		IntPtr kCGPDFContextCreator { get; }

		[Internal][Field ("kCGPDFContextOwnerPassword")]
		IntPtr kCGPDFContextOwnerPassword { get; }

		[Internal][Field ("kCGPDFContextUserPassword")]
		IntPtr kCGPDFContextUserPassword { get; }

		[Internal][Field ("kCGPDFContextEncryptionKeyLength")]
		IntPtr kCGPDFContextEncryptionKeyLength { get; }

		[Internal][Field ("kCGPDFContextAllowsPrinting")]
		IntPtr kCGPDFContextAllowsPrinting { get; }

		[Internal][Field ("kCGPDFContextAllowsCopying")]
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

		[Mac (10,13)][iOS (11,0)][TV (11,0)][Watch (4,0)]
		[Internal][Field ("kCGPDFContextAccessPermissions")]
		IntPtr kCGPDFContextAccessPermissions { get; }
	}

	[Static]
	[iOS (9,0)]
	interface CGColorSpaceNames {
		[Field ("kCGColorSpaceGenericGray")]
		NSString GenericGray { get; }

		[Field ("kCGColorSpaceGenericRGB")]
		NSString GenericRgb { get; }

		[Field ("kCGColorSpaceGenericCMYK")]
		NSString GenericCmyk { get; }

		[iOS (9,3)][Mac(10,11,2)]
		[TV (9,2)]
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

		[Mac (10,11)]
		[Field ("kCGColorSpaceGenericXYZ")]
		NSString GenericXyz { get; }

		[Mac (10,11)]
		[Field ("kCGColorSpaceACESCGLinear")]
		NSString AcesCGLinear { get; }

		[Mac (10,11)]
		[Field ("kCGColorSpaceITUR_709")]
		NSString ItuR_709 { get; }

		[Mac (10,11)]
		[Field ("kCGColorSpaceITUR_2020")]
		NSString ItuR_2020 { get; }

		[iOS (9,3)][Mac (10,11)]
		[TV (9,2)]
		[Field ("kCGColorSpaceROMMRGB")]
		NSString RommRgb { get; }

		[iOS (9,3)][Mac (10,11)]
		[TV (9,2)]
		[Field ("kCGColorSpaceDCIP3")]
		NSString Dcip3 { get; }

		[iOS (10,0)][Mac (10,12)]
		[Watch (3,0)]
		[TV (10,0)]
		[Field ("kCGColorSpaceExtendedSRGB")]
		NSString ExtendedSrgb { get; }

		[iOS (10,0)][Mac (10,12)]
		[Watch (3,0)]
		[TV (10,0)]
		[Field ("kCGColorSpaceLinearSRGB")]
		NSString LinearSrgb { get; }

		[iOS (10,0)][Mac (10,12)]
		[Watch (3,0)]
		[TV (10,0)]
		[Field ("kCGColorSpaceExtendedLinearSRGB")]
		NSString ExtendedLinearSrgb { get; }

		[iOS (10,0)][Mac (10,12)]
		[Watch (3,0)]
		[TV (10,0)]
		[Field ("kCGColorSpaceExtendedGray")]
		NSString ExtendedGray { get; }

		[iOS (10,0)][Mac (10,12)]
		[Watch (3,0)]
		[TV (10,0)]
		[Field ("kCGColorSpaceLinearGray")]
		NSString LinearGray { get; }

		[iOS (10,0)][Mac (10,12)]
		[Watch (3,0)]
		[TV (10,0)]
		[Field ("kCGColorSpaceExtendedLinearGray")]
		NSString ExtendedLinearGray { get; }

#if MONOMAC
		[Obsolete ("Now accessible as GenericCmyk")]
		[Field ("kCGColorSpaceGenericCMYK")]
		NSString GenericCMYK { get; }

		[Obsolete ("Now accessible as AdobeRgb1998")]
		[Field ("kCGColorSpaceAdobeRGB1998")]
		NSString AdobeRGB1998 { get; }

		[Obsolete ("Now accessible as Srgb")]
		[Field ("kCGColorSpaceSRGB")]
		NSString SRGB { get; }

		[Obsolete ("Now accessible as GenericRgb")]
		[Field ("kCGColorSpaceGenericRGB")]
		NSString GenericRGB { get; }

		[Obsolete ("Now accessible as GenericRgb")]
		[Field ("kCGColorSpaceGenericRGBLinear")]
		NSString GenericRGBLinear { get; }
#endif

		[iOS (11,0)][Mac (10,13)][Watch (4,0)][TV (11,0)]
		[Field ("kCGColorSpaceGenericLab")]
		NSString GenericLab { get; }
	}

	[Partial]
	partial interface CGColorConversionInfo {

		[Internal]
		[Field ("kCGColorConversionBlackPointCompensation")]
		NSString BlackPointCompensationKey { get; }

#if XAMCORE_2_0
		[Internal]
		[Field ("kCGColorConversionTRCSize")]
		[iOS (11,0), Mac(10,13), TV(11,0), Watch(4,0)]
		NSString TrcSizeKey { get; }
#endif
	}

	[iOS (10,0)][TV (10,0)][Watch (3,0)][Mac (10,12)]
	[StrongDictionary ("CGColorConversionInfo")]
	interface CGColorConversionOptions {
		bool BlackPointCompensation { get; set; }
#if XAMCORE_2_0
		CGSize TrcSize { get; set; }
#endif
	}

	[iOS(11,0), Mac(10,13)]
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
		NSString OutlineDestinationKey { get;}

		[Internal]
		[Field ("kCGPDFOutlineDestinationRect")]
		NSString DestinationRectKey { get; }

		[Internal]
		[Field ("kCGPDFContextAccessPermissions")]
		NSString AccessPermissionsKey { get; }
	}
	
	[iOS(11,0), Mac(10,13)]
	[StrongDictionary ("CGPDFOutlineKeys")]
	interface CGPDFOutlineOptions {
		string OutlineTitle { get; set; }
		NSDictionary [] OutlineChildren { get; set; }
		NSObject OutlineDestination { get; set; }
#if XAMCORE_2_0
		CGRect DestinationRect { get; set; }
#endif
	}
}
