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
	public class CKModifyRecordsOperationTest
	{
		CKModifyRecordsOperation op = null;

		[SetUp]
		public void SetUp ()
		{
			op = new CKModifyRecordsOperation (null, null);
		}

		[TearDown]
		public void TearDown ()
		{
			op?.Dispose ();
		}

		[Test]
		public void PerRecordProgressSetter ()
		{
			op.PerRecordProgress = (record, p) => { Console.WriteLine ("Progress");};
			Assert.NotNull (op.PerRecordProgress);
		}
		
		[Test]
		public void PerRecordCompletionSetter ()
		{
			op.PerRecordCompletion = (record, e) => { Console.WriteLine ("Notification");};
			Assert.NotNull (op.PerRecordCompletion);
		}

		[Test]
		public void TestCompletedSetter ()
		{
			op.Completed = (saved, deleted, e) => { Console.WriteLine ("Completed");};
			Assert.NotNull (op.Completed);
		}
	}
}
