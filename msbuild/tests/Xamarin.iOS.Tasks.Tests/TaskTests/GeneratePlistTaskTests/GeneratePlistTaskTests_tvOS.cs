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
			Task.DefaultSdkVersion = IPhoneSdks.TVOS.GetClosestInstalledSdk (IPhoneSdkVersion.V9_0, true).ToString ();
			Task.TargetFrameworkIdentifier = "Xamarin.TVOS";
		}
	}
}

