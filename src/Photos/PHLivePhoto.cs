// Copyright 2015 Xamarin Inc

using System;
using Foundation;
using System.Runtime.Versioning;

namespace Photos {

#if NET
	[SupportedOSPlatform ("ios9.1")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("macos10.12")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	public partial class PHLivePhoto {

		public const int RequestIdInvalid = 0;
	}
}
