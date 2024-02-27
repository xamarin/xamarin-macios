#if !__WATCHOS__
using System;
using NUnit.Framework;
using Foundation;
using CloudKit;
using ObjCRuntime;
using Xamarin.Utils;

namespace MonoTouchFixtures.CloudKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CKFetchRecordChangesOperationTest {
		CKFetchRecordChangesOperation op = null;
		CKRecordZoneID zoneID;

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (6, 0);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 10, throwIfOtherPlatform: false);
			zoneID = new CKRecordZoneID ("foo", "xamarin");
			op = new CKFetchRecordChangesOperation (zoneID, null);
		}

		[TearDown]
		public void TearDown ()
		{
			op?.Dispose ();
		}

		[Test]
		public void TestRecordChangedSetter ()
		{
			op.RecordChanged = (record) => { Console.WriteLine ("Changed"); };
			Assert.NotNull (op.RecordChanged);
		}

		[Test]
		public void TestRecordDeletedSetter ()
		{
			op.RecordDeleted = (record) => { Console.WriteLine ("Deleted"); };
			Assert.NotNull (op.RecordDeleted);
		}

		[Test]
		public void TestAllChangesReported ()
		{
			op.AllChangesReported = (s, c, e) => { Console.WriteLine ("Completed"); };
			Assert.NotNull (op.AllChangesReported);
		}
	}
}
#endif
