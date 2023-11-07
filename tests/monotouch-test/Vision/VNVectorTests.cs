//
// Unit tests for VNVector
//
// Authors:
//	Alex Soto <alexsoto@microsoft.com>
//
// Copyright (c) Microsoft Corporation.
//

#if !__WATCHOS__

using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

using CoreGraphics;
using Foundation;
using Vision;

namespace MonoTouchFixtures.Vision {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class VNVectorTests {

		[SetUp]
		public void Setup () => TestRuntime.AssertXcodeVersion (12, TestRuntime.MinorXcode12APIMismatch);

		[Test]
		public void VNVectorCreateTest ()
		{
			var vector = VNVector.Create (r: 10, theta: 0.5);
			Assert.NotNull (vector, "vector not null");
			Assert.AreEqual (vector.R, 10, "R");
			Assert.AreEqual (vector.Theta, 0.5, "Theta");
			Assert.That (vector.RetainCount, Is.EqualTo ((nuint) 1), "RetainCount");
		}
	}
}
#endif
