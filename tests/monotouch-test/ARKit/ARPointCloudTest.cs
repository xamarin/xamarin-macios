//
// Unit tests for ARPointCloud
//
// Authors:
//	Vincent Dondain <vidondai@microsoft.com>
//
// Copyright 2017 Microsoft. All rights reserved.
//

#if HAS_ARKIT

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ARKit;
using AVFoundation;
using Foundation;
using NUnit.Framework;
using ObjCRuntime;
using Xamarin.Utils;

#if NET
using MatrixFloat2x2 = global::CoreGraphics.NMatrix2;
using MatrixFloat3x3 = global::CoreGraphics.NMatrix3;
using MatrixFloat4x4 = global::CoreGraphics.NMatrix4;
using VectorFloat3 = global::CoreGraphics.NVector3;
#else
using MatrixFloat2x2 = global::OpenTK.NMatrix2;
using MatrixFloat3x3 = global::OpenTK.NMatrix3;
using MatrixFloat4x4 = global::OpenTK.NMatrix4;
using VectorFloat3 = global::OpenTK.NVector3;
#endif

namespace MonoTouchFixtures.ARKit {

	class ARPointCloudPoker : ARPointCloud {

		GCHandle vectorArrayHandle;
		GCHandle identifiersHandle;
		VectorFloat3 [] vectorArray;
		ulong [] identifiers;

		public ARPointCloudPoker () : base (IntPtr.Zero)
		{
		}

		public override nuint Count {
			get {
				return 2;
			}
		}

		protected unsafe override IntPtr GetRawPoints ()
		{
			vectorArray = new VectorFloat3 [] { new VectorFloat3 (1, 2, 3), new VectorFloat3 (4, 5, 6) };
			if (!vectorArrayHandle.IsAllocated)
				vectorArrayHandle = GCHandle.Alloc (vectorArray, GCHandleType.Pinned);
			return vectorArrayHandle.AddrOfPinnedObject ();
		}

		protected unsafe override IntPtr GetRawIdentifiers ()
		{
			identifiers = new ulong [] { 0, 1 };
			if (!identifiersHandle.IsAllocated)
				identifiersHandle = GCHandle.Alloc (identifiers, GCHandleType.Pinned);
			ulong* addr = (ulong*) identifiersHandle.AddrOfPinnedObject ();
			return (IntPtr) addr;
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			if (vectorArrayHandle.IsAllocated)
				vectorArrayHandle.Free ();
			if (identifiersHandle.IsAllocated)
				identifiersHandle.Free ();
		}
	}

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ARPointCloudTest {

		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);
			// The API here was introduced to Mac Catalyst later than for the other frameworks, so we have this additional check
			TestRuntime.AssertSystemVersion (ApplePlatform.MacCatalyst, 14, 0, throwIfOtherPlatform: false);
		}

		[Test]
		public void PointsTest ()
		{
			var cloud = new ARPointCloudPoker ();

			var points = cloud.Points;
			Assert.AreEqual (new VectorFloat3 (1, 2, 3), cloud.Points [0]);
			Assert.AreEqual (new VectorFloat3 (4, 5, 6), cloud.Points [1]);
		}

		[Test]
		public void IdentifiersTest ()
		{
			var cloud = new ARPointCloudPoker ();

			var points = cloud.Identifiers;
			Assert.AreEqual (0, cloud.Identifiers [0]);
			Assert.AreEqual (1, cloud.Identifiers [1]);
		}
	}
}

#endif // HAS_ARKIT
