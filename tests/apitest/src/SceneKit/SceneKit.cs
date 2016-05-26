using System;
using System.Threading.Tasks;
using NUnit.Framework;

#if !XAMCORE_2_0
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreAnimation;
using MonoMac.SceneKit;
using CGRect = System.Drawing.RectangleF;
#else
using AppKit;
using Foundation;
using CoreAnimation;
using CoreGraphics;
using SceneKit;
#endif

namespace Xamarin.Mac.Tests
{
	[TestFixture]
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
			Assert.IsTrue (s != null && s != (NSString)(string.Empty));
		}

		[Test]
		public void SCNPhysicsTestKeys_SearchModeKeyTest ()
		{
			NSString s = SCNPhysicsTestKeys.SearchModeKey;
			Assert.IsTrue (s != null && s != (NSString)(string.Empty));
		}

		[Test]
		public void SCNSceneSourceLoading_AnimationImportPolicyKeyTest ()
		{
			NSString s = SCNSceneSourceLoading.AnimationImportPolicyKey;
			Assert.IsTrue (s != null && s != (NSString)(string.Empty));
		}
	}
}