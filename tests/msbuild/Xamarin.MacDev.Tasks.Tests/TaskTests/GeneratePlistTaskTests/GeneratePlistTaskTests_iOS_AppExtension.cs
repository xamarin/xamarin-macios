using NUnit.Framework;
using Xamarin.MacDev;

namespace Xamarin.MacDev.Tasks {
	[TestFixture (true)]
	[TestFixture (false)]
	public class GeneratePlistTaskTests_iOS_AppExtension : GeneratePlistTaskTests_iOS {
		public GeneratePlistTaskTests_iOS_AppExtension (bool isDotNet)
			: base (isDotNet)
		{
		}

		protected override void ConfigureTask (bool isDotNet)
		{
			base.ConfigureTask (isDotNet);
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
