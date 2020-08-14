// This file contains api definitions shared between AppKit and UIKit

using System;
using System.Diagnostics;
using System.ComponentModel;
using Foundation;
using ObjCRuntime;
using CoreGraphics;

using CGGlyph=System.UInt16;
using NSGlyph=System.UInt32;

#if !MONOMAC
using NSColor=UIKit.UIColor;
using NSFont=UIKit.UIFont;
#endif

// dummy types to simplify build
#if !MONOMAC
using NSCell=System.Object;
using NSGlyphGenerator=System.Object;
using NSGlyphStorageOptions=System.Object;
using NSImageScaling=System.Object;
using NSRulerMarker=System.Object;
using NSRulerView=System.Object;
using NSTextBlock=System.Object;
using NSTextList=System.Object;
using NSTextTableBlock=System.Object;
using NSTextStorageEditedFlags=System.Object;
using NSTextView=System.Object;
using NSTypesetter=System.Object;
using NSTypesetterBehavior=System.Object;
using NSView=System.Object;
using NSWindow=System.Object;
#if WATCH
using NSTextContainer=System.Object;
using NSTextStorage=System.Object;
#endif // WATCH
#endif // !MONOMAC

#if MONOMAC
using TextAlignment=AppKit.NSTextAlignment;
using LineBreakMode=AppKit.NSLineBreakMode;
#else
using TextAlignment=UIKit.UITextAlignment;
using LineBreakMode=UIKit.UILineBreakMode;
#endif

#if MONOMAC
namespace AppKit {
#else
namespace UIKit {
#endif

#if XAMCORE_4_0 || MONOMAC
	delegate void NSTextLayoutEnumerateLineFragments (CGRect rect, CGRect usedRectangle, NSTextContainer textContainer, NSRange glyphRange, out bool stop);
	delegate void NSTextLayoutEnumerateEnclosingRects (CGRect rect, out bool stop);
#else
	delegate void NSTextLayoutEnumerateLineFragments (CGRect rect, CGRect usedRectangle, NSTextContainer textContainer, NSRange glyphRange, ref bool stop);
	delegate void NSTextLayoutEnumerateEnclosingRects (CGRect rect, ref bool stop);
#endif

	[NoWatch] // Header is not present in watchOS SDK.
	[iOS (7,0)]
	[DesignatedDefaultCtor]
	[BaseType (typeof (NSObject))]
	partial interface NSLayoutManager : NSSecureCoding {

#if !XAMCORE_4_0
		// This was removed in the headers in the macOS 10.11 SDK
		[NoiOS][NoTV]
		[Availability (Deprecated = Platform.Mac_10_11, Message = "Use 'TextStorage' instead.")]
		[Export ("attributedString")]
		NSAttributedString AttributedString { get; }
#endif

		[Export ("textContainers")]
		NSTextContainer [] TextContainers { get; }

		[Export ("addTextContainer:")]
		void AddTextContainer (NSTextContainer container);

		[Export ("insertTextContainer:atIndex:")]
		void InsertTextContainer (NSTextContainer container, /* NSUInteger */ nint index);

		[Export ("removeTextContainerAtIndex:")]
		void RemoveTextContainer (/* NSUInteger */ nint index);

		[Export ("textContainerChangedGeometry:")]
		void TextContainerChangedGeometry (NSTextContainer container);

		[NoiOS][NoTV]
		[Export ("textContainerChangedTextView:")]
		void TextContainerChangedTextView (NSTextContainer container);

#if !XAMCORE_4_0
		// This was removed in the headers in the macOS 10.11 SDK
		[NoiOS][NoTV]
		[Availability (Deprecated = Platform.Mac_10_11)]
		[Export ("layoutOptions")]
		NSGlyphStorageOptions LayoutOptions { get; }
#endif

		[Export ("hasNonContiguousLayout")]
		bool HasNonContiguousLayout { get; }

		/* InvalidateGlyphs */
#if XAMCORE_4_0 || MONOMAC
		[Protected]
#else
		[Internal][Sealed]
#endif
		[Export ("invalidateGlyphsForCharacterRange:changeInLength:actualCharacterRange:")]
		void InvalidateGlyphs (NSRange characterRange, /* NSInteger */ nint delta, /* nullable NSRangePointer */ IntPtr actualCharacterRange);

		[Wrap ("InvalidateGlyphs (characterRange, delta, IntPtr.Zero)")]
		void InvalidateGlyphs (NSRange characterRange, /* NSInteger */ nint delta);

#if XAMCORE_4_0 || MONOMAC
		[Sealed]
#endif
		[Export ("invalidateGlyphsForCharacterRange:changeInLength:actualCharacterRange:")]
#if XAMCORE_4_0 || MONOMAC
		void InvalidateGlyphs (NSRange characterRange, /* NSInteger */ nint delta, /* nullable NSRangePointer */ out NSRange actualCharacterRange);
#else
		void InvalidateGlyphs (NSRange charRange, /* NSInteger */ nint delta, /* nullable NSRangePointer */ out NSRange actualCharRange);
#endif

		/* InvalidateLayout */
#if XAMCORE_4_0 || MONOMAC
		[Protected]
#else
		[Internal][Sealed]
#endif
		[Export ("invalidateLayoutForCharacterRange:actualCharacterRange:")]
		void InvalidateLayout (NSRange characterRange, /* nullable NSRangePointer */ IntPtr actualCharacterRange);

		[Wrap ("InvalidateLayout (characterRange, IntPtr.Zero)")]
		void InvalidateLayout (NSRange characterRange);

#if XAMCORE_4_0 || MONOMAC
		[Sealed]
#endif
		[Export ("invalidateLayoutForCharacterRange:actualCharacterRange:")]
#if XAMCORE_4_0 || MONOMAC
		void InvalidateLayout (NSRange characterRange, /* nullable NSRangePointer */ out NSRange actualCharacterRange);
#else
		void InvalidateLayout (NSRange charRange, /* nullable NSRangePointer */ out NSRange actualCharRange);
#endif

		[Export ("invalidateDisplayForCharacterRange:")]
#if XAMCORE_4_0
		void InvalidateDisplayForCharacterRange (NSRange characterRange);
#else
		void InvalidateDisplayForCharacterRange (NSRange charRange);
#endif

		[Export ("invalidateDisplayForGlyphRange:")]
		void InvalidateDisplayForGlyphRange (NSRange glyphRange);

#if !XAMCORE_4_0
		[NoiOS][NoTV]
		[Availability (Deprecated = Platform.Mac_10_11, Message = "Use ProcessEditing (NSTextStorage textStorage, NSTextStorageEditActions editMask, NSRange newCharacterRange, nint delta, NSRange invalidatedCharacterRange) instead).")]
		[Export ("textStorage:edited:range:changeInLength:invalidatedRange:")]
		void TextStorageEdited (NSTextStorage str, NSTextStorageEditedFlags editedMask, NSRange newCharRange, nint changeInLength, NSRange invalidatedCharRange);
#endif

		[Export ("ensureGlyphsForCharacterRange:")]
#if XAMCORE_4_0
		void EnsureGlyphsForCharacterRange (NSRange characterRange);
#else
		void EnsureGlyphsForCharacterRange (NSRange charRange);
#endif

		[Export ("ensureGlyphsForGlyphRange:")]
		void EnsureGlyphsForGlyphRange (NSRange glyphRange);

		[Export ("ensureLayoutForCharacterRange:")]
#if XAMCORE_4_0
		void EnsureLayoutForCharacterRange (NSRange characterRange);
#else
		void EnsureLayoutForCharacterRange (NSRange charRange);
#endif

		[Export ("ensureLayoutForGlyphRange:")]
		void EnsureLayoutForGlyphRange (NSRange glyphRange);

		[Export ("ensureLayoutForTextContainer:")]
		void EnsureLayoutForTextContainer (NSTextContainer container);

