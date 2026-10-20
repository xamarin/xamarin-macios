// This file contains api definitions shared between AppKit and UIKit

using System.Diagnostics;
using System.ComponentModel;
using CoreAnimation;
using CoreGraphics;
using CoreText;
using UniformTypeIdentifiers;

using CGGlyph = System.UInt16;
using NSGlyph = System.UInt32;

#if HAS_WEBKIT
using WebKit;
#else
using WebPreferences = Foundation.NSObject;
#endif

#if !MONOMAC
using NSColor = UIKit.UIColor;
using NSFont = UIKit.UIFont;
#endif

// dummy types to simplify build
#if !MONOMAC
using NSAppearance = UIKit.UIAppearance;
using NSCell = System.Object;
using NSGlyphGenerator = System.Object;
using NSGlyphStorageOptions = System.Object;
using NSImageScaling = System.Object;
using NSRectEdge = System.Object;
using NSRulerMarker = System.Object;
using NSRulerView = System.Object;
using NSTextAttachmentCell = System.Object;
using NSTextTabType = System.Object;
using NSTextStorageEditedFlags = System.Object;
using NSTextView = System.Object;
using NSTypesetter = System.Object;
using NSTypesetterBehavior = System.Object;
using NSView = System.Object;
using NSWindow = System.Object;
#else
using UICollectionLayoutListConfiguration = System.Object;
using UIContentInsetsReference = System.Object;
using UIEdgeInsets = System.Object;
using UITraitCollection = System.Object;
#endif // !MONOMAC

#if MONOMAC
using BezierPath = AppKit.NSBezierPath;
using Image = AppKit.NSImage;
using TextAlignment = AppKit.NSTextAlignment;
using LineBreakMode = AppKit.NSLineBreakMode;
using CollectionLayoutSectionOrthogonalScrollingBehavior = AppKit.NSCollectionLayoutSectionOrthogonalScrollingBehavior;
using CollectionElementCategory = AppKit.NSCollectionElementCategory;
using StringAttributes = AppKit.NSStringAttributes;
using View = AppKit.NSView;
using UICollectionLayoutSectionOrthogonalScrollingProperties = System.Object;
using XTextView = AppKit.NSTextView;
using ScrollView = AppKit.NSScrollView;
#else
using BezierPath = UIKit.UIBezierPath;
using Image = UIKit.UIImage;
using TextAlignment = UIKit.UITextAlignment;
using LineBreakMode = UIKit.UILineBreakMode;
using CollectionLayoutSectionOrthogonalScrollingBehavior = UIKit.UICollectionLayoutSectionOrthogonalScrollingBehavior;
using CollectionElementCategory = UIKit.UICollectionElementCategory;
using StringAttributes = UIKit.UIStringAttributes;
using View = UIKit.UIView;
using XTextView = UIKit.UITextView;
using ScrollView = UIKit.UIScrollView;
#endif

#if MONOMAC
using IXWritingToolsCoordinatorDelegate = AppKit.INSWritingToolsCoordinatorDelegate;
using XWritingToolsBehavior = AppKit.NSWritingToolsBehavior;
using XWritingToolsCoordinator = AppKit.NSWritingToolsCoordinator;
using XWritingToolsCoordinatorAnimationParameters = AppKit.NSWritingToolsCoordinatorAnimationParameters;
using XWritingToolsCoordinatorContext = AppKit.NSWritingToolsCoordinatorContext;
using XWritingToolsCoordinatorContextScope = AppKit.NSWritingToolsCoordinatorContextScope;
using XWritingToolsCoordinatorDelegate = AppKit.NSWritingToolsCoordinatorDelegate;
using XWritingToolsCoordinatorDelegateReplaceRangeCallback = AppKit.NSWritingToolsCoordinatorDelegateReplaceRangeCallback;
using XWritingToolsCoordinatorDelegateRequestsBoundingBezierPathsCallback = AppKit.NSWritingToolsCoordinatorDelegateRequestsBoundingBezierPathsCallback;
using XWritingToolsCoordinatorDelegateRequestsContextsCallback = AppKit.NSWritingToolsCoordinatorDelegateRequestsContextsCallback;
using XWritingToolsCoordinatorDelegateRequestsDecorationContainerViewCallback = AppKit.NSWritingToolsCoordinatorDelegateRequestsDecorationContainerViewCallback;
using XWritingToolsCoordinatorDelegateRequestsGrammarResultsCallback = AppKit.NSWritingToolsCoordinatorDelegateRequestsGrammarResultsCallback;
using XWritingToolsCoordinatorDelegateRequestsPreviewForTextAnimationCallback = AppKit.NSWritingToolsCoordinatorDelegateRequestsPreviewForTextAnimationCallback;
using XWritingToolsCoordinatorDelegateRequestsPreviewForTextAnimationWithTextDecorationCallback = AppKit.NSWritingToolsCoordinatorDelegateRequestsPreviewForTextAnimationWithTextDecorationCallback;
using XWritingToolsCoordinatorDelegateRequestsRangeCallback = AppKit.NSWritingToolsCoordinatorDelegateRequestsRangeCallback;
using XWritingToolsCoordinatorDelegateRequestsSingleContainerSubrangesCallback = AppKit.NSWritingToolsCoordinatorDelegateRequestsSingleContainerSubrangesCallback;
using XWritingToolsCoordinatorDelegateRequestsUnderlinePathsCallback = AppKit.NSWritingToolsCoordinatorDelegateRequestsUnderlinePathsCallback;
using XWritingToolsCoordinatorState = AppKit.NSWritingToolsCoordinatorState;
using XWritingToolsCoordinatorTextAnimation = AppKit.NSWritingToolsCoordinatorTextAnimation;
using XWritingToolsCoordinatorTextDecoration = AppKit.NSWritingToolsCoordinatorTextDecoration;
using XWritingToolsCoordinatorTextReplacementReason = AppKit.NSWritingToolsCoordinatorTextReplacementReason;
using XWritingToolsCoordinatorTextUpdateReason = AppKit.NSWritingToolsCoordinatorTextUpdateReason;
using XWritingToolsResultOptions = AppKit.NSWritingToolsResultOptions;
#else
using IXWritingToolsCoordinatorDelegate = UIKit.IUIWritingToolsCoordinatorDelegate;
using XWritingToolsBehavior = UIKit.UIWritingToolsBehavior;
using XWritingToolsCoordinator = UIKit.UIWritingToolsCoordinator;
using XWritingToolsCoordinatorAnimationParameters = UIKit.UIWritingToolsCoordinatorAnimationParameters;
using XWritingToolsCoordinatorContext = UIKit.UIWritingToolsCoordinatorContext;
using XWritingToolsCoordinatorContextScope = UIKit.UIWritingToolsCoordinatorContextScope;
using XWritingToolsCoordinatorDelegate = UIKit.UIWritingToolsCoordinatorDelegate;
using XWritingToolsCoordinatorDelegateReplaceRangeCallback = UIKit.UIWritingToolsCoordinatorDelegateReplaceRangeCallback;
using XWritingToolsCoordinatorDelegateRequestsBoundingBezierPathsCallback = UIKit.UIWritingToolsCoordinatorDelegateRequestsBoundingBezierPathsCallback;
using XWritingToolsCoordinatorDelegateRequestsContextsCallback = UIKit.UIWritingToolsCoordinatorDelegateRequestsContextsCallback;
using XWritingToolsCoordinatorDelegateRequestsDecorationContainerViewCallback = UIKit.UIWritingToolsCoordinatorDelegateRequestsDecorationContainerViewCallback;
using XWritingToolsCoordinatorDelegateRequestsGrammarResultsCallback = UIKit.UIWritingToolsCoordinatorDelegateRequestsGrammarResultsCallback;
using XWritingToolsCoordinatorDelegateRequestsPreviewForTextAnimationCallback = UIKit.UIWritingToolsCoordinatorDelegateRequestsPreviewForTextAnimationCallback;
using XWritingToolsCoordinatorDelegateRequestsPreviewForTextAnimationWithTextDecorationCallback = UIKit.UIWritingToolsCoordinatorDelegateRequestsPreviewForTextAnimationWithTextDecorationCallback;
using XWritingToolsCoordinatorDelegateRequestsRangeCallback = UIKit.UIWritingToolsCoordinatorDelegateRequestsRangeCallback;
using XWritingToolsCoordinatorDelegateRequestsSingleContainerSubrangesCallback = UIKit.UIWritingToolsCoordinatorDelegateRequestsSingleContainerSubrangesCallback;
using XWritingToolsCoordinatorDelegateRequestsUnderlinePathsCallback = UIKit.UIWritingToolsCoordinatorDelegateRequestsUnderlinePathsCallback;
using XWritingToolsCoordinatorState = UIKit.UIWritingToolsCoordinatorState;
using XWritingToolsCoordinatorTextAnimation = UIKit.UIWritingToolsCoordinatorTextAnimation;
using XWritingToolsCoordinatorTextDecoration = UIKit.UIWritingToolsCoordinatorTextDecoration;
using XWritingToolsCoordinatorTextReplacementReason = UIKit.UIWritingToolsCoordinatorTextReplacementReason;
using XWritingToolsCoordinatorTextUpdateReason = UIKit.UIWritingToolsCoordinatorTextUpdateReason;
using XWritingToolsResultOptions = UIKit.UIWritingToolsResultOptions;
#endif

#if MONOMAC
namespace AppKit {
#else
namespace UIKit {
#endif

	/// <summary>A delegate used as the callback in <see cref="NSLayoutManager.EnumerateLineFragments(NSRange,NSTextLayoutEnumerateLineFragments)" />.</summary>
	delegate void NSTextLayoutEnumerateLineFragments (CGRect rect, CGRect usedRectangle, NSTextContainer textContainer, NSRange glyphRange, out bool stop);
	/// <summary>A delegate used as the callback in <see cref="NSLayoutManager.EnumerateEnclosingRects(NSRange,NSRange,NSTextContainer,NSTextLayoutEnumerateEnclosingRects)" />.</summary>
	delegate void NSTextLayoutEnumerateEnclosingRects (CGRect rect, out bool stop);

	// NSInteger -> NSLayoutManager.h
	/// <summary>An enumeration whose values specify actions caused by control characters.</summary>
	[Native]
	[Flags]
	[MacCatalyst (13, 1)]
	public enum NSControlCharacterAction : long {
		/// <summary>Glyphs with this action are filtered from the layout.</summary>
		ZeroAdvancement = (1 << 0),
		/// <summary>Uses <see cref="NSLayoutManagerDelegate.ShouldUseAction" /> or, if not overridden, <see cref="ZeroAdvancement" />.</summary>
		Whitespace = (1 << 1),
		/// <summary>Treated as a tab character.</summary>
		HorizontalTab = (1 << 2),
		/// <summary>Causes a line break.</summary>
		LineBreak = (1 << 3),
		/// <summary>Causes a paragraph break.</summary>
		ParagraphBreak = (1 << 4),
		/// <summary>Causes container break.</summary>
		ContainerBreak = (1 << 5),
	}

	[MacCatalyst (13, 0)]
	[Flags]
	[Native]
	public enum NSDirectionalRectEdge : ulong {
		None = 0x0,
		Top = 1uL << 0,
		Leading = 1uL << 1,
		Bottom = 1uL << 2,
		Trailing = 1uL << 3,
		All = Top | Leading | Bottom | Trailing,
	}

	// NSInteger -> NSLayoutManager.h
	/// <summary>An enumeration whose values specify characteristics of a glyph.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum NSGlyphProperty : long {
		/// <summary>Null glyph; ignored for layout and display.</summary>
		Null = (1 << 0),
		/// <summary>A control character such as tab, that has associated special behavior.</summary>
		ControlCharacter = (1 << 1),
		/// <summary>Glyphs such as whitespace that have elastic width behavior.</summary>
		Elastic = (1 << 2),
		/// <summary>Glyphs with combining properties, often characters in Unicode Mn class.</summary>
		NonBaseCharacter = (1 << 3),
	}

	// NSInteger -> NSLayoutConstraint.h
	/// <summary>An enumeration of attributes for use with constraint-based layouts.</summary>
	/// <remarks>Values used to specify particular constraint attributes in constraint-based layouts. These values are primarily used by methods in the <see cref="NSLayoutConstraint" /> class. </remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum NSLayoutAttribute : long {
		/// <summary>Indicates no attribute.</summary>
		NoAttribute = 0,
		/// <summary>The left-most X value of the element.</summary>
		Left = 1,
		/// <summary>The right-most X value of the element.</summary>
		Right,
		/// <summary>The top-most Y value of the element.</summary>
		Top,
		/// <summary>The bottom of the element.</summary>
		Bottom,
		/// <summary>In locales with right-to-left text, this value is equal to <see cref="NSLayoutAttribute.Left" />. In locales such as Hebrew or Arabic, equal to <see cref="NSLayoutAttribute.Right" />.</summary>
		Leading,
		/// <summary>In locales with right-to-left text, this value is equal to <see cref="NSLayoutAttribute.Right" />. In locales such as Hebrew or Arabic, equal to <see cref="NSLayoutAttribute.Left" />.</summary>
		Trailing,
		/// <summary>The size of the element along the X axis.</summary>
		Width,
		/// <summary>The size of the element along the Y axis.</summary>
		Height,
		/// <summary>The left-to-right midpoint of the element.</summary>
		CenterX,
		/// <summary>The top-to-bottom midpoint of the element.</summary>
		CenterY,
		/// <summary>The line on which text without descenders sits.</summary>
		Baseline,
		/// <summary>The element's last baseline.</summary>
		[MacCatalyst (13, 1)]
		LastBaseline = Baseline,
		/// <summary>The element's first baseline.</summary>
		[MacCatalyst (13, 1)]
		FirstBaseline,

