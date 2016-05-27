using System;
using System.Threading.Tasks;
using NUnit.Framework;

#if !XAMCORE_2_0
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreAnimation;
using MonoMac.SceneKit;
#else
using AppKit;
using Foundation;
using CoreAnimation;
using SceneKit;
#endif

namespace Xamarin.Mac.Tests
{
	[TestFixture]
	public class SCNWorldTests
	{
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

			if (IntPtr.Size == 8)
			{
				Assert.IsNotNull (SCNPhysicsTestKeys.BackfaceCullingKey);
			}
		}
	}
}