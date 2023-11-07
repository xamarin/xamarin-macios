//
// NSAttributedString.cs: extensions for NSAttributedString
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2013 Xamarin Inc

#nullable enable

#if MONOMAC

using System;

using AppKit;
using WebKit;
//using CoreText;

namespace Foundation 
{
	public partial class NSAttributedString
	{
		public NSAttributedString (string str,
			NSFont? font = null,
			NSColor? foregroundColor = null,
			NSColor? backgroundColor = null,
			NSColor? strokeColor = null,
			NSColor? underlineColor = null,
			NSColor? strikethroughColor = null,
			NSUnderlineStyle underlineStyle = NSUnderlineStyle.None,
			NSUnderlineStyle strikethroughStyle = NSUnderlineStyle.None,
			NSParagraphStyle? paragraphStyle = null,
			float strokeWidth = 0,
			NSShadow? shadow = null,
			NSUrl? link = null,
			bool superscript = false,
			NSTextAttachment? attachment = null,
			NSLigatureType ligature = NSLigatureType.Default,
			float baselineOffset = 0,
			float kerningAdjustment = 0,
			float obliqueness = 0,
			float expansion = 0,
			NSCursor? cursor = null,
			string? toolTip = null,
			int characterShape = 0,
			NSGlyphInfo? glyphInfo = null,
			NSArray? writingDirection = null,
			bool markedClauseSegment = false,
			NSTextLayoutOrientation verticalGlyphForm = NSTextLayoutOrientation.Horizontal,
			NSTextAlternatives? textAlternatives = null,
			NSSpellingState spellingState = NSSpellingState.None) : this (str, NSStringAttributes.ToDictionary (
				font: font,
				foregroundColor: foregroundColor,
				backgroundColor: backgroundColor,
				strokeColor: strokeColor,
				underlineColor: underlineColor,
				strikethroughColor: strikethroughColor,
				underlineStyle: underlineStyle,
				strikethroughStyle: strikethroughStyle,
				paragraphStyle: paragraphStyle,
				strokeWidth: strokeWidth,
				shadow: shadow,
				link: link,
				superscript: superscript,
				attachment: attachment,
				ligature: ligature,
				baselineOffset: baselineOffset,
				kerningAdjustment: kerningAdjustment,
				obliqueness: obliqueness,
				expansion: expansion,
				cursor: cursor,
				toolTip: toolTip,
				characterShape: characterShape,
				glyphInfo: glyphInfo,
				writingDirection: writingDirection,
				markedClauseSegment: markedClauseSegment,
				verticalGlyphForm: verticalGlyphForm,
				textAlternatives: textAlternatives,
				spellingState: spellingState
			))
		{
		}

		internal NSAttributedString (NSData data, NSAttributedStringDataType type, out NSDictionary resultDocumentAttributes)
		{
			switch (type) {
			case NSAttributedStringDataType.DocFormat:
				Handle = new NSAttributedString (data, out resultDocumentAttributes).Handle;
				break;
			case NSAttributedStringDataType.HTML:
				Handle = InitWithHTML (data, out resultDocumentAttributes);
				break;
			case NSAttributedStringDataType.RTF:
				Handle = InitWithRtf (data, out resultDocumentAttributes);
				break;
			case NSAttributedStringDataType.RTFD:
				Handle = InitWithRtfd (data, out resultDocumentAttributes);
				break;
			default:
				throw new ArgumentException("Error creating NSAttributedString.");
			}

			if (Handle == IntPtr.Zero)
				throw new ArgumentException("Error creating NSAttributedString.");
		}

		public static NSAttributedString CreateWithRTF (NSData rtfData, out NSDictionary resultDocumentAttributes)
		{
			return new NSAttributedString (rtfData, NSAttributedStringDataType.RTF, out resultDocumentAttributes);
		}

		public static NSAttributedString CreateWithRTFD (NSData rtfdData, out NSDictionary resultDocumentAttributes)
		{
			return new NSAttributedString (rtfdData, NSAttributedStringDataType.RTFD, out resultDocumentAttributes);
		}

		public static NSAttributedString CreateWithHTML (NSData htmlData, out NSDictionary resultDocumentAttributes)
		{
			return new NSAttributedString (htmlData, NSAttributedStringDataType.HTML, out resultDocumentAttributes);
		}

		public static NSAttributedString CreateWithDocFormat (NSData wordDocFormat, out NSDictionary docAttributes)
		{
			return new NSAttributedString (wordDocFormat, NSAttributedStringDataType.DocFormat, out docAttributes);
		}

		public NSStringAttributes? GetAppKitAttributes (nint location, out NSRange effectiveRange)
		{
			var attr = GetAttributes (location, out effectiveRange);
			return attr is null ? null : new NSStringAttributes (attr);
		}

		public NSStringAttributes? GetAppKitAttributes (nint location, out NSRange longestEffectiveRange, NSRange rangeLimit)
		{
			var attr = GetAttributes (location, out longestEffectiveRange, rangeLimit);
			return attr is null ? null : new NSStringAttributes (attr);
		}
	}
}

#endif // MONOMAC
