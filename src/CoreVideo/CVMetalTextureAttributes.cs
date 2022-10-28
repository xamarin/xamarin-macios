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

#nullable enable

namespace CoreVideo {
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
