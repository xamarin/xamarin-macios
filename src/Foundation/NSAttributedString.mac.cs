//
// NSAttributedString.cs: extensions for NSAttributedString
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2013 Xamarin Inc

#if MONOMAC

using System;

using AppKit;
using WebKit;
//using CoreText;

namespace Foundation 
{
	public partial class NSAttributedString
	{
		public NSAttributedString (string str, NSStringAttributes attributes)
			: this (str, attributes != null ? attributes.Dictionary : null)
		{
		}

		public NSAttributedString (string str,
			NSFont font = null,
			NSColor foregroundColor = null,
			NSColor backgroundColor = null,
			NSColor strokeColor = null,
			NSColor underlineColor = null,
			NSColor strikethroughColor = null,
			NSUnderlineStyle underlineStyle = NSUnderlineStyle.None,
			NSUnderlineStyle strikethroughStyle = NSUnderlineStyle.None,
			NSParagraphStyle paragraphStyle = null,
			float strokeWidth = 0,
			NSShadow shadow = null,
			NSUrl link = null,
			bool superscript = false,
			NSTextAttachment attachment = null,
			NSLigatureType ligature = NSLigatureType.Default,
			float baselineOffset = 0,
			float kerningAdjustment = 0,
			float obliqueness = 0,
			float expansion = 0,
			NSCursor cursor = null,
			string toolTip = null,
			int characterShape = 0,
			NSGlyphInfo glyphInfo = null,
			NSArray writingDirection = null,
			bool markedClauseSegment = false,
			NSTextLayoutOrientation verticalGlyphForm = NSTextLayoutOrientation.Horizontal,
			NSTextAlternatives textAlternatives = null,
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

		public NSStringAttributes GetAppKitAttributes (nint location, out NSRange effectiveRange)
		{
			var attr = GetAttributes (location, out effectiveRange);
			return attr == null ? null : new NSStringAttributes (attr);
		}

		public NSStringAttributes GetAppKitAttributes (nint location, out NSRange longestEffectiveRange, NSRange rangeLimit)
		{
			var attr = GetAttributes (location, out longestEffectiveRange, rangeLimit);
			return attr == null ? null : new NSStringAttributes (attr);			
		}
	}

	public partial class NSAttributedStringDocumentAttributes : DictionaryContainer {
		public NSAttributedStringDocumentAttributes () : base (new NSMutableDictionary ()) {}
		public NSAttributedStringDocumentAttributes (NSDictionary dictionary) : base (dictionary) {}

		public WebPreferences WebPreferences { 
			get {
				NSObject value;
				Dictionary.TryGetValue (NSStringAttributeKey.NSWebPreferencesDocumentOption, out value);
				return value as WebPreferences;
			}
			set {
				if (value == null)
					RemoveValue (NSStringAttributeKey.NSWebPreferencesDocumentOption);
				else
					Dictionary [NSStringAttributeKey.NSWebPreferencesDocumentOption] = value;
			}
		}
		public NSObject WebResourceLoadDelegate { 
			get {
				NSObject value;
				Dictionary.TryGetValue (NSStringAttributeKey.NSWebResourceLoadDelegateDocumentOption, out value);
				return value;
			}
			set {
				if (value == null)
					RemoveValue (NSStringAttributeKey.NSWebResourceLoadDelegateDocumentOption);
				else
					Dictionary [NSStringAttributeKey.NSWebResourceLoadDelegateDocumentOption] = value;
			}
		}

		public NSStringEncoding? StringEncoding {
			get {
				var value = GetInt32Value (NSStringAttributeKey.NSCharacterEncodingDocumentOption);
				if (value == null)
					return null;
				else
					return (NSStringEncoding) value.Value;
			}
			set {
				SetNumberValue (NSStringAttributeKey.NSCharacterEncodingDocumentOption, (int?) value);
			}
		}

		public NSString WeakDocumentType {
			get {
				return GetNSStringValue (NSStringAttributeKey.NSDocumentTypeDocumentOption);
			}
			set {
				SetStringValue (NSStringAttributeKey.NSDocumentTypeDocumentOption, value);
			}
		}

