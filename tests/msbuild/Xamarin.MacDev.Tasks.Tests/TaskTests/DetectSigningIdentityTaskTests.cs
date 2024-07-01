#nullable enable
using System;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NUnit.Framework;

using Xamarin.MacDev.Tasks;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	[TestFixture]
	public class DetectSigningIdentityTaskTests : TestBase {
		DetectSigningIdentity CreateTask (string? tmpdir = null, ApplePlatform platform = ApplePlatform.iOS, bool simulator = true, bool isDotNet = true)
		{
			if (string.IsNullOrEmpty (tmpdir))
				tmpdir = Cache.CreateTemporaryDirectory ();

			var task = CreateTask<DetectSigningIdentity> ();
			task.AppBundleName = "AssemblyName";
			task.SdkPlatform = PlatformFrameworkHelper.GetSdkPlatform (platform, simulator);
			task.TargetFrameworkMoniker = TargetFramework.GetTargetFramework (platform, isDotNet).ToString ();

			return task;
		}

		[Test]
		public void Default ()
		{
			var dir = Cache.CreateTemporaryDirectory ();
			var task = CreateTask (dir);

			ExecuteTask (task);

			Assert.IsNull (task.DetectedAppId, "DetectedAppId");
			Assert.AreEqual ("-", task.DetectedCodeSigningKey, "DetectedCodeSigningKey");
			Assert.AreEqual ($"{Xamarin.Tests.Configuration.XcodeLocation}/Toolchains/XcodeDefault.xctoolchain/usr/bin/codesign_allocate", task.DetectedCodesignAllocate, "DetectedCodesignAllocate");
			Assert.AreEqual ("Any", task.DetectedDistributionType, "DetectedDistributionType");
			Assert.IsNull (task.DetectedProvisioningProfile, "DetectedProvisioningProfile");
			Assert.IsFalse (task.HasEntitlements, "HasEntitlements");
		}

		const string EmptyEntitlements1 = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
<dict>
</dict>
</plist>";

		const string EmptyEntitlements2 = @"<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
<dict>
</dict>
</plist>";

		const string EmptyEntitlements3 = @"<plist version=""1.0"">
<dict>
</dict>
</plist>";

		const string EmptyEntitlements4 = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
<dict>
<!-- comment here! -->
</dict>
</plist>";

		const string NonEmptyEntitlements1 = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
<dict>
    <key>com.apple.security.app-sandbox</key>
    <true/>
</dict>
</plist>";

		public class EntitlementTestCase {
			public string Name = string.Empty;
			public string Entitlements = string.Empty;
			public bool Required;
			public bool IsSimulator;
			public bool? CodesignRequireProvisioningProfile;
			public bool? RequireCodeSigning;

			public override string ToString ()
			{
				return Name;
			}
		}

		static EntitlementTestCase [] GetEntitlementsTestCases ()
		{
			return new EntitlementTestCase []
			{
				// simulator
				new EntitlementTestCase { Name = nameof (EmptyEntitlements1), Entitlements = EmptyEntitlements1, IsSimulator = true },
				new EntitlementTestCase { Name = nameof (EmptyEntitlements2), Entitlements = EmptyEntitlements2, IsSimulator = true },
				new EntitlementTestCase { Name = nameof (EmptyEntitlements3), Entitlements = EmptyEntitlements3, IsSimulator = true },
				new EntitlementTestCase { Name = nameof (EmptyEntitlements4), Entitlements = EmptyEntitlements4, IsSimulator = true },
				new EntitlementTestCase { Name = nameof (NonEmptyEntitlements1), Entitlements = NonEmptyEntitlements1, IsSimulator = true },
				new EntitlementTestCase { Name = nameof (EmptyEntitlements1) + "_Required", Entitlements = EmptyEntitlements1, IsSimulator = true, CodesignRequireProvisioningProfile = true },
				new EntitlementTestCase { Name = nameof (NonEmptyEntitlements1) + "_NotRequired", Entitlements = NonEmptyEntitlements1, IsSimulator = true, CodesignRequireProvisioningProfile = false },
				// device
				new EntitlementTestCase { Name = nameof (EmptyEntitlements1) + "_Device", Entitlements = EmptyEntitlements1, IsSimulator = false, RequireCodeSigning = true },
				new EntitlementTestCase { Name = nameof (EmptyEntitlements2) + "_Device", Entitlements = EmptyEntitlements2, IsSimulator = false, RequireCodeSigning = true },
				new EntitlementTestCase { Name = nameof (EmptyEntitlements3) + "_Device", Entitlements = EmptyEntitlements3, IsSimulator = false, RequireCodeSigning = true },
				new EntitlementTestCase { Name = nameof (EmptyEntitlements4) + "_Device", Entitlements = EmptyEntitlements4, IsSimulator = false, RequireCodeSigning = true },
				new EntitlementTestCase { Name = nameof (NonEmptyEntitlements1) + "_Device", Entitlements = NonEmptyEntitlements1, IsSimulator = false, RequireCodeSigning = true },
				new EntitlementTestCase { Name = nameof (EmptyEntitlements1) + "_Required_Device", Entitlements = EmptyEntitlements1, IsSimulator = false, CodesignRequireProvisioningProfile = true, RequireCodeSigning = true },
				new EntitlementTestCase { Name = nameof (NonEmptyEntitlements1) + "_NotRequired_Device", Entitlements = NonEmptyEntitlements1, IsSimulator = false, CodesignRequireProvisioningProfile = false, RequireCodeSigning = true },
			};
		}

		[Test]
		[TestCaseSource (nameof (GetEntitlementsTestCases))]
		public void EmptyEntitlements (EntitlementTestCase testCase)
		{
			var dir = Cache.CreateTemporaryDirectory ();
			var entitlementsPath = Path.Combine (dir, "Entitlements.plist");
			File.WriteAllText (entitlementsPath, testCase.Entitlements);

			var task = CreateTask (dir, simulator: testCase.IsSimulator);
			task.BundleIdentifier = "com.tests.emptyentitlements";
			task.CodesignEntitlements = new TaskItem (entitlementsPath);
			task.SdkIsSimulator = testCase.IsSimulator;
			if (testCase.CodesignRequireProvisioningProfile.HasValue)
				task.CodesignRequireProvisioningProfile = testCase.CodesignRequireProvisioningProfile.Value.ToString ();
			if (testCase.RequireCodeSigning.HasValue)
				task.RequireCodeSigning = testCase.RequireCodeSigning.Value;

			ExecuteTask (task);

			if (testCase.IsSimulator) {
				Assert.AreEqual ("com.tests.emptyentitlements", task.DetectedAppId, "DetectedAppId");
				Assert.AreEqual ("-", task.DetectedCodeSigningKey, "DetectedCodeSigningKey");
				Assert.That (task.DetectedDistributionType, Is.EqualTo ("Any"), "DetectedDistributionType");
				Assert.That (task.DetectedProvisioningProfile, Is.Null, "DetectedProvisioningProfile");
			} else {
				if (testCase.CodesignRequireProvisioningProfile != false) {
					Assert.That (task.DetectedAppId, Does.EndWith (".com.tests.emptyentitlements"), "DetectedAppId");
				} else {
					Assert.AreEqual ("com.tests.emptyentitlements", task.DetectedAppId, "DetectedAppId");
				}
				Assert.That (task.DetectedCodeSigningKey, Has.Length.EqualTo ("20D63576DE3EA7BE419C18997CF948D759B43D53".Length), "DetectedCodeSigningKey");
				if (testCase.CodesignRequireProvisioningProfile != false) {
					Assert.That (task.DetectedDistributionType, Is.EqualTo ("Development").Or.EqualTo ("AppStore").Or.EqualTo ("Any"), "DetectedDistributionType");
					Assert.That (task.DetectedProvisioningProfile, Is.Not.Null.And.Not.Empty, "DetectedProvisioningProfile");
				} else {
					Assert.That (task.DetectedDistributionType, Is.EqualTo ("Any"), "DetectedDistributionType");
					Assert.That (task.DetectedProvisioningProfile, Is.Null, "DetectedProvisioningProfile");
				}
			}
			Assert.AreEqual ($"{Xamarin.Tests.Configuration.XcodeLocation}/Toolchains/XcodeDefault.xctoolchain/usr/bin/codesign_allocate", task.DetectedCodesignAllocate, "DetectedCodesignAllocate");
		}

		[Test]
		public void CustomEntitlements ()
		{
			var dir = Cache.CreateTemporaryDirectory ();
			var task = CreateTask (dir);
			task.CustomEntitlements = new ITaskItem [] { new TaskItem ("keychain-access-group") };
			ExecuteTask (task);

			Assert.IsNull (task.DetectedAppId, "DetectedAppId");
			Assert.AreEqual ("-", task.DetectedCodeSigningKey, "DetectedCodeSigningKey");
			Assert.AreEqual ($"{Xamarin.Tests.Configuration.XcodeLocation}/Toolchains/XcodeDefault.xctoolchain/usr/bin/codesign_allocate", task.DetectedCodesignAllocate, "DetectedCodesignAllocate");
			Assert.AreEqual ("Any", task.DetectedDistributionType, "DetectedDistributionType");
			Assert.IsNull (task.DetectedProvisioningProfile, "DetectedProvisioningProfile");
			Assert.IsTrue (task.HasEntitlements, "HasEntitlements");
		}
	}
}
