//
// CGEnums.cs: Enumerations
//
// Author:
//   Vincent Dondain (vidondai@microsoft.com)
//
// Copyright 2018-2019 Microsoft Corporation
//

#nullable enable

using System.ComponentModel;

namespace CoreGraphics {

	/// <summary>Specifies the order in which an affine transformation is combined with the current transformation.</summary>
	public enum MatrixOrder {
		/// <summary>Applies the new transformation before the current transformation.</summary>
		Prepend = 0,
		/// <summary>Applies the new transformation after the current transformation.</summary>
		Append = 1,
	}

	// untyped enum -> CGPath.h
	/// <summary>Join type for drawing operations.</summary>
	///     <remarks>Specifies how to join consecutive line or curve segments in a figure.</remarks>
	public enum CGLineJoin {
		/// <summary>Specifies a mitered join. This produces a sharp corner or a clipped corner.</summary>
		Miter,
		/// <summary>Specifies a circular join. This produces a circular arc between the lines.</summary>
		Round,
		/// <summary>Specifies a beveled join. This produces a diagonal corner.</summary>
		Bevel,
	}

	// untyped enum -> CGPath.h
	/// <summary>Style for line caps.</summary>
	public enum CGLineCap {
		/// <summary>Specifies a flat line cap.</summary>
		Butt,
		/// <summary>Specifies a round line cap.</summary>
		Round,
		/// <summary>Specifies a square line cap.</summary>
		Square,
	}

	// untyped enum -> CGContext.h
	/// <include file="../../docs/api/CoreGraphics/CGPathDrawingMode.xml" path="/Documentation/Docs[@DocId='T:CoreGraphics.CGPathDrawingMode']/*" />
	public enum CGPathDrawingMode {
		/// <summary>Fills the path using the non-zero winding rule.</summary>
		Fill,
		/// <summary>Fills the path using the even-odd rule.</summary>
		EOFill,
		/// <summary>Strokes the path.</summary>
		Stroke,
		/// <summary>Fills and strokes the path using the non-zero winding rule.</summary>
		FillStroke,
		/// <summary>Fills and strokes the path using the even-odd rule.</summary>
		EOFillStroke,
	}

	// untyped enum -> CGContext.h
	/// <summary>Text drawing mode used by Quartz.</summary>
	/// <remarks>These drawing modes are used with the <see cref="CGContext.SetTextDrawingMode" /> method and they specify how the glyphs that make up the text should be drawn.</remarks>
	public enum CGTextDrawingMode : uint {
		/// <summary>Perform a Fill operation on the text glyphs.</summary>
		Fill,
		/// <summary>Perform a Stroke operation on the text glyphs.</summary>
		Stroke,
		/// <summary>Perform a Fill operation followed by a Stroke operation on the text glyphs.</summary>
		FillStroke,
		/// <summary>Does not draw the text, merely updates the text position</summary>
		Invisible,
		/// <summary>Perform a Fill operation on the text glyphs followed by a clip operation using the current clip path.</summary>
		FillClip,
		/// <summary>Strokes the text glyphs and uses the resulting text as the clipping path.</summary>
		StrokeClip,
		/// <summary>Perform a Stroke operation on the text glyphs followed by a clip operation using the current clip path.</summary>
		FillStrokeClip,
		/// <summary>Performs a clip operation using the current clip path without drawing the text.</summary>
		Clip,
	}

