#if !__WATCHOS__
using System;

using Foundation;

using Network;

using NUnit.Framework;

namespace MonoTouchFixtures.Network {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWBrowserDescriptorTest {

		NWBrowserDescriptor descriptor;
		string type = "_ftp._tcp";
		string domain = "MonoTouchFixtures.Network";

		[OneTimeSetUp]
		public void Init () => TestRuntime.AssertXcodeVersion (11, 0);

		[SetUp]
		public void SetUp ()
		{
			descriptor = NWBrowserDescriptor.CreateBonjourService (type, domain);
		}

		[TearDown]
		public void TearDown ()
		{
			descriptor.Dispose ();
		}

		[Test]
		public void TestIncludeTxtRecordProperty ()
		{
			Assert.IsFalse (descriptor.IncludeTxtRecord, "Get default value.");
			descriptor.IncludeTxtRecord = true;
			Assert.IsTrue (descriptor.IncludeTxtRecord, "Get new value.");
		}

		[Test]
		public void TestCreateNullDomain ()
		{
			using (var newDescriptor = NWBrowserDescriptor.CreateBonjourService (type)) {
				Assert.AreEqual (type, descriptor.BonjourType, "service type");
				Assert.IsNull (newDescriptor.BonjourDomain);
			}
		}

		[Test]
		public void TestBonjourTypeProperty () => Assert.AreEqual (type, descriptor.BonjourType);

		[Test]
		public void TestBonjourDomainProperty () => Assert.AreEqual (domain, descriptor.BonjourDomain);

		[Test]
		public void TestApplicationServiceConstructor ()
		{
			TestRuntime.AssertXcodeVersion (14, 0);
			Assert.Throws<ArgumentNullException> (() => {
				using var appServiceDescriptor = NWBrowserDescriptor.CreateApplicationServiceName (null);
			}, "App service is null");

			var appName = "myService";
			using var appServiceDescriptor = NWBrowserDescriptor.CreateApplicationServiceName (appName);
			Assert.AreEqual (appName, appServiceDescriptor.ApplicationServiceName);
		}
	}
}
#endif
