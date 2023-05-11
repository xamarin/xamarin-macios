using System;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;

namespace Xharness {
	public class iOSTestProject : TestProject {
		public bool SkipiOSVariation;
		public bool SkipwatchOSVariation; // skip both
		public bool SkipwatchOSARM64_32Variation;
		public bool SkiptvOSVariation;
		public bool SkipTodayExtensionVariation;
		public bool SkipDeviceVariations;
		public bool BuildOnly;

		public iOSTestProject (TestLabel label, string path, bool isExecutableProject = true)
			: base (label, path, isExecutableProject)
		{
			Name = System.IO.Path.GetFileNameWithoutExtension (path);
		}

		public bool IsSupported (DevicePlatform devicePlatform, string productVersion)
		{
			if (MonoNativeInfo is null)
				return true;
			var min_version = MonoNativeHelper.GetMinimumOSVersion (devicePlatform);
			return Version.Parse (productVersion) >= Version.Parse (min_version);
		}

		public override TestProject Clone ()
		{
			return CompleteClone (new iOSTestProject (Label, Path, IsExecutableProject));
		}

		protected override TestProject CompleteClone (TestProject project)
		{
			var rv = (iOSTestProject) project;
			rv.SkipiOSVariation = SkipiOSVariation;
			rv.SkipwatchOSVariation = SkipwatchOSVariation;
			rv.SkipwatchOSARM64_32Variation = SkipwatchOSARM64_32Variation;
			rv.SkiptvOSVariation = SkiptvOSVariation;
			rv.SkipTodayExtensionVariation = SkipTodayExtensionVariation;
			rv.SkipDeviceVariations = SkipDeviceVariations;
			rv.BuildOnly = BuildOnly;
			return base.CompleteClone (rv);
		}
	}
}
