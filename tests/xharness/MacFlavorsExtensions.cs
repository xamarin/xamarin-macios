using System;
using Microsoft.DotNet.XHarness.iOS.Shared;

namespace Xharness {
	public static class MacFlavorsExtensions {

		public static TestPlatform ToTestPlatform (this MacFlavors flavor)
		{
			return flavor switch
			{
				MacFlavors.Console => TestPlatform.Mac,
				MacFlavors.Full => TestPlatform.Mac_Full,
				MacFlavors.Modern => TestPlatform.Mac_Modern,
				MacFlavors.System => TestPlatform.Mac_System,
				_ => throw new NotImplementedException (flavor.ToString ()),
			};
		}
	}
}
