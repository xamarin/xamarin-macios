//
// NSAttributedStringDocumentAttributes.cs
//
// Authors:
//   Rolf Bjarne Kvinge (rolf@xamarin.com)
//
// Copyright 2022 Microsoft Corp

#nullable enable

using System;

using Foundation;

namespace Foundation {
	public partial class NSAttributedStringDocumentAttributes : DictionaryContainer {
#if !COREBUILD
		public NSAttributedStringDocumentAttributes () { }
		public NSAttributedStringDocumentAttributes (NSDictionary? dictionary) : base (dictionary) { }
#endif // !COREBUILD
	}
}
