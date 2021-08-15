using NUnit.Framework;

namespace Xamarin.iOS.Tasks
{
	[TestFixture]
	public class GeneratePlistTaskTests_watchOS_WatchKitApp : GeneratePlistTaskTests_watchOS
	{
		public override void ConfigureTask ()
		{
			base.ConfigureTask ();
			Task.IsWatchApp = true;
		}

		public override void XamarinVersion ()
		{
			// WatchKit App doesn't require the com.xamarin.ios key.
		}
	}
}
