#if __MACOS__
using System;
using System.Threading.Tasks;
using NUnit.Framework;

using AppKit;
using Foundation;
using CoreAnimation;
using SceneKit;

namespace Xamarin.Mac.Tests {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class SCNWorldTests {
		[SetUp]
		public void SetUp ()
		{
			Asserts.EnsureMavericks ();
			if (Asserts.IsAtLeastElCapitan)
				Asserts.Ensure64Bit ();
		}

		[Test]
		public void SCNNode_BackfaceCulling ()
		{
			Asserts.EnsureYosemite ();

			if (IntPtr.Size == 8) {
				Assert.IsNotNull (SCNPhysicsTestKeys.BackfaceCullingKey);
			}
		}
	}
}
#endif // __MACOS__
