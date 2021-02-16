using Xamarin.MacDev.Tasks;
using Xamarin.MacDev;

namespace Xamarin.Mac.Tasks
{
	public abstract class DetectSdkLocationsTaskBase : DetectSdkLocationsCoreTaskBase
	{
		protected override IAppleSdkVersion GetDefaultSdkVersion ()
		{
			var v = CurrentSdk.GetInstalledSdkVersions (false);
			return v.Count > 0 ? v [v.Count - 1] : AppleSdkVersion.UseDefault;
		}

		protected override string GetDefaultXamarinSdkRoot ()
		{
			return Sdks.XamMac.FrameworkDirectory;
		}
	}
}
