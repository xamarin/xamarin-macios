// Copyright 2015 Xamarin Inc.

using System;
using System.Runtime.Versioning;

using Foundation;
using ModelIO;

#nullable enable

namespace GLKit {

#if NET
	[SupportedOSPlatform ("ios9.0")]
	[SupportedOSPlatform ("macos10.11")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos")]
	[UnsupportedOSPlatform ("tvos12.0")]
	[UnsupportedOSPlatform ("macos10.14")]
	[UnsupportedOSPlatform ("ios12.0")]
#if TVOS
	[Obsolete ("Starting with tvos12.0 use 'Metal' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif MONOMAC
	[Obsolete ("Starting with macos10.14 use 'Metal' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
	[Obsolete ("Starting with ios12.0 use 'Metal' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
	public partial class GLKMesh {
		public static GLKMesh []? FromAsset (MDLAsset asset, out MDLMesh [] sourceMeshes, out NSError error)
		{
			NSArray aret;

			var ret = FromAsset (asset, out aret, out error);
			sourceMeshes = NSArray.FromArray<MDLMesh> (aret);
			return ret;
		}
	}
}
