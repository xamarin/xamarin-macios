using System;
using NUnit.Framework;
#if XAMCORE_2_0
using Foundation;
using CloudKit;
using ObjCRuntime;
#else
using MonoTouch.Foundation;
using MonoTouch.CloudKit;
#endif

namespace MonoTouchFixtures.CloudKit
{

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CKMarkNotificationsReadOperationTest
	{
		CKNotificationID [] notificationIDs = new CKNotificationID [0];
		CKMarkNotificationsReadOperation op = null;

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (6, 0);
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 10, throwIfOtherPlatform: false);
			op = new CKMarkNotificationsReadOperation (notificationIDs);
		}

		[TearDown]
		public void TearDown ()
		{
			op?.Dispose ();
		}

		[Test]
		public void TestCompletedSetter ()
		{
			op.Completed = (idDict, e) => { Console.WriteLine ("Completed");};
			Assert.NotNull (op.Completed);
		}
	}
}
