#if __MACOS__
using System;
using System.Reflection;

using NUnit.Framework;
using Xamarin.Tests;
using Xamarin.Utils;

using ObjCRuntime;
using Foundation;

namespace Xamarin.Mac.Tests {
	public static class Asserts {
		public static bool IsAtLeastYosemite {
			get {
				return TestRuntime.CheckXcodeVersion (6, 1);
			}
		}

		public static bool IsAtLeastElCapitan {
			get {
				return TestRuntime.CheckXcodeVersion (7, 0);
			}
		}

		public static void EnsureYosemite ()
		{
			if (!IsAtLeastYosemite)
				Assert.Pass ("This test requires Yosemite. Skipping");
		}

		public static void EnsureMavericks ()
		{
			TestRuntime.AssertXcodeVersion (6, 0);
		}

		public static void EnsureMountainLion ()
		{
			// We're always running on at least Mountain Lion
		}

		public static void Ensure64Bit ()
		{
			if (IntPtr.Size == 4)
				Assert.Pass ("This test requires 64-bit.  Skipping");
		}

		public static bool SkipDueToAvailabilityAttribute (ICustomAttributeProvider member)
		{
			if (member is null)
				return false;
			return !member.IsAvailableOnHostPlatform ();
		}
	}
}
#endif // __MACOS__
