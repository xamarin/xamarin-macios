//
// Unit tests for CGSize
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
	public class SizeTest {

		[Test]
		public void ToStringTest ()
		{
#if NO_NFLOAT_OPERATORS
			var size = new CGSize (new NFloat (1), new NFloat (2));
#else
			var size = new CGSize ((nfloat)1, (nfloat)2);
#endif
#if NET
			Assert.AreEqual ("{1, 2}", size.ToString (), "ToString");
#else
			Assert.AreEqual ("{Width=1, Height=2}", size.ToString (), "ToString");
#endif
		}
	}
}