		/// <summary>The left margin of the element.</summary>
		[NoMac]
		[MacCatalyst (13, 1)]
		LeftMargin,
		/// <summary>The right margin of the element.</summary>
		[NoMac]
		[MacCatalyst (13, 1)]
		RightMargin,
		/// <summary>The top margin of the element.</summary>
		[NoMac]
		[MacCatalyst (13, 1)]
		TopMargin,
		/// <summary>The margin from the bottom of the element.</summary>
		[NoMac]
		[MacCatalyst (13, 1)]
		BottomMargin,
		/// <summary>The leading edge of the element's margin.</summary>
		[NoMac]
		[MacCatalyst (13, 1)]
		LeadingMargin,
		/// <summary>The trailing margin of the element.</summary>
		[NoMac]
		[MacCatalyst (13, 1)]
		TrailingMargin,
		/// <summary>The X-center of the region bounded by the element's margins.</summary>
		[NoMac]
		[MacCatalyst (13, 1)]
		CenterXWithinMargins,
		/// <summary>The Y-center of the region bounded by the element's margins.</summary>
		[NoMac]
		[MacCatalyst (13, 1)]
		CenterYWithinMargins,
	}

	// NSUInteger -> NSLayoutConstraint.h
	/// <include file="../docs/api/UIKit/NSLayoutFormatOptions.xml" path="/Documentation/Docs[@DocId='T:UIKit.NSLayoutFormatOptions']/*" />
	[Native]
	[Flags]
	[MacCatalyst (13, 1)]
	public enum NSLayoutFormatOptions : ulong {
		/// <summary>To be added.</summary>
		None = 0,

		/// <summary>Aligns all elements using their <see cref="NSLayoutAttribute.Left" /> properties.</summary>
		AlignAllLeft = (1 << (int) NSLayoutAttribute.Left),
		/// <summary>Aligns all elements using their <see cref="NSLayoutAttribute.Right" /> properties.</summary>
		AlignAllRight = (1 << (int) NSLayoutAttribute.Right),
		/// <summary>Aligns all attributes using their <see cref="NSLayoutAttribute.Top" />s.</summary>
		AlignAllTop = (1 << (int) NSLayoutAttribute.Top),
		/// <summary>Aligns all elements using their <see cref="NSLayoutAttribute.Bottom" />s.</summary>
		AlignAllBottom = (1 << (int) NSLayoutAttribute.Bottom),
		/// <summary>Aligns all elements using their <see cref="NSLayoutAttribute.Leading" />s.</summary>
		AlignAllLeading = (1 << (int) NSLayoutAttribute.Leading),
		/// <summary>Aligns all attributes using their <see cref="NSLayoutAttribute.Trailing" />s.</summary>
		AlignAllTrailing = (1 << (int) NSLayoutAttribute.Trailing),
		/// <summary>Aligns all elements using their <see cref="NSLayoutAttribute.CenterX" />s.</summary>
		AlignAllCenterX = (1 << (int) NSLayoutAttribute.CenterX),
		/// <summary>Aligns all elements using their <see cref="NSLayoutAttribute.CenterY" />s.</summary>
		AlignAllCenterY = (1 << (int) NSLayoutAttribute.CenterY),
		/// <summary>Aligns all elements using their <see cref="NSLayoutAttribute.Baseline" />s.</summary>
		AlignAllBaseline = (1 << (int) NSLayoutAttribute.Baseline),
		/// <summary>Align all elements by using their last baselines.</summary>
		[MacCatalyst (13, 1)]
		AlignAllLastBaseline = (1 << (int) NSLayoutAttribute.LastBaseline),
		/// <summary>Align all elements by using their first baselines.</summary>
		[MacCatalyst (13, 1)]
		AlignAllFirstBaseline = (1 << (int) NSLayoutAttribute.FirstBaseline),

		/// <summary>Mask that can be applied to return only the alignment portion of the format options.</summary>
		AlignmentMask = 0xFFFF,

		/* choose only one of these three
		 */
		/// <summary>Arranges objects in the normal direction of text. In most languages this is left-to-right.</summary>
		DirectionLeadingToTrailing = 0 << 16, // default
		/// <summary>Arranges objects so that the first object is left-most.</summary>
		DirectionLeftToRight = 1 << 16,
		/// <summary>Arranges objects so that the first object is right-most.</summary>
		DirectionRightToLeft = 2 << 16,

		/// <summary>Arrange objects edge to edge.</summary>
		[NoMac]
		[MacCatalyst (13, 1)]
		SpacingEdgeToEdge = 0 << 19,
		/// <summary>Arrange objects to that their baselines align.</summary>
		[NoMac]
		[MacCatalyst (13, 1)]
		SpacingBaselineToBaseline = 1 << 19,
		/// <summary>A mask that selects only spacing bits of the format options.</summary>
		[NoMac]
		[MacCatalyst (13, 1)]
		SpacingMask = 1 << 19,

		/// <summary>Mask that can be applied to return only the directional component of the format options.</summary>
		DirectionMask = 0x3 << 16,
	}

	// NSInteger -> UITextInput.h
	/// <summary>An enumeration that specifies the relation between two attributes in a <see cref="NSLayoutConstraint" /></summary>
	/// <remarks>
	///   Constraint-based layouts are based on relationships between the values of two <see cref="NSLayoutAttribute" />s.
	///   Constraints can be made more flexible by allowing relationships other than strict equality, that is,
	///   <see cref="NSLayoutRelation.GreaterThanOrEqual" /> or <see cref="NSLayoutRelation.LessThanOrEqual" />. With relations
	///   other than <see cref="NSLayoutRelation.Equal" /> the constraint solver will attempt to minimize the difference
	///   in attributes. If <see cref="NSLayoutRelation.Equal" /> is specified and the constraint solver cannot solve the
	///   system of constraints, the constraint solver will throw an exception.
	/// </remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum NSLayoutRelation : long {
		/// <summary>A less-than-or-equal relationship.</summary>
		LessThanOrEqual = -1,
		/// <summary>An equality relationship.</summary>
		Equal = 0,
		/// <summary>A greater-than-or-equal relationship.</summary>
		GreaterThanOrEqual = 1,
	}

	[MacCatalyst (13, 1)]
	[Flags]
	[Native]
	public enum NSLineBreakStrategy : ulong {
		[iOS (14, 0), TV (14, 0), MacCatalyst (14, 0)]
		None = 0x0,
		PushOut = 1uL << 0,
		[iOS (14, 0), TV (14, 0), MacCatalyst (14, 0)]
		HangulWordPriority = 1uL << 1,
		[iOS (14, 0), TV (14, 0), MacCatalyst (14, 0)]
		Standard = 0xffff,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum NSRectAlignment : long {
		None = 0,
		Top,
		TopLeading,
		Leading,
		BottomLeading,
		Bottom,
		BottomTrailing,
		Trailing,
		TopTrailing,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum NSTextScalingType : long {
		Standard = 0,
		iOS,
	}

	// NSInteger -> NSLayoutManager.h
	/// <summary>An enumeration whose values specify the direction in which text is laid out. Used with <see cref="NSTextContainer.LayoutOrientation" />.</summary>
	[Native]
	[MacCatalyst (13, 1)]
	public enum NSTextLayoutOrientation : long {
		/// <summary>To be added.</summary>
		Horizontal,
		/// <summary>Lines are rendered vertically, extending from right to left.</summary>
		Vertical,
	}

	// NSUInteger -> NSTextStorage.h
	/// <summary>A flagging enumeration whose values are used by Text Kit to specify whether actions pertain to attributes, characters, or both.</summary>
	[Native]
	[Flags]
	[MacCatalyst (13, 1)]
	public enum NSTextStorageEditActions : ulong {
		/// <summary>Attributes were modified.</summary>
		Attributes = 1,
		/// <summary>Characters were modified.</summary>
		Characters = 2,
	}

	/// <include file="../docs/api/UIKit/NSLayoutManager.xml" path="/Documentation/Docs[@DocId='T:UIKit.NSLayoutManager']/*" />
	[MacCatalyst (13, 1)]
	[DesignatedDefaultCtor]
	[BaseType (typeof (NSObject))]
	partial interface NSLayoutManager : NSSecureCoding {
		/// <summary>An array of <see cref="NSTextContainer" />s that model the geometric layout of a document.</summary>
		/// <remarks>
		///   <para>The <see cref="NSLayoutManager" /> lays out the text in its <see cref="NSLayoutManager.TextStorage" /> property in the <see cref="NSTextContainer" />s of this property, starting with the <see cref="NSTextContainer" /> at index 0. </para>
		/// </remarks>
		[Export ("textContainers")]
		NSTextContainer [] TextContainers { get; }

		/// <summary>Appends a <see cref="NSTextContainer" /> to the <see cref="NSLayoutManager.TextContainers" /> property.</summary>
		/// <param name="container">An <see cref="NSTextContainer" /> to be appended to the <see cref="NSLayoutManager.TextContainers" /> property.</param>
		/// <altmember cref="NSLayoutManager.InsertTextContainer" />
		[Export ("addTextContainer:")]
		void AddTextContainer (NSTextContainer container);

		/// <summary>Inserts the specified <see cref="NSTextContainer" /> into <see cref="NSLayoutManager.TextContainers" /> at the specified <paramref name="index" />.</summary>
		/// <param name="container">To be added.</param>
		/// <param name="index">To be added.</param>
		/// <altmember cref="NSLayoutManager.AddTextContainer" />
		[Export ("insertTextContainer:atIndex:")]
		void InsertTextContainer (NSTextContainer container, /* NSUInteger */ nint index);

		/// <summary>Removes the specified <see cref="NSTextContainer" /> from the <see cref="NSLayoutManager.TextContainers" /> array. Invalidates layout as necessary.</summary>
		/// <param name="index">To be added.</param>
		/// <remarks>To be added.</remarks>
		[Export ("removeTextContainerAtIndex:")]
		void RemoveTextContainer (/* NSUInteger */ nint index);

		/// <summary>Invalidates the layout information and glyphs for the specified <see cref="NSTextContainer" /> and any following.</summary>
		/// <param name="container">To be added.</param>
		/// <remarks>
		///   <para>Application developers will typically not need to call this method unless they have subclassed <see cref="NSTextContainer" /> (for example, creating a <see cref="NSTextContainer" /> subclass that changes shape to accommodate placed graphics).</para>
		/// </remarks>
		[Export ("textContainerChangedGeometry:")]
		void TextContainerChangedGeometry (NSTextContainer container);

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Export ("textContainerChangedTextView:")]
		void TextContainerChangedTextView (NSTextContainer container);

		/// <summary>Whether the <see cref="NSLayoutManager" /> currently contains any areas of noncontiguous layout.</summary>
		/// <remarks>
		///   <para>Even if <see cref="NSLayoutManager.AllowsNonContiguousLayout" /> is <see langword="true" />, this method may return <see langword="false" />, for instance, if layout is complete.</para>
		/// </remarks>
		[Export ("hasNonContiguousLayout")]
		bool HasNonContiguousLayout { get; }

		/* InvalidateGlyphs */
		[Protected]
		[Export ("invalidateGlyphsForCharacterRange:changeInLength:actualCharacterRange:")]
		void InvalidateGlyphs (NSRange characterRange, /* NSInteger */ nint delta, /* nullable NSRangePointer */ IntPtr actualCharacterRange);

		/// <param name="characterRange">To be added.</param>
		/// <param name="delta">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Wrap ("InvalidateGlyphs (characterRange, delta, IntPtr.Zero)")]
		void InvalidateGlyphs (NSRange characterRange, /* NSInteger */ nint delta);

		/// <summary>Invalidates the glyphs in the <paramref name="characterRange" />.</summary>
		/// <param name="characterRange">To be added.</param>
		/// <param name="delta">To be added.</param>
		/// <param name="actualCharacterRange">To be added.</param>
		/// <remarks>To be added.</remarks>
		[Sealed]
		[Export ("invalidateGlyphsForCharacterRange:changeInLength:actualCharacterRange:")]
		void InvalidateGlyphs (NSRange characterRange, /* NSInteger */ nint delta, /* nullable NSRangePointer */ out NSRange actualCharacterRange);

		/* InvalidateLayout */
		[Protected]
		[Export ("invalidateLayoutForCharacterRange:actualCharacterRange:")]
		void InvalidateLayout (NSRange characterRange, /* nullable NSRangePointer */ IntPtr actualCharacterRange);

		/// <param name="characterRange">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Wrap ("InvalidateLayout (characterRange, IntPtr.Zero)")]
		void InvalidateLayout (NSRange characterRange);

		/// <summary>Invalidates the layout for the specified character range. Does not automatically trigger re-layout.</summary>
		/// <param name="actualCharacterRange">If not <see langword="null" />, on output holds the actual range invalidated.</param>
		/// <remarks>
		///   <para>This method does not trigger either glyph generation or layout. Application developers will not normally need to call this method.</para>
		/// </remarks>
		/// <param name="characterRange">To be added.</param>
		[Sealed]
		[Export ("invalidateLayoutForCharacterRange:actualCharacterRange:")]
		void InvalidateLayout (NSRange characterRange, /* nullable NSRangePointer */ out NSRange actualCharacterRange);

		/// <summary>Invalidates the display for the given character range.</summary>
		/// <param name="characterRange">To be added.</param>
		/// <remarks>
		///   <para>This method does not automatically trigger layout.</para>
		/// </remarks>
		[Export ("invalidateDisplayForCharacterRange:")]
		void InvalidateDisplayForCharacterRange (NSRange characterRange);

		/// <summary>Invalidates the display for the given glyph range.</summary>
		/// <param name="glyphRange">To be added.</param>
		/// <remarks>
		///   <para>This method does not automatically trigger layout.</para>
		/// </remarks>
		[Export ("invalidateDisplayForGlyphRange:")]
		void InvalidateDisplayForGlyphRange (NSRange glyphRange);

		/// <summary>Forces the <see cref="NSLayoutManager" /> to generate glyphs for the specified characters, if it has not already done so.</summary>
		/// <param name="characterRange">To be added.</param>
		/// <remarks>
		///   <para>The <see cref="NSLayoutManager" /> may calculate glyphs for a range larger than the <paramref name="characterRange" />. If <see cref="NSLayoutManager.AllowsNonContiguousLayout" /> is <see langword="false" />, the range will always extend to the beginning of the text.</para>
		/// </remarks>
		[Export ("ensureGlyphsForCharacterRange:")]
		void EnsureGlyphsForCharacterRange (NSRange characterRange);

		/// <summary>Forces the <see cref="NSLayoutManager" /> to generate glyphs for the specified glyph range, if it has not already done so.</summary>
		/// <param name="glyphRange">To be added.</param>
		/// <remarks>
		///   <para>The <see cref="NSLayoutManager" /> may calculate glyphs for a range larger than the <paramref name="glyphRange" />. If <see cref="NSLayoutManager.AllowsNonContiguousLayout" /> is <see langword="false" />, the range will always extend to the beginning of the text.</para>
		/// </remarks>
		[Export ("ensureGlyphsForGlyphRange:")]
		void EnsureGlyphsForGlyphRange (NSRange glyphRange);

		/// <summary>Forces the <see cref="NSLayoutManager" /> to layout the specified characters, if it has not already done so.</summary>
		/// <param name="characterRange">To be added.</param>
		/// <remarks>
		///   <para>The <see cref="NSLayoutManager" /> may layout an area larger than the <paramref name="characterRange" />. If <see cref="NSLayoutManager.AllowsNonContiguousLayout" /> is <see langword="false" />, the range will always extend to the beginning of the text.</para>
		/// </remarks>
		[Export ("ensureLayoutForCharacterRange:")]
		void EnsureLayoutForCharacterRange (NSRange characterRange);

		/// <summary>Forces the <see cref="NSLayoutManager" /> to layout the specified glyphs, if it has not already done so.</summary>
		/// <param name="glyphRange">To be added.</param>
		/// <remarks>
		///   <para>The <see cref="NSLayoutManager" /> may layout a larger range than the specified <paramref name="glyphRange" />. If <see cref="NSLayoutManager.AllowsNonContiguousLayout" /> is <see langword="false" />, the range will always extend to the beginning of the text.</para>
		/// </remarks>
		[Export ("ensureLayoutForGlyphRange:")]
		void EnsureLayoutForGlyphRange (NSRange glyphRange);

		/// <summary>Forces the <see cref="NSLayoutManager" /> to layout the specified <see cref="NSTextContainer" />, if it has not already done so.</summary>
		/// <param name="container">To be added.</param>
		/// <remarks>
		///   <para>The <see cref="NSLayoutManager" /> may layout more than the specified <paramref name="container" />. If <see cref="NSLayoutManager.AllowsNonContiguousLayout" /> is <see langword="false" />, the range will always extend to the beginning of the text.</para>
		/// </remarks>
		[Export ("ensureLayoutForTextContainer:")]
		void EnsureLayoutForTextContainer (NSTextContainer container);

		/// <summary>Forces the layout manager to perform layout on <paramref name="bounds" /> within <paramref name="container" />.</summary>
		/// <param name="bounds">To be added.</param>
		/// <param name="container">To be added.</param>
		/// <remarks>
		///   <para>The layout manager may lay out areas larger than <paramref name="bounds" />.</para>
		/// </remarks>
		[Export ("ensureLayoutForBoundingRect:inTextContainer:")]
		void EnsureLayoutForBoundingRect (CGRect bounds, NSTextContainer container);

		/// <summary>The number of glyphs in the <see cref="NSLayoutManager" />.</summary>
		/// <value>To be added.</value>
		/// <remarks>
		///   <para>If <see cref="NSLayoutManager.AllowsNonContiguousLayout" /> is <see langword="false" />, this method will force glyph generation for all characters.</para>
		/// </remarks>
		[Export ("numberOfGlyphs")]
		nuint NumberOfGlyphs { get; }

		/// <summary>Developers should use <see cref="NSLayoutManager.GetGlyph" /> rather than this deprecated method.</summary>
		/// <param name="glyphIndex">To be added.</param>
		/// <param name="isValidIndex">To be added.</param>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("glyphAtIndex:isValidIndex:")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'GetCGGlyph' instead).")]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'GetGlyph' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'GetGlyph' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GetGlyph' instead.")]
#if MONOMAC
		NSGlyph GlyphAtIndex (nuint glyphIndex, ref bool isValidIndex);
#else
		CGGlyph GlyphAtIndex (nuint glyphIndex, ref bool isValidIndex);
#endif // MONOMAC

		/// <summary>Developers should not use this deprecated method. Developers should use <see cref="NSLayoutManager.GetGlyph" /> instead.</summary>
		/// <param name="glyphIndex">To be added.</param>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("glyphAtIndex:")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'GetCGGlyph' instead).")]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'GetGlyph' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'GetGlyph' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GetGlyph' instead.")]
#if MONOMAC
		NSGlyph GlyphAtIndex (nuint glyphIndex);
#else
		CGGlyph GlyphAtIndex (nuint glyphIndex);
#endif // MONOMAC

		/// <summary>Whether the <paramref name="glyphIndex" /> specifies a valid glyph.</summary>
		/// <param name="glyphIndex">To be added.</param>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("isValidGlyphIndex:")]
		bool IsValidGlyph (nuint glyphIndex);

		/// <summary>The index of the first character associated with the glyph at the specified index.</summary>
		/// <param name="glyphIndex">To be added.</param>
		/// <returns>To be added.</returns>
		/// <remarks>
		///   <para>If <see cref="NSLayoutManager.AllowsNonContiguousLayout" /> is <see langword="false" />, calling this method will result in generating all glyphs up to and including <paramref name="glyphIndex" />.</para>
		/// </remarks>
		[Export ("characterIndexForGlyphAtIndex:")]
		nuint GetCharacterIndex (nuint glyphIndex);

		/// <summary>The glyph index for the character at the specified index.</summary>
		/// <param name="characterIndex">To be added.</param>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("glyphIndexForCharacterAtIndex:")]
		nuint GetGlyphIndex (nuint characterIndex);

		/// <summary>Sets the NSTextContainer for the specified glyph range.</summary>
		/// <param name="container">To be added.</param>
		/// <param name="glyphRange">To be added.</param>
		/// <remarks>To be added.</remarks>
		[Export ("setTextContainer:forGlyphRange:")]
		void SetTextContainer (NSTextContainer container, NSRange glyphRange);

		/// <summary>Associated the line fragment with bounds <paramref name="fragmentRect" /> with the glyphs in <paramref name="glyphRange" />.</summary>
		/// <param name="fragmentRect">To be added.</param>
		/// <param name="glyphRange">To be added.</param>
		/// <param name="usedRect">To be added.</param>
		/// <remarks>To be added.</remarks>
		[Export ("setLineFragmentRect:forGlyphRange:usedRect:")]
		void SetLineFragment (CGRect fragmentRect, NSRange glyphRange, CGRect usedRect);

		/// <summary>Sets the details for the extra line fragment required when the text back is either totally empty or ends with a hard line break.</summary>
		/// <param name="fragmentRect">To be added.</param>
		/// <param name="usedRect">To be added.</param>
		/// <param name="container">To be added.</param>
		/// <remarks>
		///   <para>Developers should only call this method when implementing custom typesetting.</para>
		/// </remarks>
		[Export ("setExtraLineFragmentRect:usedRect:textContainer:")]
		void SetExtraLineFragment (CGRect fragmentRect, CGRect usedRect, NSTextContainer container);

		/// <summary>Sets the <paramref name="location" /> for the first glyph in <paramref name="forStartOfGlyphRange" />.</summary>
		/// <param name="location">To be added.</param>
		/// <param name="forStartOfGlyphRange">To be added.</param>
		/// <remarks>To be added.</remarks>
		[Export ("setLocation:forStartOfGlyphRange:")]
		void SetLocation (CGPoint location, NSRange forStartOfGlyphRange);

		/// <summary>Specifies that the glyph at the specified index should be marked as not shown.</summary>
		/// <param name="flag">To be added.</param>
		/// <param name="glyphIndex">To be added.</param>
		/// <remarks>
		///   <para>This method is generally only called by custom typesetters.</para>
		/// </remarks>
		[Export ("setNotShownAttribute:forGlyphAtIndex:")]
		void SetNotShownAttribute (bool flag, nuint glyphIndex);

		/// <summary>Specifies whether the glyph at the specified index draws outside the bounds of its line segment.</summary>
		/// <param name="flag">To be added.</param>
		/// <param name="glyphIndex">To be added.</param>
		/// <remarks>
		///   <para>This method is generally only called by custom typesetters.</para>
		/// </remarks>
		[Export ("setDrawsOutsideLineFragment:forGlyphAtIndex:")]
		void SetDrawsOutsideLineFragment (bool flag, nuint glyphIndex);

		/// <summary>Sets the size for the glyph to draw within the <paramref name="glyphRange" />.</summary>
		/// <param name="attachmentSize">To be added.</param>
		/// <param name="glyphRange">To be added.</param>
		/// <remarks>To be added.</remarks>
		[Export ("setAttachmentSize:forGlyphRange:")]
		void SetAttachmentSize (CGSize attachmentSize, NSRange glyphRange);

		/// <summary>The indices of the first character and glyph that are not laid out.</summary>
		/// <param name="characterIndex">To be added.</param>
		/// <param name="glyphIndex">To be added.</param>
		/// <remarks>
		///   <para>Application developers should be aware that <paramref name="characterIndex" /> and <paramref name="glyphIndex" /> may be <see langword="null" /> if the text is fully laid out.</para>
		/// </remarks>
		[Export ("getFirstUnlaidCharacterIndex:glyphIndex:")]
		void GetFirstUnlaidCharacterIndex (out nuint characterIndex, out nuint glyphIndex);

		/// <summary>The index of the first character that has not been laid out.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Export ("firstUnlaidCharacterIndex")]
		nuint FirstUnlaidCharacterIndex { get; }

		/// <summary>The index of the first glyph that has not been laid out.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Export ("firstUnlaidGlyphIndex")]
		nuint FirstUnlaidGlyphIndex { get; }

		/* GetTextContainer */
		[Protected]
		[return: NullAllowed]
		[Export ("textContainerForGlyphAtIndex:effectiveRange:")]
		NSTextContainer GetTextContainer (nuint glyphIndex, /* nullable NSRangePointer */ IntPtr effectiveGlyphRange);

		/// <param name="glyphIndex">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[return: NullAllowed]
		[Wrap ("GetTextContainer (glyphIndex, IntPtr.Zero)")]
		NSTextContainer GetTextContainer (nuint glyphIndex);

		/// <param name="glyphIndex">To be added.</param>
		/// <param name="effectiveGlyphRange">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Sealed]
		[return: NullAllowed]
		[Export ("textContainerForGlyphAtIndex:effectiveRange:")]
		NSTextContainer GetTextContainer (nuint glyphIndex, /* nullable NSRangePointer */ out NSRange effectiveGlyphRange);

		[Protected]
		[return: NullAllowed]
		[Export ("textContainerForGlyphAtIndex:effectiveRange:withoutAdditionalLayout:")]
		NSTextContainer GetTextContainer (nuint glyphIndex, IntPtr effectiveGlyphRange, bool withoutAdditionalLayout);

		/// <param name="glyphIndex">To be added.</param>
		/// <param name="flag">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[return: NullAllowed]
		[Wrap ("GetTextContainer (glyphIndex, IntPtr.Zero, flag)")]
		NSTextContainer GetTextContainer (nuint glyphIndex, bool flag);

		/// <summary>Gets the <see cref="NSTextContainer" /> containing the glyph at <paramref name="glyphIndex" />, with the option of not triggering layout.</summary>
		/// <param name="glyphIndex">The index of the glyph for which the rect is requested.</param>
		/// <param name="effectiveGlyphRange">If not <see langword="null" />, the range of all glyphs in the line fragment.</param>
		/// <param name="withoutAdditionalLayout">If <see langword="true" />, glyph generation and layout are not performed.</param>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Sealed]
		[return: NullAllowed]
		[Export ("textContainerForGlyphAtIndex:effectiveRange:withoutAdditionalLayout:")]
		NSTextContainer GetTextContainer (nuint glyphIndex, /* nullable NSRangePointer */ out NSRange effectiveGlyphRange, bool withoutAdditionalLayout);

		/// <summary>The bounding rectangle in the <see cref="NSTextContainer" />'s coordinates of the laid out glyphs in the <see cref="NSTextContainer" />.</summary>
		/// <param name="container">To be added.</param>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("usedRectForTextContainer:")]
		CGRect GetUsedRect (NSTextContainer container);

		/* GetLineFragmentRect (NSUInteger, NSRangePointer) */
		[Protected]
		[Export ("lineFragmentRectForGlyphAtIndex:effectiveRange:")]
		CGRect GetLineFragmentRect (nuint glyphIndex, /* nullable NSRangePointer */ IntPtr effectiveGlyphRange);

		/// <param name="glyphIndex">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Wrap ("GetLineFragmentRect (glyphIndex, IntPtr.Zero)")]
		CGRect GetLineFragmentRect (nuint glyphIndex);

		/// <param name="glyphIndex">To be added.</param>
		/// <param name="effectiveGlyphRange">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Sealed]
		[Export ("lineFragmentRectForGlyphAtIndex:effectiveRange:")]
		CGRect GetLineFragmentRect (nuint glyphIndex, out /* nullable NSRangePointer */ NSRange effectiveGlyphRange);

		/* GetLineFragmentRect (NSUInteger, NSRangePointer, bool) */
		[MacCatalyst (13, 1)]
		[Protected]
		[Export ("lineFragmentRectForGlyphAtIndex:effectiveRange:withoutAdditionalLayout:")]
		CGRect GetLineFragmentRect (nuint glyphIndex, /* nullable NSRangePointer */ IntPtr effectiveGlyphRange, bool withoutAdditionalLayout);

		/// <param name="glyphIndex">To be added.</param>
		/// <param name="withoutAdditionalLayout">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Wrap ("GetLineFragmentRect (glyphIndex, IntPtr.Zero)")]
		CGRect GetLineFragmentRect (nuint glyphIndex, bool withoutAdditionalLayout);

		/// <param name="glyphIndex">The index of the glyph for which the rect is requested.</param>
		/// <param name="effectiveGlyphRange">If not <see langword="null" />, the range of all glyphs in the line fragment.</param>
		/// <param name="withoutAdditionalLayout">If <see langword="true" />, glyph generation and layout are not performed.</param>
		/// <summary>Gets the line fragment <see cref="CoreGraphics.CGRect" /> containing the glyph at <paramref name="glyphIndex" />, with the option of not triggering layout.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Sealed]
		[Export ("lineFragmentRectForGlyphAtIndex:effectiveRange:withoutAdditionalLayout:")]
		CGRect GetLineFragmentRect (nuint glyphIndex, out /* nullable NSRangePointer */ NSRange effectiveGlyphRange, bool withoutAdditionalLayout);

		/* GetLineFragmentUsedRect (NSUInteger, NSRangePointer) */
		[Protected]
		[Export ("lineFragmentUsedRectForGlyphAtIndex:effectiveRange:")]
		CGRect GetLineFragmentUsedRect (nuint glyphIndex, /* nullable NSRangePointer */ IntPtr effectiveGlyphRange);

		/// <param name="glyphIndex">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Wrap ("GetLineFragmentUsedRect (glyphIndex, IntPtr.Zero)")]
		CGRect GetLineFragmentUsedRect (nuint glyphIndex);

		/// <param name="glyphIndex">To be added.</param>
		/// <param name="effectiveGlyphRange">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Sealed]
		[Export ("lineFragmentUsedRectForGlyphAtIndex:effectiveRange:")]
		CGRect GetLineFragmentUsedRect (nuint glyphIndex, out /* nullable NSRangePointer */ NSRange effectiveGlyphRange);

		/* GetLineFragmentUsedRect (NSUInteger, NSRangePointer, bool) */
		[MacCatalyst (13, 1)]
		[Protected]
		[Export ("lineFragmentUsedRectForGlyphAtIndex:effectiveRange:withoutAdditionalLayout:")]
		CGRect GetLineFragmentUsedRect (nuint glyphIndex, /* nullable NSRangePointer */ IntPtr effectiveGlyphRange, bool withoutAdditionalLayout);

		/// <param name="glyphIndex">To be added.</param>
		/// <param name="withoutAdditionalLayout">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Wrap ("GetLineFragmentUsedRect (glyphIndex, IntPtr.Zero)")]
		CGRect GetLineFragmentUsedRect (nuint glyphIndex, bool withoutAdditionalLayout);

		/// <param name="glyphIndex">The index of the glyph for which the rect is requested.</param>
		/// <param name="effectiveGlyphRange">If not <see langword="null" />, the range of all glyphs in the line fragment.</param>
		/// <param name="withoutAdditionalLayout">If <see langword="true" />, glyph generation and layout are not performed.</param>
		/// <summary>Gets the usage <see cref="CoreGraphics.CGRect" /> containing the glyph at <paramref name="glyphIndex" />, with the option of not triggering layout.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Sealed]
		[Export ("lineFragmentUsedRectForGlyphAtIndex:effectiveRange:withoutAdditionalLayout:")]
		CGRect GetLineFragmentUsedRect (nuint glyphIndex, out /* nullable NSRangePointer */ NSRange effectiveGlyphRange, bool withoutAdditionalLayout);

		/// <summary>The <see cref="CGRect" /> needed for the insertion point.</summary>
		///         <value>Returns the rectangle defining the extra line fragment for the insertion point or {0,0,0,0} if there is no such retangle.</value>
		///         <remarks>To be added.</remarks>
		[Export ("extraLineFragmentRect")]
		CGRect ExtraLineFragmentRect { get; }

		/// <summary>The rectangle enclosing the insertion point.</summary>
		/// <value>Twice the <see cref="NSTextContainer.LineFragmentPadding" />, with the insertion point in the middle.</value>
		/// <remarks>To be added.</remarks>
		[Export ("extraLineFragmentUsedRect")]
		CGRect ExtraLineFragmentUsedRect { get; }

		/// <summary>The <see cref="NSTextContainer" /> containing the <see cref="NSLayoutManager.ExtraLineFragmentRect" />.</summary>
		/// <value>Returns <see langword="null" /> if the <see cref="NSLayoutManager.ExtraLineFragmentRect" /> does not exist (is {0,0,0,0}).</value>
		/// <remarks>To be added.</remarks>
		[Export ("extraLineFragmentTextContainer")]
		NSTextContainer ExtraLineFragmentTextContainer { get; }

		/// <param name="glyphIndex">To be added.</param>
		/// <summary>The location of the glyph at the specified index, relative to the containing line fragment's origin.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>
		///           <para>This method will layout and generate glyphs for the line fragment containing the glyph at <paramref name="glyphIndex" />.</para>
		///         </remarks>
		[Export ("locationForGlyphAtIndex:")]
		CGPoint GetLocationForGlyph (nuint glyphIndex);

		/// <summary>Whether the glyph at the specified index is shown.</summary>
		/// <param name="glyphIndex">To be added.</param>
		/// <returns>To be added.</returns>
		/// <remarks>
		///   <para>Glyphs such as tabs and newlines are not typically shown, but effect layout. Spaces are considered shown, as they "show" a characteristic displacement.</para>
		///   <para>This method will cause layout up to the specified index. If <see cref="NSLayoutManager.AllowsNonContiguousLayout" /> is <see langword="true" />, the layout will be confined to the containing line fragment.</para>
		/// </remarks>
		[Export ("notShownAttributeForGlyphAtIndex:")]
		bool IsNotShownAttributeForGlyph (nuint glyphIndex);

		/// <param name="glyphIndex">To be added.</param>
		/// <summary>Returns <see langword="true" /> if the specified glyph draws outside of its line fragment rectangle.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("drawsOutsideLineFragmentForGlyphAtIndex:")]
		bool DrawsOutsideLineFragmentForGlyph (nuint glyphIndex);

		/// <param name="glyphIndex">To be added.</param>
		/// <summary>The size of the attachment cell associated with the glyph at the specified index</summary>
		/// <returns>The size of attachment cell at the glyph at <paramref name="glyphIndex" />. Returns {-1.0f, -1.0f} if there is no attachment at the specified glyph.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("attachmentSizeForGlyphAtIndex:")]
		CGSize GetAttachmentSizeForGlyph (nuint glyphIndex);

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Export ("setLayoutRect:forTextBlock:glyphRange:")]
		void SetLayoutRect (CGRect layoutRect, NSTextBlock forTextBlock, NSRange glyphRange);

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Export ("setBoundsRect:forTextBlock:glyphRange:")]
		void SetBoundsRect (CGRect boundsRect, NSTextBlock forTextBlock, NSRange glyphRange);

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Export ("layoutRectForTextBlock:glyphRange:")]
		CGRect GetLayoutRect (NSTextBlock block, NSRange glyphRange);

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Export ("boundsRectForTextBlock:glyphRange:")]
		CGRect GetBoundsRect (NSTextBlock block, NSRange glyphRange);

		/* GetLayoutRect (NSTextBlock, NSUInteger, nullable NSRangePointer) */

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Protected]
		[Export ("layoutRectForTextBlock:atIndex:effectiveRange:")]
		CGRect GetLayoutRect (NSTextBlock block, nuint glyphIndex, IntPtr effectiveGlyphRange);

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Wrap ("GetLayoutRect (block, glyphIndex, IntPtr.Zero)")]
		CGRect GetLayoutRect (NSTextBlock block, nuint glyphIndex);

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Sealed]
		[Export ("layoutRectForTextBlock:atIndex:effectiveRange:")]
		CGRect GetLayoutRect (NSTextBlock block, nuint glyphIndex, out NSRange effectiveGlyphRange);

		/* GetBoundsRect (NSTextBlock, NSUInteger, nullable NSRangePointer) */

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Protected]
		[Export ("boundsRectForTextBlock:atIndex:effectiveRange:")]
		CGRect GetBoundsRect (NSTextBlock block, nuint glyphIndex, IntPtr effectiveGlyphRange);

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Wrap ("GetBoundsRect (block, glyphIndex, IntPtr.Zero)")]
		CGRect GetBoundsRect (NSTextBlock block, nuint glyphIndex);

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Sealed]
		[Export ("boundsRectForTextBlock:atIndex:effectiveRange:")]
		CGRect GetBoundsRect (NSTextBlock block, nuint glyphIndex, out NSRange effectiveGlyphRange);

		/* GetGlyphRange (NSRange, nullable NSRangePointer) */

		/// <param name="characterRange">To be added.</param>
		///         <param name="actualCharacterRange">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Protected]
		[Export ("glyphRangeForCharacterRange:actualCharacterRange:")]
		NSRange GetGlyphRange (NSRange characterRange, IntPtr actualCharacterRange);

		/// <param name="characterRange">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Wrap ("GetGlyphRange (characterRange, IntPtr.Zero)")]
		NSRange GetGlyphRange (NSRange characterRange);

		/// <param name="characterRange">To be added.</param>
		///         <param name="actualCharacterRange">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Sealed]
		[Export ("glyphRangeForCharacterRange:actualCharacterRange:")]
		NSRange GetGlyphRange (NSRange characterRange, out NSRange actualCharacterRange);

		/* GetCharacterRange (NSRange, nullable NSRangePointer) */
		/// <param name="glyphRange">To be added.</param>
		///         <param name="actualGlyphRange">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Protected]
		[Export ("characterRangeForGlyphRange:actualGlyphRange:")]
		NSRange GetCharacterRange (NSRange glyphRange, IntPtr actualGlyphRange);

		/// <param name="glyphRange">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Wrap ("GetCharacterRange (glyphRange, IntPtr.Zero)")]
		NSRange GetCharacterRange (NSRange glyphRange);

		/// <param name="glyphRange">To be added.</param>
		///         <param name="actualGlyphRange">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Sealed]
		[Export ("characterRangeForGlyphRange:actualGlyphRange:")]
		NSRange GetCharacterRange (NSRange glyphRange, out NSRange actualGlyphRange);

		/// <summary>The range of glyph indices contained in the specified <see cref="NSTextContainer" />.</summary>
		/// <param name="container">To be added.</param>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("glyphRangeForTextContainer:")]
		NSRange GetGlyphRange (NSTextContainer container);

		/// <param name="glyphIndex">To be added.</param>
		/// <summary>The largest range of glyphs surrounding the glyph at the specified index that can be displayed using only advancement, not pairwise kerning or other adjustments.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("rangeOfNominallySpacedGlyphsContainingIndex:")]
		NSRange GetRangeOfNominallySpacedGlyphsContainingIndex (nuint glyphIndex);

		[Internal]
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Export ("rectArrayForGlyphRange:withinSelectedGlyphRange:inTextContainer:rectCount:")]
		[Deprecated (PlatformName.MacOSX, 10, 11)]
		IntPtr GetRectArray (NSRange glyphRange, NSRange selectedGlyphRange, IntPtr textContainerHandle, out nuint rectCount);

		/// <param name="glyphRange">To be added.</param>
		/// <param name="container">To be added.</param>
		/// <summary>The bounding rectangle, in container coordinates, for the glyphs in the specified range.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>
		///   <para>The returned <see cref="CGRect" /> includes the area needed for all marks associated with the glyphs, including the area needed for glyphs that draw outside of their line fragment rectangle and for marks such as underlining.</para>
		/// </remarks>
		[Export ("boundingRectForGlyphRange:inTextContainer:")]
		CGRect GetBoundingRect (NSRange glyphRange, NSTextContainer container);

		/// <param name="bounds">To be added.</param>
		/// <param name="container">To be added.</param>
		/// <summary>Returns the range of glyph indices that are at least partially in the <paramref name="bounds" />.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("glyphRangeForBoundingRect:inTextContainer:")]
		NSRange GetGlyphRangeForBoundingRect (CGRect bounds, NSTextContainer container);

		/// <param name="bounds">To be added.</param>
		/// <param name="container">To be added.</param>
		/// <summary>Returns the range of glyph indices that are at least partially in the <paramref name="bounds" /> without glyph production or layout.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("glyphRangeForBoundingRectWithoutAdditionalLayout:inTextContainer:")]
		NSRange GetGlyphRangeForBoundingRectWithoutAdditionalLayout (CGRect bounds, NSTextContainer container);

		/// <summary>The glyph index for the glyph at <paramref name="point" />, in the <paramref name="container" /> object's coordinate system.</summary>
		/// <param name="point">To be added.</param>
		/// <param name="container">To be added.</param>
		/// <param name="fractionOfDistanceThroughGlyph">To be added.</param>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("glyphIndexForPoint:inTextContainer:fractionOfDistanceThroughGlyph:")]
		nuint GetGlyphIndex (CGPoint point, NSTextContainer container, /* nullable CGFloat */ out nfloat fractionOfDistanceThroughGlyph);

		/// <summary>Developers should call <see cref="NSLayoutManager.GetGlyphIndex(CoreGraphics.CGPoint,NSTextContainer,out nfloat)" /> rather than this primitive method.</summary>
		/// <param name="point">To be added.</param>
		/// <param name="container">To be added.</param>
		/// <returns>To be added.</returns>
		/// <remarks>
		///   <para>This method is public for overriding purposes. Developers should call <see cref="NSLayoutManager.GetGlyphIndex(CGPoint,NSTextContainer,out nfloat)" />  rather than this primitive method.</para>
		/// </remarks>
		[Export ("glyphIndexForPoint:inTextContainer:")]
		nuint GetGlyphIndex (CGPoint point, NSTextContainer container);

		/// <summary>Developers should call <see cref="NSLayoutManager.GetGlyphIndex(CoreGraphics.CGPoint,NSTextContainer)" /> rather than this primitive function.</summary>
		/// <param name="point">To be added.</param>
		/// <param name="container">To be added.</param>
		/// <returns>To be added.</returns>
		/// <remarks>
		///   <para>This method is <c>public</c> for overriding purposes but is not intended for developers to call.</para>
		/// </remarks>
		[Export ("fractionOfDistanceThroughGlyphForPoint:inTextContainer:")]
		nfloat GetFractionOfDistanceThroughGlyph (CGPoint point, NSTextContainer container);

		// GetCharacterIndex (CGPoint, NSTextContainer, nullable CGFloat*)
		[Protected]
		[Export ("characterIndexForPoint:inTextContainer:fractionOfDistanceBetweenInsertionPoints:")]
		nuint GetCharacterIndex (CGPoint point, NSTextContainer container, IntPtr fractionOfDistanceBetweenInsertionPoints);

		/// <param name="point">To be added.</param>
		///         <param name="container">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Wrap ("GetCharacterIndex (point, container, IntPtr.Zero)")]
		nuint GetCharacterIndex (CGPoint point, NSTextContainer container);

		/// <param name="point">To be added.</param>
		/// <param name="container">To be added.</param>
		/// <param name="fractionOfDistanceBetweenInsertionPoints">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Sealed]
		[Export ("characterIndexForPoint:inTextContainer:fractionOfDistanceBetweenInsertionPoints:")]
		nuint GetCharacterIndex (CGPoint point, NSTextContainer container, out nfloat fractionOfDistanceBetweenInsertionPoints);

		[Protected]
		[Export ("getLineFragmentInsertionPointsForCharacterAtIndex:alternatePositions:inDisplayOrder:positions:characterIndexes:")]
		nuint GetLineFragmentInsertionPoints (nuint characterIndex, bool alternatePositions, bool inDisplayOrder, IntPtr positions, IntPtr characterIndexes);

		/* GetTemporaryAttributes (NSUInteger, nullable NSRangePointer) */

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Protected]
		[Export ("temporaryAttributesAtCharacterIndex:effectiveRange:")]
		NSDictionary<NSString, NSObject> GetTemporaryAttributes (nuint characterIndex, IntPtr effectiveCharacterRange);

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Wrap ("GetTemporaryAttributes (characterIndex, IntPtr.Zero)")]
		NSDictionary<NSString, NSObject> GetTemporaryAttributes (nuint characterIndex);

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Sealed]
		[Export ("temporaryAttributesAtCharacterIndex:effectiveRange:")]
		NSDictionary<NSString, NSObject> GetTemporaryAttributes (nuint characterIndex, out NSRange effectiveCharacterRange);

		[NoiOS, NoTV]
		[NoMacCatalyst]
		[Export ("setTemporaryAttributes:forCharacterRange:")]
		void SetTemporaryAttributes (NSDictionary attrs, NSRange charRange);

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Export ("addTemporaryAttributes:forCharacterRange:")]
		void AddTemporaryAttributes (NSDictionary<NSString, NSObject> attributes, NSRange characterRange);

		// This API can take an NSString or managed string, but some related API
		// takes a generic dictionary that can't use a managed string, so for symmetry
		// provide an NSString overload as well.
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Export ("removeTemporaryAttribute:forCharacterRange:")]
		void RemoveTemporaryAttribute (NSString attributeName, NSRange characterRange);

		/// <param name="attributeName">To be added.</param>
		///         <param name="characterRange">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[Sealed]
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Export ("removeTemporaryAttribute:forCharacterRange:")]
		void RemoveTemporaryAttribute (string attributeName, NSRange characterRange);

		/* GetTemporaryAttribute (NSString, NSUInteger, nullable NSRangePointer) */
		[Protected]
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Export ("temporaryAttribute:atCharacterIndex:effectiveRange:")]
		NSObject GetTemporaryAttribute (NSString attributeName, nuint characterIndex, /* nullable NSRangePointer */ IntPtr effectiveRange);

		[Wrap ("GetTemporaryAttribute (attributeName, characterIndex, IntPtr.Zero)")]
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		NSObject GetTemporaryAttribute (NSString attributeName, nuint characterIndex);

		[Sealed]
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Export ("temporaryAttribute:atCharacterIndex:effectiveRange:")]
		NSObject GetTemporaryAttribute (NSString attributeName, nuint characterIndex, /* nullable NSRangePointer */ out NSRange effectiveRange);

		/* GetTemporaryAttribute (NSString, NSUInteger, nullable NSRangePointer, NSRange) */

		[Protected]
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Export ("temporaryAttribute:atCharacterIndex:longestEffectiveRange:inRange:")]
		NSObject GetTemporaryAttribute (NSString attributeName, nuint characterIndex, /* nullable NSRangePointer */ IntPtr longestEffectiveRange, NSRange rangeLimit);

		[Wrap ("GetTemporaryAttribute (attributeName, characterIndex, IntPtr.Zero, rangeLimit)")]
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		NSObject GetTemporaryAttribute (NSString attributeName, nuint characterIndex, NSRange rangeLimit);

		[Sealed]
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Export ("temporaryAttribute:atCharacterIndex:longestEffectiveRange:inRange:")]
		NSObject GetTemporaryAttribute (NSString attributeName, nuint characterIndex, /* nullable NSRangePointer */ out NSRange longestEffectiveRange, NSRange rangeLimit);

		/* GetTemporaryAttributes (NSUInteger, nullable NSRangePointer, NSRange) */

		[Protected]
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Export ("temporaryAttributesAtCharacterIndex:longestEffectiveRange:inRange:")]
		NSDictionary<NSString, NSObject> GetTemporaryAttributes (nuint characterIndex, /* nullable NSRangePointer */ IntPtr longestEffectiveRange, NSRange rangeLimit);

		[Wrap ("GetTemporaryAttributes (characterIndex, IntPtr.Zero, rangeLimit)")]
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		NSDictionary<NSString, NSObject> GetTemporaryAttributes (nuint characterIndex, NSRange rangeLimit);

		[Sealed]
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Export ("temporaryAttributesAtCharacterIndex:longestEffectiveRange:inRange:")]
		NSDictionary<NSString, NSObject> GetTemporaryAttributes (nuint characterIndex, /* nullable NSRangePointer */ out NSRange longestEffectiveRange, NSRange rangeLimit);

		// This method can take an NSString or managed string, but some related API
		// takes a generic dictionary that can't use a managed string, so for symmetry
		// provide an NSString overload as well.
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Export ("addTemporaryAttribute:value:forCharacterRange:")]
		void AddTemporaryAttribute (NSString attributeName, NSObject value, NSRange characterRange);

		/// <param name="attributeName">To be added.</param>
		///         <param name="value">To be added.</param>
		///         <param name="characterRange">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[Sealed]
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Export ("addTemporaryAttribute:value:forCharacterRange:")]
		void AddTemporaryAttribute (string attributeName, NSObject value, NSRange characterRange);

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Export ("defaultLineHeightForFont:")]
		nfloat GetDefaultLineHeight (NSFont font);

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Export ("defaultBaselineOffsetForFont:")]
		nfloat GetDefaultBaselineOffset (NSFont font);

		[NullAllowed]
		[Export ("textStorage", ArgumentSemantic.Assign)]
		NSTextStorage TextStorage { get; set; }

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Deprecated (PlatformName.MacOSX, 10, 11)]
		[Export ("glyphGenerator", ArgumentSemantic.Retain)]
		NSGlyphGenerator GlyphGenerator { get; set; }

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Export ("typesetter", ArgumentSemantic.Retain)]
		NSTypesetter Typesetter { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		/// <summary>An instance of the UIKit.INSLayoutManagerDelegate model class which acts as the class delegate.</summary>
		///         <value>The instance of the UIKit.INSLayoutManagerDelegate model class</value>
		///         <remarks>
		///           <para>The delegate instance assigned to this object will be used to handle events or provide data on demand to this class.</para>
		///           <para>When setting the Delegate or WeakDelegate values events will be delivered to the specified instance instead of being delivered to the C#-style events</para>
		///           <para>This is the strongly typed version of the object, developers should use the WeakDelegate property instead if they want to merely assign a class derived from NSObject that has been decorated with [Export] attributes.</para>
		///         </remarks>
		[Wrap ("WeakDelegate")]
		INSLayoutManagerDelegate Delegate { get; set; }

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Export ("backgroundLayoutEnabled")]
		bool BackgroundLayoutEnabled { get; set; }

		[NoiOS]
		[NoTV]
		[Deprecated (PlatformName.MacOSX, 10, 11)]
		[NoMacCatalyst]
		[Export ("usesScreenFonts")]
		bool UsesScreenFonts { get; set; }

		/// <summary>Specifies whether normally-invisible characters such as whitespace should have visible glyphs.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("showsInvisibleCharacters")]
		bool ShowsInvisibleCharacters { get; set; }

		/// <summary>Specifies whether control characters should be shown or not.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("showsControlCharacters")]
		bool ShowsControlCharacters { get; set; }

		/// <summary>The hyphenation threshold.</summary>
		///         <value>A value in the range 0 to 1. 0 indicates hyphenation is off, 1.0 causes hyphenation to always be attempted.</value>
		///         <remarks>
		///           <para>Application developers should prefer to set this value to 0.0, because hyphenation is slow and consumes memory.</para>
		///         </remarks>
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Please use 'UsesDefaultHyphenation' or 'NSParagraphStyle.HyphenationFactor' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Please use 'UsesDefaultHyphenation' or 'NSParagraphStyle.HyphenationFactor' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Please use 'UsesDefaultHyphenation' or 'NSParagraphStyle.HyphenationFactor' instead.")]
		[NoMacCatalyst]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Please use 'UsesDefaultHyphenation' or 'NSParagraphStyle.HyphenationFactor' instead.")]
		[Export ("hyphenationFactor")]
