//
// Mac-specific Helpers
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//  Aaron Bockover <abock@xamarin.com>
//
// Copyright 2012-2015 Xamarin Inc. All rights reserved.
//

using System;

namespace Xamarin.Tests {
	public static class Mac {
		public static readonly Version Version_10_10 = new Version (10, 10);
		public static readonly Version Version_10_11 = new Version (10, 11);

		static PlatformInfo host => PlatformInfo.Host;

		public static bool CheckSystemVersion (int major, int minor) => host.Version >= new Version (major, minor);
		public static bool CheckSystemVersion (int major, int minor, int build) => host.Version >= new Version (major, minor, build);
		public static bool IsYosemiteOrHigher => IsAtLeast (Version_10_10);
		public static bool IsAtLeast (int major, int minor) => IsAtLeast (new Version (major, minor));
		public static bool IsAtLeast (Version version) => host.IsMac && host.Version >= version;
	}
}
