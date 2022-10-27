//
// MLDictionaryFeatureProvider.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#nullable enable

using System;
using Foundation;
using ObjCRuntime;

namespace CoreML {
	public partial class MLDictionaryFeatureProvider {

		public MLFeatureValue? this [string featureName] {
			get { return GetFeatureValue (featureName); }
		}
	}
}
