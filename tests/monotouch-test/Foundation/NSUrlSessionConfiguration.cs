using NUnit.Framework;

using Foundation;
using ObjCRuntime;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSUrlSessionConfigurationTest {

		[Test]
		public void TestSessionTypeDefault ()
		{
			using (var config = NSUrlSessionConfiguration.DefaultSessionConfiguration)
				Assert.AreEqual (NSUrlSessionConfiguration.SessionConfigurationType.Default, config.SessionType);
		}

		[Test]
		public void TestSessionTypeBackground ()
		{
			using (var config = NSUrlSessionConfiguration.CreateBackgroundSessionConfiguration ("my.identifier.test"))
				Assert.AreEqual (NSUrlSessionConfiguration.SessionConfigurationType.Background, config.SessionType);
		}

		[Test]
		public void TestSessionTypeEphemeral ()
		{
			using (var config = NSUrlSessionConfiguration.EphemeralSessionConfiguration)
				Assert.AreEqual (NSUrlSessionConfiguration.SessionConfigurationType.Ephemeral, config.SessionType);
		}
	}
}
