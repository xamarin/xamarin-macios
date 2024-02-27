using System;
using NUnit.Framework;
using Foundation;
using CloudKit;
using ObjCRuntime;
using Xamarin.Utils;

namespace MonoTouchFixtures.CloudKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CKFetchRecordsOperationTest {
		CKRecordID [] recordIDs = new CKRecordID [0];
		CKFetchRecordsOperation op = null;

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (6, 0);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 10, throwIfOtherPlatform: false);
			op = new CKFetchRecordsOperation (recordIDs);
		}

		[TearDown]
		public void TearDown ()
		{
			op?.Dispose ();
		}

		[Test]
		public void PerRecordProgressSetter ()
		{
			op.PerRecordProgress = (id, p) => { Console.WriteLine ("Notification"); };
			Assert.NotNull (op.PerRecordProgress);
		}

		[Test]
		public void PerRecordCompletionSetter ()
		{
			op.PerRecordCompletion = (record, id, e) => { Console.WriteLine ("Notification"); };
			Assert.NotNull (op.PerRecordCompletion);
		}

		[Test]
		public void TestCompletedSetter ()
		{
			op.Completed = (idDict, e) => { Console.WriteLine ("Completed"); };
			Assert.NotNull (op.Completed);
		}
	}
}