		[Export ("ensureLayoutForBoundingRect:inTextContainer:")]
		void EnsureLayoutForBoundingRect (CGRect bounds, NSTextContainer container);

#if !XAMCORE_4_0
		[NoiOS][NoTV]
		[Availability (Deprecated = Platform.Mac_10_11, Message = "Use 'SetGlyphs' instead.")]
		[Export ("insertGlyph:atGlyphIndex:characterIndex:")]
		void InsertGlyph (NSGlyph glyph, nint glyphIndex, nint charIndex);
#endif

#if !XAMCORE_4_0
		[NoiOS][NoTV]
		[Availability (Deprecated = Platform.Mac_10_11, Message = "Use 'SetGlyphs' instead.")]
		[Export ("replaceGlyphAtIndex:withGlyph:")]
		void ReplaceGlyphAtIndex (nint glyphIndex, NSGlyph newGlyph);
#endif

#if !XAMCORE_4_0
		[NoiOS][NoTV]
		[Availability (Deprecated = Platform.Mac_10_11, Message = "Use 'SetGlyphs' instead.")]
		[Export ("deleteGlyphsInRange:")]
		void DeleteGlyphs (NSRange glyphRange);
#endif

#if !XAMCORE_4_0
		[NoiOS][NoTV]
		[Availability (Deprecated = Platform.Mac_10_11, Message = "Use 'SetGlyphs' instead.")]
		[Export ("setCharacterIndex:forGlyphAtIndex:")]
		void SetCharacterIndex (nint charIndex, nint glyphIndex);
#endif

#if !XAMCORE_4_0
		[NoiOS][NoTV]
		[Availability (Deprecated = Platform.Mac_10_11, Message = "Use 'SetGlyphs' instead.")]
		[Export ("setIntAttribute:value:forGlyphAtIndex:")]
		void SetIntAttribute (nint attributeTag, nint value, nint glyphIndex);
#endif

#if !XAMCORE_4_0
		[NoiOS][NoTV]
		[Availability (Deprecated = Platform.Mac_10_11, Message = "Use 'SetGlyphs' instead.")]
		[Export ("invalidateGlyphsOnLayoutInvalidationForGlyphRange:")]
		void InvalidateGlyphsOnLayoutInvalidation (NSRange glyphRange);
#endif

		[Export ("numberOfGlyphs")]
#if XAMCORE_4_0 || !MONOMAC
		/* NSUInteger */ nuint NumberOfGlyphs { get; }
#else
		/* NSUInteger */ nint NumberOfGlyphs { get; }
#endif

#if !XAMCORE_4_0
		[Export ("glyphAtIndex:isValidIndex:")]
#if MONOMAC
		[Availability (Deprecated = Platform.Mac_10_11, Message = "Use 'GetCGGlyph' instead).")]
		NSGlyph GlyphAtIndex (nint glyphIndex, ref bool isValidIndex);
#else
 		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'GetGlyph' instead.")]
		CGGlyph GlyphAtIndex (nuint glyphIndex, ref bool isValidIndex);
#endif // MONOMAC
#endif // !XAMCORE_4_0

#if !XAMCORE_4_0
		[Export ("glyphAtIndex:")]
#if MONOMAC
		[Availability (Deprecated = Platform.Mac_10_11, Message = "Use 'GetCGGlyph' instead).")]
		NSGlyph GlyphAtIndex (nint glyphIndex);
#else
 		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'GetGlyph' instead.")]
		CGGlyph GlyphAtIndex (nuint glyphIndex);
#endif // MONOMAC
#endif // !XAMCORE_4_0

		[Export ("isValidGlyphIndex:")]
#if XAMCORE_4_0
		bool IsValidGlyph (nuint glyphIndex);
#elif MONOMAC
		bool IsValidGlyphIndex (nint glyphIndex);
#else
		bool IsValidGlyphIndex (nuint glyphIndex);
#endif

		[Export ("characterIndexForGlyphAtIndex:")]
#if XAMCORE_4_0
		nuint GetCharacterIndex (nuint glyphIndex);
#elif MONOMAC
		nuint CharacterIndexForGlyphAtIndex (nint glyphIndex);
#else
		nuint CharacterIndexForGlyphAtIndex (nuint glyphIndex);
#endif

		[Export ("glyphIndexForCharacterAtIndex:")]
#if XAMCORE_4_0
		nuint GetGlyphIndex (nuint characterIndex);
#elif MONOMAC
		nuint GlyphIndexForCharacterAtIndex (nint charIndex);
#else
		nuint GlyphIndexForCharacterAtIndex (nuint charIndex);
#endif

#if !XAMCORE_4_0
		[NoiOS][NoTV]
		[Availability (Deprecated = Platform.Mac_10_11, Message = "Use 'GetGlyphs' instead).")]
		[Export ("intAttribute:forGlyphAtIndex:")]
		nint GetIntAttribute (nint attributeTag, nint glyphIndex);
#endif

		[Export ("setTextContainer:forGlyphRange:")]
#if XAMCORE_4_0 || !MONOMAC
		void SetTextContainer (NSTextContainer container, NSRange glyphRange);
#else
		void SetTextContainerForRange (NSTextContainer container, NSRange glyphRange);
#endif

		[Export ("setLineFragmentRect:forGlyphRange:usedRect:")]
#if XAMCORE_4_0
		void SetLineFragment (CGRect fragmentRect, NSRange glyphRange, CGRect usedRect);
#else
		void SetLineFragmentRect (CGRect fragmentRect, NSRange glyphRange, CGRect usedRect);
#endif

		[Export ("setExtraLineFragmentRect:usedRect:textContainer:")]
#if XAMCORE_4_0
		void SetExtraLineFragment (CGRect fragmentRect, CGRect usedRect, NSTextContainer container);
#else
		void SetExtraLineFragmentRect (CGRect fragmentRect, CGRect usedRect, NSTextContainer container);
#endif

		[Export ("setLocation:forStartOfGlyphRange:")]
#if MONOMAC || XAMCORE_4_0
		void SetLocation (CGPoint location, NSRange forStartOfGlyphRange);
#else
		void SetLocation (CGPoint location, NSRange glyphRange);
#endif

		[Export ("setNotShownAttribute:forGlyphAtIndex:")]
#if XAMCORE_4_0 || !MONOMAC
		void SetNotShownAttribute (bool flag, nuint glyphIndex);
#else
		void SetNotShownAttribute (bool flag, nint glyphIndex);
#endif

		[Export ("setDrawsOutsideLineFragment:forGlyphAtIndex:")]
#if XAMCORE_4_0 || !MONOMAC
		void SetDrawsOutsideLineFragment (bool flag, nuint glyphIndex);
#else
		void SetDrawsOutsideLineFragment (bool flag, nint glyphIndex);
#endif

		[Export ("setAttachmentSize:forGlyphRange:")]
		void SetAttachmentSize (CGSize attachmentSize, NSRange glyphRange);

		[Export ("getFirstUnlaidCharacterIndex:glyphIndex:")]
#if XAMCORE_4_0
		void GetFirstUnlaid (out nuint characterIndex, out nuint glyphIndex);
#else
		void GetFirstUnlaidCharacterIndex (ref nuint charIndex, ref nuint glyphIndex);
#endif

		[Export ("firstUnlaidCharacterIndex")]
#if XAMCORE_4_0 || !MONOMAC
		nuint FirstUnlaidCharacterIndex { get; }
#else
		nint FirstUnlaidCharacterIndex { get; }
#endif

		[Export ("firstUnlaidGlyphIndex")]
#if XAMCORE_4_0 || !MONOMAC
		nuint FirstUnlaidGlyphIndex { get; }
#else
		nint FirstUnlaidGlyphIndex { get; }
#endif

		/* GetTextContainer */
#if XAMCORE_4_0 || MONOMAC
		[Protected]
#else
		[Sealed][Internal]
#endif
		[Export ("textContainerForGlyphAtIndex:effectiveRange:")]
		NSTextContainer GetTextContainer (nuint glyphIndex, /* nullable NSRangePointer */ IntPtr effectiveGlyphRange);

		[Wrap ("GetTextContainer (glyphIndex, IntPtr.Zero)")]
		NSTextContainer GetTextContainer (nuint glyphIndex);

#if XAMCORE_4_0 || MONOMAC
		[Sealed]
#endif
		[Export ("textContainerForGlyphAtIndex:effectiveRange:")]
		NSTextContainer GetTextContainer (nuint glyphIndex, /* nullable NSRangePointer */ out NSRange effectiveGlyphRange);

#if XAMCORE_4_0 || MONOMAC
		[Protected]
#else
		[Sealed][Internal]
#endif
		[Export ("textContainerForGlyphAtIndex:effectiveRange:withoutAdditionalLayout:")]
		NSTextContainer GetTextContainer (nuint glyphIndex, IntPtr effectiveGlyphRange, bool withoutAdditionalLayout);

