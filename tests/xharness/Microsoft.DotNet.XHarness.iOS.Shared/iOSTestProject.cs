using System;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;

namespace Microsoft.DotNet.XHarness.iOS.Shared {

	public class iOSTestProject : TestProject
	{
		public bool SkipiOSVariation;
		public bool SkipwatchOSVariation; // skip both
		public bool SkipwatchOSARM64_32Variation;
		public bool SkipwatchOS32Variation;
		public bool SkiptvOSVariation;
		public bool BuildOnly;

		public iOSTestProject ()
		{
		}

		public iOSTestProject (string path, bool isExecutableProject = true)
			: base (path, isExecutableProject)
		{
			Name = System.IO.Path.GetFileNameWithoutExtension (path);
		}

		public bool IsSupported (DevicePlatform devicePlatform, string productVersion)
		{
			if (MonoNativeInfo == null)
				return true;
			var min_version = MonoNativeHelper.GetMinimumOSVersion (devicePlatform, MonoNativeInfo.Flavor);
			return Version.Parse (productVersion) >= Version.Parse (min_version);
		}
	}
}
