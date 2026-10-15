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
	/// <summary>Specifies how Quartz blends source content with existing destination content.</summary>
	/// <remarks>
	///   <para>Blend modes combine source and destination colors and alpha values when compositing images. The Porter-Duff modes define how source and destination coverage contribute to the result.</para>
	///   <para>The PDF reference manual provides a detailed explanation of these blend modes.</para>
	///   <para>The blend equations use the following terms:</para>
	///   <list type="table">
	///     <listheader>
	///       <term>Term</term>
	///       <description>Description</description>
	///     </listheader>
	///     <item>
	///       <term>R</term>
	///       <description>Premultiplied result color.</description>
	///     </item>
	///     <item>
	///       <term>S</term>
	///       <description>Source color.</description>
	///     </item>
	///     <item>
	///       <term>D</term>
	///       <description>Destination color.</description>
	///     </item>
	///     <item>
	///       <term>Sa</term>
	///       <description>Source alpha value.</description>
	///     </item>
	///     <item>
	///       <term>Da</term>
	///       <description>Destination alpha value.</description>
	///     </item>
	///   </list>
	/// </remarks>
	public enum CGBlendMode {
		/// <summary>Draws the source over the destination using normal source-over blending.</summary>
		Normal,
		/// <summary>Multiplies source and destination color components, producing a darker result.</summary>
		Multiply,
		/// <summary>Inverts, multiplies, and inverts source and destination color components, producing a lighter result.</summary>
		Screen,
		/// <summary>Multiplies or screens the colors depending on the destination color.</summary>
		Overlay,
		/// <summary>Selects the darker source or destination color component by component.</summary>
		Darken,
		/// <summary>Selects the lighter source or destination color component by component.</summary>
		Lighten,
		/// <summary>Brightens the destination color to reflect the source color.</summary>
		ColorDodge,
		/// <summary>Darkens the destination color to reflect the source color.</summary>
		ColorBurn,
		/// <summary>Softly darkens or lightens the destination color according to the source color.</summary>
		SoftLight,
		/// <summary>Multiplies or screens the colors depending on the source color.</summary>
		HardLight,
		/// <summary>Subtracts the darker color from the lighter color component by component.</summary>
		Difference,
		/// <summary>Creates an effect similar to Difference with lower contrast.</summary>
		Exclusion,
		/// <summary>Uses the source hue with the destination saturation and luminosity.</summary>
		Hue,
		/// <summary>Uses the source saturation with the destination hue and luminosity.</summary>
		Saturation,
		/// <summary>Uses the source hue and saturation with the destination luminosity.</summary>
		Color,
		/// <summary>Uses the source luminosity with the destination hue and saturation.</summary>
		Luminosity,

		/// <summary>Clears the destination and makes it transparent (R=0).</summary>
		Clear,
		/// <summary>Copies the source color and alpha to the destination (R=S).</summary>
		Copy,
		/// <summary>Keeps the source only where the destination has alpha.</summary>
		SourceIn,
		/// <summary>Keeps the source only where the destination is transparent.</summary>
		SourceOut,
		/// <summary>Places the source over the destination, restricted to the destination's opaque area.</summary>
		SourceAtop,
		/// <summary>Places the destination over the source.</summary>
		DestinationOver,
		/// <summary>Keeps the destination only where the source has alpha.</summary>
		DestinationIn,
		/// <summary>Keeps the destination only where the source is transparent.</summary>
		DestinationOut,
		/// <summary>Places the destination over the source, restricted to the source's opaque area.</summary>
		DestinationAtop,
		/// <summary>Keeps the portions of the source and destination that are not covered by the other.</summary>
		XOR,
		/// <summary>Adds source and destination color components and subtracts full intensity, clamping the result at zero.</summary>
		PlusDarker,
		/// <summary>Adds source and destination color components, clamping the result at full intensity.</summary>
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
		/// <summary>A null object.</summary>
		Null = 1,
		/// <summary>A Boolean value.</summary>
		Boolean,
		/// <summary>An integer number.</summary>
		Integer,
		/// <summary>A real number.</summary>
		Real,
		/// <summary>A name object.</summary>
		Name,
		/// <summary>A string object.</summary>
		String,
		/// <summary>An ordered array of objects.</summary>
		Array,
		/// <summary>A dictionary of key-value pairs.</summary>
		Dictionary,
		/// <summary>A stream object containing data and an associated dictionary.</summary>
		Stream,
	};

	/// <summary>Permission flags that describe operations allowed for a PDF document.</summary>
	/// <remarks>
	///   <para>When setting permissions for a PDF document, Quartz treats some permissions as supersets of others. For example, allowing document changes also grants commenting and form-field entry.</para>
	///   <para>Allowing high-quality printing also grants low-quality printing, content copying grants accessibility extraction, and commenting grants form-field entry.</para>
	///   <para>Existing documents may not follow these relationships, so check the narrowest permission that applies when reading permissions.</para>
	/// </remarks>
	[MacCatalyst (13, 1)]
	public enum CGPDFAccessPermissions : uint {
		/// <summary>Allows printing at up to 150 DPI.</summary>
		AllowsLowQualityPrinting = (1 << 0),
		/// <summary>Allows printing at any resolution.</summary>
		AllowsHighQualityPrinting = (1 << 1),
		/// <summary>Allows changes to document contents other than page management.</summary>
		AllowsDocumentChanges = (1 << 2),
		/// <summary>Allows inserting, deleting, and rotating pages.</summary>
		AllowsDocumentAssembly = (1 << 3),
		/// <summary>Allows extracting document content, such as text and images.</summary>
		AllowsContentCopying = (1 << 4),
		/// <summary>Allows extracting document content for accessibility purposes.</summary>
		AllowsContentAccessibility = (1 << 5),
		/// <summary>Allows creating or modifying annotations, including form-field entries.</summary>
		AllowsCommenting = (1 << 6),
		/// <summary>Allows modifying form-field entries.</summary>
		AllowsFormFieldEntry = (1 << 7),
	}

	// uint32_t enum -> CGColorConversionInfo.h
	/// <summary>Specifies how a color space participates in a multi-step color conversion.</summary>
	/// <remarks>Use this value as the <see cref="CGColorConversionInfoTriple.Transform"/> in a color conversion sequence.</remarks>
	[MacCatalyst (13, 1)]
	public enum CGColorConversionInfoTransformType : uint {
		/// <summary>Marks the source color space in a conversion sequence.</summary>
		FromSpace = 0,
		/// <summary>Marks the destination color space in a conversion sequence.</summary>
		ToSpace,
		/// <summary>Applies a conversion in this color space within the sequence.</summary>
		ApplySpace,
	}
}
