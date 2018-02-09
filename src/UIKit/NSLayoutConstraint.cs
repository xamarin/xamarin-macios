//
// NSLayoutConstraint.cs:
//
// Authors:
//  Paola Villarreal <paola.villarreal@xamarin.com>
//
// Copyright 2015, Xamarin Inc
//

#if !WATCH && XAMCORE_2_0

using System;
using Foundation;
using ObjCRuntime;

namespace UIKit {
	partial class NSLayoutConstraint {
		// This solves the duplicate selector export problem while not breaking the API.
		public static NSLayoutConstraint Create (NSObject view1, NSLayoutAttribute attribute1, NSLayoutRelation relation, 
				NSObject view2, NSLayoutAttribute attribute2, nfloat multiplier, nfloat constant)
		{
			return Create ((INativeObject) view1, attribute1, relation, view2, attribute2, multiplier, constant);
		}

		[iOS (10, 0)]
		public NSLayoutAnchor<AnchorType> FirstAnchor<AnchorType> () where AnchorType : NSObject
		{
			return Runtime.GetNSObject<NSLayoutAnchor<AnchorType>> (_FirstAnchor ());
		}

		[iOS (10, 0)]
		public NSLayoutAnchor<AnchorType> SecondAnchor<AnchorType> () where AnchorType : NSObject
		{
			return Runtime.GetNSObject<NSLayoutAnchor<AnchorType>> (_SecondAnchor ());
		}
	}
}

#endif // !WATCH
