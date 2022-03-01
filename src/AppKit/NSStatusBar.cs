//
// NSStatusBar.cs: Proprietary extensions to MonoMac NSStatusBar
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2012 Xamarin Inc
//

#if !__MACCATALYST__

using System;
using System.Runtime.Versioning;

namespace AppKit
{
	public enum NSStatusItemLength
	{
		Variable = -1,
		Square = -2
	}

#if NET
	[SupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("maccatalyst")]
#endif
	public partial class NSStatusBar
	{
		public NSStatusItem CreateStatusItem (NSStatusItemLength length)
		{
			return CreateStatusItem ((float)length);
		}
	}
}
#endif // !__MACCATALYST__
