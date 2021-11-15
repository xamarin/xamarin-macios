#if HAS_ARKIT

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ARKit;
using Foundation;
using NUnit.Framework;
using ObjCRuntime;
using Xamarin.Utils;

namespace monotouchtest.ARKit {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ARSkeletonTest {

		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (12, 0);
			// The API here was introduced to Mac Catalyst later than for the other frameworks, so we have this additional check
			TestRuntime.AssertSystemVersion (ApplePlatform.MacCatalyst, 14, 0, throwIfOtherPlatform: false);
		}

		[Test]
		public void UnknownPointTest ()
		{
			using (var notKnownPoint = new NSString ("nariz"))
				Assert.IsNull (ARSkeleton.CreateJointName (notKnownPoint));
		}

	}
}
#endif // HAS_ARKIT
