using NUnit.Framework;
using Xamarin.MacDev;

namespace Xamarin.iOS.Tasks
{
	[TestFixture]
	public class GeneratePlistTaskTests_iOS : GeneratePlistTaskTests_Core
	{
		public override void ConfigureTask ()
		{
			base.ConfigureTask ();
			Task.DefaultSdkVersion = IPhoneSdks.Native.GetClosestInstalledSdk (IPhoneSdkVersion.V6_1, true).ToString ();
			Task.TargetFrameworkIdentifier = "Xamarin.iOS";
		}

		[Test]
		public override void BundleExecutable ()
		{
			base.BundleExecutable ();
			// Adding ".app" to the assembly name isn't allowed because iOS may fail to launch the app.
			Task.AssemblyName = "AssemblyName.app";
			Assert.IsFalse (Task.Execute (), "#1");
		}

		[Test]
		public override void BundleName ()
		{
			Assert.That (CompiledPlist.ContainsKey (ManifestKeys.CFBundleName), "#1");
			Assert.AreEqual (CompiledPlist.Get<PString> (ManifestKeys.CFBundleName).Value, displayName, "#2");
		}
	}
}

