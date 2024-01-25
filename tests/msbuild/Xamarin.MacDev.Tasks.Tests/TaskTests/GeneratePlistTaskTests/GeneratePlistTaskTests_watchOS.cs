using NUnit.Framework;

using Xamarin.MacDev;

using Xamarin.Tests;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	[TestFixture (false)]
	public abstract class GeneratePlistTaskTests_watchOS : GeneratePlistTaskTests_Core {
		protected override ApplePlatform Platform => ApplePlatform.WatchOS;

		public GeneratePlistTaskTests_watchOS (bool isDotNet)
			: base (isDotNet)
		{
		}

		protected override void ConfigureTask (bool isDotNet)
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.WatchOS);
			Configuration.AssertLegacyXamarinAvailable ();

			base.ConfigureTask (isDotNet);
			Task.DefaultSdkVersion = Sdks.Watch.GetClosestInstalledSdk (AppleSdkVersion.V2_0, true).ToString ();
			Task.TargetFrameworkMoniker = isDotNet ? TargetFramework.DotNet_watchOS_String : TargetFramework.Xamarin_WatchOS_1_0.ToString ();
		}
	}
}
