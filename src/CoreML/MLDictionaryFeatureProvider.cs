//
// MLDictionaryFeatureProvider.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#if XAMCORE_2_0

using System;
using Foundation;
using ObjCRuntime;

namespace CoreML {
	public partial class MLDictionaryFeatureProvider {

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		public MLFeatureValue this [string featureName] {
			get { return GetFeatureValue (featureName); }
		}
	}
}
#endif
