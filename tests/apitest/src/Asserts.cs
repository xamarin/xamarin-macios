using System;
using System.Reflection;

using NUnit.Framework;
using Xamarin.Tests;

using ObjCRuntime;
using Foundation;

namespace Xamarin.Mac.Tests
{
	public static class Asserts
	{
		public static bool IsAtLeastYosemite {
			get {
				return PlatformHelper.ToMacVersion (PlatformHelper.GetHostApiPlatform ()) >= Platform.Mac_10_10;
			}
		}

		public static bool IsAtLeastElCapitan {
			get {
				return PlatformHelper.ToMacVersion (PlatformHelper.GetHostApiPlatform ()) >= Platform.Mac_10_11;
			}
		}

		public static void EnsureYosemite ()
		{
			if (!IsAtLeastYosemite)
				Assert.Pass ("This test requires Yosemite. Skipping");
		}

		public static void EnsureMavericks ()
		{
			if (PlatformHelper.ToMacVersion (PlatformHelper.GetHostApiPlatform ()) < Platform.Mac_10_9)
				Assert.Pass ("This test requires Mavericks. Skipping");
		}

		public static void EnsureMountainLion ()
		{
			if (PlatformHelper.ToMacVersion (PlatformHelper.GetHostApiPlatform ()) < Platform.Mac_10_8)
				Assert.Pass ("This test requires Mountain Lion. Skipping");
		}

		public static void Ensure64Bit ()
		{
			if (IntPtr.Size == 4)
				Assert.Pass ("This test requires 64-bit.  Skipping");
		}

		public static bool SkipDueToAvailabilityAttribute (ICustomAttributeProvider member)
		{
			if (member == null)
				return false;
			return !member.IsAvailableOnHostPlatform ();
		}
	}
}
