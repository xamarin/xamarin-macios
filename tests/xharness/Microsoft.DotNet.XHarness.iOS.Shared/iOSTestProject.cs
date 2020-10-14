using System;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;

namespace Microsoft.DotNet.XHarness.iOS.Shared {

	public class iOSTestProject : TestProject
	{
		public bool SkipiOSVariation;
		public bool SkipiOS32Variation;
		public bool SkipiOS64Variation;
		public bool SkipwatchOSVariation; // skip both
		public bool SkipwatchOSARM64_32Variation;
		public bool SkipwatchOS32Variation;
		public bool SkiptvOSVariation;
		public bool SkipTodayExtensionVariation;
		public bool SkipDeviceVariations;
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

		public override TestProject Clone ()
		{
			var rv = (iOSTestProject) base.Clone ();
			rv.SkipiOSVariation = SkipiOSVariation;
			rv.SkipiOS32Variation = SkipiOS32Variation;
			rv.SkipiOS64Variation = SkipiOS64Variation;
			rv.SkipwatchOSVariation = SkipwatchOSVariation;
			rv.SkipwatchOSARM64_32Variation = SkipwatchOSARM64_32Variation;
			rv.SkipwatchOS32Variation = SkipwatchOS32Variation;
			rv.SkiptvOSVariation = SkiptvOSVariation;
			rv.SkipTodayExtensionVariation = SkipTodayExtensionVariation;
			rv.SkipDeviceVariations = SkipDeviceVariations;
			rv.BuildOnly = BuildOnly;
			return rv;
		}
	}
}
