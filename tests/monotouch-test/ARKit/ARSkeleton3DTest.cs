//
// Unit tests for ARSkeleton3D
//
// Authors:
//	Vincent Dondain <vidondai@microsoft.com>
//
// Copyright 2019 Microsoft. All rights reserved.
//

#if __IOS__

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ARKit;
using Foundation;
using NUnit.Framework;
using ObjCRuntime;

using Matrix4 = global::OpenTK.NMatrix4;

namespace MonoTouchFixtures.ARKit {

	class ARSkeleton3DPoker : ARSkeleton3D {

		GCHandle modelTransformsArrayHandle;
		Matrix4 [] modelTransformsArray;
		GCHandle localTransformsArrayHandle;
		Matrix4 [] localTransformsArray;

		public ARSkeleton3DPoker () : base (IntPtr.Zero)
		{
		}

		public override nuint JointCount {
			get {
				return 2;
			}
		}

		protected unsafe override IntPtr RawJointModelTransforms
		{
			get {
				modelTransformsArray = new Matrix4 [] { Matrix4.Identity, Matrix4.Identity };
				if (!modelTransformsArrayHandle.IsAllocated)
					modelTransformsArrayHandle = GCHandle.Alloc (modelTransformsArray, GCHandleType.Pinned);
				return modelTransformsArrayHandle.AddrOfPinnedObject ();
			}
		}

		protected unsafe override IntPtr RawJointLocalTransforms
		{
			get {
				localTransformsArray = new Matrix4 [] { Matrix4.Identity, Matrix4.Identity };
				if (!localTransformsArrayHandle.IsAllocated)
					localTransformsArrayHandle = GCHandle.Alloc (localTransformsArray, GCHandleType.Pinned);
				return localTransformsArrayHandle.AddrOfPinnedObject ();
			}
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			if (modelTransformsArrayHandle.IsAllocated)
				modelTransformsArrayHandle.Free ();
			if (localTransformsArrayHandle.IsAllocated)
				localTransformsArrayHandle.Free ();
		}
	}

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ARSkeleton3DTest {

		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);
		}

		[Test]
		public void JointModelTransformsTest ()
		{
			var skeleton = new ARSkeleton3DPoker ();

			var landmarks = skeleton.JointModelTransforms;
			Assert.AreEqual (Matrix4.Identity, landmarks [0]);
			Assert.AreEqual (Matrix4.Identity, landmarks [1]);
		}

		[Test]
		public void JointLocalTransformsTest ()
		{
			var skeleton = new ARSkeleton3DPoker ();

			var landmarks = skeleton.JointLocalTransforms;
			Assert.AreEqual (Matrix4.Identity, landmarks [0]);
			Assert.AreEqual (Matrix4.Identity, landmarks [1]);
		}
	}
}

#endif // __IOS__
