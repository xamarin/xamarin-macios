#if __IOS__

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ARKit;
using Foundation;
using NUnit.Framework;
using ObjCRuntime;

namespace monotouchtest.ARKit {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ARSkeletonTest {
		[Test]
		public void UnknownPointTest ()
		{
			using (var notKnownPoint = new NSString ("nariz"))
				Assert.IsNull (ARSkeleton.CreateJointName (notKnownPoint));
		}

	}
}
#endif
