//
// NSMutableFontCollection.cs
//
// Author:
//  Chris Hamons <chris.hamons@xamarin.com>
//
// Copyright 2016 Xamarin Inc. (http://xamarin.com)


using System;

#nullable enable

namespace AppKit {
#if !NET
	public partial class NSMutableFontCollection {
		[Obsolete ("macOS 10.12 does not allow creation via this constructor")]
		public NSMutableFontCollection ()
		{
		}
	}
#endif
}