#if MONOMAC
		float HyphenationFactor { get; set; } /* This is defined as float in AppKit headers. */
#else
		nfloat HyphenationFactor { get; set; } /* This is defined as CGFloat in UIKit headers. */
#endif

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Export ("defaultAttachmentScaling")]
		NSImageScaling DefaultAttachmentScaling { get; set; }

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Export ("typesetterBehavior")]
		NSTypesetterBehavior TypesetterBehavior { get; set; }

		/// <include file="../docs/api/UIKit/NSLayoutManager.xml" path="/Documentation/Docs[@DocId='P:UIKit.NSLayoutManager.AllowsNonContiguousLayout']/*" />
		[Export ("allowsNonContiguousLayout")]
		bool AllowsNonContiguousLayout { get; set; }

		/// <summary>Whether the <see cref="NSLayoutManager" /> should use the leading provided in the font.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Export ("usesFontLeading")]
		bool UsesFontLeading { get; set; }

		/// <summary>Draws background marks for the given glyph range.</summary>
		/// <param name="glyphsToShow">To be added.</param>
		/// <param name="origin">To be added.</param>
		/// <remarks>
		///   <para>Background marks include text background color, highlighting, and table backgrounds and borders. Application developers can override this function in subclasses to fully customize background drawing.</para>
		///   <para>
		///     <paramref name="glyphsToShow" /> must specify glyphs within a single <see cref="NSTextContainer" />.</para>
		/// </remarks>
		[Export ("drawBackgroundForGlyphRange:atPoint:")]
		void DrawBackground (NSRange glyphsToShow, CGPoint origin);

		/// <summary>Draws the specified glyph range.</summary>
		/// <param name="glyphsToShow">To be added.</param>
		/// <param name="origin">To be added.</param>
		/// <remarks>
		///   <para>This method causes glyph generation and layout, if needed.</para>
		///   <para>
		///     <paramref name="glyphsToShow" /> must specify glyphs within a single <see cref="NSTextContainer" />.</para>
		/// </remarks>
		[Export ("drawGlyphsForGlyphRange:atPoint:")]
		void DrawGlyphs (NSRange glyphsToShow, CGPoint origin);

		/// <param name="glyphRange">To be added.</param>
		/// <param name="glyphBuffer">To be added.</param>
		/// <param name="properties">To be added.</param>
		/// <param name="characterIndexBuffer">To be added.</param>
		/// <param name="bidiLevelBuffer">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Protected] // Class can be subclassed, and most methods can be overridden.
		[MacCatalyst (13, 1)]
		[Export ("getGlyphsInRange:glyphs:properties:characterIndexes:bidiLevels:")]
		nuint GetGlyphs (NSRange glyphRange, IntPtr glyphBuffer, IntPtr properties, IntPtr characterIndexBuffer, IntPtr bidiLevelBuffer);

		/// <param name="glyphIndex">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("propertyForGlyphAtIndex:")]
		NSGlyphProperty GetProperty (nuint glyphIndex);

		/// <param name="glyphIndex">To be added.</param>
		/// <param name="isValidIndex">To be added.</param>
		/// <summary>Retrieves the glyph as <paramref name="glyphIndex" />, setting <paramref name="isValidIndex" /> to <see langword="true" /> if the index is valid.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("CGGlyphAtIndex:isValidIndex:")]
		CGGlyph GetGlyph (nuint glyphIndex, out bool isValidIndex);

		/// <param name="glyphIndex">To be added.</param>
		/// <summary>Retrieves the glyph at <paramref name="glyphIndex" />.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>
		///   <para>Calling this method generates all glyphs up to and including the glyph at <paramref name="glyphIndex" />.</para>
		/// </remarks>
		[MacCatalyst (13, 1)]
		[Export ("CGGlyphAtIndex:")]
		CGGlyph GetGlyph (nuint glyphIndex);

		/// <param name="textStorage">To be added.</param>
		/// <param name="editMask">To be added.</param>
		/// <param name="newCharacterRange">To be added.</param>
		/// <param name="delta">To be added.</param>
		/// <param name="invalidatedCharacterRange">To be added.</param>
		/// <summary>Notifies the <see cref="NSLayoutManager" /> of an edit action.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("processEditingForTextStorage:edited:range:changeInLength:invalidatedRange:")]
		void ProcessEditing (NSTextStorage textStorage, NSTextStorageEditActions editMask, NSRange newCharacterRange, /* NSInteger */ nint delta, NSRange invalidatedCharacterRange);

		// This method can only be called from
		// NSLayoutManagerDelegate.ShouldGenerateGlyphs, and that method takes
		// the same IntPtr arguments as this one. This means that creating a
		// version of this method with nice(r) types (arrays instead of
		// IntPtr) is useless, since what the caller has is IntPtrs (from the
		// ShouldGenerateGlyphs parameters). We can revisit this if we ever
		// fix the generator to have support for C-style arrays.
		/// <param name="glyphs">To be added.</param>
		/// <param name="properties">To be added.</param>
		/// <param name="characterIndexes">To be added.</param>
		/// <param name="font">To be added.</param>
		/// <param name="glyphRange">To be added.</param>
		/// <summary>Specifies the initial glyphs and glyph properties for the specified character range.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("setGlyphs:properties:characterIndexes:font:forGlyphRange:")]
		void SetGlyphs (IntPtr glyphs, IntPtr properties, IntPtr characterIndexes, NSFont font, NSRange glyphRange);

		/// <param name="glyphIndex">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("truncatedGlyphRangeInLineFragmentForGlyphAtIndex:")]
		NSRange GetTruncatedGlyphRangeInLineFragment (nuint glyphIndex);

		/// <summary>Enumerate the line fragments intersecting with the specified glyph range.|Enumerate the line fragments intersecting with the specified glyph rane.</summary>
		/// <param name="glyphRange">To be added.</param>
		/// <param name="callback">To be added.</param>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("enumerateLineFragmentsForGlyphRange:usingBlock:")]
		void EnumerateLineFragments (NSRange glyphRange, NSTextLayoutEnumerateLineFragments callback);

		/// <summary>Enumerates the enclosing rectangles for the specified glyph range.</summary>
		/// <param name="glyphRange">To be added.</param>
		/// <param name="selectedRange">To be added.</param>
		/// <param name="textContainer">To be added.</param>
		/// <param name="callback">To be added.</param>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("enumerateEnclosingRectsForGlyphRange:withinSelectedGlyphRange:inTextContainer:usingBlock:")]
		void EnumerateEnclosingRects (NSRange glyphRange, NSRange selectedRange, NSTextContainer textContainer, NSTextLayoutEnumerateEnclosingRects callback);

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use the overload that takes 'nint glyphCount' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use the overload that takes 'nint glyphCount' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use the overload that takes 'nint glyphCount' instead.")]
		[NoMacCatalyst]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the overload that takes 'nint glyphCount' instead.")]
		[Protected] // Can be overridden
		[Export ("showCGGlyphs:positions:count:font:matrix:attributes:inContext:")]
		void ShowGlyphs (IntPtr glyphs, IntPtr positions, nuint glyphCount, NSFont font, CGAffineTransform textMatrix, NSDictionary attributes, CGContext graphicsContext);

		[MacCatalyst (13, 1)]
		[Protected] // Can be overridden
		[Export ("showCGGlyphs:positions:count:font:textMatrix:attributes:inContext:")]
		void ShowGlyphs (IntPtr glyphs, IntPtr positions, nint glyphCount, NSFont font, CGAffineTransform textMatrix, NSDictionary attributes, CGContext graphicsContext);

		// Unfortunately we can't provide a nicer API for this, because it uses C-style arrays.
		// And providing a nicer overload when it's only purpose is to be overridden is useless.
		[Advice ("This method should never be called, only overridden.")] // According to Apple's documentation
		[Protected]
		[Export ("fillBackgroundRectArray:count:forCharacterRange:color:")]
		void FillBackground (IntPtr rectArray, nuint rectCount, NSRange characterRange, NSColor color);

		/// <summary>Underlines the glyphs in <paramref name="glyphRange" />.</summary>
		/// <param name="glyphRange">The range of glyphs to be underlined.</param>
		/// <param name="underlineVal">The drawing style of the underline.</param>
		/// <param name="baselineOffset">The distance from the baseline to draw the underline.</param>
		/// <param name="lineRect">The line fragment rectangle containing <paramref name="glyphRange" />.</param>
		/// <param name="lineGlyphRange">All glyphs within <paramref name="lineRect" />.</param>
		/// <param name="containerOrigin">The origin of the <paramref name="lineRect" /> objects containing <see cref="NSTextContainer" />.</param>
		/// <remarks>
		///   <para>Developers should generally use the simpler <see cref="NSLayoutManager.Underline(Foundation.NSRange,Foundation.NSUnderlineStyle,CoreGraphics.CGRect,Foundation.NSRange,CoreGraphics.CGPoint)" /> method.</para>
		/// </remarks>
		[Export ("drawUnderlineForGlyphRange:underlineType:baselineOffset:lineFragmentRect:lineFragmentGlyphRange:containerOrigin:")]
		void DrawUnderline (NSRange glyphRange, NSUnderlineStyle underlineVal, nfloat baselineOffset, CGRect lineRect, NSRange lineGlyphRange, CGPoint containerOrigin);

		/// <summary>Underlines the glyphs in <paramref name="glyphRange" />.</summary>
		/// <param name="glyphRange">To be added.</param>
		/// <param name="underlineVal">To be added.</param>
		/// <param name="lineRect">To be added.</param>
		/// <param name="lineGlyphRange">To be added.</param>
		/// <param name="containerOrigin">To be added.</param>
		/// <remarks>To be added.</remarks>
		[Export ("underlineGlyphRange:underlineType:lineFragmentRect:lineFragmentGlyphRange:containerOrigin:")]
		void Underline (NSRange glyphRange, NSUnderlineStyle underlineVal, CGRect lineRect, NSRange lineGlyphRange, CGPoint containerOrigin);

		/// <include file="../docs/api/UIKit/NSLayoutManager.xml" path="/Documentation/Docs[@DocId='M:UIKit.NSLayoutManager.DrawStrikethrough(Foundation.NSRange,Foundation.NSUnderlineStyle,System.Runtime.InteropServices.NFloat,CoreGraphics.CGRect,Foundation.NSRange,CoreGraphics.CGPoint)']/*" />
		[Export ("drawStrikethroughForGlyphRange:strikethroughType:baselineOffset:lineFragmentRect:lineFragmentGlyphRange:containerOrigin:")]
		void DrawStrikethrough (NSRange glyphRange, NSUnderlineStyle strikethroughVal, nfloat baselineOffset, CGRect lineRect, NSRange lineGlyphRange, CGPoint containerOrigin);

		/// <summary>Draws a strikethrough through the glyphs at <paramref name="glyphRange" />.</summary>
		/// <param name="glyphRange">To be added.</param>
		/// <param name="strikethroughVal">To be added.</param>
		/// <param name="lineRect">To be added.</param>
		/// <param name="lineGlyphRange">To be added.</param>
		/// <param name="containerOrigin">To be added.</param>
		/// <remarks>To be added.</remarks>
		[Export ("strikethroughGlyphRange:strikethroughType:lineFragmentRect:lineFragmentGlyphRange:containerOrigin:")]
		void Strikethrough (NSRange glyphRange, NSUnderlineStyle strikethroughVal, CGRect lineRect, NSRange lineGlyphRange, CGPoint containerOrigin);

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Export ("showAttachmentCell:inRect:characterIndex:")]
		void ShowAttachmentCell (NSCell cell, CGRect rect, nuint characterIndex);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("limitsLayoutForSuspiciousContents")]
		bool LimitsLayoutForSuspiciousContents { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("usesDefaultHyphenation")]
		bool UsesDefaultHyphenation { get; set; }
	}

	[NoiOS]
	[NoTV]
	[NoMacCatalyst]
	[Category]
	[BaseType (typeof (NSLayoutManager))]
	interface NSLayoutManager_NSTextViewSupport {
		[Export ("rulerMarkersForTextView:paragraphStyle:ruler:")]
		NSRulerMarker [] GetRulerMarkers (NSTextView textView, NSParagraphStyle paragraphStyle, NSRulerView ruler);

		[return: NullAllowed]
		[Export ("rulerAccessoryViewForTextView:paragraphStyle:ruler:enabled:")]
		NSView GetRulerAccessoryView (NSTextView textView, NSParagraphStyle paragraphStyle, NSRulerView ruler, bool enabled);

		[Export ("layoutManagerOwnsFirstResponderInWindow:")]
		bool LayoutManagerOwnsFirstResponder (NSWindow window);

		[return: NullAllowed]
		[Export ("firstTextView", ArgumentSemantic.Assign)]
		NSTextView GetFirstTextView ();

		[return: NullAllowed]
		[Export ("textViewForBeginningOfSelection")]
		NSTextView GetTextViewForBeginningOfSelection ();
	}

	interface INSLayoutManagerDelegate { }

	/// <summary>A delegate object that exposes events for <see cref="NSLayoutManager" />s.</summary>
	/// <related type="externalDocumentation" href="https://developer.apple.com/documentation/uikit/nslayoutmanagerdelegate">Apple documentation for <c>NSLayoutManagerDelegate</c></related>
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	[MacCatalyst (13, 1)]
	interface NSLayoutManagerDelegate {
		/// <param name="sender">To be added.</param>
		///         <summary>Indicates that the NSLayoutManager has invalidated layout information (not glyph information).</summary>
		///         <remarks>To be added.</remarks>
		[Export ("layoutManagerDidInvalidateLayout:")]
		void DidInvalidatedLayout (NSLayoutManager sender);

		/// <param name="layoutManager">To be added.</param>
		///         <param name="textContainer">To be added.</param>
		///         <param name="layoutFinishedFlag">To be added.</param>
		///         <summary>Indicates that the specified NSLayoutManager has finished laying out text in the specified text container.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("layoutManager:didCompleteLayoutForTextContainer:atEnd:")]
		void DidCompleteLayout (NSLayoutManager layoutManager, [NullAllowed] NSTextContainer textContainer, bool layoutFinishedFlag);

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Export ("layoutManager:shouldUseTemporaryAttributes:forDrawingToScreen:atCharacterIndex:effectiveRange:")]
		[return: NullAllowed]
		NSDictionary<NSString, NSObject> ShouldUseTemporaryAttributes (NSLayoutManager layoutManager, NSDictionary<NSString, NSObject> temporaryAttributes, bool drawingToScreen, nuint characterIndex, ref NSRange effectiveCharacterRange);

		/// <param name="layoutManager">To be added.</param>
		///         <param name="glyphBuffer">To be added.</param>
		///         <param name="properties">To be added.</param>
		///         <param name="characterIndexes">To be added.</param>
		///         <param name="font">To be added.</param>
		///         <param name="glyphRange">To be added.</param>
		///         <summary>When overridden, allows the app developer to customize the initial glyph generation process.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("layoutManager:shouldGenerateGlyphs:properties:characterIndexes:font:forGlyphRange:")]
		nuint ShouldGenerateGlyphs (NSLayoutManager layoutManager, IntPtr glyphBuffer, IntPtr properties, IntPtr characterIndexes, NSFont font, NSRange glyphRange);

		/// <param name="layoutManager">To be added.</param>
		/// <param name="glyphIndex">To be added.</param>
		/// <param name="rect">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("layoutManager:lineSpacingAfterGlyphAtIndex:withProposedLineFragmentRect:")]
		nfloat GetLineSpacingAfterGlyph (NSLayoutManager layoutManager, nuint glyphIndex, CGRect rect);

		/// <param name="layoutManager">To be added.</param>
		/// <param name="glyphIndex">To be added.</param>
		/// <param name="rect">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("layoutManager:paragraphSpacingBeforeGlyphAtIndex:withProposedLineFragmentRect:")]
		nfloat GetParagraphSpacingBeforeGlyph (NSLayoutManager layoutManager, nuint glyphIndex, CGRect rect);

		/// <param name="layoutManager">To be added.</param>
		/// <param name="glyphIndex">To be added.</param>
		/// <param name="rect">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("layoutManager:paragraphSpacingAfterGlyphAtIndex:withProposedLineFragmentRect:")]
		nfloat GetParagraphSpacingAfterGlyph (NSLayoutManager layoutManager, nuint glyphIndex, CGRect rect);

		/// <param name="layoutManager">To be added.</param>
		/// <param name="action">To be added.</param>
		/// <param name="characterIndex">To be added.</param>
		/// <summary>The control character action for the control character at the specified index.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("layoutManager:shouldUseAction:forControlCharacterAtIndex:")]
		NSControlCharacterAction ShouldUseAction (NSLayoutManager layoutManager, NSControlCharacterAction action, nuint characterIndex);

		/// <param name="layoutManager">To be added.</param>
		/// <param name="characterIndex">To be added.</param>
		/// <summary>Whether a line should have a soft line break. Called frequently.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("layoutManager:shouldBreakLineByWordBeforeCharacterAtIndex:")]
		bool ShouldBreakLineByWordBeforeCharacter (NSLayoutManager layoutManager, nuint characterIndex);

		/// <param name="layoutManager">To be added.</param>
		/// <param name="characterIndex">To be added.</param>
		/// <summary>Whether a line should break with a hyphen at the specified point. Called frequently.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("layoutManager:shouldBreakLineByHyphenatingBeforeCharacterAtIndex:")]
		bool ShouldBreakLineByHyphenatingBeforeCharacter (NSLayoutManager layoutManager, nuint characterIndex);

		/// <param name="layoutManager">To be added.</param>
		/// <param name="glyphIndex">To be added.</param>
		/// <param name="textContainer">To be added.</param>
		/// <param name="proposedRect">To be added.</param>
		/// <param name="glyphPosition">To be added.</param>
		/// <param name="characterIndex">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("layoutManager:boundingBoxForControlGlyphAtIndex:forTextContainer:proposedLineFragment:glyphPosition:characterIndex:")]
		CGRect GetBoundingBox (NSLayoutManager layoutManager, nuint glyphIndex, NSTextContainer textContainer, CGRect proposedRect, CGPoint glyphPosition, nuint characterIndex);

		/// <param name="layoutManager">To be added.</param>
		///         <param name="textContainer">To be added.</param>
		///         <param name="oldSize">To be added.</param>
		///         <summary>The geometry of <paramref name="textContainer" /> changed from <paramref name="oldSize" />, and <paramref name="layoutManager" /> will invalidate the layout.</summary>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("layoutManager:textContainer:didChangeGeometryFromSize:")]
		void DidChangeGeometry (NSLayoutManager layoutManager, NSTextContainer textContainer, CGSize oldSize);

		/// <param name="layoutManager">To be added.</param>
		/// <param name="lineFragmentRect">To be added.</param>
		/// <param name="lineFragmentUsedRect">To be added.</param>
		/// <param name="baselineOffset">To be added.</param>
		/// <param name="textContainer">To be added.</param>
		/// <param name="glyphRange">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("layoutManager:shouldSetLineFragmentRect:lineFragmentUsedRect:baselineOffset:inTextContainer:forGlyphRange:")]
		bool ShouldSetLineFragmentRect (NSLayoutManager layoutManager, ref CGRect lineFragmentRect, ref CGRect lineFragmentUsedRect, ref nfloat baselineOffset, NSTextContainer textContainer, NSRange glyphRange);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface NSDiffableDataSourceSnapshot<SectionIdentifierType, ItemIdentifierType> : NSCopying
		where SectionIdentifierType : NSObject
		where ItemIdentifierType : NSObject {

		[Export ("numberOfItems")]
		nint NumberOfItems { get; }

		[Export ("numberOfSections")]
		nint NumberOfSections { get; }

		[Export ("sectionIdentifiers")]
		SectionIdentifierType [] SectionIdentifiers { get; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("reloadedSectionIdentifiers")]
		SectionIdentifierType [] ReloadedSectionIdentifiers { get; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("reloadedItemIdentifiers")]
		ItemIdentifierType [] ReloadedItemIdentifiers { get; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("reconfiguredItemIdentifiers")]
		ItemIdentifierType [] ReconfiguredItemIdentifiers { get; }

		[Export ("itemIdentifiers")]
		ItemIdentifierType [] ItemIdentifiers { get; }

		[Export ("numberOfItemsInSection:")]
		nint GetNumberOfItems (SectionIdentifierType sectionIdentifier);

		[Export ("itemIdentifiersInSectionWithIdentifier:")]
		ItemIdentifierType [] GetItemIdentifiersInSection (SectionIdentifierType sectionIdentifier);

		[Export ("sectionIdentifierForSectionContainingItemIdentifier:")]
		[return: NullAllowed]
		SectionIdentifierType GetSectionIdentifierForSection (ItemIdentifierType itemIdentifier);

		[Export ("indexOfItemIdentifier:")]
		nint GetIndex (ItemIdentifierType itemIdentifier);

		[Export ("indexOfSectionIdentifier:")]
		nint GetIndex (SectionIdentifierType sectionIdentifier);

		[Export ("appendItemsWithIdentifiers:")]
		void AppendItems (ItemIdentifierType [] identifiers);

		[Export ("appendItemsWithIdentifiers:intoSectionWithIdentifier:")]
		void AppendItems (ItemIdentifierType [] identifiers, SectionIdentifierType sectionIdentifier);

		[Export ("insertItemsWithIdentifiers:beforeItemWithIdentifier:")]
		void InsertItemsBefore (ItemIdentifierType [] identifiers, ItemIdentifierType itemIdentifier);

		[Export ("insertItemsWithIdentifiers:afterItemWithIdentifier:")]
		void InsertItemsAfter (ItemIdentifierType [] identifiers, ItemIdentifierType itemIdentifier);

		[Export ("deleteItemsWithIdentifiers:")]
		void DeleteItems (ItemIdentifierType [] identifiers);

		[Export ("deleteAllItems")]
		void DeleteAllItems ();

		[Export ("moveItemWithIdentifier:beforeItemWithIdentifier:")]
		void MoveItemBefore (ItemIdentifierType fromIdentifier, ItemIdentifierType toIdentifier);

		[Export ("moveItemWithIdentifier:afterItemWithIdentifier:")]
		void MoveItemAfter (ItemIdentifierType fromIdentifier, ItemIdentifierType toIdentifier);

		[Export ("reloadItemsWithIdentifiers:")]
		void ReloadItems (ItemIdentifierType [] identifiers);

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("reconfigureItemsWithIdentifiers:")]
		void ReconfigureItems (ItemIdentifierType [] identifiers);

		[Export ("appendSectionsWithIdentifiers:")]
		void AppendSections (SectionIdentifierType [] sectionIdentifiers);

		[Export ("insertSectionsWithIdentifiers:beforeSectionWithIdentifier:")]
		void InsertSectionsBefore (SectionIdentifierType [] sectionIdentifiers, SectionIdentifierType toSectionIdentifier);

		[Export ("insertSectionsWithIdentifiers:afterSectionWithIdentifier:")]
		void InsertSectionsAfter (SectionIdentifierType [] sectionIdentifiers, SectionIdentifierType toSectionIdentifier);

		[Export ("deleteSectionsWithIdentifiers:")]
		void DeleteSections (SectionIdentifierType [] sectionIdentifiers);

		[Export ("moveSectionWithIdentifier:beforeSectionWithIdentifier:")]
		void MoveSectionBefore (SectionIdentifierType fromSectionIdentifier, SectionIdentifierType toSectionIdentifier);

		[Export ("moveSectionWithIdentifier:afterSectionWithIdentifier:")]
		void MoveSectionAfter (SectionIdentifierType fromSectionIdentifier, SectionIdentifierType toSectionIdentifier);

		[Export ("reloadSectionsWithIdentifiers:")]
		void ReloadSections (SectionIdentifierType [] sectionIdentifiers);
	}

	/// <summary>A class that specifies paragraph-relevant attributes of an <see cref="Foundation.NSAttributedString" />.</summary>
	/// <remarks>
	///   <para>
	///     An immutable set of attributes associated with the display of an <see cref="Foundation.NSAttributedString" />.
	///     Important: the application developer must use the subtype <see cref="NSMutableParagraphStyle" /> if they modify
	///     the paragraph style after assignment to a <see cref="Foundation.NSAttributedString" />. Modifying an attribute
	///     of an assigned <see cref="NSParagraphStyle" /> may result in a program crash.
	///   </para>
	///   <para tool="threads">The members of this class can be used from a background thread.</para>
	/// </remarks>
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/Cocoa/Reference/ApplicationKit/Classes/NSParagraphStyle_Class/index.html">Apple documentation for <c>NSParagraphStyle</c></related>
	[ThreadSafe]
	[BaseType (typeof (NSObject))]
	[MacCatalyst (13, 1)]
	interface NSParagraphStyle : NSSecureCoding, NSMutableCopying {
		/// <summary>The distance, in points, between the bottom of one line fragment and the top of the next.</summary>
		///         <value>To be added.</value>
		///         <remarks>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="threads">This can be used from a background thread.</para>
		///         </remarks>
		[Export ("lineSpacing")]
		nfloat LineSpacing { get; [NotImplemented] set; }

		/// <summary>Distance, in points, after the paragraph.</summary>
		///         <value>To be added.</value>
		///         <remarks>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="threads">This can be used from a background thread.</para>
		///         </remarks>
		[Export ("paragraphSpacing")]
		nfloat ParagraphSpacing { get; [NotImplemented] set; }

		[Export ("alignment")]
		TextAlignment Alignment { get; [NotImplemented] set; }

		/// <summary>The indentation of the paragraph's lines, other than the first. (See <see cref="NSParagraphStyle.FirstLineHeadIndent" />.)</summary>
		/// <value>To be added.</value>
		/// <remarks>
		///   <para>(More documentation for this node is coming)</para>
		///   <para tool="threads">This can be used from a background thread.</para>
		/// </remarks>
		[Export ("headIndent")]
		nfloat HeadIndent { get; [NotImplemented] set; }

		/// <summary>The distance, in points, from the margin of a text container to the end of lines.</summary>
		///         <value>To be added.</value>
		///         <remarks>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="threads">This can be used from a background thread.</para>
		///         </remarks>
		[Export ("tailIndent")]
		nfloat TailIndent { get; [NotImplemented] set; }

		/// <summary>The indentation of the paragraph's first line.</summary>
		///         <value>To be added.</value>
		///         <remarks>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="threads">This can be used from a background thread.</para>
		///         </remarks>
		[Export ("firstLineHeadIndent")]
		nfloat FirstLineHeadIndent { get; [NotImplemented] set; }

		/// <summary>The minimum height, in points, of lines in the paragraph.</summary>
		///         <value>To be added.</value>
		///         <remarks>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="threads">This can be used from a background thread.</para>
		///         </remarks>
		[Export ("minimumLineHeight")]
		nfloat MinimumLineHeight { get; [NotImplemented] set; }

		/// <summary>The paragraph's maximum line height, in points.</summary>
		///         <value>The default of 0 indicates no limit.</value>
		///         <remarks>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="threads">This can be used from a background thread.</para>
		///         </remarks>
		[Export ("maximumLineHeight")]
		nfloat MaximumLineHeight { get; [NotImplemented] set; }

		[Export ("lineBreakMode")]
		LineBreakMode LineBreakMode { get; [NotImplemented] set; }

		/// <summary>The normal writing direction.</summary>
		///         <value>To be added.</value>
		///         <remarks>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="threads">This can be used from a background thread.</para>
		///         </remarks>
		[Export ("baseWritingDirection")]
		NSWritingDirection BaseWritingDirection { get; [NotImplemented] set; }

		/// <summary>The natural line height of the paragraph is multiplied by this factor before constraint to minimum and maximum.</summary>
		///         <value>Default is 0.</value>
		///         <remarks>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="threads">This can be used from a background thread.</para>
		///         </remarks>
		[Export ("lineHeightMultiple")]
		nfloat LineHeightMultiple { get; [NotImplemented] set; }

		/// <summary>Distance, in points, between a paragraph's top and its first line.</summary>
		///         <value>To be added.</value>
		///         <remarks>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="threads">This can be used from a background thread.</para>
		///         </remarks>
		[Export ("paragraphSpacingBefore")]
		nfloat ParagraphSpacingBefore { get; [NotImplemented] set; }

		/// <summary>The paragraph's threshold for hyphenation.</summary>
		///         <value>Ranges from 0 to 1, indicating ratio of text width to the width of line fragment. Default is 0, indicating that the layout manager's hyphenation factor is used.</value>
		///         <remarks>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="threads">This can be used from a background thread.</para>
		///         </remarks>
		[Export ("hyphenationFactor")]
		float HyphenationFactor { get; [NotImplemented] set; } // Returns a float, not nfloat.

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("usesDefaultHyphenation")]
		bool UsesDefaultHyphenation { get; }

		/// <param name="languageName">To be added.</param>
		///         <summary>The default writing direction for the specified ISO language identifier.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="threads">This can be used from a background thread.</para>
		///         </remarks>
		[Static]
		[Export ("defaultWritingDirectionForLanguage:")]
		NSWritingDirection GetDefaultWritingDirection ([NullAllowed] string languageName);

		/// <summary>The default text style.</summary>
		///         <value>Defaults are: natural text alignment, 12 28pt left-aligned tabs, word-wrapping line breaks.</value>
		///         <remarks>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="threads">This can be used from a background thread.</para>
		///         </remarks>
		[Static]
		[Export ("defaultParagraphStyle", ArgumentSemantic.Copy)]
		NSParagraphStyle Default { get; }

		/// <summary>The value, in points, of tab intervals.</summary>
		///         <value>Default is 0.</value>
		///         <remarks>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="threads">This can be used from a background thread.</para>
		///         </remarks>
		[Export ("defaultTabInterval")]
		nfloat DefaultTabInterval { get; [NotImplemented] set; }

		/// <summary>The paragraph's tab stops, sorted by location.</summary>
		///         <value>To be added.</value>
		///         <remarks>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="threads">This can be used from a background thread.</para>
		///         </remarks>
		[Export ("tabStops", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSTextTab [] TabStops { get; [NotImplemented] set; }

		[MacCatalyst (13, 1)]
		[Export ("allowsDefaultTighteningForTruncation")]
		bool AllowsDefaultTighteningForTruncation { get; [NotImplemented] set; }

		[MacCatalyst (13, 1)]
		[Export ("textBlocks")]
		NSTextBlock [] TextBlocks { get; [NotImplemented] set; }

		[MacCatalyst (13, 1)]
		[Export ("textLists")]
		NSTextList [] TextLists { get; [NotImplemented] set; }

		[NoiOS, NoTV]
		[NoMacCatalyst]
		[Export ("tighteningFactorForTruncation")]
		float TighteningFactorForTruncation { get; [NotImplemented] set; } /* float, not CGFloat */

		[NoiOS, NoTV]
		[NoMacCatalyst]
		[Export ("headerLevel")]
		nint HeaderLevel { get; [NotImplemented] set; }

		[MacCatalyst (13, 1)]
		[Export ("lineBreakStrategy")]
		NSLineBreakStrategy LineBreakStrategy { get; [NotImplemented] set; }
	}

	/// <summary>A class that extends <see cref="NSParagraphStyle" /> to allow changing subattributes.</summary>
	/// <remarks>
	///   <para>
	///   </para>
	///   <para tool="threads">The members of this class can be used from a background thread.</para>
	/// </remarks>
	/// <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/Cocoa/Reference/ApplicationKit/Classes/NSMutableParagraphStyle_Class/index.html">Apple documentation for <c>NSMutableParagraphStyle</c></related>
	[ThreadSafe]
	[BaseType (typeof (NSParagraphStyle))]
	[MacCatalyst (13, 1)]
	interface NSMutableParagraphStyle {
		/// <summary>The distance, in points, between the bottom of one line fragment and the top of the next.</summary>
		///         <value>To be added.</value>
		///         <remarks>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="threads">This can be used from a background thread.</para>
		///         </remarks>
		[Export ("lineSpacing")]
		[Override]
		nfloat LineSpacing { get; set; }

		[Export ("alignment")]
		[Override]
		TextAlignment Alignment { get; set; }

		/// <summary>The indentation of the paragraph's lines, other than the first. (See <see cref="NSMutableParagraphStyle.FirstLineHeadIndent" />.)</summary>
		/// <value>To be added.</value>
		/// <remarks>
		///   <para>(More documentation for this node is coming)</para>
		///   <para tool="threads">This can be used from a background thread.</para>
		/// </remarks>
		[Export ("headIndent")]
		[Override]
		nfloat HeadIndent { get; set; }

		/// <summary>The distance, in points, from the margin of a text container to the end of lines.</summary>
		///         <value>To be added.</value>
		///         <remarks>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="threads">This can be used from a background thread.</para>
		///         </remarks>
		[Export ("tailIndent")]
		[Override]
		nfloat TailIndent { get; set; }

		/// <summary>The indentation of the paragraph's first line.</summary>
		///         <value>To be added.</value>
		///         <remarks>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="threads">This can be used from a background thread.</para>
		///         </remarks>
		[Export ("firstLineHeadIndent")]
		[Override]
		nfloat FirstLineHeadIndent { get; set; }

		/// <summary>The minimum height, in points, of lines in the paragraph.</summary>
		///         <value>To be added.</value>
		///         <remarks>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="threads">This can be used from a background thread.</para>
		///         </remarks>
		[Export ("minimumLineHeight")]
		[Override]
		nfloat MinimumLineHeight { get; set; }

		/// <summary>The paragraph's maximum line height, in points.</summary>
		///         <value>The default of 0 indicates no limit.</value>
		///         <remarks>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="threads">This can be used from a background thread.</para>
		///         </remarks>
		[Export ("maximumLineHeight")]
		[Override]
		nfloat MaximumLineHeight { get; set; }

		[Export ("lineBreakMode")]
		[Override]
		LineBreakMode LineBreakMode { get; set; }

		/// <summary>The norml writing direction.</summary>
		///         <value>To be added.</value>
		///         <remarks>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="threads">This can be used from a background thread.</para>
		///         </remarks>
		[Export ("baseWritingDirection")]
		[Override]
		NSWritingDirection BaseWritingDirection { get; set; }

		/// <summary>The natural line height of the paragraph is multiplied by this factor before constraint to minimum and maximum.</summary>
		///         <value>To be added.</value>
		///         <remarks>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="threads">This can be used from a background thread.</para>
		///         </remarks>
		[Export ("lineHeightMultiple")]
		[Override]
		nfloat LineHeightMultiple { get; set; }

		/// <summary>Distance, in points, after the paragraph.</summary>
		///         <value>To be added.</value>
		///         <remarks>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="threads">This can be used from a background thread.</para>
		///         </remarks>
		[Export ("paragraphSpacing")]
		[Override]
		nfloat ParagraphSpacing { get; set; }

		/// <summary>Distance, in points, between a paragraph's top and its first line.</summary>
		///         <value>To be added.</value>
		///         <remarks>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="threads">This can be used from a background thread.</para>
		///         </remarks>
		[Export ("paragraphSpacingBefore")]
		[Override]
		nfloat ParagraphSpacingBefore { get; set; }

		/// <summary>The paragraph's threshold for hyphenation.</summary>
		///         <value>Ranges from 0 to 1, indicating ratio of text width to the width of line fragment. Default is 0, indicating that the layout manager's hyphenation factor is used.</value>
		///         <remarks>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="threads">This can be used from a background thread.</para>
		///         </remarks>
		[Export ("hyphenationFactor")]
		[Override]
		float HyphenationFactor { get; set; } // Returns a float, not nfloat.

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("usesDefaultHyphenation")]
		bool UsesDefaultHyphenation { get; set; }

		/// <summary>The value, in points, of tab intervals.</summary>
		///         <value>The default is 0.</value>
		///         <remarks>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="threads">This can be used from a background thread.</para>
		///         </remarks>
		[Export ("defaultTabInterval")]
		[Override]
		nfloat DefaultTabInterval { get; set; }

		/// <summary>The paragraph's tab stops, sorted by location.</summary>
		///         <value>To be added.</value>
		///         <remarks>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="threads">This can be used from a background thread.</para>
		///         </remarks>
		[Export ("tabStops", ArgumentSemantic.Copy)]
		[Override]
		[NullAllowed]
		NSTextTab [] TabStops { get; set; }

		[MacCatalyst (13, 1)]
		[Override]
		[Export ("allowsDefaultTighteningForTruncation")]
		bool AllowsDefaultTighteningForTruncation { get; set; }

		/// <summary>Adds the specified <see cref="NSTextTab" /> to the pargraph style.</summary>
		/// <param name="textTab">To be added.</param>
		/// <remarks>
		///   <para>(More documentation for this node is coming)</para>
		///   <para tool="threads">This can be used from a background thread.</para>
		/// </remarks>
		[MacCatalyst (13, 1)]
		[Export ("addTabStop:")]
		void AddTabStop (NSTextTab textTab);

		/// <param name="textTab">To be added.</param>
		///         <summary>Removes the tab stop <paramref name="textTab" />.</summary>
		///         <remarks>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="threads">This can be used from a background thread.</para>
		///         </remarks>
		[MacCatalyst (13, 1)]
		[Export ("removeTabStop:")]
		void RemoveTabStop (NSTextTab textTab);

		/// <param name="paragraphStyle">To be added.</param>
		///         <summary>Replaces the existing style with <paramref name="paragraphStyle" />.</summary>
		///         <remarks>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="threads">This can be used from a background thread.</para>
		///         </remarks>
		[MacCatalyst (13, 1)]
		[Export ("setParagraphStyle:")]
		void SetParagraphStyle (NSParagraphStyle paragraphStyle);

		[MacCatalyst (13, 1)]
		[Override]
		[Export ("textBlocks")]
		NSTextBlock [] TextBlocks { get; set; }

		[MacCatalyst (13, 1)]
		[Override]
		[Export ("textLists")]
		NSTextList [] TextLists { get; set; }

		[NoiOS, NoTV]
		[NoMacCatalyst]
		[Export ("tighteningFactorForTruncation")]
		[Override]
		float TighteningFactorForTruncation { get; set; } /* float, not CGFloat */

		[NoiOS, NoTV]
		[NoMacCatalyst]
		[Export ("headerLevel")]
		[Override]
		nint HeaderLevel { get; set; }

		[MacCatalyst (13, 1)]
		[Override]
		[Export ("lineBreakStrategy", ArgumentSemantic.Assign)]
		NSLineBreakStrategy LineBreakStrategy { get; set; }
	}

	[MacCatalyst (13, 1)]
	delegate NSCollectionLayoutGroupCustomItem [] NSCollectionLayoutGroupCustomItemProvider (INSCollectionLayoutEnvironment layoutEnvironment);

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSCollectionLayoutItem))]
	[DisableDefaultCtor]
	interface NSCollectionLayoutGroup : NSCopying {

		[Static]
		[Export ("horizontalGroupWithLayoutSize:subitem:count:")]
		NSCollectionLayoutGroup CreateHorizontal (NSCollectionLayoutSize layoutSize, NSCollectionLayoutItem subitem, nint count);

		[Static]
		[Export ("horizontalGroupWithLayoutSize:subitems:")]
		NSCollectionLayoutGroup CreateHorizontal (NSCollectionLayoutSize layoutSize, params NSCollectionLayoutItem [] subitems);

		[Static]
		[Export ("verticalGroupWithLayoutSize:subitem:count:")]
		NSCollectionLayoutGroup CreateVertical (NSCollectionLayoutSize layoutSize, NSCollectionLayoutItem subitem, nint count);

		[Static]
		[Export ("verticalGroupWithLayoutSize:subitems:")]
		NSCollectionLayoutGroup CreateVertical (NSCollectionLayoutSize layoutSize, params NSCollectionLayoutItem [] subitems);

		[Static]
		[Export ("customGroupWithLayoutSize:itemProvider:")]
		NSCollectionLayoutGroup CreateCustom (NSCollectionLayoutSize layoutSize, NSCollectionLayoutGroupCustomItemProvider itemProvider);

		[Export ("supplementaryItems", ArgumentSemantic.Copy)]
		NSCollectionLayoutSupplementaryItem [] SupplementaryItems { get; set; }

		[NullAllowed, Export ("interItemSpacing", ArgumentSemantic.Copy)]
		NSCollectionLayoutSpacing InterItemSpacing { get; set; }

		[Export ("subitems")]
		NSCollectionLayoutItem [] Subitems { get; }

		[Export ("visualDescription")]
		string VisualDescription { get; }

		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Static]
		[Export ("horizontalGroupWithLayoutSize:repeatingSubitem:count:")]
		NSCollectionLayoutGroup GetHorizontalGroup (NSCollectionLayoutSize layoutSize, NSCollectionLayoutItem repeatingSubitem, nint count);

		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Static]
		[Export ("verticalGroupWithLayoutSize:repeatingSubitem:count:")]
		NSCollectionLayoutGroup GetVerticalGroup (NSCollectionLayoutSize layoutSize, NSCollectionLayoutItem repeatingSubitem, nint count);
	}

	[MacCatalyst (13, 1)]
	delegate void NSCollectionLayoutSectionVisibleItemsInvalidationHandler (INSCollectionLayoutVisibleItem [] visibleItems, CGPoint contentOffset, INSCollectionLayoutEnvironment layoutEnvironment);

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSCollectionLayoutSection : NSCopying {

		[Static]
		[Export ("sectionWithGroup:")]
		NSCollectionLayoutSection Create (NSCollectionLayoutGroup group);

		[Export ("contentInsets", ArgumentSemantic.Assign)]
		NSDirectionalEdgeInsets ContentInsets { get; set; }

		[Export ("interGroupSpacing")]
		nfloat InterGroupSpacing { get; set; }

		[NoMac]
		[MacCatalyst (14, 0)]
		[TV (14, 0), iOS (14, 0)]
		[Export ("contentInsetsReference", ArgumentSemantic.Assign)]
		UIContentInsetsReference ContentInsetsReference { get; set; }

		[Export ("orthogonalScrollingBehavior", ArgumentSemantic.Assign)]
		CollectionLayoutSectionOrthogonalScrollingBehavior OrthogonalScrollingBehavior { get; set; }

		[Export ("boundarySupplementaryItems", ArgumentSemantic.Copy)]
		NSCollectionLayoutBoundarySupplementaryItem [] BoundarySupplementaryItems { get; set; }

		[Deprecated (PlatformName.iOS, 16, 0)]
		[Deprecated (PlatformName.TvOS, 16, 0)]
		[Deprecated (PlatformName.MacCatalyst, 16, 0)]
		[Export ("supplementariesFollowContentInsets")]
		bool SupplementariesFollowContentInsets { get; set; }

		[NullAllowed, Export ("visibleItemsInvalidationHandler", ArgumentSemantic.Copy)]
		NSCollectionLayoutSectionVisibleItemsInvalidationHandler VisibleItemsInvalidationHandler { get; set; }

		[Export ("decorationItems", ArgumentSemantic.Copy)]
		NSCollectionLayoutDecorationItem [] DecorationItems { get; set; }

		// NSCollectionLayoutSection (UICollectionLayoutListSection) category
		[NoMac]
		[MacCatalyst (14, 0)]
		[TV (14, 0), iOS (14, 0)]
		[Static]
		[Export ("sectionWithListConfiguration:layoutEnvironment:")]
		NSCollectionLayoutSection GetSection (UICollectionLayoutListConfiguration listConfiguration, INSCollectionLayoutEnvironment layoutEnvironment);

		// NSCollectionLayoutSection (TVMediaItemContentConfiguration) category
		[TV (15, 0), NoMac, NoiOS, NoMacCatalyst]
		[Static]
		[Export ("orthogonalLayoutSectionForMediaItems")]
		NSCollectionLayoutSection GetOrthogonalLayoutSectionForMediaItems ();

		[TV (16, 0), iOS (16, 0), NoMac]
		[MacCatalyst (16, 0)]
		[Export ("supplementaryContentInsetsReference", ArgumentSemantic.Assign)]
		UIContentInsetsReference SupplementaryContentInsetsReference { get; set; }

		[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0), NoMac]
		[Export ("orthogonalScrollingProperties")]
		UICollectionLayoutSectionOrthogonalScrollingProperties OrthogonalScrollingProperties { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSCollectionLayoutGroupCustomItem : NSCopying {
		[Static]
		[Export ("customItemWithFrame:")]
		NSCollectionLayoutGroupCustomItem Create (CGRect frame);

		[Static]
		[Export ("customItemWithFrame:zIndex:")]
		NSCollectionLayoutGroupCustomItem Create (CGRect frame, nint zIndex);

		[Export ("frame")]
		CGRect Frame { get; }

		[Export ("zIndex")]
		nint ZIndex { get; }
	}

	interface INSCollectionLayoutContainer { }

	[MacCatalyst (13, 1)]
	[Protocol]
	interface NSCollectionLayoutContainer {
		[Abstract]
		[Export ("contentSize")]
		CGSize ContentSize { get; }

		[Abstract]
		[Export ("effectiveContentSize")]
		CGSize EffectiveContentSize { get; }

		[Abstract]
		[Export ("contentInsets")]
		NSDirectionalEdgeInsets ContentInsets { get; }

		[Abstract]
		[Export ("effectiveContentInsets")]
		NSDirectionalEdgeInsets EffectiveContentInsets { get; }
	}

	interface INSCollectionLayoutEnvironment { }

	[MacCatalyst (13, 1)]
	[Protocol]
	interface NSCollectionLayoutEnvironment {

		[Abstract]
		[Export ("container")]
		INSCollectionLayoutContainer Container { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Abstract]
		[Export ("traitCollection")]
		UITraitCollection TraitCollection { get; }
	}

	interface INSCollectionLayoutVisibleItem { }

	[MacCatalyst (13, 1)]
	[Protocol]
	interface NSCollectionLayoutVisibleItem
#if !MONOMAC
	: UIDynamicItem
#endif
	{

		[Abstract]
		[Export ("alpha")]
		nfloat Alpha { get; set; }

		[Abstract]
		[Export ("zIndex")]
		nint ZIndex { get; set; }

		[Abstract]
		[Export ("hidden")]
		bool Hidden { [Bind ("isHidden")] get; set; }

#pragma warning disable 0109 // warning CS0109: The member 'NSCollectionLayoutVisibleItem.Center' does not hide an accessible member. The new keyword is not required.
		// Inherited from UIDynamicItem for !MONOMAC
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		new CGPoint Center { get; set; }
#pragma warning restore

#pragma warning disable 0109 // warning CS0109: The member 'NSCollectionLayoutVisibleItem.Bounds' does not hide an accessible member. The new keyword is not required.
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Abstract]
		[Export ("bounds")]
		new CGRect Bounds { get; }
#pragma warning restore

		[NoMac]
		[MacCatalyst (13, 1)]
		[Abstract]
		[Export ("transform3D", ArgumentSemantic.Assign)]
		CATransform3D Transform3D { get; set; }

		[Abstract]
		[Export ("name")]
		string Name { get; }

		[Abstract]
		[Export ("indexPath")]
		NSIndexPath IndexPath { get; }

		[Abstract]
		[Export ("frame")]
		CGRect Frame { get; }

		[Abstract]
		[Export ("representedElementCategory")]
		CollectionElementCategory RepresentedElementCategory {
			get;
		}

		[Abstract]
		[NullAllowed, Export ("representedElementKind")]
		string RepresentedElementKind { get; }
	}

	/// <include file="../docs/api/UIKit/NSLayoutAnchor`1.xml" path="/Documentation/Docs[@DocId='T:UIKit.NSLayoutAnchor`1']/*" />
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // Handle is nil
	interface NSLayoutAnchor<AnchorType> : NSCopying, NSCoding {
		/// <summary>Creates a <see cref="NSLayoutConstraint" /> whose value is equal to that of the constraint of the <paramref name="anchor" />.</summary>
		/// <param name="anchor">The <see cref="NSLayoutAnchor{AnchorType}" /> whose constraint value should be copied.</param>
		/// <returns>A new <see cref="NSLayoutConstraint" />.</returns>
		/// <remarks>
		///   <para>As with other methods of this class, this method returns a new <see cref="NSLayoutConstraint" /> but does not add and activate it to the current view.</para>
		/// </remarks>
		[Export ("constraintEqualToAnchor:")]
		NSLayoutConstraint ConstraintEqualTo (NSLayoutAnchor<AnchorType> anchor);

		/// <summary>Creates a <see cref="NSLayoutConstraint" /> whose value is at least equal to that of the constraint of the <paramref name="anchor" />.</summary>
		/// <param name="anchor">The <see cref="NSLayoutAnchor{AnchorType}" /> whose constraint value should be used.</param>
		/// <returns>A new <see cref="NSLayoutConstraint" />.</returns>
		/// <remarks>
		///   <para>As with other methods of this class, this method returns a new <see cref="NSLayoutConstraint" /> but does not add and activate it to the current view.</para>
		/// </remarks>
		[Export ("constraintGreaterThanOrEqualToAnchor:")]
		NSLayoutConstraint ConstraintGreaterThanOrEqualTo (NSLayoutAnchor<AnchorType> anchor);

		/// <summary>Creates a <see cref="NSLayoutConstraint" /> whose value is at most equal to that of the constraint of the <paramref name="anchor" />.</summary>
		/// <param name="anchor">The <see cref="NSLayoutAnchor{AnchorType}" /> whose constraint value should be used.</param>
		/// <returns>A new <see cref="NSLayoutConstraint" />.</returns>
		/// <remarks>
		///   <para>As with other methods of this class, this method returns a new <see cref="NSLayoutConstraint" /> but does not add and activate it to the current view />.</para>
		/// </remarks>
		[Export ("constraintLessThanOrEqualToAnchor:")]
		NSLayoutConstraint ConstraintLessThanOrEqualTo (NSLayoutAnchor<AnchorType> anchor);

		/// <summary>Creates a <see cref="NSLayoutConstraint" /> whose value is equal to that of the constraint of the <paramref name="anchor" /> plus <paramref name="constant" /> pixels.</summary>
		/// <param name="anchor">The <see cref="NSLayoutAnchor{AnchorType}" /> whose constraint value should be used.</param>
		/// <param name="constant">The number of logical pixels to add to the value of <paramref name="anchor" />.</param>
		/// <returns>A new <see cref="NSLayoutConstraint" />.</returns>
		/// <remarks>
		///   <para>As with other methods of this class, this method returns a new <see cref="NSLayoutConstraint" /> but does not add and activate it to the current view.</para>
		/// </remarks>
		[Export ("constraintEqualToAnchor:constant:")]
		NSLayoutConstraint ConstraintEqualTo (NSLayoutAnchor<AnchorType> anchor, nfloat constant);

		/// <summary>Creates a <see cref="NSLayoutConstraint" /> whose value is at least equal to that of the constraint of the <paramref name="anchor" /> plus <paramref name="constant" /> pixels.</summary>
		/// <param name="anchor">The <see cref="NSLayoutAnchor{AnchorType}" /> whose constraint value should be used.</param>
		/// <param name="constant">The number of logical pixels to add to the value of <paramref name="anchor" />.</param>
		/// <returns>A new <see cref="NSLayoutConstraint" />.</returns>
		/// <remarks>
		///   <para>As with other methods of this class, this method returns a new <see cref="NSLayoutConstraint" /> but does not add and activate it to the current view.</para>
		/// </remarks>
		[Export ("constraintGreaterThanOrEqualToAnchor:constant:")]
		NSLayoutConstraint ConstraintGreaterThanOrEqualTo (NSLayoutAnchor<AnchorType> anchor, nfloat constant);

		/// <summary>Creates a <see cref="NSLayoutConstraint" /> whose value is at most equal to that of the constraint of the <paramref name="anchor" /> plus <paramref name="constant" /> pixels.</summary>
		/// <param name="anchor">The <see cref="NSLayoutAnchor{AnchorType}" /> whose constraint value should be used.</param>
		/// <param name="constant">The number of logical pixels to add to the value of <paramref name="anchor" />.</param>
		/// <returns>A new <see cref="NSLayoutConstraint" />.</returns>
		/// <remarks>
		///   <para>As with other methods of this class, this method returns a new <see cref="NSLayoutConstraint" /> but does not add and activate it to the current view.</para>
		/// </remarks>
		[Export ("constraintLessThanOrEqualToAnchor:constant:")]
		NSLayoutConstraint ConstraintLessThanOrEqualTo (NSLayoutAnchor<AnchorType> anchor, nfloat constant);

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("name")]
		string Name { get; }

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[NullAllowed, Export ("item", ArgumentSemantic.Weak)]
		NSObject Item { get; }

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("hasAmbiguousLayout")]
		bool HasAmbiguousLayout { get; }

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("constraintsAffectingLayout")]
		NSLayoutConstraint [] ConstraintsAffectingLayout { get; }
	}

	/// <summary>An <see cref="NSLayoutAnchor{AnchorType}" /> whose methods create horizontal <see cref="NSLayoutConstraint" /> objects.</summary>
	/// <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/AppKit/Reference/NSLayoutXAxisAnchor/index.html">Apple documentation for <c>NSLayoutXAxisAnchor</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSLayoutAnchor<NSLayoutXAxisAnchor>))]
	[DisableDefaultCtor] // Handle is nil
	interface NSLayoutXAxisAnchor {
		/// <param name="otherAnchor">To be added.</param>
		///         <summary>Returns a layout dimension for the distance between the current anchor and <paramref name="otherAnchor" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("anchorWithOffsetToAnchor:")]
		NSLayoutDimension CreateAnchorWithOffset (NSLayoutXAxisAnchor otherAnchor);

		/// <param name="anchor">The reference anchor.</param>
		/// <param name="multiplier">The multiplier for the spacing.</param>
		/// <summary>Returns a constraint for the distance from the current anchor to the specified <paramref name="anchor" />, scaled by the specified <paramref name="multiplier" /> over system spacing.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("constraintEqualToSystemSpacingAfterAnchor:multiplier:")]
		NSLayoutConstraint ConstraintEqualToSystemSpacingAfterAnchor (NSLayoutXAxisAnchor anchor, nfloat multiplier);

		/// <param name="anchor">The reference anchor.</param>
		/// <param name="multiplier">The multiplier for the spacing.</param>
		/// <summary>Returns a constraint for at least the distance from the current anchor to the specified <paramref name="anchor" />, scaled by the specified <paramref name="multiplier" /> over system spacing.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("constraintGreaterThanOrEqualToSystemSpacingAfterAnchor:multiplier:")]
		NSLayoutConstraint ConstraintGreaterThanOrEqualToSystemSpacingAfterAnchor (NSLayoutXAxisAnchor anchor, nfloat multiplier);

		/// <param name="anchor">The reference anchor.</param>
		/// <param name="multiplier">The multiplier for the spacing.</param>
		/// <summary>Returns a constraint for at most the distance from the current anchor to the specified <paramref name="anchor" />, scaled by the specified <paramref name="multiplier" /> over system spacing.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("constraintLessThanOrEqualToSystemSpacingAfterAnchor:multiplier:")]
		NSLayoutConstraint ConstraintLessThanOrEqualToSystemSpacingAfterAnchor (NSLayoutXAxisAnchor anchor, nfloat multiplier);
	}

	/// <summary>An <see cref="NSLayoutAnchor{AnchorType}" /> whose methods create vertical <see cref="NSLayoutConstraint" /> objects.</summary>
	/// <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/AppKit/Reference/NSLayoutYAxisAnchor/index.html">Apple documentation for <c>NSLayoutYAxisAnchor</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSLayoutAnchor<NSLayoutYAxisAnchor>))]
	[DisableDefaultCtor] // Handle is nil
	interface NSLayoutYAxisAnchor {
		/// <param name="otherAnchor">To be added.</param>
		///         <summary>Returns a layout dimension for the distance between the current anchor and <paramref name="otherAnchor" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("anchorWithOffsetToAnchor:")]
		NSLayoutDimension CreateAnchorWithOffset (NSLayoutYAxisAnchor otherAnchor);

		/// <param name="anchor">The reference anchor.</param>
		/// <param name="multiplier">The multiplier for the spacing.</param>
		/// <summary>Returns a constraint for the distance from the current anchor to the specified <paramref name="anchor" />, scaled by the specified <paramref name="multiplier" /> over system spacing.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("constraintEqualToSystemSpacingBelowAnchor:multiplier:")]
		NSLayoutConstraint ConstraintEqualToSystemSpacingBelowAnchor (NSLayoutYAxisAnchor anchor, nfloat multiplier);

		/// <param name="anchor">The reference anchor.</param>
		/// <param name="multiplier">The multiplier for the spacing.</param>
		/// <summary>Returns a constraint for at least the distance from the current anchor to the specified <paramref name="anchor" />, scaled by the specified <paramref name="multiplier" /> over system spacing.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("constraintGreaterThanOrEqualToSystemSpacingBelowAnchor:multiplier:")]
		NSLayoutConstraint ConstraintGreaterThanOrEqualToSystemSpacingBelowAnchor (NSLayoutYAxisAnchor anchor, nfloat multiplier);

		/// <param name="anchor">The reference anchor.</param>
		/// <param name="multiplier">The multiplier for the spacing.</param>
		/// <summary>Returns a constraint for at most the distance from the current anchor to the specified <paramref name="anchor" />, scaled by the specified <paramref name="multiplier" /> over system spacing.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("constraintLessThanOrEqualToSystemSpacingBelowAnchor:multiplier:")]
		NSLayoutConstraint ConstraintLessThanOrEqualToSystemSpacingBelowAnchor (NSLayoutYAxisAnchor anchor, nfloat multiplier);
	}

	/// <summary>An <see cref="NSLayoutAnchor{AnchorType}" /> whose methods create dimensional <see cref="NSLayoutConstraint" /> objects.</summary>
	/// <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/AppKit/Reference/NSLayoutDimension_ClassReference/index.html">Apple documentation for <c>NSLayoutDimension</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSLayoutAnchor<NSLayoutDimension>))]
	[DisableDefaultCtor] // Handle is nil
	interface NSLayoutDimension {
		/// <summary>Creates a <see cref="NSLayoutConstraint" /> whose value is equal to that of the constraint of the <paramref name="constant" /> in logical pixels.</summary>
		/// <param name="constant">An <see cref="nfloat" /> specifying the desired constant value.</param>
		/// <returns>A new <see cref="NSLayoutConstraint" />.</returns>
		/// <remarks>
		///   <para>As with other methods of this class, this method returns a new <see cref="NSLayoutConstraint" /> but does not add and activate it to the current view.</para>
		/// </remarks>
		[Export ("constraintEqualToConstant:")]
		NSLayoutConstraint ConstraintEqualTo (nfloat constant);

		/// <summary>Creates a <see cref="NSLayoutConstraint" /> whose value is at least equal to that of the <paramref name="constant" />.</summary>
		/// <param name="constant">The number of logical pixels to add.</param>
		/// <returns>A new <see cref="NSLayoutConstraint" />.</returns>
		/// <remarks>
		///   <para>As with other methods of this class, this method returns a new <see cref="NSLayoutConstraint" /> but does not add and activate it to the current view.</para>
		/// </remarks>
		[Export ("constraintGreaterThanOrEqualToConstant:")]
		NSLayoutConstraint ConstraintGreaterThanOrEqualTo (nfloat constant);

		/// <summary>Creates a <see cref="NSLayoutConstraint" /> whose value is at at most <paramref name="constant" />.</summary>
		/// <param name="constant">The number of logical pixels to add.</param>
		/// <returns>A new <see cref="NSLayoutConstraint" />.</returns>
		/// <remarks>
		///   <para>As with other methods of this class, this method returns a new <see cref="NSLayoutConstraint" /> but does not add and activate it to the current view.</para>
		/// </remarks>
		[Export ("constraintLessThanOrEqualToConstant:")]
		NSLayoutConstraint ConstraintLessThanOrEqualTo (nfloat constant);

		/// <summary>Creates a <see cref="NSLayoutConstraint" /> whose value is equal to that of the constraint of the <paramref name="anchor" /> multiplied by <paramref name="multiplier" />.</summary>
		/// <param name="anchor">The <see cref="NSLayoutAnchor{AnchorType}" /> whose constraint value should be copied.</param>
		/// <param name="multiplier">The value by which to multiply the <paramref name="anchor" />.</param>
		/// <returns>A new <see cref="NSLayoutConstraint" />.</returns>
		/// <remarks>
		///   <para>As with other methods of this class, this method returns a new <see cref="NSLayoutConstraint" /> but does not add and activate it to the current view.</para>
		/// </remarks>
		[Export ("constraintEqualToAnchor:multiplier:")]
		NSLayoutConstraint ConstraintEqualTo (NSLayoutDimension anchor, nfloat multiplier);

		/// <summary>Creates a <see cref="NSLayoutConstraint" /> whose value is at least equal to that of the constraint of the <paramref name="anchor" /> multiplied by <paramref name="multiplier" />.</summary>
		/// <param name="anchor">The <see cref="NSLayoutAnchor{AnchorType}" /> whose constraint value should be copied.</param>
		/// <param name="multiplier">To be added.</param>
		/// <returns>A new <see cref="NSLayoutConstraint" />.</returns>
		/// <remarks>
		///   <para>As with other methods of this class, this method returns a new <see cref="NSLayoutConstraint" /> but does not add and activate it to the current view.</para>
		/// </remarks>
		[Export ("constraintGreaterThanOrEqualToAnchor:multiplier:")]
		NSLayoutConstraint ConstraintGreaterThanOrEqualTo (NSLayoutDimension anchor, nfloat multiplier);

		/// <summary>Creates a <see cref="NSLayoutConstraint" /> whose value is at most equal to that of the constraint of the <paramref name="anchor" /> times <paramref name="multiplier" />.</summary>
		/// <param name="anchor">The <see cref="NSLayoutAnchor{AnchorType}" /> whose constraint value should be copied.</param>
		/// <param name="multiplier">To be added.</param>
		/// <returns>A new <see cref="NSLayoutConstraint" />.</returns>
		/// <remarks>
		///   <para>As with other methods of this class, this method returns a new <see cref="NSLayoutConstraint" /> but does not add and activate it to the current view.</para>
		/// </remarks>
		[Export ("constraintLessThanOrEqualToAnchor:multiplier:")]
		NSLayoutConstraint ConstraintLessThanOrEqualTo (NSLayoutDimension anchor, nfloat multiplier);

		/// <summary>Creates a <see cref="NSLayoutConstraint" /> whose value is equal to that of the constraint of the <paramref name="anchor" /> multiplied by <paramref name="multiplier" /> plus <paramref name="constant" /> pixels.</summary>
		/// <param name="anchor">The <see cref="NSLayoutAnchor{AnchorType}" /> whose constraint value should be copied.</param>
		/// <param name="multiplier">The value by which to multiply the <paramref name="anchor" />.</param>
		/// <param name="constant">The number of logical pixels to add to the value of <paramref name="anchor" />.</param>
		/// <returns>A new <see cref="NSLayoutConstraint" />.</returns>
		/// <remarks>
		///   <para>As with other methods of this class, this method returns a new <see cref="NSLayoutConstraint" /> but does not add and activate it to the current view.</para>
		/// </remarks>
		[Export ("constraintEqualToAnchor:multiplier:constant:")]
		NSLayoutConstraint ConstraintEqualTo (NSLayoutDimension anchor, nfloat multiplier, nfloat constant);

		/// <summary>Creates a <see cref="NSLayoutConstraint" /> whose value is at least equal to that of the constraint of the <paramref name="anchor" /> multiplied by <paramref name="multiplier" /> and adding <paramref name="constant" /> logical pixels.</summary>
		/// <param name="anchor">The <see cref="NSLayoutAnchor{AnchorType}" /> whose constraint value should be copied.</param>
		/// <param name="multiplier">To be added.</param>
		/// <param name="constant">The number of logical pixels to add.</param>
		/// <returns>A new <see cref="NSLayoutConstraint" />.</returns>
		/// <remarks>
		///   <para>As with other methods of this class, this method returns a new <see cref="NSLayoutConstraint" /> but does not add and activate it to the current view.</para>
		/// </remarks>
		[Export ("constraintGreaterThanOrEqualToAnchor:multiplier:constant:")]
		NSLayoutConstraint ConstraintGreaterThanOrEqualTo (NSLayoutDimension anchor, nfloat multiplier, nfloat constant);

		/// <summary>Creates a <see cref="NSLayoutConstraint" /> whose value is at most equal to that of the constraint of the <paramref name="anchor" /> times the <paramref name="multiplier" /> plus <paramref name="constant" /> logical pixels.</summary>
		/// <param name="anchor">The <see cref="NSLayoutAnchor{AnchorType}" /> whose constraint value should be copied.</param>
		/// <param name="multiplier">To be added.</param>
		/// <param name="constant">The number of logical pixels to add.</param>
		/// <returns>A new <see cref="NSLayoutConstraint" />.</returns>
		/// <remarks>
		///   <para>As with other methods of this class, this method returns a new <see cref="NSLayoutConstraint" /> but does not add and activate it to the current view.</para>
		/// </remarks>
		[Export ("constraintLessThanOrEqualToAnchor:multiplier:constant:")]
		NSLayoutConstraint ConstraintLessThanOrEqualTo (NSLayoutDimension anchor, nfloat multiplier, nfloat constant);
	}

	/// <include file="../docs/api/UIKit/NSLayoutConstraint.xml" path="/Documentation/Docs[@DocId='T:UIKit.NSLayoutConstraint']/*" />
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface NSLayoutConstraint
#if MONOMAC
		: NSAnimatablePropertyContainer
