#if __MACOS__
using System;
using System.Threading.Tasks;
using NUnit.Framework;

using AppKit;
using Foundation;
using CoreAnimation;
using CoreGraphics;
using SceneKit;

namespace Xamarin.Mac.Tests {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class SceneKitTests // Generic one off tests
	{
		[SetUp]
		public void SetUp ()
		{
			Asserts.EnsureYosemite ();
			if (Asserts.IsAtLeastElCapitan)
				Asserts.Ensure64Bit ();
		}

		[Test]
		public void SCNGeometrySourceSemantic_ColorKeyTest ()
		{
			NSString s = SCNGeometrySourceSemantic.Color;
			Assert.IsTrue (s is not null && s != (NSString) (string.Empty));
		}

		[Test]
		public void SCNPhysicsTestKeys_SearchModeKeyTest ()
		{
			NSString s = SCNPhysicsTestKeys.SearchModeKey;
			Assert.IsTrue (s is not null && s != (NSString) (string.Empty));
		}

		[Test]
		public void SCNSceneSourceLoading_AnimationImportPolicyKeyTest ()
		{
			NSString s = SCNSceneSourceLoading.AnimationImportPolicyKey;
			Assert.IsTrue (s is not null && s != (NSString) (string.Empty));
		}
	}
}
#endif // __MACOS__
