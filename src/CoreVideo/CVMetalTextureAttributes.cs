// 
// CVMetalTextureAttributes.cs
//
// Authors: Alex Soto (alexsoto@microsoft.com)
//
// Copyright 2017 Xamarin Inc.
//

#if XAMCORE_2_0 && !WATCH
using System;
using XamCore.Foundation;
using XamCore.Metal;

namespace XamCore.CoreVideo {
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
