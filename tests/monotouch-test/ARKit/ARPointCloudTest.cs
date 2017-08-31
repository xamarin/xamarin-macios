//
// Unit tests for ARPointCloud
//
// Authors:
//	Vincent Dondain <vidondai@microsoft.com>
//
// Copyright 2017 Microsoft. All rights reserved.
//

#if XAMCORE_2_0 && __IOS__

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ARKit;
using AVFoundation;
using Foundation;
using NUnit.Framework;
using ObjCRuntime;
using OpenTK;

namespace MonoTouchFixtures.ARKit {

	class ARPointCloudPoker : ARPointCloud {

		GCHandle vectorArrayHandle;
		GCHandle identifiersHandle;
		Vector4 [] vectorArray;
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
			vectorArray = new Vector4 [] { new Vector4 (1, 2, 3, -1), new Vector4 (4, 5, 6, -1) };
			if (!vectorArrayHandle.IsAllocated)
				vectorArrayHandle = GCHandle.Alloc (vectorArray, GCHandleType.Pinned);
			Vector3* addr = (Vector3*)vectorArrayHandle.AddrOfPinnedObject ();
			return (IntPtr)addr;
		}

		protected unsafe override IntPtr GetRawIdentifiers ()
		{
			identifiers = new ulong [] { 0, 1 };
			if (!identifiersHandle.IsAllocated)
				identifiersHandle = GCHandle.Alloc (identifiers, GCHandleType.Pinned);
			ulong* addr = (ulong*)identifiersHandle.AddrOfPinnedObject ();
			return (IntPtr)addr;
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
		}

		[Test]
		public void PointsTest ()
		{
			var cloud = new ARPointCloudPoker ();

			var points = cloud.Points;
			Assert.AreEqual (new Vector3 (1, 2, 3), cloud.Points [0]);
			Assert.AreEqual (new Vector3 (4, 5, 6), cloud.Points [1]);
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

#endif // XAMCORE_2_0 && __IOS__
