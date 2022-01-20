//
// CGEnums.cs: Enumerations
//
// Author:
//   Vincent Dondain (vidondai@microsoft.com)
//
// Copyright 2018-2019 Microsoft Corporation
//

using System;
using System.Runtime.InteropServices;
using ObjCRuntime;

namespace CoreGraphics {

	public enum MatrixOrder {
		Prepend = 0,
		Append = 1,
	}

	// untyped enum -> CGPath.h
	public enum CGLineJoin {
		Miter,
		Round,
		Bevel,
	}

	// untyped enum -> CGPath.h
	public enum CGLineCap {
		Butt,
		Round,
		Square,
	}

	// untyped enum -> CGContext.h
	public enum CGPathDrawingMode {
		Fill,
		EOFill,
		Stroke,
		FillStroke,
		EOFillStroke,
	}

	// untyped enum -> CGContext.h
	public enum CGTextDrawingMode : uint {
		Fill,
		Stroke,
		FillStroke,
		Invisible,
		FillClip,
		StrokeClip,
		FillStrokeClip,
		Clip,
	}

	// untyped enum -> CGContext.h
#if NET
	[UnsupportedOSPlatform ("macos10.9")]
	[UnsupportedOSPlatform ("ios7.0")]
#if MONOMAC
	[Obsolete ("Starting with macos10.9.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
	[Obsolete ("Starting with ios7.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
	[Deprecated (PlatformName.iOS, 7, 0)]
	[Deprecated (PlatformName.MacOSX, 10, 9)]
#endif
	public enum CGTextEncoding {
		FontSpecific,
		MacRoman,
	}

	// untyped enum -> CGContext.h
	public enum CGInterpolationQuality {
		Default,
		None,
		Low,
		High,
		Medium,		       /* Yes, in this order, since Medium was added in 4 */
	}

	// untyped enum -> CGContext.h
	public enum CGBlendMode {
		Normal,
		Multiply,
		Screen,
		Overlay,
		Darken,
		Lighten,
		ColorDodge,
		ColorBurn,
		SoftLight,
		HardLight,
		Difference,
		Exclusion,
		Hue,
		Saturation,
		Color,
		Luminosity,

		Clear,
		Copy,
		SourceIn,
		SourceOut,
		SourceAtop,
		DestinationOver,
		DestinationIn,
		DestinationOut,
		DestinationAtop,
		XOR,
		PlusDarker,
		PlusLighter,
	}

#if NET
	[SupportedOSPlatform ("macos10.15")]
	[SupportedOSPlatform ("ios13.0")]
#else
	[Mac (10,15)]
	[iOS (13,0)]
#endif
	public enum CGPdfTagType /* int32_t */ {
		Document = 100,
		Part,
		Art,
		Section,
		Div,
		BlockQuote,
		Caption,
		Toc,
		Toci,
		Index,
		NonStructure,
		Private,
		Paragraph = 200,
		Header,
		Header1,
		Header2,
		Header3,
		Header4,
		Header5,
		Header6,
		List = 300,
		ListItem,
		Label,
		ListBody,
		Table = 400,
		TableRow,
		TableHeaderCell,
		TableDataCell,
		TableHeader,
		TableBody,
		TableFooter,
		Span = 500,
		Quote,
		Note,
		Reference,
		Bibliography,
		Code,
		Link,
		Annotation,
		Ruby = 600,
		RubyBaseText,
		RubyAnnotationText,
		RubyPunctuation,
		Warichu,
		WarichuText,
		WarichuPunctiation,
		Figure = 700,
		Formula,
		Form,
	}

	// untyped enum -> CGPDFObject.h
	public enum CGPDFObjectType {
		Null = 1,
		Boolean,
		Integer,
		Real,
		Name,
		String,
		Array,
		Dictionary,
		Stream,
	};

#if NET
	[SupportedOSPlatform ("ios11.0")]
	[SupportedOSPlatform ("macos10.13")]
#else
	[iOS (11,0)]
	[Mac (10,13)]
#endif
	public enum CGPDFAccessPermissions : uint {
		AllowsLowQualityPrinting    = (1 << 0),
		AllowsHighQualityPrinting   = (1 << 1),
		AllowsDocumentChanges       = (1 << 2),
		AllowsDocumentAssembly      = (1 << 3),
		AllowsContentCopying        = (1 << 4),
		AllowsContentAccessibility  = (1 << 5),
		AllowsCommenting            = (1 << 6),
		AllowsFormFieldEntry        = (1 << 7),
	}

	// uint32_t enum -> CGColorConverter.h
#if NET
	[SupportedOSPlatform ("tvos9.2")]
	[SupportedOSPlatform ("ios9.3")]
	[UnsupportedOSPlatform ("macos")]
#if TVOS
	[Obsolete ("Starting with tvos10.0 replaced by 'CGColorConversionInfoTransformType'.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
	[Obsolete ("Starting with ios10.0 replaced by 'CGColorConversionInfoTransformType'.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
	[TV (9,2)]
	[Obsoleted (PlatformName.TvOS, 10,0, message: "Replaced by 'CGColorConversionInfoTransformType'.")]
	[iOS (9,3)]
	[Obsoleted (PlatformName.iOS, 10,0, message: "Replaced by 'CGColorConversionInfoTransformType'.")]
	[NoWatch]
	[NoMac]
#endif
	public enum CGColorConverterTransformType : uint {
		FromSpace,
		ToSpace,
		ApplySpace,
	}

	// uint32_t enum -> CGColorConversionInfo.h
#if NET
	[SupportedOSPlatform ("ios10.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("macos10.12")]
#else
	[iOS (10,0)]
	[TV (10,0)]
	[Watch (3,0)]
	[Mac (10,12)]
#endif
	public enum CGColorConversionInfoTransformType : uint {
		FromSpace = 0,
		ToSpace,
		ApplySpace,
	}
}
