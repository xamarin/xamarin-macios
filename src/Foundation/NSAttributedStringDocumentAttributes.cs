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
#endif // !COREBUILD
	}
}
