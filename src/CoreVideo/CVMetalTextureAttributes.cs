// 
// CVMetalTextureAttributes.cs
//
// Authors: Alex Soto (alexsoto@microsoft.com)
//
// Copyright 2017 Xamarin Inc.
//

#if !WATCH
using System;
using Foundation;
using Metal;
using System.Runtime.Versioning;

#nullable enable

namespace CoreVideo {
#if NET
	[SupportedOSPlatform ("ios11.0")]
	[SupportedOSPlatform ("macos10.13")]
	[SupportedOSPlatform ("tvos11.0")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	public partial class CVMetalTextureAttributes : DictionaryContainer {

		public MTLTextureUsage? Usage {
			get {
				return (MTLTextureUsage?) (uint?) GetNUIntValue (CVMetalTextureAttributesKeys.UsageKey);
			}
			set {
				SetNumberValue (CVMetalTextureAttributesKeys.UsageKey, (nuint?) (uint?) value);
			}
		}
	}
}
#endif