		[Wrap ("GetTextContainer (glyphIndex, IntPtr.Zero, flag)")]
		NSTextContainer GetTextContainer (nuint glyphIndex, bool flag);

#if XAMCORE_4_0 || MONOMAC
		[Sealed]
#endif
		[Export ("textContainerForGlyphAtIndex:effectiveRange:withoutAdditionalLayout:")]
		NSTextContainer GetTextContainer (nuint glyphIndex, /* nullable NSRangePointer */ out NSRange effectiveGlyphRange, bool withoutAdditionalLayout);

		[Export ("usedRectForTextContainer:")]
#if XAMCORE_4_0
		CGRect GetUsedRect (NSTextContainer container);
#else
		CGRect GetUsedRectForTextContainer (NSTextContainer container);
#endif

		/* GetLineFragmentRect (NSUInteger, NSRangePointer) */
		[Protected]
		[Export ("lineFragmentRectForGlyphAtIndex:effectiveRange:")]
		CGRect GetLineFragmentRect (nuint glyphIndex, /* nullable NSRangePointer */ IntPtr effectiveGlyphRange);

		[Wrap ("GetLineFragmentRect (glyphIndex, IntPtr.Zero)")]
		CGRect GetLineFragmentRect (nuint glyphIndex);

		[Sealed]
		[Export ("lineFragmentRectForGlyphAtIndex:effectiveRange:")]
		CGRect GetLineFragmentRect (nuint glyphIndex, out /* nullable NSRangePointer */ NSRange effectiveGlyphRange);

		/* GetLineFragmentRect (NSUInteger, NSRangePointer, bool) */
		[iOS (9,0)]
#if MONOMAC || XAMCORE_4_0
		[Protected]
#else
		[Sealed][Internal]
#endif
		[Export ("lineFragmentRectForGlyphAtIndex:effectiveRange:withoutAdditionalLayout:")]
		CGRect GetLineFragmentRect (nuint glyphIndex, /* nullable NSRangePointer */ IntPtr effectiveGlyphRange, bool withoutAdditionalLayout);

		[iOS (9,0)]
		[Wrap ("GetLineFragmentRect (glyphIndex, IntPtr.Zero)")]
		CGRect GetLineFragmentRect (nuint glyphIndex, bool withoutAdditionalLayout);

		[iOS (9,0)]
#if MONOMAC || XAMCORE_4_0
		[Sealed]
#endif
		[Export ("lineFragmentRectForGlyphAtIndex:effectiveRange:withoutAdditionalLayout:")]
		CGRect GetLineFragmentRect (nuint glyphIndex, out /* nullable NSRangePointer */ NSRange effectiveGlyphRange, bool withoutAdditionalLayout);

		/* GetLineFragmentUsedRect (NSUInteger, NSRangePointer) */
		[Protected]
		[Export ("lineFragmentUsedRectForGlyphAtIndex:effectiveRange:")]
		CGRect GetLineFragmentUsedRect (nuint glyphIndex, /* nullable NSRangePointer */ IntPtr effectiveGlyphRange);

		[Wrap ("GetLineFragmentUsedRect (glyphIndex, IntPtr.Zero)")]
		CGRect GetLineFragmentUsedRect (nuint glyphIndex);

		[Sealed]
		[Export ("lineFragmentUsedRectForGlyphAtIndex:effectiveRange:")]
		CGRect GetLineFragmentUsedRect (nuint glyphIndex, out /* nullable NSRangePointer */ NSRange effectiveGlyphRange);

		/* GetLineFragmentUsedRect (NSUInteger, NSRangePointer, bool) */
		[iOS (9,0)]
#if MONOMAC || XAMCORE_4_0
		[Protected]
#else
		[Sealed][Internal]
#endif
		[Export ("lineFragmentUsedRectForGlyphAtIndex:effectiveRange:withoutAdditionalLayout:")]
		CGRect GetLineFragmentUsedRect (nuint glyphIndex, /* nullable NSRangePointer */ IntPtr effectiveGlyphRange, bool withoutAdditionalLayout);

		[iOS (9,0)]
		[Wrap ("GetLineFragmentUsedRect (glyphIndex, IntPtr.Zero)")]
		CGRect GetLineFragmentUsedRect (nuint glyphIndex, bool withoutAdditionalLayout);

		[iOS (9,0)]
#if MONOMAC || XAMCORE_4_0
		[Sealed]
#endif
		[Export ("lineFragmentUsedRectForGlyphAtIndex:effectiveRange:withoutAdditionalLayout:")]
		CGRect GetLineFragmentUsedRect (nuint glyphIndex, out /* nullable NSRangePointer */ NSRange effectiveGlyphRange, bool withoutAdditionalLayout);

		[Export ("extraLineFragmentRect")]
		CGRect ExtraLineFragmentRect { get; }

		[Export ("extraLineFragmentUsedRect")]
		CGRect ExtraLineFragmentUsedRect { get; }

		[Export ("extraLineFragmentTextContainer")]
		NSTextContainer ExtraLineFragmentTextContainer { get; }

		[Export ("locationForGlyphAtIndex:")]
#if XAMCORE_4_0
		CGPoint GetLocationForGlyph (nuint glyphIndex);
#elif MONOMAC
		CGPoint LocationForGlyphAtIndex (nint glyphIndex);
#else
		CGPoint LocationForGlyphAtIndex (nuint glyphIndex);
#endif

		[Export ("notShownAttributeForGlyphAtIndex:")]
#if XAMCORE_4_0
		bool NotShownAttributeForGlyph (nuint glyphIndex);
#elif MONOMAC
		bool NotShownAttributeForGlyphAtIndex (nint glyphIndex);
#else
		bool NotShownAttributeForGlyphAtIndex (nuint glyphIndex);
#endif

		[Export ("drawsOutsideLineFragmentForGlyphAtIndex:")]
#if XAMCORE_4_0
		bool DrawsOutsideLineFragmentForGlyph (nuint glyphIndex);
#elif MONOMAC
		bool DrawsOutsideLineFragmentForGlyphAt (nint glyphIndex);
#else
		bool DrawsOutsideLineFragmentForGlyphAtIndex (nuint glyphIndex);
#endif

		[Export ("attachmentSizeForGlyphAtIndex:")]
#if XAMCORE_4_0
		CGSize GetAttachmentSizeForGlyph (nuint glyphIndex);
#elif MONOMAC
		CGSize AttachmentSizeForGlyphAt (nint glyphIndex);
#else
		CGSize AttachmentSizeForGlyphAtIndex (nuint glyphIndex);
#endif

		[NoiOS][NoTV]
		[Export ("setLayoutRect:forTextBlock:glyphRange:")]
		void SetLayoutRect (CGRect layoutRect, NSTextBlock forTextBlock, NSRange glyphRange);

		[NoiOS][NoTV]
		[Export ("setBoundsRect:forTextBlock:glyphRange:")]
		void SetBoundsRect (CGRect boundsRect, NSTextBlock forTextBlock, NSRange glyphRange);

		[NoiOS][NoTV]
		[Export ("layoutRectForTextBlock:glyphRange:")]
#if XAMCORE_4_0
		CGRect GetLayoutRect (NSTextBlock block, NSRange glyphRange);
#else
		CGRect LayoutRect (NSTextBlock block, NSRange glyphRange);
#endif

		[NoiOS][NoTV]
		[Export ("boundsRectForTextBlock:glyphRange:")]
#if XAMCORE_4_0
		CGRect GetBoundsRect (NSTextBlock block, NSRange glyphRange);
#else
		CGRect BoundsRect (NSTextBlock block, NSRange glyphRange);
#endif

		/* GetLayoutRect (NSTextBlock, NSUInteger, nullable NSRangePointer) */

