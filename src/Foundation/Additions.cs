// Copyright 2014 Xamarin Inc. All rights reserved.

#if !MONOMAC && !WATCH
#if XAMCORE_2_0

using System;

namespace Foundation {

	// NSIndexPath UIKit Additions Reference
	// https://developer.apple.com/library/ios/#documentation/UIKit/Reference/NSIndexPath_UIKitAdditions/Reference/Reference.html
	public partial class NSIndexPath {

		// to avoid a lot of casting inside user source code we decided to expose `int` returning properties
		// https://trello.com/c/5SoMWz30/336-nsindexpath-expose-longrow-longsection-longitem-instead-of-changing-the-int-nature-of-them
		// their usage makes it very unlikely to ever exceed 2^31

		public int Row { 
			get { return (int) LongRow; }
		}

		public int Section { 
			get { return (int) LongSection; }
		}
	}
}

#endif // XAMCORE_2_0
#endif // !MONOMAC && !WATCH
