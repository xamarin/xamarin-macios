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
#if NET
		[UnsupportedOSPlatform ("ios6.0")]
#if IOS
		[Obsolete ("Starting with ios6.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.iOS, 6, 0)]
#endif
		public static readonly NSString DisableBidiProcessing = _DisableBidiProcessing;
		public static readonly NSString ForceEmbeddingLevel = _ForceEmbeddingLevel;
	}
}
#endif
