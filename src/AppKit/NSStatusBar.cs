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
using System.Runtime.InteropServices;

namespace AppKit
{
	public enum NSStatusItemLength
	{
		Variable = -1,
		Square = -2
	}

	public partial class NSStatusBar
	{
		public NSStatusItem CreateStatusItem (NSStatusItemLength length)
		{
#if NO_NFLOAT_OPERATORS
			return CreateStatusItem (new NFloat ((long) length));
#else
			return CreateStatusItem (length);
#endif
		}
	}
}
#endif // !__MACCATALYST__
