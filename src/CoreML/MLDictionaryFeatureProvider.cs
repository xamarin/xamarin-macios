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
using System.Runtime.Versioning;

namespace CoreML {
#if NET
	[SupportedOSPlatform ("tvos11.0")]
	[SupportedOSPlatform ("macos10.13")]
	[SupportedOSPlatform ("ios11.0")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	public partial class MLDictionaryFeatureProvider {

		public MLFeatureValue this [string featureName] {
			get { return GetFeatureValue (featureName); }
		}
	}
}
