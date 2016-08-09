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
	public class CKModifyBadgeOperationTest
	{
		CKModifyBadgeOperation op = null;

		[SetUp]
		public void SetUp ()
		{
			op = new CKModifyBadgeOperation (3);
		}

		[TearDown]
		public void TearDown ()
		{
			op?.Dispose ();
		}

		[Test]
		public void TestCompletedSetter ()
		{
			op.Completed = (e) => { Console.WriteLine ("Completed");};
			Assert.NotNull (op.Completed);
		}
	}
}
