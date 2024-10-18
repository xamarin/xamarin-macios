using System;
using Microsoft.DotNet.XHarness.iOS.Shared;

namespace Xharness {
	public static class MacFlavorsExtensions {

		public static TestPlatform ToTestPlatform (this MacFlavors flavor)
		{
			return flavor switch {
				MacFlavors.DotNet => TestPlatform.Mac,
				MacFlavors.MacCatalyst => TestPlatform.MacCatalyst,
				_ => throw new NotImplementedException (flavor.ToString ()),
			};
		}
	}
}
