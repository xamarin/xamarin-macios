//
// MLDictionaryFeatureProvider.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

using System;
using Foundation;
using ObjCRuntime;

namespace CoreML {
	public partial class MLDictionaryFeatureProvider {

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		public MLFeatureValue this [string featureName] {
			get { return GetFeatureValue (featureName); }
		}
	}
}
