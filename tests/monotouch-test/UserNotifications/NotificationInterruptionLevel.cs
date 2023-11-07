using System;
using NUnit.Framework;

using Foundation;
using UserNotifications;

namespace MonoTouchFixtures.UserNotifications {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class UNNotificationInterruptionLevelTest {

		[Test]
		public void EnumTest ()
		{
#if !XAMCORE_5_0
			/* Apple Docs order these enum values as:
			typedef NS_ENUM (NSUInteger, UNNotificationInterruptionLevel)
			{
				UNNotificationInterruptionLevelPassive,
				UNNotificationInterruptionLevelActive,
				UNNotificationInterruptionLevelTimeSensitive,
				UNNotificationInterruptionLevelCritical,
			} */
			Assert.AreEqual ((int) UNNotificationInterruptionLevel.Passive2, 0);
			Assert.AreEqual ((int) UNNotificationInterruptionLevel.Active2, 1);
			Assert.AreEqual ((int) UNNotificationInterruptionLevel.TimeSensitive2, 2);
			Assert.AreEqual ((int) UNNotificationInterruptionLevel.Critical2, 3);
#endif
		}
	}
}
