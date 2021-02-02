using Xamarin.MacDev.Tasks;
using Xamarin.MacDev;

namespace Xamarin.Mac.Tasks
{
	public abstract class DetectSdkLocationsTaskBase : DetectSdkLocationsCoreTaskBase
	{
		protected override IAppleSdkVersion GetDefaultSdkVersion ()
		{
			return MacOSXSdkVersion.GetDefault (CurrentSdk);
		}

		protected override string GetDefaultXamarinSdkRoot ()
		{
			return Sdks.XamMac.FrameworkDirectory;
		}
	}
}
