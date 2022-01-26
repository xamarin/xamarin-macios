//
// Unit tests for CGPoint
//
// Authors:
//	Sebastien Pouliot < sebastien.pouliot@gmail.com>
//

using System;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.CoreGraphics {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PointTest {

		[Test]
		public void ToStringTest ()
		{
#if NO_NFLOAT_OPERATORS
			var point = new CGPoint (new NFloat (1), new NFloat (2));
#else
			var point = new CGPoint ((nfloat)1, (nfloat)2);
#endif
#if NET
			Assert.AreEqual ("{1, 2}", point.ToString (), "ToString");
#else
			Assert.AreEqual ("{X=1, Y=2}", point.ToString (), "ToString");
#endif
		}
	}
}
