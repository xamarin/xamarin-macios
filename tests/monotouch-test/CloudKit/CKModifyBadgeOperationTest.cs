using System;
using NUnit.Framework;
using Foundation;
using CloudKit;
using ObjCRuntime;
using Xamarin.Utils;

namespace MonoTouchFixtures.CloudKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CKModifyBadgeOperationTest {
		CKModifyBadgeOperation op = null;

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (6, 0);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 10, throwIfOtherPlatform: false);
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
			op.Completed = (e) => { Console.WriteLine ("Completed"); };
			Assert.NotNull (op.Completed);
		}

		[Test]
		public void Default ()
		{
			// watchOS does not allow `init` so we need to ensure that our default .ctor
			// match the existing `init*` with null values (so we can remove it)
			using (var def = new CKModifyBadgeOperation ())
			using (var zr0 = new CKModifyBadgeOperation (0)) {
				Assert.That (def.BadgeValue, Is.EqualTo (zr0.BadgeValue), "BadgeValue");
				Assert.That (def.Completed, Is.EqualTo (zr0.Completed), "Completed");
			}
		}
	}
}
