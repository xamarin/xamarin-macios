using System.Linq;
using NUnit.Framework;
using Xamarin.MacDev;

namespace Xamarin.iOS.Tasks
{
	[TestFixture]
	public class GeneratePlistTaskTests_watchOS_WatchKitExtension : GeneratePlistTaskTests_watchOS
	{
		public override void ConfigureTask ()
		{
			base.ConfigureTask ();
			Task.IsWatchExtension = true;
		}

		/// <summary>
		/// watchOS 2 WatchKitExtension projects shouldn't have the UIRequiredDeviceCapabilities watch-companion value defined.
		/// As this is the only value added to UIRequiredDeviceCapabilities for a watchOS project, the test passes if undefined.
		/// </summary>
		[Test]
		public void NoWatchCompanion ()
		{
			Assert.That (CompiledPlist.ContainsKey (ManifestKeys.UIRequiredDeviceCapabilities) == false, "#1");
		}
	}
}