#endif
{
		/// <include file="../docs/api/UIKit/NSLayoutConstraint.xml" path="/Documentation/Docs[@DocId='M:UIKit.NSLayoutConstraint.FromVisualFormat(System.String,UIKit.NSLayoutFormatOptions,Foundation.NSDictionary,Foundation.NSDictionary)']/*" />
		[Static]
		[Export ("constraintsWithVisualFormat:options:metrics:views:")]
		NSLayoutConstraint [] FromVisualFormat (string format, NSLayoutFormatOptions formatOptions, [NullAllowed] NSDictionary metrics, NSDictionary views);

		/// <include file="../docs/api/UIKit/NSLayoutConstraint.xml" path="/Documentation/Docs[@DocId='M:UIKit.NSLayoutConstraint.Create(ObjCRuntime.INativeObject,UIKit.NSLayoutAttribute,UIKit.NSLayoutRelation,ObjCRuntime.INativeObject,UIKit.NSLayoutAttribute,System.Runtime.InteropServices.NFloat,System.Runtime.InteropServices.NFloat)']/*" />
		[Static]
		[Export ("constraintWithItem:attribute:relatedBy:toItem:attribute:multiplier:constant:")]
		NSLayoutConstraint Create (INativeObject view1, NSLayoutAttribute attribute1, NSLayoutRelation relation, [NullAllowed] INativeObject view2, NSLayoutAttribute attribute2, nfloat multiplier, nfloat constant);

		/// <summary>The priority of the constraint. Must be in range [0, UILayoutPriority.Required].</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("priority")]
		float Priority { get; set; } // Returns a float, not nfloat.

		/// <summary>Whether the constraint should be archived by its owning UIView.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("shouldBeArchived")]
		bool ShouldBeArchived { get; set; }

		/// <summary>The first item participating in the constraint.</summary>
		///         <value>
		///           <para />
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>
		///           <para />
		///         </remarks>
		[NullAllowed, Export ("firstItem", ArgumentSemantic.Assign)]
		NSObject FirstItem { get; }

		/// <summary>The attribute of the first item participating in the constraint.</summary>
		///         <value>
		///           <para />
		///         </value>
		///         <remarks>
		///           <para />
		///         </remarks>
		[Export ("firstAttribute")]
		NSLayoutAttribute FirstAttribute { get; }

		/// <summary>The NSRelation that holds between the two items in the constraint.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("relation")]
		NSLayoutRelation Relation { get; }

		/// <summary>The second item participating in the constraint.</summary>
		///         <value>
		///           <para />
		///         </value>
		///         <remarks>
		///           <para />
		///         </remarks>
		[Export ("secondItem", ArgumentSemantic.Assign)]
		[NullAllowed]
		NSObject SecondItem { get; }

		/// <summary>The attribute of the second item participating in the constraint.</summary>
		///         <value>
		///           <para />
		///         </value>
		///         <remarks>
		///           <para />
		///         </remarks>
		[Export ("secondAttribute")]
		NSLayoutAttribute SecondAttribute { get; }

		/// <summary>Applied to the second attribute participating in the constraint.</summary>
		///         <value>
		///           <para />
		///         </value>
		///         <remarks>
		///           <para />
		///         </remarks>
		[Export ("multiplier")]
		nfloat Multiplier { get; }

		/// <summary>Indicates the constant float applied to the constraint.</summary>
		///         <value>
		///           <para />
		///         </value>
		///         <remarks>
		///           <para />
		///         </remarks>
		[Export ("constant")]
		nfloat Constant { get; set; }

		/// <summary>Controls whether the constraint is active.  Active constraints are used as part of the auto-layout process, those that are not are ignored.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("active")]
		bool Active { [Bind ("isActive")] get; set; }

		/// <include file="../docs/api/UIKit/NSLayoutConstraint.xml" path="/Documentation/Docs[@DocId='M:UIKit.NSLayoutConstraint.ActivateConstraints(UIKit.NSLayoutConstraint[])']/*" />
		[MacCatalyst (13, 1)]
		[Static, Export ("activateConstraints:")]
		void ActivateConstraints (NSLayoutConstraint [] constraints);

		/// <summary>Deactivates all of the constraints passed.</summary>
		/// <param name="constraints">Constraints to deactivate.</param>
		/// <remarks>This method has the same effect as setting the <see cref="NSLayoutConstraint.Active" /> property to <see langword="false" />.</remarks>
		[MacCatalyst (13, 1)]
		[Static, Export ("deactivateConstraints:")]
		void DeactivateConstraints (NSLayoutConstraint [] constraints);

		[MacCatalyst (13, 1)]
		[Export ("firstAnchor", ArgumentSemantic.Copy)]
		[Internal]
		IntPtr _FirstAnchor<AnchorType> ();

		[MacCatalyst (13, 1)]
		[Export ("secondAnchor", ArgumentSemantic.Copy)]
		[Internal]
		IntPtr _SecondAnchor<AnchorType> ();

		[NullAllowed, Export ("identifier")]
		string Identifier { get; set; }
	}

	/// <summary>Defines the relationship between <see cref="NSTextAttachment" />s and a <see cref="NSLayoutManager" />.</summary>
	/// <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/UIKit/Reference/NSTextAttachmentContainer_Protocol/index.html">Apple documentation for <c>NSTextAttachmentContainer</c></related>
	[Introduced (PlatformName.iOS)]
	[MacCatalyst (13, 1)]
	[Model]
	[Protocol]
	[BaseType (typeof (NSObject))]
	partial interface NSTextAttachmentContainer {
		/// <param name="bounds">To be added.</param>
		/// <param name="textContainer">To be added.</param>
		/// <param name="characterIndex">To be added.</param>
		/// <summary>Returns an image rendered in <paramref name="bounds" />.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Abstract]
		[Export ("imageForBounds:textContainer:characterIndex:")]
		[return: NullAllowed]
		Image GetImageForBounds (CGRect bounds, [NullAllowed] NSTextContainer textContainer, nuint characterIndex);

		/// <param name="textContainer">To be added.</param>
		/// <param name="proposedLineFragment">To be added.</param>
		/// <param name="glyphPosition">To be added.</param>
		/// <param name="characterIndex">To be added.</param>
		/// <summary>Returns the bounds of the text attachment.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Abstract]
		[Export ("attachmentBoundsForTextContainer:proposedLineFragment:glyphPosition:characterIndex:")]
		CGRect GetAttachmentBounds ([NullAllowed] NSTextContainer textContainer, CGRect proposedLineFragment, CGPoint glyphPosition, nuint characterIndex);
	}

	/// <summary>An attachment to a <see cref="Foundation.NSAttributedString" />.</summary>
	///     
	///     
	/// <related type="externalDocumentation" href="https://developer.apple.com/documentation/uikit/nstextattachment">Apple documentation for <c>NSTextAttachment</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	partial interface NSTextAttachment : NSTextAttachmentContainer, NSSecureCoding, NSTextAttachmentLayout
