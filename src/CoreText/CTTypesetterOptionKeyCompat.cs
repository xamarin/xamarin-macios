//
// CTTypesetterOptionKeyCompat.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2018 Microsoft Corporation.
//

#nullable enable

#if !NET
using System;
using ObjCRuntime;
using Foundation;

namespace CoreText {
	public static partial class CTTypesetterOptionKey {
#if NET
		[ObsoletedOSPlatform ("ios6.0")]
#else
		[Deprecated (PlatformName.iOS, 6, 0)]
#endif
		public static readonly NSString DisableBidiProcessing = _DisableBidiProcessing;
		public static readonly NSString ForceEmbeddingLevel = _ForceEmbeddingLevel;
	}
}
#endif
