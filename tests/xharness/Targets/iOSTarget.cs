using System.IO;
using System.Xml;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;

namespace Xharness.Targets
{
	// iOS here means Xamarin.iOS, not iOS as opposed to tvOS/watchOS.
	public class iOSTarget : Target
	{
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
			info_plist.LoadWithoutNetworkAccess (Path.Combine (TargetDirectory, "Info.plist"));
			BundleIdentifier = info_plist.GetCFBundleIdentifier ();
			info_plist.SetMinimumOSVersion (GetMinimumOSVersion (info_plist.GetMinimumOSVersion ()));
			info_plist.SetUIDeviceFamily (UIDeviceFamily);
			Harness.Save (info_plist, target_info_plist);
		}
	}
}

