using NUnit.Framework;

namespace Xamarin.MacDev.Tasks {
	[TestFixture (false)]
	public class GeneratePlistTaskTests_watchOS_WatchKitApp : GeneratePlistTaskTests_watchOS {
		public GeneratePlistTaskTests_watchOS_WatchKitApp (bool isDotNet)
			: base (isDotNet)
		{
		}

		protected override void ConfigureTask (bool isDotNet)
		{
			base.ConfigureTask (isDotNet);
			Task.IsWatchApp = true;
		}

		public override void XamarinVersion ()
		{
			// WatchKit App doesn't require the com.xamarin.ios key.
		}
	}
}
