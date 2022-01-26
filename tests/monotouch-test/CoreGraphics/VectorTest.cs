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
#if NO_NFLOAT_OPERATORS
			var vector = new CGVector (new NFloat (1), new NFloat (2));
#else
			var vector = new CGVector ((nfloat)1, (nfloat)2);
#endif
			Assert.AreEqual ("{1, 2}", vector.ToString (), "ToString");
		}
	}
}
