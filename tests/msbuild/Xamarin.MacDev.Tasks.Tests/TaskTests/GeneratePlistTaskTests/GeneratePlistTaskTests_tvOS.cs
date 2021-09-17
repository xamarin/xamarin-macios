using NUnit.Framework;
using Xamarin.MacDev;

namespace Xamarin.iOS.Tasks
{
	[TestFixture]
	public class GeneratePlistTaskTests_tvOS : GeneratePlistTaskTests_Core
	{
		public override void ConfigureTask ()
		{
			base.ConfigureTask ();
			Task.DefaultSdkVersion = Sdks.TVOS.GetClosestInstalledSdk (AppleSdkVersion.V9_0, true).ToString ();
			Task.TargetFrameworkMoniker = "Xamarin.TVOS,v1.0";
		}
	}
}
