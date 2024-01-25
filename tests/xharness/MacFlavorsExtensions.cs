using System;

using Microsoft.DotNet.XHarness.iOS.Shared;

namespace Xharness {
	public static class MacFlavorsExtensions {

		public static TestPlatform ToTestPlatform (this MacFlavors flavor)
		{
			return flavor switch {
				MacFlavors.Console => TestPlatform.Mac,
				MacFlavors.Full => TestPlatform.Mac_Full,
				MacFlavors.Modern => TestPlatform.Mac_Modern,
				MacFlavors.System => TestPlatform.Mac_System,
				MacFlavors.DotNet => TestPlatform.Mac,
				MacFlavors.MacCatalyst => TestPlatform.MacCatalyst,
				(MacFlavors.Full | MacFlavors.Modern) => (TestPlatform.Mac | TestPlatform.Mac_Modern),
				_ => throw new NotImplementedException (flavor.ToString ()),
			};
		}
	}
}
