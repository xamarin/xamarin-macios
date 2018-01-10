using ObjCRuntime;

namespace Foundation {

	// Utility enum, ObjC uses NSString
	[NoMac]
	[iOS (7,0)]
	public enum NSDocumentType {
		Unknown = -1,
		PlainText,
		RTF,
		RTFD,
		HTML
	}

	// Utility enum, ObjC uses NSString
	[NoMac]
	[iOS (7,0)]
	public enum NSDocumentViewMode {
		Normal,
		PageLayout
			
	}

	public enum NSRunLoopMode {

		[DefaultEnumValue]
		[Field ("NSDefaultRunLoopMode")]
		Default,

		[Field ("NSRunLoopCommonModes")]
		Common,

#if MONOMAC
		[Field ("NSConnectionReplyMode")]
		ConnectionReply = 2,

		[Field ("NSModalPanelRunLoopMode", "AppKit")]
		ModalPanel,

		[Field ("NSEventTrackingRunLoopMode", "AppKit")]
		EventTracking,
#elif !WATCH
		// iOS-specific Enums start in 100 to avoid conflicting with future extensions to MonoMac
		[Field ("UITrackingRunLoopMode", "UIKit")]
		UITracking = 100,
#endif
		// If it is not part of these enumerations
		[Field (null)]
		Other = 1000
	}

	[Mac (10,9)]
	[iOS (7,0)]
	public enum NSItemDownloadingStatus {
		[Field (null)]
		Unknown = -1,

		[Field ("NSMetadataUbiquitousItemDownloadingStatusCurrent")]
		Current,

		[Field ("NSMetadataUbiquitousItemDownloadingStatusDownloaded")]
		Downloaded,

		[Field ("NSMetadataUbiquitousItemDownloadingStatusNotDownloaded")]
		NotDownloaded,
	}

	[iOS (9,0)][Mac (10,11)]
	public enum NSStringTransform {
		[Field ("NSStringTransformLatinToKatakana")]
		LatinToKatakana,

		[Field ("NSStringTransformLatinToHiragana")]
		LatinToHiragana,

		[Field ("NSStringTransformLatinToHangul")]
		LatinToHangul,

		[Field ("NSStringTransformLatinToArabic")]
		LatinToArabic,

		[Field ("NSStringTransformLatinToHebrew")]
		LatinToHebrew,

		[Field ("NSStringTransformLatinToThai")]
		LatinToThai,

		[Field ("NSStringTransformLatinToCyrillic")]
		LatinToCyrillic,

		[Field ("NSStringTransformLatinToGreek")]
		LatinToGreek,

		[Field ("NSStringTransformToLatin")]
		ToLatin,

		[Field ("NSStringTransformMandarinToLatin")]
		MandarinToLatin,

		[Field ("NSStringTransformHiraganaToKatakana")]
		HiraganaToKatakana,

		[Field ("NSStringTransformFullwidthToHalfwidth")]
		FullwidthToHalfwidth,

		[Field ("NSStringTransformToXMLHex")]
		ToXmlHex,

		[Field ("NSStringTransformToUnicodeName")]
		ToUnicodeName,

		[Field ("NSStringTransformStripCombiningMarks")]
		StripCombiningMarks,

		[Field ("NSStringTransformStripDiacritics")]
		StripDiacritics,
	}

	[NoWatch, NoTV, NoMac, iOS (11, 0)]
	[Native]
	public enum NSUrlSessionMultipathServiceType : long {
		None = 0,
		Handover = 1,
		Interactive = 2,
		Aggregate = 3,
	}
}
