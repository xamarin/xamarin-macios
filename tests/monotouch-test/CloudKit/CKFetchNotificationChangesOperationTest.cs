using System;
using NUnit.Framework;
#if XAMCORE_2_0
using Foundation;
using CloudKit;
#else
using MonoTouch.Foundation;
using MonoTouch.CloudKit;
#endif

namespace MonoTouchFixtures.CloudKit
{

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CKFetchNotificationChangesOperationTest
	{
		CKServerChangeToken token = null;
		CKFetchNotificationChangesOperation op = null;

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (6, 0);
			TestRuntime.AssertMacSystemVersion (10, 10, throwIfOtherPlatform: false);
			op = new CKFetchNotificationChangesOperation (token);
		}

		[TearDown]
		public void TearDown ()
		{
			op?.Dispose ();
		}

		[Test]
		public void TestNotificationChangedSetter ()
		{
			op.NotificationChanged = (obj) => { Console.WriteLine ("Notification");};
			Assert.NotNull (op.NotificationChanged);
		}

		[Test]
		public void TestCompletedSetter ()
		{
			op.Completed = (arg1, arg2) => { Console.WriteLine ("Completed");};
			Assert.NotNull (op.Completed);
		}

		[Test]
		public void Default ()
		{
			// watchOS does not allow `init` so we need to ensure that our default .ctor
			// match the existing `init*` with null values (so we can remove it)
			using (var mrzo = new CKFetchNotificationChangesOperation ()) {
				Assert.That (op.PreviousServerChangeToken, Is.EqualTo (mrzo.PreviousServerChangeToken), "PreviousServerChangeToken");
				Assert.That (op.Completed, Is.EqualTo (mrzo.Completed), "Completed");
				Assert.That (op.NotificationChanged, Is.EqualTo (mrzo.NotificationChanged), "NotificationChanged");
			}
		}
	}
}