	// untyped enum -> CGContext.h
	/// <summary>Text encoding, this enumeration is deprecated, use CoreText APIs instead.</summary>
	[Deprecated (PlatformName.iOS, 7, 0)]
	[Deprecated (PlatformName.TvOS, 9, 0)]
	[Deprecated (PlatformName.MacOSX, 10, 9)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
	public enum CGTextEncoding {
		/// <summary>Built-in encoding of the font.</summary>
		FontSpecific,
		/// <summary>Mac encoding, an old ASCII-like encoding system used on MacOS.</summary>
		MacRoman,
	}

	// untyped enum -> CGContext.h
	/// <summary>Quality of interpolation for drawing images.</summary>
	public enum CGInterpolationQuality {
		/// <summary>Let the context pick the best interpolation mode.</summary>
		Default,
		/// <summary>Do not interpolate.</summary>
		None,
		/// <summary>Low interpolation quality, fast processing.</summary>
		Low,
		/// <summary>High quality, at the cost of speed.</summary>
		High,
		/// <summary>Use medium-quality interpolation as a balance between speed and image quality.</summary>
		Medium,            /* Yes, in this order, since Medium was added in 4 */
	}

	// untyped enum -> CGContext.h
	/// <include file="../../docs/api/CoreGraphics/CGBlendMode.xml" path="/Documentation/Docs[@DocId='T:CoreGraphics.CGBlendMode']/*" />
	public enum CGBlendMode {
		/// <summary>To be added.</summary>
		Normal,
		/// <summary>To be added.</summary>
		Multiply,
		/// <summary>To be added.</summary>
		Screen,
		/// <summary>To be added.</summary>
		Overlay,
		/// <summary>To be added.</summary>
		Darken,
		/// <summary>To be added.</summary>
		Lighten,
		/// <summary>To be added.</summary>
		ColorDodge,
		/// <summary>To be added.</summary>
		ColorBurn,
		/// <summary>To be added.</summary>
		SoftLight,
		/// <summary>To be added.</summary>
		HardLight,
		/// <summary>To be added.</summary>
		Difference,
		/// <summary>To be added.</summary>
		Exclusion,
		/// <summary>To be added.</summary>
		Hue,
		/// <summary>To be added.</summary>
		Saturation,
		/// <summary>To be added.</summary>
		Color,
		/// <summary>To be added.</summary>
		Luminosity,

		/// <summary>Clears the destination and makes it transparent (R=0).</summary>
		Clear,
		/// <summary>Copies the source color with alpha to the target (R=S)</summary>
		Copy,
		/// <summary>To be added.</summary>
		SourceIn,
		/// <summary>To be added.</summary>
		SourceOut,
		/// <summary>To be added.</summary>
		SourceAtop,
		/// <summary>To be added.</summary>
		DestinationOver,
		/// <summary>To be added.</summary>
		DestinationIn,
		/// <summary>To be added.</summary>
		DestinationOut,
		/// <summary>To be added.</summary>
		DestinationAtop,
		/// <summary>To be added.</summary>
		XOR,
		/// <summary>To be added.</summary>
		PlusDarker,
		/// <summary>To be added.</summary>
		PlusLighter,
	}

	[MacCatalyst (13, 1)]
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
		WarichuPunctuation,
#if !XAMCORE_5_0
		[Obsolete ("Use 'WarichuPunctuation' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		WarichuPunctiation = WarichuPunctuation,
#endif
		Figure = 700,
		Formula,
		Form,
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		Object = 800,
		/// <summary>A non-structural tag for content that should be excluded from the document's logical structure.</summary>
		[NoTV, Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
		Artifact = 900,
	}

	// untyped enum -> CGPDFObject.h
	/// <summary>Enumerates the various types of values that are found in a PDF document.</summary>
	public enum CGPDFObjectType {
		/// <summary>To be added.</summary>
		Null = 1,
		/// <summary>To be added.</summary>
		Boolean,
		/// <summary>To be added.</summary>
		Integer,
		/// <summary>To be added.</summary>
		Real,
		/// <summary>To be added.</summary>
		Name,
		/// <summary>To be added.</summary>
		String,
		/// <summary>To be added.</summary>
		Array,
		/// <summary>To be added.</summary>
		Dictionary,
		/// <summary>To be added.</summary>
		Stream,
	};

	[MacCatalyst (13, 1)]
	public enum CGPDFAccessPermissions : uint {
		/// <summary>To be added.</summary>
		AllowsLowQualityPrinting = (1 << 0),
		/// <summary>To be added.</summary>
		AllowsHighQualityPrinting = (1 << 1),
		/// <summary>To be added.</summary>
		AllowsDocumentChanges = (1 << 2),
		/// <summary>To be added.</summary>
		AllowsDocumentAssembly = (1 << 3),
		/// <summary>To be added.</summary>
		AllowsContentCopying = (1 << 4),
		/// <summary>To be added.</summary>
		AllowsContentAccessibility = (1 << 5),
		/// <summary>To be added.</summary>
		AllowsCommenting = (1 << 6),
		/// <summary>To be added.</summary>
		AllowsFormFieldEntry = (1 << 7),
	}

	// uint32_t enum -> CGColorConversionInfo.h
	[MacCatalyst (13, 1)]
	public enum CGColorConversionInfoTransformType : uint {
		/// <summary>To be added.</summary>
		FromSpace = 0,
		/// <summary>To be added.</summary>
		ToSpace,
		/// <summary>To be added.</summary>
		ApplySpace,
	}
}