#if !MONOMAC
	, UIAccessibilityContentSizeCategoryImageAdjusting
#endif // !MONOMAC
	{
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Export ("initWithFileWrapper:")]
		NativeHandle Constructor (NSFileWrapper fileWrapper);

		/// <summary>Creates a new <see cref="NSTextAttachment" /> with the specified <paramref name="contentData" />.</summary>
		/// <param name="contentData">
		///   <para>To be added.</para>
		///   <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		/// </param>
		/// <param name="uti">
		///   <para>To be added.</para>
		///   <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		/// </param>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[DesignatedInitializer]
		[Export ("initWithData:ofType:")]
		[PostGet ("Contents")]
		NativeHandle Constructor ([NullAllowed] NSData contentData, [NullAllowed] string uti);

		/// <summary>The contents of the text attachment. Modification invalidates the Image property.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("contents", ArgumentSemantic.Retain)]
		NSData Contents { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("fileType", ArgumentSemantic.Retain)]
		string FileType { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("image", ArgumentSemantic.Retain)]
		Image Image { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("bounds")]
		CGRect Bounds { get; set; }

		/// <summary>The file wrapper associated with this NSTextAttachment.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("fileWrapper", ArgumentSemantic.Retain)]
		NSFileWrapper FileWrapper { get; set; }

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Export ("attachmentCell", ArgumentSemantic.Retain)]
		NSTextAttachmentCell AttachmentCell { get; set; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("textAttachmentWithImage:")]
		NSTextAttachment Create (Image image);

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("lineLayoutPadding")]
		nfloat LineLayoutPadding { get; set; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Static]
		[Export ("textAttachmentViewProviderClassForFileType:")]
		[return: NullAllowed]
		Class GetTextAttachmentViewProviderClass (string fileType);

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Static]
		[Export ("registerTextAttachmentViewProviderClass:forFileType:")]
		void RegisterViewProviderClass (Class textAttachmentViewProviderClass, string fileType);

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("allowsTextAttachmentView")]
		bool AllowsTextAttachmentView { get; set; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("usesTextAttachmentView")]
		bool UsesTextAttachmentView { get; }
	}

	[TV (15, 0), iOS (15, 0)]
	[MacCatalyst (15, 0)]
	[Protocol]
	interface NSTextAttachmentLayout {

		[MacCatalyst (15, 0)]
		[Abstract]
		[Export ("imageForBounds:attributes:location:textContainer:")]
		[return: NullAllowed]
		Image GetImageForBounds (CGRect bounds, NSDictionary<NSString, NSObject> attributes, INSTextLocation location, [NullAllowed] NSTextContainer textContainer);

		[MacCatalyst (15, 0)]
		[Abstract]
		[Export ("attachmentBoundsForAttributes:location:textContainer:proposedLineFragment:position:")]
		CGRect GetAttachmentBounds (NSDictionary<NSString, NSObject> attributes, INSTextLocation location, [NullAllowed] NSTextContainer textContainer, CGRect proposedLineFragment, CGPoint position);

		[MacCatalyst (15, 0)]
		[Abstract]
		[Export ("viewProviderForParentView:location:textContainer:")]
		[return: NullAllowed]
		NSTextAttachmentViewProvider GetViewProvider ([NullAllowed] View parentView, INSTextLocation location, [NullAllowed] NSTextContainer textContainer);
	}

	/// <include file="../docs/api/UIKit/NSTextStorage.xml" path="/Documentation/Docs[@DocId='T:UIKit.NSTextStorage']/*" />
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSMutableAttributedString), Delegates = new string [] { "Delegate" }, Events = new Type [] { typeof (NSTextStorageDelegate) })]
	partial interface NSTextStorage : NSSecureCoding {
		[Export ("initWithString:")]
		NativeHandle Constructor (string str);

		/// <summary>The NSLayoutManagers associated with this NSTextStorage. Read-only.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("layoutManagers")]
		NSLayoutManager [] LayoutManagers { get; }

		/// <param name="aLayoutManager">To be added.</param>
		///         <summary>Adds an NSLayoutManager to this NSTextStorage.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("addLayoutManager:")]
		[PostGet ("LayoutManagers")]
		void AddLayoutManager (NSLayoutManager aLayoutManager);

		/// <param name="aLayoutManager">To be added.</param>
		///         <summary>Removes an NSLayoutManager from this NSTextStorage's LayoutManagers collection.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("removeLayoutManager:")]
		[PostGet ("LayoutManagers")]
		void RemoveLayoutManager (NSLayoutManager aLayoutManager);

		/// <summary>The kinds of edits pending for this NSTextStorage.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("editedMask")]
		NSTextStorageEditActions EditedMask {
			get;
		}

		/// <summary>The range in this NSTextStorage in which pending changes have been made.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("editedRange")]
		NSRange EditedRange {
			get;
		}

		/// <summary>The change in length for the pending changes.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("changeInLength")]
		nint ChangeInLength {
			get;
		}

		[NullAllowed]
		[Export ("delegate", ArgumentSemantic.Assign)]
		NSObject WeakDelegate { get; set; }

		/// <summary>An instance of the UIKit.INSTextStorageDelegate model class which acts as the class delegate.</summary>
		///         <value>The instance of the UIKit.INSTextStorageDelegate model class</value>
		///         <remarks>
		///           <para>The delegate instance assigned to this object will be used to handle events or provide data on demand to this class.</para>
		///           <para>When setting the Delegate or WeakDelegate values events will be delivered to the specified instance instead of being delivered to the C#-style events</para>
		///           <para>This is the strongly typed version of the object, developers should use the WeakDelegate property instead if they want to merely assign a class derived from NSObject that has been decorated with [Export] attributes.</para>
		///         </remarks>
		[Wrap ("WeakDelegate")]
		INSTextStorageDelegate Delegate { get; set; }

		/// <param name="editedMask">To be added.</param>
		/// <param name="editedRange">To be added.</param>
		/// <param name="delta">To be added.</param>
		/// <summary>Indicates a change.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("edited:range:changeInLength:")]
		void Edited (NSTextStorageEditActions editedMask, NSRange editedRange, nint delta);

		/// <summary>Activates post-editing operations.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("processEditing")]
		void ProcessEditing ();

		/// <summary>Whether this NSTextStorage fixes attributes lazily. Read-only.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("fixesAttributesLazily")]
		bool FixesAttributesLazily { get; }

		/// <param name="range">To be added.</param>
		///         <summary>Invalidates attributes in the specified range.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("invalidateAttributesInRange:")]
		void InvalidateAttributes (NSRange range);

		/// <param name="range">To be added.</param>
		///         <summary>Ensures that attributes have been fixed in the given range.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("ensureAttributesAreFixedInRange:")]
		void EnsureAttributesAreFixed (NSRange range);

		[Notification, Field ("NSTextStorageWillProcessEditingNotification")]
		[Internal]
		NSString WillProcessEditingNotification { get; }

		[Notification, Field ("NSTextStorageDidProcessEditingNotification")]
		[Internal]
		NSString DidProcessEditingNotification { get; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed]
		[Export ("textStorageObserver", ArgumentSemantic.Weak)]
		INSTextStorageObserving TextStorageObserver { get; set; }
	}

	interface INSTextStorageDelegate { }

	/// <summary>A delegate object that provides events relating to processing editing for <see cref="NSTextStorage" />.</summary>
	/// <related type="externalDocumentation" href="https://developer.apple.com/documentation/uikit/nstextstoragedelegate">Apple documentation for <c>NSTextStorageDelegate</c></related>
	[MacCatalyst (13, 1)]
	[Model]
	[BaseType (typeof (NSObject))]
	[Protocol]
	partial interface NSTextStorageDelegate {
		/// <param name="notification">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use WillProcessEditing instead.")]
		[Export ("textStorageWillProcessEditing:")]
		void TextStorageWillProcessEditing (NSNotification notification);

		/// <param name="notification">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use DidProcessEditing instead.")]
		[Export ("textStorageDidProcessEditing:")]
		void TextStorageDidProcessEditing (NSNotification notification);

		/// <param name="textStorage">To be added.</param>
		/// <param name="editedMask">To be added.</param>
		/// <param name="editedRange">To be added.</param>
		/// <param name="delta">To be added.</param>
		/// <summary>Indicates that processing of the editing on the specified editedRange is about to start.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("textStorage:willProcessEditing:range:changeInLength:")]
		[EventArgs ("NSTextStorage", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the Delegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		void WillProcessEditing (NSTextStorage textStorage, NSTextStorageEditActions editedMask, NSRange editedRange, nint delta);

		/// <param name="textStorage">To be added.</param>
		/// <param name="editedMask">To be added.</param>
		/// <param name="editedRange">To be added.</param>
		/// <param name="delta">To be added.</param>
		/// <summary>Indicates that editing has completed for the specified editedRange.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("textStorage:didProcessEditing:range:changeInLength:")]
		[EventArgs ("NSTextStorage", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the Delegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		void DidProcessEditing (NSTextStorage textStorage, NSTextStorageEditActions editedMask, NSRange editedRange, nint delta);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSCollectionLayoutAnchor : NSCopying, INSCopying {
		[Static]
		[Export ("layoutAnchorWithEdges:")]
		NSCollectionLayoutAnchor Create (NSDirectionalRectEdge edges);

		[Static]
		[Export ("layoutAnchorWithEdges:absoluteOffset:")]
		NSCollectionLayoutAnchor CreateFromAbsoluteOffset (NSDirectionalRectEdge edges, CGPoint absoluteOffset);

		[Static]
		[Export ("layoutAnchorWithEdges:fractionalOffset:")]
		NSCollectionLayoutAnchor CreateFromFractionalOffset (NSDirectionalRectEdge edges, CGPoint fractionalOffset);

		[Export ("edges")]
		NSDirectionalRectEdge Edges { get; }

		[Export ("offset")]
		CGPoint Offset { get; }

		[Export ("isAbsoluteOffset")]
		bool IsAbsoluteOffset { get; }

		[Export ("isFractionalOffset")]
		bool IsFractionalOffset { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSCollectionLayoutDimension : NSCopying {
		[Static]
		[Export ("fractionalWidthDimension:")]
		NSCollectionLayoutDimension CreateFractionalWidth (nfloat fractionalWidth);

		[Static]
		[Export ("fractionalHeightDimension:")]
		NSCollectionLayoutDimension CreateFractionalHeight (nfloat fractionalHeight);

		[Static]
		[Export ("absoluteDimension:")]
		NSCollectionLayoutDimension CreateAbsolute (nfloat absoluteDimension);

		[Static]
		[Export ("estimatedDimension:")]
		NSCollectionLayoutDimension CreateEstimated (nfloat estimatedDimension);

		[Export ("isFractionalWidth")]
		bool IsFractionalWidth { get; }

		[Export ("isFractionalHeight")]
		bool IsFractionalHeight { get; }

		[Export ("isAbsolute")]
		bool IsAbsolute { get; }

		[Export ("isEstimated")]
		bool IsEstimated { get; }

		[Export ("dimension")]
		nfloat Dimension { get; }

		[TV (17, 0), iOS (17, 0), NoMac, MacCatalyst (17, 0)]
		[Static]
		[Export ("uniformAcrossSiblingsWithEstimate:")]
		NSCollectionLayoutDimension CreateUniformAcrossSiblings (nfloat estimatedDimension);

		[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0), NoMac]
		[Export ("isUniformAcrossSiblings")]
		bool IsUniformAcrossSiblings { get; }
	}


	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSCollectionLayoutSize : NSCopying {
		[Static]
		[Export ("sizeWithWidthDimension:heightDimension:")]
		NSCollectionLayoutSize Create (NSCollectionLayoutDimension width, NSCollectionLayoutDimension height);

		[Export ("widthDimension")]
		NSCollectionLayoutDimension WidthDimension { get; }

		[Export ("heightDimension")]
		NSCollectionLayoutDimension HeightDimension { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSCollectionLayoutSpacing : NSCopying {
		[Static]
		[Export ("flexibleSpacing:")]
		NSCollectionLayoutSpacing CreateFlexible (nfloat flexibleSpacing);

		[Static]
		[Export ("fixedSpacing:")]
		NSCollectionLayoutSpacing CreateFixed (nfloat fixedSpacing);

		[Export ("spacing")]
		nfloat Spacing { get; }

		[Export ("isFlexibleSpacing")]
		bool IsFlexibleSpacing { get; }

		[Export ("isFixedSpacing")]
		bool IsFixedSpacing { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSCollectionLayoutEdgeSpacing : NSCopying {
		[Static]
		[Export ("spacingForLeading:top:trailing:bottom:")]
		NSCollectionLayoutEdgeSpacing Create ([NullAllowed] NSCollectionLayoutSpacing leading, [NullAllowed] NSCollectionLayoutSpacing top, [NullAllowed] NSCollectionLayoutSpacing trailing, [NullAllowed] NSCollectionLayoutSpacing bottom);

		[NullAllowed, Export ("leading")]
		NSCollectionLayoutSpacing Leading { get; }

		[NullAllowed, Export ("top")]
		NSCollectionLayoutSpacing Top { get; }

		[NullAllowed, Export ("trailing")]
		NSCollectionLayoutSpacing Trailing { get; }

		[NullAllowed, Export ("bottom")]
		NSCollectionLayoutSpacing Bottom { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSCollectionLayoutItem))]
	[DisableDefaultCtor]
	interface NSCollectionLayoutSupplementaryItem : NSCopying {
		[Static]
		[Export ("supplementaryItemWithLayoutSize:elementKind:containerAnchor:")]
		NSCollectionLayoutSupplementaryItem Create (NSCollectionLayoutSize layoutSize, string elementKind, NSCollectionLayoutAnchor containerAnchor);

		[Static]
		[Export ("supplementaryItemWithLayoutSize:elementKind:containerAnchor:itemAnchor:")]
		NSCollectionLayoutSupplementaryItem Create (NSCollectionLayoutSize layoutSize, string elementKind, NSCollectionLayoutAnchor containerAnchor, NSCollectionLayoutAnchor itemAnchor);

		[Export ("zIndex")]
		nint ZIndex { get; set; }

		[Export ("elementKind")]
		string ElementKind { get; }

		[Export ("containerAnchor")]
		NSCollectionLayoutAnchor ContainerAnchor { get; }

		[NullAllowed, Export ("itemAnchor")]
		NSCollectionLayoutAnchor ItemAnchor { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSCollectionLayoutItem : NSCopying {
		[Static]
		[Export ("itemWithLayoutSize:")]
		NSCollectionLayoutItem Create (NSCollectionLayoutSize layoutSize);

		[Static]
		[Export ("itemWithLayoutSize:supplementaryItems:")]
		NSCollectionLayoutItem Create (NSCollectionLayoutSize layoutSize, params NSCollectionLayoutSupplementaryItem [] supplementaryItems);

		[Export ("contentInsets", ArgumentSemantic.Assign)]
		NSDirectionalEdgeInsets ContentInsets { get; set; }

		[NullAllowed, Export ("edgeSpacing", ArgumentSemantic.Copy)]
		NSCollectionLayoutEdgeSpacing EdgeSpacing { get; set; }

		[Export ("layoutSize")]
		NSCollectionLayoutSize LayoutSize { get; }

		[Export ("supplementaryItems")]
		NSCollectionLayoutSupplementaryItem [] SupplementaryItems { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSCollectionLayoutSupplementaryItem))]
	[DisableDefaultCtor]
	interface NSCollectionLayoutBoundarySupplementaryItem : NSCopying {
		[Static]
		[Export ("boundarySupplementaryItemWithLayoutSize:elementKind:alignment:")]
		NSCollectionLayoutBoundarySupplementaryItem Create (NSCollectionLayoutSize layoutSize, string elementKind, NSRectAlignment alignment);

		[Static]
		[Export ("boundarySupplementaryItemWithLayoutSize:elementKind:alignment:absoluteOffset:")]
		NSCollectionLayoutBoundarySupplementaryItem Create (NSCollectionLayoutSize layoutSize, string elementKind, NSRectAlignment alignment, CGPoint absoluteOffset);

		[Export ("extendsBoundary")]
		bool ExtendsBoundary { get; set; }

		[Export ("pinToVisibleBounds")]
		bool PinToVisibleBounds { get; set; }

		[Export ("alignment")]
		NSRectAlignment Alignment { get; }

		[Export ("offset")]
		CGPoint Offset { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSCollectionLayoutItem))]
	[DisableDefaultCtor]
	interface NSCollectionLayoutDecorationItem : NSCopying {
		[Static]
		[Export ("backgroundDecorationItemWithElementKind:")]
		NSCollectionLayoutDecorationItem Create (string elementKind);

		[Export ("zIndex")]
		nint ZIndex { get; set; }

		[Export ("elementKind")]
		string ElementKind { get; }
	}

	/// <include file="../docs/api/UIKit/NSDataAsset.xml" path="/Documentation/Docs[@DocId='T:UIKit.NSDataAsset']/*" />
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // - (instancetype)init NS_UNAVAILABLE;
	interface NSDataAsset : NSCopying {
		/// <summary>Returns the data specified in the <paramref name="name" /> folder's "Contents.json" file.</summary>
		/// <param name="name">The name of the dataset folder within the asset catalog, without the ".dataset" extension.</param>
		/// <remarks>
		///   <para>For instance, call <c>new NSDataAsset("FolderName")</c> for:</para>
		///   <para>
		///     <img href="~/xml/UIKit/_images/UIKit.NSDataAsset_FolderStructure.png" alt="Image showing the folder structure." />
		///   </para>
		/// </remarks>
		[Export ("initWithName:")]
		NativeHandle Constructor (string name);

		/// <summary>Returns the data specified in the <paramref name="name" /> folder's "Contents.json" file.</summary>
		/// <param name="name">The name of the dataset folder within the asset catalog, without the ".dataset" extension.</param>
		/// <param name="bundle">The bundle containing the asset catalog.</param>
		/// <remarks>
		///   <para>For instance, call <c>new NSDataAsset("FolderName", NSBundle.MainBundle)</c> for:</para>
		///   <para>
		///     <img href="~/xml/UIKit/_images/UIKit.NSDataAsset_FolderStructure.png" alt="Image showing the folder structure." />
		///   </para>
		/// </remarks>
		[Export ("initWithName:bundle:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string name, NSBundle bundle);

		[Export ("name")]
		string Name { get; }

		[Export ("data", ArgumentSemantic.Copy)]
		NSData Data { get; }

		/// <summary>The UTI of the <see cref="NSDataAsset.Data" />.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Export ("typeIdentifier")] // Uniform Type Identifier
		NSString TypeIdentifier { get; }
	}

	/// <summary>The visual attributes associated with a drop shadow.</summary>
	/// <remarks>
	///   <para>The <see cref="NSShadow" /> class encapsulates the visual attributes of a drop shadow.</para>
	/// </remarks>
	/// <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/Cocoa/Reference/ApplicationKit/Classes/NSShadow_Class/index.html">Apple documentation for <c>NSShadow</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	interface NSShadow : NSSecureCoding, NSCopying {
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("set")]
		void Set ();

		[Export ("shadowOffset", ArgumentSemantic.Assign)]
		CGSize ShadowOffset { get; set; }

		/// <summary>The radius of the shadow blur.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("shadowBlurRadius", ArgumentSemantic.Assign)]
		nfloat ShadowBlurRadius { get; set; }

#if MONOMAC
		[Export ("shadowColor", ArgumentSemantic.Copy)]
#else
		[Export ("shadowColor", ArgumentSemantic.Retain), NullAllowed]
#endif
		NSColor ShadowColor { get; set; }
	}

	/// <summary>Represents a tab location in Text Kit.</summary>
	///     
	/// <related type="externalDocumentation" href="https://developer.apple.com/documentation/uikit/nstexttab">Apple documentation for <c>NSTextTab</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface NSTextTab : NSSecureCoding, NSCopying {
		/// <param name="alignment">To be added.</param>
		/// <param name="location">To be added.</param>
		/// <param name="options">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[DesignatedInitializer]
		[Export ("initWithTextAlignment:location:options:")]
		[PostGet ("Options")]
		NativeHandle Constructor (TextAlignment alignment, nfloat location, NSDictionary options);

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("initWithType:location:")]
		NativeHandle Constructor (NSTextTabType type, nfloat location);

		[Export ("alignment")]
		TextAlignment Alignment { get; }

		[Export ("options")]
		NSDictionary Options { get; }

		[Export ("location")]
		nfloat Location { get; }

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("tabStopType")]
		NSTextTabType TabStopType { get; }

		/// <param name="locale">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <summary>The column terminators for the specified locale. Passing null returns the system locale.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("columnTerminatorsForLocale:")]
		NSCharacterSet GetColumnTerminators ([NullAllowed] NSLocale locale);

		/// <summary>Represents the value associated with the constant NSTabColumnTerminatorsAttributeName</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSTabColumnTerminatorsAttributeName")]
		NSString ColumnTerminatorsAttributeName { get; }
	}

	[MacCatalyst (13, 1)]
	[Protocol]
	// no [Model] since it's not exposed in any API
	// only NSTextContainer conforms to it but it's only queried by iOS itself
	interface NSTextLayoutOrientationProvider {
		/// <summary>To be added.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("layoutOrientation")]
		NSTextLayoutOrientation LayoutOrientation {
			get;
		}
	}

	/// <include file="../docs/api/UIKit/NSTextContainer.xml" path="/Documentation/Docs[@DocId='T:UIKit.NSTextContainer']/*" />
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	partial interface NSTextContainer : NSTextLayoutOrientationProvider, NSSecureCoding {
		[MacCatalyst (13, 1)]
		[DesignatedInitializer]
		[Export ("initWithSize:")]
		NativeHandle Constructor (CGSize size);

		[Deprecated (PlatformName.MacOSX, 10, 11, "Use 'new NSTextContainer (CGSize)' instead.")]
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("initWithContainerSize:"), Internal]
		[Sealed]
		IntPtr _InitWithContainerSize (CGSize size);

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("initWithSize:"), Internal]
		[Sealed]
		IntPtr _InitWithSize (CGSize size);

		[NullAllowed] // by default this property is null
		[Export ("layoutManager", ArgumentSemantic.Assign)]
		NSLayoutManager LayoutManager { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("size")]
		CGSize Size { get; set; }

		/// <summary>An array of <see cref="BezierPath" />s from which text will be excluded.</summary>
		/// <value>
		///   <para>(More documentation for this node is coming)</para>
		///   <para tool="nullallowed">This value can be <see langword="null" />.</para>
		/// </value>
		/// <remarks>
		///   <para>Exclusion paths are defined in the <see cref="NSTextContainer" />'s coordinate system (see <see cref="View.ConvertRectFromView(CoreGraphics.CGRect,View)" />).</para>
		/// </remarks>
		[MacCatalyst (13, 1)]
		[Export ("exclusionPaths", ArgumentSemantic.Copy)]
		BezierPath [] ExclusionPaths { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("lineBreakMode")]
		LineBreakMode LineBreakMode { get; set; }

		/// <summary>The amount, in points, by which text is inset within line fragment rectangles. Default is 5.0 points.</summary>
		///         <value>The default value is 5.0.</value>
		///         <remarks>To be added.</remarks>
		[Export ("lineFragmentPadding")]
		nfloat LineFragmentPadding { get; set; }

		/// <summary>The maximum number of lines that can be stored in the receiver.</summary>
		///         <value>The default value of 0 indicates no limit.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("maximumNumberOfLines")]
		nuint MaximumNumberOfLines { get; set; }

		/// <param name="proposedRect">To be added.</param>
		/// <param name="characterIndex">To be added.</param>
		/// <param name="baseWritingDirection">To be added.</param>
		/// <param name="remainingRect">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("lineFragmentRectForProposedRect:atIndex:writingDirection:remainingRect:")]
		CGRect GetLineFragmentRect (CGRect proposedRect, nuint characterIndex, NSWritingDirection baseWritingDirection, out CGRect remainingRect);

		/// <summary>Whether the <see cref="NSTextContainer" /> changes its <see cref="NSTextContainer.Size" /> as its associated <see cref="XTextView" /> is resized.</summary>
		[Export ("widthTracksTextView")]
		bool WidthTracksTextView { get; set; }

		/// <summary>Whether the <see cref="NSTextContainer" /> changes its <see cref="NSTextContainer.Size" /> as its associated <see cref="XTextView" /> is resized.</summary>
		[Export ("heightTracksTextView")]
		bool HeightTracksTextView { get; set; }

		/// <summary>Replaces the current <see cref="NSLayoutManager" />.</summary>
		/// <param name="newLayoutManager">The new <see cref="NSLayoutManager" />.</param>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("replaceLayoutManager:")]
		void ReplaceLayoutManager (NSLayoutManager newLayoutManager);

		/// <summary>Gets a Boolean value that tells whether the receiver's text container is a simply connected rectangular region that has the exact orientation of the text view.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("simpleRectangularTextContainer")]
		bool IsSimpleRectangularTextContainer { [Bind ("isSimpleRectangularTextContainer")] get; }

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Deprecated (PlatformName.MacOSX, 10, 11)]
		[Export ("containsPoint:")]
		bool ContainsPoint (CGPoint point);

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("textView", ArgumentSemantic.Weak)]
		NSTextView TextView { get; set; }

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use Size instead.")]
		[Export ("containerSize")]
		CGSize ContainerSize { get; set; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("textLayoutManager", ArgumentSemantic.Weak)]
		NSTextLayoutManager TextLayoutManager { get; }
	}

	/// <summary>String drawing extension methods for <see cref="Foundation.NSString" />.</summary>
	[ThreadSafe]
	[Category, BaseType (typeof (NSString))]
	interface NSExtendedStringDrawing {
		/// <param name="rect">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <param name="attributes">To be added.</param>
		/// <param name="context">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("drawWithRect:options:attributes:context:")]
		void WeakDrawString (CGRect rect, NSStringDrawingOptions options, [NullAllowed] NSDictionary attributes, [NullAllowed] NSStringDrawingContext context);

		/// <param name="rect">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <param name="attributes">To be added.</param>
		/// <param name="context">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Wrap ("WeakDrawString (This, rect, options, attributes.GetDictionary (), context)")]
		void DrawString (CGRect rect, NSStringDrawingOptions options, StringAttributes attributes, [NullAllowed] NSStringDrawingContext context);

		/// <param name="size">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <param name="attributes">To be added.</param>
		/// <param name="context">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("boundingRectWithSize:options:attributes:context:")]
		CGRect WeakGetBoundingRect (CGSize size, NSStringDrawingOptions options, [NullAllowed] NSDictionary attributes, [NullAllowed] NSStringDrawingContext context);

		/// <param name="size">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <param name="attributes">To be added.</param>
		/// <param name="context">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Wrap ("WeakGetBoundingRect (This, size, options, attributes.GetDictionary (), context)")]
		CGRect GetBoundingRect (CGSize size, NSStringDrawingOptions options, StringAttributes attributes, [NullAllowed] NSStringDrawingContext context);
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface NSTextLayoutManagerDelegate {
		[Export ("textLayoutManager:textLayoutFragmentForLocation:inTextElement:")]
		NSTextLayoutFragment GetTextLayoutFragment (NSTextLayoutManager textLayoutManager, INSTextLocation location, NSTextElement textElement);

		[Export ("textLayoutManager:shouldBreakLineBeforeLocation:hyphenating:")]
		bool ShouldBreakLineBeforeLocation (NSTextLayoutManager textLayoutManager, INSTextLocation location, bool hyphenating);

		[Export ("textLayoutManager:renderingAttributesForLink:atLocation:defaultAttributes:")]
		[return: NullAllowed]
		NSDictionary<NSString, NSObject> GetRenderingAttributes (NSTextLayoutManager textLayoutManager, NSObject link, INSTextLocation location, NSDictionary<NSString, NSObject> renderingAttributes);

		[TV (27, 0), iOS (27, 0), MacCatalyst (27, 0), Mac (27, 0)]
		[Export ("textLayoutManager:cacheTextAttachmentViewProvider:forTextAttachment:")]
		void CacheTextAttachmentViewProvider (NSTextLayoutManager textLayoutManager, NSTextAttachmentViewProvider viewProvider, NSTextAttachment textAttachment);

		[TV (27, 0), iOS (27, 0), MacCatalyst (27, 0), Mac (27, 0)]
		[Export ("textLayoutManager:retrieveCachedTextAttachmentViewProviderForTextAttachment:")]
		[return: NullAllowed]
		NSTextAttachmentViewProvider GetCachedTextAttachmentViewProvider (NSTextLayoutManager textLayoutManager, NSTextAttachment attachment);
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum NSTextLayoutManagerSegmentType : long {
		Standard = 0,
		Selection = 1,
		Highlight = 2,
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Flags]
	[Native]
	public enum NSTextLayoutManagerSegmentOptions : ulong {
		None = 0x0,
		RangeNotRequired = (1uL << 0),
		MiddleFragmentsExcluded = (1uL << 1),
		HeadSegmentExtended = (1uL << 2),
		TailSegmentExtended = (1uL << 3),
		UpstreamAffinity = (1uL << 4),
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Flags]
	[Native]
	public enum NSTextLayoutFragmentEnumerationOptions : ulong {
		None = 0x0,
		Reverse = (1uL << 0),
		EstimatesSize = (1uL << 1),
		EnsuresLayout = (1uL << 2),
		EnsuresExtraLineFragment = (1uL << 3),
	}

	interface INSTextLayoutManagerDelegate { }

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	delegate bool NSTextLayoutManagerEnumerateRenderingAttributesDelegate (NSTextLayoutManager textLayoutManager, NSDictionary<NSString, NSObject> attributes, NSTextRange textRange);

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	delegate bool NSTextLayoutManagerEnumerateTextSegmentsDelegate ([NullAllowed] NSTextRange textSegmentRange, CGRect textSegmentFrame, nfloat baselinePosition, NSTextContainer textContainer);

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[DesignatedDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface NSTextLayoutManager : NSSecureCoding, NSTextSelectionDataSource {
		[Wrap ("WeakDelegate")]
		[NullAllowed]
		INSTextLayoutManagerDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Export ("usesFontLeading")]
		bool UsesFontLeading { get; set; }

		[Export ("limitsLayoutForSuspiciousContents")]
		bool LimitsLayoutForSuspiciousContents { get; set; }

		[Export ("usesHyphenation")]
		bool UsesHyphenation { get; set; }

		[NullAllowed, Export ("textContentManager", ArgumentSemantic.Weak)]
		NSTextContentManager TextContentManager { get; }

		[Export ("replaceTextContentManager:")]
		void Replace (NSTextContentManager textContentManager);

		[NullAllowed, Export ("textContainer", ArgumentSemantic.Strong)]
		NSTextContainer TextContainer { get; set; }

		[Export ("usageBoundsForTextContainer")]
		CGRect UsageBoundsForTextContainer { get; }

		[Export ("textViewportLayoutController", ArgumentSemantic.Strong)]
		NSTextViewportLayoutController TextViewportLayoutController { get; }

		[NullAllowed, Export ("layoutQueue", ArgumentSemantic.Strong)]
		NSOperationQueue LayoutQueue { get; set; }

		[Export ("ensureLayoutForRange:")]
		void EnsureLayout (NSTextRange range);

		[Export ("ensureLayoutForBounds:")]
		void EnsureLayout (CGRect bounds);

		[Export ("invalidateLayoutForRange:")]
		void InvalidateLayout (NSTextRange range);

		[Export ("textLayoutFragmentForPosition:")]
		[return: NullAllowed]
		NSTextLayoutFragment GetTextLayoutFragment (CGPoint position);

		[Export ("textLayoutFragmentForLocation:")]
		[return: NullAllowed]
		NSTextLayoutFragment GetTextLayoutFragment (INSTextLocation location);

		[Export ("enumerateTextLayoutFragmentsFromLocation:options:usingBlock:")]
		[return: NullAllowed]
		INSTextLocation EnumerateTextLayoutFragments ([NullAllowed] INSTextLocation location, NSTextLayoutFragmentEnumerationOptions options, Func<NSTextLayoutFragment, bool> handler);

		[Export ("textSelections", ArgumentSemantic.Strong)]
		NSTextSelection [] TextSelections { get; set; }

		[Export ("textSelectionNavigation", ArgumentSemantic.Strong)]
		NSTextSelectionNavigation TextSelectionNavigation { get; set; }

		[Export ("enumerateRenderingAttributesFromLocation:reverse:usingBlock:")]
		void EnumerateRenderingAttributes (INSTextLocation location, bool reverse, NSTextLayoutManagerEnumerateRenderingAttributesDelegate handler);

		[Export ("setRenderingAttributes:forTextRange:")]
		void SetRenderingAttributes (NSDictionary<NSString, NSObject> renderingAttributes, NSTextRange textRange);

		[Export ("addRenderingAttribute:value:forTextRange:")]
		void AddRenderingAttribute (string renderingAttribute, [NullAllowed] NSObject value, NSTextRange textRange);

		[Export ("removeRenderingAttribute:forTextRange:")]
		void RemoveRenderingAttribute (string renderingAttribute, NSTextRange textRange);

		[Export ("invalidateRenderingAttributesForTextRange:")]
		void InvalidateRenderingAttributes (NSTextRange textRange);

		[NullAllowed, Export ("renderingAttributesValidator", ArgumentSemantic.Copy)]
		Action<NSTextLayoutManager, NSTextLayoutFragment> RenderingAttributesValidator { get; set; }

		[Static]
		[Export ("linkRenderingAttributes")]
		NSDictionary<NSString, NSObject> LinkRenderingAttributes { get; }

		[Export ("renderingAttributesForLink:atLocation:")]
		NSDictionary<NSString, NSObject> GetRenderingAttributes (NSObject link, INSTextLocation location);

		[Export ("enumerateTextSegmentsInRange:type:options:usingBlock:")]
		void EnumerateTextSegments (NSTextRange textRange, NSTextLayoutManagerSegmentType type, NSTextLayoutManagerSegmentOptions options, NSTextLayoutManagerEnumerateTextSegmentsDelegate handler);

		[Export ("replaceContentsInRange:withTextElements:")]
		void ReplaceContents (NSTextRange range, NSTextElement [] textElements);

		[Export ("replaceContentsInRange:withAttributedString:")]
		void ReplaceContents (NSTextRange range, NSAttributedString attributedString);

		[TV (26, 0), iOS (26, 0), MacCatalyst (26, 0), Mac (26, 0)]
		[Export ("resolvesNaturalAlignmentWithBaseWritingDirection")]
		bool ResolvesNaturalAlignmentWithBaseWritingDirection { get; set; }
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Flags]
	[Native]
	public enum NSTextContentManagerEnumerationOptions : ulong {
		None = 0x0,
		Reverse = (1uL << 0),
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface NSTextContentManagerDelegate {
		[Export ("textContentManager:textElementAtLocation:")]
		[return: NullAllowed]
		NSTextElement GetTextContentManager (NSTextContentManager textContentManager, INSTextLocation location);

		[Export ("textContentManager:shouldEnumerateTextElement:options:")]
		bool ShouldEnumerateTextElement (NSTextContentManager textContentManager, NSTextElement textElement, NSTextContentManagerEnumerationOptions options);
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Protocol]
	interface NSTextElementProvider {
		[Abstract]
		[Export ("documentRange", ArgumentSemantic.Strong)]
		NSTextRange DocumentRange { get; }

		[Abstract]
		[Export ("enumerateTextElementsFromLocation:options:usingBlock:")]
		[return: NullAllowed]
		INSTextLocation EnumerateTextElements ([NullAllowed] INSTextLocation textLocation, NSTextContentManagerEnumerationOptions options, Func<NSTextElement, bool> handler);

		[Abstract]
		[Export ("replaceContentsInRange:withTextElements:")]
		void ReplaceContents (NSTextRange range, [NullAllowed] NSTextElement [] textElements);

		[Abstract]
		[Export ("synchronizeToBackingStore:")]
		void Synchronize ([NullAllowed] Action<NSError?> completionHandler);

		[Export ("locationFromLocation:withOffset:")]
		[return: NullAllowed]
		INSTextLocation GetLocation (INSTextLocation location, nint offset);

		[Export ("offsetFromLocation:toLocation:")]
		nint GetOffset (INSTextLocation from, INSTextLocation to);

		[Export ("adjustedRangeFromRange:forEditingTextSelection:")]
		[return: NullAllowed]
		NSTextRange AdjustedRange (NSTextRange textRange, bool forEditingTextSelection);
	}

	interface INSTextContentManagerDelegate { }

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSTextContentManager : NSTextElementProvider, NSSecureCoding {
		[Notification]
		[Field ("NSTextContentStorageUnsupportedAttributeAddedNotification")]
		NSString StorageUnsupportedAttributeAddedNotification { get; }

		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		INSTextContentManagerDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Export ("textLayoutManagers", ArgumentSemantic.Copy)]
		NSTextLayoutManager [] TextLayoutManagers { get; }

		[Export ("addTextLayoutManager:")]
		void Add (NSTextLayoutManager textLayoutManager);

		[Export ("removeTextLayoutManager:")]
		void Remove (NSTextLayoutManager textLayoutManager);

		[NullAllowed, Export ("primaryTextLayoutManager", ArgumentSemantic.Strong)]
		NSTextLayoutManager PrimaryTextLayoutManager { get; set; }

		[Async]
		[Export ("synchronizeTextLayoutManagers:")]
		void SynchronizeTextLayoutManagers ([NullAllowed] Action<NSError?> completionHandler);

		[Export ("textElementsForRange:")]
		NSTextElement [] GetTextElements (NSTextRange range);

		[Export ("hasEditingTransaction")]
		bool HasEditingTransaction { get; }

		[Async]
		[Export ("performEditingTransactionUsingBlock:")]
		void PerformEditingTransaction (Action transaction);

		[Export ("recordEditActionInRange:newTextRange:")]
		void RecordEditAction (NSTextRange originalTextRange, NSTextRange newTextRange);

		[Export ("automaticallySynchronizesTextLayoutManagers")]
		bool AutomaticallySynchronizesTextLayoutManagers { get; set; }

		[Export ("automaticallySynchronizesToBackingStore")]
		bool AutomaticallySynchronizesToBackingStore { get; set; }
	}

	interface INSTextLocation { }

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Protocol]
	interface NSTextLocation {
		[Abstract]
		[Export ("compare:")]
		NSComparisonResult Compare (INSTextLocation location);

		[Abstract]
		[Export ("isEqual:")]
		bool IsEqual ([NullAllowed] NSObject anObject);

		[Abstract]
		[Export ("hash")]
		nuint GetNativeHash ();
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	interface NSTextElement {
		[Export ("initWithTextContentManager:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] NSTextContentManager textContentManager);

		[NullAllowed, Export ("textContentManager", ArgumentSemantic.Weak)]
		NSTextContentManager TextContentManager { get; set; }

		[NullAllowed, Export ("elementRange", ArgumentSemantic.Strong)]
		NSTextRange ElementRange { get; set; }

		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("childElements", ArgumentSemantic.Copy)]
		NSTextElement [] ChildElements { get; }

		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[NullAllowed, Export ("parentElement", ArgumentSemantic.Weak)]
		NSTextElement ParentElement { get; }

		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("isRepresentedElement")]
		bool IsRepresentedElement { get; }
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSTextElement))]
	interface NSTextParagraph {
		[Export ("initWithAttributedString:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] NSAttributedString attributedString);

		[Export ("initWithTextContentManager:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] NSTextContentManager textContentManager);

		[Export ("attributedString", ArgumentSemantic.Strong)]
		NSAttributedString AttributedString { get; }

		[NullAllowed, Export ("paragraphContentRange", ArgumentSemantic.Strong)]
		NSTextRange ParagraphContentRange { get; }

		[NullAllowed, Export ("paragraphSeparatorRange", ArgumentSemantic.Strong)]
		NSTextRange ParagraphSeparatorRange { get; }
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSTextLineFragment : NSSecureCoding {
		[Export ("initWithAttributedString:range:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSAttributedString attributedString, NSRange range);

		[Export ("initWithString:attributes:range:")]
		NativeHandle Constructor (string @string, NSDictionary<NSString, NSObject> attributes, NSRange range);

		[Export ("attributedString", ArgumentSemantic.Strong)]
		NSAttributedString AttributedString { get; }

		[Export ("characterRange")]
		NSRange CharacterRange { get; }

		[Export ("typographicBounds")]
		CGRect TypographicBounds { get; }

		[Export ("glyphOrigin")]
		CGPoint GlyphOrigin { get; }

		[Export ("drawAtPoint:inContext:")]
		void Draw (CGPoint point, CGContext context);

		[Export ("locationForCharacterAtIndex:")]
		CGPoint GetLocation (nint characterIndex);

		[Export ("characterIndexForPoint:")]
		nint GetCharacterIndex (CGPoint point);

		[Export ("fractionOfDistanceThroughGlyphForPoint:")]
		nfloat GetFractionOfDistanceThroughGlyph (CGPoint point);
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum NSTextLayoutFragmentState : ulong {
		None = 0,
		EstimatedUsageBounds = 1,
		CalculatedUsageBounds = 2,
		LayoutAvailable = 3,
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSTextAttachmentViewProvider {
		[Export ("initWithTextAttachment:parentView:textLayoutManager:location:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSTextAttachment textAttachment, [NullAllowed] View parentView, [NullAllowed] NSTextLayoutManager textLayoutManager, INSTextLocation location);

		[NullAllowed, Export ("textAttachment", ArgumentSemantic.Weak)]
		NSTextAttachment TextAttachment { get; }

		[NullAllowed, Export ("textLayoutManager", ArgumentSemantic.Weak)]
		NSTextLayoutManager TextLayoutManager { get; }

		[Export ("location", ArgumentSemantic.Strong)]
		INSTextLocation Location { get; }

		[NullAllowed, Export ("view", ArgumentSemantic.Strong)]
		View View { get; set; }

		[Export ("loadView")]
		void LoadView ();

		[Export ("tracksTextAttachmentViewBounds")]
		bool TracksTextAttachmentViewBounds { get; set; }

		[Export ("attachmentBoundsForAttributes:location:textContainer:proposedLineFragment:position:")]
		CGRect GetAttachmentBounds (NSDictionary<NSString, NSObject> attributes, INSTextLocation location, [NullAllowed] NSTextContainer textContainer, CGRect proposedLineFragment, CGPoint position);
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSTextLayoutFragment : NSSecureCoding, NSTextViewportRenderingSurfaceKey {
		[Export ("initWithTextElement:range:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSTextElement textElement, [NullAllowed] NSTextRange rangeInElement);

		[NullAllowed, Export ("textLayoutManager", ArgumentSemantic.Weak)]
		NSTextLayoutManager TextLayoutManager { get; }

		[NullAllowed, Export ("textElement", ArgumentSemantic.Weak)]
		NSTextElement TextElement { get; }

		[Export ("rangeInElement", ArgumentSemantic.Strong)]
		NSTextRange RangeInElement { get; }

		[Export ("textLineFragments", ArgumentSemantic.Copy)]
		NSTextLineFragment [] TextLineFragments { get; }

		[NullAllowed, Export ("layoutQueue", ArgumentSemantic.Strong)]
		NSOperationQueue LayoutQueue { get; set; }

		[Export ("state")]
		NSTextLayoutFragmentState State { get; }

		[Export ("invalidateLayout")]
		void InvalidateLayout ();

		[Export ("layoutFragmentFrame")]
		CGRect LayoutFragmentFrame { get; }

		[Export ("renderingSurfaceBounds")]
		CGRect RenderingSurfaceBounds { get; }

		[Export ("leadingPadding")]
		nfloat LeadingPadding { get; }

		[Export ("trailingPadding")]
		nfloat TrailingPadding { get; }

		[Export ("topMargin")]
		nfloat TopMargin { get; }

		[Export ("bottomMargin")]
		nfloat BottomMargin { get; }

		[Export ("drawAtPoint:inContext:")]
		void Draw (CGPoint point, CGContext context);

		[Export ("textAttachmentViewProviders", ArgumentSemantic.Copy)]
		NSTextAttachmentViewProvider [] TextAttachmentViewProviders { get; }

		[Export ("frameForTextAttachmentAtLocation:")]
		CGRect GetFrameForTextAttachment (INSTextLocation location);

		[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("textLineFragmentForVerticalOffset:requiresExactMatch:")]
		[return: NullAllowed]
		NSTextLineFragment GetTextLineFragment (nfloat verticalOffset, bool requiresExactMatch);

		[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("textLineFragmentForTextLocation:isUpstreamAffinity:")]
		[return: NullAllowed]
		NSTextLineFragment GetTextLineFragment (INSTextLocation textLocation, bool isUpstreamAffinity);
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSTextRange {
		[Export ("initWithLocation:endLocation:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INSTextLocation location, [NullAllowed] INSTextLocation endLocation);

		[Export ("initWithLocation:")]
		NativeHandle Constructor (INSTextLocation location);

		[Export ("empty")]
		bool Empty { [Bind ("isEmpty")] get; }

		[Export ("location", ArgumentSemantic.Strong)]
		INSTextLocation Location { get; }

		[Export ("endLocation", ArgumentSemantic.Strong)]
		INSTextLocation EndLocation { get; }

		[Export ("isEqualToTextRange:")]
		bool IsEqual (NSTextRange textRange);

		[Export ("containsLocation:")]
		bool Contains (INSTextLocation location);

		[Export ("containsRange:")]
		bool Contains (NSTextRange textRange);

		[Export ("intersectsWithTextRange:")]
		bool Intersects (NSTextRange textRange);

		[Export ("textRangeByIntersectingWithTextRange:")]
		[return: NullAllowed]
		NSTextRange GetTextRangeByIntersecting (NSTextRange textRange);

		[Export ("textRangeByFormingUnionWithTextRange:")]
		NSTextRange GetTextRangeByFormingUnion (NSTextRange textRange);
	}

	interface INSTextViewportRenderingSurface { }

	[TV (27, 0), iOS (27, 0), MacCatalyst (27, 0), Mac (27, 0)]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface NSTextViewportRenderingSurface {
	}

	interface INSTextViewportRenderingSurfaceKey { }

	[TV (18, 0), iOS (18, 0), MacCatalyst (18, 0), Mac (15, 0)]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface NSTextViewportRenderingSurfaceKey {
	}

	interface INSTextViewportLayoutControllerDelegate { }

	[TV (15, 0), iOS (15, 0)]
	[MacCatalyst (15, 0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface NSTextViewportLayoutControllerDelegate {
		[Abstract]
		[Export ("viewportBoundsForTextViewportLayoutController:")]
		CGRect GetViewportBounds (NSTextViewportLayoutController textViewportLayoutController);

		[Abstract]
		[Export ("textViewportLayoutController:configureRenderingSurfaceForTextLayoutFragment:")]
		void ConfigureRenderingSurface (NSTextViewportLayoutController textViewportLayoutController, NSTextLayoutFragment textLayoutFragment);

		[Export ("textViewportLayoutControllerWillLayout:")]
		void WillLayout (NSTextViewportLayoutController textViewportLayoutController);

		[Export ("textViewportLayoutControllerDidLayout:")]
		void DidLayout (NSTextViewportLayoutController textViewportLayoutController);

		[TV (27, 0), iOS (27, 0), MacCatalyst (27, 0), Mac (27, 0)]
		[Export ("textViewportLayoutController:cacheRenderingSurface:forKey:")]
		void CacheRenderingSurface (NSTextViewportLayoutController textViewportLayoutController, INSTextViewportRenderingSurface renderingSurface, INSTextViewportRenderingSurfaceKey renderingSurfaceKey);

		[TV (27, 0), iOS (27, 0), MacCatalyst (27, 0), Mac (27, 0)]
		[Export ("textViewportLayoutController:retrieveCachedRenderingSurfaceForKey:")]
		INSTextViewportRenderingSurface GetCachedRenderingSurface (NSTextViewportLayoutController textViewportLayoutController, INSTextViewportRenderingSurfaceKey renderingSurfaceKey);

		[TV (27, 0), iOS (27, 0), MacCatalyst (27, 0), Mac (27, 0)]
		[Export ("textViewportLayoutControllerReceivedSetNeedsLayout:")]
		void ReceivedSetNeedsLayout (NSTextViewportLayoutController textViewportLayoutController);
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSTextViewportLayoutController {
		[Export ("initWithTextLayoutManager:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSTextLayoutManager textLayoutManager);

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		INSTextViewportLayoutControllerDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[NullAllowed, Export ("textLayoutManager", ArgumentSemantic.Weak)]
		NSTextLayoutManager TextLayoutManager { get; }

		[Export ("viewportBounds")]
		CGRect ViewportBounds { get; }

		[NullAllowed, Export ("viewportRange")]
		NSTextRange ViewportRange { get; }

		[Export ("layoutViewport")]
		void LayoutViewport ();

		[Export ("relocateViewportToTextLocation:")]
		nfloat RelocateViewport (INSTextLocation textLocation);

		[Export ("adjustViewportByVerticalOffset:")]
		void AdjustViewport (nfloat verticalOffset);
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum NSTextSelectionGranularity : long {
		Character,
		Word,
		Paragraph,
		Line,
		Sentence,
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum NSTextSelectionAffinity : long {
		Upstream = 0,
		Downstream = 1,
	}


	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSTextSelection : NSSecureCoding {
		[Export ("initWithRanges:affinity:granularity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSTextRange [] textRanges, NSTextSelectionAffinity affinity, NSTextSelectionGranularity granularity);

		[Export ("initWithRange:affinity:granularity:")]
		NativeHandle Constructor (NSTextRange range, NSTextSelectionAffinity affinity, NSTextSelectionGranularity granularity);

		[Export ("initWithLocation:affinity:")]
		NativeHandle Constructor (INSTextLocation location, NSTextSelectionAffinity affinity);

		[Export ("textRanges", ArgumentSemantic.Copy)]
		NSTextRange [] TextRanges { get; }

		[Export ("granularity")]
		NSTextSelectionGranularity Granularity { get; }

		[Export ("affinity")]
		NSTextSelectionAffinity Affinity { get; }

		[Export ("transient")]
		bool Transient { [Bind ("isTransient")] get; }

		[Export ("anchorPositionOffset")]
		nfloat AnchorPositionOffset { get; set; }

		[Export ("logical")]
		bool Logical { [Bind ("isLogical")] get; set; }

		[NullAllowed, Export ("secondarySelectionLocation", ArgumentSemantic.Strong)]
		INSTextLocation SecondarySelectionLocation { get; set; }

		[Export ("typingAttributes", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> TypingAttributes { get; set; }

		[Export ("textSelectionWithTextRanges:")]
		NSTextSelection GetTextSelection (NSTextRange [] textRanges);
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	delegate void NSTextSelectionDataSourceEnumerateSubstringsDelegate ([NullAllowed] NSString substring, NSTextRange substringRange, [NullAllowed] NSTextRange enclodingRange, out bool stop);

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	delegate void NSTextSelectionDataSourceEnumerateCaretOffsetsDelegate (nfloat caretOffset, INSTextLocation location, bool leadingEdge, out bool stop);

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	delegate void NSTextSelectionDataSourceEnumerateContainerBoundariesDelegate (INSTextLocation location, out bool stop);

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum NSTextSelectionNavigationLayoutOrientation : long {
		Horizontal = 0,
		Vertical = 1,
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum NSTextSelectionNavigationWritingDirection : long {
		LeftToRight = 0,
		RightToLeft = 1,
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface NSTextSelectionDataSource {
		[Abstract]
		[Export ("documentRange", ArgumentSemantic.Strong)]
		NSTextRange DocumentRange { get; }

		[Abstract]
		[Export ("enumerateSubstringsFromLocation:options:usingBlock:")]
		void EnumerateSubstrings (INSTextLocation location, NSStringEnumerationOptions options, NSTextSelectionDataSourceEnumerateSubstringsDelegate handler);

		[Abstract]
		[Export ("textRangeForSelectionGranularity:enclosingLocation:")]
		[return: NullAllowed]
		NSTextRange GetTextRange (NSTextSelectionGranularity selectionGranularity, INSTextLocation location);

		[Abstract]
		[Export ("locationFromLocation:withOffset:")]
		[return: NullAllowed]
		INSTextLocation GetLocation (INSTextLocation location, nint offset);

		[Abstract]
		[Export ("offsetFromLocation:toLocation:")]
		nint GetOffsetFromLocation (INSTextLocation from, INSTextLocation to);

		[Abstract]
		[Export ("baseWritingDirectionAtLocation:")]
		NSTextSelectionNavigationWritingDirection GetBaseWritingDirection (INSTextLocation location);

		[Abstract]
		[Export ("enumerateCaretOffsetsInLineFragmentAtLocation:usingBlock:")]
		void EnumerateCaretOffsets (INSTextLocation location, NSTextSelectionDataSourceEnumerateCaretOffsetsDelegate handler);

		[Abstract]
		[Export ("lineFragmentRangeForPoint:inContainerAtLocation:")]
		[return: NullAllowed]
		NSTextRange GetLineFragmentRange (CGPoint point, INSTextLocation location);

		[Export ("enumerateContainerBoundariesFromLocation:reverse:usingBlock:")]
		void EnumerateContainerBoundaries (INSTextLocation location, bool reverse, NSTextSelectionDataSourceEnumerateContainerBoundariesDelegate handler);

		[Export ("textLayoutOrientationAtLocation:")]
		NSTextSelectionNavigationLayoutOrientation GetTextLayoutOrientation (INSTextLocation location);

		[TV (27, 0), iOS (27, 0), MacCatalyst (27, 0), Mac (27, 0)]
		[Export ("convertInteractionPoint:toContainerAtLocation:")]
		CGPoint ConvertInteractionPoint (CGPoint point, INSTextLocation containerLocation);
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum NSTextSelectionNavigationDirection : long {
		Forward,
		Backward,
		Right,
		Left,
		Up,
		Down,
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum NSTextSelectionNavigationDestination : long {
		Character,
		Word,
		Line,
		Sentence,
		Paragraph,
		Container,
		Document,
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Flags]
	[Native]
	public enum NSTextSelectionNavigationModifier : ulong {
		Extend = (1uL << 0),
		Visual = (1uL << 1),
		Multiple = (1uL << 2),
	}

	interface INSTextSelectionDataSource { }

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSTextSelectionNavigation {
		[Export ("initWithDataSource:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INSTextSelectionDataSource dataSource);

		[Wrap ("WeakTextSelectionDataSource")]
		[NullAllowed]
		INSTextSelectionDataSource TextSelectionDataSource { get; }

		[NullAllowed, Export ("textSelectionDataSource", ArgumentSemantic.Weak)]
		NSObject WeakTextSelectionDataSource { get; }

		[Export ("allowsNonContiguousRanges")]
		bool AllowsNonContiguousRanges { get; set; }

		[Export ("rotatesCoordinateSystemForLayoutOrientation")]
		bool RotatesCoordinateSystemForLayoutOrientation { get; set; }

		[Export ("flushLayoutCache")]
		void FlushLayoutCache ();

		[Export ("destinationSelectionForTextSelection:direction:destination:extending:confined:")]
		[return: NullAllowed]
		NSTextSelection GetDestinationSelection (NSTextSelection textSelection, NSTextSelectionNavigationDirection direction, NSTextSelectionNavigationDestination destination, bool extending, bool confined);

		[Export ("textSelectionsInteractingAtPoint:inContainerAtLocation:anchors:modifiers:selecting:bounds:")]
		NSTextSelection [] GetTextSelectionsInteracting (CGPoint point, INSTextLocation containerLocation, NSTextSelection [] anchors, NSTextSelectionNavigationModifier modifiers, bool selecting, CGRect bounds);

		[Export ("textSelectionForSelectionGranularity:enclosingTextSelection:")]
		NSTextSelection GetTextSelection (NSTextSelectionGranularity selectionGranularity, NSTextSelection textSelection);

		[Export ("textSelectionForSelectionGranularity:enclosingPoint:inContainerAtLocation:")]
		[return: NullAllowed]
		NSTextSelection GetTextSelection (NSTextSelectionGranularity selectionGranularity, CGPoint point, INSTextLocation location);

		[Export ("resolvedInsertionLocationForTextSelection:writingDirection:")]
		[return: NullAllowed]
		INSTextLocation GetResolvedInsertionLocation (NSTextSelection textSelection, NSTextSelectionNavigationWritingDirection writingDirection);

		[Export ("deletionRangesForTextSelection:direction:destination:allowsDecomposition:")]
		NSTextRange [] GetDeletionRanges (NSTextSelection textSelection, NSTextSelectionNavigationDirection direction, NSTextSelectionNavigationDestination destination, bool allowsDecomposition);
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface NSTextContentStorageDelegate : NSTextContentManagerDelegate {
		[Export ("textContentStorage:textParagraphWithRange:")]
		[return: NullAllowed]
		NSTextParagraph GetTextParagraph (NSTextContentStorage textContentStorage, NSRange range);
	}

	interface INSTextContentStorageDelegate { }

	interface INSTextStorageObserving { }

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Protocol]
	interface NSTextStorageObserving {
		[Abstract]
		[NullAllowed, Export ("textStorage", ArgumentSemantic.Strong)]
		NSTextStorage TextStorage { get; set; }

		[Abstract]
		[Export ("processEditingForTextStorage:edited:range:changeInLength:invalidatedRange:")]
		void ProcessEditing (NSTextStorage textStorage, NSTextStorageEditActions editMask, NSRange newCharRange, nint delta, NSRange invalidatedCharRange);

		[Abstract]
		[Export ("performEditingTransactionForTextStorage:usingBlock:")]
		void PerformEditingTransaction (NSTextStorage textStorage, Action transaction);
	}

	[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
	enum NSTextListMarkerFormats {
		[DefaultEnumValue]
		[Field (null)]
		CustomString = -1,

		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		[Field ("NSTextListMarkerBox")]
		Box,

		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		[Field ("NSTextListMarkerCheck")]
		Check,

		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		[Field ("NSTextListMarkerCircle")]
		Circle,

		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		[Field ("NSTextListMarkerDiamond")]
		Diamond,

		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		[Field ("NSTextListMarkerDisc")]
		Disc,

		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		[Field ("NSTextListMarkerHyphen")]
		Hyphen,

		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		[Field ("NSTextListMarkerSquare")]
		Square,

		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		[Field ("NSTextListMarkerLowercaseHexadecimal")]
		LowercaseHexadecimal,

		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		[Field ("NSTextListMarkerUppercaseHexadecimal")]
		UppercaseHexadecimal,

		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		[Field ("NSTextListMarkerOctal")]
		Octal,

		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		[Field ("NSTextListMarkerLowercaseAlpha")]
		LowercaseAlpha,

		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		[Field ("NSTextListMarkerUppercaseAlpha")]
		UppercaseAlpha,

		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		[Field ("NSTextListMarkerLowercaseLatin")]
		LowercaseLatin,

		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		[Field ("NSTextListMarkerUppercaseLatin")]
		UppercaseLatin,

		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		[Field ("NSTextListMarkerLowercaseRoman")]
		LowercaseRoman,

		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		[Field ("NSTextListMarkerUppercaseRoman")]
		UppercaseRoman,

		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		[Field ("NSTextListMarkerDecimal")]
		Decimal,
	}

	[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[Flags]
	[Native]
	public enum NSTextListOptions : ulong {
		None = 0,
		/// <summary>To be added.</summary>
		PrependEnclosingMarker = 1,
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSTextContentManager))]
	interface NSTextContentStorage : NSTextStorageObserving {
		[Wrap ("WeakDelegate")]
		[NullAllowed]
		INSTextContentStorageDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[NullAllowed, Export ("attributedString", ArgumentSemantic.Copy)]
		NSAttributedString AttributedString { get; set; }

		[Export ("attributedStringForTextElement:")]
		[return: NullAllowed]
		NSAttributedString GetAttributedString (NSTextElement textElement);

		[Export ("textElementForAttributedString:")]
		[return: NullAllowed]
		NSTextElement GetTextElement (NSAttributedString attributedString);

		[Export ("locationFromLocation:withOffset:")]
		[return: NullAllowed]
		INSTextLocation GetLocation (INSTextLocation location, nint offset);

		[Export ("offsetFromLocation:toLocation:")]
		nint GetOffset (INSTextLocation from, INSTextLocation to);

		[Export ("adjustedRangeFromRange:forEditingTextSelection:")]
		[return: NullAllowed]
		NSTextRange GetAdjustedRange (NSTextRange textRange, bool forEditingTextSelection);

		[TV (26, 0), iOS (26, 0), MacCatalyst (26, 0), Mac (26, 0)]
		[Export ("includesTextListMarkers")]
		bool IncludesTextListMarkers { get; set; }
	}

	[MacCatalyst (13, 0)]
	[BaseType (typeof (NSObject))]
	interface NSTextList : NSCoding, NSCopying, NSSecureCoding {
		[Export ("initWithMarkerFormat:options:")]
		NativeHandle Constructor (string format, NSTextListOptions mask);

		[Wrap ("this (format, NSTextListOptions.None)")]
		NativeHandle Constructor (string format);

		/// <param name="format">To be added.</param>
		/// <param name="mask">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Wrap ("this (format.GetConstant ()!, mask)")]
		NativeHandle Constructor (NSTextListMarkerFormats format, NSTextListOptions mask);

		[Wrap ("this (format.GetConstant ()!, NSTextListOptions.None)")]
		NativeHandle Constructor (NSTextListMarkerFormats format);

		[BindAs (typeof (NSTextListMarkerFormats))]
		[Export ("markerFormat")]
		NSString MarkerFormat { get; }

		[Sealed]
		[Export ("markerFormat")]
		string CustomMarkerFormat { get; }

		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("initWithMarkerFormat:options:startingItemNumber:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string markerFormat, NSTextListOptions options, nint startingItemNumber);

		[Export ("listOptions")]
		NSTextListOptions ListOptions { get; }

		[Export ("markerForItemNumber:")]
		string GetMarker (nint itemNum);

		//Detected properties
		[Export ("startingItemNumber")]
		nint StartingItemNumber { get; set; }

		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("ordered")]
		bool Ordered { [Bind ("isOrdered")] get; }

		[TV (26, 0), Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
		[Static]
		[Export ("includesTextListMarkers")]
		bool IncludesTextListMarkers { get; }
	}

	[MacCatalyst (13, 1)]
	[DesignatedDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface NSTextBlock : NSSecureCoding, NSCopying {
		[Export ("setValue:type:forDimension:")]
		void SetValue (nfloat val, NSTextBlockValueType type, NSTextBlockDimension dimension);

		[Export ("valueForDimension:")]
		nfloat GetValue (NSTextBlockDimension dimension);

		[Export ("valueTypeForDimension:")]
		NSTextBlockValueType GetValueType (NSTextBlockDimension dimension);

		[Export ("setContentWidth:type:")]
		void SetContentWidth (nfloat val, NSTextBlockValueType type);

		[Export ("contentWidth")]
		nfloat ContentWidth { get; }

		[Export ("contentWidthValueType")]
		NSTextBlockValueType ContentWidthValueType { get; }

		[NoiOS, NoTV, NoMacCatalyst]
		[Export ("setWidth:type:forLayer:edge:")]
		void SetWidth (nfloat val, NSTextBlockValueType type, NSTextBlockLayer layer, NSRectEdge edge);

		[Export ("setWidth:type:forLayer:")]
		void SetWidth (nfloat val, NSTextBlockValueType type, NSTextBlockLayer layer);

		[NoiOS, NoTV, NoMacCatalyst]
		[Export ("widthForLayer:edge:")]
		nfloat GetWidth (NSTextBlockLayer layer, NSRectEdge edge);

		[NoiOS, NoTV, NoMacCatalyst]
		[Export ("widthValueTypeForLayer:edge:")]
		NSTextBlockValueType WidthValueTypeForLayer (NSTextBlockLayer layer, NSRectEdge edge);

		[NoiOS, NoTV, NoMacCatalyst]
		[Export ("setBorderColor:forEdge:")]
		void SetBorderColor (NSColor color, NSRectEdge edge);

		[Export ("setBorderColor:")]
		void SetBorderColor ([NullAllowed] NSColor color);

		[NoiOS, NoTV, NoMacCatalyst]
		[Export ("borderColorForEdge:")]
		NSColor GetBorderColor (NSRectEdge edge);

		[Mac (27, 0), iOS (27, 0), TV (27, 0), MacCatalyst (27, 0)]
		[Export ("setWidth:type:forLayer:rectEdge:")]
		void SetWidth (nfloat width, NSTextBlockValueType type, NSTextBlockLayer layer, CGRectEdge rectEdge);

		[Mac (27, 0), iOS (27, 0), TV (27, 0), MacCatalyst (27, 0)]
		[Export ("widthForLayer:rectEdge:")]
		nfloat GetWidth (NSTextBlockLayer layer, CGRectEdge rectEdge);

		[Mac (27, 0), iOS (27, 0), TV (27, 0), MacCatalyst (27, 0)]
		[Export ("widthValueTypeForLayer:rectEdge:")]
		NSTextBlockValueType GetWidthValueType (NSTextBlockLayer layer, CGRectEdge rectEdge);

		[Mac (27, 0), iOS (27, 0), TV (27, 0), MacCatalyst (27, 0)]
		[Export ("setBorderColor:rectEdge:")]
		void SetBorderColor ([NullAllowed] NSColor color, CGRectEdge rectEdge);

		[Mac (27, 0), iOS (27, 0), TV (27, 0), MacCatalyst (27, 0)]
		[Export ("borderColorForRectEdge:")]
		[return: NullAllowed]
		NSColor GetBorderColor (CGRectEdge rectEdge);

		[NoiOS, NoTV, NoMacCatalyst]
		[Export ("rectForLayoutAtPoint:inRect:textContainer:characterRange:")]
		CGRect GetRectForLayout (CGPoint startingPoint, CGRect rect, NSTextContainer textContainer, NSRange charRange);

		[NoiOS, NoTV, NoMacCatalyst]
		[Export ("boundsRectForContentRect:inRect:textContainer:characterRange:")]
		CGRect GetBoundsRect (CGRect contentRect, CGRect rect, NSTextContainer textContainer, NSRange charRange);

		[NoiOS, NoTV, NoMacCatalyst]
		[Export ("drawBackgroundWithFrame:inView:characterRange:layoutManager:")]
		void DrawBackground (CGRect frameRect, [NullAllowed] NSView controlView, NSRange charRange, NSLayoutManager layoutManager);

		//Detected properties
		[Export ("verticalAlignment")]
		NSTextBlockVerticalAlignment VerticalAlignment { get; set; }

		[NullAllowed, Export ("backgroundColor", ArgumentSemantic.Copy)]
		NSColor BackgroundColor { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSTextBlock))]
	[DisableDefaultCtor]
	interface NSTextTableBlock {
		[DesignatedInitializer]
		[Export ("initWithTable:startingRow:rowSpan:startingColumn:columnSpan:")]
		NativeHandle Constructor (NSTextTable table, nint row, nint rowSpan, nint col, nint colSpan);

		[Export ("table")]
		NSTextTable Table { get; }

		[Export ("startingRow")]
		nint StartingRow { get; }

		[Export ("rowSpan")]
		nint RowSpan { get; }

		[Export ("startingColumn")]
		nint StartingColumn { get; }

		[Export ("columnSpan")]
		nint ColumnSpan { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSTextBlock))]
	interface NSTextTable {
		[NoiOS, NoTV, NoMacCatalyst]
		[Export ("rectForBlock:layoutAtPoint:inRect:textContainer:characterRange:")]
		CGRect GetRectForBlock (NSTextTableBlock block, CGPoint startingPoint, CGRect rect, NSTextContainer textContainer, NSRange charRange);

		[NoiOS, NoTV, NoMacCatalyst]
		[Export ("boundsRectForBlock:contentRect:inRect:textContainer:characterRange:")]
		CGRect GetBoundsRect (NSTextTableBlock block, CGRect contentRect, CGRect rect, NSTextContainer textContainer, NSRange charRange);

		[NoiOS, NoTV, NoMacCatalyst]
		[Export ("drawBackgroundForBlock:withFrame:inView:characterRange:layoutManager:")]
		void DrawBackground (NSTextTableBlock block, CGRect frameRect, NSView controlView, NSRange charRange, NSLayoutManager layoutManager);

		//Detected properties
		[Export ("numberOfColumns")]
#if MONOMAC && !XAMCORE_5_0
		nint Columns { get; set; }
#else
		nint NumberOfColumns { get; set; }
#endif

		[Export ("layoutAlgorithm")]
		NSTextTableLayoutAlgorithm LayoutAlgorithm { get; set; }

		[Export ("collapsesBorders")]
		bool CollapsesBorders { get; set; }

		[Export ("hidesEmptyCells")]
		bool HidesEmptyCells { get; set; }
	}

	[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSTextParagraph))]
	interface NSTextListElement {
		[Export ("initWithAttributedString:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] NSAttributedString attributedString);

		[Export ("initWithTextContentManager:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] NSTextContentManager textContentManager);

		[Export ("initWithParentElement:textList:contents:markerAttributes:childElements:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] NSTextListElement parent, NSTextList textList, [NullAllowed] NSAttributedString contents, [NullAllowed] NSDictionary markerAttributes, [NullAllowed] NSTextListElement [] children);

		[Static]
		[Export ("textListElementWithContents:markerAttributes:textList:childElements:")]
		NSTextListElement Create (NSAttributedString contents, [NullAllowed] NSDictionary markerAttributes, NSTextList textList, [NullAllowed] NSTextListElement [] children);

		[Static]
		[Export ("textListElementWithChildElements:textList:nestingLevel:")]
		[return: NullAllowed]
		NSTextListElement Create (NSTextListElement [] children, NSTextList textList, nint nestingLevel);

		[Export ("textList", ArgumentSemantic.Strong)]
		NSTextList TextList { get; }

		[NullAllowed, Export ("contents", ArgumentSemantic.Strong)]
		NSAttributedString Contents { get; }

		[NullAllowed, Export ("markerAttributes", ArgumentSemantic.Strong)]
		NSDictionary WeakMarkerAttributes { get; }

		[Export ("attributedString", ArgumentSemantic.Strong)]
		NSAttributedString AttributedString { get; }

		[Export ("childElements", ArgumentSemantic.Copy)]
		NSTextListElement [] ChildElements { get; }

		[NullAllowed, Export ("parentElement", ArgumentSemantic.Weak)]
		NSTextListElement ParentElement { get; }
	}

	enum NSAttributedStringDocumentType {
		[DefaultEnumValue]
		[Field (null)]
		Unknown = NSDocumentType.Unknown,

		[Field ("NSPlainTextDocumentType")]
		Plain = NSDocumentType.PlainText,

		[Field ("NSRTFDTextDocumentType")]
		Rtfd = NSDocumentType.RTFD,

		[Field ("NSRTFTextDocumentType")]
		Rtf = NSDocumentType.RTF,

		[Field ("NSHTMLTextDocumentType")]
		Html = NSDocumentType.HTML,

		[NoiOS, NoTV, NoMacCatalyst]
		[Field ("NSMacSimpleTextDocumentType")]
		MacSimple = NSDocumentType.MacSimpleText,

		[NoiOS, NoTV, NoMacCatalyst]
		[Field ("NSDocFormatTextDocumentType")]
		DocFormat = NSDocumentType.DocFormat,

		[NoiOS, NoTV, NoMacCatalyst]
		[Field ("NSWordMLTextDocumentType")]
		WordML = NSDocumentType.WordML,

		[NoiOS, NoTV, NoMacCatalyst]
		[Field ("NSWebArchiveTextDocumentType")]
		WebArchive = NSDocumentType.WebArchive,

		[NoiOS, NoTV, NoMacCatalyst]
		[Field ("NSOfficeOpenXMLTextDocumentType")]
		OfficeOpenXml = NSDocumentType.OfficeOpenXml,

		[NoiOS, NoTV, NoMacCatalyst]
		[Field ("NSOpenDocumentTextDocumentType")]
		OpenDocument = NSDocumentType.OpenDocument,
	}

	[Static]
	[Internal]
	interface NSAttributedStringDocumentAttributeKey {
		[Field ("NSDocumentTypeDocumentAttribute")]
		NSString DocumentTypeDocumentAttribute { get; }

		[NoiOS, NoTV, NoMacCatalyst]
		[Field ("NSConvertedDocumentAttribute")]
		NSString ConvertedDocumentAttribute { get; }

		[NoiOS, NoTV, NoMacCatalyst]
		[Field ("NSFileTypeDocumentAttribute")]
		NSString FileTypeDocumentAttribute { get; }

		[NoiOS, NoTV, NoMacCatalyst]
		[Field ("NSTitleDocumentAttribute")]
		NSString TitleDocumentAttribute { get; }

		[NoiOS, NoTV, NoMacCatalyst]
		[Field ("NSCompanyDocumentAttribute")]
		NSString CompanyDocumentAttribute { get; }

		[NoiOS, NoTV, NoMacCatalyst]
		[Field ("NSCopyrightDocumentAttribute")]
		NSString CopyrightDocumentAttribute { get; }

		[NoiOS, NoTV, NoMacCatalyst]
		[Field ("NSSubjectDocumentAttribute")]
		NSString SubjectDocumentAttribute { get; }

		[NoiOS, NoTV, NoMacCatalyst]
		[Field ("NSAuthorDocumentAttribute")]
		NSString AuthorDocumentAttribute { get; }

		[NoiOS, NoTV, NoMacCatalyst]
		[Field ("NSKeywordsDocumentAttribute")]
		NSString KeywordsDocumentAttribute { get; }

		[NoiOS, NoTV, NoMacCatalyst]
		[Field ("NSCommentDocumentAttribute")]
		NSString CommentDocumentAttribute { get; }

		[NoiOS, NoTV, NoMacCatalyst]
		[Field ("NSEditorDocumentAttribute")]
		NSString EditorDocumentAttribute { get; }

		[NoiOS, NoTV, NoMacCatalyst]
		[Field ("NSCreationTimeDocumentAttribute")]
		NSString CreationTimeDocumentAttribute { get; }

		[NoiOS, NoTV, NoMacCatalyst]
		[Field ("NSModificationTimeDocumentAttribute")]
		NSString ModificationTimeDocumentAttribute { get; }

		[NoiOS, NoTV, NoMacCatalyst]
		[Field ("NSManagerDocumentAttribute")]
		NSString ManagerDocumentAttribute { get; }

		[NoiOS, NoTV, NoMacCatalyst]
		[Field ("NSCategoryDocumentAttribute")]
		NSString CategoryDocumentAttribute { get; }

		[NoiOS, NoTV, NoMacCatalyst]
		[Field ("NSAppearanceDocumentAttribute")]
		NSString AppearanceDocumentAttribute { get; }

		[Field ("NSCharacterEncodingDocumentAttribute")]
		NSString CharacterEncodingDocumentAttribute { get; }

		[Field ("NSDefaultAttributesDocumentAttribute")]
		NSString DefaultAttributesDocumentAttribute { get; }

		[Field ("NSPaperSizeDocumentAttribute")]
		NSString PaperSizeDocumentAttribute { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSPaperMarginDocumentAttribute")]
		NSString PaperMarginDocumentAttribute { get; }

		[NoiOS, NoTV, NoMacCatalyst]
		[Field ("NSLeftMarginDocumentAttribute")]
		NSString LeftMarginDocumentAttribute { get; }

		[NoiOS, NoTV, NoMacCatalyst]
		[Field ("NSRightMarginDocumentAttribute")]
		NSString RightMarginDocumentAttribute { get; }

		[NoiOS, NoTV, NoMacCatalyst]
		[Field ("NSTopMarginDocumentAttribute")]
		NSString TopMarginDocumentAttribute { get; }

		[NoiOS, NoTV, NoMacCatalyst]
		[Field ("NSBottomMarginDocumentAttribute")]
		NSString BottomMarginDocumentAttribute { get; }

		[Field ("NSViewSizeDocumentAttribute")]
		NSString ViewSizeDocumentAttribute { get; }

		[Field ("NSViewZoomDocumentAttribute")]
		NSString ViewZoomDocumentAttribute { get; }

		[Field ("NSViewModeDocumentAttribute")]
		NSString ViewModeDocumentAttribute { get; }

		[Field ("NSReadOnlyDocumentAttribute")]
		NSString ReadOnlyDocumentAttribute { get; }

		[Field ("NSBackgroundColorDocumentAttribute")]
		NSString BackgroundColorDocumentAttribute { get; }

		[Field ("NSHyphenationFactorDocumentAttribute")]
		NSString HyphenationFactorDocumentAttribute { get; }

		[Field ("NSDefaultTabIntervalDocumentAttribute")]
		NSString DefaultTabIntervalDocumentAttribute { get; }

		[Field ("NSTextLayoutSectionsAttribute")]
		NSString TextLayoutSectionsAttribute { get; }

		[NoiOS, NoTV, NoMacCatalyst]
		[Field ("NSExcludedElementsDocumentAttribute")]
		NSString ExcludedElementsDocumentAttribute { get; }

		[NoiOS, NoTV, NoMacCatalyst]
		[Field ("NSTextEncodingNameDocumentAttribute")]
		NSString TextEncodingNameDocumentAttribute { get; }

		[NoiOS, NoTV, NoMacCatalyst]
		[Field ("NSPrefixSpacesDocumentAttribute")]
		NSString PrefixSpacesDocumentAttribute { get; }

		[Field ("NSTextScalingDocumentAttribute")]
		[MacCatalyst (13, 1)]
		NSString TextScalingDocumentAttribute { get; }

		[Field ("NSSourceTextScalingDocumentAttribute")]
		[MacCatalyst (13, 1)]
		NSString SourceTextScalingDocumentAttribute { get; }

		[Field ("NSCocoaVersionDocumentAttribute")]
		[MacCatalyst (13, 1)]
		NSString CocoaVersionDocumentAttribute { get; }

		[Field ("NSDefaultFontExcludedDocumentAttribute")]
		[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		NSString DefaultFontExcludedDocumentAttribute { get; }
	}

	[StrongDictionary (nameof (NSAttributedStringDocumentReadingOptionKey), Suffix = "DocumentOption")]
	interface NSAttributedStringDocumentReadingOptions {
#if XAMCORE_5_0
		[Export ("DocumentTypeDocumentOption")]
		NSAttributedStringDocumentType StrongDocumentType { get; set; }
#else
		NSAttributedStringDocumentType DocumentType { get; set; }
#endif

		// It's not documented which attributes go in this dictionary.
		NSDictionary DefaultAttributes { get; set; }

		NSStringEncoding CharacterEncoding { get; set; }

		[NoiOS, NoTV, NoMacCatalyst]
		WebPreferences WebPreferences { get; set; }

		[NoiOS, NoTV, NoMacCatalyst]
		NSObject WebResourceLoadDelegate { get; set; }

		[NoiOS, NoTV, NoMacCatalyst]
		NSUrl BaseUrl { get; set; }

		[NoiOS, NoTV, NoMacCatalyst]
		string TextEncodingName { get; set; }

		[NoiOS, NoTV, NoMacCatalyst]
		float TextSizeMultiplier { get; set; }

		[NoiOS, NoTV, NoMacCatalyst]
		float Timeout { get; set; }

		NSTextScalingType TargetTextScaling { get; set; }

		NSTextScalingType SourceTextScaling { get; set; }

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		bool TextKit1ListMarkerFormat { get; set; }
	}

	[Static]
	[Internal]
	interface NSAttributedStringDocumentReadingOptionKey {
		[MacCatalyst (13, 1)]
		[Field ("NSDocumentTypeDocumentOption")]
		NSString DocumentTypeDocumentOption { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSDefaultAttributesDocumentOption")]
		NSString DefaultAttributesDocumentOption { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSCharacterEncodingDocumentOption")]
		NSString CharacterEncodingDocumentOption { get; }

		[NoiOS, NoTV, NoMacCatalyst]
		[Field ("NSWebPreferencesDocumentOption")]
		NSString WebPreferencesDocumentOption { get; }

		[NoiOS, NoTV, NoMacCatalyst]
		[Field ("NSWebResourceLoadDelegateDocumentOption")]
		NSString WebResourceLoadDelegateDocumentOption { get; }

		[NoiOS, NoTV, NoMacCatalyst]
		[Field ("NSBaseURLDocumentOption")]
		NSString BaseUrlDocumentOption { get; }

		[NoiOS, NoTV, NoMacCatalyst]
		[Field ("NSTextEncodingNameDocumentOption")]
		NSString TextEncodingNameDocumentOption { get; }

		[NoiOS, NoTV, NoMacCatalyst]
		[Field ("NSTextSizeMultiplierDocumentOption")]
		NSString TextSizeMultiplierDocumentOption { get; }

		[NoiOS, NoTV, NoMacCatalyst]
		[Field ("NSTimeoutDocumentOption")]
		NSString TimeoutDocumentOption { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSTargetTextScalingDocumentOption")]
		NSString TargetTextScalingDocumentOption { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSSourceTextScalingDocumentOption")]
		NSString SourceTextScalingDocumentOption { get; }

		// comes from webkit
		[MacCatalyst (13, 1), NoTV]
		[Field ("NSReadAccessURLDocumentOption", "WebKit")]
		NSString ReadAccessUrlDocumentOption { get; }

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("NSTextKit1ListMarkerFormatDocumentOption")]
		NSString TextKit1ListMarkerFormatDocumentOption { get; }
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSAdaptiveImageGlyph : NSCopying, NSSecureCoding, CTAdaptiveImageProviding {
		[DesignatedInitializer]
		[Export ("initWithImageContent:")]
		NativeHandle Constructor (NSData imageContent);

		[Export ("imageContent")]
		NSData ImageContent { get; }

		[Export ("contentIdentifier")]
		string ContentIdentifier { get; }

		[Export ("contentDescription", ArgumentSemantic.Copy)]
		string ContentDescription { get; }

		[Static]
		[Export ("contentType")]
		UTType ContentType { get; }
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	enum NSTextHighlightStyle {
		[DefaultEnumValue]
		[Field ("NSTextHighlightStyleDefault")]
		Default,
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	enum NSTextHighlightColorScheme {
		[DefaultEnumValue]
		[Field ("NSTextHighlightColorSchemeDefault")]
		Default,
		[Field ("NSTextHighlightColorSchemePurple")]
		Purple,
		[Field ("NSTextHighlightColorSchemePink")]
		Pink,
		[Field ("NSTextHighlightColorSchemeOrange")]
		Orange,
		[Field ("NSTextHighlightColorSchemeMint")]
		Mint,
		[Field ("NSTextHighlightColorSchemeBlue")]
		Blue,

	}

	[NoTV, MacCatalyst (18, 2), iOS (18, 2), Mac (15, 2)]
	[Native]
#if MONOMAC
	public enum NSWritingToolsCoordinatorTextUpdateReason : long
#else
	public enum UIWritingToolsCoordinatorTextUpdateReason : long
#endif
	{
		Typing,
		UndoRedo,
	}

	[NoTV, MacCatalyst (18, 2), iOS (18, 2), Mac (15, 2)]
	[Native]
#if MONOMAC
	public enum NSWritingToolsCoordinatorState : long
#else
	public enum UIWritingToolsCoordinatorState : long
#endif
	{
		Inactive,
		Noninteractive,
		InteractiveResting,
		InteractiveStreaming,
	}

	[NoTV, MacCatalyst (18, 2), iOS (18, 2), Mac (15, 2)]
	[Native]
#if MONOMAC
	public enum NSWritingToolsCoordinatorTextReplacementReason : long
#else
	public enum UIWritingToolsCoordinatorTextReplacementReason : long
#endif
	{
		Interactive,
		Noninteractive,
		[MacCatalyst (27, 0), iOS (27, 0), Mac (27, 0)]
		Accepted = 2,
		[MacCatalyst (27, 0), iOS (27, 0), Mac (27, 0)]
		Rejected = 3,
		[MacCatalyst (27, 0), iOS (27, 0), Mac (27, 0)]
		Temporary = 4,
	}

	[NoTV, MacCatalyst (18, 2), iOS (18, 2), Mac (15, 2)]
	[Native]
#if MONOMAC
	public enum NSWritingToolsCoordinatorContextScope : long
#else
	public enum UIWritingToolsCoordinatorContextScope : long
#endif
	{
		UserSelection,
		FullDocument,
		VisibleArea,
	}

	[NoTV, MacCatalyst (18, 2), iOS (18, 2), Mac (15, 2)]
	[Native]
#if MONOMAC
	public enum NSWritingToolsCoordinatorTextAnimation : long
#else
	public enum UIWritingToolsCoordinatorTextAnimation : long
#endif
	{
		Anticipate,
		Remove,
		Insert,
		[NoiOS, NoMacCatalyst]
		AnticipateInactive = 8,
		[NoiOS, NoMacCatalyst]
		Translate = 9,
		[MacCatalyst (27, 0), iOS (27, 0), Mac (27, 0)]
		IndicateGrammar = 10,
	}

	[TV (27, 0), MacCatalyst (27, 0), iOS (27, 0), Mac (27, 0)]
	[Native]
#if MONOMAC
	public enum NSWritingToolsCoordinatorTextDecoration : long
#else
	public enum UIWritingToolsCoordinatorTextDecoration : long
#endif
	{
		None,
		GrammarUnderline,
	}

	[NoTV, MacCatalyst (18, 2), iOS (18, 2), Mac (15, 2)]
	[BaseType (typeof (NSObject))]
#if MONOMAC
	interface NSWritingToolsCoordinator
#else
	interface UIWritingToolsCoordinator : UIInteraction
#endif
	{
		[Static]
		[Export ("isWritingToolsAvailable")]
		bool IsWritingToolsAvailable { get; }

		[Export ("initWithDelegate:")]
		NativeHandle Constructor ([NullAllowed] IXWritingToolsCoordinatorDelegate @delegate);

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IXWritingToolsCoordinatorDelegate Delegate { get; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; }

#if MONOMAC
		[NullAllowed, Export ("view", ArgumentSemantic.Weak)]
		View View { get; }
#endif

		[NullAllowed, Export ("effectContainerView", ArgumentSemantic.Weak)]
		View EffectContainerView { get; set; }

		[NullAllowed, Export ("decorationContainerView", ArgumentSemantic.Weak)]
		View DecorationContainerView { get; set; }

		[Export ("state")]
		XWritingToolsCoordinatorState State { get; }

		[Export ("stopWritingTools")]
		void StopWritingTools ();

		[Export ("preferredBehavior", ArgumentSemantic.Assign)]
		XWritingToolsBehavior PreferredBehavior { get; set; }

		[Export ("behavior")]
		XWritingToolsBehavior Behavior { get; }

		[Export ("preferredResultOptions", ArgumentSemantic.Assign)]
		XWritingToolsResultOptions PreferredResultOptions { get; set; }

		[Export ("resultOptions")]
		XWritingToolsResultOptions ResultOptions { get; }

		[Export ("updateRange:withText:reason:forContextWithIdentifier:")]
		void UpdateRange (NSRange range, NSAttributedString replacementText, XWritingToolsCoordinatorTextUpdateReason reason, NSUuid contextId);

		[Export ("updateForReflowedTextInContextWithIdentifier:")]
		void UpdateForReflowedTextInContext (NSUuid contextId);

		[TV (27, 0), MacCatalyst (27, 0), iOS (27, 0), Mac (27, 0)]
		[Export ("startTextAnimation:forRange:inContext:writingDirection:")]
		[return: NullAllowed]
		NSUuid StartTextAnimation (XWritingToolsCoordinatorTextAnimation textAnimation, NSRange range, XWritingToolsCoordinatorContext context, NSWritingDirection writingDirection);

		[TV (27, 0), MacCatalyst (27, 0), iOS (27, 0), Mac (27, 0)]
		[Export ("cancelTextAnimationsWithIdentifiers:")]
		void CancelTextAnimations (NSUuid [] identifiers);

		[TV (27, 0), MacCatalyst (27, 0), iOS (27, 0), Mac (27, 0)]
		[Export ("showGrammarPresentationForRange:inContext:")]
		bool ShowGrammarPresentation (NSRange range, XWritingToolsCoordinatorContext context);

		[TV (26, 0), iOS (26, 0), MacCatalyst (26, 0), Mac (26, 0)]
		[Export ("includesTextListMarkers")]
		bool IncludesTextListMarkers { get; set; }
	}

#if MONOMAC
	interface INSWritingToolsCoordinatorDelegate { }
#else
	interface IUIWritingToolsCoordinatorDelegate { }
#endif

#if MONOMAC
	delegate void NSWritingToolsCoordinatorDelegateRequestsContextsCallback (XWritingToolsCoordinatorContext [] contexts);
	delegate void NSWritingToolsCoordinatorDelegateReplaceRangeCallback ([NullAllowed] NSAttributedString replacementText);
	delegate void NSWritingToolsCoordinatorDelegateRequestsRangeCallback (NSRange range, NSUuid contextId);
	delegate void NSWritingToolsCoordinatorDelegateRequestsBoundingBezierPathsCallback (BezierPath [] paths);
	delegate void NSWritingToolsCoordinatorDelegateRequestsUnderlinePathsCallback (BezierPath [] paths);
	delegate void NSWritingToolsCoordinatorDelegateRequestsPreviewForTextAnimationCallback ([NullAllowed] NSTextPreview [] view); // different signature vs other platforms
	delegate void NSWritingToolsCoordinatorDelegateRequestsPreviewForTextAnimationWithTextDecorationCallback ([NullAllowed] NSTextPreview [] textPreviews);
	delegate void NSWritingToolsCoordinatorDelegateRequestsGrammarResultsCallback (NSTextCheckingResult [] results);
	delegate void NSWritingToolsCoordinatorDelegateRequestsPreviewCallback ([NullAllowed] NSTextPreview textPreview); // doesn't exist on other platforms
	delegate void NSWritingToolsCoordinatorDelegateRequestsSingleContainerSubrangesCallback (/* [BindAs (typeof (NSRange[]))] */ NSValue [] ranges); // BindAs doesn't work here
	delegate void NSWritingToolsCoordinatorDelegateRequestsDecorationContainerViewCallback (View view);
#else
	delegate void UIWritingToolsCoordinatorDelegateRequestsContextsCallback (XWritingToolsCoordinatorContext [] contexts);
	delegate void UIWritingToolsCoordinatorDelegateReplaceRangeCallback ([NullAllowed] NSAttributedString replacementText);
	delegate void UIWritingToolsCoordinatorDelegateRequestsRangeCallback (NSRange range, NSUuid contextId);
	delegate void UIWritingToolsCoordinatorDelegateRequestsBoundingBezierPathsCallback (BezierPath [] paths);
	delegate void UIWritingToolsCoordinatorDelegateRequestsUnderlinePathsCallback (BezierPath [] paths);
	delegate void UIWritingToolsCoordinatorDelegateRequestsPreviewForTextAnimationCallback ([NullAllowed] View view); // different signature vs macOS
	delegate void UIWritingToolsCoordinatorDelegateRequestsPreviewForTextAnimationWithTextDecorationCallback ([NullAllowed] UITargetedPreview preview);
	delegate void UIWritingToolsCoordinatorDelegateRequestsGrammarResultsCallback (NSTextCheckingResult [] results);
	delegate void UIWritingToolsCoordinatorDelegateRequestsSingleContainerSubrangesCallback (/* [BindAs (typeof (NSRange[]))] */ NSValue [] ranges); // BindAs doesn't work here
	delegate void UIWritingToolsCoordinatorDelegateRequestsDecorationContainerViewCallback (View view);
#endif

	[NoTV, MacCatalyst (18, 2), iOS (18, 2), Mac (15, 2)]
	[Protocol (BackwardsCompatibleCodeGeneration = false), Model]
	[BaseType (typeof (NSObject))]
#if MONOMAC
	interface NSWritingToolsCoordinatorDelegate
#else
	interface UIWritingToolsCoordinatorDelegate
#endif
	{
		[Abstract]
		[Export ("writingToolsCoordinator:requestsContextsForScope:completion:")]
		void RequestsContexts (XWritingToolsCoordinator writingToolsCoordinator, XWritingToolsCoordinatorContextScope scope, XWritingToolsCoordinatorDelegateRequestsContextsCallback completion);

		[Abstract]
		[Export ("writingToolsCoordinator:replaceRange:inContext:proposedText:reason:animationParameters:completion:")]
		void ReplaceRange (XWritingToolsCoordinator writingToolsCoordinator, NSRange range, XWritingToolsCoordinatorContext context, NSAttributedString replacementText, XWritingToolsCoordinatorTextReplacementReason reason, [NullAllowed] XWritingToolsCoordinatorAnimationParameters animationParameters, XWritingToolsCoordinatorDelegateReplaceRangeCallback completion);

		[Abstract]
		[Export ("writingToolsCoordinator:selectRanges:inContext:completion:")]
		void SelectRanges (XWritingToolsCoordinator writingToolsCoordinator, NSValue [] ranges, XWritingToolsCoordinatorContext context, Action completion);

		[Deprecated (PlatformName.MacOSX, 15, 4, "Not called anymore.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 4, "Not called anymore.")]
		[Deprecated (PlatformName.iOS, 18, 4, "Not called anymore.")]
		[Export ("writingToolsCoordinator:requestsRangeInContextWithIdentifierForPoint:completion:")]
		void RequestsRange (XWritingToolsCoordinator writingToolsCoordinator, CGPoint point, XWritingToolsCoordinatorDelegateRequestsRangeCallback completion);

		[Abstract]
		[Export ("writingToolsCoordinator:requestsBoundingBezierPathsForRange:inContext:completion:")]
		void RequestsBoundingBezierPaths (XWritingToolsCoordinator writingToolsCoordinator, NSRange range, XWritingToolsCoordinatorContext context, XWritingToolsCoordinatorDelegateRequestsBoundingBezierPathsCallback completion);

		[Abstract]
		[Export ("writingToolsCoordinator:requestsUnderlinePathsForRange:inContext:completion:")]
		void RequestsUnderlinePaths (XWritingToolsCoordinator writingToolsCoordinator, NSRange range, XWritingToolsCoordinatorContext context, XWritingToolsCoordinatorDelegateRequestsUnderlinePathsCallback completion);

		[Abstract]
		[Export ("writingToolsCoordinator:prepareForTextAnimation:forRange:inContext:completion:")]
		void PrepareForTextAnimation (XWritingToolsCoordinator writingToolsCoordinator, XWritingToolsCoordinatorTextAnimation textAnimation, NSRange range, XWritingToolsCoordinatorContext context, Action completion);

		[Abstract]
		[Export ("writingToolsCoordinator:requestsPreviewForTextAnimation:ofRange:inContext:completion:")]
		void RequestsPreviewForTextAnimation (XWritingToolsCoordinator writingToolsCoordinator, XWritingToolsCoordinatorTextAnimation textAnimation, NSRange range, XWritingToolsCoordinatorContext context, XWritingToolsCoordinatorDelegateRequestsPreviewForTextAnimationCallback completion);

		[TV (27, 0), MacCatalyst (27, 0), iOS (27, 0), Mac (27, 0)]
		[Export ("writingToolsCoordinator:requestsPreviewForTextAnimation:ofRange:inContext:textDecoration:completion:")]
		void RequestsPreviewForTextAnimation (XWritingToolsCoordinator writingToolsCoordinator, XWritingToolsCoordinatorTextAnimation textAnimation, NSRange range, XWritingToolsCoordinatorContext context, XWritingToolsCoordinatorTextDecoration textDecoration, XWritingToolsCoordinatorDelegateRequestsPreviewForTextAnimationWithTextDecorationCallback completion);

		[TV (27, 0), MacCatalyst (27, 0), iOS (27, 0), Mac (27, 0)]
		[Export ("writingToolsCoordinator:requestsGrammarResultsForContext:completion:")]
		void RequestsGrammarResults (XWritingToolsCoordinator writingToolsCoordinator, XWritingToolsCoordinatorContext context, XWritingToolsCoordinatorDelegateRequestsGrammarResultsCallback completion);

		[TV (27, 0), MacCatalyst (27, 0), iOS (27, 0), Mac (27, 0)]
		[Export ("writingToolsCoordinator:setGrammarCheckingEnabled:")]
		void SetGrammarCheckingEnabled (XWritingToolsCoordinator writingToolsCoordinator, bool enabled);

#if MONOMAC
		[Abstract]
		[Export ("writingToolsCoordinator:requestsPreviewForRect:inContext:completion:")]
		void RequestsPreview (XWritingToolsCoordinator writingToolsCoordinator, CGRect rect, XWritingToolsCoordinatorContext context, NSWritingToolsCoordinatorDelegateRequestsPreviewCallback completion);
#endif

		[Abstract]
		[Export ("writingToolsCoordinator:finishTextAnimation:forRange:inContext:completion:")]
		void FinishTextAnimation (XWritingToolsCoordinator writingToolsCoordinator, XWritingToolsCoordinatorTextAnimation textAnimation, NSRange range, XWritingToolsCoordinatorContext context, Action completion);

		[Export ("writingToolsCoordinator:requestsSingleContainerSubrangesOfRange:inContext:completion:")]
		void RequestsSingleContainerSubranges (XWritingToolsCoordinator writingToolsCoordinator, NSRange range, XWritingToolsCoordinatorContext context, XWritingToolsCoordinatorDelegateRequestsSingleContainerSubrangesCallback completion);

		[Export ("writingToolsCoordinator:requestsDecorationContainerViewForRange:inContext:completion:")]
		void RequestsDecorationContainerView (XWritingToolsCoordinator writingToolsCoordinator, NSRange range, XWritingToolsCoordinatorContext context, XWritingToolsCoordinatorDelegateRequestsDecorationContainerViewCallback completion);

		[Export ("writingToolsCoordinator:willChangeToState:completion:")]
		void WillChangeToState (XWritingToolsCoordinator writingToolsCoordinator, XWritingToolsCoordinatorState newState, Action completion);
	}

	[NoTV, MacCatalyst (18, 2), iOS (18, 2), Mac (15, 2)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
#if MONOMAC
	interface NSWritingToolsCoordinatorAnimationParameters
#else
	interface UIWritingToolsCoordinatorAnimationParameters
#endif
	{
		[Export ("duration")]
		nfloat Duration { get; }

		[Export ("delay")]
		nfloat Delay { get; }

		[NullAllowed, Export ("progressHandler", ArgumentSemantic.Copy)]
		Action<float> ProgressHandler { get; set; }

		[NullAllowed, Export ("completionHandler", ArgumentSemantic.Copy)]
		Action CompletionHandler { get; set; }
	}

	[NoTV, MacCatalyst (18, 2), iOS (18, 2), Mac (15, 2)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
#if MONOMAC
	interface NSWritingToolsCoordinatorContext
#else
	interface UIWritingToolsCoordinatorContext
#endif
	{
		[Export ("initWithAttributedString:range:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSAttributedString attributedString, NSRange range);

		[Export ("attributedString", ArgumentSemantic.Copy)]
		NSAttributedString AttributedString { get; }

		[Export ("range")]
		NSRange Range { get; }

		[Export ("identifier", ArgumentSemantic.Strong)]
		NSUuid Identifier { get; }

		[Export ("resolvedRange")]
		NSRange ResolvedRange { get; }
	}

}
