//
// NSLayoutConstraint.cs:
//
// Authors:
//  Paola Villarreal <paola.villarreal@xamarin.com>
//
// Copyright 2015, Xamarin Inc
//

#if !WATCH

using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.UIKit {
	partial class NSLayoutConstraint {
		// This solves the duplicate selector export problem while not breaking the API.
		public static NSLayoutConstraint Create (NSObject view1, NSLayoutAttribute attribute1, NSLayoutRelation relation, 
				NSObject view2, NSLayoutAttribute attribute2, nfloat multiplier, nfloat constant)
		{
			return Create ((INativeObject) view1, attribute1, relation, view2, attribute2, multiplier, constant);
		}
			
	}
}

#endif // !WATCH
