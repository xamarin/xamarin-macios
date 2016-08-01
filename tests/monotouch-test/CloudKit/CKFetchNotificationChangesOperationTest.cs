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
			op = new CKFetchNotificationChangesOperation (token);
		}

		[TearDown]
		public void TearDown ()
		{
			op?.Dispose ();
		}

		public void TestNotificationChangedSetter ()
		{
			op.NotificationChanged = (obj) => { Console.WriteLine ("Notification");};
			Assert.NotNull (op.NotificationChanged);
		}

		public void TestCompletedSetter ()
		{
			op.Completed = (arg1, arg2) => { Console.WriteLine ("Completed");};
			Assert.NotNull (op.Completed);
		}
	}
}
