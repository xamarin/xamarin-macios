//
// CTTypesetterOptionKeyCompat.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2018 Microsoft Corporation.
//

#if !XAMCORE_4_0
using System;
using ObjCRuntime;
using Foundation;
using System.Runtime.Versioning;

namespace CoreText {
	public static partial class CTTypesetterOptionKey {
#if !NET
		[Deprecated (PlatformName.iOS, 6, 0)]
#else
		[UnsupportedOSPlatform ("ios6.0")]
#endif
		public static readonly NSString DisableBidiProcessing = _DisableBidiProcessing;
		public static readonly NSString ForceEmbeddingLevel = _ForceEmbeddingLevel;
	}
}
#endif
