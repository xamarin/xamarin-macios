using System.IO;
using System.Xml;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;

namespace Xharness.Targets {
	// iOS here means Xamarin.iOS, not iOS as opposed to tvOS/watchOS.
	public abstract class iOSTarget : Target {
		public iOSTestProject TestProject;

		public MonoNativeInfo MonoNativeInfo => TestProject.MonoNativeInfo;

		protected override void PostProcessExecutableProject ()
		{
			base.PostProcessExecutableProject ();

			inputProject.FixArchitectures (SimulatorArchitectures, DeviceArchitectures);
			inputProject.FixInfoPListInclude (Suffix);
			inputProject.SetExtraLinkerDefs ("extra-linker-defs" + ExtraLinkerDefsSuffix + ".xml");

			ProjectGuid = inputProject.GetProjectGuid ();
		}

		protected override void UpdateInfoPList ()
		{
			XmlDocument info_plist = new XmlDocument ();
			var target_info_plist = Path.Combine (TargetDirectory, "Info" + Suffix + ".plist");
			var original_info_plist_include = HarnessConfiguration.EvaluateRootTestsDirectory (OriginalInfoPListInclude);
			info_plist.LoadWithoutNetworkAccess (Path.Combine (TargetDirectory, original_info_plist_include));
			BundleIdentifier = info_plist.GetCFBundleIdentifier ();
			info_plist.SetMinimumOSVersion (GetMinimumOSVersion (info_plist.GetMinimumOSVersion ()));
			info_plist.SetUIDeviceFamily (UIDeviceFamily);
			info_plist.Save (target_info_plist, Harness);
		}
	}
}