		[NoiOS][NoTV]
		[Protected]
		[Export ("layoutRectForTextBlock:atIndex:effectiveRange:")]
		CGRect GetLayoutRect (NSTextBlock block, nuint glyphIndex, IntPtr effectiveGlyphRange);

		[NoiOS][NoTV]
		[Wrap ("GetLayoutRect (block, glyphIndex, IntPtr.Zero)")]
		CGRect GetLayoutRect (NSTextBlock block, nuint glyphIndex);

		[NoiOS][NoTV]
		[Sealed]
		[Export ("layoutRectForTextBlock:atIndex:effectiveRange:")]
		CGRect GetLayoutRect (NSTextBlock block, nuint glyphIndex, out NSRange effectiveGlyphRange);

		/* GetBoundsRect (NSTextBlock, NSUInteger, nullable NSRangePointer) */

		[NoiOS][NoTV]
		[Protected]
		[Export ("boundsRectForTextBlock:atIndex:effectiveRange:")]
		CGRect GetBoundsRect (NSTextBlock block, nuint glyphIndex, IntPtr effectiveGlyphRange);

		[NoiOS][NoTV]
		[Wrap ("GetBoundsRect (block, glyphIndex, IntPtr.Zero)")]
		CGRect GetBoundsRect (NSTextBlock block, nuint glyphIndex);

		[NoiOS][NoTV]
		[Sealed]
		[Export ("boundsRectForTextBlock:atIndex:effectiveRange:")]
		CGRect GetBoundsRect (NSTextBlock block, nuint glyphIndex, out NSRange effectiveGlyphRange);

		/* GetGlyphRange (NSRange, nullable NSRangePointer) */

#if XAMCORE_4_0 || !MONOMAC
		[Protected]
#else
		[Internal][Sealed]
#endif
		[Export ("glyphRangeForCharacterRange:actualCharacterRange:")]
		NSRange GetGlyphRange (NSRange characterRange, IntPtr actualCharacterRange);

		[Wrap ("GetGlyphRange (characterRange, IntPtr.Zero)")]
		NSRange GetGlyphRange (NSRange characterRange);

		[Sealed]
		[Export ("glyphRangeForCharacterRange:actualCharacterRange:")]
		NSRange GetGlyphRange (NSRange characterRange, out NSRange actualCharacterRange);

#if !XAMCORE_4_0
		[NoiOS][NoTV]
		[Obsolete ("Use 'GetGlyphRange' instead.")]
		[Export ("glyphRangeForCharacterRange:actualCharacterRange:")]
		NSRange GlyphRangeForCharacterRange (NSRange charRange, out NSRange actualCharRange);
#endif

		/* GetCharacterRange (NSRange, nullable NSRangePointer) */
#if XAMCORE_4_0 || !MONOMAC
		[Protected]
#else
		[Internal][Sealed]
#endif
		[Export ("characterRangeForGlyphRange:actualGlyphRange:")]
		NSRange GetCharacterRange (NSRange glyphRange, IntPtr actualGlyphRange);

		[Wrap ("GetCharacterRange (glyphRange, IntPtr.Zero)")]
		NSRange GetCharacterRange (NSRange glyphRange);

		[Sealed]
		[Export ("characterRangeForGlyphRange:actualGlyphRange:")]
		NSRange GetCharacterRange (NSRange glyphRange, out NSRange actualGlyphRange);

#if MONOMAC && !XAMCORE_4_0
		[Obsolete ("Use 'GetCharacterRange' instead.")]
		[Export ("characterRangeForGlyphRange:actualGlyphRange:")]
		NSRange CharacterRangeForGlyphRange (NSRange glyphRange, out NSRange actualGlyphRange);
#endif

		[Export ("glyphRangeForTextContainer:")]
		NSRange GetGlyphRange (NSTextContainer container);

		[Export ("rangeOfNominallySpacedGlyphsContainingIndex:")]
#if XAMCORE_4_0
		NSRange GetRangeOfNominallySpacedGlyphsContainingIndex (nuint glyphIndex);
#elif MONOMAC
		NSRange RangeOfNominallySpacedGlyphsContainingIndex (nint glyphIndex);
#else
		NSRange RangeOfNominallySpacedGlyphsContainingIndex (nuint glyphIndex);
#endif

		[Internal]
		[NoiOS][NoTV]
		[Export ("rectArrayForGlyphRange:withinSelectedGlyphRange:inTextContainer:rectCount:")]
		[Availability (Deprecated = Platform.Mac_10_11)]
		IntPtr GetRectArray (NSRange glyphRange, NSRange selectedGlyphRange, IntPtr textContainerHandle, out nuint rectCount);

		[Export ("boundingRectForGlyphRange:inTextContainer:")]
#if XAMCORE_4_0
		CGRect GetBoundingRect (NSRange glyphRange, NSTextContainer container);
#else
		CGRect BoundingRectForGlyphRange (NSRange glyphRange, NSTextContainer container);
#endif

		[Export ("glyphRangeForBoundingRect:inTextContainer:")]
#if XAMCORE_4_0
		NSRange GetGlyphRangeForBoundingRect (CGRect bounds, NSTextContainer container);
#else
		NSRange GlyphRangeForBoundingRect (CGRect bounds, NSTextContainer container);
#endif

		[Export ("glyphRangeForBoundingRectWithoutAdditionalLayout:inTextContainer:")]
#if XAMCORE_4_0
		NSRange GetGlyphRangeForBoundingRectWithoutAdditionalLayout (CGRect bounds, NSTextContainer container);
#else
		NSRange GlyphRangeForBoundingRectWithoutAdditionalLayout (CGRect bounds, NSTextContainer container);
#endif

		[Export ("glyphIndexForPoint:inTextContainer:fractionOfDistanceThroughGlyph:")]
#if XAMCORE_4_0
		nuint GetGlyphIndex (CGPoint point, NSTextContainer container, /* nullable CGFloat */ out nfloat fractionOfDistanceThroughGlyph);
#elif MONOMAC
		nuint GlyphIndexForPointInTextContainer (CGPoint point, NSTextContainer container, ref nfloat fractionOfDistanceThroughGlyph);
#else
		nuint GlyphIndexForPoint (CGPoint point, NSTextContainer container, ref nfloat partialFraction);
#endif

		[Export ("glyphIndexForPoint:inTextContainer:")]
#if XAMCORE_4_0
		nuint GetGlyphIndex (CGPoint point, NSTextContainer container);
#else
		nuint GlyphIndexForPoint (CGPoint point, NSTextContainer container);
#endif

		[Export ("fractionOfDistanceThroughGlyphForPoint:inTextContainer:")]
#if XAMCORE_4_0
		nfloat GetFractionOfDistanceThroughGlyph (CGPoint point, NSTextContainer container);
#else
		nfloat FractionOfDistanceThroughGlyphForPoint (CGPoint point, NSTextContainer container);
#endif

		// GetCharacterIndex (CGPoint, NSTextContainer, nullable CGFloat*)
#if XAMCORE_4_0
		[Protected]
#else
		[Sealed][Internal]
#endif
		[Export ("characterIndexForPoint:inTextContainer:fractionOfDistanceBetweenInsertionPoints:")]
		nuint GetCharacterIndex (CGPoint point, NSTextContainer container, IntPtr fractionOfDistanceBetweenInsertionPoints);

		[Wrap ("GetCharacterIndex (point, container, IntPtr.Zero)")]
		nuint GetCharacterIndex (CGPoint point, NSTextContainer container);

		[Sealed]
		[Export ("characterIndexForPoint:inTextContainer:fractionOfDistanceBetweenInsertionPoints:")]
		nuint GetCharacterIndex (CGPoint point, NSTextContainer container, out nfloat fractionOfDistanceBetweenInsertionPoints);

#if !XAMCORE_4_0
		[Obsolete ("Use 'GetCharacterIndex' instead.")]
		[Export ("characterIndexForPoint:inTextContainer:fractionOfDistanceBetweenInsertionPoints:")]
#if MONOMAC
		nuint CharacterIndexForPoint (CGPoint point, NSTextContainer container, ref nfloat fractionOfDistanceBetweenInsertionPoints);
#else
		nuint CharacterIndexForPoint (CGPoint point, NSTextContainer container, ref nfloat partialFraction);
#endif
#endif

#if XAMCORE_4_0 || !MONOMAC
		[Protected]
#endif
		[Export ("getLineFragmentInsertionPointsForCharacterAtIndex:alternatePositions:inDisplayOrder:positions:characterIndexes:")]
#if XAMCORE_4_0 || !MONOMAC
		nuint GetLineFragmentInsertionPoints (nuint characterIndex, bool alternatePositions, bool inDisplayOrder, IntPtr positions, IntPtr characterIndexes);
#else
		nuint GetLineFragmentInsertionPoints (nuint charIndex, bool aFlag, bool dFlag, IntPtr positions, IntPtr charIndexes);
#endif

