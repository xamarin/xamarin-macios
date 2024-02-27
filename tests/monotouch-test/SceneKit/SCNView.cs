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
	public class SCNViewTests {
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
#endif // __MACOS__
