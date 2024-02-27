#if __MACOS__
using System;
using System.Threading.Tasks;
using NUnit.Framework;

using Foundation;
using SceneKit;

namespace Xamarin.Mac.Tests {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class SCNGeometrySourceTests {
		[SetUp]
		public void SetUp ()
		{
			if (Asserts.IsAtLeastElCapitan)
				Asserts.Ensure64Bit ();
		}

		[Test]
		public void SCNGeometrySourceSemanticTest ()
		{
			Asserts.EnsureMountainLion ();
			Assert.IsNotNull (SCNGeometrySourceSemantic.Color, "Color");
		}

		private bool isValidEnumForPlatform (SCNGeometrySourceSemantics value)
		{
			if (Asserts.IsAtLeastYosemite)
				return true;

			switch (value) {
			case SCNGeometrySourceSemantics.Color:
			case SCNGeometrySourceSemantics.Normal:
			case SCNGeometrySourceSemantics.Texcoord:
			case SCNGeometrySourceSemantics.Vertex:
				return true;

			case SCNGeometrySourceSemantics.BoneIndices:
			case SCNGeometrySourceSemantics.BoneWeights:
			case SCNGeometrySourceSemantics.EdgeCrease:
			case SCNGeometrySourceSemantics.VertexCrease:
			default: // this might need updating with 10.11
				return Asserts.IsAtLeastYosemite;
			}
		}

		[Test]
		public void SCNGeometrySource_FromDataTest ()
		{
			Asserts.EnsureMountainLion ();
#pragma warning disable 0219
			SCNGeometrySource d = SCNGeometrySource.FromData (new NSData (), SCNGeometrySourceSemantic.Color, 1, false, 1, 1, 1, 1);
#if NET
			foreach (var s in Enum.GetValues<SCNGeometrySourceSemantics> ()) {
#else
			foreach (SCNGeometrySourceSemantics s in Enum.GetValues (typeof (SCNGeometrySourceSemantics))) {
#endif
				if (!isValidEnumForPlatform (s))
					continue;
				d = SCNGeometrySource.FromData (new NSData (), s, 1, false, 1, 1, 1, 1);
			}
#pragma warning restore 0219
		}

		[Test]
		public void SCNGeometrySource_BoneStringTests () // These were radar://17782603
		{
			Asserts.EnsureYosemite ();

#pragma warning disable 0219
			SCNGeometrySource d = SCNGeometrySource.FromData (new NSData (), SCNGeometrySourceSemantic.BoneWeights, 1, false, 1, 1, 1, 1);
			d = SCNGeometrySource.FromData (new NSData (), SCNGeometrySourceSemantic.BoneIndices, 1, false, 1, 1, 1, 1);
#pragma warning restore 0219
		}
	}
}
#endif // __MACOS__
