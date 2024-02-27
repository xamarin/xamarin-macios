using System;
using NUnit.Framework;
using Foundation;
using CloudKit;
using ObjCRuntime;
using Xamarin.Utils;

namespace MonoTouchFixtures.CloudKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CKModifyRecordZonesOperationTest {
		CKModifyRecordZonesOperation op = null;

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (6, 0);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 10, throwIfOtherPlatform: false);
			op = new CKModifyRecordZonesOperation (null, null);
		}

		[TearDown]
		public void TearDown ()
		{
			op?.Dispose ();
		}

		[Test]
		public void TestCompletedSetter ()
		{
			op.Completed = (saved, deleted, e) => { Console.WriteLine ("Completed"); };
			Assert.NotNull (op.Completed);
		}

		[Test]
		public void Default ()
		{
			// watchOS does not allow `init` so we need to ensure that our default .ctor
			// match the existing `init*` with null values (so we can remove it)
			using (var mrzo = new CKModifyRecordZonesOperation ()) {
				Assert.That (op.RecordZonesToSave, Is.EqualTo (mrzo.RecordZonesToSave), "RecordZonesToSave");
				Assert.That (op.RecordZoneIdsToDelete, Is.EqualTo (mrzo.RecordZoneIdsToDelete), "RecordZoneIdsToDelete");
				Assert.That (op.Completed, Is.EqualTo (mrzo.Completed), "Completed");
			}
		}
	}
}
