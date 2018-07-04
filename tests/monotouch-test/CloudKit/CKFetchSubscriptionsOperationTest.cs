#if !__WATCHOS__
using System;
using NUnit.Framework;
#if XAMCORE_2_0
using Foundation;
using CloudKit;
using ObjCRuntime;
#else
using MonoTouch.Foundation;
using MonoTouch.CloudKit;
#endif

namespace MonoTouchFixtures.CloudKit
{

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CKFetchSubscriptionsOperationTest
	{
		CKFetchSubscriptionsOperation op = null;
		string [] ids = new string [0];

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (6, 0);
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 10, throwIfOtherPlatform: false);
			op = new CKFetchSubscriptionsOperation (ids);
		}

		[TearDown]
		public void TearDown ()
		{
			op?.Dispose ();
		}

		[Test]
		public void TestCompletedSetter ()
		{
			op.Completed = (dict, e) => { Console.WriteLine ("Completed");};
			Assert.NotNull (op.Completed);
		}
	}
}
#endif
