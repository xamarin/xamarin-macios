using System;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;

namespace Xharness {
	public class iOSTestProject : TestProject {
		public bool SkipiOSVariation;
		public bool SkiptvOSVariation;
		public bool SkipDeviceVariations;
		public bool BuildOnly;

		public iOSTestProject (TestLabel label, string path)
			: base (label, path, true)
		{
			Name = System.IO.Path.GetFileNameWithoutExtension (path);
		}

		public bool IsSupported (DevicePlatform devicePlatform, string productVersion)
		{
			return true;
		}

		public override TestProject Clone ()
		{
			return CompleteClone (new iOSTestProject (Label, Path));
		}

		protected override TestProject CompleteClone (TestProject project)
		{
			var rv = (iOSTestProject) project;
			rv.SkipiOSVariation = SkipiOSVariation;
			rv.SkiptvOSVariation = SkiptvOSVariation;
			rv.SkipDeviceVariations = SkipDeviceVariations;
			rv.BuildOnly = BuildOnly;
			return base.CompleteClone (rv);
		}
	}
}
