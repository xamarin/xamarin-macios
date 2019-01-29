//
// Unit tests for MPSAccelerationStructure
//
// Authors:
//	Alex Soto <alexsoto@microsoft.com>
//
//
// Copyright 2019 Microsoft Corporation.
//

#if !__WATCHOS__

using System;
using System.Runtime.InteropServices;

using Foundation;
using Metal;
using MetalPerformanceShaders;

using NUnit.Framework;
using OpenTK;

namespace MonoTouchFixtures.MetalPerformanceShaders {
	[TestFixture]
	public class MPSAccelerationStructureTests {

		IMTLDevice device;
		nuint vector3Size = (nuint) Marshal.SizeOf<OpenTK.NVector3> ();

		[TestFixtureSetUp]
		public void Metal ()
		{
			TestRuntime.AssertDevice ();
			TestRuntime.AssertXcodeVersion (10, 0);

			device = MTLDevice.SystemDefault;
			// some older hardware won't have a default
			if (device == null || !MPSKernel.Supports (device))
				Assert.Inconclusive ("Metal is not supported");
		}

		[Test]
		public void MPSAxisAlignedBoundingBoxTest ()
		{
			var vertices = new NVector3 [3] {
				new NVector3 (0.25f, 0.25f, 0.0f),
				new NVector3 (0.75f, 0.25f, 0.0f),
				new NVector3 (0.50f, 0.75f, 0.0f)
			};

			var vbuffer = device.CreateBuffer (vertices, MTLResourceOptions.CpuCacheModeWriteCombined);
			var indices = new uint [] { 0, 1, 2 };
			var ibuffer = device.CreateBuffer (indices, MTLResourceOptions.CpuCacheModeWriteCombined);

			var accelerationStructure = new MPSTriangleAccelerationStructure (device) {
				VertexBuffer = vbuffer,
				VertexStride = vector3Size,
				IndexBuffer = ibuffer,
				IndexType = MPSDataType.UInt32,
				TriangleCount = 1
			};
			accelerationStructure.Rebuild ();

			var bbox = accelerationStructure.BoundingBox;
			Assert.That (bbox.Max.X, Is.EqualTo (0.75f), "bbox.Max.X");
			Assert.That (bbox.Max.Y, Is.EqualTo (0.75f), "bbox.Max.Y");
			Assert.That (bbox.Max.Z, Is.EqualTo (0.0f), "bbox.Max.Z");
			Assert.That (bbox.Min.X, Is.EqualTo (0.25f), "bbox.Min.X");
			Assert.That (bbox.Min.Y, Is.EqualTo (0.25f), "bbox.Min.Y");
			Assert.That (bbox.Min.Z, Is.EqualTo (0.0f), "bbox.Min.Z");
		}
	}
}
#endif // !__WATCHOS__
