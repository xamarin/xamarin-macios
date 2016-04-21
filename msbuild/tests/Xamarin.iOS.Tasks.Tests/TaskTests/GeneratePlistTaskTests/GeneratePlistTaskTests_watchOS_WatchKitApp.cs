using NUnit.Framework;
using Xamarin.MacDev;

namespace Xamarin.iOS.Tasks
{
	[TestFixture]
	public class GeneratePlistTaskTests_watchOS_WatchKitApp : GeneratePlistTaskTests_Core
	{
		public override void ConfigureTask ()
		{
			base.ConfigureTask ();
			Task.DefaultSdkVersion = IPhoneSdks.Watch.GetClosestInstalledSdk (IPhoneSdkVersion.V2_0, true).ToString ();
			Task.TargetFrameworkIdentifier = "Xamarin.WatchOS";
			Task.IsWatchApp = true;
		}

		public override void XamarinVersion ()
		{
			// WatchKit App doesn't require the com.xamarin.ios key.
		}
	}
}

