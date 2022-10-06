//
// XamMacArch.cs
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2014 Xamarin Inc.

using System;

namespace Xamarin.Mac.Tasks {
	[Flags]
	public enum XamMacArch {
		Default = 0,
		i386 = 1,
		x86_64 = 2,
		ARM64 = 4,
	}
}
