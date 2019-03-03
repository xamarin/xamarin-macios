using System.IO;
using System.Linq;
using Microsoft.Build.Utilities;
using NUnit.Framework;
using Xamarin.MacDev;

namespace Xamarin.iOS.Tasks
{
	class CustomCompileEntitlements : CompileEntitlements
	{
		protected override MobileProvision GetMobileProvision (MobileProvisionPlatform platform, string uuid)
		{
			if (File.Exists (ProvisioningProfile))
				return MobileProvision.LoadFromFile (ProvisioningProfile);

			return null;
		}
	}

	[TestFixture]
	public class CompileEntitlementsTaskTests : TestBase
	{
		CustomCompileEntitlements task {
			get; set;
		}

		string compiledEntitlements {
			get; set;
		}

		public override void Setup ()
		{
			base.Setup ();

			task = CreateTask<CustomCompileEntitlements> ();

			task.AppBundleDir = AppBundlePath;
			task.AppIdentifier = "32UV7A8CDE.com.xamarin.MySingleView";
			task.BundleIdentifier = "com.xamarin.MySingleView";
			task.CompiledEntitlements = new TaskItem (Path.Combine (MonoTouchProjectObjPath, "Entitlements.xcent"));
			task.Entitlements = Path.Combine ("..", "bin", "Resources", "Entitlements.plist");
			task.IsAppExtension = false;
			task.ProvisioningProfile = Path.Combine ("..", "bin", "Resources", "profile.mobileprovision");
			task.SdkPlatform = "iPhoneOS";
			task.SdkVersion = "6.1";

			compiledEntitlements = task.CompiledEntitlements.ItemSpec;
		}

		[Test (Description = "Xambug #46298")]
		public void ValidateEntitlement ()
		{
			ExecuteTask (task);
			var compiled = PDictionary.FromFile (compiledEntitlements);
			Assert.IsTrue (compiled.Get<PBoolean> (EntitlementKeys.GetTaskAllow).Value, "#1");
			Assert.AreEqual ("32UV7A8CDE.com.xamarin.MySingleView", compiled.Get<PString> ("application-identifier").Value, "#2");
			Assert.AreEqual ("Z8CSQKJE7R", compiled.Get<PString> ("com.apple.developer.team-identifier").Value, "#3");
			Assert.AreEqual ("applinks:*.xamarin.com", compiled.GetAssociatedDomains ().ToStringArray ().First (), "#4");
			Assert.AreEqual ("Z8CSQKJE7R.*", compiled.GetPassBookIdentifiers ().ToStringArray ().First (), "#5");
			Assert.AreEqual ("Z8CSQKJE7R.com.xamarin.MySingleView", compiled.GetUbiquityKeyValueStore (), "#6");
			Assert.AreEqual ("32UV7A8CDE.com.xamarin.MySingleView", compiled.GetKeychainAccessGroups ().ToStringArray ().First (), "#7");
		}

		public override void Teardown ()
		{
			base.Teardown ();

			CleanUp ();
		}
	}
}

