using NUnit.Framework;
using Xamarin.MacDev;

namespace Xamarin.iOS.Tasks
{
	[TestFixture]
	public class GeneratePlistTaskTests_iOS_AppExtension : GeneratePlistTaskTests_iOS
	{
		public override void ConfigureTask ()
		{
			base.ConfigureTask ();
			Task.IsAppExtension = true;
		}

		[Test]
		public override void BundlePackageType ()
		{
			Assert.That (CompiledPlist.ContainsKey (ManifestKeys.CFBundlePackageType), "#1");
			Assert.AreEqual (CompiledPlist.Get<PString> (ManifestKeys.CFBundlePackageType).Value, "XPC!", "#2");
		}
	}
}
