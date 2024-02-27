using NUnit.Framework;

using Xamarin.MacDev;
using Xamarin.Tests;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	[TestFixture (true)]
	[TestFixture (false)]
	public class GeneratePlistTaskTests_tvOS : GeneratePlistTaskTests_Core {
		protected override ApplePlatform Platform => ApplePlatform.TVOS;

		public GeneratePlistTaskTests_tvOS (bool isDotNet)
			: base (isDotNet)
		{
		}

		protected override void ConfigureTask (bool isDotNet)
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.TVOS);

			base.ConfigureTask (isDotNet);
			Task.DefaultSdkVersion = Sdks.TVOS.GetClosestInstalledSdk (AppleSdkVersion.V9_0, true).ToString ();
			Task.TargetFrameworkMoniker = isDotNet ? TargetFramework.DotNet_tvOS_String : TargetFramework.Xamarin_TVOS_1_0.ToString ();
		}
	}
}
