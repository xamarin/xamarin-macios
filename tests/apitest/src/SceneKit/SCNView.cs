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
	public class SCNViewTests
	{
		[SetUp]
		public void SetUp ()
		{
			Asserts.EnsureYosemite ();
			if (Asserts.IsAtLeastElCapitan)
				Asserts.Ensure64Bit ();
		}

		[Test]
		public void SCNView_TechniqueSetterTest ()
		{
			SCNView v = new SCNView (new CGRect (), (NSDictionary) null);
			SCNTechnique t = SCNTechnique.Create (new NSDictionary ());
			v.Technique = t;
		}
	}
}