		public NSDocumentType DocumentType {
			get {
				var s = GetNSStringValue (NSStringAttributeKey.NSDocumentTypeDocumentOption);

				if (s == NSStringAttributeKey.NSPlainTextDocumentType)
					return NSDocumentType.PlainText;
				if (s == NSStringAttributeKey.NSRtfTextDocumentType)
					return NSDocumentType.RTF;
				if (s == NSStringAttributeKey.NSRtfdTextDocumentType)
					return NSDocumentType.RTFD;
				if (s == NSStringAttributeKey.NSMacSimpleTextDocumentType)
					return NSDocumentType.MacSimpleText;
				if (s == NSStringAttributeKey.NSHTMLTextDocumentType)
					return NSDocumentType.HTML;
				if (s == NSStringAttributeKey.NSDocFormatTextDocumentType)
					return NSDocumentType.DocFormat;
				if (s == NSStringAttributeKey.NSWordMLTextDocumentType)
					return NSDocumentType.WordML;
				if (s == NSStringAttributeKey.NSWebArchiveTextDocumentType)
					return NSDocumentType.WebArchive;
				if (s == NSStringAttributeKey.NSOfficeOpenXMLTextDocumentType)
					return NSDocumentType.OfficeOpenXml;
				if (s == NSStringAttributeKey.NSOpenDocumentTextDocumentType)
					return NSDocumentType.OpenDocument;
				return NSDocumentType.Unknown;
			}

			set {
				switch (value){
				case NSDocumentType.PlainText:
					SetStringValue (NSStringAttributeKey.NSDocumentTypeDocumentOption, NSStringAttributeKey.NSPlainTextDocumentType);
					break;
				case NSDocumentType.RTFD:
					SetStringValue (NSStringAttributeKey.NSDocumentTypeDocumentOption, NSStringAttributeKey.NSRtfdTextDocumentType);
					break;
				case NSDocumentType.RTF:
					SetStringValue (NSStringAttributeKey.NSDocumentTypeDocumentOption, NSStringAttributeKey.NSRtfTextDocumentType);
					break;
				case NSDocumentType.HTML:
					SetStringValue (NSStringAttributeKey.NSDocumentTypeDocumentOption, NSStringAttributeKey.NSHTMLTextDocumentType);
					break;
				case NSDocumentType.MacSimpleText:
					SetStringValue (NSStringAttributeKey.NSDocumentTypeDocumentOption, NSStringAttributeKey.NSMacSimpleTextDocumentType);
					break;
				case NSDocumentType.DocFormat:
					SetStringValue (NSStringAttributeKey.NSDocumentTypeDocumentOption, NSStringAttributeKey.NSDocFormatTextDocumentType);
					break;
				case NSDocumentType.WordML:
					SetStringValue (NSStringAttributeKey.NSDocumentTypeDocumentOption, NSStringAttributeKey.NSWordMLTextDocumentType);
					break;
				case NSDocumentType.WebArchive:
					SetStringValue (NSStringAttributeKey.NSDocumentTypeDocumentOption, NSStringAttributeKey.NSWebArchiveTextDocumentType);
					break;
				case NSDocumentType.OfficeOpenXml:
					SetStringValue (NSStringAttributeKey.NSDocumentTypeDocumentOption, NSStringAttributeKey.NSOfficeOpenXMLTextDocumentType);
					break;
				case NSDocumentType.OpenDocument:
					SetStringValue (NSStringAttributeKey.NSDocumentTypeDocumentOption, NSStringAttributeKey.NSOpenDocumentTextDocumentType);
					break;
				}
			}
		}

		public NSDictionary WeakDefaultAttributes {
			get {
				NSObject value;
				Dictionary.TryGetValue (NSStringAttributeKey.NSDefaultAttributesDocumentOption, out value);
				return value as NSDictionary;
			}
			set {
				if (value == null)
					RemoveValue (NSStringAttributeKey.NSDefaultAttributesDocumentOption);
				else
					Dictionary [NSStringAttributeKey.NSDefaultAttributesDocumentOption] = value;
			}
		}

		public NSUrl BaseUrl {
			get { 
				return GetNativeValue <NSUrl> (NSStringAttributeKey.NSBaseURLDocumentOption);
			} 
			set { 
				SetNativeValue (NSStringAttributeKey.NSBaseURLDocumentOption, value);
			} 
		}

		public string TextEncodingName { 
			get {
				return (string)GetNSStringValue (NSStringAttributeKey.NSTextEncodingNameDocumentOption);
			}
			set {
				SetStringValue (NSStringAttributeKey.NSTextEncodingNameDocumentOption, (NSString)value);
			}
		}
		public float? TextSizeMultiplier { 
			get { return GetFloatValue (NSStringAttributeKey.NSTextSizeMultiplierDocumentOption); }
			set { SetNumberValue (NSStringAttributeKey.NSTextSizeMultiplierDocumentOption, (float?) value); }
		}

		public float? Timeout { 
			get { return GetFloatValue (NSStringAttributeKey.NSTimeoutDocumentOption); }
			set { SetNumberValue (NSStringAttributeKey.NSTimeoutDocumentOption, (float?)value); }
		}
	}
}

#endif // MONOMAC
