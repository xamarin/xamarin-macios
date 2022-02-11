// Copyright 2014 Xamarin Inc
//
//
using System;
using System.Runtime.Versioning;

namespace Foundation {

#if NET
	[SupportedOSPlatform ("ios7.0")]
	[SupportedOSPlatform ("macos10.9")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos")]
#endif
	public partial class NSUrlComponents {

		// we cannot make the ToString method depend on a new (iOS8) selector as the type was added before (iOS7)
//		public override string ToString ()
//		{
//			return _String;
//		}
	}
}
