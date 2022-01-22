//
// CTTypesetterOptionKeyCompat.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2018 Microsoft Corporation.
//

#if !NET
using System;
using ObjCRuntime;
using Foundation;
using System.Runtime.Versioning;

namespace CoreText {
	public static partial class CTTypesetterOptionKey {
		[Deprecated (PlatformName.iOS, 6, 0)]
		public static readonly NSString DisableBidiProcessing = _DisableBidiProcessing;
		public static readonly NSString ForceEmbeddingLevel = _ForceEmbeddingLevel;
	}
}
#endif
