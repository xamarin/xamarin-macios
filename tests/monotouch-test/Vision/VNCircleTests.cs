//
// Unit tests for VNRequestTests
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
	public class VNCircleTests {

		[SetUp]
		public void Setup () => TestRuntime.AssertXcodeVersion (12, TestRuntime.MinorXcode12APIMismatch);

		[Test]
		public void CreateUsingRadiusTest ()
		{
			var circle = VNCircle.CreateUsingRadius (new VNPoint (10, 10), radius: 10);
			Assert.NotNull (circle, "Circle not null");
			Assert.AreEqual (circle.Radius, 10, "Radius");
			Assert.AreEqual (circle.Center.X, 10, "X");
			Assert.AreEqual (circle.Center.Y, 10, "Y");
			Assert.That (circle.RetainCount, Is.EqualTo ((nuint) 1), "RetainCount");
		}

		[Test]
		public void CreateUsingDiameterTest ()
		{
			var circle = VNCircle.CreateUsingDiameter (new VNPoint (5, 6), diameter: 7);
			Assert.NotNull (circle, "Circle not null");
			Assert.AreEqual (circle.Diameter, 7, "Diameter");
			Assert.AreEqual (circle.Center.Y, 6, "Y");
			Assert.AreEqual (circle.Center.X, 5, "X");
			Assert.That (circle.RetainCount, Is.EqualTo ((nuint) 1), "RetainCount");
		}
	}
}
#endif