		/* GetTemporaryAttributes (NSUInteger, nullable NSRangePointer) */

		[NoiOS][NoTV]
		[Protected]
		[Export ("temporaryAttributesAtCharacterIndex:effectiveRange:")]
		NSDictionary<NSString, NSObject> GetTemporaryAttributes (nuint characterIndex, IntPtr effectiveCharacterRange);

		[NoiOS][NoTV]
		[Wrap ("GetTemporaryAttributes (characterIndex, IntPtr.Zero)")]
		NSDictionary<NSString, NSObject> GetTemporaryAttributes (nuint characterIndex);

		[NoiOS][NoTV]
		[Sealed]
		[Export ("temporaryAttributesAtCharacterIndex:effectiveRange:")]
		NSDictionary<NSString, NSObject> GetTemporaryAttributes (nuint characterIndex, out NSRange effectiveCharacterRange);

		[NoiOS, NoTV, NoWatch]
		[Export ("setTemporaryAttributes:forCharacterRange:")]
		void SetTemporaryAttributes (NSDictionary attrs, NSRange charRange);

		[NoiOS][NoTV]
		[Export ("addTemporaryAttributes:forCharacterRange:")]
#if XAMCORE_4_0
		void AddTemporaryAttributes (NSDictionary<NSString, NSObject> attributes, NSRange characterRange);
#else
		void AddTemporaryAttributes (NSDictionary attrs, NSRange charRange);
#endif

		// This API can take an NSString or managed string, but some related API
		// takes a generic dictionary that can't use a managed string, so for symmetry
		// provide an NSString overload as well.
#if !XAMCORE_4_0
		[Sealed]
#endif
		[NoiOS][NoTV]
		[Export ("removeTemporaryAttribute:forCharacterRange:")]
		void RemoveTemporaryAttribute (NSString attributeName, NSRange characterRange);

#if XAMCORE_4_0
		[Sealed]
#endif
		[NoiOS][NoTV]
		[Export ("removeTemporaryAttribute:forCharacterRange:")]
#if XAMCORE_4_0
		void RemoveTemporaryAttribute (string attributeName, NSRange characterRange);
#else
		void RemoveTemporaryAttribute (string attrName, NSRange charRange);
#endif

		/* GetTemporaryAttribute (NSString, NSUInteger, nullable NSRangePointer) */
		[Protected]
		[NoiOS][NoTV]
		[Export ("temporaryAttribute:atCharacterIndex:effectiveRange:")]
		NSObject GetTemporaryAttribute (NSString attributeName, nuint characterIndex, /* nullable NSRangePointer */ IntPtr effectiveRange);

		[Wrap ("GetTemporaryAttribute (attributeName, characterIndex, IntPtr.Zero)")]
		[NoiOS][NoTV]
		NSObject GetTemporaryAttribute (NSString attributeName, nuint characterIndex);

		[Sealed]
		[NoiOS][NoTV]
		[Export ("temporaryAttribute:atCharacterIndex:effectiveRange:")]
		NSObject GetTemporaryAttribute (NSString attributeName, nuint characterIndex, /* nullable NSRangePointer */ out NSRange effectiveRange);

		/* GetTemporaryAttribute (NSString, NSUInteger, nullable NSRangePointer, NSRange) */

		[Protected]
		[NoiOS][NoTV]
		[Export ("temporaryAttribute:atCharacterIndex:longestEffectiveRange:inRange:")]
		NSObject GetTemporaryAttribute (NSString attributeName, nuint characterIndex, /* nullable NSRangePointer */ IntPtr longestEffectiveRange, NSRange rangeLimit);

		[Wrap ("GetTemporaryAttribute (attributeName, characterIndex, IntPtr.Zero, rangeLimit)")]
		[NoiOS][NoTV]
		NSObject GetTemporaryAttribute (NSString attributeName, nuint characterIndex, NSRange rangeLimit);

		[Sealed]
		[NoiOS][NoTV]
		[Export ("temporaryAttribute:atCharacterIndex:longestEffectiveRange:inRange:")]
		NSObject GetTemporaryAttribute (NSString attributeName, nuint characterIndex, /* nullable NSRangePointer */ out NSRange longestEffectiveRange, NSRange rangeLimit);

		/* GetTemporaryAttributes (NSUInteger, nullable NSRangePointer, NSRange) */

		[Protected]
		[NoiOS][NoTV]
		[Export ("temporaryAttributesAtCharacterIndex:longestEffectiveRange:inRange:")]
		NSDictionary<NSString, NSObject> GetTemporaryAttributes (nuint characterIndex, /* nullable NSRangePointer */ IntPtr longestEffectiveRange, NSRange rangeLimit);

		[Wrap ("GetTemporaryAttributes (characterIndex, IntPtr.Zero, rangeLimit)")]
		[NoiOS][NoTV]
		NSDictionary<NSString, NSObject> GetTemporaryAttributes (nuint characterIndex, NSRange rangeLimit);

		[Sealed]
		[NoiOS][NoTV]
		[Export ("temporaryAttributesAtCharacterIndex:longestEffectiveRange:inRange:")]
		NSDictionary<NSString, NSObject> GetTemporaryAttributes (nuint characterIndex, /* nullable NSRangePointer */ out NSRange longestEffectiveRange, NSRange rangeLimit);

		// This method can take an NSString or managed string, but some related API
		// takes a generic dictionary that can't use a managed string, so for symmetry
		// provide an NSString overload as well.
#if !XAMCORE_4_0
		[Sealed]
#endif
		[NoiOS][NoTV]
		[Export ("addTemporaryAttribute:value:forCharacterRange:")]
		void AddTemporaryAttribute (NSString attributeName, NSObject value, NSRange characterRange);

#if XAMCORE_4_0
		[Sealed]
#endif
		[NoiOS][NoTV]
		[Export ("addTemporaryAttribute:value:forCharacterRange:")]
#if XAMCORE_4_0
		void AddTemporaryAttribute (string attributeName, NSObject value, NSRange characterRange);
#else
		void AddTemporaryAttribute (string attrName, NSObject value, NSRange charRange);
#endif

#if !XAMCORE_4_0
		[NoiOS][NoTV]
		[Availability (Deprecated = Platform.Mac_10_11)]
		[Export ("substituteFontForFont:")]
		NSFont SubstituteFontForFont (NSFont originalFont);
#endif

		[NoiOS][NoTV]
		[Export ("defaultLineHeightForFont:")]
#if XAMCORE_4_0
		nfloat GetDefaultLineHeight (NSFont font);
#else
		nfloat DefaultLineHeightForFont (NSFont theFont);
#endif

		[NoiOS][NoTV]
		[Export ("defaultBaselineOffsetForFont:")]
#if XAMCORE_4_0
		nfloat GetDefaultBaselineOffset (NSFont font);
#else
		nfloat DefaultBaselineOffsetForFont (NSFont theFont);
#endif

		[NullAllowed]
		[Export ("textStorage", ArgumentSemantic.Assign)]
		NSTextStorage TextStorage { get; set; }

		[NoiOS][NoTV]
		[Availability (Deprecated = Platform.Mac_10_11)]
		[Export ("glyphGenerator", ArgumentSemantic.Retain)]
		NSGlyphGenerator GlyphGenerator { get; set; }

