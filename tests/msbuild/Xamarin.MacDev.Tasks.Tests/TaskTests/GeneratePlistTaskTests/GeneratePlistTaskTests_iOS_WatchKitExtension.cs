using System.Linq;
using NUnit.Framework;
using Xamarin.MacDev;

namespace Xamarin.iOS.Tasks
{
	[TestFixture]
	public class GeneratePlistTaskTests_iOS_WatchKitExtension : GeneratePlistTaskTests_iOS
	{
		public override void ConfigureTask ()
		{
			base.ConfigureTask ();
			Task.IsWatchExtension = true;
		}

		/// <summary>
		/// watchOS 1 WatchKitExtension projects should always have the UIRequiredDeviceCapabilities watch-companion value defined.
		/// </summary>
		[Test]
		public void WatchCompanion ()
		{
			Assert.That (CompiledPlist.ContainsKey (ManifestKeys.UIRequiredDeviceCapabilities), "#1");

			var requiredDeviceCapabilities = CompiledPlist.Get<PArray> (ManifestKeys.UIRequiredDeviceCapabilities).ToStringArray ();
			Assert.That (requiredDeviceCapabilities.Contains ("watch-companion"), "#2");
		}
	}
}
