using System.Linq;

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
			Task.TargetArchitectures = "ARM64";
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

		[Test]
		public void RequiredDeviceCapabilities ()
		{
			PArray array;

			Assert.IsTrue (CompiledPlist.TryGetValue (ManifestKeys.UIRequiredDeviceCapabilities, out array), "#1");
			Assert.IsTrue (array.OfType<PString> ().Any (x => x.Value == "arm64"), "#2");
			Assert.IsFalse (array.OfType<PString> ().Any (x => x.Value == "armv6"), "#3");
			Assert.IsFalse (array.OfType<PString> ().Any (x => x.Value == "armv7"), "#4");
		}
	}
}

