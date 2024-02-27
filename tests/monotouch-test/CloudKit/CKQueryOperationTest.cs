using System;
using NUnit.Framework;
using Foundation;
using CloudKit;
using ObjCRuntime;
using Xamarin.Utils;

namespace MonoTouchFixtures.CloudKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CKQueryOperationTest {
		CKQueryOperation op = null;
		CKQuery q = null;

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (6, 0);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 10, throwIfOtherPlatform: false);
			q = new CKQuery ("Foo", NSPredicate.FromFormat ("email = '@xamarin'"));
			op = new CKQueryOperation (q);
		}

		[TearDown]
		public void TearDown ()
		{
			op?.Dispose ();
		}

		[Test]
		public void TestRecordFetchedSetter ()
		{
			op.RecordFetched = (record) => { Console.WriteLine ("Completed"); };
			Assert.NotNull (op.RecordFetched);
		}

		[Test]
		public void TestCompletedSetter ()
		{
			op.Completed = (cursor, e) => { Console.WriteLine ("Completed"); };
			Assert.NotNull (op.Completed);
		}
	}
}
