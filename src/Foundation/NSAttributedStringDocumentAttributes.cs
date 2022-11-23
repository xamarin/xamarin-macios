//
// NSAttributedStringDocumentAttributes.cs
//
// Authors:
//   Rolf Bjarne Kvinge (rolf@xamarin.com)
//
// Copyright 2022 Microsoft Corp

#nullable enable

using System;

#if HAS_APPKIT
using AppKit;
#endif
using Foundation;
#if HAS_UIKIT
using UIKit;
#endif

namespace Foundation {
	public partial class NSAttributedStringDocumentAttributes : DictionaryContainer {
#if !COREBUILD
		public NSAttributedStringDocumentAttributes () { }
		public NSAttributedStringDocumentAttributes (NSDictionary? dictionary) : base (dictionary) { }

		public NSStringEncoding? StringEncoding {
			get {
				return (NSStringEncoding?) (long?) GetNIntValue (NSAttributedStringDocumentAttributeKey.NSCharacterEncodingDocumentAttribute);
			}
			set {
				SetNumberValue (NSAttributedStringDocumentAttributeKey.NSCharacterEncodingDocumentAttribute, (nint?) (long?) value);
			}
		}

		public NSString? WeakDocumentType {
			get {
				return GetNSStringValue (NSAttributedStringDocumentAttributeKey.NSDocumentTypeDocumentAttribute);
			}
			set {
				SetStringValue (NSAttributedStringDocumentAttributeKey.NSDocumentTypeDocumentAttribute, value);
			}
		}

		public NSDocumentType DocumentType {
			get {
				var s = WeakDocumentType;

				if (s == NSAttributedStringDocumentType.NSPlainTextDocumentType)
					return NSDocumentType.PlainText;
				if (s == NSAttributedStringDocumentType.NSRtfdTextDocumentType)
					return NSDocumentType.RTFD;
				if (s == NSAttributedStringDocumentType.NSRtfTextDocumentType)
					return NSDocumentType.RTF;
				if (s == NSAttributedStringDocumentType.NSHtmlTextDocumentType)
					return NSDocumentType.HTML;

#if __MACOS__
				if (s == NSAttributedStringDocumentType.NSMacSimpleTextDocumentType)
					return NSDocumentType.MacSimpleText;
				if (s == NSAttributedStringDocumentType.NSDocFormatTextDocumentType)
					return NSDocumentType.DocFormat;
				if (s == NSAttributedStringDocumentType.NSWordMLTextDocumentType)
					return NSDocumentType.WordML;
				if (s == NSAttributedStringDocumentType.NSWebArchiveTextDocumentType)
					return NSDocumentType.WebArchive;
				if (s == NSAttributedStringDocumentType.NSOfficeOpenXMLTextDocumentType)
					return NSDocumentType.OfficeOpenXml;
				if (s == NSAttributedStringDocumentType.NSOpenDocumentTextDocumentType)
					return NSDocumentType.OpenDocument;
#endif // __MACOS__

				return NSDocumentType.Unknown;
			}

			set {
				NSString? documentType = null;
				switch (value) {
				case NSDocumentType.PlainText:
					documentType = NSAttributedStringDocumentType.NSPlainTextDocumentType;
					break;
				case NSDocumentType.RTFD:
					documentType = NSAttributedStringDocumentType.NSRtfdTextDocumentType;
					break;
				case NSDocumentType.RTF:
					documentType = NSAttributedStringDocumentType.NSRtfTextDocumentType;
					break;
				case NSDocumentType.HTML:
					documentType = NSAttributedStringDocumentType.NSHtmlTextDocumentType;
					break;
#if __MACOS__
				case NSDocumentType.MacSimpleText:
					documentType = NSAttributedStringDocumentType.NSMacSimpleTextDocumentType;
					break;
				case NSDocumentType.DocFormat:
					documentType = NSAttributedStringDocumentType.NSDocFormatTextDocumentType;
					break;
				case NSDocumentType.WordML:
					documentType = NSAttributedStringDocumentType.NSWordMLTextDocumentType;
					break;
				case NSDocumentType.WebArchive:
					documentType = NSAttributedStringDocumentType.NSWebArchiveTextDocumentType;
					break;
				case NSDocumentType.OfficeOpenXml:
					documentType = NSAttributedStringDocumentType.NSOfficeOpenXMLTextDocumentType;
					break;
				case NSDocumentType.OpenDocument:
					documentType = NSAttributedStringDocumentType.NSOpenDocumentTextDocumentType;
					break;
#endif // __MACOS__
				}

				if (documentType is not null)
					WeakDocumentType= documentType;
			}
		}
#endif // !COREBUILD
	}
}
