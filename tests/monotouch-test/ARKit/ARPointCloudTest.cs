//
// Unit tests for ARPointCloud
//
// Authors:
//	Vincent Dondain <vidondai@microsoft.com>
//
// Copyright 2017 Microsoft. All rights reserved.
//

#if XAMCORE_2_0 && !__TVOS__ && !__WATCHOS__ && !MONOMAC

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
			var va = new Vector3 [] { new Vector3 (1, 2, 3), new Vector3 (4, 5, 6) };
			var gch = GCHandle.Alloc (va, GCHandleType.Pinned);
			try {
				Vector3* addr = (Vector3*)gch.AddrOfPinnedObject ();
				return (IntPtr)addr;
			} finally {
				gch.Free ();
			}
		}

		protected unsafe override IntPtr GetRawIdentifiers ()
		{
			var identifiers = new ulong [] { 0, 1 };
			var gch = GCHandle.Alloc (identifiers, GCHandleType.Pinned);
			try {
				ulong* addr = (ulong*)gch.AddrOfPinnedObject ();
				return (IntPtr)addr;
			} finally {
				gch.Free ();
			}
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

#endif // XAMCORE_2_0 && !__TVOS__ && !__WATCHOS__ && !MONOMAC
