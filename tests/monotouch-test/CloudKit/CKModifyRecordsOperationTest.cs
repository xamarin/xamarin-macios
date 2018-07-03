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
			TestRuntime.AssertXcodeVersion (6, 0);
			TestRuntime.AssertMacSystemVersion (10, 10, throwIfOtherPlatform: false);
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

		[Test]
		public void Default ()
		{
			// watchOS does not allow `init` so we need to ensure that our default .ctor
			// match the existing `init*` with null values (so we can remove it)
			using (var mro = new CKModifyRecordsOperation ()) {
				Assert.That (op.Atomic, Is.EqualTo (mro.Atomic), "Atomic");
				Assert.That (op.RecordsToSave, Is.EqualTo (mro.RecordsToSave), "RecordsToSave");
				Assert.That (op.RecordIdsToDelete, Is.EqualTo (mro.RecordIdsToDelete), "RecordIdsToDelete");
				Assert.That (op.SavePolicy, Is.EqualTo (mro.SavePolicy), "SavePolicy");
				Assert.That (op.ClientChangeTokenData, Is.EqualTo (mro.ClientChangeTokenData), "ClientChangeTokenData");
				Assert.That (op.PerRecordProgress, Is.EqualTo (mro.PerRecordProgress), "PerRecordProgress");
				Assert.That (op.PerRecordCompletion, Is.EqualTo (mro.PerRecordCompletion), "PerRecordCompletion");
				Assert.That (op.Completed, Is.EqualTo (mro.Completed), "Completed");
			}
		}
	}
}
