//
// NSTextTableBlock.cs
//
// Author:
//  Chris Hamons <chris.hamons@xamarin.com>
//
// Copyright 2016 Xamarin Inc. (http://xamarin.com)


using System;


namespace AppKit
{
#if !XAMCORE_4_0
	public partial class NSTextTableBlock
	{
		[Obsolete ("macOS 10.12 does not allow creation via this constructor")]
		public NSTextTableBlock ()
		{
		}
	}
#endif
}
