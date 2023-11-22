// Unit test for Vision.GetCameraRelativePosition

#if !__WATCHOS__

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

using CoreGraphics;
using ImageIO;
using Foundation;
using Vision;
using SceneKit;

#if NET
using System.Numerics;
using Vector2 = global::System.Numerics.Vector2;
using Vector3 = global::System.Numerics.Vector3;
using Matrix3 = global::CoreGraphics.NMatrix3;
using Matrix4 = global::CoreGraphics.NMatrix4;
#else
using OpenTK;
using Vector2 = global::OpenTK.Vector2;
using Vector3 = global::OpenTK.Vector3;
using Matrix3 = global::OpenTK.NMatrix3;
using Matrix4 = global::OpenTK.NMatrix4;
#endif

namespace MonoTouchFixtures.Vision
{

	[TestFixture]
	[Preserve(AllMembers = true)]
	public class VNGetCameraRelativePositionTest
	{
		[SetUp]
		public void SetUp()
		{
			TestRuntime.AssertXcodeVersion(15, 0);
		}

		[Test]
		public void GetCameraRelativePositionTest()
		{
			var requestHandler = new VNImageRequestHandler(NSBundle.MainBundle.GetUrlForResource ("full_body", "png"), new NSDictionary());
			var request = new VNDetectHumanBodyPose3DRequest ();
			
			var didPerform = requestHandler.Perform (new VNRequest[] { request }, out NSError error);
			Assert.Null (error, $"VNImageRequestHandler.Perform should not return an error {error}");
			
			var observation = request.Results?.Length > 0 ? request.Results[0] : null;
			Assert.NotNull (observation, "VNImageRequestHandler.Perform should return a result.");

			Matrix4 expectedMatrix = new Matrix4 (
				(float) -0.98357517, (float) 0.014054606, (float) -0.17995091, (float) 0.012865879,
				(float) -0.1346774, (float) -0.7209123, (float) 0.67981434, (float) 0.9698789,
				(float) -0.12017429, (float) 0.69288385, (float) 0.71096426, (float) 1.2595181,
				(float) 0, (float) 0, (float) 0, (float) 1);

			var position = observation.GetCameraRelativePosition (out var modelPositionOut, VNHumanBodyPose3DObservationJointName.CenterHead, out NSError observationError);
			Assert.Null (observationError, $"GetCameraRelativePosition should not return an error {observationError}");
			Assert.That (modelPositionOut, Is.EqualTo (expectedMatrix), "VNVector3DGetCameraRelativePosition result is not equal to expected matrix");
		}
	}
}
#endif
