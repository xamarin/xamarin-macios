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
	public class CKModifyRecordZonesOperationTest
	{
		CKModifyRecordZonesOperation op = null;

		[SetUp]
		public void SetUp ()
		{
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
			op.Completed = (saved, deleted, e) => { Console.WriteLine ("Completed");};
			Assert.NotNull (op.Completed);
		}
	}
}
