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
			var point = new CGPoint ((nfloat) 1, (nfloat) 2);
#if NET
			Assert.AreEqual ("{1, 2}", point.ToString (), "ToString");
#else
			Assert.AreEqual ("{X=1, Y=2}", point.ToString (), "ToString");
#endif
		}
	}
}