		[NoiOS][NoTV]
		[Export ("typesetter", ArgumentSemantic.Retain)]
		NSTypesetter Typesetter { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		INSLayoutManagerDelegate Delegate { get; set; }

		[NoiOS][NoTV]
		[Export ("backgroundLayoutEnabled")]
		bool BackgroundLayoutEnabled { get; set; }

		[NoiOS][NoTV]
		[Availability (Deprecated = Platform.Mac_10_11)]
		[Export ("usesScreenFonts")]
		bool UsesScreenFonts { get; set; }

		[Export ("showsInvisibleCharacters")]
		bool ShowsInvisibleCharacters { get; set; }

		[Export ("showsControlCharacters")]
		bool ShowsControlCharacters { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Please use 'UsesDefaultHyphenation' or 'NSParagraphStyle.HyphenationFactor' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Please use 'UsesDefaultHyphenation' or 'NSParagraphStyle.HyphenationFactor' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Please use 'UsesDefaultHyphenation' or 'NSParagraphStyle.HyphenationFactor' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Please use 'UsesDefaultHyphenation' or 'NSParagraphStyle.HyphenationFactor' instead.")]
		[Unavailable (PlatformName.UIKitForMac)]
		[Advice ("This API is not available when using UIKit on macOS.")]
		[Export ("hyphenationFactor")]
#if MONOMAC
		float HyphenationFactor { get; set; } /* This is defined as float in AppKit headers. */
#else
		nfloat HyphenationFactor { get; set; } /* This is defined as CGFloat in UIKit headers. */
#endif

		[NoiOS][NoTV]
		[Export ("defaultAttachmentScaling")]
		NSImageScaling DefaultAttachmentScaling { get; set; }

		[NoiOS][NoTV]
		[Export ("typesetterBehavior")]
		NSTypesetterBehavior TypesetterBehavior { get; set; }

		[iOS (7,0)]
		[Export ("allowsNonContiguousLayout")]
		bool AllowsNonContiguousLayout { get; set; }

		[Export ("usesFontLeading")]
		bool UsesFontLeading { get; set; }

		[Export ("drawBackgroundForGlyphRange:atPoint:")]
#if XAMCORE_4_0
		void DrawBackground (NSRange glyphsToShow, CGPoint origin);
#else
		void DrawBackgroundForGlyphRange (NSRange glyphsToShow, CGPoint origin);
#endif

		[Export ("drawGlyphsForGlyphRange:atPoint:")]
#if XAMCORE_4_0 || !MONOMAC
		void DrawGlyphs (NSRange glyphsToShow, CGPoint origin);
#else
		void DrawGlyphsForGlyphRange (NSRange glyphsToShow, CGPoint origin);
#endif

		[Protected] // Class can be subclassed, and most methods can be overridden.
		[Mac (10,10)]
		[Export ("getGlyphsInRange:glyphs:properties:characterIndexes:bidiLevels:")]
		nuint GetGlyphs (NSRange glyphRange, IntPtr glyphBuffer, IntPtr properties, IntPtr characterIndexBuffer, IntPtr bidiLevelBuffer);

#if !XAMCORE_4_0 && !MONOMAC
		[Sealed]
#endif
		[Mac (10,10)]
		[Export ("propertyForGlyphAtIndex:")]
		NSGlyphProperty GetProperty (nuint glyphIndex);

#if !XAMCORE_4_0 && !MONOMAC
		[Obsolete ("Use 'GetProperty' instead.")]
		[Export ("propertyForGlyphAtIndex:")]
		NSGlyphProperty PropertyForGlyphAtIndex (nuint glyphIndex);
#endif

		[Mac (10,11)]
		[iOS (9,0)] // Show up in the iOS 7.0 headers, but they can't be found at runtime until iOS 9.
		[Export ("CGGlyphAtIndex:isValidIndex:")]
#if XAMCORE_4_0
		CGGlyph GetGlyph (nuint glyphIndex, out bool isValidIndex);
#elif MONOMAC
		CGGlyph GetCGGlyph (nuint glyphIndex, out bool isValidIndex);
#else
		CGGlyph GetGlyph (nuint glyphIndex, ref bool isValidIndex);
#endif

		[Mac (10,11)]
		[iOS (9,0)] // Show up in the iOS 7.0 headers, but they can't be found at runtime until iOS 9.
		[Export ("CGGlyphAtIndex:")]
#if XAMCORE_4_0
		CGGlyph GetGlyph (nuint glyphIndex);
#elif MONOMAC
		CGGlyph GetCGGlyph (nuint glyphIndex);
#else
		CGGlyph GetGlyph (nuint glyphIndex);
#endif

		[Mac (10,11)]
		[Export ("processEditingForTextStorage:edited:range:changeInLength:invalidatedRange:")]
#if XAMCORE_4_0
		void ProcessEditing (NSTextStorage textStorage, NSTextStorageEditActions editMask, NSRange newCharacterRange, /* NSInteger */ nint delta, NSRange invalidatedCharacterRange);
#else
		void ProcessEditing (NSTextStorage textStorage, NSTextStorageEditActions editMask, NSRange newCharRange, /* NSInteger */ nint delta, NSRange invalidatedCharRange);
#endif

		// This method can only be called from
		// NSLayoutManagerDelegate.ShouldGenerateGlyphs, and that method takes
		// the same IntPtr arguments as this one. This means that creating a
		// version of this method with nice(r) types (arrays instead of
		// IntPtr) is useless, since what the caller has is IntPtrs (from the
		// ShouldGenerateGlyphs parameters). We can revisit this if we ever
		// fix the generator to have support for C-style arrays.
		[Mac (10,11)]
		[Export ("setGlyphs:properties:characterIndexes:font:forGlyphRange:")]
#if XAMCORE_4_0
		void SetGlyphs (IntPtr glyphs, IntPtr properties, IntPtr characterIndexes, NSFont font, NSRange glyphRange);
#else
		void SetGlyphs (IntPtr glyphs, IntPtr props, IntPtr charIndexes, NSFont aFont, NSRange glyphRange);
#endif

#if !(XAMCORE_4_0 || MONOMAC)
		[Sealed]
#endif
		[Mac (10,11)]
		[Export ("truncatedGlyphRangeInLineFragmentForGlyphAtIndex:")]
		NSRange GetTruncatedGlyphRangeInLineFragment (nuint glyphIndex);

#if !(XAMCORE_4_0 || MONOMAC)
		[Obsolete ("Use 'GetTruncatedGlyphRangeInLineFragment' instead.")]
		[Mac (10,11)]
		[Export ("truncatedGlyphRangeInLineFragmentForGlyphAtIndex:")]
		NSRange TruncatedGlyphRangeInLineFragmentForGlyphAtIndex (nuint glyphIndex);
#endif

		[Mac (10,11)]
		[Export ("enumerateLineFragmentsForGlyphRange:usingBlock:")]
		void EnumerateLineFragments (NSRange glyphRange, NSTextLayoutEnumerateLineFragments callback);

		[Mac (10,11)]
		[Export ("enumerateEnclosingRectsForGlyphRange:withinSelectedGlyphRange:inTextContainer:usingBlock:")]
		void EnumerateEnclosingRects (NSRange glyphRange, NSRange selectedRange, NSTextContainer textContainer, NSTextLayoutEnumerateEnclosingRects callback);

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use the overload that takes 'nint glyphCount' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use the overload that takes 'nint glyphCount' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use the overload that takes 'nint glyphCount' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use the overload that takes 'nint glyphCount' instead.")]
		[Unavailable (PlatformName.UIKitForMac)]
		[Advice ("This API is not available when using UIKit on macOS.")]
		[Protected] // Can be overridden
		[Export ("showCGGlyphs:positions:count:font:matrix:attributes:inContext:")]
		void ShowGlyphs (IntPtr glyphs, IntPtr positions, nuint glyphCount, NSFont font, CGAffineTransform textMatrix, NSDictionary attributes, [NullAllowed] CGContext graphicsContext);

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[Protected] // Can be overridden
		[Export ("showCGGlyphs:positions:count:font:textMatrix:attributes:inContext:")]
		void ShowGlyphs (IntPtr glyphs, IntPtr positions, nint glyphCount, NSFont font, CGAffineTransform textMatrix, NSDictionary attributes, [NullAllowed] CGContext graphicsContext);

		// Unfortunately we can't provide a nicer API for this, because it uses C-style arrays.
		// And providing a nicer overload when it's only purpose is to be overridden is useless.
		[Advice ("This method should never be called, only overridden.")] // According to Apple's documentation
		[Protected]
		[Export ("fillBackgroundRectArray:count:forCharacterRange:color:")]
		void FillBackground (IntPtr rectArray, nuint rectCount, NSRange characterRange, NSColor color);

 		[Export ("drawUnderlineForGlyphRange:underlineType:baselineOffset:lineFragmentRect:lineFragmentGlyphRange:containerOrigin:")]
 		void DrawUnderline (NSRange glyphRange, NSUnderlineStyle underlineVal, nfloat baselineOffset, CGRect lineRect, NSRange lineGlyphRange, CGPoint containerOrigin);

 		[Export ("underlineGlyphRange:underlineType:lineFragmentRect:lineFragmentGlyphRange:containerOrigin:")]
 		void Underline (NSRange glyphRange, NSUnderlineStyle underlineVal, CGRect lineRect, NSRange lineGlyphRange, CGPoint containerOrigin);

 		[Export ("drawStrikethroughForGlyphRange:strikethroughType:baselineOffset:lineFragmentRect:lineFragmentGlyphRange:containerOrigin:")]
 		void DrawStrikethrough (NSRange glyphRange, NSUnderlineStyle strikethroughVal, nfloat baselineOffset, CGRect lineRect, NSRange lineGlyphRange, CGPoint containerOrigin);

 		[Export ("strikethroughGlyphRange:strikethroughType:lineFragmentRect:lineFragmentGlyphRange:containerOrigin:")]
 		void Strikethrough (NSRange glyphRange, NSUnderlineStyle strikethroughVal, CGRect lineRect, NSRange lineGlyphRange, CGPoint containerOrigin);

		[NoiOS][NoTV]
 		[Export ("showAttachmentCell:inRect:characterIndex:")]
		void ShowAttachmentCell (NSCell cell, CGRect rect, nuint characterIndex);

		[Mac (10, 14)]
		[TV (12, 0), iOS (12, 0)]
		[Export ("limitsLayoutForSuspiciousContents")]
		bool LimitsLayoutForSuspiciousContents { get; set; }

		[Mac (10,15)]
		[TV (13,0), iOS (13,0)]
		[Export ("usesDefaultHyphenation")]
		bool UsesDefaultHyphenation { get; set; }
	}

	[NoiOS][NoWatch][NoTV]
	[Category]
	[BaseType (typeof (NSLayoutManager))]
	interface NSLayoutManager_NSTextViewSupport {
		[Export ("rulerMarkersForTextView:paragraphStyle:ruler:")]
		NSRulerMarker[] GetRulerMarkers (NSTextView textView, NSParagraphStyle paragraphStyle, NSRulerView ruler);

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

	interface INSLayoutManagerDelegate {}

	[NoWatch] // Header not present in watchOS SDK.
	[iOS (7,0)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSLayoutManagerDelegate {
		[Export ("layoutManagerDidInvalidateLayout:")]
#if MONOMAC && !XAMCORE_4_0
		void LayoutInvalidated (NSLayoutManager sender);
#else
		void DidInvalidatedLayout (NSLayoutManager sender);
#endif

		[iOS (7,0)]
		[Export ("layoutManager:didCompleteLayoutForTextContainer:atEnd:")]
#if XAMCORE_4_0 || !MONOMAC
		void DidCompleteLayout (NSLayoutManager layoutManager, NSTextContainer textContainer, bool layoutFinishedFlag);
#else
		void LayoutCompleted (NSLayoutManager layoutManager, NSTextContainer textContainer, bool layoutFinishedFlag);
#endif

		[NoiOS][NoTV]
		[Export ("layoutManager:shouldUseTemporaryAttributes:forDrawingToScreen:atCharacterIndex:effectiveRange:")]
#if XAMCORE_4_0
		NSDictionary<NSString, NSObject> ShouldUseTemporaryAttributes (NSLayoutManager layoutManager, NSDictionary<NSString, NSObject> temporaryAttributes, bool drawingToScreen, nuint characterIndex, ref NSRange effectiveCharacterRange);
#else
		NSDictionary ShouldUseTemporaryAttributes (NSLayoutManager layoutManager, NSDictionary temporaryAttributes, bool drawingToScreen, nint charIndex, IntPtr effectiveCharRange);
#endif

		[Mac (10,11)]
		[Export ("layoutManager:shouldGenerateGlyphs:properties:characterIndexes:font:forGlyphRange:")]
#if XAMCORE_4_0
		nuint ShouldGenerateGlyphs (NSLayoutManager layoutManager, IntPtr glyphBuffer, IntPtr properties, IntPtr characterIndexes, NSFont font, NSRange glyphRange);
#else
		nuint ShouldGenerateGlyphs (NSLayoutManager layoutManager, IntPtr glyphBuffer, IntPtr props, IntPtr charIndexes, NSFont aFont, NSRange glyphRange);
#endif

		[Mac (10,11)]
		[Export ("layoutManager:lineSpacingAfterGlyphAtIndex:withProposedLineFragmentRect:")]
#if XAMCORE_4_0 || MONOMAC
		nfloat GetLineSpacingAfterGlyph (NSLayoutManager layoutManager, nuint glyphIndex, CGRect rect);
#else
		nfloat LineSpacingAfterGlyphAtIndex (NSLayoutManager layoutManager, nuint glyphIndex, CGRect rect);
#endif

		[Mac (10,11)]
		[Export ("layoutManager:paragraphSpacingBeforeGlyphAtIndex:withProposedLineFragmentRect:")]
#if XAMCORE_4_0 || MONOMAC
		nfloat GetParagraphSpacingBeforeGlyph (NSLayoutManager layoutManager, nuint glyphIndex, CGRect rect);
#else
		nfloat ParagraphSpacingBeforeGlyphAtIndex (NSLayoutManager layoutManager, nuint glyphIndex, CGRect rect);
#endif

		[Mac (10,11)]
		[Export ("layoutManager:paragraphSpacingAfterGlyphAtIndex:withProposedLineFragmentRect:")]
#if XAMCORE_4_0 || MONOMAC
		nfloat GetParagraphSpacingAfterGlyph (NSLayoutManager layoutManager, nuint glyphIndex, CGRect rect);
#else
		nfloat ParagraphSpacingAfterGlyphAtIndex (NSLayoutManager layoutManager, nuint glyphIndex, CGRect rect);
#endif

		[Mac (10,11)]
		[Export ("layoutManager:shouldUseAction:forControlCharacterAtIndex:")]
#if XAMCORE_4_0
		NSControlCharacterAction ShouldUseAction (NSLayoutManager layoutManager, NSControlCharacterAction action, nuint characterIndex);
#else
		NSControlCharacterAction ShouldUseAction (NSLayoutManager layoutManager, NSControlCharacterAction action, nuint charIndex);
#endif

		[Mac (10,11)]
		[Export ("layoutManager:shouldBreakLineByWordBeforeCharacterAtIndex:")]
#if XAMCORE_4_0
		bool ShouldBreakLineByWordBeforeCharacter (NSLayoutManager layoutManager, nuint characterIndex);
#else
		bool ShouldBreakLineByWordBeforeCharacter (NSLayoutManager layoutManager, nuint charIndex);
#endif

		[Mac (10,11)]
		[Export ("layoutManager:shouldBreakLineByHyphenatingBeforeCharacterAtIndex:")]
#if XAMCORE_4_0
		bool ShouldBreakLineByHyphenatingBeforeCharacter (NSLayoutManager layoutManager, nuint characterIndex);
#else
		bool ShouldBreakLineByHyphenatingBeforeCharacter (NSLayoutManager layoutManager, nuint charIndex);
#endif

		[Mac (10,11)]
		[Export ("layoutManager:boundingBoxForControlGlyphAtIndex:forTextContainer:proposedLineFragment:glyphPosition:characterIndex:")]
#if XAMCORE_4_0
		CGRect GetBoundingBox (NSLayoutManager layoutManager, nuint glyphIndex, NSTextContainer textContainer, CGRect proposedRect, CGPoint glyphPosition, nuint characterIndex);
#elif MONOMAC
		CGRect GetBoundingBox (NSLayoutManager layoutManager, nuint glyphIndex, NSTextContainer textContainer, CGRect proposedRect, CGPoint glyphPosition, nuint charIndex);
#else
		CGRect BoundingBoxForControlGlyph (NSLayoutManager layoutManager, nuint glyphIndex, NSTextContainer textContainer, CGRect proposedRect, CGPoint glyphPosition, nuint charIndex);
#endif

		[Mac (10,11)]
		[Export ("layoutManager:textContainer:didChangeGeometryFromSize:")]
		void DidChangeGeometry (NSLayoutManager layoutManager, NSTextContainer textContainer, CGSize oldSize);

		[iOS (9,0)]
		[Mac (10,11)]
		[Export ("layoutManager:shouldSetLineFragmentRect:lineFragmentUsedRect:baselineOffset:inTextContainer:forGlyphRange:")]
		bool ShouldSetLineFragmentRect (NSLayoutManager layoutManager, ref CGRect lineFragmentRect, ref CGRect lineFragmentUsedRect, ref nfloat baselineOffset, NSTextContainer textContainer, NSRange glyphRange);
	}

	[NoWatch, TV (13,0), Mac (10,15), iOS (13,0)]
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

	[ThreadSafe]
	[BaseType (typeof (NSObject))]
	interface NSParagraphStyle : NSSecureCoding, NSMutableCopying {
		[Export ("lineSpacing")]
		nfloat LineSpacing { get; [NotImplemented] set; }

		[Export ("paragraphSpacing")]
		nfloat ParagraphSpacing { get; [NotImplemented] set; }

		[Export ("alignment")]
		TextAlignment Alignment { get; [NotImplemented] set; }

		[Export ("headIndent")]
		nfloat HeadIndent { get; [NotImplemented] set; }

		[Export ("tailIndent")]
		nfloat TailIndent { get; [NotImplemented] set; }

		[Export ("firstLineHeadIndent")]
		nfloat FirstLineHeadIndent { get; [NotImplemented] set; }

		[Export ("minimumLineHeight")]
		nfloat MinimumLineHeight { get; [NotImplemented] set; }

		[Export ("maximumLineHeight")]
		nfloat MaximumLineHeight { get; [NotImplemented] set; }

		[Export ("lineBreakMode")]
		LineBreakMode LineBreakMode { get; [NotImplemented] set; }

		[Export ("baseWritingDirection")]
		NSWritingDirection BaseWritingDirection { get; [NotImplemented] set; }

		[Export ("lineHeightMultiple")]
		nfloat LineHeightMultiple { get; [NotImplemented] set; }

		[Export ("paragraphSpacingBefore")]
		nfloat ParagraphSpacingBefore { get; [NotImplemented] set; }

		[Export ("hyphenationFactor")]
		float HyphenationFactor { get; [NotImplemented] set; } // Returns a float, not nfloat.

		[Static]
		[Export ("defaultWritingDirectionForLanguage:")]
		NSWritingDirection GetDefaultWritingDirection ([NullAllowed] string languageName);

#if MONOMAC && !XAMCORE_4_0
		[Obsolete ("Use the 'GetDefaultWritingDirection' method instead.")]
		[Static]
		[Export ("defaultWritingDirectionForLanguage:")]
		NSWritingDirection DefaultWritingDirection ([NullAllowed] string languageName);
#endif

		[Static]
		[Export ("defaultParagraphStyle", ArgumentSemantic.Copy)]
		NSParagraphStyle Default { get; }

#if MONOMAC && !XAMCORE_4_0
		[Obsolete ("Use the 'Default' property instead.")]
		[Static]
		[Export ("defaultParagraphStyle", ArgumentSemantic.Copy)]
		NSParagraphStyle DefaultParagraphStyle { get; [NotImplemented] set; }
#endif

		[iOS (7,0)]
		[Export ("defaultTabInterval")]
		nfloat DefaultTabInterval { get; [NotImplemented] set; }

		[iOS (7,0)]
		[Export ("tabStops", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSTextTab[] TabStops { get; [NotImplemented] set; }

		[iOS (9,0)]
		[Mac (10,11)]
		[Export ("allowsDefaultTighteningForTruncation")]
		bool AllowsDefaultTighteningForTruncation { get; [NotImplemented] set; }

		[NoiOS, NoTV, NoWatch]
		[Export ("textBlocks")]
#if XAMCORE_4_0
		NSTextBlock [] TextBlocks { get; [NotImplemented] set; }
#else
		NSTextTableBlock [] TextBlocks { get; [NotImplemented] set; }
#endif

		[NoiOS, NoTV, NoWatch]
		[Export ("textLists")]
		NSTextList[] TextLists { get; [NotImplemented] set; }

		[NoiOS, NoTV, NoWatch]
		[Export ("tighteningFactorForTruncation")]
		float TighteningFactorForTruncation { get; [NotImplemented] set; } /* float, not CGFloat */

		[NoiOS, NoTV, NoWatch]
		[Export ("headerLevel")]
		nint HeaderLevel { get; [NotImplemented] set; }
	}

	[ThreadSafe]
	[BaseType (typeof (NSParagraphStyle))]
	interface NSMutableParagraphStyle {
		[Export ("lineSpacing")]
		[Override]
		nfloat LineSpacing { get; set; }

		[Export ("alignment")]
		[Override]
		TextAlignment Alignment { get; set; }

		[Export ("headIndent")]
		[Override]
		nfloat HeadIndent { get; set; }

		[Export ("tailIndent")]
		[Override]
		nfloat TailIndent { get; set; }

		[Export ("firstLineHeadIndent")]
		[Override]
		nfloat FirstLineHeadIndent { get; set; }

		[Export ("minimumLineHeight")]
		[Override]
		nfloat MinimumLineHeight { get; set; }

		[Export ("maximumLineHeight")]
		[Override]
		nfloat MaximumLineHeight { get; set; }

		[Export ("lineBreakMode")]
		[Override]
		LineBreakMode LineBreakMode { get; set; }

		[Export ("baseWritingDirection")]
		[Override]
		NSWritingDirection BaseWritingDirection { get; set; }

		[Export ("lineHeightMultiple")]
		[Override]
		nfloat LineHeightMultiple { get; set; }

		[Export ("paragraphSpacing")]
		[Override]
		nfloat ParagraphSpacing { get; set; }

		[Export ("paragraphSpacingBefore")]
		[Override]
		nfloat ParagraphSpacingBefore { get; set; }

		[Export ("hyphenationFactor")]
		[Override]
		float HyphenationFactor { get; set; } // Returns a float, not nfloat.

		[iOS (7,0)]
		[Export ("defaultTabInterval")]
		[Override]
		nfloat DefaultTabInterval { get; set; }

		[iOS (7,0)]
		[Export ("tabStops", ArgumentSemantic.Copy)]
		[Override]
		[NullAllowed]
		NSTextTab[] TabStops { get; set; }

		[iOS (9,0)]
		[Mac (10,11)]
		[Override]
		[Export ("allowsDefaultTighteningForTruncation")]
		bool AllowsDefaultTighteningForTruncation { get; set; }

		[iOS (9,0)]
		[Export ("addTabStop:")]
		void AddTabStop (NSTextTab textTab);

		[iOS (9,0)]
		[Export ("removeTabStop:")]
		void RemoveTabStop (NSTextTab textTab);

		[iOS (9,0)]
		[Export ("setParagraphStyle:")]
		void SetParagraphStyle (NSParagraphStyle paragraphStyle);

		[NoiOS, NoTV, NoWatch]
		[Override]
		[Export ("textBlocks")]
#if XAMCORE_4_0
		NSTextBlock [] TextBlocks { get; set; }
#else
		NSTextTableBlock [] TextBlocks { get; set; }
#endif

		[NoiOS, NoTV, NoWatch]
		[Override]
		[Export ("textLists")]
		NSTextList [] TextLists { get; set; }

		[NoiOS, NoTV, NoWatch]
		[Export ("tighteningFactorForTruncation")]
		[Override]
		float TighteningFactorForTruncation { get; set; } /* float, not CGFloat */

		[NoiOS, NoTV, NoWatch]
		[Export ("headerLevel")]
		[Override]
		nint HeaderLevel { get; set; }
	}
}
