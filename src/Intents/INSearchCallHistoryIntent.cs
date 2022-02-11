//
// INSearchCallHistoryIntent.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#if !(NET && __MACOS__)
#if !TVOS
using System;
using Foundation;
using ObjCRuntime;
using System.Runtime.Versioning;

namespace Intents {
#if NET
	[SupportedOSPlatform ("ios10.0")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("ios15.0")]
#if IOS
	[Obsolete ("Starting with ios15.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("tvos")]
#if MONOMAC
	[Obsolete ("Starting with macos10.0 unavailable on macOS, will be removed in the future.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
	public partial class INSearchCallHistoryIntent {

		public bool? Unseen {
			get { return WeakUnseen?.BoolValue; }
		}
	}
}
#endif
#endif // !(NET && __MACOS__)
