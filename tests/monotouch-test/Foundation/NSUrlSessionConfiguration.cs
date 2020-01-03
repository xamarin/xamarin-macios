using System;

using NUnit.Framework;

#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
#else
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
#endif

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	public class NSUrlSessionConfigurationTest {

		[Test]
		public void TestSessionTypeDefatul ()
		{
			using (var config = NSUrlSessionConfiguration.DefaultSessionConfiguration)
				Assert.AreEqual (NSUrlSessionConfiguration.SessionConfigurationType.Default, config.SessiontType);
		}

		[Test]
		public void TestSessionTypeBackground ()
		{
			using (var config = NSUrlSessionConfiguration.CreateBackgroundSessionConfiguration ("my.identifier.test"))
				Assert.AreEqual (NSUrlSessionConfiguration.SessionConfigurationType.Background, config.SessiontType);
		}

		[Test]
		public void TestSessionTypeEphemeral ()
		{
			using (var config = NSUrlSessionConfiguration.EphemeralSessionConfiguration)
				Assert.AreEqual (NSUrlSessionConfiguration.SessionConfigurationType.Ephemeral, config.SessiontType);
		}
	}
}