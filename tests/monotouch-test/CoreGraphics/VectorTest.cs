//
// Unit tests for CGVector
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
	public class VectorTest {

		[Test]
		public void ToStringTest ()
		{
			var vector = new CGVector ((nfloat) 1, (nfloat) 2);
			Assert.AreEqual ("{1, 2}", vector.ToString (), "ToString");
		}
	}
}
