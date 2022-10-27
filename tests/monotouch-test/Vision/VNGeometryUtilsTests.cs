//
// Unit tests for VNGeometryUtils
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

#if NET
using System.Numerics;
#else
using OpenTK;
#endif

namespace MonoTouchFixtures.Vision {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class VNGeometryUtilsTests {

		[SetUp]
		public void Setup () => TestRuntime.AssertXcodeVersion (12, TestRuntime.MinorXcode12APIMismatch);

		[Test]
		public void CreateBoundingCircleTest ()
		{
			var nvectors = new [] {
				new VNPoint (1,1),
				new VNPoint (-1,1),
				new VNPoint (-1,-1),
				new VNPoint (1,-1),
			};

			var ncircle = VNGeometryUtils.CreateBoundingCircle (nvectors, out var nerror);
			Assert.Null (nerror, "nerror was not null");
			Assert.NotNull (ncircle, "ncircle was null");

			var vectors = new [] {
				new Vector2 (1,1),
				new Vector2 (-1,1),
				new Vector2 (-1,-1),
				new Vector2 (1,-1),
			};

			var circle = VNGeometryUtils.CreateBoundingCircle (vectors, out var error);
			Assert.Null (error, "Error was not null");
			Assert.NotNull (circle, "circle was null");

			Assert.AreEqual (ncircle.Diameter, circle.Diameter, "Diameter");
			Assert.AreEqual (ncircle.Radius, circle.Radius, "Radius");
		}
	}
}
#endif
